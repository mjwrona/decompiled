// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex.NpmMetadataDiffCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex
{
  public class NpmMetadataDiffCommitHandler : FeedIndexCommitHandler
  {
    public NpmMetadataDiffCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override async Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      if (commitLogEntry.CommitOperationData is NpmMetadataDiffOperationData commitOperationData)
      {
        IDictionary<string, VersionMetadata> oldState = commitOperationData.OldVersionMetadata;
        IDictionary<string, VersionMetadata> newState = commitOperationData.NewVersionMetadata;
        List<PackageVersionIndexEntryUpdate> updates = new List<PackageVersionIndexEntryUpdate>();
        Guid packageId = await FeedIndexCommitHandler.GetFeedPackageId(context, feed, commitOperationData.PackageName.NormalizedName);
        foreach (KeyValuePair<string, VersionMetadata> keyValuePair in (IEnumerable<KeyValuePair<string, VersionMetadata>>) newState)
        {
          KeyValuePair<string, VersionMetadata> newPair = keyValuePair;
          if (oldState[newPair.Key].Deprecated != newPair.Value.Deprecated)
          {
            SemanticVersion version = SemanticVersion.Parse(newPair.Key);
            try
            {
              PackageVersion packageVersionAsync = await context.FeedIndexClient.GetPackageVersionAsync(feed, packageId, version.NormalizedVersion, true);
              (packageVersionAsync.ProtocolMetadata.Data as JObject)["deprecated"] = (JToken) new JValue(newPair.Value.Deprecated);
              updates.Add(new PackageVersionIndexEntryUpdate()
              {
                Metadata = packageVersionAsync.ProtocolMetadata,
                PackageId = packageId,
                NormalizedVersion = version.NormalizedVersion
              });
            }
            catch (PackageVersionNotFoundException ex)
            {
            }
            version = (SemanticVersion) null;
          }
          newPair = new KeyValuePair<string, VersionMetadata>();
        }
        if (!updates.Any<PackageVersionIndexEntryUpdate>())
          return;
        await context.FeedIndexClient.UpdatePackageVersionsAsync(feed, updates);
      }
      else
        await base.ApplyCommitAsync(context, feed, commitLogEntry);
    }
  }
}
