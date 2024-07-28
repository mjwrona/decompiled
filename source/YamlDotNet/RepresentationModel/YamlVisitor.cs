// Decompiled with JetBrains decompiler
// Type: YamlDotNet.RepresentationModel.YamlVisitor
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;

namespace YamlDotNet.RepresentationModel
{
  [Obsolete("Use YamlVisitorBase")]
  public abstract class YamlVisitor : IYamlVisitor
  {
    protected virtual void Visit(YamlStream stream)
    {
    }

    protected virtual void Visited(YamlStream stream)
    {
    }

    protected virtual void Visit(YamlDocument document)
    {
    }

    protected virtual void Visited(YamlDocument document)
    {
    }

    protected virtual void Visit(YamlScalarNode scalar)
    {
    }

    protected virtual void Visited(YamlScalarNode scalar)
    {
    }

    protected virtual void Visit(YamlSequenceNode sequence)
    {
    }

    protected virtual void Visited(YamlSequenceNode sequence)
    {
    }

    protected virtual void Visit(YamlMappingNode mapping)
    {
    }

    protected virtual void Visited(YamlMappingNode mapping)
    {
    }

    protected virtual void VisitChildren(YamlStream stream)
    {
      foreach (YamlDocument document in (IEnumerable<YamlDocument>) stream.Documents)
        document.Accept((IYamlVisitor) this);
    }

    protected virtual void VisitChildren(YamlDocument document)
    {
      if (document.RootNode == null)
        return;
      document.RootNode.Accept((IYamlVisitor) this);
    }

    protected virtual void VisitChildren(YamlSequenceNode sequence)
    {
      foreach (YamlNode child in (IEnumerable<YamlNode>) sequence.Children)
        child.Accept((IYamlVisitor) this);
    }

    protected virtual void VisitChildren(YamlMappingNode mapping)
    {
      foreach (KeyValuePair<YamlNode, YamlNode> child in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>) mapping.Children)
      {
        child.Key.Accept((IYamlVisitor) this);
        child.Value.Accept((IYamlVisitor) this);
      }
    }

    void IYamlVisitor.Visit(YamlStream stream)
    {
      this.Visit(stream);
      this.VisitChildren(stream);
      this.Visited(stream);
    }

    void IYamlVisitor.Visit(YamlDocument document)
    {
      this.Visit(document);
      this.VisitChildren(document);
      this.Visited(document);
    }

    void IYamlVisitor.Visit(YamlScalarNode scalar)
    {
      this.Visit(scalar);
      this.Visited(scalar);
    }

    void IYamlVisitor.Visit(YamlSequenceNode sequence)
    {
      this.Visit(sequence);
      this.VisitChildren(sequence);
      this.Visited(sequence);
    }

    void IYamlVisitor.Visit(YamlMappingNode mapping)
    {
      this.Visit(mapping);
      this.VisitChildren(mapping);
      this.Visited(mapping);
    }
  }
}
