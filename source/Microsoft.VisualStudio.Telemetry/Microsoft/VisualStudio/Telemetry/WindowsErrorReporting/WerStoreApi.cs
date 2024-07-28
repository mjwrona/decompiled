// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WerStoreApi
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  public class WerStoreApi
  {
    private static bool? doesInterfaceExist;
    private const string WerDllName = "wer.dll";

    [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern uint GetSystemDirectory([Out] StringBuilder lpBuffer, uint uSize);

    [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string fileName);

    [DllImport("Kernel32", SetLastError = true)]
    private static extern IntPtr FreeLibrary(IntPtr hLib);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int WerStoreOpen(
      WerStoreApi.REPORT_STORE_TYPES storeType,
      ref IntPtr hResportStore);

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern void WerStoreClose(IntPtr hResportStore);

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint WerStoreGetNextReportKey(
      IntPtr hResportStore,
      ref IntPtr reportKeyPtr);

    [DllImport("wer.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint WerStoreQueryReportMetadataV2(
      IntPtr hResportStore,
      string reportKey,
      [MarshalAs(UnmanagedType.Struct)] ref WerStoreApi.ReportMetaData report);

    public static bool IsStoreInterfacePresent
    {
      get
      {
        if (!WerStoreApi.doesInterfaceExist.HasValue)
        {
          IntPtr num = WerStoreApi.LoadLibrary(System.IO.Path.Combine(Environment.SystemDirectory, "wer.dll"));
          IntPtr procAddress1 = WerStoreApi.GetProcAddress(num, "WerStoreOpen");
          IntPtr procAddress2 = WerStoreApi.GetProcAddress(num, "WerStoreQueryReportMetadataV2");
          IntPtr procAddress3 = WerStoreApi.GetProcAddress(num, "WerStoreGetNextReportKey");
          WerStoreApi.doesInterfaceExist = new bool?(procAddress1 != IntPtr.Zero && procAddress2 != IntPtr.Zero && procAddress3 != IntPtr.Zero);
          WerStoreApi.FreeLibrary(num);
        }
        return WerStoreApi.doesInterfaceExist.Value;
      }
    }

    public static WerStoreApi.IWerStore GetStore(WerStoreApi.REPORT_STORE_TYPES type)
    {
      if (!WerStoreApi.IsStoreInterfacePresent)
        return (WerStoreApi.IWerStore) new WerStoreApi.EmptyStore();
      try
      {
        return (WerStoreApi.IWerStore) new WerStoreApi.WerStore(type);
      }
      catch (InvalidOperationException ex)
      {
        return (WerStoreApi.IWerStore) new WerStoreApi.EmptyStore();
      }
    }

    private static Guid ToManagedGuid(WerStoreApi.GUID nativeGuid) => new Guid(nativeGuid.Data1, nativeGuid.Data2, nativeGuid.Data3, nativeGuid.Data4);

    private static DateTime FiletimeToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME fileTime) => DateTime.FromFileTimeUtc((long) fileTime.dwHighDateTime << 32 | (long) (uint) fileTime.dwLowDateTime);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct ReportMetaData
    {
      public WerStoreApi.WER_REPORT_SIGNATURE Signature;
      public WerStoreApi.GUID BucketId;
      public WerStoreApi.GUID ReportId;
      public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
      public ulong SizeInBytes;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string CabID;
      public ulong ReportStatus;
      public WerStoreApi.GUID ReportIntegratorId;
      public uint NumberOfFiles;
      public uint SizeOfFileNames;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
      public string FileNames;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WER_REPORT_SIGNATURE
    {
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
      public string EventName;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
      public WerStoreApi.WER_REPORT_PARAMETER[] Parameters;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WER_REPORT_PARAMETER
    {
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
      public string Name;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string Value;
    }

    private struct GUID
    {
      public int Data1;
      public short Data2;
      public short Data3;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public byte[] Data4;
    }

    public enum REPORT_STORE_TYPES
    {
      MACHINE_ARCHIVE = 2,
      MACHINE_QUEUE = 3,
    }

    public interface IWerStore : IDisposable
    {
      IEnumerable<WerStoreApi.IWerReportData> GetReports();
    }

    private class EmptyStore : WerStoreApi.IWerStore, IDisposable
    {
      public IEnumerable<WerStoreApi.IWerReportData> GetReports() => Enumerable.Empty<WerStoreApi.IWerReportData>();

      public void Dispose()
      {
      }
    }

    public interface IWerReportData
    {
      Guid ReportId { get; }

      string EventType { get; }

      WerStoreApi.WER_REPORT_PARAMETER[] Parameters { get; }

      string CabID { get; }

      Guid BucketId { get; }

      DateTime TimeStamp { get; }
    }

    private class WerReportData : WerStoreApi.IWerReportData
    {
      private WerStoreApi.ReportMetaData reportInternal;

      public WerReportData(WerStoreApi.ReportMetaData data) => this.reportInternal = data;

      public Guid ReportId => WerStoreApi.ToManagedGuid(this.reportInternal.ReportId);

      public string EventType => this.reportInternal.Signature.EventName;

      public WerStoreApi.WER_REPORT_PARAMETER[] Parameters => this.reportInternal.Signature.Parameters;

      public string CabID => this.reportInternal.CabID;

      public Guid BucketId => WerStoreApi.ToManagedGuid(this.reportInternal.BucketId);

      public DateTime TimeStamp => WerStoreApi.FiletimeToDateTime(this.reportInternal.CreationTime);
    }

    private class WerStore : WerStoreApi.IWerStore, IDisposable
    {
      private IntPtr hStore;
      private bool isDisposed;

      public WerStore(WerStoreApi.REPORT_STORE_TYPES type)
      {
        this.hStore = IntPtr.Zero;
        if (WerStoreApi.WerStoreOpen(type, ref this.hStore) != 0 || this.hStore == IntPtr.Zero)
          throw new InvalidOperationException();
        this.isDisposed = false;
      }

      public void Dispose()
      {
        if (this.isDisposed)
          return;
        WerStoreApi.WerStoreClose(this.hStore);
        this.isDisposed = true;
      }

      public IEnumerable<WerStoreApi.IWerReportData> GetReports()
      {
        if (this.isDisposed)
          throw new ObjectDisposedException(nameof (WerStore));
        foreach (string key in this.GetKeys().Reverse<string>())
        {
          WerStoreApi.WerReportData report = this.GetReport(key);
          if (report != null)
            yield return (WerStoreApi.IWerReportData) report;
        }
      }

      private IEnumerable<string> GetKeys()
      {
        List<string> keys = new List<string>();
        while (true)
        {
          IntPtr zero = IntPtr.Zero;
          int nextReportKey = (int) WerStoreApi.WerStoreGetNextReportKey(this.hStore, ref zero);
          if (!(zero == IntPtr.Zero))
          {
            string stringUni = Marshal.PtrToStringUni(zero);
            keys.Add(stringUni);
          }
          else
            break;
        }
        return (IEnumerable<string>) keys;
      }

      private WerStoreApi.WerReportData GetReport(string key)
      {
        WerStoreApi.ReportMetaData report = new WerStoreApi.ReportMetaData();
        switch (WerStoreApi.WerStoreQueryReportMetadataV2(this.hStore, key, ref report))
        {
          case 0:
            return new WerStoreApi.WerReportData(report);
          case 2147942405:
            return (WerStoreApi.WerReportData) null;
          default:
            return new WerStoreApi.WerReportData(report);
        }
      }
    }
  }
}
