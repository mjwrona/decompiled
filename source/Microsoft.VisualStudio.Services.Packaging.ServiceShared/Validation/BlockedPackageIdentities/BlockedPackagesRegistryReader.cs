// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities.BlockedPackagesRegistryReader
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities
{
  public class BlockedPackagesRegistryReader : 
    IFactory<IProtocol, IEnumerable<BlockedPackageIdentity>>
  {
    private readonly IDeploymentLevelRegistryService registry;

    public BlockedPackagesRegistryReader(IDeploymentLevelRegistryService registry) => this.registry = registry;

    public IEnumerable<BlockedPackageIdentity> Get(IProtocol protocol) => this.registry.Read((RegistryQuery) ("/Configuration/Packaging/BlockedPackageIdentities/Global/" + protocol.CorrectlyCasedName + "/*")).Select<RegistryItem, BlockedPackagesRegistryReader.BlockedPackageRegistryRecord>((Func<RegistryItem, BlockedPackagesRegistryReader.BlockedPackageRegistryRecord>) (x => JsonConvert.DeserializeObject<BlockedPackagesRegistryReader.BlockedPackageRegistryRecord>(x.Value))).Select<BlockedPackagesRegistryReader.BlockedPackageRegistryRecord, BlockedPackageIdentity>((Func<BlockedPackagesRegistryReader.BlockedPackageRegistryRecord, BlockedPackageIdentity>) (entry => new BlockedPackageIdentity((IPackageName) new BlockedPackagesRegistryReader.SynthesizedPackageName(protocol, entry.NormalizedName), entry.NormalizedVersion == null ? (IPackageVersion) null : (IPackageVersion) new BlockedPackagesRegistryReader.SynthesizedPackageVersion(entry.NormalizedVersion))));

    private class BlockedPackageRegistryRecord
    {
      public string NormalizedName { get; }

      public string NormalizedVersion { get; }

      public BlockedPackageRegistryRecord(string normalizedName, string normalizedVersion)
      {
        this.NormalizedName = normalizedName;
        this.NormalizedVersion = normalizedVersion;
      }
    }

    private class SynthesizedPackageName : IPackageName
    {
      public IProtocol Protocol { get; }

      public string DisplayName => this.NormalizedName;

      public string NormalizedName { get; }

      public SynthesizedPackageName(IProtocol protocol, string normalizedName)
      {
        this.Protocol = protocol;
        this.NormalizedName = normalizedName;
      }
    }

    private class SynthesizedPackageVersion : IPackageVersion
    {
      public string DisplayVersion => this.NormalizedVersion;

      public string NormalizedVersion { get; }

      public SynthesizedPackageVersion(string normalizedVersion) => this.NormalizedVersion = normalizedVersion;
    }
  }
}
