// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssAadVSAccountProviderToken
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Client
{
  [Serializable]
  public class VssAadVSAccountProviderToken : IssuedToken
  {
    private AuthenticationResult m_authResult;

    public VssAadVSAccountProviderToken(AuthenticationResult result)
    {
      this.FromStorage = true;
      this.m_authResult = result;
      if (string.IsNullOrWhiteSpace(this.m_authResult?.AccessToken))
        return;
      this.Authenticated();
    }

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.Aad;

    internal override void ApplyTo(IHttpRequest request)
    {
      if (this.m_authResult == null)
        return;
      request.Headers.SetValue("Authorization", this.m_authResult.CreateAuthorizationHeader());
    }
  }
}
