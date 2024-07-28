// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WorkItemIndexingStatusProvider
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class WorkItemIndexingStatusProvider : IndexingStatusProvider
  {
    [StaticSafe]
    private static readonly HashSet<string> s_indexingOperations = new HashSet<string>()
    {
      "CrawlMetadata",
      "BeginBulkIndex",
      "CompleteBulkIndex",
      "BeginEntityRename",
      "UpdateClassificationNode"
    };
    [StaticSafe]
    private static readonly HashSet<int> s_indexingJobTrigger = new HashSet<int>()
    {
      1,
      4,
      33
    };

    internal WorkItemIndexingStatusProvider(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
    }

    protected override IEntityType EntityType => (IEntityType) WorkItemEntityType.GetInstance();

    public override ISet<string> GetSupportedOperations() => (ISet<string>) WorkItemIndexingStatusProvider.s_indexingOperations;

    public override ISet<int> GetSupportedJobTriggers() => (ISet<int>) WorkItemIndexingStatusProvider.s_indexingJobTrigger;

    protected override CollectionIndexingStatus? GetIndexingStatus(
      IVssRequestContext requestContext,
      int trigger,
      out IndexingStatusDetails indexingStatusDetails)
    {
      indexingStatusDetails = (IndexingStatusDetails) null;
      switch (trigger)
      {
        case 1:
          return new CollectionIndexingStatus?(CollectionIndexingStatus.Onboarding);
        case 4:
        case 33:
          return new CollectionIndexingStatus?(requestContext.IsWorkItemReindexingWithZeroStalenessFeatureEnabled() ? CollectionIndexingStatus.NotIndexing : CollectionIndexingStatus.Reindexing);
        default:
          return new CollectionIndexingStatus?();
      }
    }
  }
}
