// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ProjectCreatedEvent
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Notifications;

namespace Microsoft.TeamFoundation.Server
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
