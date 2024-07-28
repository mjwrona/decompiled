// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.Migration.MetaTaskDocumentBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.Migration
{
  internal class MetaTaskDocumentBinder : ObjectBinder<MetaTaskDocumentData>
  {
    private SqlColumnBinder dataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder definitionIdColumn = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder revisionColumn = new SqlColumnBinder("Revision");
    private SqlColumnBinder metadataDocumentColumn = new SqlColumnBinder("MetadataDocument");

    protected override MetaTaskDocumentData Bind()
    {
      MetaTaskDocumentData taskDocumentData = new MetaTaskDocumentData()
      {
        DataspaceId = this.dataspaceIdColumn.GetInt32((IDataReader) this.Reader),
        DefinitionId = this.definitionIdColumn.GetGuid((IDataReader) this.Reader),
        Revision = this.revisionColumn.GetInt32((IDataReader) this.Reader, 1, 1),
        MetadataDocument = this.metadataDocumentColumn.GetBytes((IDataReader) this.Reader, false)
      };
      taskDocumentData.MetaTaskDocument = JsonConvert.SerializeObject((object) JsonUtility.Deserialize<TaskGroup>(taskDocumentData.MetadataDocument));
      return taskDocumentData;
    }
  }
}
