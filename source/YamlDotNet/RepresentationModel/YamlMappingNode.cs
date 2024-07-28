// Decompiled with JetBrains decompiler
// Type: YamlDotNet.RepresentationModel.YamlMappingNode
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.RepresentationModel
{
  [Serializable]
  public sealed class YamlMappingNode : 
    YamlNode,
    IEnumerable<KeyValuePair<YamlNode, YamlNode>>,
    IEnumerable,
    IYamlConvertible
  {
    private readonly IDictionary<YamlNode, YamlNode> children = (IDictionary<YamlNode, YamlNode>) new Dictionary<YamlNode, YamlNode>();

    public IDictionary<YamlNode, YamlNode> Children => this.children;

    public MappingStyle Style { get; set; }

    internal YamlMappingNode(IParser parser, DocumentLoadingState state) => this.Load(parser, state);

    private void Load(IParser parser, DocumentLoadingState state)
    {
      MappingStart yamlEvent = parser.Expect<MappingStart>();
      this.Load((NodeEvent) yamlEvent, state);
      this.Style = yamlEvent.Style;
      bool flag = false;
      while (!parser.Accept<MappingEnd>())
      {
        YamlNode node1 = YamlNode.ParseNode(parser, state);
        YamlNode node2 = YamlNode.ParseNode(parser, state);
        try
        {
          this.children.Add(node1, node2);
        }
        catch (ArgumentException ex)
        {
          throw new YamlException(node1.Start, node1.End, "Duplicate key", (Exception) ex);
        }
        flag = ((flag ? 1 : 0) | (node1 is YamlAliasNode ? 1 : (node2 is YamlAliasNode ? 1 : 0))) != 0;
      }
      if (flag)
        state.AddNodeWithUnresolvedAliases((YamlNode) this);
      parser.Expect<MappingEnd>();
    }

    public YamlMappingNode()
    {
    }

    public YamlMappingNode(params KeyValuePair<YamlNode, YamlNode>[] children)
      : this((IEnumerable<KeyValuePair<YamlNode, YamlNode>>) children)
    {
    }

    public YamlMappingNode(
      IEnumerable<KeyValuePair<YamlNode, YamlNode>> children)
    {
      foreach (KeyValuePair<YamlNode, YamlNode> child in children)
        this.children.Add(child);
    }

    public YamlMappingNode(params YamlNode[] children)
      : this((IEnumerable<YamlNode>) children)
    {
    }

    public YamlMappingNode(IEnumerable<YamlNode> children)
    {
      using (IEnumerator<YamlNode> enumerator = children.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          YamlNode current = enumerator.Current;
          if (!enumerator.MoveNext())
            throw new ArgumentException("When constructing a mapping node with a sequence, the number of elements of the sequence must be even.");
          this.Add(current, enumerator.Current);
        }
      }
    }

    public void Add(YamlNode key, YamlNode value) => this.children.Add(key, value);

    public void Add(string key, YamlNode value) => this.children.Add((YamlNode) new YamlScalarNode(key), value);

    public void Add(YamlNode key, string value) => this.children.Add(key, (YamlNode) new YamlScalarNode(value));

    public void Add(string key, string value) => this.children.Add((YamlNode) new YamlScalarNode(key), (YamlNode) new YamlScalarNode(value));

    internal override void ResolveAliases(DocumentLoadingState state)
    {
      Dictionary<YamlNode, YamlNode> dictionary1 = (Dictionary<YamlNode, YamlNode>) null;
      Dictionary<YamlNode, YamlNode> dictionary2 = (Dictionary<YamlNode, YamlNode>) null;
      foreach (KeyValuePair<YamlNode, YamlNode> child in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>) this.children)
      {
        if (child.Key is YamlAliasNode)
        {
          if (dictionary1 == null)
            dictionary1 = new Dictionary<YamlNode, YamlNode>();
          dictionary1.Add(child.Key, state.GetNode(child.Key.Anchor, true, child.Key.Start, child.Key.End));
        }
        if (child.Value is YamlAliasNode)
        {
          if (dictionary2 == null)
            dictionary2 = new Dictionary<YamlNode, YamlNode>();
          dictionary2.Add(child.Key, state.GetNode(child.Value.Anchor, true, child.Value.Start, child.Value.End));
        }
      }
      if (dictionary2 != null)
      {
        foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in dictionary2)
          this.children[keyValuePair.Key] = keyValuePair.Value;
      }
      if (dictionary1 == null)
        return;
      foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in dictionary1)
      {
        YamlNode child = this.children[keyValuePair.Key];
        this.children.Remove(keyValuePair.Key);
        this.children.Add(keyValuePair.Value, child);
      }
    }

    internal override void Emit(IEmitter emitter, EmitterState state)
    {
      emitter.Emit((ParsingEvent) new MappingStart(this.Anchor, this.Tag, true, this.Style));
      foreach (KeyValuePair<YamlNode, YamlNode> child in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>) this.children)
      {
        child.Key.Save(emitter, state);
        child.Value.Save(emitter, state);
      }
      emitter.Emit((ParsingEvent) new MappingEnd());
    }

    public override void Accept(IYamlVisitor visitor) => visitor.Visit(this);

    public override bool Equals(object obj)
    {
      if (!(obj is YamlMappingNode other) || !this.Equals((YamlNode) other) || this.children.Count != other.children.Count)
        return false;
      foreach (KeyValuePair<YamlNode, YamlNode> child in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>) this.children)
      {
        YamlNode second;
        if (!other.children.TryGetValue(child.Key, out second) || !YamlNode.SafeEquals((object) child.Value, (object) second))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int h1 = base.GetHashCode();
      foreach (KeyValuePair<YamlNode, YamlNode> child in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>) this.children)
      {
        h1 = YamlNode.CombineHashCodes(h1, YamlNode.GetHashCode((object) child.Key));
        h1 = YamlNode.CombineHashCodes(h1, YamlNode.GetHashCode((object) child.Value));
      }
      return h1;
    }

    internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
    {
      YamlMappingNode yamlMappingNode = this;
      level.Increment();
      yield return (YamlNode) yamlMappingNode;
      foreach (KeyValuePair<YamlNode, YamlNode> child1 in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>) yamlMappingNode.children)
      {
        KeyValuePair<YamlNode, YamlNode> child = child1;
        foreach (YamlNode safeAllNode in child.Key.SafeAllNodes(level))
          yield return safeAllNode;
        foreach (YamlNode safeAllNode in child.Value.SafeAllNodes(level))
          yield return safeAllNode;
        child = new KeyValuePair<YamlNode, YamlNode>();
      }
      level.Decrement();
    }

    public override YamlNodeType NodeType => YamlNodeType.Mapping;

    internal override string ToString(RecursionLevel level)
    {
      if (!level.TryIncrement())
        return "WARNING! INFINITE RECURSION!";
      StringBuilder stringBuilder = new StringBuilder("{ ");
      foreach (KeyValuePair<YamlNode, YamlNode> child in (IEnumerable<KeyValuePair<YamlNode, YamlNode>>) this.children)
      {
        if (stringBuilder.Length > 2)
          stringBuilder.Append(", ");
        stringBuilder.Append("{ ").Append(child.Key.ToString(level)).Append(", ").Append(child.Value.ToString(level)).Append(" }");
      }
      stringBuilder.Append(" }");
      level.Decrement();
      return stringBuilder.ToString();
    }

    public IEnumerator<KeyValuePair<YamlNode, YamlNode>> GetEnumerator() => this.children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    void IYamlConvertible.Read(
      IParser parser,
      Type expectedType,
      ObjectDeserializer nestedObjectDeserializer)
    {
      this.Load(parser, new DocumentLoadingState());
    }

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) => this.Emit(emitter, new EmitterState());

    public static YamlMappingNode FromObject(object mapping)
    {
      if (mapping == null)
        throw new ArgumentNullException(nameof (mapping));
      YamlMappingNode yamlMappingNode = new YamlMappingNode();
      foreach (PropertyInfo publicProperty in mapping.GetType().GetPublicProperties())
      {
        if (publicProperty.CanRead && publicProperty.GetGetMethod().GetParameters().Length == 0)
        {
          object obj = publicProperty.GetValue(mapping, (object[]) null);
          if (!(obj is YamlNode yamlNode1))
            yamlNode1 = (YamlNode) Convert.ToString(obj);
          YamlNode yamlNode2 = yamlNode1;
          yamlMappingNode.Add(publicProperty.Name, yamlNode2);
        }
      }
      return yamlMappingNode;
    }
  }
}
