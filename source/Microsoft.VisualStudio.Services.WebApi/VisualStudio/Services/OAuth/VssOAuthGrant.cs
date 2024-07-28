// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthGrant
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public abstract class VssOAuthGrant : IVssOAuthTokenParameterProvider
  {
    private readonly VssOAuthGrantType m_grantType;
    private static readonly Lazy<VssOAuthClientCredentialsGrant> s_clientCredentialsGrant = new Lazy<VssOAuthClientCredentialsGrant>((Func<VssOAuthClientCredentialsGrant>) (() => new VssOAuthClientCredentialsGrant()));

    protected VssOAuthGrant(VssOAuthGrantType grantType) => this.m_grantType = grantType;

    public VssOAuthGrantType GrantType => this.m_grantType;

    public static VssOAuthClientCredentialsGrant ClientCredentials => VssOAuthGrant.s_clientCredentialsGrant.Value;

    protected abstract void SetParameters(IDictionary<string, string> parameters);

    void IVssOAuthTokenParameterProvider.SetParameters(IDictionary<string, string> parameters) => this.SetParameters(parameters);
  }
}
