// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RefreshPackageIntermediateData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RefreshPackageIntermediateData
  {
    public IReadOnlyDictionary<IPackageVersion, IReadOnlyList<IUpstreamVersionInstance>> AllVersionInstances { get; }

    public IReadOnlyList<IUpstreamVersionInstance> SelectedInstances { get; }

    public bool ExternalVersionsFromUpstreamAvailable { get; }

    public IReadOnlyCollection<IMetadataEntry> LocalEntries { get; }

    public IReadOnlyCollection<IMetadataEntry> UpstreamEntriesToRetain { get; }

    public IReadOnlyCollection<IMetadataEntry> UpstreamEntriesToAddOrUpdate { get; }

    public RefreshPackageIntermediateData(
      IReadOnlyDictionary<IPackageVersion, IReadOnlyList<IUpstreamVersionInstance>> allVersionInstances,
      IReadOnlyList<IUpstreamVersionInstance> selectedInstances,
      bool externalVersionsFromUpstreamAvailable,
      IReadOnlyCollection<IMetadataEntry> localEntries,
      IReadOnlyCollection<IMetadataEntry> upstreamEntriesToRetain,
      IReadOnlyCollection<IMetadataEntry> upstreamEntriesToAddOrUpdate)
    {
      this.AllVersionInstances = allVersionInstances;
      this.SelectedInstances = selectedInstances;
      this.ExternalVersionsFromUpstreamAvailable = externalVersionsFromUpstreamAvailable;
      this.LocalEntries = localEntries;
      this.UpstreamEntriesToRetain = upstreamEntriesToRetain;
      this.UpstreamEntriesToAddOrUpdate = upstreamEntriesToAddOrUpdate;
    }
  }
}
