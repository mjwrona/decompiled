// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightResourceDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.JsonLight
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to keep the logic together for better readability.")]
  internal sealed class ODataJsonLightResourceDeserializer : 
    ODataJsonLightPropertyAndValueDeserializer
  {
    internal ODataJsonLightResourceDeserializer(ODataJsonLightInputContext jsonLightInputContext)
      : base(jsonLightInputContext)
    {
    }

    internal void ReadResourceSetContentStart()
    {
      if (this.JsonReader.NodeType != JsonNodeType.StartArray)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_CannotReadResourceSetContentStart((object) this.JsonReader.NodeType));
      this.JsonReader.ReadStartArray();
    }

    internal void ReadResourceSetContentEnd() => this.JsonReader.ReadEndArray();

    internal void ReadResourceTypeName(IODataJsonLightReaderResourceState resourceState)
    {
      if (this.JsonReader.NodeType != JsonNodeType.Property)
        return;
      string propertyName = this.JsonReader.GetPropertyName();
      if (string.CompareOrdinal("@odata.type", propertyName) != 0 && !this.CompareSimplifiedODataAnnotation("@type", propertyName))
        return;
      this.JsonReader.Read();
      resourceState.Resource.TypeName = this.ReadODataTypeAnnotationValue();
    }

    internal ODataDeletedResource IsDeletedResource()
    {
      ODataDeletedResource odataDeletedResource = (ODataDeletedResource) null;
      if (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        string propertyName = this.JsonReader.GetPropertyName();
        if (string.CompareOrdinal("@odata.removed", propertyName) == 0 || this.CompareSimplifiedODataAnnotation("@removed", propertyName))
        {
          DeltaDeletedEntryReason reason = DeltaDeletedEntryReason.Changed;
          Uri id = (Uri) null;
          this.JsonReader.Read();
          if (this.JsonReader.NodeType != JsonNodeType.PrimitiveValue)
          {
            while (this.JsonReader.NodeType != JsonNodeType.EndObject && this.JsonReader.Read())
            {
              if (this.JsonReader.NodeType == JsonNodeType.Property && string.CompareOrdinal("reason", this.JsonReader.GetPropertyName()) == 0)
              {
                this.JsonReader.Read();
                if (string.CompareOrdinal("deleted", this.JsonReader.ReadStringValue()) == 0)
                  reason = DeltaDeletedEntryReason.Deleted;
              }
            }
          }
          else if (this.JsonReader.Value != null)
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_DeltaRemovedAnnotationMustBeObject(this.JsonReader.Value));
          this.JsonReader.Read();
          string str = this.JsonReader.NodeType == JsonNodeType.Property ? this.JsonReader.GetPropertyName() : throw new ODataException(Microsoft.OData.Strings.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
          if (string.CompareOrdinal("@odata.id", str) == 0 || this.CompareSimplifiedODataAnnotation("@id", str))
          {
            this.JsonReader.Read();
            id = UriUtils.StringToUri(this.JsonReader.ReadStringValue());
          }
          odataDeletedResource = ReaderUtils.CreateDeletedResource(id, reason);
        }
      }
      return odataDeletedResource;
    }

    internal ODataDeletedResource ReadDeletedEntry()
    {
      Uri id = (Uri) null;
      DeltaDeletedEntryReason reason = DeltaDeletedEntryReason.Changed;
      if (this.JsonReader.NodeType == JsonNodeType.Property && string.CompareOrdinal("id", this.JsonReader.GetPropertyName()) == 0)
      {
        this.JsonReader.Read();
        id = this.JsonReader.ReadUriValue();
      }
      if (this.JsonReader.NodeType == JsonNodeType.Property && string.CompareOrdinal("reason", this.JsonReader.GetPropertyName()) == 0)
      {
        this.JsonReader.Read();
        if (string.CompareOrdinal("deleted", this.JsonReader.ReadStringValue()) == 0)
          reason = DeltaDeletedEntryReason.Deleted;
      }
      while (this.JsonReader.NodeType != JsonNodeType.EndObject && this.JsonReader.Read())
      {
        if (this.JsonReader.NodeType == JsonNodeType.StartObject || this.JsonReader.NodeType == JsonNodeType.StartArray)
          throw new ODataException(Microsoft.OData.Strings.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
      }
      return ReaderUtils.CreateDeletedResource(id, reason);
    }

    internal void ReadDeltaLinkSource(ODataDeltaLinkBase link)
    {
      if (this.JsonReader.NodeType != JsonNodeType.Property || string.CompareOrdinal("source", this.JsonReader.GetPropertyName()) != 0)
        return;
      this.JsonReader.Read();
      link.Source = this.JsonReader.ReadUriValue();
    }

    internal void ReadDeltaLinkRelationship(ODataDeltaLinkBase link)
    {
      if (this.JsonReader.NodeType != JsonNodeType.Property || string.CompareOrdinal("relationship", this.JsonReader.GetPropertyName()) != 0)
        return;
      this.JsonReader.Read();
      link.Relationship = this.JsonReader.ReadStringValue();
    }

    internal void ReadDeltaLinkTarget(ODataDeltaLinkBase link)
    {
      if (this.JsonReader.NodeType != JsonNodeType.Property || string.CompareOrdinal("target", this.JsonReader.GetPropertyName()) != 0)
        return;
      this.JsonReader.Read();
      link.Target = this.JsonReader.ReadUriValue();
    }

    internal ODataJsonLightReaderNestedInfo ReadResourceContent(
      IODataJsonLightReaderResourceState resourceState)
    {
      ODataJsonLightReaderNestedInfo readerNestedResourceInfo = (ODataJsonLightReaderNestedInfo) null;
      while (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        this.ReadPropertyCustomAnnotationValue = new Func<PropertyAndAnnotationCollector, string, object>(((ODataJsonLightPropertyAndValueDeserializer) this).ReadCustomInstanceAnnotationValue);
        this.ProcessProperty(resourceState.PropertyAndAnnotationCollector, new Func<string, object>(this.ReadEntryPropertyAnnotationValue), (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
        {
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject:
              this.ReadOverPropertyName();
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
              resourceState.AnyPropertyFound = true;
              readerNestedResourceInfo = this.ReadPropertyWithValue(resourceState, propertyName, false);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              resourceState.AnyPropertyFound = true;
              readerNestedResourceInfo = this.ReadPropertyWithoutValue(resourceState, propertyName);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              this.ReadOverPropertyName();
              object annotationValue = this.ReadODataOrCustomInstanceAnnotationValue(resourceState, propertyParsingResult, propertyName);
              this.ApplyEntryInstanceAnnotation(resourceState, propertyName, annotationValue);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              this.ReadOverPropertyName();
              this.ReadMetadataReferencePropertyValue(resourceState, propertyName);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.NestedDeltaResourceSet:
              resourceState.AnyPropertyFound = true;
              readerNestedResourceInfo = this.ReadPropertyWithValue(resourceState, propertyName, true);
              break;
          }
        }));
        if (readerNestedResourceInfo != null)
          break;
      }
      return readerNestedResourceInfo;
    }

    internal object ReadODataOrCustomInstanceAnnotationValue(
      IODataJsonLightReaderResourceState resourceState,
      ODataJsonLightDeserializer.PropertyParsingResult propertyParsingResult,
      string annotationName)
    {
      object annotationValue = this.ReadEntryInstanceAnnotation(annotationName, resourceState.AnyPropertyFound, true, resourceState.PropertyAndAnnotationCollector);
      if (propertyParsingResult == ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation)
        resourceState.PropertyAndAnnotationCollector.AddODataScopeAnnotation(annotationName, annotationValue);
      else
        resourceState.PropertyAndAnnotationCollector.AddCustomScopeAnnotation(annotationName, annotationValue);
      return annotationValue;
    }

    internal void ValidateMediaEntity(IODataJsonLightReaderResourceState resourceState)
    {
      ODataResourceBase resource = resourceState.Resource;
      if (resource == null || !(resourceState.ResourceType is IEdmEntityType resourceType))
        return;
      if (!this.ReadingResponse && resourceType.HasStream && resource.MediaResource == null)
      {
        ODataStreamReferenceValue mediaResource = resource.MediaResource;
        ODataJsonLightReaderUtils.EnsureInstance<ODataStreamReferenceValue>(ref mediaResource);
        this.SetEntryMediaResource(resourceState, mediaResource);
      }
      this.ReaderValidator.ValidateMediaResource(resource, resourceType);
    }

    internal void ReadTopLevelResourceSetAnnotations(
      ODataResourceSetBase resourceSet,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool forResourceSetStart,
      bool readAllResourceSetProperties)
    {
      bool buffering = false;
      try
      {
        while (this.JsonReader.NodeType == JsonNodeType.Property)
        {
          bool foundValueProperty = false;
          if (!forResourceSetStart & readAllResourceSetProperties)
            propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(false);
          this.ProcessProperty(propertyAndAnnotationCollector, new Func<string, object>(((ODataJsonLightPropertyAndValueDeserializer) this).ReadTypePropertyAnnotationValue), (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParseResult, propertyName) =>
          {
            this.ReadOverPropertyName();
            switch (propertyParseResult)
            {
              case ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject:
                break;
              case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
                if (string.CompareOrdinal("value", propertyName) != 0)
                  throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_InvalidPropertyInTopLevelResourceSet((object) propertyName, (object) "value"));
                if (readAllResourceSetProperties)
                {
                  this.JsonReader.StartBuffering();
                  buffering = true;
                  this.JsonReader.SkipValue();
                  break;
                }
                foundValueProperty = true;
                break;
              case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet((object) propertyName));
              case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
                this.ReadODataOrCustomInstanceAnnotationValue(resourceSet, propertyAndAnnotationCollector, forResourceSetStart, readAllResourceSetProperties, propertyParseResult, propertyName);
                break;
              case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
                if (!(resourceSet is ODataResourceSet))
                  throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
                this.ReadMetadataReferencePropertyValue((ODataResourceSet) resourceSet, propertyName);
                break;
              default:
                throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightResourceDeserializer_ReadTopLevelResourceSetAnnotations));
            }
          }));
          if (foundValueProperty)
            return;
        }
      }
      finally
      {
        if (buffering)
          this.JsonReader.StopBuffering();
      }
      if (forResourceSetStart && !readAllResourceSetProperties)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_ExpectedResourceSetPropertyNotFound((object) "value"));
    }

    internal void ReadODataOrCustomInstanceAnnotationValue(
      ODataResourceSetBase resourceSet,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool forResourceSetStart,
      bool readAllResourceSetProperties,
      ODataJsonLightDeserializer.PropertyParsingResult propertyParseResult,
      string annotationName)
    {
      if (propertyParseResult == ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation)
        propertyAndAnnotationCollector.AddODataScopeAnnotation(annotationName, this.JsonReader.Value);
      if (forResourceSetStart || !readAllResourceSetProperties)
        this.ReadAndApplyResourceSetInstanceAnnotationValue(annotationName, resourceSet, propertyAndAnnotationCollector);
      else
        this.JsonReader.SkipValue();
    }

    internal object ReadEntryPropertyAnnotationValue(string propertyAnnotationName)
    {
      string str;
      if (this.TryReadODataTypeAnnotationValue(propertyAnnotationName, out str))
        return (object) str;
      switch (propertyAnnotationName)
      {
        case "odata.associationLink":
        case "odata.context":
        case "odata.mediaEditLink":
        case "odata.mediaReadLink":
        case "odata.navigationLink":
        case "odata.nextLink":
          return (object) this.ReadAndValidateAnnotationStringValueAsUri(propertyAnnotationName);
        case "odata.bind":
          if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            return (object) new ODataEntityReferenceLink()
            {
              Url = this.ReadAndValidateAnnotationStringValueAsUri("odata.bind")
            };
          LinkedList<ODataEntityReferenceLink> linkedList = new LinkedList<ODataEntityReferenceLink>();
          this.JsonReader.Read();
          while (this.JsonReader.NodeType != JsonNodeType.EndArray)
            linkedList.AddLast(new ODataEntityReferenceLink()
            {
              Url = this.ReadAndValidateAnnotationStringValueAsUri("odata.bind")
            });
          this.JsonReader.Read();
          return linkedList.Count != 0 ? (object) linkedList : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_EmptyBindArray((object) "odata.bind"));
        case "odata.count":
          return (object) this.ReadAndValidateAnnotationAsLongForIeee754Compatible(propertyAnnotationName);
        case "odata.mediaContentType":
        case "odata.mediaEtag":
          return (object) this.ReadAndValidateAnnotationStringValue(propertyAnnotationName);
        default:
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) propertyAnnotationName));
      }
    }

    internal object ReadEntryInstanceAnnotation(
      string annotationName,
      bool anyPropertyFound,
      bool typeAnnotationFound,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      switch (annotationName)
      {
        case "odata.editLink":
        case "odata.mediaEditLink":
        case "odata.mediaReadLink":
        case "odata.readLink":
          return (object) this.ReadAndValidateAnnotationStringValueAsUri(annotationName);
        case "odata.etag":
          if (anyPropertyFound)
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty((object) annotationName));
          return (object) this.ReadAndValidateAnnotationStringValue(annotationName);
        case "odata.id":
          if (anyPropertyFound)
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty((object) annotationName));
          return (object) this.ReadAnnotationStringValueAsUri(annotationName);
        case "odata.mediaContentType":
        case "odata.mediaEtag":
          return (object) this.ReadAndValidateAnnotationStringValue(annotationName);
        case "odata.removed":
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedDeletedEntryInResponsePayload);
        case "odata.type":
          if (!typeAnnotationFound)
            return (object) this.ReadODataTypeAnnotationValue();
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_ResourceTypeAnnotationNotFirst);
        default:
          ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
          return this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, annotationName);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "The casts aren't actually being done multiple times, since they occur in different cases of the switch statement.")]
    internal void ApplyEntryInstanceAnnotation(
      IODataJsonLightReaderResourceState resourceState,
      string annotationName,
      object annotationValue)
    {
      ODataResourceBase resource = resourceState.Resource;
      ODataStreamReferenceValue mediaResource = resource.MediaResource;
      switch (annotationName)
      {
        case "odata.editLink":
          resource.EditLink = (Uri) annotationValue;
          break;
        case "odata.etag":
          resource.ETag = (string) annotationValue;
          break;
        case "odata.id":
          if (annotationValue == null)
          {
            resource.IsTransient = true;
            break;
          }
          resource.Id = (Uri) annotationValue;
          break;
        case "odata.mediaContentType":
          ODataJsonLightReaderUtils.EnsureInstance<ODataStreamReferenceValue>(ref mediaResource);
          mediaResource.ContentType = (string) annotationValue;
          break;
        case "odata.mediaEditLink":
          ODataJsonLightReaderUtils.EnsureInstance<ODataStreamReferenceValue>(ref mediaResource);
          mediaResource.EditLink = (Uri) annotationValue;
          break;
        case "odata.mediaEtag":
          ODataJsonLightReaderUtils.EnsureInstance<ODataStreamReferenceValue>(ref mediaResource);
          mediaResource.ETag = (string) annotationValue;
          break;
        case "odata.mediaReadLink":
          ODataJsonLightReaderUtils.EnsureInstance<ODataStreamReferenceValue>(ref mediaResource);
          mediaResource.ReadLink = (Uri) annotationValue;
          break;
        case "odata.readLink":
          resource.ReadLink = (Uri) annotationValue;
          break;
        case "odata.type":
          resource.TypeName = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string) annotationValue));
          break;
        default:
          ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
          resource.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, annotationValue.ToODataValue()));
          break;
      }
      if (mediaResource == null || resource.MediaResource != null)
        return;
      this.SetEntryMediaResource(resourceState, mediaResource);
    }

    internal void ReadAndApplyResourceSetInstanceAnnotationValue(
      string annotationName,
      ODataResourceSetBase resourceSet,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      switch (annotationName)
      {
        case "odata.count":
          resourceSet.Count = new long?(this.ReadAndValidateAnnotationAsLongForIeee754Compatible("odata.count"));
          break;
        case "odata.nextLink":
          resourceSet.NextPageLink = this.ReadAndValidateAnnotationStringValueAsUri("odata.nextLink");
          break;
        case "odata.deltaLink":
          resourceSet.DeltaLink = this.ReadAndValidateAnnotationStringValueAsUri("odata.deltaLink");
          break;
        case "odata.type":
          this.ReadAndValidateAnnotationStringValue("odata.type");
          break;
        default:
          ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
          object objectToConvert = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, annotationName);
          resourceSet.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, objectToConvert.ToODataValue()));
          break;
      }
    }

    internal ODataJsonLightReaderNestedInfo ReadPropertyWithoutValue(
      IODataJsonLightReaderResourceState resourceState,
      string propertyName)
    {
      ODataJsonLightReaderNestedInfo readerNestedInfo = (ODataJsonLightReaderNestedInfo) null;
      IEdmStructuredType resourceType = resourceState.ResourceType;
      IEdmProperty edmProperty = this.ReaderValidator.ValidatePropertyDefined(propertyName, resourceType);
      if (edmProperty != null && !edmProperty.Type.IsUntyped())
      {
        if (edmProperty is IEdmNavigationProperty navigationProperty)
        {
          ODataJsonLightReaderNestedResourceInfo nestedResourceInfo;
          if (this.ReadingResponse)
          {
            nestedResourceInfo = ODataJsonLightResourceDeserializer.ReadDeferredNestedResourceInfo(resourceState, propertyName, navigationProperty);
          }
          else
          {
            nestedResourceInfo = navigationProperty.Type.IsCollection() ? ODataJsonLightPropertyAndValueDeserializer.ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, propertyName, false) : ODataJsonLightPropertyAndValueDeserializer.ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, propertyName, false);
            if (!nestedResourceInfo.HasEntityReferenceLink)
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink((object) propertyName, (object) "odata.bind"));
          }
          resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(nestedResourceInfo.NestedResourceInfo);
          readerNestedInfo = (ODataJsonLightReaderNestedInfo) nestedResourceInfo;
        }
        else
        {
          IEdmTypeReference type = edmProperty.Type;
          if (!type.IsStream())
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_PropertyWithoutValueWithWrongType((object) propertyName, (object) type.FullName()));
          ODataStreamReferenceValue propertyValue = this.ReadStreamPropertyValue(resourceState, propertyName);
          ODataJsonLightPropertyAndValueDeserializer.AddResourceProperty(resourceState, edmProperty.Name, (object) propertyValue);
        }
      }
      else
        readerNestedInfo = this.ReadUndeclaredProperty(resourceState, propertyName, false);
      return readerNestedInfo;
    }

    internal void ReadNextLinkAnnotationAtResourceSetEnd(
      ODataResourceSetBase resourceSet,
      ODataJsonLightReaderNestedResourceInfo expandedNestedResourceInfo,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      if (expandedNestedResourceInfo != null)
      {
        this.ReadExpandedResourceSetAnnotationsAtResourceSetEnd(resourceSet, expandedNestedResourceInfo);
      }
      else
      {
        bool readAllResourceSetProperties = this.JsonReader is ReorderingJsonReader;
        this.ReadTopLevelResourceSetAnnotations(resourceSet, propertyAndAnnotationCollector, false, readAllResourceSetProperties);
      }
    }

    private static ODataJsonLightReaderNestedResourceInfo ReadDeferredNestedResourceInfo(
      IODataJsonLightReaderResourceState resourceState,
      string navigationPropertyName,
      IEdmNavigationProperty navigationProperty)
    {
      ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
      {
        Name = navigationPropertyName,
        IsCollection = navigationProperty == null ? new bool?() : new bool?(navigationProperty.Type.IsCollection())
      };
      foreach (KeyValuePair<string, object> propertyAnnotation in (IEnumerable<KeyValuePair<string, object>>) resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
      {
        switch (propertyAnnotation.Key)
        {
          case "odata.navigationLink":
            nestedResourceInfo.Url = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.associationLink":
            nestedResourceInfo.AssociationLinkUrl = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.type":
            nestedResourceInfo.TypeAnnotation = new ODataTypeAnnotation((string) propertyAnnotation.Value);
            continue;
          default:
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedDeferredLinkPropertyAnnotation((object) nestedResourceInfo.Name, (object) propertyAnnotation.Key));
        }
      }
      return ODataJsonLightReaderNestedResourceInfo.CreateDeferredLinkInfo(nestedResourceInfo, navigationProperty);
    }

    private void ReadExpandedResourceSetAnnotationsAtResourceSetEnd(
      ODataResourceSetBase resourceSet,
      ODataJsonLightReaderNestedResourceInfo expandedNestedResourceInfo)
    {
      string propertyName;
      string annotationName;
      while (this.JsonReader.NodeType == JsonNodeType.Property && ODataJsonLightDeserializer.TryParsePropertyAnnotation(this.JsonReader.GetPropertyName(), out propertyName, out annotationName) && string.CompareOrdinal(propertyName, expandedNestedResourceInfo.NestedResourceInfo.Name) == 0)
      {
        if (!this.ReadingResponse)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation((object) propertyName, (object) annotationName));
        this.JsonReader.Read();
        switch (this.CompleteSimplifiedODataAnnotation(annotationName))
        {
          case "odata.nextLink":
            resourceSet.NextPageLink = !(resourceSet.NextPageLink != (Uri) null) ? this.ReadAndValidateAnnotationStringValueAsUri("odata.nextLink") : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_DuplicateNestedResourceSetAnnotation((object) "odata.nextLink", (object) expandedNestedResourceInfo.NestedResourceInfo.Name));
            continue;
          case "odata.count":
            if (resourceSet.Count.HasValue)
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_DuplicateNestedResourceSetAnnotation((object) "odata.count", (object) expandedNestedResourceInfo.NestedResourceInfo.Name));
            resourceSet.Count = new long?(this.ReadAndValidateAnnotationAsLongForIeee754Compatible("odata.count"));
            continue;
          default:
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet((object) annotationName, (object) expandedNestedResourceInfo.NestedResourceInfo.Name));
        }
      }
    }

    private void SetEntryMediaResource(
      IODataJsonLightReaderResourceState resourceState,
      ODataStreamReferenceValue mediaResource)
    {
      ODataResourceBase resource = resourceState.Resource;
      ODataResourceMetadataBuilder builderForReader = this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState, this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment, false);
      mediaResource.SetMetadataBuilder(builderForReader, (string) null);
      resource.MediaResource = mediaResource;
    }

    private ODataJsonLightReaderNestedInfo ReadPropertyWithValue(
      IODataJsonLightReaderResourceState resourceState,
      string propertyName,
      bool isDeltaResourceSet)
    {
      ODataJsonLightReaderNestedInfo readerNestedInfo1 = (ODataJsonLightReaderNestedInfo) null;
      IEdmStructuredType resourceType = resourceState.ResourceType;
      IEdmProperty edmProperty = this.ReaderValidator.ValidatePropertyDefined(propertyName, resourceType);
      bool flag = edmProperty != null && edmProperty.Type.IsCollection();
      if (edmProperty is IEdmStructuralProperty structuralProperty)
      {
        ODataJsonLightReaderNestedInfo readerNestedInfo2 = this.TryReadAsStream(resourceState, structuralProperty, structuralProperty.Type, structuralProperty.Name);
        if (readerNestedInfo2 != null)
          return readerNestedInfo2;
      }
      if (edmProperty != null && !edmProperty.Type.IsUntyped())
      {
        this.ReadOverPropertyName();
        IEdmStructuredType structuredType = structuralProperty == null ? (IEdmStructuredType) null : structuralProperty.Type.ToStructuredType();
        IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
        if (structuredType != null)
        {
          ODataJsonLightPropertyAndValueDeserializer.ValidateExpandedNestedResourceInfoPropertyValue((IJsonReader) this.JsonReader, new bool?(flag), propertyName);
          ODataJsonLightReaderNestedResourceInfo nestedResourceInfo = !flag ? ODataJsonLightPropertyAndValueDeserializer.ReadNonExpandedResourceNestedResourceInfo(resourceState, structuralProperty, structuredType, structuralProperty.Name) : ODataJsonLightPropertyAndValueDeserializer.ReadNonExpandedResourceSetNestedResourceInfo(resourceState, structuralProperty, structuredType, structuralProperty.Name);
          resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(nestedResourceInfo.NestedResourceInfo);
          readerNestedInfo1 = (ODataJsonLightReaderNestedInfo) nestedResourceInfo;
        }
        else if (navigationProperty != null)
        {
          ODataJsonLightPropertyAndValueDeserializer.ValidateExpandedNestedResourceInfoPropertyValue((IJsonReader) this.JsonReader, new bool?(flag), propertyName);
          ODataJsonLightReaderNestedResourceInfo nestedResourceInfo = !flag ? (this.ReadingResponse ? ODataJsonLightPropertyAndValueDeserializer.ReadExpandedResourceNestedResourceInfo(resourceState, navigationProperty, propertyName, navigationProperty.Type.ToStructuredType(), this.MessageReaderSettings) : ODataJsonLightPropertyAndValueDeserializer.ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, propertyName, true)) : (this.ReadingResponse | isDeltaResourceSet ? ODataJsonLightPropertyAndValueDeserializer.ReadExpandedResourceSetNestedResourceInfo(resourceState, navigationProperty, navigationProperty.Type.ToStructuredType(), propertyName, isDeltaResourceSet) : ODataJsonLightPropertyAndValueDeserializer.ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, propertyName, true));
          resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(nestedResourceInfo.NestedResourceInfo);
          readerNestedInfo1 = (ODataJsonLightReaderNestedInfo) nestedResourceInfo;
        }
        else
        {
          IEnumerable<string> derivedTypeConstraints = this.JsonLightInputContext.Model.GetDerivedTypeConstraints((IEdmVocabularyAnnotatable) edmProperty);
          if (derivedTypeConstraints != null)
            resourceState.PropertyAndAnnotationCollector.SetDerivedTypeValidator(propertyName, new DerivedTypeValidator(edmProperty.Type.Definition, derivedTypeConstraints, "property", propertyName));
          this.ReadEntryDataProperty(resourceState, edmProperty, ODataJsonLightPropertyAndValueDeserializer.ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName));
        }
      }
      else
        readerNestedInfo1 = this.ReadUndeclaredProperty(resourceState, propertyName, true);
      return readerNestedInfo1;
    }

    private ODataJsonLightReaderNestedInfo TryReadAsStream(
      IODataJsonLightReaderResourceState resourceState,
      IEdmStructuralProperty property,
      IEdmTypeReference propertyType,
      string propertyName)
    {
      IEdmPrimitiveType type = (IEdmPrimitiveType) null;
      bool flag1;
      if (propertyType != null)
      {
        type = propertyType.Definition.AsElementType() as IEdmPrimitiveType;
        flag1 = propertyType.IsCollection();
      }
      else
        flag1 = this.JsonReader.NodeType != JsonNodeType.PrimitiveValue;
      Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> readAsStreamFunc = this.MessageReaderSettings.ReadAsStreamFunc;
      if ((type == null || !type.IsStream() && (readAsStreamFunc == null || property != null && property.IsKey() || ((type.IsBinary() ? 1 : (type.IsString() ? 1 : 0)) | (flag1 ? 1 : 0)) == 0 || !readAsStreamFunc(type, flag1, propertyName, (IEdmProperty) property))) && (propertyType != null && !propertyType.Definition.AsElementType().IsUntyped() || !flag1 && !this.JsonReader.CanStream() || readAsStreamFunc == null || !readAsStreamFunc((IEdmPrimitiveType) null, flag1, propertyName, (IEdmProperty) property)))
        return (ODataJsonLightReaderNestedInfo) null;
      if (flag1)
      {
        this.ReadOverPropertyName();
        IEdmType elementType = propertyType == null ? (IEdmType) EdmCoreModel.Instance.GetUntypedType() : propertyType.Definition.AsElementType();
        return (ODataJsonLightReaderNestedInfo) ODataJsonLightPropertyAndValueDeserializer.ReadStreamCollectionNestedResourceInfo(resourceState, property, propertyName, elementType);
      }
      ODataPropertyInfo nestedPropertyInfo;
      if (type != null && type.PrimitiveKind == EdmPrimitiveTypeKind.Stream)
      {
        ODataStreamPropertyInfo streamPropertyInfo = this.ReadStreamPropertyInfo(resourceState, propertyName);
        if (this.JsonReader.NodeType == JsonNodeType.Property)
        {
          bool flag2 = false;
          if (streamPropertyInfo.ContentType != null)
          {
            if (streamPropertyInfo.ContentType.Contains("application/json"))
              flag2 = true;
          }
          else if (property != null)
          {
            IEdmVocabularyAnnotation vocabularyAnnotation = property.VocabularyAnnotations(this.Model).FirstOrDefault<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (a => a.Term == CoreVocabularyModel.MediaTypeTerm));
            if (vocabularyAnnotation != null && vocabularyAnnotation.Value is IEdmStringConstantExpression constantExpression && constantExpression.Value.Contains("application/json"))
              flag2 = true;
          }
          if (!flag2)
            this.ReadOverPropertyName();
        }
        ODataStreamReferenceValue propertyValue = this.ReadStreamPropertyValue(resourceState, propertyName);
        ODataJsonLightPropertyAndValueDeserializer.AddResourceProperty(resourceState, propertyName, (object) propertyValue);
        nestedPropertyInfo = (ODataPropertyInfo) streamPropertyInfo;
      }
      else
      {
        this.ReadOverPropertyName();
        nestedPropertyInfo = new ODataPropertyInfo()
        {
          PrimitiveTypeKind = type == null ? EdmPrimitiveTypeKind.None : type.PrimitiveKind,
          Name = propertyName
        };
      }
      return (ODataJsonLightReaderNestedInfo) new ODataJsonLightReaderNestedPropertyInfo(nestedPropertyInfo, (IEdmProperty) property);
    }

    private void ReadOverPropertyName()
    {
      if (this.JsonReader.NodeType != JsonNodeType.Property)
        return;
      this.JsonReader.Read();
    }

    private static bool IsJsonStream(ODataStreamPropertyInfo streamPropertyInfo) => streamPropertyInfo.ContentType != null && streamPropertyInfo.ContentType.Contains("application/json");

    private void ReadEntryDataProperty(
      IODataJsonLightReaderResourceState resourceState,
      IEdmProperty edmProperty,
      string propertyTypeName)
    {
      ODataNullValueBehaviorKind valueBehaviorKind = this.ReadingResponse ? ODataNullValueBehaviorKind.Default : this.Model.NullValueReadBehaviorKind(edmProperty);
      object propertyValue = this.ReadNonEntityValue(propertyTypeName, edmProperty.Type, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, valueBehaviorKind == ODataNullValueBehaviorKind.Default, false, false, edmProperty.Name);
      if (valueBehaviorKind == ODataNullValueBehaviorKind.IgnoreValue && propertyValue == null)
        return;
      ODataJsonLightPropertyAndValueDeserializer.AddResourceProperty(resourceState, edmProperty.Name, propertyValue);
    }

    private ODataJsonLightReaderNestedInfo InnerReadUndeclaredProperty(
      IODataJsonLightReaderResourceState resourceState,
      IEdmStructuredType owningStructuredType,
      string propertyName,
      bool propertyWithValue)
    {
      if (!propertyWithValue)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_OpenPropertyWithoutValue((object) propertyName));
      bool insideResourceValue = false;
      string payloadTypeName1 = ODataJsonLightPropertyAndValueDeserializer.ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName);
      string payloadTypeName2 = this.TryReadOrPeekPayloadType(resourceState.PropertyAndAnnotationCollector, propertyName, insideResourceValue);
      IEdmType edmType = ReaderValidationUtils.ResolvePayloadTypeName(this.Model, (IEdmTypeReference) null, payloadTypeName2, EdmTypeKind.Complex, this.MessageReaderSettings.ClientCustomTypeResolver, out EdmTypeKind _);
      IEdmTypeReference edmTypeReference1 = (IEdmTypeReference) null;
      if (!string.IsNullOrEmpty(payloadTypeName2) && edmType != null)
        edmTypeReference1 = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(EdmTypeKind.None, new bool?(), (IEdmType) null, (IEdmTypeReference) null, payloadTypeName2, this.Model, new Func<EdmTypeKind>(((ODataJsonLightPropertyAndValueDeserializer) this).GetNonEntityValueKind), out EdmTypeKind _, out ODataTypeAnnotation _);
      ODataJsonLightReaderNestedInfo readerNestedInfo = this.TryReadAsStream(resourceState, (IEdmStructuralProperty) null, edmTypeReference1, propertyName);
      if (readerNestedInfo != null)
        return readerNestedInfo;
      IEdmTypeReference edmTypeReference2 = ODataJsonLightPropertyAndValueDeserializer.ResolveUntypedType(this.JsonReader.NodeType, this.JsonReader.Value, payloadTypeName2, edmTypeReference1, this.MessageReaderSettings.PrimitiveTypeResolver, this.MessageReaderSettings.ReadUntypedAsString, !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);
      bool flag = edmTypeReference2.IsCollection();
      IEdmStructuredType structuredType = edmTypeReference2.ToStructuredType();
      if (structuredType != null)
      {
        ODataJsonLightPropertyAndValueDeserializer.ValidateExpandedNestedResourceInfoPropertyValue((IJsonReader) this.JsonReader, new bool?(flag), propertyName);
        return flag ? (ODataJsonLightReaderNestedInfo) ODataJsonLightPropertyAndValueDeserializer.ReadNonExpandedResourceSetNestedResourceInfo(resourceState, (IEdmStructuralProperty) null, structuredType, propertyName) : (ODataJsonLightReaderNestedInfo) ODataJsonLightPropertyAndValueDeserializer.ReadNonExpandedResourceNestedResourceInfo(resourceState, (IEdmStructuralProperty) null, structuredType, propertyName);
      }
      object propertyValue = edmTypeReference2 is IEdmUntypedTypeReference ? (object) this.JsonReader.ReadAsUntypedOrNullValue() : this.ReadNonEntityValue(payloadTypeName1, edmTypeReference2, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, true, false, false, propertyName, new bool?(true));
      ValidationUtils.ValidateOpenPropertyValue(propertyName, propertyValue);
      ODataJsonLightPropertyAndValueDeserializer.AddResourceProperty(resourceState, propertyName, propertyValue);
      return (ODataJsonLightReaderNestedInfo) null;
    }

    private ODataJsonLightReaderNestedInfo ReadUndeclaredProperty(
      IODataJsonLightReaderResourceState resourceState,
      string propertyName,
      bool propertyWithValue)
    {
      IDictionary<string, object> propertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName);
      object obj;
      if (propertyAnnotations.TryGetValue("odata.mediaEditLink", out obj) || propertyAnnotations.TryGetValue("odata.mediaReadLink", out obj) || propertyAnnotations.TryGetValue("odata.mediaContentType", out obj) || propertyAnnotations.TryGetValue("odata.mediaEtag", out obj))
      {
        ODataStreamReferenceValue propertyValue = this.ReadStreamPropertyValue(resourceState, propertyName);
        ODataJsonLightPropertyAndValueDeserializer.AddResourceProperty(resourceState, propertyName, (object) propertyValue);
        if (!propertyWithValue)
          return (ODataJsonLightReaderNestedInfo) null;
        ODataStreamPropertyInfo streamPropertyInfo = this.ReadStreamPropertyInfo(resourceState, propertyName);
        if (!ODataJsonLightResourceDeserializer.IsJsonStream(streamPropertyInfo))
          this.JsonReader.Read();
        return (ODataJsonLightReaderNestedInfo) new ODataJsonLightReaderNestedPropertyInfo((ODataPropertyInfo) streamPropertyInfo, (IEdmProperty) null);
      }
      if (propertyWithValue)
        this.JsonReader.Read();
      if (propertyAnnotations.TryGetValue("odata.navigationLink", out obj) || propertyAnnotations.TryGetValue("odata.associationLink", out obj))
      {
        ODataJsonLightReaderNestedResourceInfo nestedResourceInfo = ODataJsonLightResourceDeserializer.ReadDeferredNestedResourceInfo(resourceState, propertyName, (IEdmNavigationProperty) null);
        resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(nestedResourceInfo.NestedResourceInfo);
        if (propertyWithValue)
        {
          ODataJsonLightPropertyAndValueDeserializer.ValidateExpandedNestedResourceInfoPropertyValue((IJsonReader) this.JsonReader, new bool?(), propertyName);
          this.JsonReader.SkipValue();
        }
        return (ODataJsonLightReaderNestedInfo) nestedResourceInfo;
      }
      if (resourceState.ResourceType.IsOpen)
        return this.InnerReadUndeclaredProperty(resourceState, resourceState.ResourceType, propertyName, propertyWithValue);
      if (!propertyWithValue)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_PropertyWithoutValueWithUnknownType((object) propertyName));
      ODataJsonLightPropertyAndValueDeserializer.ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName);
      if (this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType)
        return (ODataJsonLightReaderNestedInfo) null;
      bool isTopLevelPropertyValue = false;
      return (ODataJsonLightReaderNestedInfo) this.InnerReadUndeclaredProperty(resourceState, propertyName, isTopLevelPropertyValue);
    }

    private ODataStreamReferenceValue ReadStreamPropertyValue(
      IODataJsonLightReaderResourceState resourceState,
      string streamPropertyName)
    {
      ODataStreamReferenceValue streamInfo = new ODataStreamReferenceValue();
      this.ReadStreamInfo((IODataStreamReferenceInfo) streamInfo, resourceState, streamPropertyName);
      ODataResourceMetadataBuilder builderForReader = this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState, this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment, false);
      streamInfo.SetMetadataBuilder(builderForReader, streamPropertyName);
      return streamInfo;
    }

    private ODataStreamPropertyInfo ReadStreamPropertyInfo(
      IODataJsonLightReaderResourceState resourceState,
      string streamPropertyName)
    {
      ODataStreamPropertyInfo streamPropertyInfo = new ODataStreamPropertyInfo();
      streamPropertyInfo.Name = streamPropertyName;
      ODataStreamPropertyInfo streamInfo = streamPropertyInfo;
      this.ReadStreamInfo((IODataStreamReferenceInfo) streamInfo, resourceState, streamPropertyName);
      ODataResourceMetadataBuilder builderForReader = this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState, this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment, false);
      streamInfo.SetMetadataBuilder(builderForReader, streamPropertyName);
      return streamInfo;
    }

    private void ReadStreamInfo(
      IODataStreamReferenceInfo streamInfo,
      IODataJsonLightReaderResourceState resourceState,
      string streamPropertyName)
    {
      foreach (KeyValuePair<string, object> propertyAnnotation in (IEnumerable<KeyValuePair<string, object>>) resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(streamPropertyName))
      {
        switch (propertyAnnotation.Key)
        {
          case "odata.mediaEditLink":
            streamInfo.EditLink = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.mediaReadLink":
            streamInfo.ReadLink = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.mediaEtag":
            streamInfo.ETag = (string) propertyAnnotation.Value;
            continue;
          case "odata.mediaContentType":
            streamInfo.ContentType = (string) propertyAnnotation.Value;
            continue;
          case "odata.type":
            continue;
          default:
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedStreamPropertyAnnotation((object) streamPropertyName, (object) propertyAnnotation.Key));
        }
      }
      if (!this.ReadingResponse && (streamInfo.ETag != null || streamInfo.EditLink != (Uri) null || streamInfo.ReadLink != (Uri) null))
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_StreamPropertyInRequest((object) streamPropertyName));
    }

    private void ReadSingleOperationValue(
      IODataJsonOperationsDeserializerContext readerContext,
      IODataJsonLightReaderResourceState resourceState,
      string metadataReferencePropertyName,
      bool insideArray)
    {
      if (readerContext.JsonReader.NodeType != JsonNodeType.StartObject)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue((object) metadataReferencePropertyName, (object) readerContext.JsonReader.NodeType));
      readerContext.JsonReader.ReadStartObject();
      ODataOperation operationAndAddToEntry = this.CreateODataOperationAndAddToEntry(readerContext, metadataReferencePropertyName);
      if (operationAndAddToEntry == null)
      {
        while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
        {
          readerContext.JsonReader.ReadPropertyName();
          readerContext.JsonReader.SkipValue();
        }
        readerContext.JsonReader.ReadEndObject();
      }
      else
      {
        while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
        {
          string str1 = ODataAnnotationNames.RemoveAnnotationPrefix(readerContext.JsonReader.ReadPropertyName());
          switch (str1)
          {
            case "title":
              if (operationAndAddToEntry.Title != null)
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation((object) str1, (object) metadataReferencePropertyName));
              string propertyValue = readerContext.JsonReader.ReadStringValue("title");
              ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull((object) propertyValue, str1, metadataReferencePropertyName);
              operationAndAddToEntry.Title = propertyValue;
              continue;
            case "target":
              if (operationAndAddToEntry.Target != (Uri) null)
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation((object) str1, (object) metadataReferencePropertyName));
              string str2 = readerContext.JsonReader.ReadStringValue("target");
              ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull((object) str2, str1, metadataReferencePropertyName);
              operationAndAddToEntry.Target = readerContext.ProcessUriFromPayload(str2);
              continue;
            default:
              readerContext.JsonReader.SkipValue();
              continue;
          }
        }
        if (operationAndAddToEntry.Target == (Uri) null & insideArray)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_OperationMissingTargetProperty((object) metadataReferencePropertyName));
        readerContext.JsonReader.ReadEndObject();
        this.SetMetadataBuilder(resourceState, operationAndAddToEntry);
      }
    }

    private void ReadSingleOperationValue(
      ODataResourceSet resourceSet,
      string metadataReferencePropertyName,
      bool insideArray)
    {
      if (this.JsonReader.NodeType != JsonNodeType.StartObject)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue((object) metadataReferencePropertyName, (object) this.JsonReader.NodeType));
      this.JsonReader.ReadStartObject();
      ODataOperation addToResourceSet = this.CreateODataOperationAndAddToResourceSet(resourceSet, metadataReferencePropertyName);
      if (addToResourceSet == null)
      {
        while (this.JsonReader.NodeType == JsonNodeType.Property)
        {
          this.JsonReader.ReadPropertyName();
          this.JsonReader.SkipValue();
        }
        this.JsonReader.ReadEndObject();
      }
      else
      {
        while (this.JsonReader.NodeType == JsonNodeType.Property)
        {
          string str1 = ODataAnnotationNames.RemoveAnnotationPrefix(this.JsonReader.ReadPropertyName());
          switch (str1)
          {
            case "title":
              if (addToResourceSet.Title != null)
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation((object) str1, (object) metadataReferencePropertyName));
              string propertyValue = this.JsonReader.ReadStringValue("title");
              ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull((object) propertyValue, str1, metadataReferencePropertyName);
              addToResourceSet.Title = propertyValue;
              continue;
            case "target":
              if (addToResourceSet.Target != (Uri) null)
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation((object) str1, (object) metadataReferencePropertyName));
              string str2 = this.JsonReader.ReadStringValue("target");
              ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull((object) str2, str1, metadataReferencePropertyName);
              addToResourceSet.Target = this.ProcessUriFromPayload(str2);
              continue;
            default:
              this.JsonReader.SkipValue();
              continue;
          }
        }
        if (addToResourceSet.Target == (Uri) null & insideArray)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_OperationMissingTargetProperty((object) metadataReferencePropertyName));
        this.JsonReader.ReadEndObject();
      }
    }

    private void SetMetadataBuilder(
      IODataJsonLightReaderResourceState resourceState,
      ODataOperation operation)
    {
      ODataResourceMetadataBuilder builderForReader = this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState, this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment, false);
      operation.SetMetadataBuilder(builderForReader, this.ContextUriParseResult.MetadataDocumentUri);
    }

    private ODataOperation CreateODataOperationAndAddToEntry(
      IODataJsonOperationsDeserializerContext readerContext,
      string metadataReferencePropertyName)
    {
      IEdmOperation edmOperation = this.JsonLightInputContext.Model.ResolveOperations(ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName)).FirstOrDefault<IEdmOperation>();
      if (edmOperation == null)
        return (ODataOperation) null;
      bool isAction;
      ODataOperation odataOperation = ODataJsonLightUtils.CreateODataOperation(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName, edmOperation, out isAction);
      if (isAction)
        readerContext.AddActionToResource((ODataAction) odataOperation);
      else
        readerContext.AddFunctionToResource((ODataFunction) odataOperation);
      return odataOperation;
    }

    private ODataOperation CreateODataOperationAndAddToResourceSet(
      ODataResourceSet resourceSet,
      string metadataReferencePropertyName)
    {
      IEdmOperation edmOperation = this.JsonLightInputContext.Model.ResolveOperations(ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName)).FirstOrDefault<IEdmOperation>();
      if (edmOperation == null)
        return (ODataOperation) null;
      bool isAction;
      ODataOperation odataOperation = ODataJsonLightUtils.CreateODataOperation(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName, edmOperation, out isAction);
      if (isAction)
        resourceSet.AddAction((ODataAction) odataOperation);
      else
        resourceSet.AddFunction((ODataFunction) odataOperation);
      return odataOperation;
    }

    private void ReadMetadataReferencePropertyValue(
      IODataJsonLightReaderResourceState resourceState,
      string metadataReferencePropertyName)
    {
      this.ValidateCanReadMetadataReferenceProperty();
      ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);
      IODataJsonOperationsDeserializerContext readerContext = (IODataJsonOperationsDeserializerContext) new ODataJsonLightResourceDeserializer.OperationsDeserializerContext(resourceState.Resource, this);
      bool insideArray = false;
      if (readerContext.JsonReader.NodeType == JsonNodeType.StartArray)
      {
        readerContext.JsonReader.ReadStartArray();
        insideArray = true;
      }
      do
      {
        this.ReadSingleOperationValue(readerContext, resourceState, metadataReferencePropertyName, insideArray);
      }
      while (insideArray && readerContext.JsonReader.NodeType != JsonNodeType.EndArray);
      if (!insideArray)
        return;
      readerContext.JsonReader.ReadEndArray();
    }

    private void ReadMetadataReferencePropertyValue(
      ODataResourceSet resourceSet,
      string metadataReferencePropertyName)
    {
      this.ValidateCanReadMetadataReferenceProperty();
      ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);
      bool insideArray = false;
      if (this.JsonReader.NodeType == JsonNodeType.StartArray)
      {
        this.JsonReader.ReadStartArray();
        insideArray = true;
      }
      do
      {
        this.ReadSingleOperationValue(resourceSet, metadataReferencePropertyName, insideArray);
      }
      while (insideArray && this.JsonReader.NodeType != JsonNodeType.EndArray);
      if (!insideArray)
        return;
      this.JsonReader.ReadEndArray();
    }

    private void ValidateCanReadMetadataReferenceProperty()
    {
      if (!this.ReadingResponse)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_MetadataReferencePropertyInRequest);
    }

    private sealed class OperationsDeserializerContext : IODataJsonOperationsDeserializerContext
    {
      private ODataResourceBase resource;
      private ODataJsonLightResourceDeserializer jsonLightResourceDeserializer;

      public OperationsDeserializerContext(
        ODataResourceBase resource,
        ODataJsonLightResourceDeserializer jsonLightResourceDeserializer)
      {
        this.resource = resource;
        this.jsonLightResourceDeserializer = jsonLightResourceDeserializer;
      }

      public IJsonReader JsonReader => (IJsonReader) this.jsonLightResourceDeserializer.JsonReader;

      public Uri ProcessUriFromPayload(string uriFromPayload) => this.jsonLightResourceDeserializer.ProcessUriFromPayload(uriFromPayload);

      public void AddActionToResource(ODataAction action) => this.resource.AddAction(action);

      public void AddFunctionToResource(ODataFunction function) => this.resource.AddFunction(function);
    }
  }
}
