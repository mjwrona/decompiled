// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectDeletedEvent
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Notifications;
using System;

namespace Microsoft.TeamFoundation.Server.Types
{
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-tfs.project-deleted-event")]
  public class ProjectDeletedEvent
  {
    private string _projectUri;
    private string _projectName;
    private string _deletedTime;
    private long _revision;
    private Guid _userId;

    public ProjectDeletedEvent()
    {
      this._projectUri = string.Empty;
      this._projectName = string.Empty;
      this._deletedTime = string.Empty;
      this._userId = Guid.Empty;
    }

    public ProjectDeletedEvent(string projectUri)
      : this()
    {
      this._projectUri = projectUri == null ? string.Empty : projectUri.Trim();
    }

    public string Uri
    {
      get => this._projectUri;
      set => this._projectUri = value;
    }

    public string Name
    {
      get => this._projectName;
      set => this._projectName = value;
    }

    public Guid UserId
    {
      get => this._userId;
      set => this._userId = value;
    }

    public string DeletedTime
    {
      get => this._deletedTime;
      set => this._deletedTime = value;
    }

    public long Revision
    {
      get => this._revision;
      set => this._revision = value;
    }

    public ProjectInfo ToProjectInfo() => new ProjectInfo(ProjectInfo.GetProjectId(this.Uri), this.Name, ProjectState.Deleted)
    {
      Revision = this.Revision
    };

    public static class Roles
    {
      public static string DeletedUser = "deletedUser";
    }
  }
}
