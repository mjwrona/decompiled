// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events.ICodeReviewEvent
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Subscription.Events
{
  public interface ICodeReviewEvent
  {
    Guid ProjectId { get; }

    string ProjectName { get; }

    Review CodeReview { get; set; }

    IdentityRef EventTriggeredBy { get; set; }

    string EventTrigger { get; set; }

    string CreatedByEmail { get; set; }

    bool HasRequiredReviewers { get; set; }

    bool HasOptionalReviewers { get; set; }

    string SourceMessage { get; set; }

    List<EventReviewer> Reviewers { get; set; }

    ToolContext ExternalToolContext { get; set; }
  }
}
