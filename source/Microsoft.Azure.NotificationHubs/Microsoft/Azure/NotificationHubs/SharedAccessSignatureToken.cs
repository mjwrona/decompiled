// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SharedAccessSignatureToken
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.Azure.NotificationHubs
{
  public class SharedAccessSignatureToken : SimpleWebSecurityToken
  {
    public const int MaxKeyNameLength = 256;
    public const int MaxKeyLength = 256;
    public const string SharedAccessSignature = "SharedAccessSignature";
    public const string SignedResource = "sr";
    public const string Signature = "sig";
    public const string SignedKeyName = "skn";
    public const string SignedExpiry = "se";
    public const string SignedResourceFullFieldName = "SharedAccessSignature sr";
    public const string SasKeyValueSeparator = "=";
    public const string SasPairSeparator = "&";

    public SharedAccessSignatureToken(string tokenString, DateTime expiry, string audience)
      : base(tokenString, expiry, audience)
    {
    }

    public SharedAccessSignatureToken(string tokenString, DateTime expiry)
      : base(tokenString, expiry)
    {
    }

    public SharedAccessSignatureToken(string tokenString)
      : this("uuid:" + Guid.NewGuid().ToString(), tokenString)
    {
    }

    public SharedAccessSignatureToken(string id, string tokenString)
      : base(id, tokenString)
    {
    }

    protected override string AudienceFieldName => "SharedAccessSignature sr";

    protected override string ExpiresOnFieldName => "se";

    protected override string KeyValueSeparator => "=";

    protected override string PairSeparator => "&";

    internal static void Validate(string sharedAccessSignature)
    {
      IDictionary<string, string> dictionary = !string.IsNullOrEmpty(sharedAccessSignature) ? SharedAccessSignatureToken.ExtractFieldValues(sharedAccessSignature) : throw new ArgumentNullException(nameof (sharedAccessSignature));
      if (!dictionary.TryGetValue("sig", out string _))
        throw new ArgumentNullException("sig");
      if (!dictionary.TryGetValue("se", out string _))
        throw new ArgumentNullException("se");
      if (!dictionary.TryGetValue("skn", out string _))
        throw new ArgumentNullException("skn");
      if (!dictionary.TryGetValue("sr", out string _))
        throw new ArgumentNullException("sr");
    }

    private static IDictionary<string, string> ExtractFieldValues(string sharedAccessSignature)
    {
      string[] strArray1 = sharedAccessSignature.Split();
      if (!string.Equals(strArray1[0].Trim(), "SharedAccessSignature", StringComparison.OrdinalIgnoreCase) || strArray1.Length != 2)
        throw new ArgumentNullException(nameof (sharedAccessSignature));
      IDictionary<string, string> fieldValues = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string str1 = strArray1[1].Trim();
      string[] separator = new string[1]{ "&" };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.None))
      {
        if (str2 != string.Empty)
        {
          string[] strArray2 = str2.Split(new string[1]
          {
            "="
          }, StringSplitOptions.None);
          if (string.Equals(strArray2[0], "sr", StringComparison.OrdinalIgnoreCase))
            fieldValues.Add(strArray2[0], strArray2[1]);
          else
            fieldValues.Add(strArray2[0], HttpUtility.UrlDecode(strArray2[1]));
        }
      }
      return fieldValues;
    }
  }
}
