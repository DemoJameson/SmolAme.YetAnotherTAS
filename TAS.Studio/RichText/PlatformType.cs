﻿using System;
using System.Runtime.InteropServices;

namespace TAS.Studio.RichText;

public static class PlatformType {
    const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
    const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
    const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
    const ushort PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF;

    [DllImport("kernel32.dll")]
    static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

    [DllImport("kernel32.dll")]
    static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

    public static Platform GetOperationSystemPlatform() {
        var sysInfo = new SYSTEM_INFO();

        // WinXP and older - use GetNativeSystemInfo
        if (Environment.OSVersion.Version.Major > 5 ||
            (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1)) {
            GetNativeSystemInfo(ref sysInfo);
        }
        // else use GetSystemInfo
        else {
            GetSystemInfo(ref sysInfo);
        }

        switch (sysInfo.wProcessorArchitecture) {
            case PROCESSOR_ARCHITECTURE_IA64:
            case PROCESSOR_ARCHITECTURE_AMD64:
                return Platform.X64;

            case PROCESSOR_ARCHITECTURE_INTEL:
                return Platform.X86;

            default:
                return Platform.Unknown;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SYSTEM_INFO {
        public readonly ushort wProcessorArchitecture;
        public readonly ushort wReserved;
        public readonly uint dwPageSize;
        public readonly IntPtr lpMinimumApplicationAddress;
        public readonly IntPtr lpMaximumApplicationAddress;
        public readonly UIntPtr dwActiveProcessorMask;
        public readonly uint dwNumberOfProcessors;
        public readonly uint dwProcessorType;
        public readonly uint dwAllocationGranularity;
        public readonly ushort wProcessorLevel;
        public readonly ushort wProcessorRevision;
    };
}

public enum Platform {
    X86,
    X64,
    Unknown
}