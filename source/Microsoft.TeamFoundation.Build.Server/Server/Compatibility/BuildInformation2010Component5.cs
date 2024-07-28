// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildInformation2010Component5
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal class BuildInformation2010Component5 : BuildInformation2010Component4
  {
    protected static SqlMetaData[] typ_BuildDataspaceIdPositionTable = new SqlMetaData[4]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Uri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Position", SqlDbType.Int)
    };

    public BuildInformation2010Component5()
    {
      this.ServiceVersion = ServiceVersion.V5;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection QueryBuildInformation(
      IEnumerable<BuildInfo> builds,
      IList<string> informationTypes)
    {
      this.TraceEnter(0, nameof (QueryBuildInformation));
      this.PrepareStoredProcedure("prc_QueryBuildInformation2");
      this.BindBuildInfoUriDataspacePositionTable("@buildUriTable", builds);
      if (informationTypes.Count > 1 || informationTypes[0] != BuildConstants.Star)
      {
        this.BindStringTable("@informationTypeTable", (IEnumerable<string>) informationTypes, maxLength: 256);
        this.BindBoolean("@allTypes", false);
      }
      else
      {
        this.BindStringTable("@informationTypeTable", (IEnumerable<string>) null);
        this.BindBoolean("@allTypes", true);
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildInformationNode2010>((ObjectBinder<BuildInformationNode2010>) new BuildInformationNode2010Binder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryBuildInformation));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildInformationWithConversion(
      IEnumerable<string> uris,
      IList<string> informationTypes)
    {
      throw new NotImplementedException();
    }

    protected virtual SqlParameter BindBuildInfoUriDataspacePositionTable(
      string parameterName,
      IEnumerable<BuildInfo> rows)
    {
      int index = 0;
      return this.BindTable(parameterName, "typ_BuildDataspaceIdPositionTable", (rows ?? Enumerable.Empty<BuildInfo>()).Select<BuildInfo, SqlDataRecord>((System.Func<BuildInfo, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x, ++index))));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(BuildInfo row, int index)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildInformation2010Component5.typ_BuildDataspaceIdPositionTable);
      sqlDataRecord.SetInt32(0, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord.SetDBNull(1);
      sqlDataRecord.SetString(2, DBHelper.ExtractDbId(row.Uri));
      sqlDataRecord.SetInt32(3, ++index);
      return sqlDataRecord;
    }
  }
}
