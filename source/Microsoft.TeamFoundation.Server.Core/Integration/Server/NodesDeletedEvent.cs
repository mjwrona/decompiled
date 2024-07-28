// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.NodesDeletedEvent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Integration.Server
{
  public class NodesDeletedEvent
  {
    private string _projectUri;
    private string _reclassifyUri;
    private string _deletedUser;
    private string _deletedTime;
    private DeletedNode[] _nodesDeleted;

    public NodesDeletedEvent()
    {
      this._projectUri = string.Empty;
      this._deletedUser = string.Empty;
      this._deletedTime = string.Empty;
    }

    public NodesDeletedEvent(
      string projectUri,
      string reclassifyUri,
      string deletedUser,
      string deletedTime,
      DeletedNode[] nodesDeleted)
    {
      this._projectUri = projectUri == null ? string.Empty : projectUri.Trim();
      this._reclassifyUri = reclassifyUri == null ? string.Empty : reclassifyUri.Trim();
      this._deletedUser = deletedUser == null ? string.Empty : deletedUser.Trim();
      this._deletedTime = deletedTime == null ? string.Empty : deletedTime.Trim();
      this._nodesDeleted = nodesDeleted;
    }

    public string ProjectUri
    {
      get => this._projectUri;
      set => this._projectUri = value;
    }

    public string DeletedUser
    {
      get => this._deletedUser;
      set => this._deletedUser = value;
    }

    public string DeletedTime
    {
      get => this._deletedTime;
      set => this._deletedTime = value;
    }

    public DeletedNode[] NodesDeleted
    {
      get => this._nodesDeleted;
      set => this._nodesDeleted = value;
    }
  }
}
