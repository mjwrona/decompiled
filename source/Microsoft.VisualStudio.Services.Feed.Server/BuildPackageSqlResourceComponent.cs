// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.BuildPackageSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class BuildPackageSqlResourceComponent : FeedSqlResourceComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<BuildPackageSqlResourceComponent>(1)
    }, "BuildPackage");
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static BuildPackageSqlResourceComponent() => BuildPackageSqlResourceComponent.sqlExceptionFactories.Add(1620009, new SqlExceptionFactory(typeof (UnknownDatabaseErrorOcurredException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorVal, sqlException, sqlError) => (Exception) UnknownDatabaseErrorOcurredException.Create(sqlError.ExtractString("optionalMessageString")))));

    public BuildPackageSqlResourceComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) BuildPackageSqlResourceComponent.sqlExceptionFactories;

    protected IEnumerable<BuildPackage> ReadBuildPackages()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildPackage>((ObjectBinder<BuildPackage>) new BuildPackageBinder());
        List<BuildPackage> buildPackageList = new List<BuildPackage>();
        foreach (BuildPackage buildPackage in resultCollection.GetCurrent<BuildPackage>())
          buildPackageList.Add(buildPackage);
        return (IEnumerable<BuildPackage>) buildPackageList;
      }
    }

    public IEnumerable<BuildPackage> GetPackagesByBuildId(int buildId)
    {
      this.PrepareStoredProcedure("Feed.prc_GetPackagesByBuildId");
      this.BindInt("@buildId", buildId);
      return this.ReadBuildPackages();
    }
  }
}
