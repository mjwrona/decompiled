// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent14
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Contracts;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent14 : PackageSqlResourceComponent13
  {
    public override IEnumerable<PackageDependencyDetails> GetPackageDependencyByName(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType,
      IEnumerable<PackageDependency> packageDependencies)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackagesByName");
      this.BindFeedIdentity(feed.GetIdentity());
      this.BindString("@protocolType", protocolType, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindPackageDependenciesList("@packageNamesList", packageDependencies);
      return this.ReadPackageDependencyDetails();
    }

    protected IEnumerable<PackageDependencyDetails> ReadPackageDependencyDetails()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PackageDependencyDetails>((ObjectBinder<PackageDependencyDetails>) new PackageDependencyDetailsBinder());
        List<PackageDependencyDetails> dependencyDetailsList = new List<PackageDependencyDetails>();
        foreach (PackageDependencyDetails dependencyDetails in resultCollection.GetCurrent<PackageDependencyDetails>())
          dependencyDetailsList.Add(dependencyDetails);
        return (IEnumerable<PackageDependencyDetails>) dependencyDetailsList;
      }
    }

    protected SqlParameter BindPackageDependenciesList(
      string parameterName,
      IEnumerable<PackageDependency> packageDependencies)
    {
      packageDependencies = packageDependencies ?? Enumerable.Empty<PackageDependency>();
      int order = 0;
      System.Func<PackageDependency, SqlDataRecord> selector = (System.Func<PackageDependency, SqlDataRecord>) (dep =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.Typ_PackageNamesList);
        sqlDataRecord.SetString(0, dep.PackageName);
        sqlDataRecord.SetInt32(1, order++);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Feed.typ_PackageNamesList", packageDependencies.Select<PackageDependency, SqlDataRecord>(selector));
    }

    protected virtual SqlMetaData[] Typ_PackageNamesList => new SqlMetaData[2]
    {
      new SqlMetaData("PackageName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Ordering", SqlDbType.Int)
    };
  }
}
