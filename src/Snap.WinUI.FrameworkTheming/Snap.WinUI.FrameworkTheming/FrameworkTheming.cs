using System.Runtime.InteropServices;

namespace Snap.WinUI.FrameworkTheming;

public static class FrameworkTheming
{
    public static unsafe void SetTheme(Theme theme)
    {
        checked
        {
            nint mux = GetModuleHandleW("microsoft.ui.xaml.dll");
            nint getDXamlCoreFunc = mux + 0x5ED00C; // DXamlInstanceStorage::GetValue(void** phValue)
            nint onThemeChangedFunc = mux + 0x8D3590; // FrameworkTheming::OnThemeChanged(FrameworkTheming* this, bool forceUpdate)

            delegate* unmanaged<void**, long> getDXamlCoreDelegate = (delegate* unmanaged<void**, long>)getDXamlCoreFunc;
            delegate* unmanaged<byte*, byte, long> onThemeChangedDelegate = (delegate* unmanaged<byte*, byte, long>)onThemeChangedFunc;

            void* dxamlCorePtr = (void*)0ul;
            getDXamlCoreDelegate(&dxamlCorePtr);

            //                                                GetHandle
            //                                                   GetFrameworkTheming
            //                                                    v      v
            byte* themingPtr = *(byte**)(*((ulong*)dxamlCorePtr + 8) + 1648L);

            // m_requestedTheme 
            themingPtr[80] = (byte)theme;
            onThemeChangedDelegate(themingPtr, 1);
        }
    }
    
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint GetModuleHandleW(string lpModuleName);
}