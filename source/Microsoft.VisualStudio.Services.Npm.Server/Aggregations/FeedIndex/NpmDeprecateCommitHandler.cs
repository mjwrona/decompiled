// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex.NpmDeprecateCommitHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.FeedIndex
{
  public class NpmDeprecateCommitHandler : FeedIndexCommitHandler
  {
    public NpmDeprecateCommitHandler(FeedIndexCommitHandler successor = null)
      : base(successor)
    {
    }

    public override async Task ApplyCommitAsync(
      IFeedIndexAggregationContext context,
      FeedCore feed,
      ICommitLogEntry commitLogEntry)
    {
      if (commitLogEntry.CommitOperationData is NpmDeprecateOperationData op)
      {
        string normalizedName = op.Identity.Name.NormalizedName;
        string normalizedPackageVersion = op.Identity.Version.NormalizedVersion;
        try
        {
          Guid packageId = await FeedIndexCommitHandler.GetFeedPackageId(context, feed, normalizedName);
          Guid feedPackageVersion = await FeedIndexCommitHandler.GetFeedPackageVersion(context, feed, packageId, normalizedPackageVersion);
          PackageVersion packageVersionAsync = await context.FeedIndexClient.GetPackageVersionAsync(feed, packageId, normalizedPackageVersion, true);
          (packageVersionAsync.ProtocolMetadata.Data as JObject)["deprecated"] = (JToken) new JValue(op.DeprecateMessage);
          await context.FeedIndexClient.UpdatePackageVersionsAsync(feed, new List<PackageVersionIndexEntryUpdate>()
          {
            new PackageVersionIndexEntryUpdate()
            {
              Metadata = packageVersionAsync.ProtocolMetadata,
              PackageId = packageId,
              NormalizedVersion = normalizedPackageVersion
            }
          });
          packageId = new Guid();
          op = (NpmDeprecateOperationData) null;
        }
        catch (PackageNotFoundException ex)
        {
          op = (NpmDeprecateOperationData) null;
        }
        catch (PackageVersionNotFoundException ex)
        {
          op = (NpmDeprecateOperationData) null;
        }
      }
      else
      {
        await base.ApplyCommitAsync(context, feed, commitLogEntry);
        op = (NpmDeprecateOperationData) null;
      }
    }
  }
}
