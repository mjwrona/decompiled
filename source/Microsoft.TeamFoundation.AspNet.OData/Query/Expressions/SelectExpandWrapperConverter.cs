// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.SelectExpandWrapperConverter
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class SelectExpandWrapperConverter : JsonConverter
  {
    private static readonly Func<IEdmModel, IEdmStructuredType, IPropertyMapper> _mapperProvider = (Func<IEdmModel, IEdmStructuredType, IPropertyMapper>) ((model, type) => (IPropertyMapper) new SelectExpandWrapperConverter.JsonPropertyNameMapper(model, type));

    public override bool CanConvert(Type objectType) => !(objectType == (Type) null) ? objectType.IsAssignableFrom(typeof (ISelectExpandWrapper)) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (objectType));

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (!(value is ISelectExpandWrapper selectExpandWrapper))
        return;
      serializer.Serialize(writer, (object) selectExpandWrapper.ToDictionary(SelectExpandWrapperConverter._mapperProvider));
    }

    private class JsonPropertyNameMapper : IPropertyMapper
    {
      private IEdmModel _model;
      private IEdmStructuredType _type;

      public JsonPropertyNameMapper(IEdmModel model, IEdmStructuredType type)
      {
        this._model = model;
        this._type = type;
      }

      public string MapProperty(string propertyName)
      {
        IEdmProperty property = this._type.Properties().Single<IEdmProperty>((Func<IEdmProperty, bool>) (s => s.Name == propertyName));
        JsonPropertyAttribute jsonProperty = SelectExpandWrapperConverter.JsonPropertyNameMapper.GetJsonProperty(this.GetPropertyInfo(property));
        return jsonProperty != null && !string.IsNullOrWhiteSpace(jsonProperty.PropertyName) ? jsonProperty.PropertyName : property.Name;
      }

      private PropertyInfo GetPropertyInfo(IEdmProperty property)
      {
        ClrPropertyInfoAnnotation annotationValue = this._model.GetAnnotationValue<ClrPropertyInfoAnnotation>((IEdmElement) property);
        return annotationValue != null ? annotationValue.ClrPropertyInfo : this._model.GetAnnotationValue<ClrTypeAnnotation>((IEdmElement) property.DeclaringType).ClrType.GetProperty(property.Name);
      }

      private static JsonPropertyAttribute GetJsonProperty(PropertyInfo property) => property.GetCustomAttributes(typeof (JsonPropertyAttribute), false).OfType<JsonPropertyAttribute>().SingleOrDefault<JsonPropertyAttribute>();
    }
  }
}
