// Decompiled with JetBrains decompiler
// Type: YamlDotNet.RepresentationModel.DocumentLoadingState
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.RepresentationModel
{
  internal class DocumentLoadingState
  {
    private readonly IDictionary<string, YamlNode> anchors = (IDictionary<string, YamlNode>) new Dictionary<string, YamlNode>();
    private readonly IList<YamlNode> nodesWithUnresolvedAliases = (IList<YamlNode>) new List<YamlNode>();

    public void AddAnchor(YamlNode node)
    {
      if (node.Anchor == null)
        throw new ArgumentException("The specified node does not have an anchor");
      if (this.anchors.ContainsKey(node.Anchor))
        this.anchors[node.Anchor] = node;
      else
        this.anchors.Add(node.Anchor, node);
    }

    public YamlNode GetNode(string anchor, bool throwException, Mark start, Mark end)
    {
      YamlNode node;
      if (this.anchors.TryGetValue(anchor, out node))
        return node;
      if (throwException)
        throw new AnchorNotFoundException(start, end, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The anchor '{0}' does not exists", new object[1]
        {
          (object) anchor
        }));
      return (YamlNode) null;
    }

    public void AddNodeWithUnresolvedAliases(YamlNode node) => this.nodesWithUnresolvedAliases.Add(node);

    public void ResolveAliases()
    {
      foreach (YamlNode withUnresolvedAlias in (IEnumerable<YamlNode>) this.nodesWithUnresolvedAliases)
        withUnresolvedAlias.ResolveAliases(this);
    }
  }
}
