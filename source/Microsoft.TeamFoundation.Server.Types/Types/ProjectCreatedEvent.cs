// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectCreatedEvent
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.VisualStudio.Services.Notifications;

namespace Microsoft.TeamFoundation.Server.Types
{
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-tfs.project-created-event")]
  public class ProjectCreatedEvent
  {
    private string _projectUri;
    private string _name;

    public ProjectCreatedEvent()
    {
      this._projectUri = string.Empty;
      this._name = string.Empty;
    }

    public ProjectCreatedEvent(string projectUri, string name)
    {
      this._projectUri = projectUri == null ? string.Empty : projectUri.Trim();
      this._name = name == null ? string.Empty : name.Trim();
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
  }
}
