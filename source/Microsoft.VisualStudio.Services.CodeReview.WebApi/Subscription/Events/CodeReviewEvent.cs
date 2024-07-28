// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events.CodeReviewEvent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events
{
  public class CodeReviewEvent : ICodeReviewEvent
  {
    protected CodeReviewEventNotification codeReviewNotification;

    public CodeReviewEvent()
    {
    }

    public CodeReviewEvent(
      CodeReviewEventNotification codeReviewNotification,
      Review review,
      string reviewEventType)
    {
      this.codeReviewNotification = codeReviewNotification;
      this.ProjectId = codeReviewNotification.ProjectId;
      this.CodeReview = review;
      this.EventTrigger = codeReviewNotification.GetType().Name;
      this.ReviewEventType = reviewEventType;
      if (review.Reviewers != null)
      {
        review.Reviewers = (IList<Reviewer>) ReviewerExtensions.GetSortedReviewers(review.Reviewers);
        List<EventReviewer> eventReviewerList = new List<EventReviewer>();
        foreach (Reviewer reviewer in (IEnumerable<Reviewer>) review.Reviewers)
        {
          EventReviewer eventReviewer = new EventReviewer(reviewer);
          eventReviewerList.Add(eventReviewer);
          if (eventReviewer.IsRequiredKind)
            this.HasRequiredReviewers = true;
          else
            this.HasOptionalReviewers = true;
        }
        this.Reviewers = eventReviewerList;
      }
      if (this.CodeReview.Iterations != null && this.CodeReview.Iterations.Count > 0)
        this.LatestIterationDescription = this.CodeReview.Iterations.Last<Iteration>().Description;
      else
        this.LatestIterationDescription = this.CodeReview.Description;
    }

    public Guid GetActor()
    {
      Guid result = Guid.Empty;
      if (this.EventTriggeredBy != null)
        Guid.TryParse(this.EventTriggeredBy.Id, out result);
      return result;
    }

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public Review CodeReview { get; set; }

    [DataMember]
    public string ProjectName { get; set; }

    [DataMember]
    public IdentityRef EventTriggeredBy { get; set; }

    [DataMember]
    public string EventTrigger { get; set; }

    [DataMember]
    public string SourceMessage { get; set; }

    [DataMember]
    public List<EventReviewer> Reviewers { get; set; }

    [DataMember]
    public string CreatedByEmail { get; set; }

    [DataMember]
    public bool HasRequiredReviewers { get; set; }

    [DataMember]
    public bool HasOptionalReviewers { get; set; }

    [DataMember]
    public ToolContext ExternalToolContext { get; set; }

    [DataMember]
    public string ReviewEventType { get; set; }

    [DataMember]
    public string SummaryMessage { get; set; }

    [DataMember]
    public string LatestIterationDescription { get; set; }

    public string FromAddress => this.GetFromAddress();

    public virtual void AddActors(VssNotificationEvent notificationEvent)
    {
      if (this.CodeReview.Author != null)
        notificationEvent.AddActor(CodeReviewEvent.Roles.Author, new Guid(this.CodeReview.Author.Id));
      Guid actor = this.GetActor();
      if (actor != Guid.Empty)
        notificationEvent.AddActor(VssNotificationEvent.Roles.Initiator, actor);
      if (this.Reviewers == null)
        return;
      foreach (EventReviewer reviewer in this.Reviewers)
        notificationEvent.AddActor(CodeReviewEvent.Roles.Reviewer, new Guid(reviewer.Identity.Id));
    }

    public virtual void UpdateEventTriggeredBy()
    {
      IdentityRef identityRef = (IdentityRef) null;
      if (this.codeReviewNotification.Requester != null)
        identityRef = this.codeReviewNotification.Requester;
      this.EventTriggeredBy = identityRef;
    }

    protected List<ChangeEntry> GetSortedChangeListByModifiedPath()
    {
      List<ChangeEntry> listByModifiedPath = new List<ChangeEntry>();
      if (this.CodeReview.Iterations != null && this.CodeReview.Iterations.Count > 0)
      {
        Iteration iteration = this.CodeReview.Iterations.Last<Iteration>();
        if (iteration.ChangeList != null)
        {
          List<ChangeEntry> changesToSort = new List<ChangeEntry>();
          foreach (ChangeEntry change in (IEnumerable<ChangeEntry>) iteration.ChangeList)
          {
            if (change.Type == ChangeType.Delete)
            {
              ChangeEntry changeEntry = new ChangeEntry()
              {
                Modified = change.Base,
                Type = change.Type,
                TotalChangesCount = change.TotalChangesCount
              };
              changesToSort.Add(changeEntry);
            }
            else
              changesToSort.Add(change);
          }
          listByModifiedPath = ChangesExtension.GetSortedChangeList(changesToSort);
        }
      }
      return listByModifiedPath;
    }

    private string GetFromAddress()
    {
      if (!string.IsNullOrEmpty(this.CodeReview.Author.DisplayName) && !string.IsNullOrEmpty(this.CreatedByEmail))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\" <{1}>", (object) this.CodeReview.Author.DisplayName, (object) this.CreatedByEmail);
      return !string.IsNullOrEmpty(this.CreatedByEmail) ? this.CreatedByEmail : (string) null;
    }

    public static class Roles
    {
      public static string Author = "author";
      public static string Reviewer = "reviewer";
      public static string ChangedReviewers = "changedreviewers";
      public static string TriggeredBy = "triggeredBy";
      public static string Receiver = "receiver";
    }
  }
}
