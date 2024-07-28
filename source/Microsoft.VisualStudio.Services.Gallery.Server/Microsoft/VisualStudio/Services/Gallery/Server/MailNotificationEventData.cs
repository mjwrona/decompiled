// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MailNotificationEventData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DataContract]
  public abstract class MailNotificationEventData
  {
    [DataMember]
    public string CreatedDate { get; set; }

    [DataMember]
    public string UserDisplayName { get; set; }

    [DataMember]
    public string UnsubscribeUrl { get; set; }

    [DataMember]
    public string Subject { get; set; }

    [DataMember]
    public string IntroductionNote { get; set; }

    [DataMember]
    public string HeaderNote { get; set; }

    [DataMember]
    public string NotificationContent { get; set; }

    [DataMember]
    public string ActionButtonText { get; set; }

    [DataMember]
    public string ActionButtonUrl { get; set; }

    public string EventType { get; set; }

    public string GetAnchorText(string title, string link) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<a href ='{0}' style= 'text-decoration:none' target='_blank'>{1}</a>", (object) link, (object) title);
  }
}
