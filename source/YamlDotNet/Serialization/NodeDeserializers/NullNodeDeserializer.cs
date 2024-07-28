// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.NodeDeserializers.NullNodeDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeDeserializers
{
  public sealed class NullNodeDeserializer : INodeDeserializer
  {
    bool INodeDeserializer.Deserialize(
      IParser parser,
      Type expectedType,
      Func<IParser, Type, object> nestedObjectDeserializer,
      out object value)
    {
      value = (object) null;
      NodeEvent nodeEvent = parser.Peek<NodeEvent>();
      int num = nodeEvent == null ? 0 : (this.NodeIsNull(nodeEvent) ? 1 : 0);
      if (num == 0)
        return num != 0;
      parser.SkipThisAndNestedEvents();
      return num != 0;
    }

    private bool NodeIsNull(NodeEvent nodeEvent)
    {
      if (nodeEvent.Tag == "tag:yaml.org,2002:null")
        return true;
      if (!(nodeEvent is Scalar scalar) || scalar.Style != ScalarStyle.Plain)
        return false;
      string str = scalar.Value;
      return str == "" || str == "~" || str == "null" || str == "Null" || str == "NULL";
    }
  }
}
