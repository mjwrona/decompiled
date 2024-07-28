// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.Profile
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  [DataContract]
  public class Profile : ITimeStamped, IVersioned, ICloneable
  {
    internal const string CoreContainerName = "Core";

    public Profile() => this.CoreAttributes = (IDictionary<string, CoreProfileAttribute>) new Dictionary<string, CoreProfileAttribute>((IEqualityComparer<string>) VssStringComparer.AttributesDescriptor);

    public string DisplayName
    {
      get => this.GetAttributeFromCoreContainer<string>(nameof (DisplayName), (string) null);
      set => this.SetAttributeInCoreContainer(nameof (DisplayName), (object) value);
    }

    public string PublicAlias
    {
      get => this.GetAttributeFromCoreContainer<string>(nameof (PublicAlias), (string) null);
      set => this.SetAttributeInCoreContainer(nameof (PublicAlias), (object) value);
    }

    public string CountryName
    {
      get => this.GetAttributeFromCoreContainer<string>(nameof (CountryName), (string) null);
      set => this.SetAttributeInCoreContainer(nameof (CountryName), (object) value);
    }

    public string EmailAddress
    {
      get => this.GetAttributeFromCoreContainer<string>(nameof (EmailAddress), (string) null);
      set => this.SetAttributeInCoreContainer(nameof (EmailAddress), (object) value);
    }

    public string UnconfirmedEmailAddress
    {
      get => this.GetAttributeFromCoreContainer<string>(nameof (UnconfirmedEmailAddress), (string) null);
      set => this.SetAttributeInCoreContainer(nameof (UnconfirmedEmailAddress), (object) value);
    }

    public DateTimeOffset CreatedDateTime
    {
      get => this.GetAttributeFromCoreContainer<DateTimeOffset>("DateCreated", new DateTimeOffset());
      set => this.SetAttributeInCoreContainer("DateCreated", (object) value);
    }

    public Avatar Avatar
    {
      get => this.GetAttributeFromCoreContainer<Avatar>(nameof (Avatar), (Avatar) null);
      set => this.SetAttributeInCoreContainer(nameof (Avatar), (object) value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public AttributesContainer ApplicationContainer { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal IDictionary<string, CoreProfileAttribute> CoreAttributes { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int CoreRevision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTimeOffset TimeStamp { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid Id { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false)]
    public ProfileState ProfileState { get; set; }

    public int TermsOfServiceVersion
    {
      get => this.GetAttributeFromCoreContainer<int>(nameof (TermsOfServiceVersion), 0);
      set => this.SetAttributeInCoreContainer(nameof (TermsOfServiceVersion), (object) value);
    }

    public DateTimeOffset TermsOfServiceAcceptDate
    {
      get => this.GetAttributeFromCoreContainer<DateTimeOffset>(nameof (TermsOfServiceAcceptDate), new DateTimeOffset());
      set => this.SetAttributeInCoreContainer(nameof (TermsOfServiceAcceptDate), (object) value);
    }

    public bool? ContactWithOffers
    {
      get
      {
        CoreProfileAttribute profileAttribute;
        this.CoreAttributes.TryGetValue(nameof (ContactWithOffers), out profileAttribute);
        return profileAttribute != null && profileAttribute.Value != null && profileAttribute.Value is bool ? (bool?) profileAttribute.Value : new bool?();
      }
      set => this.SetAttributeInCoreContainer(nameof (ContactWithOffers), (object) value);
    }

    private T GetAttributeFromCoreContainer<T>(string attributeName, T defaultValue)
    {
      CoreProfileAttribute profileAttribute;
      this.CoreAttributes.TryGetValue(attributeName, out profileAttribute);
      return profileAttribute != null && profileAttribute.Value != null && profileAttribute.Value.GetType() == typeof (T) ? (T) profileAttribute.Value : defaultValue;
    }

    private void SetAttributeInCoreContainer(string attributeName, object value)
    {
      CoreProfileAttribute profileAttribute1;
      if (this.CoreAttributes.TryGetValue(attributeName, out profileAttribute1))
      {
        profileAttribute1.Value = value;
      }
      else
      {
        IDictionary<string, CoreProfileAttribute> coreAttributes = this.CoreAttributes;
        string key = attributeName;
        CoreProfileAttribute profileAttribute2 = new CoreProfileAttribute();
        profileAttribute2.Descriptor = new AttributeDescriptor("Core", attributeName);
        profileAttribute2.Value = value;
        coreAttributes.Add(key, profileAttribute2);
      }
    }

    public CoreProfileAttribute GetCoreAttribute(string attributeName)
    {
      CoreProfileAttribute profileAttribute;
      this.CoreAttributes.TryGetValue(attributeName, out profileAttribute);
      return profileAttribute == null ? (CoreProfileAttribute) null : (CoreProfileAttribute) profileAttribute.Clone();
    }

    public object Clone()
    {
      Microsoft.VisualStudio.Services.Profile.Profile profile = this.MemberwiseClone() as Microsoft.VisualStudio.Services.Profile.Profile;
      profile.CoreAttributes = this.CoreAttributes != null ? (IDictionary<string, CoreProfileAttribute>) this.CoreAttributes.ToDictionary<KeyValuePair<string, CoreProfileAttribute>, string, CoreProfileAttribute>((Func<KeyValuePair<string, CoreProfileAttribute>, string>) (x => x.Key), (Func<KeyValuePair<string, CoreProfileAttribute>, CoreProfileAttribute>) (x => (CoreProfileAttribute) x.Value.Clone())) : (IDictionary<string, CoreProfileAttribute>) null;
      profile.ApplicationContainer = this.ApplicationContainer != null ? (AttributesContainer) this.ApplicationContainer.Clone() : (AttributesContainer) null;
      return (object) profile;
    }

    internal class CoreAttributeNames
    {
      internal const string DisplayName = "DisplayName";
      internal const string PublicAlias = "PublicAlias";
      internal const string EmailAddress = "EmailAddress";
      internal const string DefaultEmailAddress = "DefaultEmailAddress";
      internal const string UnconfirmedEmailAddress = "UnconfirmedEmailAddress";
      internal const string CountryName = "CountryName";
      internal const string Avatar = "Avatar";
      internal const string TermsOfServiceVersion = "TermsOfServiceVersion";
      internal const string TermsOfServiceAcceptDate = "TermsOfServiceAcceptDate";
      internal const string ContactWithOffers = "ContactWithOffers";
      internal const string DateCreated = "DateCreated";
      internal static readonly List<string> AttributeNameList = new List<string>()
      {
        nameof (DisplayName),
        nameof (PublicAlias),
        nameof (EmailAddress),
        nameof (UnconfirmedEmailAddress),
        nameof (CountryName),
        nameof (Avatar),
        nameof (TermsOfServiceVersion),
        nameof (TermsOfServiceAcceptDate),
        nameof (ContactWithOffers),
        nameof (DateCreated)
      };
    }
  }
}
