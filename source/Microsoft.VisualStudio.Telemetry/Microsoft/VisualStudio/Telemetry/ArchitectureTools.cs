// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.ArchitectureTools
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using Windows.Win32;
using Windows.Win32.System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal static class ArchitectureTools
  {
    private static readonly Lazy<NativeMethods.SystemInfo> systemInformation = new Lazy<NativeMethods.SystemInfo>((Func<NativeMethods.SystemInfo>) (() =>
    {
      NativeMethods.SystemInfo systemInfo = new NativeMethods.SystemInfo();
      NativeMethods.GetNativeSystemInfo(ref systemInfo);
      return systemInfo;
    }), false);
    private static readonly Dictionary<ArchitectureTools.ProcessorArchitectureType, string> processorArchitectureName = new Dictionary<ArchitectureTools.ProcessorArchitectureType, string>()
    {
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureIntel,
        "Intel"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureMips,
        "MIPS"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureAlpha,
        "Alpha"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitecturePpc,
        "PPC"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureShx,
        "SHX"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureArm,
        "ARM"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureIA64,
        "IA64"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureAlpha64,
        "Alpha64"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureMsil,
        "MSIL"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureAmd64,
        "AMD64"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureArm64,
        "ARM64"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureIA32OnWin64,
        "IA32 on Win64"
      },
      {
        ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureUnknown,
        string.Empty
      }
    };
    private const ushort PROCESS_INFORMATION_CLASS_22000_ProcessMachineTypeInfo = 9;

    internal static void GetImageFileMachineArchitectures(
      out string processArch,
      out string nativeMachineArch)
    {
      ArchitectureTools.ProcessorArchitectureType processArch1;
      ArchitectureTools.ProcessorArchitectureType nativeMachineArch1;
      ArchitectureTools.GetImageFileMachineArchitectures(out processArch1, out nativeMachineArch1);
      processArch = ArchitectureTools.processorArchitectureName[processArch1];
      nativeMachineArch = ArchitectureTools.processorArchitectureName[nativeMachineArch1];
    }

    private static unsafe void GetImageFileMachineArchitectures(
      out ArchitectureTools.ProcessorArchitectureType processArch,
      out ArchitectureTools.ProcessorArchitectureType nativeMachineArch)
    {
      if (!Platform.IsWindows)
      {
        processArch = ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureUnknown;
        nativeMachineArch = ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureUnknown;
      }
      else
      {
        ushort imageFileMachine1 = 0;
        ushort imageFileMachine2 = 0;
        try
        {
          ushort processMachine;
          ushort nativeMachine;
          NativeMethods.IsWow64Process2(NativeMethods.GetCurrentProcess(), out processMachine, out nativeMachine);
          imageFileMachine1 = processMachine;
          imageFileMachine2 = nativeMachine;
          nativeMachineArch = ArchitectureTools.ImageFileMachineToProcessArchitectureType(imageFileMachine2);
        }
        catch (EntryPointNotFoundException ex)
        {
          nativeMachineArch = (ArchitectureTools.ProcessorArchitectureType) ArchitectureTools.systemInformation.Value.ProcessorArchitecture;
        }
        if (imageFileMachine1 == (ushort) 0)
        {
          if (imageFileMachine2 == (ushort) 43620)
          {
            ArchitectureTools.PROCESS_MACHINE_INFORMATION machineInformation;
            imageFileMachine1 = !(bool) PInvoke.GetProcessInformation(PInvoke.GetCurrentProcess(), PROCESS_INFORMATION_CLASS.ProcessInformationClassMax, (void*) &machineInformation, (uint) sizeof (ArchitectureTools.PROCESS_MACHINE_INFORMATION)) ? (ushort) 0 : machineInformation.ProcessMachine;
          }
          else
          {
            ushort num;
            switch (IntPtr.Size)
            {
              case 4:
                num = (ushort) 332;
                break;
              case 8:
                num = (ushort) 34404;
                break;
              default:
                num = (ushort) 0;
                break;
            }
            imageFileMachine1 = num;
          }
        }
        processArch = ArchitectureTools.ImageFileMachineToProcessArchitectureType(imageFileMachine1);
      }
    }

    private static ArchitectureTools.ProcessorArchitectureType ImageFileMachineToProcessArchitectureType(
      ushort imageFileMachine)
    {
      switch (imageFileMachine)
      {
        case 332:
          return ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureIntel;
        case 448:
        case 450:
        case 452:
          return ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureArm;
        case 34404:
          return ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureAmd64;
        case 43620:
          return ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureArm64;
        default:
          return ArchitectureTools.ProcessorArchitectureType.ProcessorArchitectureUnknown;
      }
    }

    private struct PROCESS_MACHINE_INFORMATION
    {
      public ushort ProcessMachine;
      public ushort Res0l;
      public ArchitectureTools.MACHINE_ATTRIBUTES MachineAttributes;
    }

    private enum MACHINE_ATTRIBUTES
    {
      UserEnabled = 1,
      KernelEnabled = 2,
      Wow64Container = 4,
    }

    private enum ProcessorArchitectureType
    {
      ProcessorArchitectureIntel = 0,
      ProcessorArchitectureMips = 1,
      ProcessorArchitectureAlpha = 2,
      ProcessorArchitecturePpc = 3,
      ProcessorArchitectureShx = 4,
      ProcessorArchitectureArm = 5,
      ProcessorArchitectureIA64 = 6,
      ProcessorArchitectureAlpha64 = 7,
      ProcessorArchitectureMsil = 8,
      ProcessorArchitectureAmd64 = 9,
      ProcessorArchitectureIA32OnWin64 = 10, // 0x0000000A
      ProcessorArchitectureArm64 = 12, // 0x0000000C
      ProcessorArchitectureUnknown = 65535, // 0x0000FFFF
    }
  }
}
