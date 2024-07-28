// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationGrant
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [KnownType(typeof (RefreshTokenGrant))]
  [KnownType(typeof (JwtBearerAuthorizationGrant))]
  [JsonConverter(typeof (AuthorizationGrantJsonConverter))]
  public abstract class AuthorizationGrant
  {
    public AuthorizationGrant(GrantType grantType) => this.GrantType = grantType != GrantType.None ? grantType : throw new ArgumentException("Grant type is required.");

    [JsonConverter(typeof (StringEnumConverter))]
    public GrantType GrantType { get; private set; }
  }
}
