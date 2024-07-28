// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsImpersonationMessageHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TfsImpersonationMessageHandler : DelegatingHandler
  {
    private const string c_area = "Identity";
    private const string c_layer = "TfsImpersonationMessageHandler";

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      object obj = (object) null;
      if (request.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj))
      {
        IVssRequestContext requestContext = obj as IVssRequestContext;
        if (requestContext != null && requestContext.UserContext != (IdentityDescriptor) null && !requestContext.IsSystemContext)
        {
          if ((requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.UseDelegatedS2STokens) || request.Properties.GetCastedValueOrDefault<string, bool>(FrameworkServerConstants.UseDelegatedS2STokens)) && !request.Properties.GetCastedValueOrDefault<string, bool>(FrameworkServerConstants.DisableDelegatedS2STokens))
          {
            ClaimsIdentity delegatedUser = this.GetDelegatedUser(requestContext);
            requestContext.TraceConditionally(639664894, TraceLevel.Info, "Identity", nameof (TfsImpersonationMessageHandler), (Func<string>) (() => string.Format("Identity {0} resolved to delegated user {1}", (object) requestContext.UserContext, (object) TfsImpersonationMessageHandler.ToString(delegatedUser))));
            request.Properties[TfsApiPropertyKeys.DelegatedUser] = (object) delegatedUser;
          }
          else
            request.Headers.Add("X-TFS-Impersonate", TFCommonUtil.MakeDescriptorSearchFactor(requestContext.UserContext.IdentityType, requestContext.UserContext.Identifier));
        }
      }
      return base.SendAsync(request, cancellationToken);
    }

    private ClaimsIdentity GetDelegatedUser(IVssRequestContext requestContext)
    {
      ClaimsIdentity identity;
      if (!requestContext.RootContext.TryGetItem<ClaimsIdentity>(RequestContextItemsKeys.AuthorizedClaimsIdentity, out identity) || identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier") == null && identity.FindFirst("PUID") == null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        requestContext.Trace(639664895, TraceLevel.Info, "Identity", nameof (TfsImpersonationMessageHandler), "Creating authorized identity for {0}", (object) userIdentity);
        if (userIdentity != null)
          identity = ClaimsProvider.GetIdentity(userIdentity, (string) null);
      }
      string str;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AuthorizationIdClaim") && requestContext.RootContext.TryGetItem<string>(RequestContextItemsKeys.AuthorizationId, out str) && identity != null)
        identity.AddClaim(new Claim("https://schemas.microsoft.com/teamfoundationserver/2010/12/claims/authorizationid", str));
      return identity;
    }

    private static string ToString(ClaimsIdentity claimsIdentity) => claimsIdentity != null ? "[" + string.Join(",", claimsIdentity.Claims.Select<Claim, string>((Func<Claim, string>) (x => x.Type + "=" + x.Value))) + "]" : "<null>";
  }
}
