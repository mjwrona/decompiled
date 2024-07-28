// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew.MavenUpdateUpstreamMetadataWithFilesOpApplier
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Aggregations.PackageMetadata.OperationAppliersNew
{
  public class MavenUpdateUpstreamMetadataWithFilesOpApplier : 
    UpdateUpstreamMetadataWithFilesOpApplier<MavenPackageIdentity, MavenPackageName, MavenPackageVersion, IMavenMetadataEntry, IMavenMetadataEntryWritable>
  {
    public MavenUpdateUpstreamMetadataWithFilesOpApplier(
      IComparer<IPackageVersion> versionComparer,
      IEqualityComparer<IPackageVersion> versionEqualityComparer)
      : base(versionComparer, versionEqualityComparer)
    {
    }

    public override MetadataDocument<IMavenMetadataEntry> Apply(
      ICommitLogEntry commitLogEntry,
      MetadataDocument<IMavenMetadataEntry> currentDoc)
    {
      MetadataDocument<IMavenMetadataEntry> metadataDocument = base.Apply(commitLogEntry, currentDoc);
      if (!(commitLogEntry.CommitOperationData is IUpdateUpstreamMetadataOperationData<MavenPackageIdentity, MavenPackageName, MavenPackageVersion, IMavenMetadataEntry> commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_InvalidCommitEntryType());
      Dictionary<string, List<UpstreamSourceInfo>> dictionary = (metadataDocument.Properties.SourceChainMap ?? new List<List<UpstreamSourceInfo>>()).ToDictionary<List<UpstreamSourceInfo>, string>((Func<List<UpstreamSourceInfo>, string>) (sourceChain => UpstreamSourceInfoUtils.GetSourceChainKey((IEnumerable<UpstreamSourceInfo>) sourceChain.ToList<UpstreamSourceInfo>())));
      foreach (IMavenMetadataEntry upstreamEntry in commitOperationData.UpstreamEntries)
      {
        string sourceChainKey = UpstreamSourceInfoUtils.GetSourceChainKey(upstreamEntry.SourceChain);
        if (!dictionary.ContainsKey(sourceChainKey))
          dictionary.Add(sourceChainKey, upstreamEntry.SourceChain.ToList<UpstreamSourceInfo>());
      }
      return new MetadataDocument<IMavenMetadataEntry>(metadataDocument.Entries, (IMetadataDocumentProperties) new MetadataDocumentProperties((IPackageName) commitOperationData.PackageName, metadataDocument.Properties.UpstreamsConfigurationHash, metadataDocument.Properties.UpstreamsLastRefreshedUtc, dictionary.Values.ToList<List<UpstreamSourceInfo>>(), commitOperationData.PackageNameMetadata));
    }
  }
}
