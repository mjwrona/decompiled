// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.CssDataProviderNode
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;

namespace Microsoft.TeamFoundation.Server
{
  internal sealed class CssDataProviderNode : CssNode
  {
    private NodeInfo _nodeInfo;
    private CssNodeCollection _children;

    internal CssDataProviderNode(NodeInfo nodeInfo)
      : base(nodeInfo.Name)
    {
      this._nodeInfo = nodeInfo;
      this._children = new CssNodeCollection();
    }

    internal CssNodeCollection Children
    {
      set
      {
        if (value.Count > 0)
          this.HasChildren = true;
        this._children = value;
      }
      get => this._children;
    }

    internal NodeInfo NodeInfo => this._nodeInfo;
  }
}
