// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationCommonViewData
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  [ClientIncludeModel(RestClientLanguages.TypeScript)]
  public class NotificationCommonViewData
  {
    [DataMember]
    public bool IsAdminMode { get; set; }

    [DataMember]
    public NotificationAdminSettings AdminSettings { get; set; }

    [DataMember]
    public IdentityRef SubscriberIdentity { get; set; }

    [DataMember]
    public NotificationSubscriber Subscriber { get; set; }

    [DataMember]
    public string SubscriberEmail { get; set; }

    [DataMember]
    public bool IsSubscriberEmailPending { get; set; }

    [DataMember]
    public bool HasManagePermission { get; set; }

    [DataMember]
    public string DefaultServiceInstanceType { get; set; }

    [DataMember]
    public string AdminPageUrl { get; set; }

    [DataMember]
    public IEnumerable<NotificationEventPublisher> EventsPublishers { get; set; }

    [DataMember]
    public string LastUsedTeamProjectName { get; set; }

    [DataMember]
    public Guid ProjectValidUsersId { get; set; }

    [DataMember]
    public bool AsciiOnlyAddresses { get; set; }
  }
}
