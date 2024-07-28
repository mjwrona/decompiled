// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ProjectRenamedEvent
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  [Obsolete("The ProjectRenamedEvent class is obsolete.")]
  public class ProjectRenamedEvent
  {
    private string _projectUri;
    private string _newName;
    private string _oldName;

    public ProjectRenamedEvent()
    {
      this._projectUri = string.Empty;
      this._newName = string.Empty;
      this._oldName = string.Empty;
    }

    public ProjectRenamedEvent(string projectUri, string newName, string oldName)
    {
      this._projectUri = projectUri == null ? string.Empty : projectUri.Trim();
      this._newName = newName == null ? string.Empty : newName.Trim();
      this._oldName = oldName == null ? string.Empty : oldName.Trim();
    }

    public string Uri
    {
      get => this._projectUri;
      set => this._projectUri = value;
    }

    public string NewName
    {
      get => this._newName;
      set => this._newName = value;
    }

    public string OldName
    {
      get => this._oldName;
      set => this._oldName = value;
    }
  }
}
