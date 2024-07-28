// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.Msal.MsalRedirectController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers.Msal
{
  [SupportedRouteArea("Public", NavigationContextLevels.All)]
  public class MsalRedirectController : WebPlatformController
  {
    private readonly IAadAuthenticationSessionTokenProvider _aadAuthenticationSessionTokenProvider;
    private readonly IMsalConfigHelper _msalConfigHelper;

    public MsalRedirectController()
      : this((IAadAuthenticationSessionTokenProvider) new AadAuthenticationSessionTokenProvider(), (IMsalConfigHelper) MsalConfigHelper.Instance)
    {
    }

    public MsalRedirectController(
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
      // ISSUE: reference to a compiler-generated field
      if (MsalRedirectController.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalRedirectController.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "ClientId", typeof (MsalRedirectController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = MsalRedirectController.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) MsalRedirectController.\u003C\u003Eo__4.\u003C\u003Ep__0, this.ViewBag, authOptions.ClientId);
      // ISSUE: reference to a compiler-generated field
      if (MsalRedirectController.\u003C\u003Eo__4.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        MsalRedirectController.\u003C\u003Eo__4.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "LogLevel", typeof (MsalRedirectController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = MsalRedirectController.\u003C\u003Eo__4.\u003C\u003Ep__1.Target((CallSite) MsalRedirectController.\u003C\u003Eo__4.\u003C\u003Ep__1, this.ViewBag, msalConfig.LogLevel);
      return (ActionResult) this.View("MsalRedirect");
    }
  }
}
