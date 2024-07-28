// Decompiled with JetBrains decompiler
// Type: Nest.PropertiesFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Formatters;
using System.Collections.Generic;
using System.Reflection;

namespace Nest
{
  internal class PropertiesFormatter : IJsonFormatter<IProperties>, IJsonFormatter
  {
    private static readonly InterfaceDictionaryFormatter<PropertyName, IProperty> Formatter = new InterfaceDictionaryFormatter<PropertyName, IProperty>();

    public IProperties Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      Properties properties = new Properties(formatterResolver.GetConnectionSettings());
      IJsonFormatter<IProperty> formatter = formatterResolver.GetFormatter<IProperty>();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        string name = reader.ReadPropertyName();
        if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
        {
          reader.ReadNextBlock();
        }
        else
        {
          IProperty property = formatter.Deserialize(ref reader, formatterResolver);
          property.Name = (PropertyName) name;
          properties.Add((PropertyName) name, property);
        }
      }
      return (IProperties) properties;
    }

    public void Serialize(
      ref JsonWriter writer,
      IProperties value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        IConnectionSettingsValues connectionSettings = formatterResolver.GetConnectionSettings();
        Properties properties = new Properties(connectionSettings);
        foreach (KeyValuePair<PropertyName, IProperty> keyValuePair in (IEnumerable<KeyValuePair<PropertyName, IProperty>>) value)
        {
          PropertyInfo clrOrigin = keyValuePair.Value is IPropertyWithClrOrigin propertyWithClrOrigin ? propertyWithClrOrigin.ClrOrigin : (PropertyInfo) null;
          if (clrOrigin == (PropertyInfo) null)
          {
            properties[keyValuePair.Key] = keyValuePair.Value;
          }
          else
          {
            IPropertyMapping propertyMapping;
            if (connectionSettings.PropertyMappings.TryGetValue((MemberInfo) clrOrigin, out propertyMapping))
            {
              if (!propertyMapping.Ignore)
                properties[(PropertyName) propertyMapping.Name] = keyValuePair.Value;
            }
            else
            {
              propertyMapping = connectionSettings.PropertyMappingProvider?.CreatePropertyMapping((MemberInfo) clrOrigin);
              if (propertyMapping == null || !propertyMapping.Ignore)
                properties[keyValuePair.Key] = keyValuePair.Value;
            }
          }
        }
        PropertiesFormatter.Formatter.Serialize(ref writer, (IDictionary<PropertyName, IProperty>) properties, formatterResolver);
      }
    }
  }
}
