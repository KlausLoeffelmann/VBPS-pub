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
  // Code size       60 (0x3c)
  .maxstack  3
  .locals init (System.EventHandler V_0) //del
  IL_0000:  ldsfld     "MyClass1._Closure$__.$I0-0 As System.EventHandler"
  IL_0005:  brfalse.s  IL_000e
  IL_0007:  ldsfld     "MyClass1._Closure$__.$I0-0 As System.EventHandler"
  IL_000c:  br.s       IL_0024
  IL_000e:  ldsfld     "MyClass1._Closure$__.$I As MyClass1._Closure$__"
  IL_0013:  ldftn      "Sub MyClass1._Closure$__._Lambda$__0-0(Object, System.EventArgs)"
  IL_0019:  newobj     "Sub System.EventHandler..ctor(Object, System.IntPtr)"
  IL_001e:  dup
  IL_001f:  stsfld     "MyClass1._Closure$__.$I0-0 As System.EventHandler"
  IL_0024:  stloc.0
  IL_0025:  ldstr      "qq"
  IL_002a:  call       "Function System.AppDomain.CreateDomain(String) As System.AppDomain"
  IL_002f:  dup
  IL_0030:  ldloc.0
  IL_0031:  callvirt   "Sub System.AppDomain.add_DomainUnload(System.EventHandler)"
  IL_0036:  call       "Sub System.AppDomain.Unload(System.AppDomain)"
  IL_003b:  ret
}
]]>)
        End Sub
    End Class
End Namespace
