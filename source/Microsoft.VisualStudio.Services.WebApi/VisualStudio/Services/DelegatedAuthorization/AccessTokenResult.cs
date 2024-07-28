// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AccessTokenResult
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [DataContract]
  [ClientIncludeModel]
  public class AccessTokenResult
  {
    [DataMember]
    public Guid AuthorizationId { get; set; }

    [DataMember]
    public JsonWebToken AccessToken { get; set; }

    [DataMember]
    public string TokenType { get; set; }

    [DataMember]
    public DateTime ValidTo { get; set; }

    [DataMember]
    public RefreshTokenGrant RefreshToken { get; set; }

    [DataMember]
    public TokenError AccessTokenError { get; set; }

    [DataMember]
    public bool HasError => this.AccessTokenError != 0;

    [DataMember]
    public string ErrorDescription { get; set; }

    [DataMember]
    public string Scope { get; set; }

    [DataMember]
    internal bool IsFirstPartyClient { get; set; }
  }
}
