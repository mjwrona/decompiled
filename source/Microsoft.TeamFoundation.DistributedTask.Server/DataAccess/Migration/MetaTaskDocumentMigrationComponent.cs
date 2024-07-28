// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.Migration.MetaTaskDocumentMigrationComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.Migration
{
  internal class MetaTaskDocumentMigrationComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<MetaTaskDocumentMigrationComponent>(1)
    }, "DistributedTaskMetaTaskDocumentMigration", "DistributedTask");
    private static readonly SqlMetaData[] MetaTaskDocumentSqlData = new SqlMetaData[4]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("DefinitionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Revision", SqlDbType.Int),
      new SqlMetaData("MetaTaskDocument", SqlDbType.NVarChar, -1L)
    };

    public MetaTaskDocumentMigrationComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public IList<MetaTaskDocumentData> GetMetaTaskMetadataDocumentMigrationData()
    {
      this.PrepareStoredProcedure("Task.prc_GetMetaTaskMetadataDocumentMigrationData");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, (IVssRequestContext) null))
      {
        resultCollection.AddBinder<MetaTaskDocumentData>((ObjectBinder<MetaTaskDocumentData>) new MetaTaskDocumentBinder());
        return (IList<MetaTaskDocumentData>) resultCollection.GetCurrent<MetaTaskDocumentData>().Items;
      }
    }

    public void BulkUpdateMetaTaskDocument(IEnumerable<MetaTaskDocumentData> updatedData)
    {
      this.PrepareStoredProcedure("Task.prc_BulkUpdateMetaTaskDocument");
      this.BindTable("metaTaskData", "Task.typ_MetaTaskDocumentTable", this.GetSqlDataRecords(updatedData));
      this.ExecuteNonQuery();
    }

    private IEnumerable<SqlDataRecord> GetSqlDataRecords(IEnumerable<MetaTaskDocumentData> rows)
    {
      List<SqlDataRecord> sqlDataRecords = new List<SqlDataRecord>();
      if (rows == null)
        return (IEnumerable<SqlDataRecord>) sqlDataRecords;
      foreach (MetaTaskDocumentData taskDocumentData in rows.Where<MetaTaskDocumentData>((System.Func<MetaTaskDocumentData, bool>) (r => r != null)))
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(MetaTaskDocumentMigrationComponent.MetaTaskDocumentSqlData);
        sqlDataRecord.SetInt32(0, taskDocumentData.DataspaceId);
        sqlDataRecord.SetGuid(1, taskDocumentData.DefinitionId);
        sqlDataRecord.SetInt32(2, taskDocumentData.Revision);
        sqlDataRecord.SetString(3, taskDocumentData.MetaTaskDocument);
        sqlDataRecords.Add(sqlDataRecord);
      }
      return (IEnumerable<SqlDataRecord>) sqlDataRecords;
    }
  }
}
