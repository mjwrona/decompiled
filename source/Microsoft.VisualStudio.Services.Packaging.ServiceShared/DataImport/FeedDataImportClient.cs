// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataImport.FeedDataImportClient
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.DataImport;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataImport
{
  public class FeedDataImportClient : IFeedService
  {
    private readonly IVssRequestContext requestContext;
    private readonly ImportSettings importSettings;

    public FeedDataImportClient(IVssRequestContext requestContext, ImportSettings settings)
    {
      this.requestContext = requestContext;
      this.importSettings = settings;
    }

    public FeedCore GetFeed(Guid projectId, string feedId)
    {
      using (FeedDataImportSqlResourceComponent feedTableComponent = this.CreateFeedTableComponent(this.importSettings))
        return feedTableComponent.GetFeed(new Guid(feedId));
    }

    public Task<FeedCore> GetFeedAsync(Guid projectId, string feedId) => Task.FromResult<FeedCore>(this.GetFeed(projectId, feedId));

    public FeedCore GetFeedByIdForAnyScope(Guid feedId, bool includeSoftDeletedFeeds = false) => throw new NotImplementedException();

    public Task<FeedCore> GetFeedByIdForAnyScopeAsync(
      Guid feedId,
      bool includeSoftDeletedFeeds = false,
      Func<FeedCore, bool> rejectCachedFeedIf = null)
    {
      throw new NotImplementedException();
    }

    public Task<FeedView> GetLocalViewOrDefaultAsync(FeedCore feed) => throw new NotImplementedException();

    public Task<IReadOnlyList<FeedView>> GetViewsAsync(FeedCore feed) => throw new NotImplementedException();

    public IEnumerable<FeedCore> GetFeeds(bool includeSoftDeletedFeeds = false)
    {
      using (FeedDataImportSqlResourceComponent feedTableComponent = this.CreateFeedTableComponent(this.importSettings))
        return (IEnumerable<FeedCore>) feedTableComponent.GetFeeds();
    }

    public Task<IEnumerable<FeedCore>> GetFeedsAsync(bool includeSoftDeletedFeeds = false) => Task.FromResult<IEnumerable<FeedCore>>(this.GetFeeds(includeSoftDeletedFeeds));

    public FeedView GetView(FeedCore feed, string viewId) => throw new NotImplementedException();

    private FeedDataImportSqlResourceComponent CreateFeedTableComponent(
      ImportSettings importSettings)
    {
      FeedDataImportSqlResourceComponent componentRaw = importSettings.DatabaseProperties.SqlConnectionInfo.CreateComponentRaw<FeedDataImportSqlResourceComponent>(handleNoResourceManagementSchema: true);
      componentRaw.PartitionId = importSettings.PartitionId;
      return componentRaw;
    }
  }
}
