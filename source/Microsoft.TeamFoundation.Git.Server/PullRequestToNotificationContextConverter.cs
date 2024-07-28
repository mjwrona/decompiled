// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestToNotificationContextConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestToNotificationContextConverter
  {
    public static NotificationContext ToNotificationContext(
      this ShareNotificationContext prNotificationContext)
    {
      if (prNotificationContext == null)
        return (NotificationContext) null;
      return new NotificationContext()
      {
        Message = prNotificationContext.Message,
        Receivers = prNotificationContext.Receivers.ToList<IdentityRef>()
      };
    }
  }
}
