// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionDataProviderScopeService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ExtensionDataProviderScopeService : 
    IExtensionDataProviderScopeService,
    IVssFrameworkService
  {
    private IDataProviderScopeProvider m_scopeProvider;
    private const string s_area = "ContributionService";
    private const string s_layer = "ExtensionDataProviderScopeService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10015521, "ContributionService", nameof (ExtensionDataProviderScopeService), nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        using (IDisposableReadOnlyList<IDataProviderScopeProvider> extensions = systemRequestContext.GetExtensions<IDataProviderScopeProvider>(throwOnError: true))
          this.m_scopeProvider = extensions.Count<IDataProviderScopeProvider>() <= 1 ? extensions.FirstOrDefault<IDataProviderScopeProvider>() : throw new InvalidOperationException("Only one data provider scope provider can be deployed to a service.");
      }
      finally
      {
        systemRequestContext.TraceEnter(10015521, "ContributionService", nameof (ExtensionDataProviderScopeService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IDataProviderScopeProvider GetScopeProvider(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      return this.m_scopeProvider;
    }
  }
}
