// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class CounterComponent2 : CounterComponent
  {
    public override async Task<int> IncrementCounterAsync(
      Guid scopeId,
      string planType,
      int definitionId,
      Guid planId,
      string prefix,
      int seed)
    {
      CounterComponent2 counterComponent2 = this;
      counterComponent2.PrepareStoredProcedure("Task.prc_IncrementCounter");
      counterComponent2.BindDefaultDataspaceId(scopeId);
      counterComponent2.BindInt("@definitionId", definitionId);
      counterComponent2.BindString("@planType", planType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      counterComponent2.BindNullableGuid("@planId", new Guid?(planId), true);
      counterComponent2.BindString("@prefix", prefix, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      counterComponent2.BindInt("@seed", seed);
      return (int) await counterComponent2.ExecuteScalarAsync();
    }
  }
}
