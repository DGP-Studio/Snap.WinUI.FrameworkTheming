using System.Runtime.InteropServices;

namespace Snap.WinUI.FrameworkTheming.Win32;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct IMAGE_DOS_HEADER
{
    public ushort e_magic;
    public ushort e_cblp;
    public ushort e_cp;
    public ushort e_crlc;
    public ushort e_cparhdr;
    public ushort e_minalloc;
    public ushort e_maxalloc;
    public ushort e_ss;
    public ushort e_sp;
    public ushort e_csum;
    public ushort e_ip;
    public ushort e_cs;
    public ushort e_lfarlc;
    public ushort e_ovno;
    public unsafe fixed ushort e_res[4];
    public ushort e_oemid;
    public ushort e_oeminfo;
    public unsafe fixed ushort e_res2[10];
    public int e_lfanew;
}