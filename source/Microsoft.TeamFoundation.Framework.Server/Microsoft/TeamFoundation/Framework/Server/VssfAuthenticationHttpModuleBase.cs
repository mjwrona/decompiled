// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssfAuthenticationHttpModuleBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssfAuthenticationHttpModuleBase : IHttpModule
  {
    public void Init(HttpApplication context)
    {
      this.InitModule((HttpContextBase) new HttpContextWrapper(context.Context));
      context.AuthenticateRequest += (EventHandler) ((sender, eventArgs) => this.InvokeEventTarget(sender, eventArgs, VssfAuthenticationHttpModuleBase.HttpRequestStage.Authenticate));
      context.PostAuthenticateRequest += (EventHandler) ((sender, eventArgs) => this.InvokeEventTarget(sender, eventArgs, VssfAuthenticationHttpModuleBase.HttpRequestStage.PostAuthenticate));
      context.AuthorizeRequest += (EventHandler) ((sender, eventArgs) => this.InvokeEventTarget(sender, eventArgs, VssfAuthenticationHttpModuleBase.HttpRequestStage.Authorize));
      context.PostAuthorizeRequest += (EventHandler) ((sender, eventArgs) => this.InvokeEventTarget(sender, eventArgs, VssfAuthenticationHttpModuleBase.HttpRequestStage.PostAuthorize));
      context.EndRequest += (EventHandler) ((sender, eventArgs) => this.InvokeEventTarget(sender, eventArgs, VssfAuthenticationHttpModuleBase.HttpRequestStage.End));
    }

    protected virtual bool SkipIfAlreadyAuthenticated => true;

    public void Dispose()
    {
    }

    private void InvokeEventTarget(
      object sender,
      EventArgs eventArgs,
      VssfAuthenticationHttpModuleBase.HttpRequestStage requestStage)
    {
      HttpContextWrapper httpContext = new HttpContextWrapper(((HttpApplication) sender).Context);
      if (this.SkipIfAlreadyAuthenticated && requestStage != VssfAuthenticationHttpModuleBase.HttpRequestStage.End && httpContext.User != null && httpContext.User.Identity.IsAuthenticated || !(httpContext.Items[(object) "IVssRequestContext"] is IVssRequestContext requestContext) || httpContext.Items.Contains((object) HttpContextConstants.ArrRequestRouted))
        return;
      switch (requestStage)
      {
        case VssfAuthenticationHttpModuleBase.HttpRequestStage.Authenticate:
          this.OnAuthenticateRequest(requestContext, (HttpContextBase) httpContext, eventArgs);
          break;
        case VssfAuthenticationHttpModuleBase.HttpRequestStage.PostAuthenticate:
          this.OnPostAuthenticateRequest(requestContext, (HttpContextBase) httpContext, eventArgs);
          break;
        case VssfAuthenticationHttpModuleBase.HttpRequestStage.Authorize:
          this.OnAuthorizeRequest(requestContext, (HttpContextBase) httpContext, eventArgs);
          break;
        case VssfAuthenticationHttpModuleBase.HttpRequestStage.PostAuthorize:
          this.OnPostAuthorizeRequest(requestContext, (HttpContextBase) httpContext, eventArgs);
          break;
        case VssfAuthenticationHttpModuleBase.HttpRequestStage.End:
          this.OnEndRequest(requestContext, (HttpContextBase) httpContext, eventArgs);
          break;
      }
    }

    public virtual void InitModule(HttpContextBase context)
    {
    }

    public virtual void OnAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
    }

    public virtual void OnPostAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
    }

    public virtual void OnAuthorizeRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
    }

    public virtual void OnPostAuthorizeRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
    }

    public virtual void OnEndRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
    }

    private enum HttpRequestStage
    {
      Authenticate,
      PostAuthenticate,
      Authorize,
      PostAuthorize,
      End,
    }
  }
}
