// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionComponent7
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionComponent7 : MetaTaskDefinitionComponent6
  {
    public override MetaTaskDefinitionData AddMetaTaskDefinition(
      Guid projectId,
      TaskGroup taskGroup)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddMetaTaskDefinition)))
      {
        this.PrepareStoredProcedure("Task.prc_AddMetaTaskDefinition", projectId);
        this.BindGuid("@definitionId", taskGroup.Id);
        this.BindString("@name", taskGroup.Name, 128, false, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", Guid.Parse(taskGroup.CreatedBy.Id));
        this.BindNullableGuid("@parentDefinitionId", taskGroup.ParentDefinitionId);
        this.BindString("@metaTaskDocument", JsonConvert.SerializeObject((object) taskGroup), -1, false, SqlDbType.NVarChar);
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
        this.BindString("@name", taskGroup.Name, 128, false, SqlDbType.NVarChar);
        this.BindInt("@revision", taskGroup.Revision);
        this.BindGuid("@modifiedBy", Guid.Parse(taskGroup.ModifiedBy.Id));
        this.BindInt("@majorVersion", taskGroup.Version.Major);
        this.BindString("@metaTaskDocument", JsonConvert.SerializeObject((object) taskGroup), -1, false, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<MetaTaskDefinitionData>((ObjectBinder<MetaTaskDefinitionData>) new MetaTaskDefinitionDataBinder());
          return resultCollection.GetCurrent<MetaTaskDefinitionData>().Items.Single<MetaTaskDefinitionData>();
        }
      }
    }

    public override MetaTaskDefinitionData UpdateMetaTaskDefinitionAndDisableOldVersions(
      Guid projectId,
      TaskGroup taskGroup,
      bool disablePriorVersions,
      bool enablePriorVersionEdit,
      out List<int> disabledVersions)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateMetaTaskDefinitionAndDisableOldVersions)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateMetaTaskDefinition", projectId);
        this.BindGuid("@definitionId", taskGroup.Id);
        this.BindString("@name", taskGroup.Name, 128, false, SqlDbType.NVarChar);
        this.BindInt("@revision", taskGroup.Revision);
        this.BindGuid("@modifiedBy", Guid.Parse(taskGroup.ModifiedBy.Id));
        this.BindInt("@majorVersion", taskGroup.Version.Major);
        this.BindBoolean("@preview", taskGroup.Preview);
        this.BindBoolean("@disablePriorVersions", disablePriorVersions);
        this.BindString("@metaTaskDocument", JsonConvert.SerializeObject((object) taskGroup), -1, false, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<MetaTaskDefinitionData>((ObjectBinder<MetaTaskDefinitionData>) new MetaTaskDefinitionDataBinder());
          MetaTaskDefinitionData taskDefinitionData = resultCollection.GetCurrent<MetaTaskDefinitionData>().Items.Single<MetaTaskDefinitionData>();
          if (disablePriorVersions)
          {
            resultCollection.AddBinder<int>((ObjectBinder<int>) new MajorVersionBinder());
            resultCollection.NextResult();
            disabledVersions = resultCollection.GetCurrent<int>().Items;
          }
          else
            disabledVersions = new List<int>();
          return taskDefinitionData;
        }
      }
    }
  }
}
