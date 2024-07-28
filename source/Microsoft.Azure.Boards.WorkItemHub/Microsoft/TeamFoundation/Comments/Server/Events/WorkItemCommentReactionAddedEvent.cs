// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.Events.WorkItemCommentReactionAddedEvent
// Assembly: Microsoft.Azure.Boards.WorkItemHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 749A696A-54F8-4B6F-8877-B350F1725C24
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.WorkItemHub.dll

using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Comments.Server.Events
{
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-work.workitem-reaction-added-event")]
  [DataContract]
  public class WorkItemCommentReactionAddedEvent
  {
    [DataMember]
    public IdentityRef ReactionAddedBy { get; set; }

    [DataMember]
    public string ReactionTypeIcon { get; set; }

    [DataMember]
    public string ArtifactKindFriendlyName { get; set; }

    [DataMember]
    public string ArtifactType { get; set; }

    [DataMember]
    public ArtifactInfo ArtifactInfo { get; set; }
  }
}
