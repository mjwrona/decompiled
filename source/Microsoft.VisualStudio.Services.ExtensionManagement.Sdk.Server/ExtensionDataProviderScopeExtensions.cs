// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionDataProviderScopeExtensions
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public static class ExtensionDataProviderScopeExtensions
  {
    public static IDataProviderScope GetRequestScope(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ExtensionDataProviderScopeService>().GetScopeProvider(vssRequestContext)?.GetRequestScope(requestContext);
    }

    public static IDataProviderScope GetScope(
      IVssRequestContext requestContext,
      string scopeName,
      string scopeValue)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IDataProviderScopeProvider scopeProvider = vssRequestContext.GetService<IExtensionDataProviderScopeService>().GetScopeProvider(vssRequestContext);
      return scopeProvider != null && string.Equals(scopeProvider.ScopeName, scopeName, StringComparison.OrdinalIgnoreCase) ? scopeProvider.GetScope(requestContext, scopeValue) : (IDataProviderScope) new DefaultDataProviderScope(scopeName, scopeValue);
    }
  }
}
