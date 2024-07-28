// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedChangeSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedChangeSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<FeedChangeSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<FeedChangeSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<FeedChangeSqlResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<FeedChangeSqlResourceComponent4>(4)
    }, "FeedChange");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public FeedChangeSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) FeedChangeSqlResourceComponent.sqlExceptionFactories;

    protected virtual PackageVersionBindOptions ReadPackageChangesBindOptions => PackageVersionBindOptions.None;

    public virtual IEnumerable<FeedChange> GetFeedChanges(
      Guid? projectId,
      bool includeDeleted,
      long token,
      int batchSize)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedChanges");
      this.BindInt("@dataspaceId", this.GetDataspaceId(Guid.Empty));
      this.BindBoolean("@includeDeleted", includeDeleted);
      this.BindLong("@token", token);
      this.BindInt("@batchSize", batchSize);
      return this.ReadFeedChanges();
    }

    public virtual FeedChange GetFeedChange(FeedIdentity feedId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetFeedChange");
      this.BindFeedIdentity(feedId);
      return this.ReadFeedChange();
    }

    public virtual IEnumerable<PackageChange> GetPackageChanges(
      FeedIdentity feedId,
      long token,
      int batchSize)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageChanges");
      this.BindFeedIdentity(feedId);
      this.BindLong("@token", token);
      this.BindInt("@batchSize", batchSize);
      return this.ReadPackageChanges();
    }

    protected FeedChange ReadFeedChange() => this.ReadFeedChanges().FirstOrDefault<FeedChange>();

    protected IEnumerable<FeedChange> ReadFeedChanges()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FeedChange>((ObjectBinder<FeedChange>) new FeedChangeBinder((TeamFoundationSqlResourceComponent) this));
        return (IEnumerable<FeedChange>) resultCollection.GetCurrent<FeedChange>().Items;
      }
    }

    protected FeedChange ReadFeedChangeWithUpstreams() => this.ReadFeedChangesWithUpstreams().FirstOrDefault<FeedChange>();

    protected IEnumerable<FeedChange> ReadFeedChangesWithUpstreams()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        Guid instanceId = this.RequestContext.ServiceHost.CollectionServiceHost.InstanceId;
        resultCollection.AddBinder<FeedChange>((ObjectBinder<FeedChange>) new FeedChangeBinder((TeamFoundationSqlResourceComponent) this));
        resultCollection.AddBinder<UpstreamSourceSql>((ObjectBinder<UpstreamSourceSql>) new UpstreamBinder(instanceId));
        List<FeedChange> items = resultCollection.GetCurrent<FeedChange>().Items;
        Dictionary<Guid, FeedChange> dictionary = items.ToDictionary<FeedChange, Guid, FeedChange>((System.Func<FeedChange, Guid>) (x => x.Feed.Id), (System.Func<FeedChange, FeedChange>) (x => x));
        if (resultCollection.TryNextResult())
        {
          foreach (UpstreamSourceSql upstreamSourceSql in resultCollection.GetCurrent<UpstreamSourceSql>().Items)
            dictionary[upstreamSourceSql.FeedId].Feed.UpstreamSources.Add(upstreamSourceSql.ToUpstreamSource());
        }
        return (IEnumerable<FeedChange>) items;
      }
    }

    protected IEnumerable<PackageChange> ReadPackageChanges()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        PackageVersionBindOptions changesBindOptions = this.ReadPackageChangesBindOptions;
        resultCollection.AddBinder<PackageChange>((ObjectBinder<PackageChange>) new PackageChangeBinder((IBindOnto<PackageVersion>) new PackageVersionBinder(changesBindOptions, (IBindOnto<MinimalPackageVersion>) new MinimalPackageVersionBinder(changesBindOptions))));
        return (IEnumerable<PackageChange>) resultCollection.GetCurrent<PackageChange>().Items;
      }
    }
  }
}
