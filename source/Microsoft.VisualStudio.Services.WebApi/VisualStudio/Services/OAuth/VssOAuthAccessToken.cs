// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthAccessToken
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public sealed class VssOAuthAccessToken : IssuedToken
  {
    private readonly string m_value;
    private readonly DateTime m_validTo;

    public VssOAuthAccessToken(string value)
      : this(value, DateTime.MaxValue)
    {
    }

    public VssOAuthAccessToken(string value, DateTime validTo)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      this.m_value = value;
      this.m_validTo = validTo;
    }

    public VssOAuthAccessToken(JsonWebToken value)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(value, nameof (value));
      this.m_value = value.EncodedToken;
      this.m_validTo = value.ValidTo;
    }

    public DateTime ValidTo => this.m_validTo;

    public string Value => this.m_value;

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.OAuth;

    internal override void ApplyTo(IHttpRequest request) => request.Headers.SetValue("Authorization", "Bearer " + this.m_value);
  }
}
