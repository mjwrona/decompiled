// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LoopbackHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LoopbackHandler : DelegatingHandler
  {
    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage message,
      CancellationToken token)
    {
      IVssRequestContext vssRequestContext1 = (IVssRequestContext) null;
      object obj = (object) null;
      if (message.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj))
        vssRequestContext1 = obj as IVssRequestContext;
      else if (HttpContext.Current != null)
        vssRequestContext1 = (IVssRequestContext) HttpContext.Current.Items[(object) HttpContextConstants.IVssRequestContext];
      if (vssRequestContext1 == null)
        throw new NotSupportedException(FrameworkResources.RequestContextRequiredForOperation());
      GenericHttpRequestContext loopbackContext = (GenericHttpRequestContext) null;
      HttpResponseMessage httpResponseMessage1;
      try
      {
        IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment);
        string virtualPathRoot = WebApiConfiguration.GetVirtualPathRoot(vssRequestContext2);
        HttpRequestMessageContextWrapper messageContextWrapper = new HttpRequestMessageContextWrapper(virtualPathRoot, message);
        IUrlHostResolutionService service1 = vssRequestContext2.GetService<IUrlHostResolutionService>();
        if (!HostingEnvironment.IsHosted)
          service1.ApplicationVirtualPath = virtualPathRoot;
        HostRouteContext hostRouteContext = service1.ResolveHost(vssRequestContext2, messageContextWrapper.Request.Url);
        messageContextWrapper.Items[(object) HttpContextConstants.ServiceHostRouteContext] = (object) hostRouteContext;
        RequestContextType contextType = RequestContextType.UserContext;
        bool flag = true;
        if (vssRequestContext1.IsServicingContext)
        {
          contextType = RequestContextType.ServicingContext;
          flag = false;
        }
        IInternalTeamFoundationHostManagementService service2 = vssRequestContext2.GetService<IInternalTeamFoundationHostManagementService>();
        loopbackContext = (GenericHttpRequestContext) service2.BeginRequest(vssRequestContext2, hostRouteContext.HostId, contextType, true, (flag ? 1 : 0) != 0, (IReadOnlyList<IRequestActor>) null, HostRequestType.GenericHttp, (object) messageContextWrapper);
        vssRequestContext1.AddDisposableResource((IDisposable) loopbackContext);
        loopbackContext.WebRequestContextInternal().RequestRestrictions = new RequestRestrictions(RequiredAuthentication.ValidatedUser, AllowedHandler.TfsHttpHandler, "Loopback");
        this.RewriteRequestUrl((IVssRequestContext) loopbackContext, message);
        IdentityService service3 = vssRequestContext2.GetService<IdentityService>();
        IdentityDescriptor descriptor = vssRequestContext2.ServiceHost.SystemDescriptor();
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity1 = service3.ReadIdentities(vssRequestContext2, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          descriptor
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        Microsoft.VisualStudio.Services.Identity.Identity userContextIdentity = readIdentity1 != null ? readIdentity1 : throw new IdentityNotFoundException(descriptor);
        string domainUserName = IdentityHelper.GetDomainUserName(readIdentity1);
        string authenticatedUserName = domainUserName;
        List<IRequestActor> actors = new List<IRequestActor>()
        {
          RequestActor.CreateRequestActor(vssRequestContext2, readIdentity1.Descriptor, readIdentity1.Id)
        };
        if (vssRequestContext1.UserContext != (IdentityDescriptor) null && !vssRequestContext1.IsSystemContext)
        {
          Microsoft.VisualStudio.Services.Identity.Identity readIdentity2 = service3.ReadIdentities(vssRequestContext2, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            vssRequestContext1.UserContext
          }, QueryMembership.None, (IEnumerable<string>) null)[0];
          if (readIdentity2 == null)
            throw new IdentityNotFoundException(vssRequestContext1.UserContext);
          actors.Add(RequestActor.CreateRequestActor(vssRequestContext2, readIdentity2.Descriptor, readIdentity2.Id));
          authenticatedUserName = IdentityHelper.GetDomainUserName(readIdentity2);
          userContextIdentity = readIdentity2;
        }
        service2.SetupUserContext((IVssRequestContext) loopbackContext, (IReadOnlyList<IRequestActor>) actors, authenticatedUserName, domainUserName);
        loopbackContext.SetUserIdentityTracingItems(userContextIdentity);
        message.Properties.Add(HttpContextConstants.IVssRequestContext, (object) loopbackContext);
        messageContextWrapper.Items.Add((object) HttpContextConstants.IVssRequestContext, (object) loopbackContext);
        message.Properties.Add(TfsApiPropertyKeys.HttpContext, (object) messageContextWrapper);
        if (message.Content == null)
          message.Content = LoopbackHandler.CreateEmptyContent();
        HttpResponseMessage httpResponseMessage2 = await base.SendAsync(message, token).ConfigureAwait(false);
        if (httpResponseMessage2.Content == null)
          httpResponseMessage2.Content = LoopbackHandler.CreateEmptyContent();
        httpResponseMessage1 = httpResponseMessage2;
      }
      catch (Exception ex)
      {
        if (loopbackContext != null)
        {
          loopbackContext.Dispose();
          loopbackContext = (GenericHttpRequestContext) null;
        }
        throw;
      }
      loopbackContext = (GenericHttpRequestContext) null;
      return httpResponseMessage1;
    }

    private void RewriteRequestUrl(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      string absolutePath = requestMessage.RequestUri.AbsolutePath;
      if (absolutePath.IndexOf(HttpRouteCollectionExtensions.DefaultRoutePrefix, StringComparison.OrdinalIgnoreCase) < 0)
        return;
      string str1 = new Uri(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker)).AbsolutePath.TrimEnd('/');
      string str2 = absolutePath.Substring(str1.Length).TrimStart('/');
      requestMessage.RequestUri = new UriBuilder(requestMessage.RequestUri)
      {
        Path = (VirtualPathUtility.AppendTrailingSlash(WebApiConfiguration.GetVirtualPathRoot(requestContext)) + str2)
      }.Uri;
    }

    private static HttpContent CreateEmptyContent() => (HttpContent) new StreamContent((Stream) new MemoryStream(Array.Empty<byte>(), false));
  }
}
