// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionComponent9
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionComponent9 : MetaTaskDefinitionComponent8
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
        this.BindNullableDateTime("@continuationToken", continuationToken);
        this.BindInt("@top", top);
        this.BindByte("@queryOrder", (byte) queryOrder);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<MetaTaskDefinitionData>((ObjectBinder<MetaTaskDefinitionData>) new MetaTaskDefinitionDataBinder());
          return resultCollection.GetCurrent<MetaTaskDefinitionData>().Items;
        }
      }
    }
  }
}
