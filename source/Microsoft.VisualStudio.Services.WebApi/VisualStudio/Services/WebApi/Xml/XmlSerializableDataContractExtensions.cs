// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Xml.XmlSerializableDataContractExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi.Xml
{
  public static class XmlSerializableDataContractExtensions
  {
    private static ConcurrentDictionary<TypeInfo, XmlSerializableDataContractExtensions.SerializableProperties> SerializablePropertiesByType = new ConcurrentDictionary<TypeInfo, XmlSerializableDataContractExtensions.SerializableProperties>();
    private static ConcurrentDictionary<TypeInfo, string> NamespacesByType = new ConcurrentDictionary<TypeInfo, string>();
    private static ConcurrentDictionary<XmlSerializableDataContractExtensions.SerializerKey, XmlSerializer> Serializers = new ConcurrentDictionary<XmlSerializableDataContractExtensions.SerializerKey, XmlSerializer>();

    public static void ReadDataMemberXml<T>(this XmlReader reader, T destination) where T : class
    {
      ArgumentUtility.CheckForNull<XmlReader>(reader, nameof (reader));
      int num1 = reader.IsEmptyElement ? 1 : 0;
      reader.ReadStartElement();
      if (num1 != 0)
        return;
      IReadOnlyDictionary<string, XmlSerializableDataContractExtensions.SerializableProperty> mappedByName = XmlSerializableDataContractExtensions.GetSerializableProperties(destination.GetType().GetTypeInfo()).MappedByName;
      while (reader.IsStartElement())
      {
        XmlSerializableDataContractExtensions.SerializableProperty serializableProperty;
        if (!mappedByName.TryGetValue(reader.LocalName, out serializableProperty))
        {
          reader.ReadOuterXml();
        }
        else
        {
          int num2 = reader.IsEmptyElement ? 1 : 0;
          object obj = !((object) destination is GraphSubjectBase) || !(serializableProperty.SerializedName == "Descriptor") ? XmlSerializableDataContractExtensions.GetSerializer(reader.NamespaceURI, reader.LocalName, serializableProperty.SerializedType).Deserialize(reader.ReadSubtree()) : (object) XmlSerializableDataContractExtensions.GetSerializer(reader.NamespaceURI, reader.LocalName, typeof (SubjectDescriptor)).Deserialize(reader.ReadSubtree()).ToString();
          serializableProperty.SetValue((object) destination, obj);
          if (num2 != 0)
            reader.ReadOuterXml();
          else
            reader.ReadEndElement();
        }
      }
      reader.ReadEndElement();
    }

    public static void WriteDataMemberXml(this XmlWriter writer, object source)
    {
      ArgumentUtility.CheckForNull<XmlWriter>(writer, nameof (writer));
      TypeInfo typeInfo = source.GetType().GetTypeInfo();
      string rootNamespace = writer.Settings == null ? XmlSerializableDataContractExtensions.GetNamespace(typeInfo) : (string) null;
      foreach (XmlSerializableDataContractExtensions.SerializableProperty serializableProperty in (IEnumerable<XmlSerializableDataContractExtensions.SerializableProperty>) XmlSerializableDataContractExtensions.GetSerializableProperties(typeInfo).EnumeratedInOrder)
      {
        if (serializableProperty.ShouldSerialize(source))
        {
          object o = serializableProperty.GetValue(source);
          if (!serializableProperty.IsIgnorableDefaultValue(o))
          {
            XmlSerializableDataContractExtensions.GetSerializer(rootNamespace, serializableProperty.SerializedName, serializableProperty.SerializedType).Serialize(writer, o);
            if (!string.IsNullOrEmpty(serializableProperty.SerializedNameForCamelCaseCompat))
              XmlSerializableDataContractExtensions.GetSerializer(rootNamespace, serializableProperty.SerializedNameForCamelCaseCompat, serializableProperty.SerializedType).Serialize(writer, o);
          }
        }
      }
    }

    private static string GetNamespace(TypeInfo type)
    {
      string str;
      if (!XmlSerializableDataContractExtensions.NamespacesByType.TryGetValue(type, out str))
      {
        str = "http://schemas.datacontract.org/2004/07/" + type.Namespace;
        XmlSerializableDataContractExtensions.NamespacesByType.TryAdd(type, str);
      }
      return str;
    }

    private static XmlSerializableDataContractExtensions.SerializableProperties GetSerializableProperties(
      TypeInfo type)
    {
      XmlSerializableDataContractExtensions.SerializableProperties serializableProperties1;
      if (XmlSerializableDataContractExtensions.SerializablePropertiesByType.TryGetValue(type, out serializableProperties1))
        return serializableProperties1;
      bool enableCamelCaseNameCompat = type.GetCustomAttribute(typeof (XmlSerializableDataContractAttribute)) is XmlSerializableDataContractAttribute customAttribute1 && customAttribute1.EnableCamelCaseNameCompat;
      List<XmlSerializableDataContractExtensions.SerializableProperty> declaredProperties = new List<XmlSerializableDataContractExtensions.SerializableProperty>();
      foreach (PropertyInfo declaredProperty in type.DeclaredProperties)
      {
        if (declaredProperty.GetCustomAttribute(typeof (XmlIgnoreAttribute)) == null && !(declaredProperty.SetMethod == (MethodInfo) null) && declaredProperty.GetCustomAttribute(typeof (DataMemberAttribute)) is DataMemberAttribute customAttribute2)
        {
          string name = "ShouldSerialize" + declaredProperty.Name;
          MethodInfo declaredMethod = type.GetDeclaredMethod(name);
          declaredProperties.Add(new XmlSerializableDataContractExtensions.SerializableProperty(declaredProperty, customAttribute2, declaredMethod, enableCamelCaseNameCompat));
        }
      }
      IEnumerable<XmlSerializableDataContractExtensions.SerializableProperty> inheritedProperties = Enumerable.Empty<XmlSerializableDataContractExtensions.SerializableProperty>();
      if (type.BaseType != typeof (object))
        inheritedProperties = (IEnumerable<XmlSerializableDataContractExtensions.SerializableProperty>) XmlSerializableDataContractExtensions.GetSerializableProperties(type.BaseType.GetTypeInfo()).EnumeratedInOrder;
      XmlSerializableDataContractExtensions.SerializableProperties serializableProperties2 = new XmlSerializableDataContractExtensions.SerializableProperties((IEnumerable<XmlSerializableDataContractExtensions.SerializableProperty>) declaredProperties, inheritedProperties);
      return XmlSerializableDataContractExtensions.SerializablePropertiesByType.GetOrAdd(type, serializableProperties2);
    }

    private static XmlSerializer GetSerializer(
      string rootNamespace,
      string elementName,
      Type elementType)
    {
      XmlSerializableDataContractExtensions.SerializerKey key = new XmlSerializableDataContractExtensions.SerializerKey(rootNamespace, elementName, elementType);
      return XmlSerializableDataContractExtensions.Serializers.GetOrAdd(key, (Func<XmlSerializableDataContractExtensions.SerializerKey, XmlSerializer>) (_ => new XmlSerializer(elementType, new XmlRootAttribute(elementName)
      {
        Namespace = rootNamespace
      })));
    }

    private static HashSet<TOut> ToHashSet<TIn, TOut>(
      this IEnumerable<TIn> source,
      Func<TIn, TOut> selector)
    {
      return new HashSet<TOut>(source.Select<TIn, TOut>(selector));
    }

    private class SerializableProperties
    {
      public IReadOnlyDictionary<string, XmlSerializableDataContractExtensions.SerializableProperty> MappedByName { get; }

      public IReadOnlyList<XmlSerializableDataContractExtensions.SerializableProperty> EnumeratedInOrder { get; }

      public SerializableProperties(
        IEnumerable<XmlSerializableDataContractExtensions.SerializableProperty> declaredProperties,
        IEnumerable<XmlSerializableDataContractExtensions.SerializableProperty> inheritedProperties)
      {
        HashSet<string> declaredPropertyNames = declaredProperties.ToHashSet<XmlSerializableDataContractExtensions.SerializableProperty, string>((Func<XmlSerializableDataContractExtensions.SerializableProperty, string>) (property => property.SerializedName));
        this.EnumeratedInOrder = (IReadOnlyList<XmlSerializableDataContractExtensions.SerializableProperty>) inheritedProperties.Where<XmlSerializableDataContractExtensions.SerializableProperty>((Func<XmlSerializableDataContractExtensions.SerializableProperty, bool>) (inheritedProperty => !declaredPropertyNames.Contains(inheritedProperty.SerializedName))).Concat<XmlSerializableDataContractExtensions.SerializableProperty>((IEnumerable<XmlSerializableDataContractExtensions.SerializableProperty>) declaredProperties.OrderBy<XmlSerializableDataContractExtensions.SerializableProperty, string>((Func<XmlSerializableDataContractExtensions.SerializableProperty, string>) (property => property.SerializedName))).ToList<XmlSerializableDataContractExtensions.SerializableProperty>();
        Dictionary<string, XmlSerializableDataContractExtensions.SerializableProperty> dictionary = new Dictionary<string, XmlSerializableDataContractExtensions.SerializableProperty>();
        foreach (XmlSerializableDataContractExtensions.SerializableProperty serializableProperty in (IEnumerable<XmlSerializableDataContractExtensions.SerializableProperty>) this.EnumeratedInOrder)
        {
          dictionary.Add(serializableProperty.SerializedName, serializableProperty);
          if (serializableProperty.SerializedNameForCamelCaseCompat != null)
            dictionary.TryAdd<string, XmlSerializableDataContractExtensions.SerializableProperty>(serializableProperty.SerializedNameForCamelCaseCompat, serializableProperty);
        }
        this.MappedByName = (IReadOnlyDictionary<string, XmlSerializableDataContractExtensions.SerializableProperty>) dictionary;
      }

      private Dictionary<string, XmlSerializableDataContractExtensions.SerializableProperty> PropertiesDictionary { get; }
    }

    [DebuggerDisplay("Name={SerializedName} Type={SerializedType}")]
    private class SerializableProperty
    {
      private static ConcurrentDictionary<Type, object> DefaultValuesByType = new ConcurrentDictionary<Type, object>();

      public Type SerializedType => this.Property.PropertyType;

      public string SerializedName { get; }

      public string SerializedNameForCamelCaseCompat { get; }

      public SerializableProperty(
        PropertyInfo property,
        DataMemberAttribute dataMember,
        MethodInfo shouldSerializeMethod,
        bool enableCamelCaseNameCompat)
      {
        this.Property = property;
        this.DataMember = dataMember;
        this.ShouldSerializeMethod = shouldSerializeMethod;
        this.SerializedName = this.DataMember?.Name ?? this.Property.Name;
        this.SerializedNameForCamelCaseCompat = this.ComputeSerializedNameForCameCaseCompat(enableCamelCaseNameCompat);
      }

      public object GetValue(object @object) => this.Property.GetValue(@object);

      public void SetValue(object @object, object value) => this.Property.SetValue(@object, value);

      public bool ShouldSerialize(object @object) => this.ShouldSerializeMethod == (MethodInfo) null || (bool) this.ShouldSerializeMethod.Invoke(@object, new object[0]);

      public bool IsIgnorableDefaultValue(object value)
      {
        if (this.DataMember.EmitDefaultValue)
          return false;
        Type serializedType = this.SerializedType;
        if (!serializedType.GetTypeInfo().IsValueType)
          return value == null;
        object orAdd = XmlSerializableDataContractExtensions.SerializableProperty.DefaultValuesByType.GetOrAdd(serializedType, (Func<Type, object>) (key => Activator.CreateInstance(key)));
        return object.Equals(value, orAdd);
      }

      private string ComputeSerializedNameForCameCaseCompat(bool enableCamelCaseNameCompat)
      {
        if (!enableCamelCaseNameCompat)
          return (string) null;
        string upperCamelCase = XmlSerializableDataContractExtensions.SerializableProperty.ConvertToUpperCamelCase(this.SerializedName);
        return string.Equals(upperCamelCase, this.SerializedName) ? (string) null : upperCamelCase;
      }

      private static string ConvertToUpperCamelCase(string input) => char.ToUpperInvariant(input[0]).ToString() + input.Substring(1);

      private PropertyInfo Property { get; }

      private DataMemberAttribute DataMember { get; }

      private MethodInfo ShouldSerializeMethod { get; }
    }

    private struct SerializerKey
    {
      public string RootNamespace { get; }

      public string ElementName { get; }

      public Type ElementType { get; }

      public SerializerKey(string rootNamespace, string elementName, Type elementType)
      {
        ArgumentUtility.CheckForNull<string>(elementName, nameof (elementName));
        ArgumentUtility.CheckForNull<Type>(elementType, nameof (elementType));
        this.RootNamespace = rootNamespace;
        this.ElementName = elementName;
        this.ElementType = elementType;
      }

      public override bool Equals(object other) => other is XmlSerializableDataContractExtensions.SerializerKey serializerKey && this.RootNamespace == serializerKey.RootNamespace && this.ElementName == serializerKey.ElementName && this.ElementType == serializerKey.ElementType;

      public override int GetHashCode()
      {
        int num1 = 7443;
        int num2 = (num1 << 19) - num1;
        string rootNamespace = this.RootNamespace;
        int hashCode = rootNamespace != null ? rootNamespace.GetHashCode() : 0;
        int num3 = num2 + hashCode;
        int num4 = (num3 << 19) - num3 + this.ElementName.GetHashCode();
        return (num4 << 19) - num4 + this.ElementType.GetHashCode();
      }
    }
  }
}
