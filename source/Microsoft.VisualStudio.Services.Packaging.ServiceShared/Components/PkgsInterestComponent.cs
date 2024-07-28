// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Components.PkgsInterestComponent
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Components
{
  public class PkgsInterestComponent : PkgsSqlResourceComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory((IComponentCreator[]) new ComponentCreator<PkgsInterestComponent>[1]
    {
      new ComponentCreator<PkgsInterestComponent>(1)
    }, "PkgsInterest");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public PkgsInterestComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.None;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PkgsInterestComponent.sqlExceptionFactories;

    public virtual void RegisterPackageInterest(
      IEnumerable<PackageInterestData> packageInterestData)
    {
      this.ExecuteNonQueryStoredProcedure<PkgsInterestComponent>(this, 5727001, 5727002, 5727003, (Action) (() => this.PrepareRegisterPackageInterest(packageInterestData)));
    }

    public virtual IEnumerable<FeedInterestedInPackage> GetInterestedFeeds(
      WellKnownUpstreamSource source,
      IPackageName packageName)
    {
      return (IEnumerable<FeedInterestedInPackage>) this.ExecuteQueryStoredProcedure<List<FeedInterestedInPackage>>(5727004, 5727005, 5727006, (Action) (() => this.PrepareGetInterestedFeeds((byte) source.Tag, packageName.NormalizedName)), new System.Func<ResultCollection, List<FeedInterestedInPackage>>(this.GetInterestedFeedsProcessor));
    }

    protected virtual SqlMetaData[] typ_PackageInterestTable => new SqlMetaData[5]
    {
      new SqlMetaData("HostId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("FeedId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UpstreamId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("RegistryId", SqlDbType.TinyInt),
      new SqlMetaData("NormalizedName", SqlDbType.NVarChar, (long) byte.MaxValue)
    };

    protected virtual void PrepareRegisterPackageInterest(IEnumerable<PackageInterestData> data)
    {
      this.PrepareStoredProcedure("Packaging.prc_RegisterPackageInterest");
      this.BindTable("@interest", "Packaging.typ_PackageInterestTable", data.Select<PackageInterestData, SqlDataRecord>((System.Func<PackageInterestData, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_PackageInterestTable);
        sqlDataRecord.SetGuid(0, row.FeedInterestedInPackage.Collection.Guid);
        sqlDataRecord.SetGuid(1, row.FeedInterestedInPackage.Feed.Guid);
        sqlDataRecord.SetGuid(2, row.FeedInterestedInPackage.UpstreamId);
        sqlDataRecord.SetByte(3, (byte) row.Source.Tag);
        sqlDataRecord.SetString(4, row.PackageName.NormalizedName);
        return sqlDataRecord;
      })));
    }

    protected virtual void PrepareGetInterestedFeeds(byte registryId, string normalizedName)
    {
      this.PrepareStoredProcedure("Packaging.prc_GetInterestedFeeds");
      this.BindByte("@registryId", registryId);
      this.BindString("@normalizedName", normalizedName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
    }

    protected virtual List<FeedInterestedInPackage> GetInterestedFeedsProcessor(ResultCollection rc)
    {
      rc.AddBinder<FeedInterestedInPackage>((ObjectBinder<FeedInterestedInPackage>) new PkgsInterestComponent.InterestedFeedsBinder());
      return rc.GetCurrent<FeedInterestedInPackage>().Items;
    }

    public virtual IEnumerable<string> GetAllPackagesWithInterestedFeeds(
      WellKnownUpstreamSource source)
    {
      throw new ServiceVersionNotSupportedException(PkgsInterestComponent.ComponentFactory.ServiceName, this.Version, 2);
    }

    internal class InterestedFeedsBinder : ObjectBinder<FeedInterestedInPackage>
    {
      private SqlColumnBinder Collection = new SqlColumnBinder("HostId");
      private SqlColumnBinder Feed = new SqlColumnBinder("FeedId");
      private SqlColumnBinder UpstreamId = new SqlColumnBinder(nameof (UpstreamId));

      protected override FeedInterestedInPackage Bind() => new FeedInterestedInPackage((CollectionId) this.Collection.GetGuid((IDataReader) this.Reader), (FeedId) this.Feed.GetGuid((IDataReader) this.Reader), this.UpstreamId.GetGuid((IDataReader) this.Reader));
    }
  }
}
