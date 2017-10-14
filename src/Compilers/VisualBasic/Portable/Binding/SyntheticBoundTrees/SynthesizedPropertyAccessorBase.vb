' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports Microsoft.CodeAnalysis.PooledObjects

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    Friend Module SynthesizedPropertyAccessorHelper

        Enum UserInterfaceUsageScope
            None
            [Property]
            [Type]
        End Enum

        Private onPropertyChangedSubSyncer As New Object

        Friend Function GetBoundMethodBody(accessor As MethodSymbol,
                                           backingField As FieldSymbol,
                                           compilationState As TypeCompilationState,
                                           diagnostics As DiagnosticBag,
                                           Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock

            methodBodyBinder = Nothing

            ' NOTE: Current implementation of this method does generate the code for both getter and setter,
            '       Ideally it could have been split into two different implementations, but the code gen is
            '       quite similar in these two cases and current types hierarchy makes this solution preferable

            'IMPLEMENTING AutoProperties with INotifyPropertyChanged.
            Dim propertySymbol = DirectCast(accessor.AssociatedSymbol, PropertySymbol)
            Dim containingType = accessor.ContainingType

            Dim userInterfaceScope = UserInterfaceUsageScope.None

            If accessor.MethodKind = MethodKind.PropertySet Then

                '1st Requirement: Containing Type implements INotifyPropertyChanged or inherits from type which has it implemented.
                Dim siteDiagnostics As New HashSet(Of DiagnosticInfo)

                Dim isImplementingINotifyPropertyChanged = compilationState.Compilation.GetWellKnownType(
                                        WellKnownType.System_ComponentModel_INotifyPropertyChanged).IsBaseTypeOrInterfaceOf(containingType, siteDiagnostics)

                '2nd Requirement: Must have Attribute defined.
                If isImplementingINotifyPropertyChanged Then

                    Dim propertyAttributes = propertySymbol.GetAttributes().
                                        Where(Function(attributeItem) attributeItem?.AttributeClass?.Name = "UserInterfaceAttribute" AndAlso
                                                                      attributeItem?.AttributeClass?.ContainingNamespace?.Name = "CompilerServices").FirstOrDefault()

                    If propertyAttributes IsNot Nothing Then
                        If propertyAttributes.NamedArguments.Count = 0 Then
                            userInterfaceScope = UserInterfaceUsageScope.Property
                        Else
                            Debug.Assert(propertyAttributes.NamedArguments(0).Key = "Use")
                            userInterfaceScope = If(CBool(propertyAttributes.NamedArguments(0).Value.Value),
                                                                  UserInterfaceUsageScope.Property,
                                                                  UserInterfaceUsageScope.None)
                        End If
                    Else
                        'Might also be defined on Class level.
                        Dim typeAttributes = containingType.GetAttributes().
                                            Where(Function(attributeItem) attributeItem.AttributeClass.Name = "UserInterfaceAttribute" AndAlso
                                                                          attributeItem.AttributeClass.ContainingNamespace.Name = "CompilerServices").FirstOrDefault()
                        If typeAttributes IsNot Nothing Then
                            If typeAttributes.NamedArguments.Count = 0 Then
                                userInterfaceScope = UserInterfaceUsageScope.Type
                            Else
                                Debug.Assert(typeAttributes.NamedArguments(0).Key = "Use")
                                userInterfaceScope = If(CBool(typeAttributes.NamedArguments(0).Value.Value),
                                                                  UserInterfaceUsageScope.Type,
                                                                  UserInterfaceUsageScope.None)
                            End If
                        End If
                    End If
                End If
            End If

            Dim syntax = DirectCast(VisualBasic.VisualBasicSyntaxTree.Dummy.GetRoot(), VisualBasicSyntaxNode)
            Dim meSymbol As ParameterSymbol = Nothing
            Dim meReference As BoundExpression = Nothing
            Dim meReferenceAsObject As BoundExpression = Nothing

            If Not accessor.IsShared Then
                meSymbol = accessor.MeParameter
                meReference = New BoundMeReference(syntax, meSymbol.Type)
                meReferenceAsObject = New BoundMeReference(syntax,
                                                           accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object))
            End If

            Dim isOverride As Boolean = propertySymbol.IsWithEvents AndAlso propertySymbol.IsOverrides

            Dim field As FieldSymbol = Nothing
            Dim fieldAccess As BoundFieldAccess = Nothing
            Dim fieldAccessAsObject As BoundFieldAccess = Nothing

            Dim myBaseReference As BoundExpression = Nothing
            Dim baseGet As BoundExpression = Nothing

            If isOverride Then
                ' overriding property gets its value via a base call
                myBaseReference = New BoundMyBaseReference(syntax, meSymbol.Type)
                Dim baseGetSym = propertySymbol.GetMethod.OverriddenMethod

                baseGet = New BoundCall(
                    syntax,
                    baseGetSym,
                    Nothing,
                    myBaseReference,
                    ImmutableArray(Of BoundExpression).Empty,
                    Nothing,
                    type:=baseGetSym.ReturnType,
                    suppressObjectClone:=True)
            Else
                ' not overriding property operates with field
                field = backingField
                fieldAccess = New BoundFieldAccess(syntax, meReference, field, True, field.Type)
                fieldAccessAsObject = New BoundFieldAccess(syntax, meReference, field, True,
                                                           accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object)).MakeRValue
            End If

            Dim exitLabel = New GeneratedLabelSymbol("exit")

            Dim statements As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance
            Dim locals As ImmutableArray(Of LocalSymbol)
            Dim returnLocal As BoundLocal

            If accessor.MethodKind = MethodKind.PropertyGet Then
                ' Declare local variable for function return.
                Dim local = New SynthesizedLocal(accessor, accessor.ReturnType, SynthesizedLocalKind.LoweringTemp)

                Dim returnValue As BoundExpression
                If isOverride Then
                    returnValue = baseGet
                Else
                    returnValue = fieldAccess.MakeRValue()
                End If

                statements.Add(New BoundReturnStatement(syntax, returnValue, local, exitLabel).MakeCompilerGenerated())

                locals = ImmutableArray.Create(Of LocalSymbol)(local)
                returnLocal = New BoundLocal(syntax, local, isLValue:=False, type:=local.Type)
            Else
                Debug.Assert(accessor.MethodKind = MethodKind.PropertySet)

                ' NOTE: at this point number of parameters in a VALID property must be 1.
                '       In the case when an auto-property has some parameters we assume that 
                '       ERR_AutoPropertyCantHaveParams(36759) is already generated,
                '       in this case we just ignore all the parameters and assume that the 
                '       last parameter is what we need to use below
                Debug.Assert(accessor.ParameterCount >= 1)
                Dim parameter = accessor.Parameters(accessor.ParameterCount - 1)
                Dim parameterAccess = New BoundParameter(syntax, parameter, isLValue:=False, type:=parameter.Type)
                Dim parameterAccessAsObject = New BoundParameter(syntax, parameter, isLValue:=False,
                                                                 type:=accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object))

                Dim eventsToHookup As ArrayBuilder(Of ValueTuple(Of EventSymbol, PropertySymbol)) = Nothing

                ' contains temps for handler delegates followed by other stuff that is needed.
                ' so it will have at least eventsToHookup.Count temps
                Dim temps As ArrayBuilder(Of LocalSymbol) = Nothing
                ' accesses to the handler delegates
                ' we use them once to unhook from old source and then again to hook to the new source
                Dim handlerlocalAccesses As ArrayBuilder(Of BoundLocal) = Nothing

                ' //process Handles that need to be hooked up in this method
                ' //if there are events to hook up, the body will look like this:
                '
                ' Dim tempHandlerLocal = AddressOf handlerMethod   ' addressOf is already bound and may contain conversion
                ' . . .
                ' Dim tempHandlerLocalN = AddressOf handlerMethodN   
                '
                ' Dim valueTemp = [ _backingField | BaseGet ] 
                ' If valueTemp isnot nothing
                '
                '       // unhook handlers from the old value. 
                '       // Note that we can use the handler temps we have just created. 
                '       // Delegate identity is {target, method} so that will work
                '
                '       valueTemp.E1.Remove(tempLocalHandler1)
                '       valueTemp.E2.Remove(tempLocalHandler2)
                '
                ' End If
                '
                ' //Now store the new value
                '
                ' [ _backingField = value | BaseSet(value) ]
                ' 
                ' // re-read the value (we use same assignment here as before)
                ' valueTemp = [ _backingField | BaseGet ] 
                '
                ' If valueTemp isnot nothing
                '
                '       // re-hook handlers to the new value. 
                '
                '       valueTemp.E1.Add(tempLocalHandler1)
                '       valueTemp.E2.Add(tempLocalHandler2)
                '
                ' End If
                '
                If propertySymbol.IsWithEvents Then
                    For Each member In accessor.ContainingType.GetMembers()
                        If member.Kind = SymbolKind.Method Then
                            Dim methodMember = DirectCast(member, MethodSymbol)

                            Dim handledEvents = methodMember.HandledEvents

                            ' if method has definition and implementation parts
                            ' their "Handles" should be merged.
                            If methodMember.IsPartial Then
                                Dim implementationPart = methodMember.PartialImplementationPart
                                If implementationPart IsNot Nothing Then
                                    handledEvents = handledEvents.Concat(implementationPart.HandledEvents)
                                Else
                                    ' partial methods with no implementation do not handle anything
                                    Continue For
                                End If
                            End If

                            If Not handledEvents.IsEmpty Then
                                For Each handledEvent In handledEvents
                                    If handledEvent.hookupMethod = accessor Then
                                        If eventsToHookup Is Nothing Then
                                            eventsToHookup = ArrayBuilder(Of ValueTuple(Of EventSymbol, PropertySymbol)).GetInstance
                                            temps = ArrayBuilder(Of LocalSymbol).GetInstance
                                            handlerlocalAccesses = ArrayBuilder(Of BoundLocal).GetInstance
                                        End If

                                        eventsToHookup.Add(New ValueTuple(Of EventSymbol, PropertySymbol)(
                                                           DirectCast(handledEvent.EventSymbol, EventSymbol),
                                                           DirectCast(handledEvent.WithEventsSourceProperty, PropertySymbol)))
                                        Dim handlerLocal = New SynthesizedLocal(accessor, handledEvent.delegateCreation.Type, SynthesizedLocalKind.LoweringTemp)
                                        temps.Add(handlerLocal)

                                        Dim localAccess = New BoundLocal(syntax, handlerLocal, handlerLocal.Type)
                                        handlerlocalAccesses.Add(localAccess.MakeRValue())

                                        Dim handlerLocalinit = New BoundExpressionStatement(
                                                               syntax,
                                                               New BoundAssignmentOperator(
                                                                   syntax,
                                                                   localAccess,
                                                                   handledEvent.delegateCreation,
                                                                   False,
                                                                   localAccess.Type))

                                        statements.Add(handlerLocalinit)

                                    End If
                                Next
                            End If
                        End If
                    Next
                End If

                Dim withEventsLocalAccess As BoundLocal = Nothing
                Dim withEventsLocalStore As BoundExpressionStatement = Nothing

                ' need to unhook old handlers before setting a new event source
                If eventsToHookup IsNot Nothing Then
                    Dim withEventsValue As BoundExpression
                    If isOverride Then
                        withEventsValue = baseGet
                    Else
                        withEventsValue = fieldAccess.MakeRValue()
                    End If

                    Dim withEventsLocal = New SynthesizedLocal(accessor, withEventsValue.Type, SynthesizedLocalKind.LoweringTemp)
                    temps.Add(withEventsLocal)

                    withEventsLocalAccess = New BoundLocal(syntax, withEventsLocal, withEventsLocal.Type)
                    withEventsLocalStore = New BoundExpressionStatement(
                        syntax,
                        New BoundAssignmentOperator(
                            syntax,
                            withEventsLocalAccess,
                            withEventsValue,
                            True,
                            withEventsLocal.Type))

                    statements.Add(withEventsLocalStore)

                    ' if witheventsLocalStore isnot nothing
                    '           ...
                    '           withEventsLocalAccess.eventN_remove(handlerLocalN)
                    '           ...
                    Dim eventRemovals = ArrayBuilder(Of BoundStatement).GetInstance
                    For i As Integer = 0 To eventsToHookup.Count - 1
                        Dim eventSymbol As EventSymbol = eventsToHookup(i).Item1
                        ' Normally, we would synthesize lowered bound nodes, but we know that these nodes will
                        ' be run through the LocalRewriter.  Let the LocalRewriter handle the special code for
                        ' WinRT events.
                        Dim withEventsProviderAccess As BoundExpression = withEventsLocalAccess

                        Dim providerProperty = eventsToHookup(i).Item2
                        If providerProperty IsNot Nothing Then
                            withEventsProviderAccess = New BoundPropertyAccess(syntax,
                                                                               providerProperty,
                                                                               Nothing,
                                                                               PropertyAccessKind.Get,
                                                                               False,
                                                                               If(providerProperty.IsShared, Nothing, withEventsLocalAccess),
                                                                               ImmutableArray(Of BoundExpression).Empty)
                        End If

                        eventRemovals.Add(
                            New BoundRemoveHandlerStatement(
                                syntax:=syntax,
                                eventAccess:=New BoundEventAccess(syntax, withEventsProviderAccess, eventSymbol, eventSymbol.Type),
                                handler:=handlerlocalAccesses(i)))
                    Next

                    Dim removalStatement = New BoundStatementList(syntax, eventRemovals.ToImmutableAndFree)

                    Dim conditionalRemoval = New BoundIfStatement(
                                             syntax,
                                             (New BoundBinaryOperator(
                                                 syntax,
                                                 BinaryOperatorKind.IsNot,
                                                 withEventsLocalAccess.MakeRValue(),
                                                 New BoundLiteral(syntax, ConstantValue.Nothing,
                                                                  accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object)),
                                                 False,
                                                 accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Boolean))).MakeCompilerGenerated,
                                             removalStatement,
                                             Nothing)

                    statements.Add(conditionalRemoval.MakeCompilerGenerated)
                End If

                ' set the value of the property
                ' if it is overriding, call the base
                ' otherwise assign to associated field.
                Dim valueSettingExpression As BoundExpression
                If isOverride Then
                    Dim baseSet = accessor.OverriddenMethod
                    valueSettingExpression = New BoundCall(
                        syntax,
                        baseSet,
                        Nothing,
                        myBaseReference,
                        ImmutableArray.Create(Of BoundExpression)(parameterAccess),
                        Nothing,
                        suppressObjectClone:=True,
                        type:=baseSet.ReturnType)
                Else
                    valueSettingExpression = New BoundAssignmentOperator(
                        syntax,
                        fieldAccess,
                        parameterAccess,
                        suppressObjectClone:=False,
                        type:=propertySymbol.Type)
                End If

                'We need to generate
                'If Not Object.Equals(field,value) Then
                '   field = value
                '   RaiseEvent Me.INotifyPropertyChange(Me, New PropertyChangeEventArgs("PropertyName"))
                'End If
                If userInterfaceScope <> UserInterfaceUsageScope.None Then

                    Try
                        'Let's find out, if this type already has the Event Raiser Method OnPropertyChanged.
                        Dim onPropertyChangedMethod = accessor.ContainingType.GetMethodsToEmit().Where(Function(eventItem) eventItem.Name = "OnPropertyChanged").FirstOrDefault

                        If onPropertyChangedMethod IsNot Nothing Then
                            'Check for correct Signature.
                        Else
                            If compilationState.HasSynthesizedMethods Then
                                onPropertyChangedMethod = DirectCast(compilationState.SynthesizedMethods.Where(Function(item) item.Method.Name = "OnPropertyChanged").FirstOrDefault.Method, MethodSymbol)
                            End If
                        End If

                        If onPropertyChangedMethod Is Nothing Then
                            'Let's find out, if the method is defined by a base class.
                            Dim members = accessor.ContainingType.GetMembersFromTypeAndAllBaseTypes("OnPropertyChanged",
                                                                                                  compilationState.Compilation)
                            onPropertyChangedMethod = TryCast(members.FirstOrDefault?.Item2, MethodSymbol)
                        End If

                        Dim objectEqualsMethod = DirectCast(accessor.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Object__EqualsObjectObject),
                                                                MethodSymbol)

                        Dim objectEqualConditionalExpression = New BoundCall(syntax,
                                                                                 objectEqualsMethod,
                                                                                 Nothing,
                                                                                 New BoundTypeExpression(syntax, accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object)),
                                                                                 ImmutableArray.Create(Of BoundExpression)(fieldAccessAsObject, parameterAccessAsObject),
                                                                                 Nothing,
                                                                                 objectEqualsMethod.ReturnType,
                                                                                 suppressObjectClone:=True)

                        Dim notObjectEqualConditionalExpression As New BoundUnaryOperator(syntax,
                                                                                              UnaryOperatorKind.Not,
                                                                                              objectEqualConditionalExpression,
                                                                                              True,
                                                                                              accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Boolean))

                        Dim newEventArgsExpression = New BoundObjectCreationExpression(syntax,
                                                                                               DirectCast(compilationState.Compilation.GetWellKnownTypeMember(WellKnownMember.System_ComponentModel_PropertyChangedEventArgs__ctor), MethodSymbol),
                                                                                               ImmutableArray.Create(Of BoundExpression)({New BoundLiteral(syntax, ConstantValue.Create(accessor.AssociatedSymbol.Name),
                                                                                                                                                          accessor.ContainingAssembly.GetSpecialType(SpecialType.System_String))}),
                                                                                               Nothing,
                                                                                               compilationState.Compilation.GetWellKnownType(WellKnownType.System_ComponentModel_PropertyChangedEventArgs))

                        Dim consequenceStatementList = ArrayBuilder(Of BoundStatement).GetInstance
                        consequenceStatementList.Add(New BoundExpressionStatement(syntax, valueSettingExpression).MakeCompilerGenerated())

                        If onPropertyChangedMethod IsNot Nothing Then
                            'We are raising PropertyChanged via OnPropertyChanged.

                            Dim OnPropertyChangedCall = New BoundCall(syntax,
                                                                          onPropertyChangedMethod,
                                                                          Nothing,
                                                                          meReference,
                                                                          ImmutableArray.Create(Of BoundExpression)(newEventArgsExpression),
                                                                          Nothing,
                                                                          onPropertyChangedMethod.ReturnType,
                                                                          suppressObjectClone:=True)

                            Dim OnPropertyChangedInvocation = New BoundExpressionStatement(syntax, OnPropertyChangedCall)
                            consequenceStatementList.Add(OnPropertyChangedInvocation)
                        Else
                            'We are raising PropertyChanged directly via RaiseEvent, which will get lowered later.
                            Dim propertyChangedEvent = accessor.ContainingType.GetEventsToEmit().
                                                    Where(Function(eventItem) eventItem.Name = "PropertyChanged").FirstOrDefault

                            Dim propertyChangedEventAccess = New BoundEventAccess(syntax, meReference, propertyChangedEvent, propertyChangedEvent.Type)

                            Dim invokeMethod = DirectCast(propertyChangedEvent.Type.GetMembers.Where(Function(methodItem) methodItem.Name = "Invoke").FirstOrDefault, MethodSymbol)

                            Dim receiver = New BoundFieldAccess(syntax,
                                                meReference,
                                                propertyChangedEvent.AssociatedField,
                                                False,
                                                propertyChangedEvent.AssociatedField.Type).MakeCompilerGenerated

                            Dim eventInfoCall = New BoundCall(syntax,
                                                          invokeMethod,
                                                          Nothing,
                                                          receiver,
                                                          ImmutableArray.Create(Of BoundExpression)(meReferenceAsObject, newEventArgsExpression),
                                                          Nothing,
                                                          invokeMethod.ReturnType,
                                                          suppressObjectClone:=True).MakeCompilerGenerated

                            Dim rEventStatement = New BoundRaiseEventStatement(syntax, propertyChangedEvent, eventInfoCall)

                            consequenceStatementList.Add(rEventStatement)

                        End If
                        Dim objectsEqualityTest = New BoundIfStatement(
                                             syntax,
                                             notObjectEqualConditionalExpression.MakeCompilerGenerated,
                                             New BoundStatementList(syntax, consequenceStatementList.ToImmutableAndFree),
                                             Nothing)

                        statements.Add(objectsEqualityTest.MakeCompilerGenerated)
                    Catch ex As Exception
                        If Debugger.IsAttached Then
                            Debugger.Break()
                        End If
                    End Try
                Else
                    'memberValue = value
                    statements.Add(
                    (New BoundExpressionStatement(
                        syntax,
                        valueSettingExpression).MakeCompilerGenerated()))
                End If

                ' after setting new event source, hookup handlers
                If eventsToHookup IsNot Nothing Then
                    statements.Add(withEventsLocalStore)

                    ' if witheventsLocalStore isnot nothing
                    '           ...
                    '           withEventsLocalAccess.eventN_add(handlerLocalN)
                    '           ...
                    Dim eventAdds = ArrayBuilder(Of BoundStatement).GetInstance
                    For i As Integer = 0 To eventsToHookup.Count - 1
                        Dim eventSymbol As EventSymbol = eventsToHookup(i).Item1
                        ' Normally, we would synthesize lowered bound nodes, but we know that these nodes will
                        ' be run through the LocalRewriter.  Let the LocalRewriter handle the special code for
                        ' WinRT events.
                        Dim withEventsProviderAccess As BoundExpression = withEventsLocalAccess
                        Dim providerProperty = eventsToHookup(i).Item2
                        If providerProperty IsNot Nothing Then
                            withEventsProviderAccess = New BoundPropertyAccess(syntax,
                                                                               providerProperty,
                                                                               Nothing,
                                                                               PropertyAccessKind.Get,
                                                                               False,
                                                                               If(providerProperty.IsShared, Nothing, withEventsLocalAccess),
                                                                               ImmutableArray(Of BoundExpression).Empty)
                        End If

                        eventAdds.Add(
                            New BoundAddHandlerStatement(
                                syntax:=syntax,
                                eventAccess:=New BoundEventAccess(syntax, withEventsProviderAccess, eventSymbol, eventSymbol.Type),
                                handler:=handlerlocalAccesses(i)))
                    Next

                    Dim addStatement = New BoundStatementList(syntax, eventAdds.ToImmutableAndFree())

                    Dim conditionalAdd = New BoundIfStatement(
                                             syntax,
                                             (New BoundBinaryOperator(
                                                 syntax,
                                                 BinaryOperatorKind.IsNot,
                                                 withEventsLocalAccess.MakeRValue(),
                                                 New BoundLiteral(syntax, ConstantValue.Nothing,
                                                                  accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object)),
                                                 False,
                                                 accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Boolean))).MakeCompilerGenerated,
                                             addStatement,
                                             Nothing)

                    statements.Add(conditionalAdd.MakeCompilerGenerated)
                End If

                locals = If(temps Is Nothing, ImmutableArray(Of LocalSymbol).Empty, temps.ToImmutableAndFree)
                returnLocal = Nothing

                If eventsToHookup IsNot Nothing Then
                    eventsToHookup.Free()
                    handlerlocalAccesses.Free()
                End If
            End If

            statements.Add((New BoundLabelStatement(syntax, exitLabel)).MakeCompilerGenerated())
            statements.Add((New BoundReturnStatement(syntax, returnLocal, Nothing, Nothing)).MakeCompilerGenerated())

            Return (New BoundBlock(syntax, Nothing, locals, statements.ToImmutableAndFree())).MakeCompilerGenerated()
        End Function

    End Module

End Namespace
