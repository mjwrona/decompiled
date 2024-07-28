// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BasicAuthenticationModule
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Identity;
using System.Security.Principal;
using System.Web;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class BasicAuthenticationModule : FrameworkBasicAuthenticationModule
  {
    protected override FrameworkBasicAuthenticationModule.AuthenticationStatus AuthenticateAsAlternateCredential(
      IVssRequestContext requestContext,
      HttpContextBase context,
      string username,
      string password)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IVssIdentityRetrievalService>().ResolveEligibleActorByBasicAuthAlias(requestContext, username);
      if (identity == null)
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Unhandled;
      if (!requestContext.GetService<ITeamFoundationBasicAuthService>().IsValidBasicCredential(requestContext, identity, password))
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected;
      if (requestContext.IsFeatureEnabled("VisualStudio.AlternateCredentials.Disable"))
        return FrameworkBasicAuthenticationModule.AuthenticationStatus.Rejected_AltCredsDeprecated;
      context.User = (IPrincipal) new AlternateLoginPrincipal("Basic", identity);
      AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.Basic);
      requestContext.To(TeamFoundationHostType.Deployment).Items.Add(RequestContextItemsKeys.AlternateAuthCredentialsContextKey, (object) true);
      return FrameworkBasicAuthenticationModule.AuthenticationStatus.Authenticated;
    }
  }
}
