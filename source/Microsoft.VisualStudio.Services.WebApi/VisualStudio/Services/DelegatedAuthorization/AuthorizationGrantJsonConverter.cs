// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationGrantJsonConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationGrantJsonConverter : VssJsonCreationConverter<AuthorizationGrant>
  {
    protected override AuthorizationGrant Create(Type objectType, JObject jsonObject)
    {
      JToken jtoken1 = jsonObject.GetValue("GrantType", StringComparison.OrdinalIgnoreCase);
      if (jtoken1 == null)
        throw new ArgumentException(WebApiResources.UnknownEntityType((object) jtoken1));
      GrantType result;
      if (jtoken1.Type == JTokenType.Integer)
        result = (GrantType) (int) jtoken1;
      else if (jtoken1.Type != JTokenType.String || !Enum.TryParse<GrantType>((string) jtoken1, out result))
        return (AuthorizationGrant) null;
      AuthorizationGrant authorizationGrant = (AuthorizationGrant) null;
      JToken jtoken2 = jsonObject.GetValue("jwt");
      if (jtoken2 == null)
        return (AuthorizationGrant) null;
      JsonWebToken jwt = JsonWebToken.Create(jtoken2.ToString());
      switch (result)
      {
        case GrantType.JwtBearer:
          authorizationGrant = (AuthorizationGrant) new JwtBearerAuthorizationGrant(jwt);
          break;
        case GrantType.RefreshToken:
          authorizationGrant = (AuthorizationGrant) new RefreshTokenGrant(jwt);
          break;
      }
      return authorizationGrant;
    }
  }
}
