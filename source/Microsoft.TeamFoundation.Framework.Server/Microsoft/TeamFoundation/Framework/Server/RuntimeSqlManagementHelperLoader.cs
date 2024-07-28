// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RuntimeSqlManagementHelperLoader
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class RuntimeSqlManagementHelperLoader
  {
    private static string s_typeName = "Microsoft.VisualStudio.Services.Cloud.RuntimeSqlManagementHelper";

    public static IRuntimeSqlManagementHelper Default => (IRuntimeSqlManagementHelper) new DefaultRuntimeSqlManagementHelper();

    public static IRuntimeSqlManagementHelper LoadRuntimeSqlManagementHelper()
    {
      Type type = (Type) null;
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        type = assembly.GetType(RuntimeSqlManagementHelperLoader.s_typeName);
        if (type != (Type) null)
          break;
      }
      return type == (Type) null ? (IRuntimeSqlManagementHelper) new DefaultRuntimeSqlManagementHelper() : (IRuntimeSqlManagementHelper) Activator.CreateInstance(type);
    }

    public static void InitializeManagementSettingsForDeploymentContext(
      IVssRequestContext requestContext,
      string subscriptionId,
      string resourceManagerAadTenantId,
      string resourceManagerEndpointUrl,
      string resourceGroupName)
    {
      requestContext.Items[RequestContextItemsKeys.AzureSubscriptionId] = string.IsNullOrEmpty(subscriptionId) ? (object) Guid.Empty : (object) new Guid(subscriptionId);
      requestContext.Items[RequestContextItemsKeys.ResourceManagerAadTenantId] = (object) resourceManagerAadTenantId;
      requestContext.Items[RequestContextItemsKeys.ResourceManagementUrl] = (object) resourceManagerEndpointUrl;
      requestContext.Items[RequestContextItemsKeys.ResourceGroupName] = (object) resourceGroupName;
    }
  }
}
