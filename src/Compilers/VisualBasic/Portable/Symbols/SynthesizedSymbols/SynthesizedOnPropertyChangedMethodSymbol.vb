' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports Microsoft.CodeAnalysis.PooledObjects

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    Friend NotInheritable Class SynthesizedOnPropertyChangedMethodSymbol
        Inherits SynthesizedRegularMethodBase

        Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)
        Private ReadOnly _isOverrides As Boolean
        Private Const MethodName As String = "OnPropertyChanged"

        Public Sub New(node As VisualBasicSyntaxNode, container As NamedTypeSymbol)
            MyBase.New(node, container, MethodName)

            _isOverrides = False

            _parameters = ImmutableArray.Create(Of ParameterSymbol)(
                      New SynthesizedParameterSimpleSymbol(Me,
                                                           MyBase.DeclaringCompilation.GetWellKnownType(WellKnownType.System_ComponentModel_PropertyChangedEventArgs),
                                                           0,
                                                           "eArgs"))
        End Sub

        Public Shared Function IsPropertyChangedEvent(symbol As Symbol, compilation As VisualBasicCompilation) As Boolean
            If symbol.Kind <> SymbolKind.Event Then
                Return False
            End If

            Dim eventSymbol As EventSymbol = DirectCast(symbol, EventSymbol)
            Dim parameters As ImmutableArray(Of ParameterSymbol) = eventSymbol.DelegateParameters
            If parameters.Count <> 2 Then
                Return False
            End If

            If parameters(0).Type.SpecialType <> SpecialType.System_Object Then
                Return False
            End If

            Dim eventArgsType As TypeSymbol = compilation.GetWellKnownType(WellKnownType.System_ComponentModel_PropertyChangedEventArgs)
            Return parameters(1).Type.Equals(eventArgsType)
        End Function

        Friend Overrides Function GetBoundMethodBody(compilationState As TypeCompilationState,
                                                     diagnostics As DiagnosticBag,
                                                     <Out> ByRef Optional methodBodyBinder As Binder = Nothing) As BoundBlock

            Dim propertyChangedEvent As EventSymbol = DirectCast(ContainingType.GetMembers("PropertyChanged").Where(Function(symbol) IsPropertyChangedEvent(symbol, DeclaringCompilation)).FirstOrDefault, EventSymbol)

            If propertyChangedEvent Is Nothing Then
                ' If there is no PropertyChanged event, return an empty block so that the
                ' output will be well formed.
                Return New BoundBlock(Syntax,
                                      Nothing,
                                      ImmutableArray.Create(Of LocalSymbol),
                                      ImmutableArray.Create(Of BoundStatement)(
                                          New BoundReturnStatement(Syntax, Nothing, Nothing, Nothing)))

            End If

            Debug.Assert(propertyChangedEvent IsNot Nothing)

            Dim associatedFieldReference =
                New BoundFieldAccess(Syntax,
                                     New BoundMeReference(Syntax, Me.ContainingType),
                                     propertyChangedEvent.AssociatedField,
                                     False,
                                     propertyChangedEvent.AssociatedField.Type).MakeCompilerGenerated

            Dim eArgsAccess = New BoundParameter(Syntax,
                                                 Me.Parameters(0),
                                                 compilationState.Compilation.GetWellKnownType(WellKnownType.System_ComponentModel_PropertyChangedEventArgs)).MakeRValue

            Dim fieldReferenceTemporary As LocalSymbol = New SynthesizedLocal(Me, associatedFieldReference.Type, SynthesizedLocalKind.LoweringTemp)
            Dim tempAccess = Function() New BoundLocal(Syntax, fieldReferenceTemporary, fieldReferenceTemporary.Type).MakeCompilerGenerated

            Dim tempInit = New BoundExpressionStatement(Syntax,
                               New BoundAssignmentOperator(Syntax, tempAccess(), associatedFieldReference, True, associatedFieldReference.Type)).MakeCompilerGenerated

            Dim meReferenceAsObject = New BoundMeReference(Syntax, Me.ContainingAssembly.GetSpecialType(SpecialType.System_Object))
            Dim invokeMethod = DirectCast(propertyChangedEvent.Type.GetMembers("Invoke").FirstOrDefault, MethodSymbol)
            Dim eventInfoCall = New BoundCall(Syntax,
                                              invokeMethod,
                                              Nothing,
                                              tempAccess(),
                                              ImmutableArray.Create(Of BoundExpression)(meReferenceAsObject, eArgsAccess),
                                              Nothing,
                                              invokeMethod.ReturnType,
                                              suppressObjectClone:=True)

            Dim invokeStatement = New BoundExpressionStatement(Syntax, eventInfoCall)

            Dim condition = New BoundBinaryOperator(Syntax,
                                                    BinaryOperatorKind.Is,
                                                    tempAccess().MakeRValue(),
                                                    New BoundLiteral(Syntax,
                                                                     ConstantValue.Nothing,
                                                                     compilationState.Compilation.GetSpecialType(SpecialType.System_Object)),
                                                    False,
                                                    compilationState.Compilation.GetSpecialType(SpecialType.System_Boolean)).MakeCompilerGenerated

            Dim skipEventRaise As New GeneratedLabelSymbol("skipEventRaise")

            Dim ifNullSkip = New BoundConditionalGoto(Syntax, condition, True, skipEventRaise).MakeCompilerGenerated

            Dim block = New BoundBlock(Syntax,
                                       Nothing,
                                       ImmutableArray.Create(fieldReferenceTemporary),
                                       ImmutableArray.Create(Of BoundStatement)(
                                           tempInit,
                                           ifNullSkip,
                                           invokeStatement,
                                           New BoundLabelStatement(Syntax, skipEventRaise),
                                           New BoundReturnStatement(Syntax, Nothing, Nothing, Nothing)))
            Return block
        End Function

        Friend Overrides Sub SetMetadataName(metadataName As String)
            ' Nothing to do here.
        End Sub

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
                Return Accessibility.Protected
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
