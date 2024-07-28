// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectUpdatedEvent
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Types
{
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-tfs.project-updated-event")]
  public class ProjectUpdatedEvent
  {
    private string _projectUri;
    private string _name;
    private ProjectState _state;
    private Guid _userId;
    private DateTime _timeStamp;
    private long _revision;
    private bool _shouldInvalidateSystemStore;
    private IList<ProjectProperty> _updatedProperties;
    private ProjectVisibility _visibility;

    public ProjectUpdatedEvent()
    {
      this._projectUri = string.Empty;
      this._name = string.Empty;
      this._state = ProjectState.Unchanged;
      this._userId = Guid.Empty;
      this._timeStamp = DateTime.MinValue;
      this._revision = 0L;
      this._shouldInvalidateSystemStore = false;
      this._updatedProperties = (IList<ProjectProperty>) new List<ProjectProperty>();
      this._visibility = ProjectVisibility.Unchanged;
    }

    public ProjectUpdatedEvent(
      string projectUri,
      string name,
      ProjectState state,
      Guid userId,
      DateTime timeStamp,
      long revision,
      bool shouldInvalidateSystemStore = false,
      IList<ProjectProperty> updatedProperties = null,
      ProjectVisibility visibility = ProjectVisibility.Unchanged)
    {
      this._projectUri = projectUri == null ? string.Empty : projectUri.Trim();
      this._name = name == null ? string.Empty : name.Trim();
      this._state = state;
      this._userId = userId;
      this._timeStamp = timeStamp;
      this._revision = revision;
      this._shouldInvalidateSystemStore = shouldInvalidateSystemStore;
      this._updatedProperties = updatedProperties;
      this._visibility = visibility;
    }

    public string Uri
    {
      get => this._projectUri;
      set => this._projectUri = value;
    }

    public string Name
    {
      get => this._name;
      set => this._name = value;
    }

    public ProjectState State
    {
      get => this._state;
      set => this._state = value;
    }

    public ProjectVisibility Visibility
    {
      get => this._visibility;
      set => this._visibility = value;
    }

    public Guid UserId
    {
      get => this._userId;
      set => this._userId = value;
    }

    public DateTime TimeStamp
    {
      get => this._timeStamp;
      set => this._timeStamp = value;
    }

    public long Revision
    {
      get => this._revision;
      set => this._revision = value;
    }

    public bool ShouldInvalidateSystemStore
    {
      get => this._shouldInvalidateSystemStore;
      set => this._shouldInvalidateSystemStore = value;
    }

    public IList<ProjectProperty> UpdatedProperties
    {
      get => this._updatedProperties;
      set => this._updatedProperties = value;
    }

    public Dictionary<string, object> ProjectOperationProperties { get; set; }

    public ProjectInfo ToProjectInfo() => new ProjectInfo(ProjectInfo.GetProjectId(this.Uri), this.Name, this.State, this.Visibility)
    {
      Revision = this.Revision,
      Properties = this.UpdatedProperties
    };

    public override string ToString() => string.Format("[Uri={0}, Name={1}, State={2}, UserId={3}, TimeStamp={4}, Revision={5}, Visibility={6}]", (object) this.Uri, (object) this.Name, (object) this.State, (object) this.UserId, (object) this.TimeStamp, (object) this.Revision, (object) this.Visibility);

    public T GetOperationProperty<T>(string propertyName, T defaultValue)
    {
      object obj1;
      return this.ProjectOperationProperties != null && this.ProjectOperationProperties.TryGetValue(propertyName, out obj1) && obj1 != null && obj1 is T obj2 ? obj2 : defaultValue;
    }
  }
}
