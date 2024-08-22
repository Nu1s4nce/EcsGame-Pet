using System.Runtime.InteropServices;

public struct DataStruct
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string header;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
    public string message;
}
