using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: DisableRuntimeMarshalling]

namespace Snap.WinUI.FrameworkTheming;

public static unsafe partial class FrameworkTheming
{
    private static readonly nint mux = GetModuleHandleW("microsoft.ui.xaml.dll");

    // DXamlInstanceStorage::GetValue(void** phValue)
    // _Check_return_ HRESULT GetValue(_Outptr_result_maybenull_ Handle* phValue)
    private static readonly delegate* unmanaged[Stdcall]<DXamlCoreAbi**, int> pDXamlInstanceStorageGetValue = (delegate* unmanaged[Stdcall]<DXamlCoreAbi**, int>)(mux + 0x5EE96C);

    // FrameworkTheming::OnThemeChanged(FrameworkTheming* this, bool forceUpdate)
    // _Check_return_ HRESULT OnThemeChanged(bool forceUpdate = false)
    private static readonly delegate* unmanaged[Stdcall]<FrameworkThemingAbi*, bool, int> pFrameworkThemingOnThemeChanged = (delegate* unmanaged[Stdcall]<FrameworkThemingAbi*, bool, int>)(mux + 0x8D5040);

    public static void SetTheme(Theme theme)
    {
        DXamlCoreAbi* pXamlCore = default;
        pDXamlInstanceStorageGetValue(&pXamlCore);

        // CCoreServices DirectUI::DXamlServices::GetHandle
        CCoreServiceAbi* pCoreService = (CCoreServiceAbi*)((ulong*)pXamlCore + 8);

        // CApplication::SetValue
        // std::unique_ptr<FrameworkTheming> m_spTheming 
        FrameworkThemingAbi* theming = *(FrameworkThemingAbi**)(*(ulong*)pCoreService + 0x670L);
        ((Theme*)theming)[0x50] = theme;
        pFrameworkThemingOnThemeChanged(theming, true);
    }

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandleW(string lpModuleName);

    private readonly struct CCoreServiceAbi;

    private readonly struct DXamlCoreAbi;

    private readonly struct FrameworkThemingAbi;
}