namespace Snap.WinUI.FrameworkTheming.Win32;

public struct IMAGE_FILE_HEADER
{
    public IMAGE_FILE_MACHINE Machine;
    public ushort NumberOfSections;
    public uint TimeDateStamp;
    public uint PointerToSymbolTable;
    public uint NumberOfSymbols;
    public ushort SizeOfOptionalHeader;
    public IMAGE_FILE_CHARACTERISTICS Characteristics;
}