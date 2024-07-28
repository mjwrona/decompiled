// Decompiled with JetBrains decompiler
// Type: Nest.PropertyWalker
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Nest
{
  public class PropertyWalker
  {
    private readonly int _maxRecursion;
    private readonly ConcurrentDictionary<Type, int> _seenTypes;
    private readonly Type _type;
    private readonly IPropertyVisitor _visitor;

    public PropertyWalker(Type type, IPropertyVisitor visitor, int maxRecursion = 0)
    {
      this._type = PropertyWalker.GetUnderlyingType(type);
      this._visitor = visitor ?? (IPropertyVisitor) new NoopPropertyVisitor();
      this._maxRecursion = maxRecursion;
      this._seenTypes = new ConcurrentDictionary<Type, int>();
      this._seenTypes.TryAdd(this._type, 0);
    }

    private PropertyWalker(
      Type type,
      IPropertyVisitor visitor,
      int maxRecursion,
      ConcurrentDictionary<Type, int> seenTypes)
    {
      this._type = type;
      this._visitor = visitor;
      this._maxRecursion = maxRecursion;
      this._seenTypes = seenTypes;
    }

    public IProperties GetProperties(ConcurrentDictionary<Type, int> seenTypes = null, int maxRecursion = 0)
    {
      Properties properties = new Properties();
      int num;
      if (seenTypes != null && seenTypes.TryGetValue(this._type, out num) && num > maxRecursion)
        return (IProperties) properties;
      foreach (PropertyInfo allProperty in this._type.GetAllProperties())
      {
        ElasticsearchPropertyAttributeBase attribute = ElasticsearchPropertyAttributeBase.From((MemberInfo) allProperty);
        if ((attribute == null || !attribute.Ignore) && !this._visitor.SkipProperty(allProperty, attribute))
        {
          IProperty property = this.GetProperty(allProperty, attribute);
          if (property is IPropertyWithClrOrigin propertyWithClrOrigin)
            propertyWithClrOrigin.ClrOrigin = allProperty;
          properties.Add((PropertyName) allProperty, property);
        }
      }
      return (IProperties) properties;
    }

    private IProperty GetProperty(
      PropertyInfo propertyInfo,
      ElasticsearchPropertyAttributeBase attribute)
    {
      IProperty property = this._visitor.Visit(propertyInfo, attribute);
      if (property != null)
        return property;
      if (propertyInfo.GetMethod.IsStatic)
        return (IProperty) null;
      IProperty type = (IProperty) attribute ?? PropertyWalker.InferProperty(propertyInfo);
      if (type is IObjectProperty objectProperty)
      {
        Type underlyingType = PropertyWalker.GetUnderlyingType(propertyInfo.PropertyType);
        ConcurrentDictionary<Type, int> seenTypes = new ConcurrentDictionary<Type, int>((IEnumerable<KeyValuePair<Type, int>>) this._seenTypes);
        seenTypes.AddOrUpdate(underlyingType, 0, (Func<Type, int, int>) ((t, i) => ++i));
        PropertyWalker propertyWalker = new PropertyWalker(underlyingType, this._visitor, this._maxRecursion, seenTypes);
        objectProperty.Properties = propertyWalker.GetProperties(seenTypes, this._maxRecursion);
      }
      this._visitor.Visit(type, propertyInfo, attribute);
      return type;
    }

    private static IProperty InferProperty(PropertyInfo propertyInfo)
    {
      Type underlyingType = PropertyWalker.GetUnderlyingType(propertyInfo.PropertyType);
      if (underlyingType == typeof (string))
      {
        TextProperty textProperty = new TextProperty();
        textProperty.Fields = (IProperties) new Properties()
        {
          {
            (PropertyName) "keyword",
            (IProperty) new KeywordProperty()
            {
              IgnoreAbove = new int?(256)
            }
          }
        };
        return (IProperty) textProperty;
      }
      if (underlyingType.IsEnum)
        return underlyingType.GetCustomAttribute<StringEnumAttribute>() != null || propertyInfo.GetCustomAttribute<StringEnumAttribute>() != null ? (IProperty) new KeywordProperty() : (IProperty) new NumberProperty(NumberType.Integer);
      if (underlyingType.IsValueType)
      {
        switch (underlyingType.Name)
        {
          case "Boolean":
            return (IProperty) new BooleanProperty();
          case "Byte":
          case "Int16":
            return (IProperty) new NumberProperty(NumberType.Short);
          case "Char":
          case "Guid":
            return (IProperty) new KeywordProperty();
          case "DateTime":
          case "DateTimeOffset":
            return (IProperty) new DateProperty();
          case "Decimal":
          case "Double":
          case "UInt64":
            return (IProperty) new NumberProperty(NumberType.Double);
          case "Int32":
          case "UInt16":
            return (IProperty) new NumberProperty(NumberType.Integer);
          case "Int64":
          case "TimeSpan":
          case "UInt32":
            return (IProperty) new NumberProperty(NumberType.Long);
          case "SByte":
            return (IProperty) new NumberProperty(NumberType.Byte);
          case "Single":
            return (IProperty) new NumberProperty(NumberType.Float);
        }
      }
      if (underlyingType == typeof (GeoLocation))
        return (IProperty) new GeoPointProperty();
      if (underlyingType == typeof (CompletionField))
        return (IProperty) new CompletionProperty();
      if (underlyingType == typeof (DateRange))
        return (IProperty) new DateRangeProperty();
      if (underlyingType == typeof (DoubleRange))
        return (IProperty) new DoubleRangeProperty();
      if (underlyingType == typeof (FloatRange))
        return (IProperty) new FloatRangeProperty();
      if (underlyingType == typeof (IntegerRange))
        return (IProperty) new IntegerRangeProperty();
      if (underlyingType == typeof (LongRange))
        return (IProperty) new LongRangeProperty();
      if (underlyingType == typeof (IpAddressRange))
        return (IProperty) new IpRangeProperty();
      if (underlyingType == typeof (QueryContainer))
        return (IProperty) new PercolatorProperty();
      return underlyingType == typeof (IGeoShape) ? (IProperty) new GeoShapeProperty() : (IProperty) new ObjectProperty();
    }

    private static Type GetUnderlyingType(Type type)
    {
      if (type.IsArray)
        return type.GetElementType();
      return type.IsGenericType && type.GetGenericArguments().Length == 1 && (((IEnumerable<Type>) type.GetInterfaces()).HasAny<Type>((Func<Type, bool>) (t => t == typeof (IEnumerable))) || Nullable.GetUnderlyingType(type) != (Type) null) ? type.GetGenericArguments()[0] : type;
    }
  }
}
