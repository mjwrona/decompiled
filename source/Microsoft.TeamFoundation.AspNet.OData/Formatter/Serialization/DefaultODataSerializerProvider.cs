// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.DefaultODataSerializerProvider
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class DefaultODataSerializerProvider : ODataSerializerProvider
  {
    private readonly IServiceProvider _rootContainer;

    public override ODataSerializer GetODataPayloadSerializer(Type type, HttpRequestMessage request) => this.GetODataPayloadSerializerImpl(type, (Func<IEdmModel>) (() => request.GetModel()), request.ODataProperties().Path, typeof (HttpError));

    public DefaultODataSerializerProvider(IServiceProvider rootContainer) => this._rootContainer = rootContainer != null ? rootContainer : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (rootContainer));

    public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
    {
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      switch (edmType.TypeKind())
      {
        case EdmTypeKind.Primitive:
          return (ODataEdmTypeSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataPrimitiveSerializer>(this._rootContainer);
        case EdmTypeKind.Entity:
        case EdmTypeKind.Complex:
          return (ODataEdmTypeSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataResourceSerializer>(this._rootContainer);
        case EdmTypeKind.Collection:
          IEdmCollectionTypeReference type = edmType.AsCollection();
          if (type.Definition.IsDeltaFeed())
            return (ODataEdmTypeSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataDeltaFeedSerializer>(this._rootContainer);
          return type.ElementType().IsEntity() || type.ElementType().IsComplex() ? (ODataEdmTypeSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataResourceSetSerializer>(this._rootContainer) : (ODataEdmTypeSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataCollectionSerializer>(this._rootContainer);
        case EdmTypeKind.Enum:
          return (ODataEdmTypeSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataEnumSerializer>(this._rootContainer);
        default:
          return (ODataEdmTypeSerializer) null;
      }
    }

    internal ODataSerializer GetODataPayloadSerializerImpl(
      Type type,
      Func<IEdmModel> modelFunction,
      Microsoft.AspNet.OData.Routing.ODataPath path,
      Type errorType)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (modelFunction == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelFunction));
      if (type == typeof (ODataServiceDocument))
        return (ODataSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataServiceDocumentSerializer>(this._rootContainer);
      if (type == typeof (Uri) || type == typeof (ODataEntityReferenceLink))
        return (ODataSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataEntityReferenceLinkSerializer>(this._rootContainer);
      if (TypeHelper.IsTypeAssignableFrom(typeof (IEnumerable<Uri>), type) || type == typeof (ODataEntityReferenceLinks))
        return (ODataSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataEntityReferenceLinksSerializer>(this._rootContainer);
      if (type == typeof (ODataError) || type == errorType)
        return (ODataSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataErrorSerializer>(this._rootContainer);
      if (TypeHelper.IsTypeAssignableFrom(typeof (IEdmModel), type))
        return (ODataSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataMetadataSerializer>(this._rootContainer);
      IEdmModel model = modelFunction();
      IEdmTypeReference edmType = model.GetTypeMappingCache().GetEdmType(type, model);
      if (edmType == null)
        return (ODataSerializer) null;
      bool flag1 = path != null && path.Segments.LastOrDefault<ODataPathSegment>() is CountSegment;
      bool flag2 = path != null && path.Segments.LastOrDefault<ODataPathSegment>() is ValueSegment;
      return ((edmType.IsPrimitive() ? 1 : (edmType.IsEnum() ? 1 : 0)) & (flag2 ? 1 : 0) | (flag1 ? 1 : 0)) != 0 ? (ODataSerializer) ServiceProviderServiceExtensions.GetRequiredService<ODataRawValueSerializer>(this._rootContainer) : (ODataSerializer) this.GetEdmTypeSerializer(edmType);
    }
  }
}
