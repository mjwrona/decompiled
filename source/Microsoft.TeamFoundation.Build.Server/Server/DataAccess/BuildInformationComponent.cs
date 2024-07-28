// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildInformationComponent
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
  internal class BuildInformationComponent : BuildSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<BuildInformationComponent>(1),
      (IComponentCreator) new ComponentCreator<BuildInformationComponent2>(2),
      (IComponentCreator) new ComponentCreator<BuildInformationComponent3>(3),
      (IComponentCreator) new ComponentCreator<BuildInformationComponent4>(4),
      (IComponentCreator) new ComponentCreator<BuildInformationComponent5>(5)
    }, "BuildInformation", "Build");
    protected static SqlMetaData[] typ_BuildInformationChangeRequestTable = new SqlMetaData[6]
    {
      new SqlMetaData("ItemIndex", SqlDbType.Int),
      new SqlMetaData("ChangeType", SqlDbType.TinyInt),
      new SqlMetaData("NodeId", SqlDbType.Int),
      new SqlMetaData("NodeType", SqlDbType.NVarChar, 128L),
      new SqlMetaData("ParentId", SqlDbType.Int),
      new SqlMetaData("EditOptions", SqlDbType.TinyInt)
    };
    protected static SqlMetaData[] typ_BuildInformationFieldTable = new SqlMetaData[3]
    {
      new SqlMetaData("NodeId", SqlDbType.Int),
      new SqlMetaData("FieldName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("FieldValue", SqlDbType.NVarChar, -1L)
    };

    public BuildInformationComponent() => this.ServiceVersion = ServiceVersion.V1;

    protected override string TraceArea => "Build";

    internal virtual ResultCollection QueryBuildInformation(
      ICollection<BuildDetail> builds,
      IList<string> informationTypes)
    {
      this.TraceEnter(0, nameof (QueryBuildInformation));
      this.PrepareStoredProcedure("prc_QueryBuildInformation");
      this.BindTable<KeyValuePair<int, string>>("@buildUriTable", (TeamFoundationTableValueParameter<KeyValuePair<int, string>>) BuildSqlResourceComponent.UrisToOrderedStringTable(builds.Select<BuildDetail, string>((System.Func<BuildDetail, string>) (x => x.Uri))));
      if (informationTypes.Count > 1 || informationTypes[0] != BuildConstants.Star)
      {
        this.BindTable<string>("@informationTypeTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) informationTypes, false, 256));
        this.BindBoolean("@allTypes", false);
      }
      else
      {
        this.BindTable<string>("@informationTypeTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) null));
        this.BindBoolean("@allTypes", true);
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildInformationRow>((ObjectBinder<BuildInformationRow>) new BuildInformationBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (QueryBuildInformation));
      return resultCollection;
    }

    internal virtual ResultCollection UpdateBuildInformation(
      Guid projectId,
      string buildUri,
      ICollection<InformationChangeRequest> changes,
      TeamFoundationIdentity requestedBy)
    {
      this.TraceEnter(0, nameof (UpdateBuildInformation));
      this.PrepareStoredProcedure("prc_UpdateBuildInformation");
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
      resultCollection.AddBinder<BuildInformationRow>((ObjectBinder<BuildInformationRow>) new BuildInformationBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (UpdateBuildInformation));
      return resultCollection;
    }

    protected virtual SqlParameter BindBuildInformationChangeRequestTable(
      string parameterName,
      IEnumerable<InformationChangeRequest> rows)
    {
      int index = 0;
      return this.BindTable(parameterName, "typ_BuildInformationChangeRequestTable", (rows ?? Enumerable.Empty<InformationChangeRequest>()).Select<InformationChangeRequest, SqlDataRecord>((System.Func<InformationChangeRequest, SqlDataRecord>) (x => this.ConvertToSqlDataRecord(x, ++index))));
    }

    protected virtual SqlParameter BindBuildInformationFieldTable(
      string parameterName,
      IEnumerable<InformationField> rows)
    {
      return this.BindTable(parameterName, "typ_BuildInformationFieldTable", (rows ?? Enumerable.Empty<InformationField>()).Select<InformationField, SqlDataRecord>(new System.Func<InformationField, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(InformationChangeRequest row, int index)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildInformationComponent.typ_BuildInformationChangeRequestTable);
      sqlDataRecord.SetInt32(0, index);
      switch (row)
      {
        case InformationAddRequest informationAddRequest:
          sqlDataRecord.SetByte(1, (byte) 1);
          sqlDataRecord.SetInt32(2, informationAddRequest.NodeId);
          sqlDataRecord.SetString(3, informationAddRequest.NodeType);
          if (informationAddRequest.ParentId != 0)
            sqlDataRecord.SetInt32(4, informationAddRequest.ParentId);
          else
            sqlDataRecord.SetDBNull(4);
          sqlDataRecord.SetDBNull(5);
          break;
        case InformationEditRequest informationEditRequest:
          sqlDataRecord.SetByte(1, (byte) 2);
          sqlDataRecord.SetInt32(2, informationEditRequest.NodeId);
          sqlDataRecord.SetDBNull(3);
          sqlDataRecord.SetDBNull(4);
          sqlDataRecord.SetByte(5, (byte) informationEditRequest.Options);
          break;
        case InformationDeleteRequest informationDeleteRequest:
          sqlDataRecord.SetByte(1, (byte) 3);
          sqlDataRecord.SetInt32(2, informationDeleteRequest.NodeId);
          sqlDataRecord.SetDBNull(3);
          sqlDataRecord.SetDBNull(4);
          sqlDataRecord.SetDBNull(5);
          break;
      }
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(InformationField row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildInformationComponent.typ_BuildInformationFieldTable);
      sqlDataRecord.SetInt32(0, row.NodeId);
      sqlDataRecord.SetString(1, row.Name);
      sqlDataRecord.SetString(2, row.Value ?? string.Empty);
      return sqlDataRecord;
    }
  }
}
