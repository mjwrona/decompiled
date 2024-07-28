// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SimpleWebSecurityToken
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Web;

namespace Microsoft.Azure.NotificationHubs
{
  public class SimpleWebSecurityToken : SecurityToken
  {
    private const string InternalExpiresOnFieldName = "ExpiresOn";
    private const string InternalAudienceFieldName = "Audience";
    private const string InternalKeyValueSeparator = "=";
    private const string InternalPairSeparator = "&";
    private static readonly Func<string, string> Decoder = new Func<string, string>(HttpUtility.UrlDecode);
    private static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private readonly string id;
    private readonly string token;
    private readonly DateTime validFrom;
    private readonly DateTime validTo;
    private readonly string audience;

    public SimpleWebSecurityToken(string tokenString, DateTime expiry, string audience)
    {
      if (tokenString == null)
        throw new NullReferenceException(nameof (tokenString));
      if (audience == null)
        throw new NullReferenceException(nameof (audience));
      this.id = "uuid:" + Guid.NewGuid().ToString();
      this.token = tokenString;
      this.validFrom = DateTime.MinValue;
      this.validTo = expiry;
      this.audience = audience;
    }

    public SimpleWebSecurityToken(string tokenString, DateTime expiry)
    {
      if (tokenString == null)
        throw new NullReferenceException(nameof (tokenString));
      this.id = "uuid:" + Guid.NewGuid().ToString();
      this.token = tokenString;
      this.validFrom = DateTime.MinValue;
      this.validTo = expiry;
      this.audience = this.GetAudienceFromToken(tokenString);
    }

    public SimpleWebSecurityToken(string tokenString)
      : this("uuid:" + Guid.NewGuid().ToString(), tokenString)
    {
    }

    public SimpleWebSecurityToken(string id, string tokenString)
    {
      if (id == null)
        throw new NullReferenceException(nameof (id));
      if (tokenString == null)
        throw new NullReferenceException(nameof (tokenString));
      this.id = id;
      this.token = tokenString;
      this.GetExpirationDateAndAudienceFromToken(tokenString, out this.validTo, out this.audience);
    }

    public string Audience => this.audience;

    public DateTime ExpiresOn => this.validTo;

    protected virtual string ExpiresOnFieldName => "ExpiresOn";

    protected virtual string AudienceFieldName => "Audience";

    protected virtual string KeyValueSeparator => "=";

    protected virtual string PairSeparator => "&";

    public override string Id => this.id;

    public override ReadOnlyCollection<SecurityKey> SecurityKeys => new ReadOnlyCollection<SecurityKey>((IList<SecurityKey>) new List<SecurityKey>());

    public override DateTime ValidFrom => this.validFrom;

    public override DateTime ValidTo => this.validTo;

    public string Token => this.token;

    private string GetAudienceFromToken(string token)
    {
      string audienceFromToken;
      if (!SimpleWebSecurityToken.Decode(token, SimpleWebSecurityToken.Decoder, SimpleWebSecurityToken.Decoder, this.KeyValueSeparator, this.PairSeparator).TryGetValue(this.AudienceFieldName, out audienceFromToken))
        throw new FormatException(SRClient.TokenAudience);
      return audienceFromToken;
    }

    private void GetExpirationDateAndAudienceFromToken(
      string token,
      out DateTime expiresOn,
      out string audience)
    {
      IDictionary<string, string> dictionary = SimpleWebSecurityToken.Decode(token, SimpleWebSecurityToken.Decoder, SimpleWebSecurityToken.Decoder, this.KeyValueSeparator, this.PairSeparator);
      string s;
      if (!dictionary.TryGetValue(this.ExpiresOnFieldName, out s))
        throw new FormatException(SRClient.TokenExpiresOn);
      if (!dictionary.TryGetValue(this.AudienceFieldName, out audience))
        throw new FormatException(SRClient.TokenAudience);
      expiresOn = SimpleWebSecurityToken.EpochTime + TimeSpan.FromSeconds(double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    private static IDictionary<string, string> Decode(
      string encodedString,
      Func<string, string> keyDecoder,
      Func<string, string> valueDecoder,
      string keyValueSeparator,
      string pairSeparator)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      foreach (string str in (IEnumerable<string>) encodedString.Split(new string[1]
      {
        pairSeparator
      }, StringSplitOptions.None))
      {
        string[] strArray = str.Split(new string[1]
        {
          keyValueSeparator
        }, StringSplitOptions.None);
        if (strArray.Length != 2)
          throw new FormatException(SRClient.InvalidEncoding);
        dictionary.Add(keyDecoder(strArray[0]), valueDecoder(strArray[1]));
      }
      return dictionary;
    }
  }
}
