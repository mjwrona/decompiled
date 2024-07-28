// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataMetadataContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Evaluation
{
  internal sealed class ODataMetadataContext : IODataMetadataContext
  {
    private readonly IEdmModel model;
    private readonly EdmTypeResolver edmTypeResolver;
    private readonly Dictionary<IEdmType, IList<IEdmOperation>> bindableOperationsCache;
    private readonly bool isResponse;
    private readonly Func<IEdmStructuredType, bool> operationsBoundToStructuredTypeMustBeContainerQualified;
    private readonly Uri metadataDocumentUri;
    private readonly ODataUri odataUri;
    private Uri serviceBaseUri;
    private JsonLightMetadataLevel metadataLevel;

    public ODataMetadataContext(
      bool isResponse,
      IEdmModel model,
      Uri metadataDocumentUri,
      ODataUri odataUri)
      : this(isResponse, (Func<IEdmStructuredType, bool>) null, (EdmTypeResolver) EdmTypeWriterResolver.Instance, model, metadataDocumentUri, odataUri)
    {
    }

    public ODataMetadataContext(
      bool isResponse,
      Func<IEdmStructuredType, bool> operationsBoundToStructuredTypeMustBeContainerQualified,
      EdmTypeResolver edmTypeResolver,
      IEdmModel model,
      Uri metadataDocumentUri,
      ODataUri odataUri)
    {
      this.isResponse = isResponse;
      this.operationsBoundToStructuredTypeMustBeContainerQualified = operationsBoundToStructuredTypeMustBeContainerQualified ?? new Func<IEdmStructuredType, bool>(EdmLibraryExtensions.OperationsBoundToStructuredTypeMustBeContainerQualified);
      this.edmTypeResolver = edmTypeResolver;
      this.model = model;
      this.metadataDocumentUri = metadataDocumentUri;
      this.bindableOperationsCache = new Dictionary<IEdmType, IList<IEdmOperation>>((IEqualityComparer<IEdmType>) ReferenceEqualityComparer<IEdmType>.Instance);
      this.odataUri = odataUri;
    }

    public ODataMetadataContext(
      bool isResponse,
      Func<IEdmStructuredType, bool> operationsBoundToEntityTypeMustBeContainerQualified,
      EdmTypeResolver edmTypeResolver,
      IEdmModel model,
      Uri metadataDocumentUri,
      ODataUri odataUri,
      JsonLightMetadataLevel metadataLevel)
      : this(isResponse, operationsBoundToEntityTypeMustBeContainerQualified, edmTypeResolver, model, metadataDocumentUri, odataUri)
    {
      this.metadataLevel = metadataLevel;
    }

    public IEdmModel Model => this.model;

    public Uri ServiceBaseUri
    {
      get
      {
        Uri serviceBaseUri = this.serviceBaseUri;
        return (object) serviceBaseUri != null ? serviceBaseUri : (this.serviceBaseUri = new Uri(this.MetadataDocumentUri, "./"));
      }
    }

    public Uri MetadataDocumentUri => !(this.metadataDocumentUri == (Uri) null) ? this.metadataDocumentUri : throw new ODataException(Microsoft.OData.Strings.ODataJsonLightResourceMetadataContext_MetadataAnnotationMustBeInPayload((object) "odata.context"));

    public ODataUri ODataUri => this.odataUri;

    public ODataResourceMetadataBuilder GetResourceMetadataBuilderForReader(
      IODataJsonLightReaderResourceState resourceState,
      bool useKeyAsSegment,
      bool isDelta = false)
    {
      if (resourceState.MetadataBuilder == null)
      {
        ODataResourceBase resource = resourceState.Resource;
        if (this.isResponse && !isDelta)
        {
          ODataTypeAnnotation typeAnnotation = resource.TypeAnnotation;
          IEdmStructuredType edmStructuredType = (IEdmStructuredType) null;
          if (typeAnnotation != null)
          {
            if (typeAnnotation.Type != null)
              edmStructuredType = typeAnnotation.Type as IEdmStructuredType;
            else if (typeAnnotation.TypeName != null)
              edmStructuredType = this.model.FindType(typeAnnotation.TypeName) as IEdmStructuredType;
          }
          if (edmStructuredType == null)
            edmStructuredType = resourceState.ResourceType;
          IEdmNavigationSource navigationSource = resourceState.NavigationSource;
          IEdmEntityType elementType = this.edmTypeResolver.GetElementType(navigationSource);
          IODataResourceTypeContext typeContext = (IODataResourceTypeContext) ODataResourceTypeContext.Create((ODataResourceSerializationInfo) null, navigationSource, elementType, resourceState.ResourceTypeFromMetadata ?? resourceState.ResourceType, true);
          IODataResourceMetadataContext resourceMetadataContext = (IODataResourceMetadataContext) ODataResourceMetadataContext.Create(resource, typeContext, (ODataResourceSerializationInfo) null, edmStructuredType, (IODataMetadataContext) this, resourceState.SelectedProperties, (ODataMetadataSelector) null);
          ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(this.ServiceBaseUri, useKeyAsSegment ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses);
          resourceState.MetadataBuilder = !edmStructuredType.IsODataEntityTypeKind() ? (ODataResourceMetadataBuilder) new ODataConventionalResourceMetadataBuilder(resourceMetadataContext, (IODataMetadataContext) this, (ODataUriBuilder) uriBuilder) : (ODataResourceMetadataBuilder) new ODataConventionalEntityMetadataBuilder(resourceMetadataContext, (IODataMetadataContext) this, (ODataUriBuilder) uriBuilder);
        }
        else
          resourceState.MetadataBuilder = (ODataResourceMetadataBuilder) new NoOpResourceMetadataBuilder(resource);
      }
      return resourceState.MetadataBuilder;
    }

    public IEnumerable<IEdmOperation> GetBindableOperationsForType(IEdmType bindingType)
    {
      IList<IEdmOperation> operationsForType;
      if (!this.bindableOperationsCache.TryGetValue(bindingType, out operationsForType))
      {
        operationsForType = MetadataUtils.CalculateBindableOperationsForType(bindingType, this.model, this.edmTypeResolver);
        this.bindableOperationsCache.Add(bindingType, operationsForType);
      }
      return (IEnumerable<IEdmOperation>) operationsForType;
    }

    public bool OperationsBoundToStructuredTypeMustBeContainerQualified(
      IEdmStructuredType structuredType)
    {
      return this.operationsBoundToStructuredTypeMustBeContainerQualified(structuredType);
    }
  }
}
