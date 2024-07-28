// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.WinTrust
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WinTrust
  {
    internal static bool VerifyEmbeddedSignature(string fileName, out uint winVerifyExitCode)
    {
      using (WinTrust.Natives.WinTrustData pWVTData = new WinTrust.Natives.WinTrustData(fileName))
      {
        Guid pgActionID = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");
        WinTrust.Natives.WinVerifyTrustResult verifyTrustResult = WinTrust.Natives.WinVerifyTrust(WinTrust.Natives.INVALID_HANDLE_VALUE, pgActionID, pWVTData);
        winVerifyExitCode = (uint) verifyTrustResult;
        return verifyTrustResult == WinTrust.Natives.WinVerifyTrustResult.Success;
      }
    }

    public static bool VerifyMicrosoftSignature(string fileName, out uint winVerifyExitCode)
    {
      bool flag = false;
      using (WinTrust.Natives.WinTrustData pWVTData = new WinTrust.Natives.WinTrustData(fileName))
      {
        Guid pgActionID = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");
        WinTrust.Natives.WinVerifyTrustResult verifyTrustResult = WinTrust.Natives.WinVerifyTrust(WinTrust.Natives.INVALID_HANDLE_VALUE, pgActionID, pWVTData);
        winVerifyExitCode = (uint) verifyTrustResult;
        flag = verifyTrustResult == WinTrust.Natives.WinVerifyTrustResult.Success;
      }
      if (flag)
      {
        try
        {
          string subject = new X509Certificate(fileName).Subject;
          if (!subject.Contains("CN=Microsoft"))
          {
            if (!subject.Contains("CN=VS Bld Lab"))
            {
              winVerifyExitCode = 2148204545U;
              flag = false;
            }
          }
        }
        catch
        {
          winVerifyExitCode = 2148204811U;
          flag = false;
        }
      }
      return flag;
    }

    internal static class Natives
    {
      internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
      internal const string WINTRUST_ACTION_GENERIC_VERIFY_V2 = "{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}";

      [DllImport("wintrust.dll", CharSet = CharSet.Unicode)]
      internal static extern WinTrust.Natives.WinVerifyTrustResult WinVerifyTrust(
        [In] IntPtr hwnd,
        [MarshalAs(UnmanagedType.LPStruct), In] Guid pgActionID,
        [In] WinTrust.Natives.WinTrustData pWVTData);

      private enum WinTrustDataUIChoice : uint
      {
        All = 1,
        None = 2,
        NoBad = 3,
        NoGood = 4,
      }

      private enum WinTrustDataRevocationChecks : uint
      {
        None,
        WholeChain,
      }

      private enum WinTrustDataChoice : uint
      {
        File = 1,
        Catalog = 2,
        Blob = 3,
        Signer = 4,
        Certificate = 5,
      }

      private enum WinTrustDataStateAction : uint
      {
        Ignore,
        Verify,
        Close,
        AutoCache,
        AutoCacheFlush,
      }

      [Flags]
      private enum WinTrustDataProvFlags : uint
      {
        UseIe4TrustFlag = 1,
        NoIe4ChainFlag = 2,
        NoPolicyUsageFlag = 4,
        RevocationCheckNone = 16, // 0x00000010
        RevocationCheckEndCert = 32, // 0x00000020
        RevocationCheckChain = 64, // 0x00000040
        RevocationCheckChainExcludeRoot = 128, // 0x00000080
        SaferFlag = 256, // 0x00000100
        HashOnlyFlag = 512, // 0x00000200
        UseDefaultOsverCheck = 1024, // 0x00000400
        LifetimeSigningFlag = 2048, // 0x00000800
        CacheOnlyUrlRetrieval = 4096, // 0x00001000
      }

      private enum WinTrustDataUIContext : uint
      {
        Execute,
        Install,
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal class WinTrustFileInfo : IDisposable
      {
        private uint StructSize = (uint) Marshal.SizeOf(typeof (WinTrust.Natives.WinTrustFileInfo));
        private IntPtr pszFilePath;
        private IntPtr hFile = IntPtr.Zero;
        private IntPtr pgKnownSubject = IntPtr.Zero;
        private bool disposed;

        internal WinTrustFileInfo(string _filePath) => this.pszFilePath = Marshal.StringToCoTaskMemAuto(_filePath);

        public void Dispose()
        {
          this.Dispose(true);
          GC.SuppressFinalize((object) this);
        }

        private void Dispose(bool disposing)
        {
          if (!this.disposed && this.pszFilePath != IntPtr.Zero)
          {
            Marshal.FreeCoTaskMem(this.pszFilePath);
            this.pszFilePath = IntPtr.Zero;
          }
          this.disposed = true;
        }

        ~WinTrustFileInfo() => this.Dispose(false);
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal sealed class WinTrustData : IDisposable
      {
        private uint StructSize = (uint) Marshal.SizeOf(typeof (WinTrust.Natives.WinTrustData));
        private IntPtr PolicyCallbackData = IntPtr.Zero;
        private IntPtr SIPClientData = IntPtr.Zero;
        private WinTrust.Natives.WinTrustDataUIChoice UIChoice = WinTrust.Natives.WinTrustDataUIChoice.None;
        private WinTrust.Natives.WinTrustDataRevocationChecks RevocationChecks;
        private WinTrust.Natives.WinTrustDataChoice UnionChoice = WinTrust.Natives.WinTrustDataChoice.File;
        private SafeAllocMemHandle FileInfoPtr;
        private WinTrust.Natives.WinTrustFileInfo _wtfiData;
        private WinTrust.Natives.WinTrustDataStateAction StateAction;
        private IntPtr StateData = IntPtr.Zero;
        private WinTrust.Natives.WinTrustDataProvFlags ProvFlags = WinTrust.Natives.WinTrustDataProvFlags.SaferFlag;
        private WinTrust.Natives.WinTrustDataUIContext UIContext;

        internal WinTrustData(string _fileName)
        {
          this._wtfiData = new WinTrust.Natives.WinTrustFileInfo(_fileName);
          this.FileInfoPtr = new SafeAllocMemHandle(Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof (WinTrust.Natives.WinTrustFileInfo))));
          Marshal.StructureToPtr<WinTrust.Natives.WinTrustFileInfo>(this._wtfiData, this.FileInfoPtr.DangerousGetHandle(), false);
        }

        public void Dispose()
        {
          if (this.FileInfoPtr != null && !this.FileInfoPtr.IsInvalid)
          {
            this.FileInfoPtr.Dispose();
            this.FileInfoPtr.SetHandleAsInvalid();
          }
          if (this._wtfiData != null)
          {
            this._wtfiData.Dispose();
            this._wtfiData = (WinTrust.Natives.WinTrustFileInfo) null;
          }
          GC.SuppressFinalize((object) this);
        }

        ~WinTrustData() => this.Dispose();
      }

      internal enum WinVerifyTrustResult : uint
      {
        Success = 0,
        ProviderUnknown = 2148204545, // 0x800B0001
        ActionUnknown = 2148204546, // 0x800B0002
        SubjectFormUnknown = 2148204547, // 0x800B0003
        SubjectNotTrusted = 2148204548, // 0x800B0004
        TRUST_E_FAIL = 2148204811, // 0x800B010B
      }
    }
  }
}
