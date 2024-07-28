// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Identity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Identity : IEquatable<Identity>, IComparable<Identity>, ISecuredObject
  {
    internal const string IsMruPropertyKey = "IsMru";
    private static readonly IEnumerable<string> LocalProperties = (IEnumerable<string>) new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer)
    {
      nameof (IsMru)
    };
    private static readonly IEnumerable<string> DirectoryObjectProperties = (IEnumerable<string>) new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer)
    {
      nameof (Active),
      nameof (DisplayName),
      nameof (SubjectDescriptor),
      nameof (Department),
      nameof (JobTitle),
      nameof (Mail),
      nameof (MailNickname),
      nameof (PhysicalDeliveryOfficeName),
      nameof (SamAccountName),
      nameof (SignInAddress),
      nameof (Surname),
      nameof (Description)
    };

    [JsonProperty("entityId")]
    public string EntityId { get; internal set; }

    [JsonProperty("entityType")]
    public string EntityType { get; internal set; }

    [JsonProperty("originDirectory")]
    public string OriginDirectory { get; internal set; }

    [JsonProperty("originId")]
    public string OriginId { get; internal set; }

    [JsonProperty("localDirectory")]
    public string LocalDirectory { get; internal set; }

    [JsonProperty("localId")]
    public string LocalId { get; internal set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; internal set; }

    [JsonProperty("scopeName")]
    public string ScopeName { get; internal set; }

    [JsonProperty("samAccountName")]
    public string SamAccountName => this[nameof (SamAccountName)] as string;

    [JsonProperty("active")]
    public bool? Active => this[nameof (Active)] as bool?;

    [JsonProperty("subjectDescriptor")]
    public Microsoft.VisualStudio.Services.Common.SubjectDescriptor? SubjectDescriptor => this[nameof (SubjectDescriptor)] as Microsoft.VisualStudio.Services.Common.SubjectDescriptor?;

    [JsonProperty("department")]
    public string Department => this[nameof (Department)] as string;

    [JsonProperty("jobTitle")]
    public string JobTitle => this[nameof (JobTitle)] as string;

    [JsonProperty("mail")]
    public string Mail => this[nameof (Mail)] as string;

    [JsonProperty("mailNickname")]
    public string MailNickname => this[nameof (MailNickname)] as string;

    [JsonProperty("physicalDeliveryOfficeName")]
    public string PhysicalDeliveryOfficeName => this[nameof (PhysicalDeliveryOfficeName)] as string;

    [JsonProperty("signInAddress")]
    public string SignInAddress => this[nameof (SignInAddress)] as string;

    [JsonProperty("surname")]
    public string Surname => this[nameof (Surname)] as string;

    [JsonProperty("guest")]
    public bool? Guest => this[nameof (Guest)] as bool?;

    [JsonProperty("telephoneNumber")]
    public string TelephoneNumber => this[nameof (TelephoneNumber)] as string;

    [JsonProperty("description")]
    public string Description => this[nameof (Description)] as string;

    [JsonProperty("isMru")]
    public bool? IsMru => this[nameof (IsMru)] as bool?;

    Guid ISecuredObject.NamespaceId => IdentityPickerSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => IdentityPickerSecurityConstants.RootToken;

    internal IDictionary<string, object> Properties { get; set; }

    public object this[string propertyName]
    {
      get
      {
        object obj = (object) null;
        if (this.Properties != null)
          this.Properties.TryGetValue(propertyName, out obj);
        return obj;
      }
      internal set
      {
        if (this.Properties == null || !this.Properties.ContainsKey(propertyName))
          return;
        this.Properties[propertyName] = value;
      }
    }

    public bool Equals(Identity other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.EntityId == other.EntityId && this.EntityType == other.EntityType;
    }

    public override int GetHashCode() => this.EntityId.GetHashCode() * 397 ^ this.EntityType.GetHashCode();

    public int CompareTo(Identity other)
    {
      int num1 = this.EntityId.CompareTo(other.EntityId);
      if (num1 != 0)
        return num1;
      int num2 = this.DisplayName.CompareTo(other.DisplayName);
      return num2 != 0 ? num2 : this.SignInAddress.CompareTo(other.SignInAddress);
    }

    internal static IEnumerable<string> AllPropertyKeys => Identity.DirectoryObjectProperties.Union<string>(Identity.LocalProperties);
  }
}
