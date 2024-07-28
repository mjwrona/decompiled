// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Authentication.SignedInCiEventPublisher
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;

namespace Microsoft.VisualStudio.Services.Authentication
{
  public class SignedInCiEventPublisher
  {
    public virtual AuthenticationCiEvent.Flows GetAuthFlowBasedOnIdentity(
      IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity == null)
        return AuthenticationCiEvent.Flows.Unknown;
      int num = userIdentity.IsExternalUser ? 1 : 0;
      return userIdentity.IsExternalUser ? AuthenticationCiEvent.Flows.Aad : AuthenticationCiEvent.Flows.Msa;
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      AuthenticationCiEvent.Sources source,
      bool isAadCallback)
    {
      if (isAadCallback || TenantPickerHelper.HasTenantHint(requestContext))
        return;
      SignedInCiEvent signedInCiEvent = new SignedInCiEvent();
      signedInCiEvent.Source = source;
      signedInCiEvent.Flow = this.GetAuthFlowBasedOnIdentity(requestContext);
      signedInCiEvent.Publish(requestContext);
    }
  }
}
