// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataModelBinderConverter
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNet.OData.Formatter
{
  public static class ODataModelBinderConverter
  {
    private static readonly MethodInfo EnumTryParseMethod = ((IEnumerable<MethodInfo>) typeof (Enum).GetMethods()).Single<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "TryParse" && m.GetParameters().Length == 2));
    private static readonly MethodInfo CastMethodInfo = typeof (Enumerable).GetMethod("Cast");

    public static object Convert(
      object graph,
      IEdmTypeReference edmTypeReference,
      Type clrType,
      string parameterName,
      ODataDeserializerContext readContext,
      IServiceProvider requestContainer)
    {
      switch (graph)
      {
        case null:
        case ODataNullValue _:
          return (object) null;
        case ODataCollectionValue collectionValue:
          return ODataModelBinderConverter.ConvertCollection(collectionValue, edmTypeReference, clrType, parameterName, readContext, requestContainer);
        case ODataEnumValue odataEnumValue:
          IEdmEnumTypeReference edmType = edmTypeReference.AsEnum();
          return ServiceProviderServiceExtensions.GetRequiredService<ODataDeserializerProvider>(requestContainer).GetEdmTypeDeserializer((IEdmTypeReference) edmType).ReadInline((object) odataEnumValue, (IEdmTypeReference) edmType, readContext);
        default:
          return edmTypeReference.IsPrimitive() ? EdmPrimitiveHelpers.ConvertPrimitiveValue(graph is ConstantNode constantNode ? constantNode.Value : graph, clrType) : ODataModelBinderConverter.ConvertResourceOrResourceSet(graph, edmTypeReference, readContext);
      }
    }

    internal static object ConvertTo(string valueString, Type type)
    {
      if (valueString == null)
        return (object) null;
      if (TypeHelper.IsNullable(type) && string.Equals(valueString, "null", StringComparison.Ordinal))
        return (object) null;
      if (TypeHelper.IsEnum(type))
      {
        string[] strArray = valueString.Split(new char[1]
        {
          '\''
        }, StringSplitOptions.None);
        if (strArray.Length == 3 && string.IsNullOrEmpty(strArray[2]))
          valueString = strArray[1];
        Type underlyingTypeOrSelf = TypeHelper.GetUnderlyingTypeOrSelf(type);
        object[] parameters = new object[2]
        {
          (object) valueString,
          Enum.ToObject(underlyingTypeOrSelf, 0)
        };
        if (!(bool) ODataModelBinderConverter.EnumTryParseMethod.MakeGenericMethod(underlyingTypeOrSelf).Invoke((object) null, parameters))
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ModelBinderUtil_ValueCannotBeEnum, (object) valueString, (object) type.Name);
        return parameters[1];
      }
      if (!(type == typeof (Date)))
      {
        if (!(type == typeof (Date?)))
        {
          object obj;
          try
          {
            obj = ODataUriUtils.ConvertFromUriLiteral(valueString, ODataVersion.V4);
          }
          catch
          {
            if (type == typeof (string))
              return (object) valueString;
            throw;
          }
          bool isNonstandardEdmPrimitive;
          EdmLibHelpers.IsNonstandardEdmPrimitive(type, out isNonstandardEdmPrimitive);
          if (isNonstandardEdmPrimitive)
            return EdmPrimitiveHelpers.ConvertPrimitiveValue(obj, type);
          Type type1 = Nullable.GetUnderlyingType(type);
          if ((object) type1 == null)
            type1 = type;
          type = type1;
          return System.Convert.ChangeType(obj, type, (IFormatProvider) CultureInfo.InvariantCulture);
        }
      }
      EdmCoreModel instance = EdmCoreModel.Instance;
      IEdmPrimitiveTypeReference typeReferenceOrNull = EdmLibHelpers.GetEdmPrimitiveTypeReferenceOrNull(type);
      return ODataUriUtils.ConvertFromUriLiteral(valueString, ODataVersion.V4, (IEdmModel) instance, (IEdmTypeReference) typeReferenceOrNull);
    }

    private static object ConvertCollection(
      ODataCollectionValue collectionValue,
      IEdmTypeReference edmTypeReference,
      Type clrType,
      string parameterName,
      ODataDeserializerContext readContext,
      IServiceProvider requestContainer)
    {
      IEdmCollectionTypeReference collectionTypeReference = edmTypeReference as IEdmCollectionTypeReference;
      object obj = ServiceProviderServiceExtensions.GetRequiredService<ODataDeserializerProvider>(requestContainer).GetEdmTypeDeserializer((IEdmTypeReference) collectionTypeReference).ReadInline((object) collectionValue, (IEdmTypeReference) collectionTypeReference, readContext);
      if (obj == null)
        return (object) null;
      IEnumerable items = obj as IEnumerable;
      Type elementType;
      if (!TypeHelper.IsCollection(clrType, out elementType))
        throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SRResources.ParameterTypeIsNotCollection, new object[2]
        {
          (object) parameterName,
          (object) clrType
        }));
      IEnumerable instance;
      if (!CollectionDeserializationHelpers.TryCreateInstance(clrType, collectionTypeReference, elementType, out instance))
        return (object) null;
      items.AddToCollection(instance, elementType, parameterName, clrType);
      if (clrType.IsArray)
        instance = CollectionDeserializationHelpers.ToArray(instance, elementType);
      return (object) instance;
    }

    private static object ConvertResourceOrResourceSet(
      object oDataValue,
      IEdmTypeReference edmTypeReference,
      ODataDeserializerContext readContext)
    {
      string str = oDataValue as string;
      if (edmTypeReference.IsNullable && string.Equals(str, "null", StringComparison.Ordinal))
        return (object) null;
      IWebApiRequestMessage internalRequest = readContext.InternalRequest;
      ODataMessageReaderSettings readerSettings = internalRequest.ReaderSettings;
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
      {
        memoryStream.Seek(0L, SeekOrigin.Begin);
        using (ODataMessageReader oDataMessageReader = new ODataMessageReader((IODataRequestMessage) new ODataMessageWrapper((Stream) memoryStream, (Dictionary<string, string>) null, internalRequest.ODataContentIdMapping), readerSettings, readContext.Model))
          return edmTypeReference.IsCollection() ? ODataModelBinderConverter.ConvertResourceSet(oDataMessageReader, edmTypeReference, readContext) : ODataModelBinderConverter.ConvertResource(oDataMessageReader, edmTypeReference, readContext);
      }
    }

    private static object ConvertResourceSet(
      ODataMessageReader oDataMessageReader,
      IEdmTypeReference edmTypeReference,
      ODataDeserializerContext readContext)
    {
      IEdmCollectionTypeReference collectionTypeReference = edmTypeReference.AsCollection();
      EdmEntitySet entitySet = (EdmEntitySet) null;
      if (collectionTypeReference.ElementType().IsEntity())
        entitySet = new EdmEntitySet(readContext.Model.EntityContainer, "temp", collectionTypeReference.ElementType().AsEntity().EntityDefinition());
      ODataResourceSetWrapper resourceSet = oDataMessageReader.CreateODataUriParameterResourceSetReader((IEdmEntitySetBase) entitySet, collectionTypeReference.ElementType().AsStructured().StructuredDefinition()).ReadResourceOrResourceSet() as ODataResourceSetWrapper;
      if (!(readContext.InternalRequest.DeserializerProvider.GetEdmTypeDeserializer((IEdmTypeReference) collectionTypeReference).ReadInline((object) resourceSet, (IEdmTypeReference) collectionTypeReference, readContext) is IEnumerable sources))
        return (object) null;
      IEnumerable enumerable = sources;
      if (collectionTypeReference.ElementType().IsEntity())
        enumerable = ODataModelBinderConverter.CovertResourceSetIds(sources, resourceSet, collectionTypeReference, readContext);
      if (readContext.IsUntyped)
        return (object) enumerable.ConvertToEdmObject(collectionTypeReference);
      Type clrType = EdmLibHelpers.GetClrType(collectionTypeReference.ElementType(), readContext.Model);
      return (object) (ODataModelBinderConverter.CastMethodInfo.MakeGenericMethod(clrType).Invoke((object) null, new object[1]
      {
        (object) enumerable
      }) as IEnumerable);
    }

    private static object ConvertResource(
      ODataMessageReader oDataMessageReader,
      IEdmTypeReference edmTypeReference,
      ODataDeserializerContext readContext)
    {
      EdmEntitySet edmEntitySet = (EdmEntitySet) null;
      if (edmTypeReference.IsEntity())
      {
        IEdmEntityTypeReference type = edmTypeReference.AsEntity();
        edmEntitySet = new EdmEntitySet(readContext.Model.EntityContainer, "temp", type.EntityDefinition());
      }
      ODataResourceWrapper odataResourceWrapper = oDataMessageReader.CreateODataUriParameterResourceReader((IEdmNavigationSource) edmEntitySet, edmTypeReference.ToStructuredType()).ReadResourceOrResourceSet() as ODataResourceWrapper;
      object source = readContext.InternalRequest.DeserializerProvider.GetEdmTypeDeserializer(edmTypeReference).ReadInline((object) odataResourceWrapper, edmTypeReference, readContext);
      if (!edmTypeReference.IsEntity())
        return source;
      IEdmEntityTypeReference entityTypeReference = edmTypeReference.AsEntity();
      return ODataModelBinderConverter.CovertResourceId(source, odataResourceWrapper.Resource, entityTypeReference, readContext);
    }

    private static IEnumerable CovertResourceSetIds(
      IEnumerable sources,
      ODataResourceSetWrapper resourceSet,
      IEdmCollectionTypeReference collectionType,
      ODataDeserializerContext readContext)
    {
      IEdmEntityTypeReference entityTypeReference = collectionType.ElementType().AsEntity();
      int i = 0;
      foreach (object source in sources)
      {
        object obj = ODataModelBinderConverter.CovertResourceId(source, resourceSet.Resources[i].Resource, entityTypeReference, readContext);
        ++i;
        yield return obj;
      }
    }

    private static object CovertResourceId(
      object source,
      ODataResource resource,
      IEdmEntityTypeReference entityTypeReference,
      ODataDeserializerContext readContext)
    {
      if (resource.Id == (Uri) null || resource.Properties.Any<ODataProperty>())
        return source;
      IWebApiRequestMessage internalRequest = readContext.InternalRequest;
      IWebApiUrlHelper internalUrlHelper = readContext.InternalUrlHelper;
      DefaultODataPathHandler pathHandler1 = new DefaultODataPathHandler();
      string routeName = internalRequest.Context.RouteName;
      IODataPathHandler pathHandler2 = internalRequest.PathHandler;
      List<ODataPathSegment> segments = new List<ODataPathSegment>();
      string odataLink = internalUrlHelper.CreateODataLink(routeName, pathHandler2, (IList<ODataPathSegment>) segments);
      IEnumerable<KeyValuePair<string, object>> keys = ODataModelBinderConverter.GetKeys(pathHandler1, odataLink, resource.Id, internalRequest.RequestContainer);
      IList<IEdmStructuralProperty> list = (IList<IEdmStructuralProperty>) entityTypeReference.Key().ToList<IEdmStructuralProperty>();
      if (list.Count == 1 && keys.Count<KeyValuePair<string, object>>() == 1)
      {
        object propertyValue = keys.First<KeyValuePair<string, object>>().Value;
        DeserializationHelpers.SetDeclaredProperty(source, EdmTypeKind.Primitive, list[0].Name, propertyValue, (IEdmProperty) list[0], readContext);
        return source;
      }
      IDictionary<string, object> dictionary = (IDictionary<string, object>) keys.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (e => e.Key), (Func<KeyValuePair<string, object>, object>) (e => e.Value));
      foreach (IEdmStructuralProperty structuralProperty in (IEnumerable<IEdmStructuralProperty>) list)
      {
        object propertyValue;
        if (dictionary.TryGetValue(structuralProperty.Name, out propertyValue))
          DeserializationHelpers.SetDeclaredProperty(source, EdmTypeKind.Primitive, structuralProperty.Name, propertyValue, (IEdmProperty) structuralProperty, readContext);
      }
      return source;
    }

    private static IEnumerable<KeyValuePair<string, object>> GetKeys(
      DefaultODataPathHandler pathHandler,
      string serviceRoot,
      Uri uri,
      IServiceProvider requestContainer)
    {
      return (pathHandler.Parse(serviceRoot, uri.ToString(), requestContainer).Segments.OfType<KeySegment>().Last<KeySegment>() ?? throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EntityReferenceMustHasKeySegment, (object) uri)).Keys;
    }
  }
}
