// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightEntityReferenceLinkDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightEntityReferenceLinkDeserializer : 
    ODataJsonLightPropertyAndValueDeserializer
  {
    internal ODataJsonLightEntityReferenceLinkDeserializer(
      ODataJsonLightInputContext jsonLightInputContext)
      : base(jsonLightInputContext)
    {
    }

    internal ODataEntityReferenceLinks ReadEntityReferenceLinks()
    {
      PropertyAndAnnotationCollector annotationCollector = this.CreatePropertyAndAnnotationCollector();
      this.ReadPayloadStart(ODataPayloadKind.EntityReferenceLinks, annotationCollector, false, false);
      ODataEntityReferenceLinks entityReferenceLinks = this.ReadEntityReferenceLinksImplementation(annotationCollector);
      this.ReadPayloadEnd(false);
      return entityReferenceLinks;
    }

    internal Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
    {
      PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
      return this.ReadPayloadStartAsync(ODataPayloadKind.EntityReferenceLinks, propertyAndAnnotationCollector, false, false).FollowOnSuccessWith<ODataEntityReferenceLinks>((Func<Task, ODataEntityReferenceLinks>) (t =>
      {
        ODataEntityReferenceLinks entityReferenceLinks = this.ReadEntityReferenceLinksImplementation(propertyAndAnnotationCollector);
        this.ReadPayloadEnd(false);
        return entityReferenceLinks;
      }));
    }

    internal ODataEntityReferenceLink ReadEntityReferenceLink()
    {
      PropertyAndAnnotationCollector annotationCollector = this.CreatePropertyAndAnnotationCollector();
      this.ReadPayloadStart(ODataPayloadKind.EntityReferenceLink, annotationCollector, false, false);
      ODataEntityReferenceLink entityReferenceLink = this.ReadEntityReferenceLinkImplementation(annotationCollector);
      this.ReadPayloadEnd(false);
      return entityReferenceLink;
    }

    internal Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
    {
      PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
      return this.ReadPayloadStartAsync(ODataPayloadKind.EntityReferenceLink, propertyAndAnnotationCollector, false, false).FollowOnSuccessWith<ODataEntityReferenceLink>((Func<Task, ODataEntityReferenceLink>) (t =>
      {
        ODataEntityReferenceLink entityReferenceLink = this.ReadEntityReferenceLinkImplementation(propertyAndAnnotationCollector);
        this.ReadPayloadEnd(false);
        return entityReferenceLink;
      }));
    }

    private ODataEntityReferenceLinks ReadEntityReferenceLinksImplementation(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      ODataEntityReferenceLinks links = new ODataEntityReferenceLinks();
      this.ReadEntityReferenceLinksAnnotations(links, propertyAndAnnotationCollector, true);
      this.JsonReader.ReadStartArray();
      List<ODataEntityReferenceLink> sourceList = new List<ODataEntityReferenceLink>();
      PropertyAndAnnotationCollector annotationCollector = this.JsonLightInputContext.CreatePropertyAndAnnotationCollector();
      while (this.JsonReader.NodeType != JsonNodeType.EndArray)
      {
        ODataEntityReferenceLink entityReferenceLink = this.ReadSingleEntityReferenceLink(annotationCollector, false);
        sourceList.Add(entityReferenceLink);
        annotationCollector.Reset();
      }
      this.JsonReader.ReadEndArray();
      this.ReadEntityReferenceLinksAnnotations(links, propertyAndAnnotationCollector, false);
      this.JsonReader.ReadEndObject();
      links.Links = (IEnumerable<ODataEntityReferenceLink>) new ReadOnlyEnumerable<ODataEntityReferenceLink>((IList<ODataEntityReferenceLink>) sourceList);
      return links;
    }

    private ODataEntityReferenceLink ReadEntityReferenceLinkImplementation(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      return this.ReadSingleEntityReferenceLink(propertyAndAnnotationCollector, true);
    }

    private void ReadEntityReferenceLinksAnnotations(
      ODataEntityReferenceLinks links,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool forLinksStart)
    {
      while (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        Func<string, object> readPropertyAnnotationValue = (Func<string, object>) (annotationName =>
        {
          throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks);
        });
        bool foundValueProperty = false;
        this.ProcessProperty(propertyAndAnnotationCollector, readPropertyAnnotationValue, (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParseResult, propertyName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParseResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject:
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
              if (string.CompareOrdinal("value", propertyName) != 0)
                throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound((object) propertyName, (object) "value"));
              foundValueProperty = true;
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              if (string.CompareOrdinal("odata.nextLink", propertyName) == 0)
              {
                this.ReadEntityReferenceLinksNextLinkAnnotationValue(links);
                break;
              }
              if (string.CompareOrdinal("odata.count", propertyName) != 0)
                throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) propertyName));
              this.ReadEntityReferenceCountAnnotationValue(links);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
              object objectToConvert = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, propertyName);
              links.InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, objectToConvert.ToODataValue()));
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
            default:
              throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightEntityReferenceLinkDeserializer_ReadEntityReferenceLinksAnnotations));
          }
        }));
        if (foundValueProperty)
          return;
      }
      if (forLinksStart)
        throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound((object) "value"));
    }

    private void ReadEntityReferenceLinksNextLinkAnnotationValue(ODataEntityReferenceLinks links)
    {
      Uri uri = this.ReadAndValidateAnnotationStringValueAsUri("odata.nextLink");
      links.NextPageLink = uri;
    }

    private void ReadEntityReferenceCountAnnotationValue(ODataEntityReferenceLinks links) => links.Count = new long?(this.ReadAndValidateAnnotationAsLongForIeee754Compatible("odata.count"));

    private ODataEntityReferenceLink ReadSingleEntityReferenceLink(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool topLevel)
    {
      if (!topLevel)
      {
        if (this.JsonReader.NodeType != JsonNodeType.StartObject)
          throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue((object) this.JsonReader.NodeType));
        this.JsonReader.ReadStartObject();
      }
      ODataEntityReferenceLink[] entityReferenceLink = new ODataEntityReferenceLink[1];
      Func<string, object> readPropertyAnnotationValue = (Func<string, object>) (annotationName =>
      {
        throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink((object) annotationName));
      });
      while (this.JsonReader.NodeType == JsonNodeType.Property)
        this.ProcessProperty(propertyAndAnnotationCollector, readPropertyAnnotationValue, (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject:
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              if (string.CompareOrdinal("odata.id", propertyName) != 0)
                throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink((object) propertyName, (object) "odata.id"));
              if (entityReferenceLink[0] != null)
                throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink((object) "odata.id"));
              entityReferenceLink[0] = new ODataEntityReferenceLink()
              {
                Url = this.ProcessUriFromPayload(this.JsonReader.ReadStringValue("odata.id") ?? throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull((object) "odata.id")))
              };
              ReaderValidationUtils.ValidateEntityReferenceLink(entityReferenceLink[0]);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              if (entityReferenceLink[0] == null)
                throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty((object) "odata.id"));
              ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
              object objectToConvert = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, propertyName);
              entityReferenceLink[0].InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, objectToConvert.ToODataValue()));
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
            default:
              throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightEntityReferenceLinkDeserializer_ReadSingleEntityReferenceLink));
          }
        }));
      if (entityReferenceLink[0] == null)
        throw new ODataException(Strings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty((object) "odata.id"));
      this.JsonReader.ReadEndObject();
      return entityReferenceLink[0];
    }
  }
}
