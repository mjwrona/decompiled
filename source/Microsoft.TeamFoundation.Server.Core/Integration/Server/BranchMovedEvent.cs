// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.BranchMovedEvent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Integration.Server
{
  public class BranchMovedEvent
  {
    private string _projectUri;
    private string _nodeUri;
    private string _oldParentUri;
    private string _newParentUri;

    public BranchMovedEvent()
    {
      this._projectUri = string.Empty;
      this._nodeUri = string.Empty;
      this._oldParentUri = string.Empty;
      this._newParentUri = string.Empty;
    }

    public BranchMovedEvent(
      string projectUri,
      string nodeUri,
      string oldParentUri,
      string newParentUri)
    {
      this._projectUri = projectUri == null ? string.Empty : projectUri.Trim();
      this._nodeUri = nodeUri == null ? string.Empty : nodeUri.Trim();
      this._oldParentUri = oldParentUri == null ? string.Empty : oldParentUri.Trim();
      this._newParentUri = newParentUri == null ? string.Empty : newParentUri.Trim();
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

    public string OldParentUri
    {
      get => this._oldParentUri;
      set => this._oldParentUri = value;
    }

    public string NewParentUri
    {
      get => this._newParentUri;
      set => this._newParentUri = value;
    }
  }
}
