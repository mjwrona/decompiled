// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FeedExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class FeedExtensions
  {
    public static IndexingUnit ToFeedIndexingUnit(
      this Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IndexingUnit parentIndexingUnit,
      long feedContinuationToken,
      bool isPublic = true)
    {
      if (feed == null)
        throw new ArgumentNullException(nameof (feed));
      if (parentIndexingUnit == null)
        throw new ArgumentNullException(nameof (parentIndexingUnit));
      return new IndexingUnit(feed.Id, "Feed", (IEntityType) PackageEntityType.GetInstance(), parentIndexingUnit.IndexingUnitId)
      {
        TFSEntityAttributes = (TFSEntityAttributes) new FeedTFSAttributes()
        {
          FeedId = feed.Id,
          FeedName = feed.FullyQualifiedName
        },
        Properties = (IndexingProperties) new FeedIndexingProperties()
        {
          FeedId = feed.Id,
          FeedName = feed.Name,
          ProjectId = (feed.Project != (ProjectReference) null ? feed.Project.Id : new Guid()),
          FullyQualifiedName = feed.FullyQualifiedName,
          LatestPackageContinuationToken = 0L,
          IsPublicFeed = isPublic,
          FeedContinuationToken = feedContinuationToken
        }
      };
    }
  }
}
