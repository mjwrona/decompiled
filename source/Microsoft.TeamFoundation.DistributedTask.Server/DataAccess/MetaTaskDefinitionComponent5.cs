// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionComponent5
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionComponent5 : MetaTaskDefinitionComponent4
  {
    public override List<MetaTaskDefinitionData> GetMetaTaskDefinitions(
      Guid projectId,
      Guid? definitionId = null,
      Guid? taskIdFilter = null,
      bool deleted = false,
      DateTime? continuationToken = null,
      int top = 0,
      TaskGroupQueryOrder queryOrder = TaskGroupQueryOrder.CreatedOnDescending)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetMetaTaskDefinitions)))
      {
        this.PrepareStoredProcedure("Task.prc_GetMetaTaskDefinitions", projectId);
        this.BindNullableGuid("@definitionId", definitionId);
        this.BindNullableGuid("@taskIdFilter", taskIdFilter);
        this.BindBoolean("@deleted", deleted);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<MetaTaskDefinitionData>((ObjectBinder<MetaTaskDefinitionData>) new MetaTaskDefinitionDataBinder());
          return resultCollection.GetCurrent<MetaTaskDefinitionData>().Items;
        }
      }
    }

    public override void SoftDeleteTaskGroup(
      Guid projectId,
      Guid taskGroupId,
      Guid modifiedBy,
      string comment = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SoftDeleteTaskGroup)))
      {
        this.PrepareStoredProcedure("Task.prc_SoftDeleteTaskGroup", projectId);
        this.BindGuid("@definitionId", taskGroupId);
        this.BindGuid("@modifiedBy", modifiedBy);
        this.BindString("@comment", comment, 2048, true, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public override void UndeleteTaskGroup(
      Guid projectId,
      Guid taskGroupId,
      Guid modifiedBy,
      string comment = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UndeleteTaskGroup)))
      {
        this.PrepareStoredProcedure("Task.prc_UndeleteTaskGroup", projectId);
        this.BindGuid("@definitionId", taskGroupId);
        this.BindGuid("@modifiedBy", modifiedBy);
        this.BindString("@comment", comment, 2048, true, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public override IList<int> HardDeleteTaskGroups(int daysToRetain)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (HardDeleteTaskGroups)))
      {
        this.PrepareStoredProcedure("Task.prc_HardDeleteTaskGroups");
        this.BindGuid("@namespaceGuid", this.MetaTaskNamespaceId);
        this.BindInt("@daysToRetain", daysToRetain);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<MetaTaskDefinitionRevisionData>((ObjectBinder<MetaTaskDefinitionRevisionData>) new MetaTaskDefinitionRevisionDataBinder());
          return (IList<int>) resultCollection.GetCurrent<MetaTaskDefinitionRevisionData>().Items.Where<MetaTaskDefinitionRevisionData>((System.Func<MetaTaskDefinitionRevisionData, bool>) (i => i.FileId > 0)).Select<MetaTaskDefinitionRevisionData, int>((System.Func<MetaTaskDefinitionRevisionData, int>) (i => i.FileId)).ToList<int>();
        }
      }
    }
  }
}
