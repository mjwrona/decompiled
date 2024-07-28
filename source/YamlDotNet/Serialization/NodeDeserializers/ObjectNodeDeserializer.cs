// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.NodeDeserializers.ObjectNodeDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
  public sealed class ObjectNodeDeserializer : INodeDeserializer
  {
    private readonly IObjectFactory _objectFactory;
    private readonly ITypeInspector _typeDescriptor;
    private readonly bool _ignoreUnmatched;

    public ObjectNodeDeserializer(
      IObjectFactory objectFactory,
      ITypeInspector typeDescriptor,
      bool ignoreUnmatched)
    {
      this._objectFactory = objectFactory;
      this._typeDescriptor = typeDescriptor;
      this._ignoreUnmatched = ignoreUnmatched;
    }

    bool INodeDeserializer.Deserialize(
      IParser parser,
      Type expectedType,
      Func<IParser, Type, object> nestedObjectDeserializer,
      out object value)
    {
      if (parser.Allow<MappingStart>() == null)
      {
        value = (object) null;
        return false;
      }
      value = this._objectFactory.Create(expectedType);
      while (!parser.Accept<MappingEnd>())
      {
        Scalar scalar = parser.Expect<Scalar>();
        IPropertyDescriptor property = this._typeDescriptor.GetProperty(expectedType, (object) null, scalar.Value, this._ignoreUnmatched);
        if (property == null)
        {
          parser.SkipThisAndNestedEvents();
        }
        else
        {
          object obj1 = nestedObjectDeserializer(parser, property.Type);
          if (!(obj1 is IValuePromise valuePromise))
          {
            object obj2 = TypeConverter.ChangeType(obj1, property.Type);
            property.Write(value, obj2);
          }
          else
          {
            object valueRef = value;
            valuePromise.ValueAvailable += (Action<object>) (v => property.Write(valueRef, TypeConverter.ChangeType(v, property.Type)));
          }
        }
      }
      parser.Expect<MappingEnd>();
      return true;
    }
  }
}
