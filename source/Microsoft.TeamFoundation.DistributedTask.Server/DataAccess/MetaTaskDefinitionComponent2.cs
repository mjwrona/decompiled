// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionComponent2 : MetaTaskDefinitionComponent
  {
    public override MetaTaskDefinitionData AddMetaTaskDefinition(
      Guid projectId,
      TaskGroup taskGroup)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddMetaTaskDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_AddMetaTaskDefinition", projectId);
        this.BindGuid("@definitionId", taskGroup.Id);
        this.BindString("@name", taskGroup.Name, 40, false, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", Guid.Parse(taskGroup.CreatedBy.Id));
        this.BindBinary("@metadataDocument", JsonUtility.Serialize((object) taskGroup), SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<MetaTaskDefinitionData>((ObjectBinder<MetaTaskDefinitionData>) new MetaTaskDefinitionDataBinder());
          return resultCollection.GetCurrent<MetaTaskDefinitionData>().Items.Single<MetaTaskDefinitionData>();
        }
      }
    }

    public override MetaTaskDefinitionData UpdateMetaTaskDefinition(
      Guid projectId,
      TaskGroup taskGroup,
      bool enablePriorVersionEdit)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateMetaTaskDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateMetaTaskDefinition", projectId);
        this.BindGuid("@definitionId", taskGroup.Id);
        this.BindString("@name", taskGroup.Name, 40, false, SqlDbType.NVarChar);
        this.BindInt("@revision", taskGroup.Revision);
        this.BindGuid("@modifiedBy", Guid.Parse(taskGroup.ModifiedBy.Id));
        this.BindBinary("@metadataDocument", JsonUtility.Serialize((object) taskGroup), SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<MetaTaskDefinitionData>((ObjectBinder<MetaTaskDefinitionData>) new MetaTaskDefinitionDataBinder());
          return resultCollection.GetCurrent<MetaTaskDefinitionData>().Items.Single<MetaTaskDefinitionData>();
        }
      }
    }
  }
}
