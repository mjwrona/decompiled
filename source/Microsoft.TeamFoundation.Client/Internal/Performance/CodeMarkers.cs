// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkers
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.Performance
{
  internal sealed class CodeMarkers
  {
    public static readonly CodeMarkers Instance = new CodeMarkers();
    private const string AtomName = "VSCodeMarkersEnabled";
    private const string DllName = "Microsoft.Internal.Performance.CodeMarkers.dll";
    private bool fUseCodeMarkers;

    private CodeMarkers() => this.fUseCodeMarkers = CodeMarkers.NativeMethods.FindAtom("VSCodeMarkersEnabled") > (ushort) 0;

    public void CodeMarker(int nTimerID)
    {
      if (!this.fUseCodeMarkers)
        return;
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker(nTimerID, (byte[]) null, 0);
      }
      catch (DllNotFoundException ex)
      {
        this.fUseCodeMarkers = false;
      }
    }

    public void CodeMarkerEx(int nTimerID, byte[] aBuff)
    {
      if (!this.fUseCodeMarkers)
        return;
      if (aBuff == null)
        throw new ArgumentNullException(nameof (aBuff));
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker(nTimerID, aBuff, aBuff.Length);
      }
      catch (DllNotFoundException ex)
      {
        this.fUseCodeMarkers = false;
      }
    }

    public void CodeMarkerEx(int nTimerID, Guid guidData) => this.CodeMarkerEx(nTimerID, guidData.ToByteArray());

    public void CodeMarkerEx(int nTimerID, uint uintData) => this.CodeMarkerEx(nTimerID, BitConverter.GetBytes(uintData));

    public void CodeMarkerEx(int nTimerID, ulong ulongData) => this.CodeMarkerEx(nTimerID, BitConverter.GetBytes(ulongData));

    public void InitPerformanceDll(int iApp, string strRegRoot)
    {
      this.fUseCodeMarkers = false;
      if (!CodeMarkers.UseCodeMarkers(strRegRoot))
        return;
      try
      {
        int num = (int) CodeMarkers.NativeMethods.AddAtom("VSCodeMarkersEnabled");
        CodeMarkers.NativeMethods.DllInitPerf(iApp);
        this.fUseCodeMarkers = true;
      }
      catch (DllNotFoundException ex)
      {
      }
    }

    private static bool UseCodeMarkers(string strRegRoot) => !string.IsNullOrEmpty(CodeMarkers.GetPerformanceSubKey(Registry.LocalMachine, strRegRoot));

    private static string GetPerformanceSubKey(RegistryKey hKey, string strRegRoot)
    {
      if (hKey == null)
        return (string) null;
      string performanceSubKey = (string) null;
      using (RegistryKey registryKey = hKey.OpenSubKey(strRegRoot + "\\Performance"))
      {
        if (registryKey != null)
          performanceSubKey = registryKey.GetValue("").ToString();
      }
      return performanceSubKey;
    }

    public void UninitializePerformanceDLL(int iApp)
    {
      if (!this.fUseCodeMarkers)
        return;
      this.fUseCodeMarkers = false;
      ushort atom = CodeMarkers.NativeMethods.FindAtom("VSCodeMarkersEnabled");
      if (atom != (ushort) 0)
      {
        int num = (int) CodeMarkers.NativeMethods.DeleteAtom(atom);
      }
      try
      {
        CodeMarkers.NativeMethods.DllUnInitPerf(iApp);
      }
      catch (DllNotFoundException ex)
      {
      }
    }

    private static class NativeMethods
    {
      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "InitPerf")]
      public static extern void DllInitPerf(int iApp);

      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "UnInitPerf")]
      public static extern void DllUnInitPerf(int iApp);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      public static extern ushort AddAtom(string lpString);

      [DllImport("kernel32.dll")]
      public static extern ushort DeleteAtom(ushort atom);

      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
      public static extern void DllPerfCodeMarker(int nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] aUserParams, int cbParams);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      public static extern ushort FindAtom(string lpString);
    }
  }
}
