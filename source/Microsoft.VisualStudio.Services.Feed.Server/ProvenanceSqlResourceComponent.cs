// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ProvenanceSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class ProvenanceSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<ProvenanceSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<ProvenanceSqlResourceComponent2>(2)
    }, "PackageVersionProvenance");
    private static readonly Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public ProvenanceSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ProvenanceSqlResourceComponent.sqlExceptionFactories;

    public virtual PackageVersionProvenance GetPackageVersionProvenance(
      FeedIdentity feedId,
      Guid packageId,
      Guid packageVersionId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackageVersionProvenance");
      this.BindFeedIdentity(feedId);
      this.BindGuid("@packageId", packageId);
      this.BindGuid("@packageVersionId", packageVersionId);
      return this.ReadPackageVersionProvenance();
    }

    protected PackageVersionProvenance ReadPackageVersionProvenance() => this.ReadPackageVersionProvenanceRows().FirstOrDefault<PackageVersionProvenance>();

    protected IEnumerable<PackageVersionProvenance> ReadPackageVersionProvenanceRows()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PackageVersionProvenanceRow>((ObjectBinder<PackageVersionProvenanceRow>) new PackageVersionProvenanceBinder());
        return resultCollection.GetCurrent<PackageVersionProvenanceRow>().Items.Select<PackageVersionProvenanceRow, PackageVersionProvenance>(new System.Func<PackageVersionProvenanceRow, PackageVersionProvenance>(this.ToPackageVersionProvenance));
      }
    }

    private PackageVersionProvenance ToPackageVersionProvenance(PackageVersionProvenanceRow row) => new PackageVersionProvenance()
    {
      FeedId = row.FeedId,
      PackageId = row.PackageId,
      PackageVersionId = row.PackageVersionId,
      Provenance = JsonConvert.DeserializeObject<Provenance>(row.Provenance)
    };
  }
}
