// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata.INpmMetadataEntry
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata
{
  public interface INpmMetadataEntry : 
    IMetadataEntry<NpmPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    ICreateWriteable<INpmMetadataEntryWriteable>
  {
    PackageManifest PackageManifest { get; }

    ContentBytes PackageJsonContentBytes { get; }

    string Deprecated { get; }

    PackageJsonOptions PackageJsonOptions { get; }

    string PackageSha1Sum { get; }

    PackageJson PackageJson { get; }

    bool HasGypFileAtRoot { get; }

    bool HasServerJsAtRoot { get; }

    JToken GitHead { get; }
  }
}
