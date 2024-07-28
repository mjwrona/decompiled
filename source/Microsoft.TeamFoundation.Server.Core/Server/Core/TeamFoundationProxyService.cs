// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationProxyService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class TeamFoundationProxyService : ITeamFoundationProxyService, IVssFrameworkService
  {
    private static readonly Guid s_oauthAreaId = new Guid("585028FE-17D8-49E2-9A1B-EFB4D8502156");

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.TeamFoundation.Core.WebApi.Proxy AddProxy(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.Proxy proxy)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Core.WebApi.Proxy>(proxy, nameof (proxy));
      ArgumentUtility.CheckForNull<string>(proxy.Url, "Url");
      try
      {
        Uri uri = new Uri(proxy.Url, UriKind.Absolute);
      }
      catch (UriFormatException ex)
      {
        throw new InvalidProxyUriException(ex.Message, (Exception) ex);
      }
      this.CheckPermission(requestContext, ProxyPermissions.Manage);
      if (proxy.Authorization != null)
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          ArgumentUtility.CheckForNull<PublicKey>(proxy.Authorization.PublicKey, "PublicKey");
        else
          ArgumentUtility.CheckForNull<IdentityDescriptor>(proxy.Authorization.Identity, "Identity");
      }
      using (ProxyComponent component = requestContext.CreateComponent<ProxyComponent>())
        proxy.ProxyId = component.AddProxy(proxy);
      if (proxy.Authorization != null)
        this.AddProxyAuthorization(requestContext, proxy);
      return proxy;
    }

    private void AddProxyAuthorization(IVssRequestContext requestContext, Microsoft.TeamFoundation.Core.WebApi.Proxy proxy)
    {
      IdentityService service1 = requestContext.GetService<IdentityService>();
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IdentityDescriptor memberDescriptor;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string identifier = proxy.ProxyId.ToString("D");
        memberDescriptor = IdentityHelper.CreateFrameworkIdentityDescriptor(FrameworkIdentityType.ServiceIdentity, requestContext.ServiceHost.InstanceId, "ProxyService", identifier);
        Microsoft.VisualStudio.Services.Identity.Identity identity = service1.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          memberDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() ?? service1.CreateFrameworkIdentity(vssRequestContext, FrameworkIdentityType.ServiceIdentity, "ProxyService", identifier, "Proxy Service Account");
        proxy.Authorization.ClientId = proxy.ProxyId;
        Registration registration = new Registration()
        {
          ClientType = ClientType.MediumTrust,
          IdentityId = identity.Id,
          IsValid = true,
          PublicKey = proxy.Authorization.PublicKey.ToXmlString(),
          RegistrationId = proxy.Authorization.ClientId,
          RegistrationName = "Proxy Service",
          Scopes = "vso.proxy",
          RegistrationDescription = "Proxy Service for " + proxy.Url
        };
        IDelegatedAuthorizationRegistrationService service2 = vssRequestContext.GetService<IDelegatedAuthorizationRegistrationService>();
        try
        {
          service2.Create(vssRequestContext, registration);
        }
        catch (RegistrationAlreadyExistsException ex)
        {
          service2.Update(vssRequestContext, registration);
        }
        vssRequestContext.GetService<IDelegatedAuthorizationService>().AuthorizeHost(vssRequestContext, registration.RegistrationId);
        ILocationDataProvider locationData = requestContext.GetService<ILocationService>().GetLocationData(requestContext, TeamFoundationProxyService.s_oauthAreaId);
        proxy.Authorization.AuthorizationUrl = locationData.GetResourceUri(requestContext, "oauth2", OAuth2ResourceIds.Token, (object) null, false);
      }
      else
        memberDescriptor = proxy.Authorization.Identity;
      service1.AddMemberToGroup(vssRequestContext, GroupWellKnownIdentityDescriptors.Proxy.ServiceAccounts, memberDescriptor);
    }

    public List<Microsoft.TeamFoundation.Core.WebApi.Proxy> QueryProxies(
      IVssRequestContext requestContext,
      IList<string> proxyUrls)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
      this.CheckPermission(requestContext, ProxyPermissions.Read);
      if (proxyUrls != null && proxyUrls.Count > 0)
      {
        for (int index = 0; index < proxyUrls.Count; ++index)
          ArgumentUtility.CheckForNull<string>(proxyUrls[index], string.Format("proxyUrls[{0}]", (object) index));
      }
      using (ProxyComponent component = requestContext.CreateComponent<ProxyComponent>())
        return component.QueryProxies(proxyUrls);
    }

    public void DeleteProxy(IVssRequestContext requestContext, string proxyUrl, string site = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckProjectCollectionRequestContext();
      ArgumentUtility.CheckForNull<string>(proxyUrl, nameof (proxyUrl));
      this.CheckPermission(requestContext, ProxyPermissions.Manage);
      List<Microsoft.TeamFoundation.Core.WebApi.Proxy> source;
      using (ProxyComponent component = requestContext.CreateComponent<ProxyComponent>())
        source = component.QueryProxies((IList<string>) new string[1]
        {
          proxyUrl
        });
      if (!string.IsNullOrEmpty(site))
        source = source.Where<Microsoft.TeamFoundation.Core.WebApi.Proxy>((Func<Microsoft.TeamFoundation.Core.WebApi.Proxy, bool>) (x => string.Equals(x.Site, site, StringComparison.OrdinalIgnoreCase))).ToList<Microsoft.TeamFoundation.Core.WebApi.Proxy>();
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        foreach (Microsoft.TeamFoundation.Core.WebApi.Proxy proxy in source)
        {
          if (proxy.ProxyId != Guid.Empty)
          {
            IVssRequestContext vssRequestContext = requestContext.Elevate();
            vssRequestContext.GetService<IDelegatedAuthorizationRegistrationService>().Delete(vssRequestContext, proxy.ProxyId);
          }
        }
      }
      using (ProxyComponent component = requestContext.CreateComponent<ProxyComponent>())
        component.DeleteProxy(proxyUrl, site);
    }

    private void CheckPermission(IVssRequestContext requestContext, int permissions)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.ProxyNamespaceId);
      if (securityNamespace == null)
      {
        if (!requestContext.IsSystemContext && permissions != ProxyPermissions.Read)
          throw new AccessCheckException(requestContext.UserContext, FrameworkSecurity.ProxyNamespaceToken, permissions, FrameworkSecurity.ProxyNamespaceId, TFCommonResources.AccessCheckExceptionTokenFormat((object) requestContext.GetUserId().ToString(), (object) FrameworkSecurity.ProxyNamespaceToken, (object) permissions));
      }
      else
        securityNamespace.CheckPermission(requestContext, FrameworkSecurity.ProxyNamespaceToken, permissions);
    }
  }
}
