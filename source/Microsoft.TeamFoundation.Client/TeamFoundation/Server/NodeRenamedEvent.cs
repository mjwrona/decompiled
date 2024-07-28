// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.NodeRenamedEvent
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation.Server
{
  public class NodeRenamedEvent
  {
    private string _projectUri;
    private string _nodeUri;
    private string _newName;
    private string _oldName;

    public NodeRenamedEvent()
    {
      this._projectUri = string.Empty;
      this._nodeUri = string.Empty;
      this._newName = string.Empty;
      this._oldName = string.Empty;
    }

    public NodeRenamedEvent(string projectUri, string nodeUri, string newName, string oldName)
    {
      this._projectUri = projectUri == null ? string.Empty : projectUri.Trim();
      this._nodeUri = nodeUri == null ? string.Empty : nodeUri.Trim();
      this._newName = newName == null ? string.Empty : newName.Trim();
      this._oldName = oldName == null ? string.Empty : oldName.Trim();
    }

    public string ProjectUri
    {
      get => this._projectUri;
      set => this._projectUri = value;
    }

    public string NodeUri
    {
      get => this._nodeUri;
      set => this._nodeUri = value;
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
