// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AccessToken
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AccessToken
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Guid AccessId { get; set; }

    public Guid AuthorizationId { get; set; }

    public DateTimeOffset ValidFrom { get; set; }

    public DateTimeOffset ValidTo { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public DateTimeOffset Refreshed { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsRefresh { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsValid { get; set; }

    public JsonWebToken Token { get; set; }

    public string TokenType { get; set; }

    public JsonWebToken RefreshToken { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Guid RegistrationId { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal string Scope { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool IsFirstPartyClient { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal TokenError AccessTokenError { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool HasError => this.AccessTokenError != 0;
  }
}
