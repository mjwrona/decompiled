// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementStatisticsComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementStatisticsComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<IdentityManagementStatisticsComponent>(1),
      (IComponentCreator) new ComponentCreator<IdentityManagementStatisticsComponent2>(2)
    }, "IdentityManagementStatistics");

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "IdentityManagementStatisticsComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) IdentityManagementStatisticsComponent.s_sqlExceptionFactories;

    public int GetNumberOfUsers()
    {
      this.PrepareStoredProcedure("prc_GetNumberOfUsers");
      return (int) this.ExecuteScalar();
    }

    public virtual long GetMaxSequenceIdOfIdentity()
    {
      this.TraceEnter(47011200, nameof (GetMaxSequenceIdOfIdentity));
      try
      {
        this.PrepareStoredProcedure("prc_GetIdentityMaxSequenceId", false);
        return (long) this.ExecuteScalar();
      }
      finally
      {
        this.TraceLeave(47011209, nameof (GetMaxSequenceIdOfIdentity));
      }
    }

    public virtual void SetMaxSequenceIdOfIdentity(long maxSequenceId)
    {
      this.TraceEnter(47011210, nameof (SetMaxSequenceIdOfIdentity));
      try
      {
        this.PrepareStoredProcedure("prc_SetIdentityMaxSequenceId");
        this.BindLong("@maxSequenceId", maxSequenceId);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(47011219, nameof (SetMaxSequenceIdOfIdentity));
      }
    }
  }
}
