// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightServiceDocumentDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightServiceDocumentDeserializer : ODataJsonLightDeserializer
  {
    internal ODataJsonLightServiceDocumentDeserializer(
      ODataJsonLightInputContext jsonLightInputContext)
      : base(jsonLightInputContext)
    {
    }

    internal ODataServiceDocument ReadServiceDocument()
    {
      PropertyAndAnnotationCollector annotationCollector = this.CreatePropertyAndAnnotationCollector();
      this.ReadPayloadStart(ODataPayloadKind.ServiceDocument, annotationCollector, false, false);
      ODataServiceDocument odataServiceDocument = this.ReadServiceDocumentImplementation(annotationCollector);
      this.ReadPayloadEnd(false);
      return odataServiceDocument;
    }

    internal Task<ODataServiceDocument> ReadServiceDocumentAsync()
    {
      PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
      return this.ReadPayloadStartAsync(ODataPayloadKind.ServiceDocument, propertyAndAnnotationCollector, false, false).FollowOnSuccessWith<ODataServiceDocument>((Func<Task, ODataServiceDocument>) (t =>
      {
        ODataServiceDocument odataServiceDocument = this.ReadServiceDocumentImplementation(propertyAndAnnotationCollector);
        this.ReadPayloadEnd(false);
        return odataServiceDocument;
      }));
    }

    private ODataServiceDocument ReadServiceDocumentImplementation(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      List<ODataServiceDocumentElement>[] serviceDocumentElements = new List<ODataServiceDocumentElement>[1];
      while (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        Func<string, object> readPropertyAnnotationValue = (Func<string, object>) (annotationName =>
        {
          throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument((object) annotationName, (object) "value"));
        });
        this.ProcessProperty(propertyAndAnnotationCollector, readPropertyAnnotationValue, (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
              if (string.CompareOrdinal("value", propertyName) != 0)
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument((object) propertyName, (object) "value"));
              if (serviceDocumentElements[0] != null)
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument((object) "value"));
              serviceDocumentElements[0] = new List<ODataServiceDocumentElement>();
              this.JsonReader.ReadStartArray();
              PropertyAndAnnotationCollector annotationCollector = this.CreatePropertyAndAnnotationCollector();
              while (this.JsonReader.NodeType != JsonNodeType.EndArray)
              {
                ODataServiceDocumentElement serviceDocumentElement = this.ReadServiceDocumentElement(annotationCollector);
                if (serviceDocumentElement != null)
                  serviceDocumentElements[0].Add(serviceDocumentElement);
                annotationCollector.Reset();
              }
              this.JsonReader.ReadEndArray();
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument((object) propertyName, (object) "value"));
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              this.JsonReader.SkipValue();
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
          }
        }));
      }
      if (serviceDocumentElements[0] == null)
        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument((object) "value"));
      this.JsonReader.ReadEndObject();
      return new ODataServiceDocument()
      {
        EntitySets = (IEnumerable<ODataEntitySetInfo>) new ReadOnlyEnumerable<ODataEntitySetInfo>((IList<ODataEntitySetInfo>) serviceDocumentElements[0].OfType<ODataEntitySetInfo>().ToList<ODataEntitySetInfo>()),
        FunctionImports = (IEnumerable<ODataFunctionImportInfo>) new ReadOnlyEnumerable<ODataFunctionImportInfo>((IList<ODataFunctionImportInfo>) serviceDocumentElements[0].OfType<ODataFunctionImportInfo>().ToList<ODataFunctionImportInfo>()),
        Singletons = (IEnumerable<ODataSingletonInfo>) new ReadOnlyEnumerable<ODataSingletonInfo>((IList<ODataSingletonInfo>) serviceDocumentElements[0].OfType<ODataSingletonInfo>().ToList<ODataSingletonInfo>())
      };
    }

    private ODataServiceDocumentElement ReadServiceDocumentElement(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      this.JsonReader.ReadStartObject();
      string[] name = new string[1];
      string[] url = new string[1];
      string[] kind = new string[1];
      string[] title = new string[1];
      while (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        Func<string, object> readPropertyAnnotationValue = (Func<string, object>) (annotationName =>
        {
          throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement((object) annotationName));
        });
        this.ProcessProperty(propertyAndAnnotationCollector, readPropertyAnnotationValue, (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
              if (string.CompareOrdinal("name", propertyName) == 0)
              {
                if (name[0] != null)
                  throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement((object) "name"));
                name[0] = this.JsonReader.ReadStringValue();
                break;
              }
              if (string.CompareOrdinal("url", propertyName) == 0)
              {
                if (url[0] != null)
                  throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement((object) "url"));
                url[0] = this.JsonReader.ReadStringValue();
                ValidationUtils.ValidateServiceDocumentElementUrl(url[0]);
                break;
              }
              if (string.CompareOrdinal("kind", propertyName) == 0)
              {
                if (kind[0] != null)
                  throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement((object) "kind"));
                kind[0] = this.JsonReader.ReadStringValue();
                break;
              }
              if (string.CompareOrdinal("title", propertyName) != 0)
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement((object) propertyName, (object) "name", (object) "url"));
              if (title[0] != null)
                throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement((object) "title"));
              title[0] = this.JsonReader.ReadStringValue();
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              this.JsonReader.SkipValue();
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
          }
        }));
      }
      if (string.IsNullOrEmpty(name[0]))
        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement((object) "name"));
      if (string.IsNullOrEmpty(url[0]))
        throw new ODataException(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement((object) "url"));
      ODataServiceDocumentElement serviceDocumentElement = (ODataServiceDocumentElement) null;
      if (kind[0] != null)
      {
        if (kind[0].Equals("EntitySet", StringComparison.Ordinal))
          serviceDocumentElement = (ODataServiceDocumentElement) new ODataEntitySetInfo();
        else if (kind[0].Equals("FunctionImport", StringComparison.Ordinal))
          serviceDocumentElement = (ODataServiceDocumentElement) new ODataFunctionImportInfo();
        else if (kind[0].Equals("Singleton", StringComparison.Ordinal))
          serviceDocumentElement = (ODataServiceDocumentElement) new ODataSingletonInfo();
      }
      else
        serviceDocumentElement = (ODataServiceDocumentElement) new ODataEntitySetInfo();
      if (serviceDocumentElement != null)
      {
        serviceDocumentElement.Url = this.ProcessUriFromPayload(url[0]);
        serviceDocumentElement.Name = name[0];
        serviceDocumentElement.Title = title[0];
      }
      this.JsonReader.ReadEndObject();
      return serviceDocumentElement;
    }
  }
}
