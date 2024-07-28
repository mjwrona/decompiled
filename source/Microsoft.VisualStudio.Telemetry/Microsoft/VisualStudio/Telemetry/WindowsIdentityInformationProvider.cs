// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsIdentityInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class WindowsIdentityInformationProvider : IdentityInformationProvider
  {
    private static Func<IPersistentPropertyBag> defaultStorage = (Func<IPersistentPropertyBag>) (() => (IPersistentPropertyBag) new MachinePropertyBag(WindowsIdentityInformationProvider.DefaultStorageLocation));
    private static readonly Lazy<BiosInformation> biosInformation = new Lazy<BiosInformation>(new Func<BiosInformation>(WindowsIdentityInformationProvider.InitializeBiosInformation));

    internal WindowsIdentityInformationProvider(Func<IPersistentPropertyBag> store = null)
      : base(store ?? WindowsIdentityInformationProvider.defaultStorage)
    {
    }

    private static string DefaultStorageLocation
    {
      get
      {
        string path1 = ((IEnumerable<string>) new string[2]
        {
          Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
          Environment.GetEnvironmentVariable("ALLUSERSPROFILE")
        }).FirstOrDefault<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s)));
        return path1 == null ? (string) null : System.IO.Path.Combine(path1, IdentityInformationProvider.DefaultStorageFileName);
      }
    }

    public override string BiosSerialNumber => WindowsIdentityInformationProvider.biosInformation.Value.SerialNumber;

    public override Guid BiosUUID => WindowsIdentityInformationProvider.biosInformation.Value.UUID;

    public override BiosFirmwareTableParserError BiosInformationError => WindowsIdentityInformationProvider.biosInformation.Value.Error;

    private static BiosInformation InitializeBiosInformation() => BiosFirmwareTableParser.ParseBiosFirmwareTable(WindowsFirmwareInformationProvider.GetSystemFirmwareTable(NativeMethods.FirmwareTableProviderSignature.RSMB, ((IEnumerable<string>) WindowsFirmwareInformationProvider.EnumSystemFirmwareTables(NativeMethods.FirmwareTableProviderSignature.RSMB)).FirstOrDefault<string>()));
  }
}
