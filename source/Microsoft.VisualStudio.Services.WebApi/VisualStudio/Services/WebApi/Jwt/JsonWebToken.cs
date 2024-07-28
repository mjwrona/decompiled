// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Jwt.JsonWebToken
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebApi.Jwt
{
  [DataContract]
  [JsonConverter(typeof (JsonWebToken.JsonWebTokenConverter))]
  public sealed class JsonWebToken : IssuedToken
  {
    private const int DefaultLifetime = 300;
    private JsonWebToken.JWTHeader _header;
    private JsonWebToken.JWTPayload _payload;
    private byte[] _signature;
    private string _encodedToken;

    public static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      VssSigningCredentials credentials)
    {
      return JsonWebToken.Create(issuer, audience, validFrom, validTo, new DateTime(), (IEnumerable<Claim>) null, (JsonWebToken) null, (string) null, credentials, false, false);
    }

    public static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      IEnumerable<Claim> additionalClaims,
      JsonWebToken actor)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Claim>>(additionalClaims, nameof (additionalClaims));
      ArgumentUtility.CheckForNull<JsonWebToken>(actor, nameof (actor));
      return JsonWebToken.Create(issuer, audience, validFrom, validTo, new DateTime(), additionalClaims, actor, (string) null, (VssSigningCredentials) null, false, false);
    }

    public static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      IEnumerable<Claim> additionalClaims,
      string actorToken)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Claim>>(additionalClaims, nameof (additionalClaims));
      ArgumentUtility.CheckStringForNullOrEmpty(actorToken, nameof (actorToken));
      return JsonWebToken.Create(issuer, audience, validFrom, validTo, new DateTime(), additionalClaims, (JsonWebToken) null, actorToken, (VssSigningCredentials) null, false, false);
    }

    public static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      IEnumerable<Claim> additionalClaims,
      VssSigningCredentials credentials)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Claim>>(additionalClaims, nameof (additionalClaims));
      return JsonWebToken.Create(issuer, audience, validFrom, validTo, new DateTime(), additionalClaims, (JsonWebToken) null, (string) null, credentials, false, false);
    }

    public static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      IEnumerable<Claim> additionalClaims,
      VssSigningCredentials credentials,
      bool allowExpiredCertificate)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Claim>>(additionalClaims, nameof (additionalClaims));
      return JsonWebToken.Create(issuer, audience, validFrom, validTo, new DateTime(), additionalClaims, (JsonWebToken) null, (string) null, credentials, allowExpiredCertificate, false);
    }

    public static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      DateTime issuedAt,
      IEnumerable<Claim> additionalClaims,
      VssSigningCredentials credentials)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Claim>>(additionalClaims, nameof (additionalClaims));
      return JsonWebToken.Create(issuer, audience, validFrom, validTo, issuedAt, additionalClaims, (JsonWebToken) null, (string) null, credentials, false, false);
    }

    private static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      DateTime issuedAt,
      IEnumerable<Claim> additionalClaims,
      JsonWebToken actor,
      string actorToken,
      VssSigningCredentials credentials,
      bool allowExpiredCertificate,
      bool includeKeyId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(issuer, nameof (issuer));
      ArgumentUtility.CheckStringForNullOrEmpty(audience, nameof (audience));
      validFrom = validFrom == new DateTime() ? DateTime.UtcNow : validFrom.ToUniversalTime();
      validTo = validTo == new DateTime() ? DateTime.UtcNow + TimeSpan.FromSeconds(300.0) : validTo.ToUniversalTime();
      issuedAt = issuedAt == new DateTime() ? new DateTime() : issuedAt.ToUniversalTime();
      JsonWebToken.JWTHeader header = JsonWebToken.GetHeader(credentials, allowExpiredCertificate, includeKeyId);
      JsonWebToken.JWTPayload payload = new JsonWebToken.JWTPayload(additionalClaims)
      {
        Issuer = issuer,
        Audience = audience,
        ValidFrom = validFrom,
        ValidTo = validTo,
        IssuedAt = issuedAt
      };
      if (actor != null)
        payload.Actor = actor;
      else if (actorToken != null)
        payload.ActorToken = actorToken;
      byte[] signature = JsonWebToken.GetSignature(header, payload, header.Algorithm, credentials);
      return new JsonWebToken(header, payload, signature);
    }

    public static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      DateTime issuedAt,
      IEnumerable<Claim> additionalClaims)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(issuer, nameof (issuer));
      ArgumentUtility.CheckStringForNullOrEmpty(audience, nameof (audience));
      validFrom = validFrom == new DateTime() ? DateTime.UtcNow : validFrom.ToUniversalTime();
      validTo = validTo == new DateTime() ? DateTime.UtcNow + TimeSpan.FromSeconds(300.0) : validTo.ToUniversalTime();
      issuedAt = issuedAt == new DateTime() ? new DateTime() : issuedAt.ToUniversalTime();
      return new JsonWebToken(new JsonWebToken.JWTHeader()
      {
        Algorithm = JWTAlgorithm.None
      }, new JsonWebToken.JWTPayload(additionalClaims)
      {
        Issuer = issuer,
        Audience = audience,
        ValidFrom = validFrom,
        ValidTo = validTo,
        IssuedAt = issuedAt
      }, (byte[]) null);
    }

    public static JsonWebToken Create(
      string issuer,
      string audience,
      DateTime validFrom,
      DateTime validTo,
      DateTime issuedAt,
      IEnumerable<Claim> additionalClaims,
      VssSigningCredentials credentials,
      bool allowExpiredCertificate,
      bool includeKeyId)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Claim>>(additionalClaims, nameof (additionalClaims));
      return JsonWebToken.Create(issuer, audience, validFrom, validTo, issuedAt, additionalClaims, (JsonWebToken) null, (string) null, credentials, allowExpiredCertificate, includeKeyId);
    }

    public static JsonWebToken Create(string jwtEncodedString)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(jwtEncodedString, nameof (jwtEncodedString));
      return new JValue(jwtEncodedString).ToObject<JsonWebToken>();
    }

    private JsonWebToken()
    {
    }

    private JsonWebToken(
      JsonWebToken.JWTHeader header,
      JsonWebToken.JWTPayload payload,
      byte[] signature)
    {
      ArgumentUtility.CheckForNull<JsonWebToken.JWTHeader>(header, nameof (header));
      ArgumentUtility.CheckForNull<JsonWebToken.JWTPayload>(payload, nameof (payload));
      this._header = header;
      this._payload = payload;
      this._signature = signature;
    }

    protected internal override VssCredentialsType CredentialType => VssCredentialsType.S2S;

    internal override void ApplyTo(IHttpRequest request) => request.Headers.SetValue("Authorization", "Bearer " + this.EncodedToken);

    public string TokenType => this._header.Type;

    public JWTAlgorithm Algorithm => this._header.Algorithm;

    public string CertificateThumbprint => this._header.CertificateThumbprint;

    public string Audience => this._payload.Audience;

    public string Issuer => this._payload.Issuer;

    public string Subject => this._payload.Subject;

    public string NameIdentifier => this._payload.NameIdentifier;

    public string IdentityProvider => this._payload.IdentityProvider;

    public DateTime ValidTo => this._payload.ValidTo;

    public DateTime ValidFrom => this._payload.ValidFrom;

    public DateTime IssuedAt => this._payload.IssuedAt;

    public bool TrustedForDelegation => this._payload.TrustedForDelegation;

    public JsonWebToken Actor => this._payload.Actor;

    public string ApplicationIdentifier => this._payload.ApplicationIdentifier;

    public string EncodedToken
    {
      get
      {
        if (string.IsNullOrEmpty(this._encodedToken))
          this._encodedToken = this.Encode();
        return this._encodedToken;
      }
      private set => this._encodedToken = value;
    }

    public string Scopes => this._payload.Scopes;

    public override string ToString() => string.Format("{0}.{1}", (object) this._header.ToString(), (object) this._payload.ToString());

    internal IDictionary<string, object> Header => (IDictionary<string, object>) this._header;

    internal IDictionary<string, object> Payload => (IDictionary<string, object>) this._payload;

    internal byte[] Signature => this._signature;

    private static JsonWebToken.JWTHeader GetHeader(
      VssSigningCredentials credentials,
      bool allowExpired,
      bool includeKeyId)
    {
      JsonWebToken.JWTHeader headers = new JsonWebToken.JWTHeader();
      JWTAlgorithm jwtAlgorithm = JsonWebTokenUtilities.ValidateSigningCredentials(credentials, allowExpired);
      headers.Algorithm = jwtAlgorithm;
      if (jwtAlgorithm != JWTAlgorithm.None && credentials is IJsonWebTokenHeaderProvider tokenHeaderProvider)
        tokenHeaderProvider.SetHeaders((IDictionary<string, object>) headers);
      if (includeKeyId && !string.IsNullOrEmpty(credentials.KeyId))
        headers.KeyId = credentials.KeyId;
      return headers;
    }

    private static byte[] GetSignature(
      JsonWebToken.JWTHeader header,
      JsonWebToken.JWTPayload payload,
      VssSigningCredentials credentials,
      bool allowExpired)
    {
      JWTAlgorithm alg = JsonWebTokenUtilities.ValidateSigningCredentials(credentials, allowExpired);
      return JsonWebToken.GetSignature(header, payload, alg, credentials);
    }

    private static byte[] GetSignature(
      JsonWebToken.JWTHeader header,
      JsonWebToken.JWTPayload payload,
      JWTAlgorithm alg,
      VssSigningCredentials signingCredentials)
    {
      if (alg == JWTAlgorithm.None)
        return (byte[]) null;
      ArgumentUtility.CheckForNull<JsonWebToken.JWTHeader>(header, nameof (header));
      ArgumentUtility.CheckForNull<JsonWebToken.JWTPayload>(payload, nameof (payload));
      byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}.{1}", (object) header.JsonEncode<JsonWebToken.JWTHeader>(), (object) payload.JsonEncode<JsonWebToken.JWTPayload>()));
      if ((uint) (alg - 1) <= 1U)
        return signingCredentials.SignData(bytes);
      throw new InvalidOperationException();
    }

    private string Encode()
    {
      string str1 = this._header.JsonEncode<JsonWebToken.JWTHeader>();
      string str2 = this._payload.JsonEncode<JsonWebToken.JWTPayload>();
      string str3 = (string) null;
      if (this._signature != null)
        str3 = this._signature.ToBase64StringNoPadding();
      return string.Format("{0}.{1}.{2}", (object) str1, (object) str2, (object) str3);
    }

    private void OnDeserialized()
    {
      string[] strArray = !string.IsNullOrEmpty(this._encodedToken) ? this._encodedToken.Split('.') : throw new JsonWebTokenDeserializationException();
      this._header = strArray.Length == 3 ? JsonWebTokenUtilities.JsonDecode<JsonWebToken.JWTHeader>(strArray[0]) : throw new JsonWebTokenDeserializationException();
      this._payload = JsonWebTokenUtilities.JsonDecode<JsonWebToken.JWTPayload>(strArray[1]);
      if (string.IsNullOrEmpty(strArray[2]))
        return;
      this._signature = strArray[2].FromBase64StringNoPadding();
    }

    [JsonDictionary]
    private abstract class JWTSectionBase : Dictionary<string, object>
    {
      public override string ToString() => JsonConvert.SerializeObject((object) this, JsonWebTokenUtilities.DefaultSerializerSettings);

      protected T TryGetValueOrDefault<T>(string key)
      {
        object obj;
        if (!this.TryGetValue(key, out obj))
          return default (T);
        if (typeof (T) == typeof (DateTime))
          return (T) (ValueType) this.ConvertDateTime(obj);
        return typeof (T).GetTypeInfo().IsEnum && obj is string ? (T) Enum.Parse(typeof (T), (string) obj) : (T) Convert.ChangeType(obj, typeof (T));
      }

      protected DateTime ConvertDateTime(object obj) => obj is DateTime dateTime ? dateTime : Convert.ToInt64(obj).FromUnixEpochTime();
    }

    private class JWTHeader : JsonWebToken.JWTSectionBase
    {
      public JWTHeader() => this.Type = "JWT";

      internal string Type
      {
        get => this.TryGetValueOrDefault<string>("typ");
        set
        {
          if (string.IsNullOrEmpty(value))
            this.Remove("typ");
          else
            this["typ"] = (object) value;
        }
      }

      internal JWTAlgorithm Algorithm
      {
        get => this.TryGetValueOrDefault<JWTAlgorithm>("alg");
        set => this["alg"] = (object) value;
      }

      internal string CertificateThumbprint
      {
        get => this.TryGetValueOrDefault<string>("x5t");
        set
        {
          if (string.IsNullOrEmpty(value))
            this.Remove("x5t");
          else
            this["x5t"] = (object) value;
        }
      }

      internal string KeyId
      {
        get => this.TryGetValueOrDefault<string>("kid");
        set
        {
          if (string.IsNullOrEmpty(value))
            this.Remove("kid");
          else
            this["kid"] = (object) value;
        }
      }
    }

    private class JWTPayload : JsonWebToken.JWTSectionBase
    {
      private JsonWebToken _actorToken;

      public JWTPayload()
      {
      }

      internal JWTPayload(IEnumerable<Claim> claims) => this.AddRange<KeyValuePair<string, object>, JsonWebToken.JWTPayload>((IEnumerable<KeyValuePair<string, object>>) JsonWebTokenUtilities.TranslateToJwtClaims(claims.AsEmptyIfNull<Claim>()));

      internal string Audience
      {
        get => this.TryGetValueOrDefault<string>("aud");
        set
        {
          ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (Audience));
          this["aud"] = (object) value;
        }
      }

      internal string Issuer
      {
        get => this.TryGetValueOrDefault<string>("iss");
        set
        {
          ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (Issuer));
          this["iss"] = (object) value;
        }
      }

      internal string Subject
      {
        get => this.TryGetValueOrDefault<string>("sub");
        set
        {
          ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (Subject));
          this["sub"] = (object) value;
        }
      }

      internal string NameIdentifier
      {
        get => this.TryGetValueOrDefault<string>("nameid");
        set
        {
          if (string.IsNullOrEmpty(value))
            this.Remove("nameid");
          else
            this["nameid"] = (object) value;
        }
      }

      internal string IdentityProvider
      {
        get => this.TryGetValueOrDefault<string>("identityprovider");
        set
        {
          if (string.IsNullOrEmpty(value))
            this.Remove("identityprovider");
          else
            this["identityprovider"] = (object) value;
        }
      }

      internal DateTime ValidTo
      {
        get => this.TryGetValueOrDefault<DateTime>("exp");
        set => this["exp"] = (object) value;
      }

      internal DateTime ValidFrom
      {
        get => this.TryGetValueOrDefault<DateTime>("nbf");
        set => this["nbf"] = (object) value;
      }

      internal DateTime IssuedAt
      {
        get => this.TryGetValueOrDefault<DateTime>("iat");
        set
        {
          if (value == new DateTime())
            this.Remove("iat");
          else
            this["iat"] = (object) value;
        }
      }

      internal bool TrustedForDelegation
      {
        get => this.TryGetValueOrDefault<bool>("trustedfordelegation");
        set => this["trustedfordelegation"] = (object) value;
      }

      internal string ApplicationIdentifier
      {
        get => this.TryGetValueOrDefault<string>("appid");
        set
        {
          if (string.IsNullOrEmpty(value))
            this.Remove("appid");
          else
            this["appid"] = (object) value;
        }
      }

      internal JsonWebToken Actor
      {
        get
        {
          if (this._actorToken == null && this.TryGetValueOrDefault<string>("actort") != null)
            this._actorToken = JsonConvert.DeserializeObject<JsonWebToken>((string) this["actort"], JsonWebTokenUtilities.DefaultSerializerSettings);
          return this._actorToken;
        }
        set
        {
          if (value == null)
            this.Remove("actort");
          else
            this["actort"] = (object) JsonConvert.SerializeObject((object) value);
        }
      }

      internal string ActorToken
      {
        get => this.TryGetValueOrDefault<string>("actort");
        set => this["actort"] = (object) value;
      }

      internal string Scopes
      {
        get => this.TryGetValueOrDefault<string>("scp");
        set => this["scp"] = (object) value;
      }
    }

    internal class JsonWebTokenConverter : VssSecureJsonConverter
    {
      public override bool CanConvert(Type objectType) => objectType == typeof (JsonWebToken);

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        if (reader.TokenType == JsonToken.Null)
          return (object) null;
        if (reader.TokenType != JsonToken.String)
          throw new JsonWebTokenDeserializationException();
        JsonWebToken jsonWebToken = new JsonWebToken();
        jsonWebToken.EncodedToken = (string) reader.Value;
        jsonWebToken.OnDeserialized();
        return (object) jsonWebToken;
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        base.WriteJson(writer, value, serializer);
        if (!(value is JsonWebToken))
          throw new JsonWebTokenSerializationException();
        writer.WriteValue(((JsonWebToken) value).EncodedToken);
      }
    }
  }
}
