using Snap.WinUI.FrameworkTheming.Win32;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: DisableRuntimeMarshalling]

namespace Snap.WinUI.FrameworkTheming;

public static unsafe partial class FrameworkTheming
{
    private static readonly nint mux = GetModuleHandleW("microsoft.ui.xaml.dll");

    private static readonly delegate* unmanaged[Stdcall]<DXamlCoreAbi**, int> pDXamlInstanceStorageGetValue;

    private static readonly delegate* unmanaged[Stdcall]<CCoreServiceAbi*> pDXamlServicesGetHandle;

    private static readonly delegate* unmanaged[Stdcall]<FrameworkThemingAbi*, bool, int> pFrameworkThemingOnThemeChanged;

    static FrameworkTheming()
    {
        ReadOnlySpan<byte> muxBytes = new((void*)mux, (int)GetImageSize(mux));

        // DXamlInstanceStorage::GetValue(void** phValue)
        // _Check_return_ HRESULT GetValue(_Outptr_result_maybenull_ Handle* phValue)
        // 40 53                    push    rbx
        // 48 83 EC 20              sub     rsp, 20h
        // 48 8B D9                 mov     rbx, phValue
        // 8B 0D (?? ?? 6B 00)      mov     ecx, cs:?g_dwTlsIndex@DXamlInstanceStorage@@3KA ; dwTlsIndex
        ReadOnlySpan<byte> patternDXamlInstanceStorageGetValue = [0x40, 0x53, 0x48, 0x83, 0xEC, 0x20, 0x48, 0x8B, 0xD9, 0x8B, 0x0D];

        pDXamlInstanceStorageGetValue = (delegate* unmanaged[Stdcall]<DXamlCoreAbi**, int>)(mux + muxBytes.IndexOf(patternDXamlInstanceStorageGetValue));

        // CCoreServices *__fastcall InternalDebugInterop::GetCore(InternalDebugInterop *this)
        // 48 89 4C 24 08           mov     [rsp+arg_0], rcx
        //
        // CCoreServices *__fastcall DirectUI::DXamlServices::GetHandle()
        // 48 83 EC 28              sub     rsp, 28h
        // 48 83 64 24 30 00        and     [rsp+28h+arg_0], 0
        // 48 8D 4C 24 30           lea     rcx, [rsp+28h+arg_0]
        // ?? ?? ?? ?? ??           call
        // 48 8B 44 24 30           mov     rax, [rsp+28h+arg_0]
        // 48 8B 40 40              mov     rax, [rax+40h]
        // 48 83 C4 28              add     rsp, 28h
        // C3                       retn
        ReadOnlySpan<byte> patternInternalDebugInteropGetCore = [0x48, 0x89, 0x4C, 0x24, 0x08];
        ReadOnlySpan<byte> patternDXamlServicesGetHandle = [0x48, 0x83, 0xEC, 0x28, 0x48, 0x83, 0x64, 0x24, 0x30, 0x00, 0x48, 0x8D, 0x4C, 0x24, 0x30];
        ReadOnlySpan<byte> secondPatternDXamlServicesGetHandle = [0x48, 0x8B, 0x44, 0x24, 0x30, 0x48, 0x8B, 0x40, 0x40, 0x48, 0x83, 0xC4, 0x28, 0xC3];

        int offsetDXamlServicesGetHandle = muxBytes.IndexOf(patternDXamlServicesGetHandle);
        while (offsetDXamlServicesGetHandle < muxBytes.Length)
        {
            offsetDXamlServicesGetHandle += muxBytes[offsetDXamlServicesGetHandle..].IndexOf(patternDXamlServicesGetHandle);
            if (!muxBytes.Slice(offsetDXamlServicesGetHandle - 5, 5).SequenceEqual(patternInternalDebugInteropGetCore) && muxBytes.Slice(offsetDXamlServicesGetHandle + patternDXamlServicesGetHandle.Length + 5, secondPatternDXamlServicesGetHandle.Length).SequenceEqual(secondPatternDXamlServicesGetHandle))
            {
                break;
            }

            offsetDXamlServicesGetHandle += patternDXamlServicesGetHandle.Length;
        }

        if (offsetDXamlServicesGetHandle >= muxBytes.Length)
        {
            throw new IndexOutOfRangeException();
        }

        pDXamlServicesGetHandle = (delegate* unmanaged[Stdcall]<CCoreServiceAbi*>)(mux + offsetDXamlServicesGetHandle);

        // FrameworkTheming::OnThemeChanged(FrameworkTheming* this, bool forceUpdate)
        // _Check_return_ HRESULT OnThemeChanged(bool forceUpdate = false)
        // 48 89 5C 24 10           mov     [rsp+arg_8], rbx
        // 48 89 6C 24 18           mov     [rsp+arg_10], rbp
        // 48 89 74 24 20           mov     [rsp+arg_18], rsi
        // 57                       push    rdi
        // 41 56                    push    r14
        // 41 57                    push    r15
        // 48 83 EC 40              sub     rsp, 40h
        // 48 8B 05 ?? ?? 3D 00     mov     rax, cs:__security_cookie
        ReadOnlySpan<byte> patternFrameworkThemingOnThemeChanged = [0x48, 0x89, 0x5C, 0x24, 0x10, 0x48, 0x89, 0x6C, 0x24, 0x18, 0x48, 0x89, 0x74, 0x24, 0x20, 0x57, 0x41, 0x56, 0x41, 0x57, 0x48, 0x83, 0xEC, 0x40, 0x48, 0x8B, 0x05];
        ReadOnlySpan<byte> secondPatternFrameworkThemingOnThemeChanged = [0x3D, 0x00];

        int offsetFrameworkThemingOnThemeChanged = muxBytes.IndexOf(patternFrameworkThemingOnThemeChanged);
        while (offsetFrameworkThemingOnThemeChanged < muxBytes.Length)
        {
            offsetFrameworkThemingOnThemeChanged += muxBytes[offsetFrameworkThemingOnThemeChanged..].IndexOf(patternFrameworkThemingOnThemeChanged);
            if (muxBytes.Slice(offsetFrameworkThemingOnThemeChanged + patternFrameworkThemingOnThemeChanged.Length + 2, secondPatternFrameworkThemingOnThemeChanged.Length).SequenceEqual(secondPatternFrameworkThemingOnThemeChanged))
            {
                break;
            }

            offsetFrameworkThemingOnThemeChanged += patternFrameworkThemingOnThemeChanged.Length;
        }

        if (offsetFrameworkThemingOnThemeChanged >= muxBytes.Length)
        {
            throw new IndexOutOfRangeException();
        }

        pFrameworkThemingOnThemeChanged = (delegate* unmanaged[Stdcall]<FrameworkThemingAbi*, bool, int>)(mux + offsetFrameworkThemingOnThemeChanged);
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint GetImageSize(nint baseAddress)
    {
        IMAGE_DOS_HEADER* pImageDosHeader = (IMAGE_DOS_HEADER*)baseAddress;
        IMAGE_NT_HEADERS64* pImageNtHeader = (IMAGE_NT_HEADERS64*)(pImageDosHeader->e_lfanew + baseAddress);
        return pImageNtHeader->OptionalHeader.SizeOfImage;
    }

    private readonly struct CCoreServiceAbi;

    private readonly struct DXamlCoreAbi;

    private readonly struct FrameworkThemingAbi;
}