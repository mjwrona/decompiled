// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectGraphTraversalStrategies.FullObjectGraphTraversalStrategy
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Helpers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ObjectGraphTraversalStrategies
{
  public class FullObjectGraphTraversalStrategy : IObjectGraphTraversalStrategy
  {
    private readonly int maxRecursion;
    private readonly ITypeInspector typeDescriptor;
    private readonly ITypeResolver typeResolver;
    private INamingConvention namingConvention;

    public FullObjectGraphTraversalStrategy(
      ITypeInspector typeDescriptor,
      ITypeResolver typeResolver,
      int maxRecursion,
      INamingConvention namingConvention)
    {
      if (maxRecursion <= 0)
        throw new ArgumentOutOfRangeException(nameof (maxRecursion), (object) maxRecursion, "maxRecursion must be greater than 1");
      this.typeDescriptor = typeDescriptor != null ? typeDescriptor : throw new ArgumentNullException(nameof (typeDescriptor));
      this.typeResolver = typeResolver != null ? typeResolver : throw new ArgumentNullException(nameof (typeResolver));
      this.maxRecursion = maxRecursion;
      this.namingConvention = namingConvention;
    }

    void IObjectGraphTraversalStrategy.Traverse<TContext>(
      IObjectDescriptor graph,
      IObjectGraphVisitor<TContext> visitor,
      TContext context)
    {
      this.Traverse<TContext>(graph, visitor, 0, context);
    }

    protected virtual void Traverse<TContext>(
      IObjectDescriptor value,
      IObjectGraphVisitor<TContext> visitor,
      int currentDepth,
      TContext context)
    {
      if (++currentDepth > this.maxRecursion)
        throw new InvalidOperationException("Too much recursion when traversing the object graph");
      if (!visitor.Enter(value, context))
        return;
      TypeCode typeCode = value.Type.GetTypeCode();
      switch (typeCode)
      {
        case TypeCode.Empty:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", new object[1]
          {
            (object) typeCode
          }));
        case TypeCode.Boolean:
        case TypeCode.Char:
        case TypeCode.SByte:
        case TypeCode.Byte:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
        case TypeCode.DateTime:
        case TypeCode.String:
          visitor.VisitScalar(value, context);
          break;
        default:
          if (value.IsDbNull())
            visitor.VisitScalar((IObjectDescriptor) new ObjectDescriptor((object) null, typeof (object), typeof (object)), context);
          if (value.Value == null || (object) value.Type == (object) typeof (TimeSpan))
          {
            visitor.VisitScalar(value, context);
            break;
          }
          Type underlyingType = Nullable.GetUnderlyingType(value.Type);
          if ((object) underlyingType != null)
          {
            this.Traverse<TContext>((IObjectDescriptor) new ObjectDescriptor(value.Value, underlyingType, value.Type, value.ScalarStyle), visitor, currentDepth, context);
            break;
          }
          this.TraverseObject<TContext>(value, visitor, currentDepth, context);
          break;
      }
    }

    protected virtual void TraverseObject<TContext>(
      IObjectDescriptor value,
      IObjectGraphVisitor<TContext> visitor,
      int currentDepth,
      TContext context)
    {
      if (typeof (IDictionary).IsAssignableFrom(value.Type))
      {
        this.TraverseDictionary<TContext>(value, visitor, currentDepth, typeof (object), typeof (object), context);
      }
      else
      {
        Type genericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof (IDictionary<,>));
        if ((object) genericInterface != null)
        {
          GenericDictionaryToNonGenericAdapter nonGenericAdapter = new GenericDictionaryToNonGenericAdapter(value.Value, genericInterface);
          Type[] genericArguments = genericInterface.GetGenericArguments();
          this.TraverseDictionary<TContext>((IObjectDescriptor) new ObjectDescriptor((object) nonGenericAdapter, value.Type, value.StaticType, value.ScalarStyle), visitor, currentDepth, genericArguments[0], genericArguments[1], context);
        }
        else if (typeof (IEnumerable).IsAssignableFrom(value.Type))
          this.TraverseList<TContext>(value, visitor, currentDepth, context);
        else
          this.TraverseProperties<TContext>(value, visitor, currentDepth, context);
      }
    }

    protected virtual void TraverseDictionary<TContext>(
      IObjectDescriptor dictionary,
      IObjectGraphVisitor<TContext> visitor,
      int currentDepth,
      Type keyType,
      Type valueType,
      TContext context)
    {
      visitor.VisitMappingStart(dictionary, keyType, valueType, context);
      bool flag = dictionary.Type.FullName.Equals("System.Dynamic.ExpandoObject");
      foreach (DictionaryEntry dictionaryEntry in (IDictionary) dictionary.Value)
      {
        IObjectDescriptor objectDescriptor1 = this.GetObjectDescriptor(flag ? (object) this.namingConvention.Apply(dictionaryEntry.Key.ToString()) : dictionaryEntry.Key, keyType);
        IObjectDescriptor objectDescriptor2 = this.GetObjectDescriptor(dictionaryEntry.Value, valueType);
        if (visitor.EnterMapping(objectDescriptor1, objectDescriptor2, context))
        {
          this.Traverse<TContext>(objectDescriptor1, visitor, currentDepth, context);
          this.Traverse<TContext>(objectDescriptor2, visitor, currentDepth, context);
        }
      }
      visitor.VisitMappingEnd(dictionary, context);
    }

    private void TraverseList<TContext>(
      IObjectDescriptor value,
      IObjectGraphVisitor<TContext> visitor,
      int currentDepth,
      TContext context)
    {
      Type genericInterface = ReflectionUtility.GetImplementedGenericInterface(value.Type, typeof (IEnumerable<>));
      Type type = (object) genericInterface != null ? genericInterface.GetGenericArguments()[0] : typeof (object);
      visitor.VisitSequenceStart(value, type, context);
      foreach (object obj in (IEnumerable) value.Value)
        this.Traverse<TContext>(this.GetObjectDescriptor(obj, type), visitor, currentDepth, context);
      visitor.VisitSequenceEnd(value, context);
    }

    protected virtual void TraverseProperties<TContext>(
      IObjectDescriptor value,
      IObjectGraphVisitor<TContext> visitor,
      int currentDepth,
      TContext context)
    {
      visitor.VisitMappingStart(value, typeof (string), typeof (object), context);
      foreach (IPropertyDescriptor property in this.typeDescriptor.GetProperties(value.Type, value.Value))
      {
        IObjectDescriptor objectDescriptor = property.Read(value.Value);
        if (visitor.EnterMapping(property, objectDescriptor, context))
        {
          this.Traverse<TContext>((IObjectDescriptor) new ObjectDescriptor((object) property.Name, typeof (string), typeof (string)), visitor, currentDepth, context);
          this.Traverse<TContext>(objectDescriptor, visitor, currentDepth, context);
        }
      }
      visitor.VisitMappingEnd(value, context);
    }

    private IObjectDescriptor GetObjectDescriptor(object value, Type staticType) => (IObjectDescriptor) new ObjectDescriptor(value, this.typeResolver.Resolve(staticType, value), staticType);
  }
}
