// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ODataJsonSchemaPropertySegmentTranslator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class ODataJsonSchemaPropertySegmentTranslator : PathSegmentTranslator<ODataJsonSchema>
  {
    private IEdmModel model;

    public ODataJsonSchemaPropertySegmentTranslator(IEdmModel model) => this.model = model;

    public override ODataJsonSchema Translate(NavigationPropertySegment segment) => new ODataJsonSchema()
    {
      DisplayNameAnnotation = this.GetPropertyName((IEdmProperty) segment.NavigationProperty, "Display.DisplayName"),
      Type = ODataJsonSchemaType.Object
    };

    public override ODataJsonSchema Translate(PropertySegment segment)
    {
      this.ThrowIfNotPrimitive(segment);
      return new ODataJsonSchema()
      {
        DisplayNameAnnotation = this.GetPropertyName((IEdmProperty) segment.Property, "Display.DisplayName"),
        ReferenceNameAnnotation = this.GetPropertyName((IEdmProperty) segment.Property, "Ref.ReferenceName"),
        Type = this.GetSchemaType(segment.Property.Type),
        Format = this.GetSchemaFormat(segment.Property.Type)
      };
    }

    private ODataJsonSchemaType GetSchemaType(IEdmTypeReference type)
    {
      switch (type.PrimitiveKind())
      {
        case EdmPrimitiveTypeKind.Binary:
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Guid:
        case EdmPrimitiveTypeKind.String:
        case EdmPrimitiveTypeKind.Duration:
        case EdmPrimitiveTypeKind.Date:
        case EdmPrimitiveTypeKind.TimeOfDay:
          return ODataJsonSchemaType.String;
        case EdmPrimitiveTypeKind.Boolean:
          return ODataJsonSchemaType.Boolean;
        case EdmPrimitiveTypeKind.Byte:
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
          return ODataJsonSchemaType.Integer;
        case EdmPrimitiveTypeKind.Decimal:
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Single:
          return ODataJsonSchemaType.Number;
        default:
          return ODataJsonSchemaType.Undefined;
      }
    }

    private ODataJsonSchemaFormat GetSchemaFormat(IEdmTypeReference type)
    {
      switch (type.PrimitiveKind())
      {
        case EdmPrimitiveTypeKind.Binary:
          return ODataJsonSchemaFormat.Base64Url;
        case EdmPrimitiveTypeKind.Byte:
          return ODataJsonSchemaFormat.UInt8;
        case EdmPrimitiveTypeKind.DateTimeOffset:
          return ODataJsonSchemaFormat.DateTime;
        case EdmPrimitiveTypeKind.Decimal:
          return ODataJsonSchemaFormat.Decimal;
        case EdmPrimitiveTypeKind.Double:
          return ODataJsonSchemaFormat.Double;
        case EdmPrimitiveTypeKind.Guid:
          return ODataJsonSchemaFormat.Uuid;
        case EdmPrimitiveTypeKind.Int16:
          return ODataJsonSchemaFormat.Int16;
        case EdmPrimitiveTypeKind.Int32:
          return ODataJsonSchemaFormat.Int32;
        case EdmPrimitiveTypeKind.Int64:
          return ODataJsonSchemaFormat.Int64;
        case EdmPrimitiveTypeKind.SByte:
          return ODataJsonSchemaFormat.Int8;
        case EdmPrimitiveTypeKind.Single:
          return ODataJsonSchemaFormat.Single;
        case EdmPrimitiveTypeKind.Duration:
          return ODataJsonSchemaFormat.Duration;
        case EdmPrimitiveTypeKind.Date:
          return ODataJsonSchemaFormat.Date;
        case EdmPrimitiveTypeKind.TimeOfDay:
          return ODataJsonSchemaFormat.Time;
        default:
          return ODataJsonSchemaFormat.Undefined;
      }
    }

    private void ThrowIfNotPrimitive(PropertySegment segment)
    {
      if (!segment.Property.Type.IsPrimitive())
        throw new ViewSchemaTranslateNonPrimitivePropertyException(segment.Property.Name);
    }

    private string GetPropertyName(IEdmProperty property, string term)
    {
      string propertyName = this.model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>((IEdmVocabularyAnnotatable) property, term).FirstOrDefault<IEdmVocabularyAnnotation>()?.Value is IEdmStringConstantExpression constantExpression ? constantExpression.Value : (string) null;
      if (term.Equals("Display.DisplayName"))
        propertyName = propertyName ?? property.Name;
      return propertyName;
    }
  }
}
