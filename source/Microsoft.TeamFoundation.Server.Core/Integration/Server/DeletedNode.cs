// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.DeletedNode
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Integration.Server
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
