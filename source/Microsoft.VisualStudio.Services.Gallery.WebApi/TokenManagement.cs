// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.TokenManagement
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public static class TokenManagement
  {
    private const string s_encodedHeader = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9";
    private static DateTime s_startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private static char[] s_fullTokenSeperator = new char[1]
    {
      '.'
    };
    private const string s_IssuerClaim = "iss";
    private const string s_SubjectClaim = "sub";
    private const string s_AudienceClaim = "aud";
    private const string s_ExpirationClaim = "exp";
    private const string s_NotBeforeClaim = "nbf";
    private const string s_IssuedTimeClaim = "iat";
    private const string s_IdentifierClaim = "jti";
    private static HashSet<string> s_restrictedElements = new HashSet<string>()
    {
      "iss",
      "sub",
      "aud",
      "exp",
      "nbf",
      "iat",
      "jti"
    };

    public static byte[] CreateSigningKey(int keyLength)
    {
      if (keyLength % 8 != 0)
        throw new ArgumentException(GalleryWebApiResources.KeyLengthMustBeDivisibleByEight((object) keyLength));
      RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
      byte[] signingKey = new byte[keyLength / 8];
      byte[] data = signingKey;
      randomNumberGenerator.GetBytes(data);
      return signingKey;
    }

    public static string GenerateJwtToken(
      JwtClaims jwtClaims,
      byte[] signingKey,
      int expirationInSeconds)
    {
      return TokenManagement.GenerateJwtToken(new JwtClaims(jwtClaims)
      {
        Expiration = new DateTime?(DateTime.UtcNow.AddSeconds((double) expirationInSeconds))
      }, signingKey);
    }

    public static string GenerateJwtToken(JwtClaims jwtClaims, byte[] signingKey)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (jwtClaims.ExtraClaims != null)
      {
        foreach (string key in jwtClaims.ExtraClaims.Keys)
        {
          if (TokenManagement.s_restrictedElements.Contains(key))
            throw new InvalidTokenException(GalleryWebApiResources.InvalidElementForToken((object) key));
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IDictionary<string, string>) jwtClaims.ExtraClaims, (IEqualityComparer<string>) StringComparer.Ordinal);
      if (!string.IsNullOrEmpty(jwtClaims.Issuer))
        dictionary["iss"] = jwtClaims.Issuer;
      if (!string.IsNullOrEmpty(jwtClaims.Subject))
        dictionary["sub"] = jwtClaims.Subject;
      if (!string.IsNullOrEmpty(jwtClaims.Audience))
        dictionary["aud"] = jwtClaims.Audience;
      if (!string.IsNullOrEmpty(jwtClaims.Identifier))
        dictionary["jti"] = jwtClaims.Identifier;
      if (jwtClaims.IssuedTime.HasValue)
        dictionary["iat"] = TokenManagement.GetNumericDateValue(jwtClaims.IssuedTime.Value, "iat");
      if (jwtClaims.NotBefore.HasValue)
        dictionary["nbf"] = TokenManagement.GetNumericDateValue(jwtClaims.NotBefore.Value, "nbf");
      if (jwtClaims.Expiration.HasValue)
        dictionary["exp"] = TokenManagement.GetNumericDateValue(jwtClaims.Expiration.Value, "exp");
      string stringToHash = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9." + Convert.ToBase64String(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject((object) dictionary)));
      string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(TokenManagement.GetHashValue(stringToHash, signingKey)));
      return stringToHash + "." + base64String;
    }

    public static JwtClaims ParseJwtToken(string signedToken, byte[] signingKey)
    {
      JwtClaims jwtToken = new JwtClaims();
      string[] strArray = signedToken.Split(TokenManagement.s_fullTokenSeperator, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 3)
        throw new InvalidTokenException(GalleryWebApiResources.InvalidSignedToken((object) GalleryWebApiResources.ThereMustBeElementsAndSignature()));
      if (Convert.ToBase64String(Encoding.ASCII.GetBytes(TokenManagement.GetHashValue(strArray[0] + "." + strArray[1], signingKey))) != strArray[2])
        throw new InvalidTokenException(GalleryWebApiResources.InvalidSignedToken((object) GalleryWebApiResources.TheSignatureMustMatchTheElements()));
      strArray[0] = Encoding.ASCII.GetString(Convert.FromBase64String(strArray[0]));
      strArray[1] = Encoding.ASCII.GetString(Convert.FromBase64String(strArray[1]));
      JwtHeader jwtHeader = JsonConvert.DeserializeObject<JwtHeader>(strArray[0]);
      if (!string.Equals(jwtHeader.TokenType, "JWT") || !string.Equals(jwtHeader.Algorithm, "HS256"))
        throw new InvalidTokenException(GalleryWebApiResources.UnsupportedTokenFormat());
      Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(strArray[1]);
      string s;
      if (dictionary.TryGetValue("exp", out s))
      {
        jwtToken.Expiration = new DateTime?(TokenManagement.s_startTime.AddSeconds((double) int.Parse(s)));
        dictionary.Remove("exp");
      }
      if (jwtToken.Expiration.HasValue && DateTime.UtcNow >= jwtToken.Expiration.Value)
        throw new InvalidTokenException(GalleryWebApiResources.InvalidSignedToken((object) GalleryWebApiResources.PrivateTokenHasExpired()));
      if (dictionary.TryGetValue("iss", out s))
      {
        jwtToken.Issuer = s;
        dictionary.Remove("iss");
      }
      if (dictionary.TryGetValue("sub", out s))
      {
        jwtToken.Subject = s;
        dictionary.Remove("sub");
      }
      if (dictionary.TryGetValue("aud", out s))
      {
        jwtToken.Audience = s;
        dictionary.Remove("aud");
      }
      if (dictionary.TryGetValue("jti", out s))
      {
        jwtToken.Identifier = s;
        dictionary.Remove("jti");
      }
      jwtToken.ExtraClaims = dictionary;
      return jwtToken;
    }

    private static string GetHashValue(string stringToHash, byte[] hashKey) => Convert.ToBase64String(new HMACSHA256(hashKey).ComputeHash(Encoding.ASCII.GetBytes(stringToHash.ToString())));

    private static string GetNumericDateValue(DateTime dateTime, string dateName)
    {
      double totalSeconds = (dateTime - TokenManagement.s_startTime).TotalSeconds;
      return totalSeconds >= 0.0 ? Math.Round(totalSeconds).ToString() : throw new ArgumentException(GalleryWebApiResources.InvalidTokenDate((object) dateName));
    }
  }
}
