// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.DeletedNode
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  public class DeletedNode
  {
    private string _uri;
    private DeletedNode[] _children;

    public DeletedNode()
    {
      this._uri = string.Empty;
      this._children = Array.Empty<DeletedNode>();
    }

    public DeletedNode(string uri)
    {
      this._uri = uri;
      this._children = Array.Empty<DeletedNode>();
    }

    public DeletedNode(string uri, DeletedNode[] children)
    {
      this._uri = uri;
      this._children = children;
    }

    public string Uri
    {
      get => this._uri;
      set => this._uri = value;
    }

    public DeletedNode[] Children
    {
      get => this._children;
      set => this._children = value;
    }
  }
}
