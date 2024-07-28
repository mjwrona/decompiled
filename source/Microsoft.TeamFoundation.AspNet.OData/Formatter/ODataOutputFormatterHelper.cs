// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataOutputFormatterHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter
{
  internal static class ODataOutputFormatterHelper
  {
    internal static bool TryGetContentHeader(
      Type type,
      MediaTypeHeaderValue mediaType,
      out MediaTypeHeaderValue newMediaType)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      newMediaType = (MediaTypeHeaderValue) null;
      if (mediaType == null)
        return false;
      if (mediaType.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase) && !mediaType.Parameters.Any<NameValueHeaderValue>((Func<NameValueHeaderValue, bool>) (p => p.Name.Equals("odata.metadata", StringComparison.OrdinalIgnoreCase))))
        mediaType.Parameters.Add(new NameValueHeaderValue("odata.metadata", "minimal"));
      newMediaType = (MediaTypeHeaderValue) ((ICloneable) mediaType).Clone();
      return true;
    }

    internal static bool TryGetCharSet(
      MediaTypeHeaderValue mediaType,
      IEnumerable<string> acceptCharsetValues,
      out string charSet)
    {
      charSet = string.Empty;
      return mediaType != null && !acceptCharsetValues.Any<string>((Func<string, bool>) (cs => cs.Equals(mediaType.CharSet, StringComparison.OrdinalIgnoreCase)));
    }

    internal static bool CanWriteType(
      Type type,
      IEnumerable<ODataPayloadKind> payloadKinds,
      bool isGenericSingleResult,
      IWebApiRequestMessage internalRequest,
      Func<Type, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer> getODataPayloadSerializer)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      Type elementType;
      ODataPayloadKind? nullable = typeof (IEdmObject).IsAssignableFrom(type) || TypeHelper.IsCollection(type, out elementType) && typeof (IEdmObject).IsAssignableFrom(elementType) ? ODataOutputFormatterHelper.GetEdmObjectPayloadKind(type, internalRequest) : ODataOutputFormatterHelper.GetClrObjectResponsePayloadKind(type, isGenericSingleResult, getODataPayloadSerializer);
      return nullable.HasValue && payloadKinds.Contains<ODataPayloadKind>(nullable.Value);
    }

    internal static void WriteToStream(
      Type type,
      object value,
      IEdmModel model,
      ODataVersion version,
      Uri baseAddress,
      MediaTypeHeaderValue contentType,
      IWebApiUrlHelper internaUrlHelper,
      IWebApiRequestMessage internalRequest,
      IWebApiHeaders internalRequestHeaders,
      Func<IServiceProvider, ODataMessageWrapper> getODataMessageWrapper,
      Func<IEdmTypeReference, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer> getEdmTypeSerializer,
      Func<Type, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer> getODataPayloadSerializer,
      Func<ODataSerializerContext> getODataSerializerContext)
    {
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestMustHaveModel);
      if (value is IEdmStructuredObject structuredObject)
        structuredObject.SetModel(model);
      Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer serializer = ODataOutputFormatterHelper.GetSerializer(type, value, internalRequest, getEdmTypeSerializer, getODataPayloadSerializer);
      Microsoft.AspNet.OData.Routing.ODataPath path = internalRequest.Context.Path;
      IEdmNavigationSource navigationSource = path == null ? (IEdmNavigationSource) null : path.NavigationSource;
      IODataResponseMessage responseMessage = ODataOutputFormatterHelper.PrepareResponseMessage(internalRequest, internalRequestHeaders, getODataMessageWrapper);
      ODataMessageWriterSettings writerSettings = internalRequest.WriterSettings;
      writerSettings.BaseUri = baseAddress;
      writerSettings.Version = new ODataVersion?(version);
      writerSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
      if (internaUrlHelper.CreateODataLink((ODataPathSegment) MetadataSegment.Instance) == null)
        throw new SerializationException(SRResources.UnableToDetermineMetadataUrl);
      SelectExpandClause selectExpandClause = (SelectExpandClause) null;
      if (internalRequest.Context.QueryOptions != null && internalRequest.Context.QueryOptions.SelectExpand != null)
      {
        if (internalRequest.Context.QueryOptions.SelectExpand.ProcessedSelectExpandClause != internalRequest.Context.ProcessedSelectExpandClause)
          selectExpandClause = internalRequest.Context.ProcessedSelectExpandClause;
      }
      else if (internalRequest.Context.ProcessedSelectExpandClause != null)
        selectExpandClause = internalRequest.Context.ProcessedSelectExpandClause;
      writerSettings.ODataUri = new ODataUri()
      {
        ServiceRoot = baseAddress,
        SelectAndExpand = internalRequest.Context.ProcessedSelectExpandClause,
        Apply = internalRequest.Context.ApplyClause,
        Path = path == null || ODataOutputFormatterHelper.IsOperationPath(path) ? (Microsoft.OData.UriParser.ODataPath) null : path.Path
      };
      ODataMetadataLevel odataMetadataLevel = ODataMetadataLevel.MinimalMetadata;
      if (contentType != null)
      {
        IEnumerable<KeyValuePair<string, string>> parameters = contentType.Parameters.Select<NameValueHeaderValue, KeyValuePair<string, string>>((Func<NameValueHeaderValue, KeyValuePair<string, string>>) (val => new KeyValuePair<string, string>(val.Name, val.Value)));
        odataMetadataLevel = ODataMediaTypes.GetMetadataLevel(contentType.MediaType, parameters);
      }
      using (ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage, writerSettings, model))
      {
        ODataSerializerContext writeContext = getODataSerializerContext();
        writeContext.NavigationSource = navigationSource;
        writeContext.Model = model;
        writeContext.RootElementName = ODataOutputFormatterHelper.GetRootElementName(path) ?? "root";
        writeContext.SkipExpensiveAvailabilityChecks = serializer.ODataPayloadKind == ODataPayloadKind.ResourceSet;
        writeContext.Path = path;
        writeContext.MetadataLevel = odataMetadataLevel;
        writeContext.QueryOptions = internalRequest.Context.QueryOptions;
        if (selectExpandClause != null)
          writeContext.SelectExpandClause = selectExpandClause;
        serializer.WriteObject(value, type, messageWriter, writeContext);
      }
    }

    internal static IODataResponseMessage PrepareResponseMessage(
      IWebApiRequestMessage internalRequest,
      IWebApiHeaders internalRequestHeaders,
      Func<IServiceProvider, ODataMessageWrapper> getODataMessageWrapper)
    {
      string requestPreferHeader = RequestPreferenceHelpers.GetRequestPreferHeader(internalRequestHeaders);
      string str1 = (string) null;
      string str2 = (string) null;
      int? nullable = new int?();
      if (!string.IsNullOrEmpty(requestPreferHeader))
      {
        ODataMessageWrapper requestMessage = getODataMessageWrapper((IServiceProvider) null);
        requestMessage.SetHeader("Prefer", requestPreferHeader);
        str1 = requestMessage.PreferHeader().AnnotationFilter;
        str2 = requestMessage.PreferHeader().OmitValues;
        nullable = requestMessage.PreferHeader().MaxPageSize;
      }
      IODataResponseMessage responseMessage = (IODataResponseMessage) getODataMessageWrapper(internalRequest.RequestContainer);
      if (str1 != null)
        responseMessage.PreferenceAppliedHeader().AnnotationFilter = str1;
      if (str2 != null)
        responseMessage.PreferenceAppliedHeader().OmitValues = str2;
      if (nullable.HasValue)
        responseMessage.PreferenceAppliedHeader().MaxPageSize = nullable;
      return responseMessage;
    }

    private static ODataPayloadKind? GetClrObjectResponsePayloadKind(
      Type type,
      bool isGenericSingleResult,
      Func<Type, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer> getODataPayloadSerializer)
    {
      if (isGenericSingleResult)
        type = type.GetGenericArguments()[0];
      return getODataPayloadSerializer(type)?.ODataPayloadKind;
    }

    private static ODataPayloadKind? GetEdmObjectPayloadKind(
      Type type,
      IWebApiRequestMessage internalRequest)
    {
      if (internalRequest.IsCountRequest())
        return new ODataPayloadKind?(ODataPayloadKind.Value);
      Type elementType;
      if (TypeHelper.IsCollection(type, out elementType))
      {
        if (typeof (IEdmComplexObject).IsAssignableFrom(elementType) || typeof (IEdmEnumObject).IsAssignableFrom(elementType))
          return new ODataPayloadKind?(ODataPayloadKind.Collection);
        if (typeof (IEdmEntityObject).IsAssignableFrom(elementType))
          return new ODataPayloadKind?(ODataPayloadKind.ResourceSet);
        if (typeof (IEdmChangedObject).IsAssignableFrom(elementType))
          return new ODataPayloadKind?(ODataPayloadKind.Delta);
      }
      else
      {
        if (typeof (IEdmComplexObject).IsAssignableFrom(elementType) || typeof (IEdmEnumObject).IsAssignableFrom(elementType))
          return new ODataPayloadKind?(ODataPayloadKind.Property);
        if (typeof (IEdmEntityObject).IsAssignableFrom(elementType))
          return new ODataPayloadKind?(ODataPayloadKind.Resource);
      }
      return new ODataPayloadKind?();
    }

    private static Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer GetSerializer(
      Type type,
      object value,
      IWebApiRequestMessage internalRequest,
      Func<IEdmTypeReference, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer> getEdmTypeSerializer,
      Func<Type, Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer> getODataPayloadSerializer)
    {
      Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializer serializer;
      if (value is IEdmObject edmObject)
      {
        IEdmTypeReference edmType = edmObject.GetEdmType();
        if (edmType == null)
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.EdmTypeCannotBeNull, (object) edmObject.GetType().FullName, (object) typeof (IEdmObject).Name));
        serializer = getEdmTypeSerializer(edmType);
        if (serializer == null)
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) edmType.ToTraceString()));
      }
      else
      {
        if (internalRequest.Context.ApplyClause == null)
          type = value == null ? type : value.GetType();
        serializer = getODataPayloadSerializer(type);
        if (serializer == null)
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) type.Name));
      }
      return serializer;
    }

    private static string GetRootElementName(Microsoft.AspNet.OData.Routing.ODataPath path)
    {
      if (path != null)
      {
        switch (path.Segments.LastOrDefault<ODataPathSegment>())
        {
          case OperationSegment operationSegment when operationSegment.Operations.Single<IEdmOperation>() is IEdmAction edmAction:
            return edmAction.Name;
          case PropertySegment propertySegment:
            return propertySegment.Property.Name;
        }
      }
      return (string) null;
    }

    private static bool IsOperationPath(Microsoft.AspNet.OData.Routing.ODataPath path)
    {
      if (path == null)
        return false;
      foreach (ODataPathSegment segment in path.Segments)
      {
        if (segment is OperationSegment || segment is OperationImportSegment)
          return true;
      }
      return false;
    }
  }
}
