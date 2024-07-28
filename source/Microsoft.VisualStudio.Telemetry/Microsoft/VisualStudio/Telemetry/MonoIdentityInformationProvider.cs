// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MonoIdentityInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class MonoIdentityInformationProvider : IdentityInformationProvider
  {
    private static Func<IPersistentPropertyBag> defaultStorage = (Func<IPersistentPropertyBag>) (() => (IPersistentPropertyBag) new MachinePropertyBag(MonoIdentityInformationProvider.DefaultStorageLocation));

    internal MonoIdentityInformationProvider(Func<IPersistentPropertyBag> store = null)
      : base(store ?? MonoIdentityInformationProvider.defaultStorage)
    {
    }

    private static string DefaultStorageLocation => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", IdentityInformationProvider.DefaultStorageFileName);

    public override string BiosSerialNumber => "Not Implemented";

    public override Guid BiosUUID => Guid.Empty;

    public override BiosFirmwareTableParserError BiosInformationError => BiosFirmwareTableParserError.Success;
  }
}
