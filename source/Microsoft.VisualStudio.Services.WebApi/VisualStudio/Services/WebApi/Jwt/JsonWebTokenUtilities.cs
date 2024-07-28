// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Jwt.JsonWebTokenUtilities
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebApi.Jwt
{
  public static class JsonWebTokenUtilities
  {
    internal static readonly JsonSerializerSettings DefaultSerializerSettings;
    internal static readonly short MinKeySize = 2048;

    static JsonWebTokenUtilities()
    {
      JsonWebTokenUtilities.DefaultSerializerSettings = new JsonSerializerSettings();
      JsonWebTokenUtilities.DefaultSerializerSettings.Converters.Add((JsonConverter) new UnixEpochDateTimeConverter());
      JsonWebTokenUtilities.DefaultSerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter());
    }

    internal static string JsonEncode<T>(this T obj) => JsonWebTokenUtilities.JsonEncode((object) obj);

    internal static string JsonEncode(object o)
    {
      ArgumentUtility.CheckForNull<object>(o, nameof (o));
      return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(o, JsonWebTokenUtilities.DefaultSerializerSettings)).ToBase64StringNoPadding();
    }

    internal static T JsonDecode<T>(string encodedString)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(encodedString, nameof (encodedString));
      return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(encodedString.FromBase64StringNoPadding()), JsonWebTokenUtilities.DefaultSerializerSettings);
    }

    internal static IDictionary<string, object> TranslateToJwtClaims(IEnumerable<Claim> claims)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Claim>>(claims, nameof (claims));
      Dictionary<string, object> jwtClaims = new Dictionary<string, object>();
      foreach (Claim claim in claims)
      {
        string str = claim.Type;
        if (string.CompareOrdinal(str, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") == 0)
          str = "nameid";
        else if (string.CompareOrdinal(str, "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider") == 0)
          str = "identityprovider";
        jwtClaims.Add(str, (object) claim.Value);
      }
      return (IDictionary<string, object>) jwtClaims;
    }

    internal static IEnumerable<Claim> TranslateFromJwtClaims(IDictionary<string, object> claims)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(claims, nameof (claims));
      List<Claim> claimList = new List<Claim>(claims.Count);
      foreach (KeyValuePair<string, object> claim in (IEnumerable<KeyValuePair<string, object>>) claims)
      {
        string str = claim.Key;
        if (string.CompareOrdinal(str, "nameid") == 0)
          str = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        else if (string.CompareOrdinal(str, "identityprovider") == 0)
          str = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        claimList.Add(new Claim(str, claim.Value.ToString()));
      }
      return (IEnumerable<Claim>) claimList;
    }

    internal static IEnumerable<Claim> ExtractClaims(this JsonWebToken token)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(token, nameof (token));
      return JsonWebTokenUtilities.TranslateFromJwtClaims(token.Payload);
    }

    public static bool IsExpired(this JsonWebToken token)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(token, nameof (token));
      return DateTime.UtcNow > token.ValidTo;
    }

    internal static JWTAlgorithm ValidateSigningCredentials(
      VssSigningCredentials credentials,
      bool allowExpiredToken = false)
    {
      if (credentials == null)
        return JWTAlgorithm.None;
      if (!credentials.CanSignData)
        throw new InvalidCredentialsException(JwtResources.SigningTokenNoPrivateKey());
      if (!allowExpiredToken && credentials.ValidTo.ToUniversalTime() < DateTime.UtcNow - TimeSpan.FromMinutes(5.0))
        throw new InvalidCredentialsException(JwtResources.SigningTokenExpired());
      return credentials.SignatureAlgorithm;
    }

    public static ClaimsPrincipal ValidateToken(
      this JsonWebToken token,
      JsonWebTokenValidationParameters parameters)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(token, nameof (token));
      ArgumentUtility.CheckForNull<JsonWebTokenValidationParameters>(parameters, nameof (parameters));
      ClaimsIdentity claimsIdentity1 = JsonWebTokenUtilities.ValidateActor(token, parameters);
      JsonWebTokenUtilities.ValidateLifetime(token, parameters);
      JsonWebTokenUtilities.ValidateAudience(token, parameters);
      JsonWebTokenUtilities.ValidateSignature(token, parameters);
      JsonWebTokenUtilities.ValidateIssuer(token, parameters);
      ClaimsIdentity claimsIdentity2 = new ClaimsIdentity("Federation", parameters.IdentityNameClaimType, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
      if (claimsIdentity1 != null)
        claimsIdentity2.Actor = claimsIdentity1;
      foreach (Claim claim in token.ExtractClaims())
        claimsIdentity2.AddClaim(new Claim(claim.Type, claim.Value, claim.ValueType, token.Issuer));
      return new ClaimsPrincipal((IIdentity) claimsIdentity2);
    }

    private static ClaimsIdentity ValidateActor(
      JsonWebToken token,
      JsonWebTokenValidationParameters parameters)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(token, nameof (token));
      ArgumentUtility.CheckForNull<JsonWebTokenValidationParameters>(parameters, nameof (parameters));
      if (!parameters.ValidateActor)
        return (ClaimsIdentity) null;
      ClaimsPrincipal claimsPrincipal = token.Actor.ValidateToken(parameters.ActorValidationParameters);
      return claimsPrincipal?.Identity is ClaimsIdentity ? (ClaimsIdentity) claimsPrincipal.Identity : throw new ActorValidationException();
    }

    private static void ValidateLifetime(
      JsonWebToken token,
      JsonWebTokenValidationParameters parameters)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(token, nameof (token));
      ArgumentUtility.CheckForNull<JsonWebTokenValidationParameters>(parameters, nameof (parameters));
      if ((parameters.ValidateNotBefore || parameters.ValidateExpiration) && parameters.ClockSkewInSeconds < 0)
        throw new InvalidClockSkewException();
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) parameters.ClockSkewInSeconds);
      if (parameters.ValidateNotBefore && token.ValidFrom == new DateTime())
        throw new InvalidValidFromValueException();
      if (parameters.ValidateExpiration && token.ValidTo == new DateTime())
        throw new InvalidValidToValueException();
      if (parameters.ValidateExpiration && parameters.ValidateNotBefore && token.ValidFrom > token.ValidTo)
        throw new ValidFromAfterValidToException();
      if (parameters.ValidateNotBefore && token.ValidFrom > DateTime.UtcNow + timeSpan)
        throw new TokenNotYetValidException();
      if (parameters.ValidateExpiration && token.ValidTo < DateTime.UtcNow - timeSpan)
        throw new TokenExpiredException();
    }

    private static void ValidateAudience(
      JsonWebToken token,
      JsonWebTokenValidationParameters parameters)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(token, nameof (token));
      ArgumentUtility.CheckForNull<JsonWebTokenValidationParameters>(parameters, nameof (parameters));
      if (parameters.ValidateAudience)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(token.Audience, "Audience");
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) parameters.AllowedAudiences, "AllowedAudiences");
        foreach (string allowedAudience in parameters.AllowedAudiences)
        {
          if (string.Equals(allowedAudience, token.Audience, StringComparison.OrdinalIgnoreCase))
            return;
        }
        throw new InvalidAudienceException();
      }
    }

    private static void ValidateSignature(
      JsonWebToken token,
      JsonWebTokenValidationParameters parameters)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(token, nameof (token));
      ArgumentUtility.CheckForNull<JsonWebTokenValidationParameters>(parameters, nameof (parameters));
      if (!parameters.ValidateSignature)
        return;
      string[] strArray = token.EncodedToken.Split('.');
      if (strArray.Length != 3)
        throw new InvalidTokenException(JwtResources.EncodedTokenDataMalformed());
      if (string.IsNullOrEmpty(strArray[2]))
        throw new InvalidTokenException(JwtResources.SignatureNotFound());
      if (token.Algorithm == JWTAlgorithm.None)
        throw new InvalidTokenException(JwtResources.InvalidSignatureAlgorithm());
      ArgumentUtility.CheckForNull<VssSigningCredentials>(parameters.SigningCredentials, "SigningCredentials");
      byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}.{1}", (object) strArray[0], (object) strArray[1]));
      byte[] signature = strArray[2].FromBase64StringNoPadding();
      try
      {
        if (parameters.SigningCredentials.VerifySignature(bytes, signature))
          return;
      }
      catch (Exception ex)
      {
      }
      throw new SignatureValidationException();
    }

    private static void ValidateIssuer(
      JsonWebToken token,
      JsonWebTokenValidationParameters parameters)
    {
      ArgumentUtility.CheckForNull<JsonWebToken>(token, nameof (token));
      ArgumentUtility.CheckForNull<JsonWebTokenValidationParameters>(parameters, nameof (parameters));
      if (parameters.ValidateIssuer)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(token.Issuer, "Issuer");
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) parameters.ValidIssuers, "ValidIssuers");
        foreach (string validIssuer in parameters.ValidIssuers)
        {
          if (string.Equals(validIssuer, token.Issuer, StringComparison.OrdinalIgnoreCase))
            return;
        }
        throw new InvalidIssuerException();
      }
    }
  }
}
