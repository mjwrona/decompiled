// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.CoreAuthenticationModule
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class CoreAuthenticationModule : IHttpModule
  {
    private const string ReadByPseudoSubjectDescriptorFeatureFlag = "AzureDevOps.Services.CoreAuthenticationModule.ReadByPseudoSubjectDescriptor";

    public void Init(HttpApplication context)
    {
      context.PostAuthenticateRequest += new EventHandler(this.OnPostAuthenticateRequest);
      context.PostAuthorizeRequest += new EventHandler(this.OnPostAuthorizeRequest);
    }

    public void Dispose()
    {
    }

    private void OnPostAuthenticateRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      IVssRequestContext requestContext = (IVssRequestContext) context.Items[(object) HttpContextConstants.IVssRequestContext];
      if (requestContext == null)
        return;
      if (context.User != null && context.User.Identity.IsAuthenticated)
      {
        Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this.ResolveIdentity(requestContext, context.User.Identity, true);
        this.SetupRequestIdentity(requestContext, context, authenticatedIdentity);
      }
      this.ApplyRequestRestrictions(requestContext, context);
    }

    private void OnPostAuthorizeRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      IVssRequestContext vssRequestContext = (IVssRequestContext) context.Items[(object) HttpContextConstants.IVssRequestContext];
      if (vssRequestContext == null || !context.Request.Path.EndsWith("/_signedin", StringComparison.OrdinalIgnoreCase) || !StringComparer.OrdinalIgnoreCase.Equals(context.Request.HttpMethod, "GET") && !StringComparer.OrdinalIgnoreCase.Equals(context.Request.HttpMethod, "POST"))
        return;
      string x = context.Request.QueryString["protocol"];
      string url = context.Request.QueryString["reply_to"];
      if (StringComparer.OrdinalIgnoreCase.Equals(x, "javascriptnotify"))
      {
        IReadOnlyList<string> tokenData = vssRequestContext.GetService<ITeamFoundationAuthenticationService>().GetSessionSecurityTokenDataFromCookies(vssRequestContext).FirstOrDefault<SessionSecurityTokenData>().TokenData;
        context.Response.StatusCode = 200;
        context.Response.ContentEncoding = Encoding.UTF8;
        context.Response.ContentType = "text/html";
        context.Response.Write(CoreAuthenticationModule.GetJavascriptNotifyContent(tokenData));
        context.ApplicationInstance.CompleteRequest();
      }
      else
      {
        if (string.IsNullOrEmpty(url))
          return;
        context.Response.Redirect(url, false);
        context.ApplicationInstance.CompleteRequest();
      }
    }

    private Guid GetStorageKeyByDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IUserIdentifierConversionService service = vssRequestContext.GetService<IUserIdentifierConversionService>();
      try
      {
        return service.GetStorageKeyByDescriptor(vssRequestContext, descriptor, true);
      }
      catch (UserDoesNotExistException ex1)
      {
        requestContext.TraceException(2430001, "Authentication", nameof (CoreAuthenticationModule), (Exception) ex1);
        Guid property1 = identity.GetProperty<Guid>("Domain", Guid.Empty);
        Guid property2 = identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
        requestContext.Trace(2430000, TraceLevel.Error, "Authentication", nameof (CoreAuthenticationModule), string.Format("UserDoesNotExistException - SubjectDescriptor = {0}, Tenant = {1}, OID = {2}", (object) descriptor, (object) property1.ToString(), (object) property2.ToString()));
        if (requestContext.IsFeatureEnabled("AzureDevOps.Services.CoreAuthenticationModule.ReadByPseudoSubjectDescriptor"))
        {
          SubjectDescriptor subjectDescriptor = UserHelper.CreatePseudoSubjectDescriptor(identity.Descriptor);
          try
          {
            return service.GetStorageKeyByDescriptor(vssRequestContext, subjectDescriptor, true);
          }
          catch (UserDoesNotExistException ex2)
          {
            requestContext.TraceException(2430002, "Authentication", nameof (CoreAuthenticationModule), (Exception) ex2);
            requestContext.Trace(2430004, TraceLevel.Error, "Authentication", nameof (CoreAuthenticationModule), string.Format("UserDoesNotExistException - SubjectDescriptor = {0}, Tenant = {1}, OID = {2}", (object) subjectDescriptor, (object) property1.ToString(), (object) property2.ToString()));
          }
        }
        return IdentityCuidHelper.ComputeCuid(requestContext, (IReadOnlyVssIdentity) identity, NameBasedGuidVersion.Six);
      }
    }

    private Microsoft.VisualStudio.Services.Identity.Identity ResolveIdentity(
      IVssRequestContext requestContext,
      IIdentity bclIdentity,
      bool setTracingItems)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      string identityType = IdentityHelper.GetIdentityType(bclIdentity);
      IIdentityProvider identityProvider;
      if (service.SyncAgents == null || !service.SyncAgents.TryGetValue(identityType, out identityProvider))
        throw new NotSupportedException(FrameworkResources.IdentityProviderNotFoundMessage((object) identityType));
      IdentityDescriptor descriptor = identityProvider.CreateDescriptor(vssRequestContext, bclIdentity);
      Microsoft.VisualStudio.Services.Identity.Identity identity = identityProvider.GetIdentity(vssRequestContext, bclIdentity);
      IdentityTracingItems identityTracingItems = IdentityHelper.GenerateIdentityTracingItems(vssRequestContext, (IReadOnlyVssIdentity) identity);
      if (setTracingItems)
        requestContext.RootContext.Items[RequestContextItemsKeys.IdentityTracingItems] = (object) identityTracingItems;
      if (identity.IsServiceIdentity)
      {
        requestContext.Trace(2430003, TraceLevel.Warning, "Authentication", nameof (CoreAuthenticationModule), string.Format("ResolveIdentity identity: Id = {0}, SubjectDescriptor = {1}", (object) identity.Id, (object) identity.SubjectDescriptor));
        return identity;
      }
      if (!ServicePrincipals.IsServicePrincipal(requestContext, descriptor))
      {
        identity.SetProperty("CUID", (object) identityTracingItems.Cuid);
        identity.SubjectDescriptor = identity.CreateSubjectDescriptor(requestContext);
        Guid storageKeyByDescriptor = this.GetStorageKeyByDescriptor(requestContext, identity.SubjectDescriptor, identity);
        identity.Id = identity.MasterId = storageKeyByDescriptor;
      }
      requestContext.Trace(2430003, TraceLevel.Warning, "Authentication", nameof (CoreAuthenticationModule), string.Format("ResolveIdentity identity: Id = {0}, SubjectDescriptor = {1}", (object) identity.Id, (object) identity.SubjectDescriptor));
      return identity;
    }

    private Microsoft.VisualStudio.Services.Identity.Identity ResolveImpersonatedIdentity(
      IVssRequestContext requestContext,
      HttpContext httpContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IIdentity bclIdentity;
      if (requestContext.TryGetItem<IIdentity>(RequestContextItemsKeys.TfsImpersonate, out bclIdentity))
      {
        requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, false);
        identity = this.ResolveIdentity(requestContext, bclIdentity, true);
      }
      return identity;
    }

    private void SetupRequestIdentity(
      IVssRequestContext requestContext,
      HttpContext httpContext,
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity)
    {
      string domainUserName = IdentityHelper.GetDomainUserName(authenticatedIdentity);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = authenticatedIdentity;
      requestContext.Items[authenticatedIdentity.Descriptor.ToString()] = (object) authenticatedIdentity;
      List<IRequestActor> requestActorList = new List<IRequestActor>()
      {
        RequestActor.CreateRequestActor(requestContext, authenticatedIdentity.Descriptor, authenticatedIdentity.Id)
      };
      requestContext.RequestContextInternal().Actors = (IReadOnlyList<IRequestActor>) requestActorList;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.ResolveImpersonatedIdentity(requestContext, httpContext);
      if (identity2 != null)
      {
        identity1 = identity2;
        requestContext.Items[identity1.Descriptor.ToString()] = (object) identity1;
        httpContext.Items[(object) HttpContextConstants.UserIdentityInternal] = (object) identity1;
        requestActorList.Add(RequestActor.CreateRequestActor(requestContext, identity2.Descriptor, identity2.Id));
        domainUserName = IdentityHelper.GetDomainUserName(identity2);
        ((VssRequestContext) requestContext).ClearUserContextCache();
      }
      requestContext.RequestContextInternal().SetAuthenticatedUserName(domainUserName);
      requestContext.RequestContextInternal().SetDomainUserName(domainUserName);
      requestContext.Trace(2430003, TraceLevel.Warning, "Authentication", nameof (CoreAuthenticationModule), string.Format("SetupRequestIdentity identities: authenticatedIdentity = {0}, userContextIdentity = {1}", (object) authenticatedIdentity.MasterId, (object) identity1.MasterId));
    }

    private void ApplyRequestRestrictions(
      IVssRequestContext requestContext,
      HttpContext httpContext)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation"))
        return;
      IdentityValidationResult validationResult = requestContext.To(TeamFoundationHostType.Deployment).GetService<IIdentityValidationService>().ValidateRequestIdentity(requestContext);
      if (validationResult.IsSuccess)
        return;
      IHttpApplication applicationInstance = new HttpContextWrapper(httpContext).GetApplicationInstance();
      if (validationResult.Exception != null)
        TeamFoundationApplicationCore.CompleteRequest(requestContext, applicationInstance, validationResult.HttpStatusCode, validationResult.Exception);
      else
        TeamFoundationApplicationCore.CompleteRequest(applicationInstance, validationResult.HttpStatusCode, validationResult.ResultMessage, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, validationResult.ResultMessage, (string) null);
    }

    private static string GetJavascriptNotifyContent(IReadOnlyList<string> authTokenParts)
    {
      string newValue = string.Join(",", authTokenParts.Select<string, string>((Func<string, string>) (part => "\"" + part + "\"")));
      return FrameworkResources.SignedInContentForJavascriptNotify().Replace("$$TokenData$$", newValue);
    }
  }
}
