// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Comments.WorkItemCommentSourceContext
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Comments
{
  internal class WorkItemCommentSourceContext : IMentionSourceContext
  {
    public WorkItemCommentSourceContext(
      int workItemId,
      Guid projectGuid,
      string displayText,
      MentionActionType actions,
      Guid mentioner)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(displayText, nameof (displayText));
      ArgumentUtility.CheckForEmptyGuid(projectGuid, nameof (projectGuid));
      ArgumentUtility.CheckForEmptyGuid(mentioner, nameof (mentioner));
      this.Identifier = workItemId.ToString();
      this.ProjectGuid = projectGuid;
      this.DisplayText = displayText;
      this.SupportedActionTypes = actions;
      this.Mentioner = mentioner;
      this.NormalizedId = workItemId.ToString();
    }

    public string Type => MentionConstants.WorkItemType;

    public string Identifier { get; }

    public Guid ProjectGuid { get; }

    public string DisplayText { get; }

    public MentionActionType SupportedActionTypes { get; }

    public Guid Mentioner { get; }

    public string NormalizedId { get; }
  }
}
