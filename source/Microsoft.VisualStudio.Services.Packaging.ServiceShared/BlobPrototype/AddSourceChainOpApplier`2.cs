// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AddSourceChainOpApplier`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class AddSourceChainOpApplier<TPackageIdentity, TMetadataEntry> : 
    IMetadataDocumentOpApplier<MetadataDocument<TMetadataEntry>>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
  {
    public MetadataDocument<TMetadataEntry> Apply(
      ICommitLogEntry commitLogEntry,
      MetadataDocument<TMetadataEntry> currentState)
    {
      if ((commitLogEntry.CommitOperationData is IAddOperationData commitOperationData ? commitOperationData.SourceChain : (IEnumerable<UpstreamSourceInfo>) null) == null || !commitOperationData.SourceChain.Any<UpstreamSourceInfo>())
        return currentState;
      string sourceChainKey = UpstreamSourceInfoUtils.GetSourceChainKey(commitOperationData.SourceChain);
      List<List<UpstreamSourceInfo>> collection = currentState.Properties?.SourceChainMap ?? new List<List<UpstreamSourceInfo>>();
      foreach (List<UpstreamSourceInfo> sourceChain in collection)
      {
        if (sourceChainKey == UpstreamSourceInfoUtils.GetSourceChainKey((IEnumerable<UpstreamSourceInfo>) sourceChain))
          return currentState;
      }
      List<List<UpstreamSourceInfo>> sourceChainMap = new List<List<UpstreamSourceInfo>>((IEnumerable<List<UpstreamSourceInfo>>) collection)
      {
        commitOperationData.SourceChain.ToList<UpstreamSourceInfo>()
      };
      return new MetadataDocument<TMetadataEntry>(currentState.Entries, (IMetadataDocumentProperties) new MetadataDocumentProperties(commitOperationData.PackageName, currentState.Properties?.UpstreamsConfigurationHash, (DateTime?) currentState.Properties?.UpstreamsLastRefreshedUtc, sourceChainMap, currentState.Properties?.NameMetadata));
    }
  }
}
