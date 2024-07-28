// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.DefaultODataDeserializerProvider
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using System;
using System.Net.Http;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class DefaultODataDeserializerProvider : ODataDeserializerProvider
  {
    private readonly IServiceProvider _rootContainer;

    public override ODataDeserializer GetODataDeserializer(Type type, HttpRequestMessage request) => this.GetODataDeserializerImpl(type, (Func<IEdmModel>) (() => request.GetModel()));

    public DefaultODataDeserializerProvider(IServiceProvider rootContainer) => this._rootContainer = rootContainer != null ? rootContainer : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (rootContainer));

    public override ODataEdmTypeDeserializer GetEdmTypeDeserializer(IEdmTypeReference edmType)
    {
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      switch (edmType.TypeKind())
      {
        case EdmTypeKind.Primitive:
          return (ODataEdmTypeDeserializer) this._rootContainer.GetRequiredService<ODataPrimitiveDeserializer>();
        case EdmTypeKind.Entity:
        case EdmTypeKind.Complex:
          return (ODataEdmTypeDeserializer) this._rootContainer.GetRequiredService<ODataResourceDeserializer>();
        case EdmTypeKind.Collection:
          IEdmCollectionTypeReference type = edmType.AsCollection();
          return type.ElementType().IsEntity() || type.ElementType().IsComplex() ? (ODataEdmTypeDeserializer) this._rootContainer.GetRequiredService<ODataResourceSetDeserializer>() : (ODataEdmTypeDeserializer) this._rootContainer.GetRequiredService<ODataCollectionDeserializer>();
        case EdmTypeKind.Enum:
          return (ODataEdmTypeDeserializer) this._rootContainer.GetRequiredService<ODataEnumDeserializer>();
        default:
          return (ODataEdmTypeDeserializer) null;
      }
    }

    internal ODataDeserializer GetODataDeserializerImpl(Type type, Func<IEdmModel> modelFunction)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (modelFunction == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelFunction));
      if (type == typeof (Uri))
        return (ODataDeserializer) this._rootContainer.GetRequiredService<ODataEntityReferenceLinkDeserializer>();
      if (type == typeof (ODataActionParameters) || type == typeof (ODataUntypedActionParameters))
        return (ODataDeserializer) this._rootContainer.GetRequiredService<ODataActionPayloadDeserializer>();
      IEdmModel model = modelFunction();
      IEdmTypeReference edmType = model.GetTypeMappingCache().GetEdmType(type, model);
      return edmType == null ? (ODataDeserializer) null : (ODataDeserializer) this.GetEdmTypeDeserializer(edmType);
    }
  }
}
