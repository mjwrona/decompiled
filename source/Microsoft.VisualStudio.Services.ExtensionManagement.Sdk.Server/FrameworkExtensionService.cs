// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FrameworkExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class FrameworkExtensionService : IExtensionService, IVssFrameworkService
  {
    private static readonly string s_area = "FrameworkExtensionStateService";
    private static readonly string s_layer = "IVssFrameworkService";
    private static readonly RegistryQuery s_longTimeoutQuery = new RegistryQuery("/Configuration/ExtensionService/LongTimeout");
    private static readonly RegistryQuery s_shortTimeoutQuery = new RegistryQuery("/Configuration/ExtensionService/ShortTimeout");
    private const int c_defaultShortTimeout = 8000;
    private const int c_defaultLongTimeout = 10000;

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public List<ExtensionState> GetExtensionStates(
      IVssRequestContext requestContext,
      bool includeDisabledExtensions,
      bool includeErrors,
      bool forceRefresh)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        requestContext.Trace(10013300, TraceLevel.Info, FrameworkExtensionService.s_area, FrameworkExtensionService.s_layer, "Extensions are only available at the collection level.  Attempt to access them from following code path: {0}", (object) Environment.StackTrace);
        return new List<ExtensionState>();
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      ExtensionManagementHttpClient client = requestContext.Elevate().GetClient<ExtensionManagementHttpClient>(this.GetHttpClientOptionsForEventualReadConsistencyLevel(requestContext));
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        linkedTokenSource.CancelAfter(service.GetValue<int>(vssRequestContext, in FrameworkExtensionService.s_shortTimeoutQuery, 8000));
        try
        {
          return client.GetStatesAsync(new bool?(includeDisabledExtensions), new bool?(includeErrors), forceRefresh: new bool?(forceRefresh), cancellationToken: linkedTokenSource.Token).SyncResult<List<ExtensionState>>();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013296, FrameworkExtensionService.s_area, FrameworkExtensionService.s_layer, ex);
          throw;
        }
      }
    }

    public List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      bool? includeDisabledExtensions = null,
      bool? includeErrors = null,
      IEnumerable<string> assetTypes = null,
      bool? includeInstallationIssues = null)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(10013305, TraceLevel.Info, FrameworkExtensionService.s_area, FrameworkExtensionService.s_layer, "Extensions are not available at deployment level.  Attempt to access them from following code path: {0}", (object) Environment.StackTrace);
        return new List<InstalledExtension>();
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      ExtensionManagementHttpClient client = requestContext.Elevate().GetClient<ExtensionManagementHttpClient>(this.GetHttpClientOptionsForEventualReadConsistencyLevel(requestContext));
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        linkedTokenSource.CancelAfter(service.GetValue<int>(vssRequestContext, in FrameworkExtensionService.s_shortTimeoutQuery, 8000));
        try
        {
          return client.GetInstalledExtensionsAsync(includeDisabledExtensions, includeErrors, assetTypes, includeInstallationIssues, cancellationToken: linkedTokenSource.Token).SyncResult<List<InstalledExtension>>();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013296, FrameworkExtensionService.s_area, FrameworkExtensionService.s_layer, ex);
          throw;
        }
      }
    }

    public List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      InstalledExtensionQuery extensionQuery)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(10013301, TraceLevel.Info, FrameworkExtensionService.s_area, FrameworkExtensionService.s_layer, "Extensions are not available at deployment level.  Attempt to access them from following code path: {0}", (object) Environment.StackTrace);
        return new List<InstalledExtension>();
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      ExtensionManagementHttpClient client = requestContext.Elevate().GetClient<ExtensionManagementHttpClient>(this.GetHttpClientOptionsForEventualReadConsistencyLevel(requestContext));
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        linkedTokenSource.CancelAfter(service.GetValue<int>(vssRequestContext, in FrameworkExtensionService.s_longTimeoutQuery, 10000));
        try
        {
          return client.QueryExtensionsAsync(extensionQuery, cancellationToken: linkedTokenSource.Token).SyncResult<List<InstalledExtension>>();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013296, FrameworkExtensionService.s_area, FrameworkExtensionService.s_layer, ex);
          throw;
        }
      }
    }

    private VssHttpClientOptions GetHttpClientOptionsForEventualReadConsistencyLevel(
      IVssRequestContext requestContext)
    {
      VssHttpClientOptions consistencyLevel = (VssHttpClientOptions) null;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.EnableExtensionsGetHttpClientOptionsForEventualReadConsistencyLevel") && this.IsVssReadConsistencyLevelEventual(requestContext))
      {
        consistencyLevel = requestContext.GetHttpClientOptionsForEventualReadConsistencyLevel("VisualStudio.Services.EnableExtensionsGetHttpClientOptionsForEventualReadConsistencyLevel");
        requestContext.Trace(10013302, TraceLevel.Info, FrameworkExtensionService.s_area, FrameworkExtensionService.s_layer, "Initialized VssHttpClientOptions with VssReadConsistencyLevel.Eventual");
      }
      return consistencyLevel;
    }

    private bool IsVssReadConsistencyLevelEventual(IVssRequestContext requestContext)
    {
      VssReadConsistencyLevel consistencyLevel;
      return !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.Items.TryGetValue<VssReadConsistencyLevel>("ReadConsistencyLevel", out consistencyLevel) && consistencyLevel == VssReadConsistencyLevel.Eventual;
    }
  }
}
