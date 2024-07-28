// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationEventType
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class NotificationEventType
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    public EventSerializerType SerializationFormat { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember]
    public NotificationEventPublisher EventPublisher { get; set; }

    public string Alias { get; set; }

    [DataMember]
    public NotificationEventTypeCategory Category { get; set; }

    [DataMember]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, NotificationEventField> Fields { get; set; }

    [DataMember]
    public List<NotificationEventRole> Roles { get; set; }

    [DataMember]
    public List<string> SupportedScopes { get; set; }

    [DataMember]
    public bool HasInitiator { get; set; }

    public bool HasDynamicRoles { get; set; }

    [DataMember]
    public bool CustomSubscriptionsAllowed { get; set; }

    [DataMember]
    public string Icon { get; set; }

    [DataMember]
    public string Color { get; set; }

    internal bool HasGroupRoles { get; set; }

    public void AddField(string fieldId)
    {
      if (this.Fields == null)
        this.Fields = new Dictionary<string, NotificationEventField>();
      this.Fields.Add(fieldId, (NotificationEventField) null);
    }

    public NotificationEventType ShallowClone() => new NotificationEventType()
    {
      Id = this.Id,
      Name = this.Name,
      SerializationFormat = this.SerializationFormat,
      Url = this.Url,
      EventPublisher = this.EventPublisher,
      Alias = this.Alias,
      Category = this.Category,
      Fields = this.Fields,
      Roles = this.Roles,
      SupportedScopes = this.SupportedScopes,
      HasInitiator = this.HasInitiator,
      HasDynamicRoles = this.HasDynamicRoles,
      CustomSubscriptionsAllowed = this.CustomSubscriptionsAllowed,
      Icon = this.Icon,
      Color = this.Color,
      HasGroupRoles = this.HasGroupRoles
    };
  }
}
