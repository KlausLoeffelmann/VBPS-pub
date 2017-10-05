' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports Microsoft.CodeAnalysis.PooledObjects

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    Partial Friend NotInheritable Class SynthesizedOnPropertyChangedMethodSymbol
        Inherits SynthesizedRegularMethodBase

        Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)
        Private ReadOnly _isOverrides As Boolean
        Private const METHOD_NAME As String = "OnPropertyChanged"

        Public Sub New(node As VisualBasicSyntaxNode, container As NamedTypeSymbol, isOverrides As Boolean)
            MyBase.New(node, container, METHOD_NAME)
            _isOverrides = isOverrides

            Dim decCompilation = DeclaringCompilation

            _parameters = ImmutableArray.Create(Of ParameterSymbol)(
                      New SynthesizedParameterSimpleSymbol(Me,
                                                           DeclaringCompilation.GetWellKnownType(WellKnownType.System_ComponentModel_PropertyChangedEventArgs),
                                                           0, "eArgs"))
        End Sub

        Friend Overrides Function GetBoundMethodBody(compilationState As TypeCompilationState, diagnostics As DiagnosticBag,
                                                     <Out> ByRef Optional methodBodyBinder As Binder = Nothing) As BoundBlock


            Dim meReference = New BoundMeReference(Syntax, Me.ContainingType)
            Dim meReferenceAsObject = New BoundMeReference(Syntax, Me.ContainingAssembly.GetSpecialType(SpecialType.System_Object))

            Dim propertyChangedEvent = Me.ContainingType.GetEventsToEmit().
                                       Where(Function(eventItem) eventItem.Name = "PropertyChanged").FirstOrDefault

            Dim propertyChangedEventAccess = New BoundEventAccess(Syntax, meReference, propertyChangedEvent, propertyChangedEvent.Type)
            Dim invokeMethod = DirectCast(propertyChangedEvent.Type.GetMembers.Where(Function(methodItem) methodItem.Name = "Invoke").FirstOrDefault, MethodSymbol)

            Dim receiver = New BoundFieldAccess(Syntax,
                            meReference,
                            propertyChangedEvent.AssociatedField,
                            False,
                            propertyChangedEvent.AssociatedField.Type).MakeCompilerGenerated

            Dim eArgsAccess = New BoundParameter(Syntax, Me.Parameters(0),
                                                 compilationState.Compilation.GetWellKnownType(WellKnownType.System_ComponentModel_PropertyChangedEventArgs)).MakeRValue

            Dim eventInfoCall = New BoundCall(Syntax,
                                      invokeMethod,
                                      Nothing,
                                      receiver,
                                      ImmutableArray.Create(Of BoundExpression)(meReferenceAsObject, eArgsAccess),
                                      Nothing,
                                      invokeMethod.ReturnType,
                                      suppressObjectClone:=True).MakeCompilerGenerated

            Dim raiseEventStatement = New BoundRaiseEventStatement(Syntax, propertyChangedEvent, eventInfoCall)

            'Dim block As BoundBlock = New BoundBlock(syntax,
            '                           Nothing,
            '                           ImmutableArray(Of LocalSymbol).Empty,
            '                           ImmutableArray.Create(Of BoundStatement)(raiseEventStatement))

            Dim F = New SyntheticBoundNodeFactory(Me, Me, Syntax, compilationState, diagnostics)

            Dim tempEventArgs As LocalSymbol = F.SynthesizedLocal(F.WellKnownType(WellKnownType.System_ComponentModel_PropertyChangedEventArgs))

            Dim block = F.Block(ImmutableArray.Create(Of LocalSymbol)(tempEventArgs),
                    F.Assignment(F.Local(tempEventArgs, True), eArgsAccess))
            block = block.MakeCompilerGenerated
            Return block

        End Function

        Public Overrides ReadOnly Property IsOverrides As Boolean
            Get
                Return _isOverrides
            End Get
        End Property

        Public Overrides ReadOnly Property IsOverridable As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property IsNotOverridable As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property IsOverloads As Boolean
            Get
                Return False
            End Get
        End Property

        Friend Overrides ReadOnly Property ParameterCount As Integer
            Get
                Return 1
            End Get
        End Property

        Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
            Get
                Return Me._parameters
            End Get
        End Property

        Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
            Get
                Return Accessibility.ProtectedAndInternal
            End Get
        End Property

        Public Overrides ReadOnly Property IsSub As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property ReturnType As TypeSymbol
            Get
                Return ContainingAssembly.GetSpecialType(SpecialType.System_Void)
            End Get
        End Property

        Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
            Get
                Return False
            End Get
        End Property

        Friend Overrides Function CalculateLocalSyntaxOffset(localPosition As Integer, localTree As SyntaxTree) As Integer
            Throw ExceptionUtilities.Unreachable
        End Function
    End Class
End Namespace
