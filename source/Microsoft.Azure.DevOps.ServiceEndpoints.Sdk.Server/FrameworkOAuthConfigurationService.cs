// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.FrameworkOAuthConfigurationService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class FrameworkOAuthConfigurationService : 
    IOAuthConfigurationService2,
    IVssFrameworkService
  {
    private const string c_layer = "FrameworkOAuthConfigurationService";

    public AuthConfiguration GetAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkOAuthConfigurationService), nameof (GetAuthConfiguration)))
        return requestContext.GetClient<ServiceEndpointHttpClient>().GetOAuthConfigurationAsync(configurationId, (object) requestContext).SyncResult<OAuthConfiguration>().ConvertToAuthConfiguration();
    }

    public void ReadAuthConfigurationSecrets(
      IVssRequestContext requestContext,
      AuthConfiguration configuration)
    {
      throw new NotImplementedException();
    }

    public IList<AuthConfiguration> GetAuthConfigurationsByIds(
      IVssRequestContext requestContext,
      IList<Guid> configurationsList)
    {
      throw new NotImplementedException();
    }

    public AuthConfiguration CreateAuthConfiguration(
      IVssRequestContext requestContext,
      OAuthConfigurationParams configurationParams)
    {
      using (new MethodScope(requestContext, nameof (FrameworkOAuthConfigurationService), nameof (CreateAuthConfiguration)))
        return requestContext.GetClient<ServiceEndpointHttpClient>().CreateOAuthConfigurationAsync(configurationParams, (object) requestContext).SyncResult<OAuthConfiguration>().ConvertToAuthConfiguration();
    }

    public IList<AuthConfiguration> GetAuthConfigurations(
      IVssRequestContext requestContext,
      string endpointType,
      OAuthConfigurationActionFilter actionFilter = OAuthConfigurationActionFilter.None)
    {
      throw new NotImplementedException();
    }

    public AuthConfiguration DeleteAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configurationId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkOAuthConfigurationService), nameof (DeleteAuthConfiguration)))
        return requestContext.GetClient<ServiceEndpointHttpClient>().DeleteOAuthConfigurationAsync(configurationId, (object) requestContext).SyncResult<OAuthConfiguration>().ConvertToAuthConfiguration();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
