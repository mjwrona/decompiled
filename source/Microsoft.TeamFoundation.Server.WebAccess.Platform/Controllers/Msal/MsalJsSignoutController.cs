// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.Msal.MsalJsSignoutController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers.Msal
{
  [SupportedRouteArea("Public", NavigationContextLevels.All)]
  [SecurityHeaders(true, true, true)]
  public class MsalJsSignoutController : WebPlatformController
  {
    private readonly IAadAuthenticationSessionTokenProvider _aadAuthenticationSessionTokenProvider;
    private readonly IMsalConfigHelper _msalConfigHelper;

    public MsalJsSignoutController()
      : this((IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider(), (IMsalConfigHelper) MsalConfigHelper.Instance)
    {
    }

    public MsalJsSignoutController(
      IAadAuthenticationSessionTokenProvider authenticationSessionTokenProvider,
      IMsalConfigHelper msalConfigHelper)
    {
      this._aadAuthenticationSessionTokenProvider = authenticationSessionTokenProvider;
      this._msalConfigHelper = msalConfigHelper;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public ActionResult Index()
    {
      AuthOptions authOptions = this._aadAuthenticationSessionTokenProvider.GetAuthOptions(this.TfsRequestContext);
      MsalConfig msalConfig = this._msalConfigHelper.GetMsalConfig(this.TfsRequestContext);
      Uri uri = new Uri(this.TfsRequestContext.GetService<ITeamFoundationAuthenticationService>().LocationForRealm(this.TfsRequestContext, "/_signout"));
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      string policyValue = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Configuration/WebPlatform/MsalJsSignout/ContentSecurityPolicy/FrameAncestors", "*.vssps.visualstudio.com *.dev.azure.com dev.azure.com");
      vssRequestContext.GetService<IContentSecurityPolicyHeaderManagementService>().AddAdditionalPolicyForDirective(this.TfsRequestContext, this.HttpContext, "frame-ancestors", policyValue);
      string[] strArray = uri.Host.Split(new char[1]{ '.' }, 2);
      // ISSUE: reference to a compiler-generated field
      if (MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "TargetOriginRoot", typeof (MsalJsSignoutController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__0, this.ViewBag, strArray.Length == 2 ? strArray[1] : uri.Host);
      // ISSUE: reference to a compiler-generated field
      if (MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ClientId", typeof (MsalJsSignoutController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__1.Target((CallSite) MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__1, this.ViewBag, authOptions.ClientId);
      // ISSUE: reference to a compiler-generated field
      if (MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Authority", typeof (MsalJsSignoutController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__2.Target((CallSite) MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__2, this.ViewBag, authOptions.Authority);
      // ISSUE: reference to a compiler-generated field
      if (MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CacheLocation", typeof (MsalJsSignoutController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__3.Target((CallSite) MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__3, this.ViewBag, msalConfig.CacheLocation);
      // ISSUE: reference to a compiler-generated field
      if (MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "LogLevel", typeof (MsalJsSignoutController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__4.Target((CallSite) MsalJsSignoutController.\u003C\u003Eo__4.\u003C\u003Ep__4, this.ViewBag, msalConfig.LogLevel);
      return (ActionResult) this.View("MsalSignout");
    }
  }
}
