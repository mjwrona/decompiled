// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.NodeCreatedEvent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Integration.Server
{
  public class NodeCreatedEvent
  {
    private string _projectUri;
    private string _nodeUri;
    private string _name;

    public NodeCreatedEvent()
    {
      this._projectUri = string.Empty;
      this._nodeUri = string.Empty;
      this._name = string.Empty;
    }

    public NodeCreatedEvent(string projectUri, string nodeUri, string name)
    {
      this._projectUri = projectUri == null ? string.Empty : projectUri.Trim();
      this._nodeUri = nodeUri == null ? string.Empty : nodeUri.Trim();
      this._name = name == null ? string.Empty : name.Trim();
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

    public string Name
    {
      get => this._name;
      set => this._name = value;
    }
  }
}
