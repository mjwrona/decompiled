// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.Client.AccountCreateInfoInternal
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Account.Client
{
  [DataContract(Name = "AccountCreateInfo")]
  internal class AccountCreateInfoInternal
  {
    internal AccountCreateInfoInternal(
      string name,
      string organization,
      Guid creatorId,
      CultureInfo language = null,
      CultureInfo culture = null,
      TimeZoneInfo timeZone = null,
      IDictionary<string, object> properties = null)
    {
      this.Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof (name));
      this.Organization = organization;
      this.Creator = creatorId;
      if (language != null || culture != null || timeZone != null)
        this.Preferences = new AccountCreateInfoInternal.AccountPreferencesInternal(language, culture, timeZone);
      this.Properties = properties == null ? new PropertiesCollection() : new PropertiesCollection(properties);
    }

    internal AccountCreateInfoInternal(
      string name,
      string organization,
      Guid creatorId,
      List<KeyValuePair<Guid, Guid>> serviceDefinitions,
      CultureInfo language = null,
      CultureInfo culture = null,
      TimeZoneInfo timeZone = null,
      IDictionary<string, object> properties = null)
      : this(name, organization, creatorId, language, culture, timeZone, properties)
    {
      this.ServiceDefinitions = serviceDefinitions;
    }

    [DataMember(Name = "AccountName")]
    public string Name { get; private set; }

    [DataMember]
    public string Organization { get; private set; }

    [DataMember]
    public Guid Creator { get; private set; }

    [DataMember]
    public AccountCreateInfoInternal.AccountPreferencesInternal Preferences { get; private set; }

    [DataMember]
    public PropertiesCollection Properties { get; private set; }

    [DataMember]
    public List<KeyValuePair<Guid, Guid>> ServiceDefinitions { get; set; }

    [DataContract(Name = "AccountPreferences")]
    internal class AccountPreferencesInternal
    {
      internal AccountPreferencesInternal(
        CultureInfo language,
        CultureInfo culture,
        TimeZoneInfo timeZone)
      {
        this.Language = language;
        this.Culture = culture;
        this.TimeZone = timeZone;
      }

      [DataMember]
      public CultureInfo Language { get; private set; }

      [DataMember]
      public CultureInfo Culture { get; private set; }

      [DataMember]
      public TimeZoneInfo TimeZone { get; private set; }
    }
  }
}
