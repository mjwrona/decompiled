// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildInformationComponent5
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildInformationComponent5 : BuildInformationComponent4
  {
    protected static SqlMetaData[] typ_BuildDataspaceIdPositionTable = new SqlMetaData[4]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("Uri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Position", SqlDbType.Int)
    };

    public BuildInformationComponent5()
    {
      this.ServiceVersion = ServiceVersion.V5;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection QueryBuildInformation(
      ICollection<BuildDetail> builds,
      IList<string> informationTypes)
    {
      this.TraceEnter(0, nameof (QueryBuildInformation));
      this.PrepareStoredProcedure("prc_QueryBuildInformation2", 3600);
      this.BindBuildDataspaceUriPositionTable("@buildUriTable", (IEnumerable<BuildDetail>) builds);
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
      resultCollection.AddBinder<BuildInformationNode>((ObjectBinder<BuildInformationNode>) new BuildInformationBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryBuildInformation));
      return resultCollection;
    }

    internal override ResultCollection UpdateBuildInformation(
      Guid projectId,
      string buildUri,
      ICollection<InformationChangeRequest> changes,
      TeamFoundationIdentity requestedBy)
    {
      this.TraceEnter(0, nameof (UpdateBuildInformation));
      this.PrepareStoredProcedure("prc_UpdateBuildInformation");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindUri("@buildUri", DBHelper.ExtractDbId(buildUri), false);
      this.BindBuildInformationChangeRequestTable("@informationChangeRequestTable", (IEnumerable<InformationChangeRequest>) changes);
      List<InformationField> rows = new List<InformationField>();
      foreach (InformationChangeRequest change in (IEnumerable<InformationChangeRequest>) changes)
      {
        InformationAddRequest informationAddRequest = change as InformationAddRequest;
        InformationEditRequest informationEditRequest = change as InformationEditRequest;
        if (informationAddRequest != null)
          rows.AddRange((IEnumerable<InformationField>) informationAddRequest.Fields);
        else if (informationEditRequest != null)
          rows.AddRange((IEnumerable<InformationField>) informationEditRequest.Fields);
      }
      this.BindBuildInformationFieldTable("@informationFieldTable", (IEnumerable<InformationField>) rows);
      this.BindIdentity("@requestedBy", requestedBy);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildInformationNode>((ObjectBinder<BuildInformationNode>) new BuildInformationBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (UpdateBuildInformation));
      return resultCollection;
    }

    protected virtual SqlParameter BindBuildDataspaceUriPositionTable(
      string parameterName,
      IEnumerable<BuildDetail> rows)
    {
      int index = 0;
      return this.BindTable(parameterName, "typ_BuildDataspaceIdPositionTable", (rows ?? Enumerable.Empty<BuildDetail>()).Select<BuildDetail, SqlDataRecord>((System.Func<BuildDetail, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x, ++index))));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(BuildDetail row, int index)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildInformationComponent5.typ_BuildDataspaceIdPositionTable);
      sqlDataRecord.SetInt32(0, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord.SetDBNull(1);
      sqlDataRecord.SetString(2, DBHelper.ExtractDbId(row.Uri));
      sqlDataRecord.SetInt32(3, index);
      return sqlDataRecord;
    }
  }
}
