// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.Msal.MsalSignedInController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers.Msal
{
  [SupportedRouteArea("Public", NavigationContextLevels.All)]
  public class MsalSignedInController : WebPlatformController
  {
    private readonly IAadAuthenticationSessionTokenProvider _aadAuthenticationSessionTokenProvider;
    private readonly IMsalConfigHelper _msalConfigHelper;

    public MsalSignedInController()
      : this((IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider(), (IMsalConfigHelper) MsalConfigHelper.Instance)
    {
    }

    public MsalSignedInController(
      IAadAuthenticationSessionTokenProvider authenticationSessionTokenProvider,
      IMsalConfigHelper msalConfigHelper)
    {
      this._aadAuthenticationSessionTokenProvider = authenticationSessionTokenProvider;
      this._msalConfigHelper = msalConfigHelper;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    [TfsBypassAntiForgeryValidation]
    public ActionResult Index()
    {
      AuthOptions authOptions = this._aadAuthenticationSessionTokenProvider.GetAuthOptions(this.TfsRequestContext);
      MsalConfig msalConfig = this._msalConfigHelper.GetMsalConfig(this.TfsRequestContext);
      SecureFlowLocation location;
      int num = (int) SecureFlowLocation.TryCreate(this.TfsRequestContext, this.Request.Form["location"], SecureFlowLocation.GetDefaultLocation(this.TfsRequestContext), out location);
      // ISSUE: reference to a compiler-generated field
      if (MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ClientId", typeof (MsalSignedInController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__0, this.ViewBag, authOptions.ClientId);
      // ISSUE: reference to a compiler-generated field
      if (MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Authority", typeof (MsalSignedInController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__1.Target((CallSite) MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__1, this.ViewBag, authOptions.Authority);
      // ISSUE: reference to a compiler-generated field
      if (MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string[], object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Scopes", typeof (MsalSignedInController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__2.Target((CallSite) MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__2, this.ViewBag, authOptions.Scopes);
      // ISSUE: reference to a compiler-generated field
      if (MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, Guid, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ActivityId", typeof (MsalSignedInController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__3.Target((CallSite) MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__3, this.ViewBag, this.TfsRequestContext.ActivityId);
      // ISSUE: reference to a compiler-generated field
      if (MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Code", typeof (MsalSignedInController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__4.Target((CallSite) MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__4, this.ViewBag, HttpUtility.HtmlEncode(this.Request.Form["spa_authorization_code"]));
      // ISSUE: reference to a compiler-generated field
      if (MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Location", typeof (MsalSignedInController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__5.Target((CallSite) MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__5, this.ViewBag, location.ToString());
      // ISSUE: reference to a compiler-generated field
      if (MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "CacheLocation", typeof (MsalSignedInController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__6.Target((CallSite) MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__6, this.ViewBag, msalConfig.CacheLocation);
      // ISSUE: reference to a compiler-generated field
      if (MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "LogLevel", typeof (MsalSignedInController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__7.Target((CallSite) MsalSignedInController.\u003C\u003Eo__4.\u003C\u003Ep__7, this.ViewBag, msalConfig.LogLevel);
      return (ActionResult) this.View("MsalSignedIn");
    }
  }
}
