// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMembershipTransferComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityMembershipTransferComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<IdentityMembershipTransferComponent>(1)
    }, "IdentityTransfer");

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = nameof (IdentityMembershipTransferComponent);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
      int num = requestContext.IsSystemContext ? 10 : int.MaxValue;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) (key + "-" + this.InitialCatalog + "-" + this.ApplicationIntent.ToString()));
      this.ComponentLevelCommandSetter.CommandPropertiesDefaults.WithExecutionMaxConcurrentRequests(num);
    }

    public List<KeyValuePair<Guid, Guid>> ReadIdentitiesFromTransferQueue()
    {
      this.PrepareStoredProcedure("prc_ReadIdentityTransferQueue");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<KeyValuePair<Guid, Guid>>((ObjectBinder<KeyValuePair<Guid, Guid>>) new IdentitiesToTransferColumns());
        return resultCollection.GetCurrent<KeyValuePair<Guid, Guid>>().Items;
      }
    }

    public void RemoveFromTransferQueue(IEnumerable<Guid> identitiesToRemove)
    {
      this.PrepareStoredProcedure("prc_DeleteFromIdentityTransferQueue");
      this.BindGuidTable("@ids", identitiesToRemove);
      this.ExecuteNonQuery();
    }
  }
}
