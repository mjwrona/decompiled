// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.SharedAccessAuthorizationRule
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [DataContract(Name = "SharedAccessAuthorizationRule", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class SharedAccessAuthorizationRule : AuthorizationRule
  {
    private const int SupportedSASKeyLength = 44;
    private const string FixedClaimType = "SharedAccessKey";
    private const string FixedClaimValue = "None";
    public static readonly DataContractSerializer Serializer = new DataContractSerializer(typeof (SharedAccessAuthorizationRule));

    private SharedAccessAuthorizationRule()
    {
    }

    public SharedAccessAuthorizationRule(string keyName, IEnumerable<AccessRights> rights)
      : this(keyName, SharedAccessAuthorizationRule.GenerateRandomKey(), SharedAccessAuthorizationRule.GenerateRandomKey(), rights)
    {
    }

    public SharedAccessAuthorizationRule(
      string keyName,
      string primaryKey,
      IEnumerable<AccessRights> rights)
      : this(keyName, primaryKey, SharedAccessAuthorizationRule.GenerateRandomKey(), rights)
    {
    }

    public SharedAccessAuthorizationRule(
      string keyName,
      string primaryKey,
      string secondaryKey,
      IEnumerable<AccessRights> rights)
    {
      this.ClaimType = "SharedAccessKey";
      this.ClaimValue = "None";
      this.PrimaryKey = primaryKey;
      this.SecondaryKey = secondaryKey;
      this.Rights = rights;
      this.KeyName = keyName;
    }

    protected override void OnValidate()
    {
      if (string.IsNullOrEmpty(this.InternalKeyName) || !string.Equals(this.InternalKeyName, HttpUtility.UrlEncode(this.InternalKeyName)))
        throw new InvalidDataContractException(SRCore.SharedAccessAuthorizationRuleKeyContainsInvalidCharacters);
      if (this.InternalKeyName.Length > 256)
        throw new InvalidDataContractException(SRCore.SharedAccessAuthorizationRuleKeyNameTooBig((object) 256));
      if (string.IsNullOrEmpty(this.InternalPrimaryKey))
        throw new InvalidDataContractException(SRCore.SharedAccessAuthorizationRuleRequiresPrimaryKey);
      if (Encoding.ASCII.GetByteCount(this.InternalPrimaryKey) != 44)
        throw new InvalidDataContractException(SRCore.SharedAccessRuleAllowsFixedLengthKeys((object) 44));
      if (!SharedAccessAuthorizationRule.CheckBase64(this.InternalPrimaryKey))
        throw new InvalidDataContractException(SRCore.SharedAccessKeyShouldbeBase64);
      if (!string.IsNullOrEmpty(this.InternalSecondaryKey))
      {
        if (Encoding.ASCII.GetByteCount(this.InternalSecondaryKey) != 44)
          throw new InvalidDataContractException(SRCore.SharedAccessRuleAllowsFixedLengthKeys((object) 44));
        if (!SharedAccessAuthorizationRule.CheckBase64(this.InternalSecondaryKey))
          throw new InvalidDataContractException(SRCore.SharedAccessKeyShouldbeBase64);
      }
      if (!SharedAccessAuthorizationRule.IsValidCombinationOfRights(this.Rights))
        throw new InvalidDataContractException(SRClient.InvalidCombinationOfManageRight);
    }

    private static bool CheckBase64(string base64EncodedString)
    {
      try
      {
        Convert.FromBase64String(base64EncodedString);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public override sealed string KeyName
    {
      get => this.InternalKeyName;
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          throw new ArgumentNullException(nameof (KeyName));
        if (!string.Equals(this.InternalKeyName, HttpUtility.UrlEncode(this.InternalKeyName)))
          throw new ArgumentException(SRCore.SharedAccessAuthorizationRuleKeyContainsInvalidCharacters);
        this.InternalKeyName = value.Length <= 256 ? value : throw new ArgumentOutOfRangeException(nameof (KeyName), SRCore.ArgumentStringTooBig((object) nameof (KeyName), (object) 256));
      }
    }

    public string PrimaryKey
    {
      get => this.InternalPrimaryKey;
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          throw new ArgumentNullException(nameof (PrimaryKey));
        this.InternalPrimaryKey = value.Length <= 256 ? value : throw new ArgumentOutOfRangeException(nameof (PrimaryKey), SRCore.ArgumentStringTooBig((object) nameof (PrimaryKey), (object) 256));
      }
    }

    public string SecondaryKey
    {
      get => this.InternalSecondaryKey;
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          this.InternalSecondaryKey = value;
        else if (value.Length > 256)
          throw new ArgumentOutOfRangeException(nameof (SecondaryKey), SRCore.ArgumentStringTooBig((object) nameof (SecondaryKey), (object) 256));
        this.InternalSecondaryKey = value;
      }
    }

    protected override void ValidateRights(IEnumerable<AccessRights> value)
    {
      base.ValidateRights(value);
      if (!SharedAccessAuthorizationRule.IsValidCombinationOfRights(value))
        throw new ArgumentException(SRClient.InvalidCombinationOfManageRight);
    }

    private static bool IsValidCombinationOfRights(IEnumerable<AccessRights> rights) => !rights.Contains<AccessRights>(AccessRights.Manage) || rights.Count<AccessRights>() == 3;

    public override int GetHashCode()
    {
      int hashCode = base.GetHashCode();
      string[] strArray = new string[3]
      {
        this.KeyName,
        this.PrimaryKey,
        this.SecondaryKey
      };
      foreach (string str in strArray)
      {
        if (!string.IsNullOrEmpty(str))
          hashCode += str.GetHashCode();
      }
      return hashCode;
    }

    public override bool Equals(object obj)
    {
      if (!base.Equals(obj))
        return false;
      SharedAccessAuthorizationRule authorizationRule = (SharedAccessAuthorizationRule) obj;
      return string.Equals(this.KeyName, authorizationRule.KeyName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.PrimaryKey, authorizationRule.PrimaryKey, StringComparison.OrdinalIgnoreCase) && string.Equals(this.SecondaryKey, authorizationRule.SecondaryKey, StringComparison.OrdinalIgnoreCase);
    }

    [DataMember(Name = "KeyName", IsRequired = true, Order = 1001, EmitDefaultValue = false)]
    internal string InternalKeyName { get; set; }

    [DataMember(Name = "PrimaryKey", IsRequired = true, Order = 1002, EmitDefaultValue = false)]
    internal string InternalPrimaryKey { get; set; }

    [DataMember(Name = "SecondaryKey", IsRequired = false, Order = 1003, EmitDefaultValue = false)]
    internal string InternalSecondaryKey { get; set; }

    public static string GenerateRandomKey()
    {
      byte[] numArray = new byte[32];
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
        cryptoServiceProvider.GetBytes(numArray);
      return Convert.ToBase64String(numArray);
    }
  }
}
