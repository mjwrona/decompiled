// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Metadata.INpmMetadataEntryWriteable
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Npm.Server.Metadata
{
  public interface INpmMetadataEntryWriteable : 
    INpmMetadataEntry,
    IMetadataEntry<NpmPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    ICreateWriteable<INpmMetadataEntryWriteable>,
    IMetadataEntryWriteable<NpmPackageIdentity>,
    IMetadataEntryWritable
  {
    void SetPackageJsonBytes(byte[] bytes, bool areBytesCompressed);

    new PackageManifest PackageManifest { get; set; }

    new PackageJsonOptions PackageJsonOptions { get; set; }

    new string PackageSha1Sum { get; set; }

    new string Deprecated { get; set; }
  }
}
