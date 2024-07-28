// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CustomerSupportRequestCreationFailedMailNotification
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [NotificationEventBindings(EventSerializerType.Json, "ms.GalleryNotifications.csr-submit-GHIssue-creation-failed-event")]
  [DataContract]
  public class CustomerSupportRequestCreationFailedMailNotification : MailNotificationEventData
  {
    public CustomerSupportRequestCreationFailedMailNotification() => this.EventType = "ms.GalleryNotifications.csr-submit-GHIssue-creation-failed-event";

    [DataMember]
    public string Vsid { get; set; }

    [DataMember]
    public string GithubRepoLink { get; set; }

    [DataMember]
    public string EmailId { get; set; }

    [DataMember]
    public string ExtensionName { get; set; }

    [DataMember]
    public string PublisherName { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Reason { get; set; }

    [DataMember]
    public string SourceLink { get; set; }

    [DataMember]
    public Uri ExtensionURL { get; set; }

    [DataMember]
    public long ReviewId { get; set; }

    [DataMember]
    public DateTime ReviewDate { get; set; }

    [DataMember]
    public int ReviewRating { get; set; }

    [DataMember]
    public Guid Reviewer { get; set; }

    [DataMember]
    public string ReviewText { get; set; }

    [DataMember]
    public string GithubIssueCreateFailureReason { get; set; }

    public CustomerSupportRequestCreationFailedMailNotification(
      CustomerSupportRequest customerSupportData)
    {
      this.EventType = "ms.GalleryNotifications.csr-submit-GHIssue-creation-failed-event";
      this.Vsid = customerSupportData.ReporterVSID;
      this.EmailId = customerSupportData.EmailId;
      this.ExtensionName = customerSupportData.ExtensionName;
      this.PublisherName = customerSupportData.PublisherName;
      this.Reason = customerSupportData.Reason;
      this.Description = customerSupportData.Message;
      this.SourceLink = customerSupportData.SourceLink;
      this.ExtensionURL = customerSupportData.ExtensionURL;
      if (!(customerSupportData.SourceLink == "appealReview") || customerSupportData.Review == null)
        return;
      this.ReviewId = customerSupportData.Review.Id;
      this.ReviewText = customerSupportData.Review.Text;
      this.ReviewRating = (int) customerSupportData.Review.Rating;
      this.Reviewer = customerSupportData.Review.UserId;
      this.ReviewDate = customerSupportData.Review.UpdatedDate;
    }
  }
}
