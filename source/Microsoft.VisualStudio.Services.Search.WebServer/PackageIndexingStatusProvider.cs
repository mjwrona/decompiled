// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.PackageIndexingStatusProvider
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class PackageIndexingStatusProvider : IndexingStatusProvider
  {
    [StaticSafe]
    private static readonly HashSet<string> s_indexingOperations = new HashSet<string>()
    {
      "BeginBulkIndex",
      "CompleteBulkIndex"
    };
    [StaticSafe]
    private static readonly HashSet<int> s_indexingJobTrigger = new HashSet<int>()
    {
      1,
      33
    };

    public override ISet<string> GetSupportedOperations() => (ISet<string>) PackageIndexingStatusProvider.s_indexingOperations;

    public override ISet<int> GetSupportedJobTriggers() => (ISet<int>) PackageIndexingStatusProvider.s_indexingJobTrigger;

    internal PackageIndexingStatusProvider(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
    }

    protected override IEntityType EntityType => (IEntityType) PackageEntityType.GetInstance();

    protected override CollectionIndexingStatus? GetIndexingStatus(
      IVssRequestContext requestContext,
      int trigger,
      out IndexingStatusDetails indexingStatusDetails)
    {
      indexingStatusDetails = (IndexingStatusDetails) null;
      if (trigger == 1)
        return new CollectionIndexingStatus?(CollectionIndexingStatus.Onboarding);
      return trigger == 33 ? new CollectionIndexingStatus?(CollectionIndexingStatus.Reindexing) : new CollectionIndexingStatus?();
    }
  }
}
