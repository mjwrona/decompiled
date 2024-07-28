// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.InUseDetection
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class InUseDetection
  {
    internal const string MessageMatchRegex = "[^\"^'^„^“]+['\"„“](.*)['\"””][^'^\"^”^”]+";
    private const int RmRebootReasonNone = 0;
    private const int CCH_RM_MAX_APP_NAME = 255;
    private const int CCH_RM_MAX_SVC_NAME = 63;

    public static bool TryGetProcessesUsingFile(
      IOException ioEx,
      out string path,
      out IList<Process> processes)
    {
      if (ioEx.HResult == -2147024864)
      {
        path = (string) null;
        FieldInfo field = typeof (IOException).GetField("_maybeFullPath", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != (FieldInfo) null)
          path = (string) field.GetValue((object) ioEx);
        if (path == null)
        {
          Match match = Regex.Match(ioEx.Message, "[^\"^'^„^“]+['\"„“](.*)['\"””][^'^\"^”^”]+");
          if (match.Success && match.Groups.Count == 2)
            path = match.Groups[1].Value;
        }
        if (path != null)
        {
          try
          {
            processes = InUseDetection.GetProcessesUsingFiles(path);
            return true;
          }
          catch (Win32Exception ex)
          {
          }
        }
      }
      processes = (IList<Process>) null;
      path = (string) null;
      return false;
    }

    public static IList<Process> GetProcessesUsingFiles(params string[] filePaths)
    {
      try
      {
        return InUseDetection.GetProcessesUsingFilesInternal(filePaths);
      }
      catch (Win32Exception ex)
      {
        return InUseDetection.GetProcessesUsingFilesInternal(filePaths);
      }
    }

    private static IList<Process> GetProcessesUsingFilesInternal(params string[] filePaths)
    {
      List<Process> usingFilesInternal = new List<Process>();
      uint pSessionHandle;
      int error1 = InUseDetection.RmStartSession(out pSessionHandle, 0, Guid.NewGuid().ToString("N"));
      if (error1 != 0)
        throw new Win32Exception(error1);
      try
      {
        string[] rgsFilenames = new string[filePaths.Length];
        filePaths.CopyTo((Array) rgsFilenames, 0);
        int error2 = InUseDetection.RmRegisterResources(pSessionHandle, (uint) rgsFilenames.Length, rgsFilenames, 0U, (InUseDetection.RM_UNIQUE_PROCESS[]) null, 0U, (string[]) null);
        if (error2 != 0)
          throw new Win32Exception(error2);
        uint pnProcInfoNeeded = 0;
        uint pnProcInfo = 0;
        uint lpdwRebootReasons = 0;
        int list1 = InUseDetection.RmGetList(pSessionHandle, out pnProcInfoNeeded, ref pnProcInfo, (InUseDetection.RM_PROCESS_INFO[]) null, ref lpdwRebootReasons);
        switch (list1)
        {
          case 0:
            break;
          case 234:
            InUseDetection.RM_PROCESS_INFO[] rgAffectedApps = new InUseDetection.RM_PROCESS_INFO[(int) pnProcInfoNeeded];
            uint length = (uint) rgAffectedApps.Length;
            int list2 = InUseDetection.RmGetList(pSessionHandle, out pnProcInfoNeeded, ref length, rgAffectedApps, ref lpdwRebootReasons);
            if (list2 != 0)
              throw new Win32Exception(list2);
            for (int index = 0; (long) index < (long) length; ++index)
            {
              try
              {
                usingFilesInternal.Add(Process.GetProcessById(rgAffectedApps[index].Process.dwProcessId));
              }
              catch (ArgumentException ex)
              {
              }
            }
            break;
          default:
            throw new Win32Exception(list1);
        }
      }
      finally
      {
        InUseDetection.RmEndSession(pSessionHandle);
      }
      return (IList<Process>) usingFilesInternal;
    }

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    private static extern int RmStartSession(
      out uint pSessionHandle,
      int dwSessionFlags,
      string strSessionKey);

    [DllImport("rstrtmgr.dll")]
    private static extern int RmEndSession(uint pSessionHandle);

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    private static extern int RmRegisterResources(
      uint pSessionHandle,
      uint nFiles,
      string[] rgsFilenames,
      uint nApplications,
      InUseDetection.RM_UNIQUE_PROCESS[] rgApplications,
      uint nServices,
      string[] rgsServiceNames);

    [DllImport("rstrtmgr.dll")]
    private static extern int RmGetList(
      uint dwSessionHandle,
      out uint pnProcInfoNeeded,
      ref uint pnProcInfo,
      [In, Out] InUseDetection.RM_PROCESS_INFO[] rgAffectedApps,
      ref uint lpdwRebootReasons);

    private struct RM_UNIQUE_PROCESS
    {
      public int dwProcessId;
      public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct RM_PROCESS_INFO
    {
      public InUseDetection.RM_UNIQUE_PROCESS Process;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string strAppName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string strServiceShortName;
      public InUseDetection.RM_APP_TYPE ApplicationType;
      public uint AppStatus;
      public uint TSSessionId;
      [MarshalAs(UnmanagedType.Bool)]
      public bool bRestartable;
    }

    private enum RM_APP_TYPE
    {
      RmUnknownApp = 0,
      RmMainWindow = 1,
      RmOtherWindow = 2,
      RmService = 3,
      RmExplorer = 4,
      RmConsole = 5,
      RmCritical = 1000, // 0x000003E8
    }
  }
}
