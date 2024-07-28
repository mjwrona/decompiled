// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightPropertyAndValueDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal class ODataJsonLightPropertyAndValueDeserializer : ODataJsonLightDeserializer
  {
    private static readonly object missingPropertyValue = new object();
    private int recursionDepth;

    internal ODataJsonLightPropertyAndValueDeserializer(
      ODataJsonLightInputContext jsonLightInputContext)
      : base(jsonLightInputContext)
    {
    }

    internal ODataProperty ReadTopLevelProperty(IEdmTypeReference expectedPropertyTypeReference)
    {
      PropertyAndAnnotationCollector annotationCollector = this.CreatePropertyAndAnnotationCollector();
      this.ReadPayloadStart(ODataPayloadKind.Property, annotationCollector, false, false);
      ODataProperty odataProperty = this.ReadTopLevelPropertyImplementation(expectedPropertyTypeReference, annotationCollector);
      this.ReadPayloadEnd(false);
      return odataProperty;
    }

    internal Task<ODataProperty> ReadTopLevelPropertyAsync(
      IEdmTypeReference expectedPropertyTypeReference)
    {
      PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
      return this.ReadPayloadStartAsync(ODataPayloadKind.Property, propertyAndAnnotationCollector, false, false).FollowOnSuccessWith<ODataProperty>((Func<Task, ODataProperty>) (t =>
      {
        ODataProperty odataProperty = this.ReadTopLevelPropertyImplementation(expectedPropertyTypeReference, propertyAndAnnotationCollector);
        this.ReadPayloadEnd(false);
        return odataProperty;
      }));
    }

    internal object ReadNonEntityValue(
      string payloadTypeName,
      IEdmTypeReference expectedValueTypeReference,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      CollectionWithoutExpectedTypeValidator collectionValidator,
      bool validateNullValue,
      bool isTopLevelPropertyValue,
      bool insideResourceValue,
      string propertyName,
      bool? isDynamicProperty = null)
    {
      return this.ReadNonEntityValueImplementation(payloadTypeName, expectedValueTypeReference, propertyAndAnnotationCollector, collectionValidator, validateNullValue, isTopLevelPropertyValue, insideResourceValue, propertyName, isDynamicProperty);
    }

    internal object ReadCustomInstanceAnnotationValue(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      string name)
    {
      string odataType = (string) null;
      object typeName;
      if (propertyAndAnnotationCollector.GetODataPropertyAnnotations(name).TryGetValue("odata.type", out typeName))
        odataType = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string) typeName));
      return this.ReadODataOrCustomInstanceAnnotationValue(name, odataType);
    }

    internal object ReadODataOrCustomInstanceAnnotationValue(
      string annotationName,
      string odataType)
    {
      IEdmTypeReference expectedTypeReference = MetadataUtils.LookupTypeOfTerm(annotationName, this.Model);
      return this.ReadNonEntityValueImplementation(odataType, expectedTypeReference, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, false, false, false, annotationName);
    }

    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Each code path casts to bool at most one time, and only if needed.")]
    internal static IEdmTypeReference ResolveUntypedType(
      JsonNodeType jsonReaderNodeType,
      object jsonReaderValue,
      string payloadTypeName,
      IEdmTypeReference payloadTypeReference,
      Func<object, string, IEdmTypeReference> primitiveTypeResolver,
      bool readUntypedAsString,
      bool generateTypeIfMissing)
    {
      if (payloadTypeReference != null && payloadTypeReference.TypeKind() != EdmTypeKind.Untyped | readUntypedAsString)
        return payloadTypeReference;
      if (readUntypedAsString)
        return jsonReaderNodeType == JsonNodeType.PrimitiveValue && jsonReaderValue is bool ? (IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(true) : (IEdmTypeReference) EdmCoreModel.Instance.GetUntyped();
      switch (jsonReaderNodeType)
      {
        case JsonNodeType.StartObject:
          if (!(payloadTypeName != null & generateTypeIfMissing))
            return new EdmUntypedStructuredType().ToTypeReference(true);
          string namespaceName1;
          string typeName1;
          bool isCollection1;
          TypeUtils.ParseQualifiedTypeName(payloadTypeName, out namespaceName1, out typeName1, out isCollection1);
          if (isCollection1)
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_CollectionTypeNotExpected((object) payloadTypeName));
          return new EdmUntypedStructuredType(namespaceName1, typeName1).ToTypeReference(true);
        case JsonNodeType.StartArray:
          if (!(payloadTypeName != null & generateTypeIfMissing))
            return new EdmCollectionType(new EdmUntypedStructuredType().ToTypeReference(true)).ToTypeReference(true);
          string namespaceName2;
          string typeName2;
          bool isCollection2;
          TypeUtils.ParseQualifiedTypeName(payloadTypeName, out namespaceName2, out typeName2, out isCollection2);
          if (!isCollection2)
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_CollectionTypeExpected((object) payloadTypeName));
          return new EdmCollectionType(new EdmUntypedStructuredType(namespaceName2, typeName2).ToTypeReference(true)).ToTypeReference(true);
        case JsonNodeType.PrimitiveValue:
          if (primitiveTypeResolver != null)
          {
            IEdmTypeReference edmTypeReference = primitiveTypeResolver(jsonReaderValue, payloadTypeName);
            if (edmTypeReference != null)
              return edmTypeReference;
          }
          IEdmTypeReference type;
          switch (jsonReaderValue)
          {
            case null:
              if (payloadTypeName != null)
              {
                string namespaceName3;
                string typeName3;
                bool isCollection3;
                TypeUtils.ParseQualifiedTypeName(payloadTypeName, out namespaceName3, out typeName3, out isCollection3);
                IEdmTypeReference typeReference = new EdmUntypedStructuredType(namespaceName3, typeName3).ToTypeReference(true);
                return !isCollection3 ? typeReference : new EdmCollectionType(typeReference).ToTypeReference(true);
              }
              type = (IEdmTypeReference) EdmCoreModel.Instance.GetString(true);
              break;
            case bool _:
              type = (IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(true);
              break;
            case string _:
              type = (IEdmTypeReference) EdmCoreModel.Instance.GetString(true);
              break;
            default:
              type = (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(true);
              break;
          }
          if (payloadTypeName != null)
          {
            string namespaceName4;
            string typeName4;
            bool isCollection4;
            TypeUtils.ParseQualifiedTypeName(payloadTypeName, out namespaceName4, out typeName4, out isCollection4);
            if (isCollection4)
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_CollectionTypeNotExpected((object) payloadTypeName));
            type = new EdmTypeDefinition(namespaceName4, typeName4, type.PrimitiveKind()).ToTypeReference(true);
          }
          return type;
        default:
          return (IEdmTypeReference) EdmCoreModel.Instance.GetUntyped();
      }
    }

    protected string TryReadOrPeekPayloadType(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      string propertyName,
      bool insideResourceValue)
    {
      string str = ODataJsonLightPropertyAndValueDeserializer.ValidateDataPropertyTypeNameAnnotation(propertyAndAnnotationCollector, propertyName);
      bool flag = this.JsonReader.NodeType == JsonNodeType.StartObject;
      if (string.IsNullOrEmpty(str) & flag)
      {
        try
        {
          this.JsonReader.StartBuffering();
          propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
          string payloadTypeName;
          if (this.TryReadPayloadTypeFromObject(propertyAndAnnotationCollector, insideResourceValue, out payloadTypeName))
            str = payloadTypeName;
        }
        finally
        {
          this.JsonReader.StopBuffering();
        }
      }
      return str;
    }

    protected ODataJsonLightReaderNestedResourceInfo InnerReadUndeclaredProperty(
      IODataJsonLightReaderResourceState resourceState,
      string propertyName,
      bool isTopLevelPropertyValue)
    {
      PropertyAndAnnotationCollector annotationCollector = resourceState.PropertyAndAnnotationCollector;
      bool insideResourceValue = false;
      string payloadTypeName1 = ODataJsonLightPropertyAndValueDeserializer.ValidateDataPropertyTypeNameAnnotation(annotationCollector, propertyName);
      string payloadTypeName2 = this.TryReadOrPeekPayloadType(annotationCollector, propertyName, insideResourceValue);
      IEdmType edmType = ReaderValidationUtils.ResolvePayloadTypeName(this.Model, (IEdmTypeReference) null, payloadTypeName2, EdmTypeKind.Complex, this.MessageReaderSettings.ClientCustomTypeResolver, out EdmTypeKind _);
      IEdmTypeReference payloadTypeReference = (IEdmTypeReference) null;
      if (!string.IsNullOrEmpty(payloadTypeName2) && edmType != null)
        payloadTypeReference = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(EdmTypeKind.None, new bool?(), (IEdmType) null, (IEdmTypeReference) null, payloadTypeName2, this.Model, new Func<EdmTypeKind>(this.GetNonEntityValueKind), out EdmTypeKind _, out ODataTypeAnnotation _);
      IEdmTypeReference edmTypeReference = ODataJsonLightPropertyAndValueDeserializer.ResolveUntypedType(this.JsonReader.NodeType, this.JsonReader.Value, payloadTypeName2, payloadTypeReference, this.MessageReaderSettings.PrimitiveTypeResolver, this.MessageReaderSettings.ReadUntypedAsString, !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);
      if (edmTypeReference.ToStructuredType() != null)
      {
        bool flag = edmTypeReference.IsCollection();
        ODataJsonLightPropertyAndValueDeserializer.ValidateExpandedNestedResourceInfoPropertyValue((IJsonReader) this.JsonReader, new bool?(flag), propertyName);
        ODataJsonLightReaderNestedResourceInfo nestedResourceInfo = !flag ? (this.ReadingResponse ? ODataJsonLightPropertyAndValueDeserializer.ReadExpandedResourceNestedResourceInfo(resourceState, (IEdmNavigationProperty) null, propertyName, edmTypeReference.ToStructuredType(), this.MessageReaderSettings) : ODataJsonLightPropertyAndValueDeserializer.ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, (IEdmNavigationProperty) null, propertyName, true)) : (this.ReadingResponse ? ODataJsonLightPropertyAndValueDeserializer.ReadExpandedResourceSetNestedResourceInfo(resourceState, (IEdmNavigationProperty) null, edmTypeReference.ToStructuredType(), propertyName, false) : ODataJsonLightPropertyAndValueDeserializer.ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, (IEdmNavigationProperty) null, propertyName, true));
        resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(nestedResourceInfo.NestedResourceInfo);
        return nestedResourceInfo;
      }
      object propertyValue = edmTypeReference is IEdmUntypedTypeReference ? (object) this.JsonReader.ReadAsUntypedOrNullValue() : this.ReadNonEntityValueImplementation(payloadTypeName1, edmTypeReference, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, false, isTopLevelPropertyValue, insideResourceValue, propertyName);
      ODataJsonLightPropertyAndValueDeserializer.AddResourceProperty(resourceState, propertyName, propertyValue);
      return (ODataJsonLightReaderNestedResourceInfo) null;
    }

    protected static void ValidateExpandedNestedResourceInfoPropertyValue(
      IJsonReader jsonReader,
      bool? isCollection,
      string propertyName)
    {
      JsonNodeType nodeType = jsonReader.NodeType;
      if (nodeType == JsonNodeType.StartArray)
      {
        bool? nullable = isCollection;
        bool flag = false;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_CannotReadSingletonNestedResource((object) nodeType, (object) propertyName));
      }
      else
      {
        if ((nodeType != JsonNodeType.PrimitiveValue || jsonReader.Value != null) && nodeType != JsonNodeType.StartObject)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_CannotReadNestedResource((object) propertyName));
        bool? nullable = isCollection;
        bool flag = true;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_CannotReadCollectionNestedResource((object) nodeType, (object) propertyName));
      }
    }

    protected static ODataJsonLightReaderNestedResourceInfo ReadNonExpandedResourceSetNestedResourceInfo(
      IODataJsonLightReaderResourceState resourceState,
      IEdmStructuralProperty collectionProperty,
      IEdmStructuredType nestedResourceType,
      string propertyName)
    {
      ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
      {
        Name = propertyName,
        IsCollection = new bool?(true),
        IsComplex = true
      };
      ODataResourceSet collectionResourceSet = ODataJsonLightPropertyAndValueDeserializer.CreateCollectionResourceSet(resourceState, propertyName);
      return ODataJsonLightReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(nestedResourceInfo, (IEdmProperty) collectionProperty, (IEdmType) nestedResourceType, (ODataResourceSetBase) collectionResourceSet);
    }

    protected static ODataJsonLightReaderNestedResourceInfo ReadNonExpandedResourceNestedResourceInfo(
      IODataJsonLightReaderResourceState resourceState,
      IEdmStructuralProperty complexProperty,
      IEdmStructuredType nestedResourceType,
      string propertyName)
    {
      ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
      {
        Name = propertyName,
        IsCollection = new bool?(false),
        IsComplex = true
      };
      if (ODataJsonLightPropertyAndValueDeserializer.ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, nestedResourceInfo.Name) != null)
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation((object) "odata.type"));
      return ODataJsonLightReaderNestedResourceInfo.CreateResourceReaderNestedResourceInfo(nestedResourceInfo, (IEdmProperty) complexProperty, nestedResourceType);
    }

    protected static ODataJsonLightReaderNestedResourceInfo ReadExpandedResourceNestedResourceInfo(
      IODataJsonLightReaderResourceState resourceState,
      IEdmNavigationProperty navigationProperty,
      string propertyName,
      IEdmStructuredType propertyType,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
      {
        Name = propertyName,
        IsCollection = new bool?(false)
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
          case "odata.context":
            nestedResourceInfo.ContextUrl = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.type":
            nestedResourceInfo.TypeAnnotation = new ODataTypeAnnotation((string) propertyAnnotation.Value);
            continue;
          default:
            if (messageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType)
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation((object) nestedResourceInfo.Name, (object) propertyAnnotation.Key));
            continue;
        }
      }
      return ODataJsonLightReaderNestedResourceInfo.CreateResourceReaderNestedResourceInfo(nestedResourceInfo, (IEdmProperty) navigationProperty, propertyType);
    }

    protected static ODataJsonLightReaderNestedResourceInfo ReadExpandedResourceSetNestedResourceInfo(
      IODataJsonLightReaderResourceState resourceState,
      IEdmNavigationProperty navigationProperty,
      IEdmStructuredType propertyType,
      string propertyName,
      bool isDeltaResourceSet)
    {
      ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
      {
        Name = propertyName,
        IsCollection = new bool?(true)
      };
      ODataResourceSetBase resourceSet = !isDeltaResourceSet ? (ODataResourceSetBase) new ODataResourceSet() : (ODataResourceSetBase) new ODataDeltaResourceSet();
      foreach (KeyValuePair<string, object> propertyAnnotation in (IEnumerable<KeyValuePair<string, object>>) resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
      {
        switch (propertyAnnotation.Key)
        {
          case "odata.associationLink":
            nestedResourceInfo.AssociationLinkUrl = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.context":
            nestedResourceInfo.ContextUrl = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.count":
            resourceSet.Count = (long?) propertyAnnotation.Value;
            continue;
          case "odata.navigationLink":
            nestedResourceInfo.Url = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.nextLink":
            resourceSet.NextPageLink = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.type":
            resourceSet.TypeName = (string) propertyAnnotation.Value;
            continue;
          default:
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation((object) nestedResourceInfo.Name, (object) propertyAnnotation.Key));
        }
      }
      return ODataJsonLightReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(nestedResourceInfo, (IEdmProperty) navigationProperty, (IEdmType) propertyType, resourceSet);
    }

    protected static ODataJsonLightReaderNestedResourceInfo ReadStreamCollectionNestedResourceInfo(
      IODataJsonLightReaderResourceState resourceState,
      IEdmStructuralProperty collectionProperty,
      string propertyName,
      IEdmType elementType)
    {
      ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
      {
        Name = propertyName,
        IsCollection = new bool?(true),
        IsComplex = false
      };
      ODataResourceSet collectionResourceSet = ODataJsonLightPropertyAndValueDeserializer.CreateCollectionResourceSet(resourceState, propertyName);
      return ODataJsonLightReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(nestedResourceInfo, (IEdmProperty) collectionProperty, elementType, (ODataResourceSetBase) collectionResourceSet);
    }

    protected static ODataJsonLightReaderNestedResourceInfo ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(
      IODataJsonLightReaderResourceState resourceState,
      IEdmNavigationProperty navigationProperty,
      string propertyName,
      bool isExpanded)
    {
      ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
      {
        Name = propertyName,
        IsCollection = new bool?(false)
      };
      ODataEntityReferenceLink entityReferenceLink = (ODataEntityReferenceLink) null;
      foreach (KeyValuePair<string, object> propertyAnnotation in (IEnumerable<KeyValuePair<string, object>>) resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
      {
        if (!(propertyAnnotation.Key == "odata.bind"))
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation((object) nestedResourceInfo.Name, (object) propertyAnnotation.Key, (object) "odata.bind"));
        if (propertyAnnotation.Value is LinkedList<ODataEntityReferenceLink>)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_ArrayValueForSingletonBindPropertyAnnotation((object) nestedResourceInfo.Name, (object) "odata.bind"));
        if (isExpanded)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_SingletonNavigationPropertyWithBindingAndValue((object) nestedResourceInfo.Name, (object) "odata.bind"));
        entityReferenceLink = (ODataEntityReferenceLink) propertyAnnotation.Value;
      }
      return ODataJsonLightReaderNestedResourceInfo.CreateSingletonEntityReferenceLinkInfo(nestedResourceInfo, navigationProperty, entityReferenceLink, isExpanded);
    }

    protected static ODataJsonLightReaderNestedResourceInfo ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(
      IODataJsonLightReaderResourceState resourceState,
      IEdmNavigationProperty navigationProperty,
      string propertyName,
      bool isExpanded)
    {
      ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
      {
        Name = propertyName,
        IsCollection = new bool?(true)
      };
      LinkedList<ODataEntityReferenceLink> entityReferenceLinks = (LinkedList<ODataEntityReferenceLink>) null;
      foreach (KeyValuePair<string, object> propertyAnnotation in (IEnumerable<KeyValuePair<string, object>>) resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
      {
        if (!(propertyAnnotation.Key == "odata.bind"))
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation((object) nestedResourceInfo.Name, (object) propertyAnnotation.Key, (object) "odata.bind"));
        entityReferenceLinks = !(propertyAnnotation.Value is ODataEntityReferenceLink) ? (LinkedList<ODataEntityReferenceLink>) propertyAnnotation.Value : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_StringValueForCollectionBindPropertyAnnotation((object) nestedResourceInfo.Name, (object) "odata.bind"));
      }
      return ODataJsonLightReaderNestedResourceInfo.CreateCollectionEntityReferenceLinksInfo(nestedResourceInfo, navigationProperty, entityReferenceLinks, isExpanded);
    }

    protected static string ValidateDataPropertyTypeNameAnnotation(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      string propertyName)
    {
      string resourceTypeName = (string) null;
      foreach (KeyValuePair<string, object> propertyAnnotation in (IEnumerable<KeyValuePair<string, object>>) propertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName))
      {
        if (string.CompareOrdinal(propertyAnnotation.Key, "odata.type") == 0)
          resourceTypeName = (string) propertyAnnotation.Value;
      }
      if (resourceTypeName != null)
        propertyAndAnnotationCollector.GetDerivedTypeValidator(propertyName)?.ValidateResourceType(resourceTypeName);
      return resourceTypeName;
    }

    protected static ODataProperty AddResourceProperty(
      IODataJsonLightReaderResourceState resourceState,
      string propertyName,
      object propertyValue)
    {
      ODataProperty odataProperty = new ODataProperty();
      odataProperty.Name = propertyName;
      odataProperty.Value = propertyValue;
      ODataProperty property = odataProperty;
      ODataJsonLightPropertyAndValueDeserializer.AttachODataAnnotations(resourceState, propertyName, property);
      foreach (KeyValuePair<string, object> propertyAnnotation in resourceState.PropertyAndAnnotationCollector.GetCustomPropertyAnnotations(propertyName))
      {
        if (propertyAnnotation.Value != null)
          property.InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyAnnotation.Key, propertyAnnotation.Value.ToODataValue()));
      }
      resourceState.PropertyAndAnnotationCollector.CheckForDuplicatePropertyNames(property);
      ODataResourceBase resource = resourceState.Resource;
      resource.Properties = (IEnumerable<ODataProperty>) resource.Properties.ConcatToReadOnlyEnumerable<ODataProperty>("Properties", property);
      return property;
    }

    protected static void AttachODataAnnotations(
      IODataJsonLightReaderResourceState resourceState,
      string propertyName,
      ODataProperty property)
    {
      foreach (KeyValuePair<string, object> keyValuePair in propertyName.Length == 0 ? (IEnumerable<KeyValuePair<string, object>>) resourceState.PropertyAndAnnotationCollector.GetODataScopeAnnotation() : (IEnumerable<KeyValuePair<string, object>>) resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName))
      {
        if (string.Equals(keyValuePair.Key, "odata.type", StringComparison.Ordinal) || string.Equals(keyValuePair.Key, "@type", StringComparison.Ordinal))
        {
          property.TypeAnnotation = new ODataTypeAnnotation(ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string) keyValuePair.Value)));
        }
        else
        {
          Uri uri;
          ODataValue annotationValue = (uri = keyValuePair.Value as Uri) != (Uri) null ? (ODataValue) new ODataPrimitiveValue((object) uri.OriginalString) : keyValuePair.Value.ToODataValue();
          property.InstanceAnnotations.Add(new ODataInstanceAnnotation(keyValuePair.Key, annotationValue, true));
        }
      }
    }

    protected bool TryReadODataTypeAnnotationValue(string annotationName, out string value)
    {
      if (string.CompareOrdinal(annotationName, "odata.type") == 0)
      {
        value = this.ReadODataTypeAnnotationValue();
        return true;
      }
      value = (string) null;
      return false;
    }

    protected string ReadODataTypeAnnotationValue()
    {
      string p0 = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName(this.JsonReader.ReadStringValue()));
      return p0 != null ? p0 : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName((object) p0));
    }

    protected object ReadTypePropertyAnnotationValue(string propertyAnnotationName)
    {
      string str;
      if (this.TryReadODataTypeAnnotationValue(propertyAnnotationName, out str))
        return (object) str;
      throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) propertyAnnotationName));
    }

    protected EdmTypeKind GetNonEntityValueKind()
    {
      switch (this.JsonReader.NodeType)
      {
        case JsonNodeType.StartArray:
          return EdmTypeKind.Collection;
        case JsonNodeType.PrimitiveValue:
          return EdmTypeKind.Primitive;
        default:
          return EdmTypeKind.Complex;
      }
    }

    private static ODataResourceSet CreateCollectionResourceSet(
      IODataJsonLightReaderResourceState resourceState,
      string propertyName)
    {
      ODataResourceSet collectionResourceSet = new ODataResourceSet();
      foreach (KeyValuePair<string, object> propertyAnnotation in (IEnumerable<KeyValuePair<string, object>>) resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName))
      {
        switch (propertyAnnotation.Key)
        {
          case "odata.nextLink":
            collectionResourceSet.NextPageLink = (Uri) propertyAnnotation.Value;
            continue;
          case "odata.count":
            collectionResourceSet.Count = (long?) propertyAnnotation.Value;
            continue;
          case "odata.type":
            collectionResourceSet.TypeName = (string) propertyAnnotation.Value;
            continue;
          default:
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceDeserializer_UnexpectedComplexCollectionPropertyAnnotation((object) propertyName, (object) propertyAnnotation.Key));
        }
      }
      return collectionResourceSet;
    }

    private bool TryReadODataTypeAnnotation(out string payloadTypeName)
    {
      payloadTypeName = (string) null;
      bool flag = false;
      string propertyName = this.JsonReader.GetPropertyName();
      if (string.CompareOrdinal(propertyName, "@odata.type") == 0 || this.CompareSimplifiedODataAnnotation("@type", propertyName))
      {
        int num = (int) this.JsonReader.ReadNext();
        payloadTypeName = this.ReadODataTypeAnnotationValue();
        flag = true;
      }
      return flag;
    }

    private ODataProperty ReadTopLevelPropertyImplementation(
      IEdmTypeReference expectedPropertyTypeReference,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      expectedPropertyTypeReference = this.UpdateExpectedTypeBasedOnContextUri(expectedPropertyTypeReference);
      object propertyValue = ODataJsonLightPropertyAndValueDeserializer.missingPropertyValue;
      Collection<ODataInstanceAnnotation> customInstanceAnnotations = new Collection<ODataInstanceAnnotation>();
      if (this.IsTopLevel6xNullValue())
      {
        this.ReaderValidator.ValidateNullValue(expectedPropertyTypeReference, true, (string) null, new bool?());
        this.ValidateNoPropertyInNullPayload(propertyAndAnnotationCollector);
        propertyValue = (object) null;
      }
      else
      {
        string payloadTypeName = (string) null;
        if (this.ReadingResourceProperty(propertyAndAnnotationCollector, expectedPropertyTypeReference, out payloadTypeName))
        {
          propertyValue = this.ReadNonEntityValue(payloadTypeName, expectedPropertyTypeReference, propertyAndAnnotationCollector, (CollectionWithoutExpectedTypeValidator) null, true, true, true, (string) null);
        }
        else
        {
          bool isReordering = this.JsonReader is ReorderingJsonReader;
          Func<string, object> readPropertyAnnotationValue = (Func<string, object>) (annotationName =>
          {
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation((object) annotationName));
          });
          while (this.JsonReader.NodeType == JsonNodeType.Property)
            this.ProcessProperty(propertyAndAnnotationCollector, readPropertyAnnotationValue, (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
            {
              if (this.JsonReader.NodeType == JsonNodeType.Property)
                this.JsonReader.Read();
              switch (propertyParsingResult)
              {
                case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
                  if (string.CompareOrdinal("value", propertyName) != 0)
                    throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName((object) propertyName, (object) "value"));
                  propertyValue = this.ReadNonEntityValue(payloadTypeName, expectedPropertyTypeReference, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, true, true, false, propertyName);
                  break;
                case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
                  throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty((object) propertyName));
                case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
                  if (string.CompareOrdinal("odata.type", propertyName) != 0)
                    throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) propertyName));
                  if (isReordering)
                  {
                    this.JsonReader.SkipValue();
                    break;
                  }
                  if (ODataJsonLightPropertyAndValueDeserializer.missingPropertyValue != propertyValue)
                    throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_TypePropertyAfterValueProperty((object) "odata.type", (object) "value"));
                  payloadTypeName = this.ReadODataTypeAnnotationValue();
                  break;
                case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
                  ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                  object objectToConvert = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, propertyName);
                  customInstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, objectToConvert.ToODataValue()));
                  break;
                case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
                  throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
              }
            }));
          if (ODataJsonLightPropertyAndValueDeserializer.missingPropertyValue == propertyValue)
            throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
        }
      }
      ODataProperty odataProperty1 = new ODataProperty();
      odataProperty1.Name = (string) null;
      odataProperty1.Value = propertyValue;
      odataProperty1.InstanceAnnotations = (ICollection<ODataInstanceAnnotation>) customInstanceAnnotations;
      ODataProperty odataProperty2 = odataProperty1;
      this.JsonReader.Read();
      return odataProperty2;
    }

    private IEdmTypeReference UpdateExpectedTypeBasedOnContextUri(
      IEdmTypeReference expectedPropertyTypeReference)
    {
      if (this.ContextUriParseResult == null || this.ContextUriParseResult.EdmType == null)
        return expectedPropertyTypeReference;
      IEdmType edmType = this.ContextUriParseResult.EdmType;
      if (expectedPropertyTypeReference != null && !expectedPropertyTypeReference.Definition.IsAssignableFrom(edmType))
        throw new ODataException(Microsoft.OData.Strings.ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType((object) UriUtils.UriToString(this.ContextUriParseResult.ContextUri), (object) edmType.FullTypeName(), (object) expectedPropertyTypeReference.FullName()));
      bool nullable = true;
      if (expectedPropertyTypeReference != null)
        nullable = expectedPropertyTypeReference.IsNullable;
      return edmType.ToTypeReference(nullable);
    }

    private ODataCollectionValue ReadCollectionValue(
      IEdmCollectionTypeReference collectionValueTypeReference,
      string payloadTypeName,
      ODataTypeAnnotation typeAnnotation)
    {
      this.IncreaseRecursionDepth();
      this.JsonReader.ReadStartArray();
      ODataCollectionValue odataCollectionValue = new ODataCollectionValue();
      odataCollectionValue.TypeName = collectionValueTypeReference != null ? collectionValueTypeReference.FullName() : payloadTypeName;
      if (typeAnnotation != null)
        odataCollectionValue.TypeAnnotation = typeAnnotation;
      List<object> sourceList = new List<object>();
      PropertyAndAnnotationCollector annotationCollector = this.CreatePropertyAndAnnotationCollector();
      IEdmTypeReference edmTypeReference = (IEdmTypeReference) null;
      if (collectionValueTypeReference != null)
        edmTypeReference = collectionValueTypeReference.CollectionDefinition().ElementType;
      CollectionWithoutExpectedTypeValidator collectionValidator = (CollectionWithoutExpectedTypeValidator) null;
      while (this.JsonReader.NodeType != JsonNodeType.EndArray)
      {
        object obj = this.ReadNonEntityValueImplementation((string) null, edmTypeReference, annotationCollector, collectionValidator, true, false, false, (string) null);
        ValidationUtils.ValidateCollectionItem(obj, edmTypeReference.IsNullable());
        sourceList.Add(obj);
      }
      this.JsonReader.ReadEndArray();
      odataCollectionValue.Items = (IEnumerable<object>) new ReadOnlyEnumerable<object>((IList<object>) sourceList);
      this.DecreaseRecursionDepth();
      return odataCollectionValue;
    }

    private object ReadTypeDefinitionValue(
      bool insideJsonObjectValue,
      IEdmTypeDefinitionReference expectedValueTypeReference,
      bool validateNullValue,
      string propertyName)
    {
      object p0 = this.ReadPrimitiveValue(insideJsonObjectValue, expectedValueTypeReference.AsPrimitive(), validateNullValue, propertyName);
      try
      {
        return this.Model.GetPrimitiveValueConverter((IEdmTypeReference) expectedValueTypeReference).ConvertFromUnderlyingType(p0);
      }
      catch (OverflowException ex)
      {
        throw new ODataException(Microsoft.OData.Strings.EdmLibraryExtensions_ValueOverflowForUnderlyingType(p0, (object) expectedValueTypeReference.FullName()));
      }
    }

    private object ReadPrimitiveValue(
      bool insideJsonObjectValue,
      IEdmPrimitiveTypeReference expectedValueTypeReference,
      bool validateNullValue,
      string propertyName)
    {
      object obj;
      if (expectedValueTypeReference != null && expectedValueTypeReference.IsSpatial())
      {
        obj = (object) ODataJsonReaderCoreUtils.ReadSpatialValue((IJsonReader) this.JsonReader, insideJsonObjectValue, (ODataInputContext) this.JsonLightInputContext, expectedValueTypeReference, validateNullValue, this.recursionDepth, propertyName);
      }
      else
      {
        if (insideJsonObjectValue)
          throw new ODataException(Microsoft.OData.Strings.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName((object) JsonNodeType.PrimitiveValue, (object) JsonNodeType.StartObject, (object) propertyName));
        obj = this.JsonReader.ReadPrimitiveValue();
        if (expectedValueTypeReference != null)
        {
          if ((expectedValueTypeReference.IsDecimal() || expectedValueTypeReference.IsInt64()) && obj != null && obj is string ^ this.JsonReader.IsIeee754Compatible)
            throw new ODataException(Microsoft.OData.Strings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter((object) expectedValueTypeReference.FullName()));
          obj = ODataJsonLightReaderUtils.ConvertValue(obj, expectedValueTypeReference, this.MessageReaderSettings, validateNullValue, propertyName, this.JsonLightInputContext.PayloadValueConverter);
        }
        else if (obj is Decimal num)
          return (object) Convert.ToDouble(num);
      }
      return obj;
    }

    private object ReadEnumValue(
      bool insideJsonObjectValue,
      IEdmEnumTypeReference expectedValueTypeReference,
      bool validateNullValue,
      string propertyName)
    {
      if (insideJsonObjectValue)
        throw new ODataException(Microsoft.OData.Strings.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName((object) JsonNodeType.PrimitiveValue, (object) JsonNodeType.StartObject, (object) propertyName));
      return (object) new ODataEnumValue(this.JsonReader.ReadStringValue(), expectedValueTypeReference.FullName());
    }

    private ODataResourceValue ReadResourceValue(
      bool insideJsonObjectValue,
      bool insideResourceValue,
      string propertyName,
      IEdmStructuredTypeReference structuredTypeReference,
      string payloadTypeName,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      if (!insideJsonObjectValue && !insideResourceValue)
      {
        if (this.JsonReader.NodeType != JsonNodeType.StartObject)
        {
          string str = structuredTypeReference != null ? structuredTypeReference.FullName() : payloadTypeName;
          throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The property with name '{0}' was found with a value node of type '{1}'; however, a resource value of type '{2}' was expected.", new object[3]
          {
            (object) propertyName,
            (object) this.JsonReader.NodeType,
            (object) str
          }));
        }
        this.JsonReader.Read();
      }
      return this.ReadResourceValue(structuredTypeReference, payloadTypeName, propertyAndAnnotationCollector);
    }

    private ODataResourceValue ReadResourceValue(
      IEdmStructuredTypeReference structuredTypeReference,
      string payloadTypeName,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      this.IncreaseRecursionDepth();
      ODataResourceValue resourceValue = new ODataResourceValue();
      resourceValue.TypeName = structuredTypeReference != null ? structuredTypeReference.FullName() : payloadTypeName;
      if (structuredTypeReference != null)
        resourceValue.TypeAnnotation = new ODataTypeAnnotation(resourceValue.TypeName);
      List<ODataProperty> properties = new List<ODataProperty>();
      while (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        this.ReadPropertyCustomAnnotationValue = new Func<PropertyAndAnnotationCollector, string, object>(this.ReadCustomInstanceAnnotationValue);
        this.ProcessProperty(propertyAndAnnotationCollector, new Func<string, object>(this.ReadTypePropertyAnnotationValue), (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
              ODataProperty property1 = new ODataProperty();
              property1.Name = propertyName;
              IEdmProperty property2 = (IEdmProperty) null;
              if (structuredTypeReference != null)
                property2 = ReaderValidationUtils.ValidatePropertyDefined(propertyName, structuredTypeReference.StructuredDefinition(), this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType);
              ODataNullValueBehaviorKind valueBehaviorKind = this.ReadingResponse || property2 == null ? ODataNullValueBehaviorKind.Default : this.Model.NullValueReadBehaviorKind(property2);
              object obj = this.ReadNonEntityValueImplementation(ODataJsonLightPropertyAndValueDeserializer.ValidateDataPropertyTypeNameAnnotation(propertyAndAnnotationCollector, propertyName), property2 == null ? (IEdmTypeReference) null : property2.Type, (PropertyAndAnnotationCollector) null, (CollectionWithoutExpectedTypeValidator) null, valueBehaviorKind == ODataNullValueBehaviorKind.Default, false, false, propertyName, new bool?(property2 == null));
              if (valueBehaviorKind == ODataNullValueBehaviorKind.IgnoreValue && obj == null)
                break;
              propertyAndAnnotationCollector.CheckForDuplicatePropertyNames(property1);
              property1.Value = obj;
              IEnumerable<KeyValuePair<string, object>> propertyAnnotations = propertyAndAnnotationCollector.GetCustomPropertyAnnotations(propertyName);
              if (propertyAnnotations != null)
              {
                foreach (KeyValuePair<string, object> keyValuePair in propertyAnnotations)
                {
                  if (keyValuePair.Value != null)
                    property1.InstanceAnnotations.Add(new ODataInstanceAnnotation(keyValuePair.Key, keyValuePair.Value.ToODataValue()));
                }
              }
              properties.Add(property1);
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_ResourceValuePropertyAnnotationWithoutProperty((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              if (string.CompareOrdinal("odata.type", propertyName) == 0)
                throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_ResourceTypeAnnotationNotFirst);
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
              object objectToConvert = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, propertyName);
              resourceValue.InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, objectToConvert.ToODataValue()));
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
          }
        }));
      }
      this.JsonReader.ReadEndObject();
      resourceValue.Properties = (IEnumerable<ODataProperty>) new ReadOnlyEnumerable<ODataProperty>((IList<ODataProperty>) properties);
      this.DecreaseRecursionDepth();
      return resourceValue;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "No easy way to refactor.")]
    private object ReadNonEntityValueImplementation(
      string payloadTypeName,
      IEdmTypeReference expectedTypeReference,
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      CollectionWithoutExpectedTypeValidator collectionValidator,
      bool validateNullValue,
      bool isTopLevelPropertyValue,
      bool insideResourceValue,
      string propertyName,
      bool? isDynamicProperty = null)
    {
      bool insideJsonObjectValue = this.JsonReader.NodeType == JsonNodeType.StartObject;
      bool flag1 = false;
      if (insideJsonObjectValue | insideResourceValue)
      {
        if (propertyAndAnnotationCollector == null)
          propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
        else
          propertyAndAnnotationCollector.Reset();
        if (!insideResourceValue)
        {
          string payloadTypeName1;
          flag1 = this.TryReadPayloadTypeFromObject(propertyAndAnnotationCollector, insideResourceValue, out payloadTypeName1);
          if (flag1)
            payloadTypeName = payloadTypeName1;
        }
      }
      EdmTypeKind targetTypeKind;
      ODataTypeAnnotation typeAnnotation;
      IEdmTypeReference edmTypeReference = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(EdmTypeKind.None, new bool?(), (IEdmType) null, expectedTypeReference, payloadTypeName, this.Model, new Func<EdmTypeKind>(this.GetNonEntityValueKind), out targetTypeKind, out typeAnnotation);
      if (targetTypeKind == EdmTypeKind.Untyped || targetTypeKind == EdmTypeKind.None)
      {
        edmTypeReference = ODataJsonLightPropertyAndValueDeserializer.ResolveUntypedType(this.JsonReader.NodeType, this.JsonReader.Value, payloadTypeName, expectedTypeReference, this.MessageReaderSettings.PrimitiveTypeResolver, this.MessageReaderSettings.ReadUntypedAsString, !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);
        targetTypeKind = edmTypeReference.TypeKind();
      }
      object payloadItem;
      if (ODataJsonReaderCoreUtils.TryReadNullValue((IJsonReader) this.JsonReader, (ODataInputContext) this.JsonLightInputContext, edmTypeReference, validateNullValue, propertyName, isDynamicProperty))
      {
        if (this.JsonLightInputContext.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata & validateNullValue && edmTypeReference != null && !edmTypeReference.IsNullable)
        {
          if (targetTypeKind == EdmTypeKind.Collection)
          {
            bool? nullable = isDynamicProperty;
            bool flag2 = true;
            if (nullable.GetValueOrDefault() == flag2 & nullable.HasValue)
              goto label_14;
          }
          throw new ODataException(Microsoft.OData.Strings.ReaderValidationUtils_NullNamedValueForNonNullableType((object) propertyName, (object) edmTypeReference.FullName()));
        }
label_14:
        payloadItem = (object) null;
      }
      else
      {
        switch (targetTypeKind)
        {
          case EdmTypeKind.Primitive:
            IEdmPrimitiveTypeReference expectedValueTypeReference1 = edmTypeReference == null ? (IEdmPrimitiveTypeReference) null : edmTypeReference.AsPrimitive();
            if (flag1)
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue((object) "odata.type"));
            payloadItem = this.ReadPrimitiveValue(insideJsonObjectValue, expectedValueTypeReference1, validateNullValue, propertyName);
            break;
          case EdmTypeKind.Entity:
          case EdmTypeKind.Complex:
            IEdmStructuredTypeReference structuredTypeReference = edmTypeReference == null ? (IEdmStructuredTypeReference) null : edmTypeReference.AsStructured();
            payloadItem = (object) this.ReadResourceValue(insideJsonObjectValue, insideResourceValue, propertyName, structuredTypeReference, payloadTypeName, propertyAndAnnotationCollector);
            break;
          case EdmTypeKind.Collection:
            IEdmCollectionTypeReference collectionValueTypeReference = ValidationUtils.ValidateCollectionType(edmTypeReference);
            if (insideJsonObjectValue)
              throw new ODataException(Microsoft.OData.Strings.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName((object) JsonNodeType.StartArray, (object) JsonNodeType.StartObject, (object) propertyName));
            payloadItem = (object) this.ReadCollectionValue(collectionValueTypeReference, payloadTypeName, typeAnnotation);
            break;
          case EdmTypeKind.Enum:
            IEdmEnumTypeReference expectedValueTypeReference2 = edmTypeReference == null ? (IEdmEnumTypeReference) null : edmTypeReference.AsEnum();
            payloadItem = this.ReadEnumValue(insideJsonObjectValue, expectedValueTypeReference2, validateNullValue, propertyName);
            break;
          case EdmTypeKind.TypeDefinition:
            payloadItem = this.ReadTypeDefinitionValue(insideJsonObjectValue, expectedTypeReference.AsTypeDefinition(), validateNullValue, propertyName);
            break;
          case EdmTypeKind.Untyped:
            payloadItem = (object) this.JsonReader.ReadAsUntypedOrNullValue();
            break;
          default:
            throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.ODataJsonLightPropertyAndValueDeserializer_ReadPropertyValue));
        }
        if (collectionValidator != null)
        {
          string payloadTypeName2 = ODataJsonLightReaderUtils.GetPayloadTypeName(payloadItem);
          collectionValidator.ValidateCollectionItem(payloadTypeName2, targetTypeKind);
        }
      }
      return payloadItem;
    }

    private bool TryReadPayloadTypeFromObject(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      bool insideResourceValue,
      out string payloadTypeName)
    {
      bool flag = false;
      payloadTypeName = (string) null;
      if (!insideResourceValue)
        this.JsonReader.ReadStartObject();
      if (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        flag = this.TryReadODataTypeAnnotation(out payloadTypeName);
        if (flag)
          propertyAndAnnotationCollector.MarkPropertyAsProcessed("odata.type");
      }
      return flag;
    }

    private bool ReadingResourceProperty(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector,
      IEdmTypeReference expectedPropertyTypeReference,
      out string payloadTypeName)
    {
      payloadTypeName = (string) null;
      bool flag = false;
      if (expectedPropertyTypeReference != null)
        flag = expectedPropertyTypeReference.IsStructured();
      if (this.JsonReader.NodeType == JsonNodeType.Property && this.TryReadODataTypeAnnotation(out payloadTypeName))
      {
        propertyAndAnnotationCollector.MarkPropertyAsProcessed("odata.type");
        IEdmType expectedType = (IEdmType) null;
        if (expectedPropertyTypeReference != null)
          expectedType = expectedPropertyTypeReference.Definition;
        EdmTypeKind typeKind = EdmTypeKind.None;
        IEdmType type = MetadataUtils.ResolveTypeNameForRead(this.Model, expectedType, payloadTypeName, this.MessageReaderSettings.ClientCustomTypeResolver, out typeKind);
        if (type != null)
          flag = type.IsODataComplexTypeKind() || type.IsODataEntityTypeKind();
      }
      return flag;
    }

    private bool IsTopLevel6xNullValue()
    {
      bool flag1 = this.JsonReader.NodeType == JsonNodeType.Property && string.CompareOrdinal("@odata.null", this.JsonReader.GetPropertyName()) == 0;
      if (flag1)
      {
        int num = (int) this.JsonReader.ReadNext();
        if (!(this.JsonReader.ReadPrimitiveValue() is bool flag2) || !flag2)
          throw new ODataException(Microsoft.OData.Strings.ODataJsonLightReaderUtils_InvalidValueForODataNullAnnotation((object) "odata.null", (object) "true"));
      }
      return flag1;
    }

    private void ValidateNoPropertyInNullPayload(
      PropertyAndAnnotationCollector propertyAndAnnotationCollector)
    {
      Func<string, object> readPropertyAnnotationValue = (Func<string, object>) (annotationName =>
      {
        throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation((object) annotationName));
      });
      while (this.JsonReader.NodeType == JsonNodeType.Property)
        this.ProcessProperty(propertyAndAnnotationCollector, readPropertyAnnotationValue, (Action<ODataJsonLightDeserializer.PropertyParsingResult, string>) ((propertyParsingResult, propertyName) =>
        {
          if (this.JsonReader.NodeType == JsonNodeType.Property)
            this.JsonReader.Read();
          switch (propertyParsingResult)
          {
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties((object) propertyName));
            case ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation:
              this.JsonReader.SkipValue();
              break;
            case ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty:
              throw new ODataException(Microsoft.OData.Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty((object) propertyName));
          }
        }));
    }

    private void IncreaseRecursionDepth() => ValidationUtils.IncreaseAndValidateRecursionDepth(ref this.recursionDepth, this.MessageReaderSettings.MessageQuotas.MaxNestingDepth);

    private void DecreaseRecursionDepth() => --this.recursionDepth;

    [Conditional("DEBUG")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is needed in DEBUG build.")]
    private void AssertRecursionDepthIsZero()
    {
    }
  }
}
