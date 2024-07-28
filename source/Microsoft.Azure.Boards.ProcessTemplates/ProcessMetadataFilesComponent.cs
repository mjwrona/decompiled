// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessMetadataFilesComponent
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessMetadataFilesComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly SqlMetaData[] typ_ProcessMetadataTable = new SqlMetaData[4]
    {
      new SqlMetaData("ProcessTypeId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ResourceType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ResourceName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("FileId", SqlDbType.Int)
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<ProcessMetadataFilesComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ProcessMetadataFilesComponent2>(2)
    }, "ProcessMetadataFiles");

    protected SqlParameter BindMetadataFileTable(
      string parameterName,
      IEnumerable<ProcessMetadataFileEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<ProcessMetadataFileEntry>();
      System.Func<ProcessMetadataFileEntry, SqlDataRecord> selector = (System.Func<ProcessMetadataFileEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProcessMetadataFilesComponent.typ_ProcessMetadataTable);
        sqlDataRecord.SetGuid(0, entry.ProcessTypeId);
        sqlDataRecord.SetString(1, entry.ResourceType);
        sqlDataRecord.SetString(2, entry.ResourceName);
        sqlDataRecord.SetSqlInt32(3, (SqlInt32) entry.FileId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ProcessMetadataTable", rows.Select<ProcessMetadataFileEntry, SqlDataRecord>(selector));
    }

    public virtual void AddUpdateMetadataFiles(ProcessMetadataFileEntry[] metadataFiles)
    {
      this.PrepareStoredProcedure("prc_AddUpdateProcessMetadataFiles");
      this.BindMetadataFileTable("@metadataFiles", (IEnumerable<ProcessMetadataFileEntry>) metadataFiles);
      this.ExecuteNonQuery();
    }

    public virtual int? GetProcessMetadataFileId(
      Guid processTypeId,
      string resourceType,
      string resourceName)
    {
      this.PrepareStoredProcedure("prc_GetProcessMetadataFileId");
      this.BindGuid("@processTypeId", processTypeId);
      this.BindString("@resourceType", resourceType, 256, false, SqlDbType.NVarChar);
      this.BindString("@resourceName", resourceName, 256, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessMetadataFileEntry>((ObjectBinder<ProcessMetadataFileEntry>) new ProcessMetadataFilesComponent.ProcessMetadataFilesComponentBinder());
      List<ProcessMetadataFileEntry> items = resultCollection.GetCurrent<ProcessMetadataFileEntry>().Items;
      return items == null || items.Count == 0 ? new int?() : new int?(items[0].FileId);
    }

    public virtual IEnumerable<ProcessMetadataFileEntry> GetProcessMetadataFiles(
      ProcessMetadataFileEntry[] metadataFiles)
    {
      this.PrepareStoredProcedure("prc_GetProcessMetadataFiles");
      this.BindMetadataFileTable("@metadataFiles", (IEnumerable<ProcessMetadataFileEntry>) metadataFiles);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessMetadataFileEntry>((ObjectBinder<ProcessMetadataFileEntry>) new ProcessMetadataFilesComponent.ProcessMetadataFilesComponentBinder());
      return (IEnumerable<ProcessMetadataFileEntry>) resultCollection.GetCurrent<ProcessMetadataFileEntry>().Items;
    }

    public virtual IEnumerable<ProcessMetadataFileEntry> GetAllProcessMetadataFiles() => (IEnumerable<ProcessMetadataFileEntry>) null;

    protected class ProcessMetadataFilesComponentBinder : ObjectBinder<ProcessMetadataFileEntry>
    {
      private SqlColumnBinder m_ProcessTypeId = new SqlColumnBinder("ProcessTypeId");
      private SqlColumnBinder m_ResourceType = new SqlColumnBinder("ResourceType");
      private SqlColumnBinder m_ResourceName = new SqlColumnBinder("ResourceName");
      private SqlColumnBinder m_FileId = new SqlColumnBinder("FileId");

      protected override ProcessMetadataFileEntry Bind() => new ProcessMetadataFileEntry()
      {
        ProcessTypeId = this.m_ProcessTypeId.GetGuid((IDataReader) this.Reader),
        ResourceType = this.m_ResourceType.GetString((IDataReader) this.Reader, false),
        ResourceName = this.m_ResourceName.GetString((IDataReader) this.Reader, false),
        FileId = this.m_FileId.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
