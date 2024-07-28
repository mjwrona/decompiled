// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions.TeamFoundationRequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions
{
  public static class TeamFoundationRequestContextExtensions
  {
    public static TReturn ExecuteWithinUsingWithComponent<TComponent, TReturn>(
      this IVssRequestContext requestContext,
      string operationName,
      Func<TComponent, TReturn> action)
      where TComponent : class, ISqlResourceComponent, new()
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<TComponent, TReturn>>(action, nameof (action));
      DatabaseConnectionType? connectionType = requestContext.IsEventualReadConsistencyLevel(operationName) ? new DatabaseConnectionType?(DatabaseConnectionType.IntentReadOnly) : new DatabaseConnectionType?();
      return requestContext.ExecuteWithinUsingWithComponent<TComponent, TReturn>(action, connectionType);
    }

    public static TReturn ExecuteWithinUsingWithComponent<TComponent, TReturn>(
      this IVssRequestContext requestContext,
      Func<TComponent, TReturn> action,
      DatabaseConnectionType? connectionType = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Func<TComponent, TReturn>>(action, nameof (action));
      TComponent component = TeamFoundationRequestContextExtensions.CreateComponent<TComponent>(requestContext, connectionType);
      try
      {
        return action(component);
      }
      finally
      {
        if ((object) component != null)
          component.Dispose();
      }
    }

    public static void ExecuteWithinUsingWithComponent<TComponent>(
      this IVssRequestContext requestContext,
      Action<TComponent> action)
      where TComponent : class, ISqlResourceComponent, new()
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Action<TComponent>>(action, nameof (action));
      TComponent component = TeamFoundationRequestContextExtensions.CreateComponent<TComponent>(requestContext);
      try
      {
        action(component);
      }
      finally
      {
        if ((object) component != null)
          component.Dispose();
      }
    }

    private static TComponent CreateComponent<TComponent>(
      IVssRequestContext requestContext,
      DatabaseConnectionType? connectionType = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      return connectionType.HasValue ? requestContext.CreateComponent<TComponent>(connectionType: connectionType) : requestContext.CreateComponent<TComponent>();
    }

    public static TReturn ExecuteAsyncAndGetResult<TReturn>(
      this IVssRequestContext requestContext,
      Func<Task<TReturn>> func)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (func == null)
        throw new ArgumentNullException(nameof (func));
      return func().GetResult<TReturn>(requestContext.CancellationToken);
    }

    public static TReturn ExecuteAsyncAndSyncResult<TReturn>(
      this IVssRequestContext requestContext,
      Func<Task<TReturn>> func)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (func == null)
        throw new ArgumentNullException(nameof (func));
      return func().SyncResult<TReturn>(requestContext.CancellationToken);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public static TReturn GetRegistryKeyValue<TReturn>(
      this IVssRequestContext requestContext,
      string registryKey,
      TReturn defaultValue = null)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrWhiteSpace(registryKey))
        throw new ArgumentNullException(nameof (registryKey));
      return requestContext.GetService<IVssRegistryService>().GetValue<TReturn>(requestContext, (RegistryQuery) registryKey, defaultValue);
    }

    public static bool IsMicrosoftTenant(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.GetOrganizationAadTenantId() == new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47");
    }

    public static T RunWithReadConsistencyLevel<T>(
      this IVssRequestContext context,
      string operationName,
      VssReadConsistencyLevel level,
      Func<IVssRequestContext, T> action)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<Func<IVssRequestContext, T>>(action, nameof (action));
      string consistencyLevelKey = TeamFoundationRequestContextExtensions.CreateReadConsistencyLevelKey(operationName);
      IDictionary<string, object> items = context.Items;
      bool flag = items.ContainsKey(consistencyLevelKey);
      object obj = flag ? items[consistencyLevelKey] : (object) null;
      items[consistencyLevelKey] = (object) level;
      try
      {
        return action(context);
      }
      finally
      {
        if (flag)
          items[consistencyLevelKey] = obj;
        else
          items.Remove(consistencyLevelKey);
      }
    }

    private static bool IsEventualReadConsistencyLevel(
      this IVssRequestContext context,
      string operationName)
    {
      VssReadConsistencyLevel? consistencyLevel1 = context.GetReadConsistencyLevel(operationName);
      VssReadConsistencyLevel consistencyLevel2 = VssReadConsistencyLevel.Eventual;
      return consistencyLevel1.GetValueOrDefault() == consistencyLevel2 & consistencyLevel1.HasValue;
    }

    private static VssReadConsistencyLevel? GetReadConsistencyLevel(
      this IVssRequestContext context,
      string operationName)
    {
      if (string.IsNullOrEmpty(operationName))
        return new VssReadConsistencyLevel?();
      string consistencyLevelKey = TeamFoundationRequestContextExtensions.CreateReadConsistencyLevelKey(operationName);
      VssReadConsistencyLevel consistencyLevel;
      return context.Items.TryGetValue<VssReadConsistencyLevel>(consistencyLevelKey, out consistencyLevel) ? new VssReadConsistencyLevel?(consistencyLevel) : new VssReadConsistencyLevel?();
    }

    private static string CreateReadConsistencyLevelKey(string operationName)
    {
      string consistencyLevelKey = "ReadConsistencyLevel";
      if (!string.IsNullOrEmpty(operationName))
        consistencyLevelKey = consistencyLevelKey + "_" + operationName;
      return consistencyLevelKey;
    }
  }
}
