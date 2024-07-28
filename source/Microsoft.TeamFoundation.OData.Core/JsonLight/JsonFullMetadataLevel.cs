// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.JsonFullMetadataLevel
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.JsonLight
{
  internal sealed class JsonFullMetadataLevel : JsonLightMetadataLevel
  {
    private readonly IEdmModel model;
    private readonly Uri metadataDocumentUri;

    internal JsonFullMetadataLevel(Uri metadataDocumentUri, IEdmModel model)
    {
      this.metadataDocumentUri = metadataDocumentUri;
      this.model = model;
    }

    private Uri NonNullMetadataDocumentUri => !(this.metadataDocumentUri == (Uri) null) ? this.metadataDocumentUri : throw new ODataException(Microsoft.OData.Strings.ODataOutputContext_MetadataDocumentUriMissing);

    internal override JsonLightTypeNameOracle GetTypeNameOracle() => (JsonLightTypeNameOracle) new JsonFullMetadataTypeNameOracle();

    internal override ODataResourceMetadataBuilder CreateResourceMetadataBuilder(
      ODataResourceBase resource,
      IODataResourceTypeContext typeContext,
      ODataResourceSerializationInfo serializationInfo,
      IEdmStructuredType actualResourceType,
      SelectedPropertiesNode selectedProperties,
      bool isResponse,
      bool keyAsSegment,
      ODataUri odataUri,
      ODataMessageWriterSettings settings)
    {
      IODataMetadataContext metadataContext = (IODataMetadataContext) new ODataMetadataContext(isResponse, this.model, this.NonNullMetadataDocumentUri, odataUri);
      ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(metadataContext.ServiceBaseUri, keyAsSegment ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses);
      IODataResourceMetadataContext resourceMetadataContext = (IODataResourceMetadataContext) ODataResourceMetadataContext.Create(resource, typeContext, serializationInfo, actualResourceType, metadataContext, selectedProperties, settings?.MetadataSelector);
      return actualResourceType != null && actualResourceType.TypeKind == EdmTypeKind.Entity || actualResourceType == null && typeContext.NavigationSourceKind != EdmNavigationSourceKind.None ? (ODataResourceMetadataBuilder) new ODataConventionalEntityMetadataBuilder(resourceMetadataContext, metadataContext, (ODataUriBuilder) uriBuilder) : (ODataResourceMetadataBuilder) new ODataConventionalResourceMetadataBuilder(resourceMetadataContext, metadataContext, (ODataUriBuilder) uriBuilder);
    }

    internal override void InjectMetadataBuilder(
      ODataResourceBase resource,
      ODataResourceMetadataBuilder builder)
    {
      base.InjectMetadataBuilder(resource, builder);
      resource.NonComputedMediaResource?.SetMetadataBuilder(builder, (string) null);
      if (resource.NonComputedProperties != null)
      {
        foreach (ODataProperty computedProperty in resource.NonComputedProperties)
        {
          if (computedProperty.ODataValue is ODataStreamReferenceValue odataValue)
            odataValue.SetMetadataBuilder(builder, computedProperty.Name);
        }
      }
      IEnumerable<ODataOperation> odataOperations = ODataUtilsInternal.ConcatEnumerables<ODataOperation>((IEnumerable<ODataOperation>) resource.NonComputedActions, (IEnumerable<ODataOperation>) resource.NonComputedFunctions);
      if (odataOperations == null)
        return;
      foreach (ODataOperation odataOperation in odataOperations)
        odataOperation.SetMetadataBuilder(builder, this.NonNullMetadataDocumentUri);
    }
  }
}
