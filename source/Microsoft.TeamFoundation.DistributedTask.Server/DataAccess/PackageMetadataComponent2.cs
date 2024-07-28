// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.PackageMetadataComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class PackageMetadataComponent2 : PackageMetadataComponent
  {
    public PackageMetadataComponent2() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override PackageData AddPackage(
      string packageType,
      string platform,
      PackageVersion version,
      IDictionary<string, string> data)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddPackage)))
      {
        this.PrepareStoredProcedure("Task.prc_AddPackage");
        this.BindString("@packageType", packageType, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@platform", platform, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@majorVersion", version.Major);
        this.BindInt("@minorVersion", version.Minor);
        this.BindInt("@patchVersion", version.Patch);
        if (data != null)
          this.BindBinary("@data", JsonUtility.Serialize((object) data), SqlDbType.VarBinary);
        else
          this.BindNullValue("@data", SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PackageData>((ObjectBinder<PackageData>) new PackageDataBinder());
          return resultCollection.GetCurrent<PackageData>().FirstOrDefault<PackageData>();
        }
      }
    }

    public override void DeletePackage(string packageType, string platform, PackageVersion version)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeletePackage)))
      {
        this.PrepareStoredProcedure("Task.prc_DeletePackage");
        this.BindString("@packageType", packageType, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@platform", platform, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@majorVersion", version.Major);
        this.BindInt("@minorVersion", version.Minor);
        this.BindInt("@patchVersion", version.Patch);
        this.ExecuteNonQuery();
      }
    }

    public override PackageData GetPackage(
      string packageType,
      string platform,
      PackageVersion version)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPackage)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPackage");
        this.BindString("@packageType", packageType, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@platform", platform, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableInt("@majorVersion", version?.Major);
        this.BindNullableInt("@minorVersion", version?.Minor);
        this.BindNullableInt("@patchVersion", version?.Patch);
        PackageData package = (PackageData) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PackageData>((ObjectBinder<PackageData>) new PackageDataBinder());
          package = resultCollection.GetCurrent<PackageData>().FirstOrDefault<PackageData>();
        }
        return package;
      }
    }

    public override List<PackageData> GetPackages(string packageType, string platform, int top)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPackages)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPackages");
        this.BindString("@packageType", packageType, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@platform", platform, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@top", top);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PackageData>((ObjectBinder<PackageData>) new PackageDataBinder());
          return resultCollection.GetCurrent<PackageData>().Items;
        }
      }
    }
  }
}
