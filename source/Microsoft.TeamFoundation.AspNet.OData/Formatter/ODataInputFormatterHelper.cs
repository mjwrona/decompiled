// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataInputFormatterHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Formatter
{
  internal static class ODataInputFormatterHelper
  {
    internal static bool CanReadType(
      Type type,
      IEdmModel model,
      ODataPath path,
      IEnumerable<ODataPayloadKind> payloadKinds,
      Func<IEdmTypeReference, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer> getEdmTypeDeserializer,
      Func<Type, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer> getODataPayloadDeserializer)
    {
      if (type == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer deserializer = ODataInputFormatterHelper.GetDeserializer(type, path, model, getEdmTypeDeserializer, getODataPayloadDeserializer, out IEdmTypeReference _);
      return deserializer != null && payloadKinds.Contains<ODataPayloadKind>(deserializer.ODataPayloadKind);
    }

    internal static object ReadFromStream(
      Type type,
      object defaultValue,
      IEdmModel model,
      Uri baseAddress,
      IWebApiRequestMessage internalRequest,
      Func<IODataRequestMessage> getODataRequestMessage,
      Func<IEdmTypeReference, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer> getEdmTypeDeserializer,
      Func<Type, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer> getODataPayloadDeserializer,
      Func<ODataDeserializerContext> getODataDeserializerContext,
      Action<IDisposable> registerForDisposeAction,
      Action<Exception> logErrorAction)
    {
      IEdmTypeReference expectedPayloadType;
      Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer deserializer = ODataInputFormatterHelper.GetDeserializer(type, internalRequest.Context.Path, model, getEdmTypeDeserializer, getODataPayloadDeserializer, out expectedPayloadType);
      if (deserializer == null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (type), SRResources.FormatterReadIsNotSupportedForType, (object) type.FullName, (object) typeof (ODataInputFormatterHelper).FullName);
      try
      {
        ODataMessageReaderSettings readerSettings = internalRequest.ReaderSettings;
        readerSettings.BaseUri = baseAddress;
        readerSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
        ODataMessageReader messageReader = new ODataMessageReader(getODataRequestMessage(), readerSettings, model);
        registerForDisposeAction((IDisposable) messageReader);
        ODataPath path = internalRequest.Context.Path;
        ODataDeserializerContext readContext = getODataDeserializerContext();
        readContext.Path = path;
        readContext.Model = model;
        readContext.ResourceType = type;
        readContext.ResourceEdmType = expectedPayloadType;
        return deserializer.Read(messageReader, type, readContext);
      }
      catch (Exception ex)
      {
        logErrorAction(ex);
        return defaultValue;
      }
    }

    private static Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer GetDeserializer(
      Type type,
      ODataPath path,
      IEdmModel model,
      Func<IEdmTypeReference, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer> getEdmTypeDeserializer,
      Func<Type, Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer> getODataPayloadDeserializer,
      out IEdmTypeReference expectedPayloadType)
    {
      expectedPayloadType = EdmLibHelpers.GetExpectedPayloadType(type, path, model);
      Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializer deserializer = getODataPayloadDeserializer(type);
      if (deserializer == null && expectedPayloadType != null)
        deserializer = getEdmTypeDeserializer(expectedPayloadType);
      return deserializer;
    }
  }
}
