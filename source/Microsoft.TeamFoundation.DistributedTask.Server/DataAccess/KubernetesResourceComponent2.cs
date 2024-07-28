// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.KubernetesResourceComponent2
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
  internal class KubernetesResourceComponent2 : KubernetesResourceComponent
  {
    public override IList<KubernetesResource> DeleteEnvironmentResources(
      Guid projectId,
      int environmentId,
      Guid deletedBy)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteEnvironmentResources)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAllKubernetesResources");
        this.BindInt("@environmentId", environmentId);
        this.BindGuid("@deletedBy", deletedBy);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<KubernetesResource>((ObjectBinder<KubernetesResource>) new KubernetesResourceBinder());
          return (IList<KubernetesResource>) resultCollection.GetCurrent<KubernetesResource>().Items;
        }
      }
    }
  }
}
