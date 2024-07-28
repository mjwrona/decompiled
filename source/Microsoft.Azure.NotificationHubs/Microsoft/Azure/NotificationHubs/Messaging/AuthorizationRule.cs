// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [DataContract(Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  [KnownType(typeof (AllowRule))]
  [KnownType(typeof (SharedAccessAuthorizationRule))]
  public abstract class AuthorizationRule
  {
    public const string NameIdentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public const string ShortNameIdentifierClaimType = "nameidentifier";
    public const string UpnClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
    public const string ShortUpnClaimType = "upn";
    public const string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    public const string RoleRoleClaimType = "role";
    public const string SharedAccessKeyClaimType = "sharedaccesskey";

    internal AuthorizationRule()
    {
      this.CreatedTime = DateTime.UtcNow;
      this.ModifiedTime = DateTime.UtcNow;
      this.Revision = 0L;
    }

    public string IssuerName
    {
      get => this.InternalIssuerName;
      set => this.InternalIssuerName = value;
    }

    public string ClaimType
    {
      get => this.InternalClaimType;
      set => this.InternalClaimType = value;
    }

    public string ClaimValue
    {
      get => this.InternalClaimValue;
      set => this.InternalClaimValue = value;
    }

    public IEnumerable<AccessRights> Rights
    {
      get => this.InternalRights;
      set
      {
        this.ValidateRights(value);
        this.InternalRights = value;
      }
    }

    public abstract string KeyName { get; set; }

    [DataMember(IsRequired = false, Order = 1006, EmitDefaultValue = false)]
    public DateTime CreatedTime { get; private set; }

    [DataMember(IsRequired = false, Order = 1007, EmitDefaultValue = false)]
    public DateTime ModifiedTime { get; private set; }

    [DataMember(IsRequired = false, Order = 1008, EmitDefaultValue = false)]
    public long Revision { get; set; }

    protected virtual void OnValidate()
    {
    }

    protected virtual void ValidateRights(IEnumerable<AccessRights> value)
    {
      if (value == null || !value.Any<AccessRights>() || value.Count<AccessRights>() > 3)
        throw new ArgumentException(SRClient.NullEmptyRights((object) 3));
      if (!AuthorizationRule.AreAccessRightsUnique(value))
        throw new ArgumentException(SRClient.CannotHaveDuplicateAccessRights);
    }

    internal void MarkModified()
    {
      this.ModifiedTime = DateTime.UtcNow;
      ++this.Revision;
    }

    internal void Validate()
    {
      if (this.Rights == null || !this.Rights.Any<AccessRights>() || this.Rights.Count<AccessRights>() > 3)
        throw new InvalidDataContractException(SRClient.NullEmptyRights((object) 3));
      if (!AuthorizationRule.AreAccessRightsUnique(this.Rights))
        throw new InvalidDataContractException(SRClient.CannotHaveDuplicateAccessRights);
      this.OnValidate();
    }

    public override int GetHashCode()
    {
      int hashCode = 0;
      string[] strArray = new string[3]
      {
        this.IssuerName,
        this.ClaimValue,
        this.ClaimType
      };
      foreach (string str in strArray)
      {
        if (!string.IsNullOrEmpty(str))
          hashCode += str.GetHashCode();
      }
      return hashCode;
    }

    public virtual AuthorizationRule Clone() => (AuthorizationRule) this.MemberwiseClone();

    public override bool Equals(object obj)
    {
      if (!(this.GetType() == obj.GetType()))
        return false;
      AuthorizationRule authorizationRule = (AuthorizationRule) obj;
      if (!string.Equals(this.IssuerName, authorizationRule.IssuerName, StringComparison.OrdinalIgnoreCase) || !string.Equals(this.ClaimType, authorizationRule.ClaimType, StringComparison.OrdinalIgnoreCase) || !string.Equals(this.ClaimValue, authorizationRule.ClaimValue, StringComparison.OrdinalIgnoreCase) || this.Rights != null && authorizationRule.Rights == null || this.Rights == null && authorizationRule.Rights != null)
        return false;
      if (this.Rights == null || authorizationRule.Rights == null)
        return true;
      HashSet<AccessRights> source = new HashSet<AccessRights>(this.Rights);
      HashSet<AccessRights> accessRightsSet = new HashSet<AccessRights>(authorizationRule.Rights);
      return accessRightsSet.Count == source.Count && source.All<AccessRights>(new Func<AccessRights, bool>(accessRightsSet.Contains));
    }

    [DataMember(Name = "IssuerName", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
    internal string InternalIssuerName { get; set; }

    [DataMember(Name = "ClaimType", IsRequired = false, Order = 1003, EmitDefaultValue = false)]
    internal string InternalClaimType { get; set; }

    [DataMember(Name = "ClaimValue", IsRequired = true, Order = 1004, EmitDefaultValue = false)]
    internal string InternalClaimValue { get; set; }

    [DataMember(Name = "Rights", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
    internal IEnumerable<AccessRights> InternalRights { get; set; }

    private static bool AreAccessRightsUnique(IEnumerable<AccessRights> rights)
    {
      HashSet<AccessRights> accessRightsSet = new HashSet<AccessRights>(rights);
      return rights.Count<AccessRights>() == accessRightsSet.Count;
    }
  }
}
