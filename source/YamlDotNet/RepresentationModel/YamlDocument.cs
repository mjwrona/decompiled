// Decompiled with JetBrains decompiler
// Type: YamlDotNet.RepresentationModel.YamlDocument
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
  [Serializable]
  public class YamlDocument
  {
    public YamlNode RootNode { get; private set; }

    public YamlDocument(YamlNode rootNode) => this.RootNode = rootNode;

    public YamlDocument(string rootNode) => this.RootNode = (YamlNode) new YamlScalarNode(rootNode);

    internal YamlDocument(IParser parser)
    {
      DocumentLoadingState state = new DocumentLoadingState();
      parser.Expect<DocumentStart>();
      while (!parser.Accept<DocumentEnd>())
      {
        this.RootNode = YamlNode.ParseNode(parser, state);
        if (this.RootNode is YamlAliasNode)
          throw new YamlException();
      }
      state.ResolveAliases();
      parser.Expect<DocumentEnd>();
    }

    private void AssignAnchors() => new YamlDocument.AnchorAssigningVisitor().AssignAnchors(this);

    internal void Save(IEmitter emitter, bool assignAnchors = true)
    {
      if (assignAnchors)
        this.AssignAnchors();
      emitter.Emit((ParsingEvent) new DocumentStart());
      this.RootNode.Save(emitter, new EmitterState());
      emitter.Emit((ParsingEvent) new DocumentEnd(false));
    }

    public void Accept(IYamlVisitor visitor) => visitor.Visit(this);

    public IEnumerable<YamlNode> AllNodes => this.RootNode.AllNodes;

    private class AnchorAssigningVisitor : YamlVisitorBase
    {
      private readonly HashSet<string> existingAnchors = new HashSet<string>();
      private readonly Dictionary<YamlNode, bool> visitedNodes = new Dictionary<YamlNode, bool>();

      public void AssignAnchors(YamlDocument document)
      {
        this.existingAnchors.Clear();
        this.visitedNodes.Clear();
        document.Accept((IYamlVisitor) this);
        Random random = new Random();
        foreach (KeyValuePair<YamlNode, bool> visitedNode in this.visitedNodes)
        {
          if (visitedNode.Value)
          {
            string anchor;
            if (!string.IsNullOrEmpty(visitedNode.Key.Anchor) && !this.existingAnchors.Contains(visitedNode.Key.Anchor))
            {
              anchor = visitedNode.Key.Anchor;
            }
            else
            {
              do
              {
                anchor = random.Next().ToString((IFormatProvider) CultureInfo.InvariantCulture);
              }
              while (this.existingAnchors.Contains(anchor));
            }
            this.existingAnchors.Add(anchor);
            visitedNode.Key.Anchor = anchor;
          }
        }
      }

      private bool VisitNodeAndFindDuplicates(YamlNode node)
      {
        bool flag;
        if (this.visitedNodes.TryGetValue(node, out flag))
        {
          if (!flag)
            this.visitedNodes[node] = true;
          return !flag;
        }
        this.visitedNodes.Add(node, false);
        return false;
      }

      public override void Visit(YamlScalarNode scalar) => this.VisitNodeAndFindDuplicates((YamlNode) scalar);

      public override void Visit(YamlMappingNode mapping)
      {
        if (this.VisitNodeAndFindDuplicates((YamlNode) mapping))
          return;
        base.Visit(mapping);
      }

      public override void Visit(YamlSequenceNode sequence)
      {
        if (this.VisitNodeAndFindDuplicates((YamlNode) sequence))
          return;
        base.Visit(sequence);
      }
    }
  }
}
