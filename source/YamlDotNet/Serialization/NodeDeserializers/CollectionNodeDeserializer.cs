// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.NodeDeserializers.CollectionNodeDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Helpers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
  public sealed class CollectionNodeDeserializer : INodeDeserializer
  {
    private readonly IObjectFactory _objectFactory;

    public CollectionNodeDeserializer(IObjectFactory objectFactory) => this._objectFactory = objectFactory;

    bool INodeDeserializer.Deserialize(
      IParser parser,
      Type expectedType,
      Func<IParser, Type, object> nestedObjectDeserializer,
      out object value)
    {
      bool canUpdate = true;
      Type genericInterface1 = ReflectionUtility.GetImplementedGenericInterface(expectedType, typeof (ICollection<>));
      Type tItem;
      if ((object) genericInterface1 != null)
      {
        tItem = genericInterface1.GetGenericArguments()[0];
        value = this._objectFactory.Create(expectedType);
        if (!(value is IList result))
        {
          Type genericInterface2 = ReflectionUtility.GetImplementedGenericInterface(expectedType, typeof (IList<>));
          canUpdate = genericInterface2 != null;
          result = (IList) new GenericCollectionToNonGenericAdapter(value, genericInterface1, genericInterface2);
        }
      }
      else if (typeof (IList).IsAssignableFrom(expectedType))
      {
        tItem = typeof (object);
        value = this._objectFactory.Create(expectedType);
        result = (IList) value;
      }
      else
      {
        value = (object) null;
        return false;
      }
      CollectionNodeDeserializer.DeserializeHelper(tItem, parser, nestedObjectDeserializer, result, canUpdate);
      return true;
    }

    internal static void DeserializeHelper(
      Type tItem,
      IParser parser,
      Func<IParser, Type, object> nestedObjectDeserializer,
      IList result,
      bool canUpdate)
    {
      parser.Expect<SequenceStart>();
      while (!parser.Accept<SequenceEnd>())
      {
        ParsingEvent current = parser.Current;
        object obj = nestedObjectDeserializer(parser, tItem);
        if (!(obj is IValuePromise valuePromise))
        {
          result.Add(TypeConverter.ChangeType(obj, tItem));
        }
        else
        {
          if (!canUpdate)
            throw new ForwardAnchorNotSupportedException(current.Start, current.End, "Forward alias references are not allowed because this type does not implement IList<>");
          int index = result.Add(tItem.IsValueType() ? Activator.CreateInstance(tItem) : (object) null);
          valuePromise.ValueAvailable += (Action<object>) (v => result[index] = TypeConverter.ChangeType(v, tItem));
        }
      }
      parser.Expect<SequenceEnd>();
    }
  }
}
