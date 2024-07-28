// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationSubscription
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class NotificationSubscription
  {
    private ReferenceLinks m_links;

    [DataMember]
    public string Id { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string UniqueId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, string> ExtendedProperties { get; set; }

    [DataMember]
    public IdentityRef Subscriber { get; set; }

    [DataMember]
    public SubscriptionStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StatusMessage { get; set; }

    [DataMember]
    public SubscriptionFlags Flags { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef LastModifiedBy { get; set; }

    [DataMember]
    public SubscriptionScope Scope { get; set; }

    [DataMember(IsRequired = true)]
    [JsonConverter(typeof (SubscriptionFilterConverter))]
    public ISubscriptionFilter Filter { get; set; }

    [DataMember]
    [JsonConverter(typeof (SubscriptionChannelConverter))]
    public ISubscriptionChannel Channel { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public SubscriptionAdminSettings AdminSettings { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public SubscriptionUserSettings UserSettings { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

    [DataMember]
    public SubscriptionPermissions Permissions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public SubscriptionDiagnostics Diagnostics { get; set; }
  }
}
