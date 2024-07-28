// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ValueDeserializers.NodeValueDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ValueDeserializers
{
  public sealed class NodeValueDeserializer : IValueDeserializer
  {
    private readonly IList<INodeDeserializer> deserializers;
    private readonly IList<INodeTypeResolver> typeResolvers;

    public NodeValueDeserializer(
      IList<INodeDeserializer> deserializers,
      IList<INodeTypeResolver> typeResolvers)
    {
      this.deserializers = deserializers != null ? deserializers : throw new ArgumentNullException(nameof (deserializers));
      this.typeResolvers = typeResolvers != null ? typeResolvers : throw new ArgumentNullException(nameof (typeResolvers));
    }

    public object DeserializeValue(
      IParser parser,
      Type expectedType,
      SerializerState state,
      IValueDeserializer nestedObjectDeserializer)
    {
      NodeEvent nodeEvent = parser.Peek<NodeEvent>();
      Type typeFromEvent = this.GetTypeFromEvent(nodeEvent, expectedType);
      try
      {
        foreach (INodeDeserializer deserializer in (IEnumerable<INodeDeserializer>) this.deserializers)
        {
          object obj;
          if (deserializer.Deserialize(parser, typeFromEvent, (Func<IParser, Type, object>) ((r, t) => nestedObjectDeserializer.DeserializeValue(r, t, state, nestedObjectDeserializer)), out obj))
            return TypeConverter.ChangeType(obj, expectedType);
        }
      }
      catch (YamlException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new YamlException(nodeEvent.Start, nodeEvent.End, "Exception during deserialization", ex);
      }
      throw new YamlException(nodeEvent.Start, nodeEvent.End, string.Format("No node deserializer was able to deserialize the node into type {0}", (object) expectedType.AssemblyQualifiedName));
    }

    private Type GetTypeFromEvent(NodeEvent nodeEvent, Type currentType)
    {
      foreach (INodeTypeResolver typeResolver in (IEnumerable<INodeTypeResolver>) this.typeResolvers)
      {
        if (typeResolver.Resolve(nodeEvent, ref currentType))
          break;
      }
      return currentType;
    }
  }
}
