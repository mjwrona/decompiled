// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectRenamedEvent
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.Types
{
  [DataContract]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-tfs.project-renamed-event")]
  public class ProjectRenamedEvent
  {
    public ProjectRenamedEvent()
    {
      this.PreviousName = string.Empty;
      this.CurrentName = string.Empty;
    }

    public ProjectRenamedEvent(Guid projectId, string previousName, string currentName)
      : this()
    {
      this.ProjectId = projectId;
      this.PreviousName = previousName;
      this.CurrentName = currentName;
    }

    public ProjectRenamedEvent(ProjectRenamedEvent renamedEvent)
      : this(renamedEvent.ProjectId, renamedEvent.PreviousName, renamedEvent.CurrentName)
    {
    }

    public override string ToString() => string.Format("[ProjectId={0}, PreviousName={1}, CurrentName={2}]", (object) this.ProjectId, (object) this.PreviousName, (object) this.CurrentName);

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public string PreviousName { get; set; }

    [DataMember]
    public string CurrentName { get; set; }

    [DataMember]
    public string ProjectUrl { get; set; }

    [DataMember]
    public string CollectionUrl { get; set; }

    [DataMember]
    public string CollectionName { get; set; }

    [DataMember]
    public bool NotifyAllUsers { get; set; }
  }
}
