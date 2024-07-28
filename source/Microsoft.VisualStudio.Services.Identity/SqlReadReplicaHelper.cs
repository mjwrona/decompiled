// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SqlReadReplicaHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class SqlReadReplicaHelper
  {
    private const string Area = "IdentityService";
    private const string Layer = "SqlReadReplicaHelper";
    private static readonly TimeSpan s_defaultCircuitBreakerTimeout = TimeSpan.FromMinutes(5.0);

    internal static void ExecuteReadReplicaAwareOperation<TComponent>(
      IVssRequestContext requestContext,
      bool shouldReadFromReplica,
      Action<TComponent, bool> operation)
      where TComponent : TeamFoundationSqlResourceComponent, new()
    {
      if (!requestContext.IsFeatureEnabled(FrameworkServerConstants.ReadOnlySqlComponent) || !shouldReadFromReplica)
      {
        requestContext.Trace(2432351, TraceLevel.Info, "IdentityService", nameof (SqlReadReplicaHelper), string.Format("Reading group memberships from read-write replica. Component={0}", (object) typeof (TComponent)));
        TComponent component = requestContext.CreateComponent<TComponent>();
        try
        {
          operation(component, true);
        }
        finally
        {
          if ((object) component != null)
            ((IDisposable) component).Dispose();
        }
      }
      else
      {
        requestContext.Trace(2432351, TraceLevel.Info, "IdentityService", nameof (SqlReadReplicaHelper), string.Format("Reading group memberships from read-only replica. Component={0}", (object) typeof (TComponent)));
        string key = typeof (TComponent)?.ToString() + "-" + requestContext.FrameworkConnectionInfo.InitialCatalog + "-" + ApplicationIntent.ReadOnly.ToString();
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) key).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(SqlReadReplicaHelper.s_defaultCircuitBreakerTimeout).WithExecutionMaxConcurrentRequests(int.MaxValue));
        CommandPropertiesRegistry properties = new CommandPropertiesRegistry(requestContext, (CommandKey) key, setter.CommandPropertiesDefaults);
        new CommandService(requestContext, setter, (ICommandProperties) properties, (Action) (() =>
        {
          TComponent component = requestContext.CreateComponent<TComponent>(connectionType: new DatabaseConnectionType?(DatabaseConnectionType.IntentReadOnly));
          try
          {
            operation(component, false);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(2432356, TraceLevel.Info, "IdentityService", nameof (SqlReadReplicaHelper), ex, "Reading from replica failed, falling back to primary.");
            throw;
          }
          finally
          {
            if ((object) component != null)
              ((IDisposable) component).Dispose();
          }
        }), (Action) (() =>
        {
          TComponent component = requestContext.CreateComponent<TComponent>();
          try
          {
            operation(component, true);
          }
          finally
          {
            if ((object) component != null)
              ((IDisposable) component).Dispose();
          }
        })).Execute();
      }
    }
  }
}
