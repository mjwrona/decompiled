// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNoteComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CheckinNoteComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<CheckinNoteComponent>(1, true),
      (IComponentCreator) new ComponentCreator<CheckinNoteComponent2>(2),
      (IComponentCreator) new ComponentCreator<CheckinNoteComponent3>(3)
    }, "VCCheckinNote");
    private static readonly SqlMetaData[] typ_ReleaseNoteDefinition = new SqlMetaData[3]
    {
      new SqlMetaData("FieldName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("Required", SqlDbType.Bit),
      new SqlMetaData("DisplayOrder", SqlDbType.Int)
    };

    protected SqlParameter BindCheckinNoteFieldDefinitionTable(
      string parameterName,
      IEnumerable<CheckinNoteFieldDefinition> rows)
    {
      rows = rows ?? Enumerable.Empty<CheckinNoteFieldDefinition>();
      System.Func<CheckinNoteFieldDefinition, SqlDataRecord> selector = (System.Func<CheckinNoteFieldDefinition, SqlDataRecord>) (definition =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CheckinNoteComponent.typ_ReleaseNoteDefinition);
        sqlDataRecord.SetString(0, definition.Name);
        sqlDataRecord.SetBoolean(1, definition.Required);
        sqlDataRecord.SetInt32(2, definition.DisplayOrder);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ReleaseNoteDefinition", rows.Select<CheckinNoteFieldDefinition, SqlDataRecord>(selector));
    }

    public virtual void CreateDefinition(
      string associatedServerItem,
      CheckinNoteFieldDefinition[] checkinNoteFields)
    {
      this.PrepareStoredProcedure("prc_CreateReleaseNoteDefinition");
      this.BindServerItem("@associatedItem", associatedServerItem, false);
      this.BindCheckinNoteFieldDefinitionTable("@definitionList", (IEnumerable<CheckinNoteFieldDefinition>) checkinNoteFields);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection QueryDefinition(List<string> associatedServerItemList)
    {
      List<string> rows = new List<string>(associatedServerItemList.Count);
      foreach (string associatedServerItem in associatedServerItemList)
        rows.Add(DBPath.ServerToDatabasePath(associatedServerItem));
      this.PrepareStoredProcedure("prc_QueryReleaseNoteDefinition");
      this.BindStringTable("@associatedItems", (IEnumerable<string>) rows);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<CheckinNoteFieldDefinition>((ObjectBinder<CheckinNoteFieldDefinition>) new CheckinNoteFieldDefinitionColumns());
      return resultCollection;
    }

    public void UpdateCheckinNoteFieldName(string existingFieldName, string newFieldName)
    {
      this.PrepareStoredProcedure("prc_UpdateReleaseNoteFieldDefinition");
      this.BindString("@existingFieldName", existingFieldName, 64, false, SqlDbType.NVarChar);
      this.BindString("@newFieldName", newFieldName, 64, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public ResultCollection QueryCheckinNoteFieldNames()
    {
      this.PrepareStoredProcedure("prc_QueryReleaseNoteFieldNames");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<string>((ObjectBinder<string>) new CheckinNoteFieldNameColumns());
      return resultCollection;
    }
  }
}
