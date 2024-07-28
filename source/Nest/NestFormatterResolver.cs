// Decompiled with JetBrains decompiler
// Type: Nest.NestFormatterResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Formatters;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Nest
{
  internal class NestFormatterResolver : IJsonFormatterResolver, IJsonFormatterResolverWithSettings
  {
    private readonly IJsonFormatter<object> _fallbackFormatter;
    private readonly NestFormatterResolver.InnerResolver _innerFormatterResolver;

    public NestFormatterResolver(IConnectionSettingsValues settings)
    {
      this.Settings = settings;
      this._innerFormatterResolver = new NestFormatterResolver.InnerResolver(settings);
      this._fallbackFormatter = (IJsonFormatter<object>) new DynamicObjectTypeFallbackFormatter(new IJsonFormatterResolver[1]
      {
        (IJsonFormatterResolver) this._innerFormatterResolver
      });
    }

    public IConnectionSettingsValues Settings { get; }

    public IJsonFormatter<T> GetFormatter<T>() => !(typeof (T) == typeof (object)) ? this._innerFormatterResolver.GetFormatter<T>() : (IJsonFormatter<T>) this._fallbackFormatter;

    internal sealed class InnerResolver : IJsonFormatterResolver
    {
      private static readonly IJsonFormatterResolver[] Resolvers = new IJsonFormatterResolver[9]
      {
        DynamicCompositeResolver.Create(new IJsonFormatter[9]
        {
          (IJsonFormatter) new QueryContainerCollectionFormatter(),
          (IJsonFormatter) new SimpleQueryStringFlagsFormatter(),
          (IJsonFormatter) new TimeSpanTicksFormatter(),
          (IJsonFormatter) new NullableTimeSpanTicksFormatter(),
          (IJsonFormatter) new JsonNetCompatibleUriFormatter(),
          (IJsonFormatter) new GeoOrientationFormatter(),
          (IJsonFormatter) new NullableGeoOrientationFormatter(),
          (IJsonFormatter) new ShapeOrientationFormatter(),
          (IJsonFormatter) new NullableShapeOrientationFormatter()
        }, new IJsonFormatterResolver[0]),
        BuiltinResolver.Instance,
        ElasticsearchNetEnumResolver.Instance,
        AttributeFormatterResolver.Instance,
        ReadAsFormatterResolver.Instance,
        (IJsonFormatterResolver) IsADictionaryFormatterResolver.Instance,
        DynamicGenericResolver.Instance,
        (IJsonFormatterResolver) InterfaceGenericDictionaryResolver.Instance,
        (IJsonFormatterResolver) InterfaceGenericReadOnlyDictionaryResolver.Instance
      };
      private readonly IJsonFormatterResolver _finalFormatter;
      private readonly ConcurrentDictionary<Type, object> _formatters = new ConcurrentDictionary<Type, object>();
      private readonly IConnectionSettingsValues _settings;

      internal InnerResolver(IConnectionSettingsValues settings)
      {
        this._settings = settings;
        this._finalFormatter = DynamicObjectResolver.Create(new Func<MemberInfo, JsonProperty>(this.GetMapping), new Lazy<Func<string, string>>((Func<Func<string, string>>) (() => settings.DefaultFieldNameInferrer)), true);
      }

      public IJsonFormatter<T> GetFormatter<T>() => (IJsonFormatter<T>) this._formatters.GetOrAdd(typeof (T), (Func<Type, object>) (type =>
      {
        foreach (IJsonFormatterResolver resolver in NestFormatterResolver.InnerResolver.Resolvers)
        {
          IJsonFormatter<T> formatter = resolver.GetFormatter<T>();
          if (formatter != null)
            return (object) formatter;
        }
        return (object) this._finalFormatter.GetFormatter<T>();
      }));

      private JsonProperty GetMapping(MemberInfo member)
      {
        IPropertyMapping propertyMapping1;
        if (!this._settings.PropertyMappings.TryGetValue(member, out propertyMapping1))
          propertyMapping1 = (IPropertyMapping) ElasticsearchPropertyAttributeBase.From(member);
        IPropertyMapping propertyMapping2 = this._settings.PropertyMappingProvider?.CreatePropertyMapping(member);
        JsonProperty property = new JsonProperty(propertyMapping1?.Name ?? propertyMapping2?.Name);
        bool? nullable = propertyMapping1 != null ? new bool?(propertyMapping1.Ignore) : propertyMapping2?.Ignore;
        if (nullable.HasValue)
          property.Ignore = nullable.Value;
        if (propertyMapping1 != null || propertyMapping2 != null)
          property.AllowPrivate = new bool?(true);
        if (member.GetCustomAttribute<StringEnumAttribute>() != null)
          NestFormatterResolver.InnerResolver.CreateEnumFormatterForProperty(member, property);
        else if (member.GetCustomAttribute<StringTimeSpanAttribute>() != null)
        {
          PropertyInfo propertyInfo = member as PropertyInfo;
          if ((object) propertyInfo == null)
          {
            FieldInfo fieldInfo = member as FieldInfo;
            if ((object) fieldInfo != null)
              property.JsonFormatter = BuiltinResolver.BuiltinResolverGetFormatterHelper.GetFormatter(fieldInfo.FieldType);
          }
          else
            property.JsonFormatter = BuiltinResolver.BuiltinResolverGetFormatterHelper.GetFormatter(propertyInfo.PropertyType);
        }
        else if (member.GetCustomAttribute<MachineLearningDateTimeAttribute>() != null)
          property.JsonFormatter = (object) MachineLearningDateTimeFormatter.Instance;
        return property;
      }

      private static void CreateEnumFormatterForType(Type type, JsonProperty property)
      {
        if (type.IsEnum)
        {
          property.JsonFormatter = typeof (EnumFormatter<>).MakeGenericType(type).CreateInstance((object) true);
        }
        else
        {
          if (!type.IsNullable())
            return;
          Type underlyingType = Nullable.GetUnderlyingType(type);
          if (!underlyingType.IsEnum)
            return;
          object instance = typeof (EnumFormatter<>).MakeGenericType(underlyingType).CreateInstance((object) true);
          property.JsonFormatter = typeof (StaticNullableFormatter<>).MakeGenericType(underlyingType).CreateInstance(instance);
        }
      }

      private static void CreateEnumFormatterForProperty(MemberInfo member, JsonProperty property)
      {
        PropertyInfo propertyInfo = member as PropertyInfo;
        if ((object) propertyInfo == null)
        {
          FieldInfo fieldInfo = member as FieldInfo;
          if ((object) fieldInfo == null)
            return;
          NestFormatterResolver.InnerResolver.CreateEnumFormatterForType(fieldInfo.FieldType, property);
        }
        else
          NestFormatterResolver.InnerResolver.CreateEnumFormatterForType(propertyInfo.PropertyType, property);
      }
    }
  }
}
