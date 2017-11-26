Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests
    Public Class CodeGenUserInterfaceProperties
        Inherits BasicTestBase

        <Fact()>
        Public Sub SimpleUserInterfaceProperty()
            CompileAndVerify(
    <compilation>
        <file name="a.vb">
Imports System.ComponentModel

Public Class UserInterfaceOnProperty
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public UserInterface Property FooProp As String
End Class
        </file>
    </compilation>).
                VerifyIL("UserInterfaceOnProperty.set_FooProp",
            <![CDATA[
{
  // Code size       49 (0x31)
  .maxstack  3
  .locals init (System.ComponentModel.PropertyChangedEventHandler V_0)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      "UserInterfaceOnProperty._FooProp As String"
  IL_0006:  ldarg.1
  IL_0007:  call       "Function Object.Equals(Object, Object) As Boolean"
  IL_000c:  brtrue.s   IL_0030
  IL_000e:  ldarg.0
  IL_000f:  ldarg.1
  IL_0010:  stfld      "UserInterfaceOnProperty._FooProp As String"
  IL_0015:  ldarg.0
  IL_0016:  ldfld      "UserInterfaceOnProperty.PropertyChangedEvent As System.ComponentModel.PropertyChangedEventHandler"
  IL_001b:  stloc.0
  IL_001c:  ldloc.0
  IL_001d:  brfalse.s  IL_0030
  IL_001f:  ldloc.0
  IL_0020:  ldarg.0
  IL_0021:  ldstr      "FooProp"
  IL_0026:  newobj     "Sub System.ComponentModel.PropertyChangedEventArgs..ctor(String)"
  IL_002b:  callvirt   "Sub System.ComponentModel.PropertyChangedEventHandler.Invoke(Object, System.ComponentModel.PropertyChangedEventArgs)"
  IL_0030:  ret
}
]]>)
        End Sub

        <Fact()>
        Public Sub SimpleUserInterfacePropertyUsingOnNotifyChanged()
            CompileAndVerify(
    <compilation>
        <file name="a.vb">
Imports System.ComponentModel

Public Class UserInterfaceOnProperty
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    
    Protected Overridable Sub OnPropertyChanged(eArgs as PropertyChangedEventArgs)
        RaiseEvent PropertyChanged(me,eArgs)
    End Sub

    Public UserInterface Property FooProp As String
End Class
        </file>
    </compilation>).
                VerifyIL("UserInterfaceOnProperty.set_FooProp",
            <![CDATA[
{
  // Code size       38 (0x26)
  .maxstack  2
  IL_0000:  ldarg.0
  IL_0001:  ldfld      "UserInterfaceOnProperty._FooProp As String"
  IL_0006:  ldarg.1
  IL_0007:  call       "Function Object.Equals(Object, Object) As Boolean"
  IL_000c:  brtrue.s   IL_0025
  IL_000e:  ldarg.0
  IL_000f:  ldarg.1
  IL_0010:  stfld      "UserInterfaceOnProperty._FooProp As String"
  IL_0015:  ldarg.0
  IL_0016:  ldstr      "FooProp"
  IL_001b:  newobj     "Sub System.ComponentModel.PropertyChangedEventArgs..ctor(String)"
  IL_0020:  callvirt   "Sub UserInterfaceOnProperty.OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs)"
  IL_0025:  ret
}
]]>)
        End Sub

    End Class
End Namespace
