// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataActionPayloadDeserializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class ODataActionPayloadDeserializer : ODataDeserializer
  {
    private static readonly MethodInfo _castMethodInfo = typeof (Enumerable).GetMethod("Cast");

    public ODataActionPayloadDeserializer(ODataDeserializerProvider deserializerProvider)
      : base(ODataPayloadKind.Parameter)
    {
      this.DeserializerProvider = deserializerProvider != null ? deserializerProvider : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (deserializerProvider));
    }

    public ODataDeserializerProvider DeserializerProvider { get; private set; }

    public override object Read(
      ODataMessageReader messageReader,
      Type type,
      ODataDeserializerContext readContext)
    {
      if (messageReader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageReader));
      IEdmAction action = ODataActionPayloadDeserializer.GetAction(readContext);
      Dictionary<string, object> dictionary = !(type == typeof (ODataActionParameters)) ? (Dictionary<string, object>) new ODataUntypedActionParameters(action) : (Dictionary<string, object>) new ODataActionParameters();
      ODataParameterReader odataParameterReader = messageReader.CreateODataParameterReader((IEdmOperation) action);
      while (odataParameterReader.Read())
      {
        string parameterName = (string) null;
        switch (odataParameterReader.State)
        {
          case ODataParameterReaderState.Value:
            parameterName = odataParameterReader.Name;
            IEdmOperationParameter operationParameter1 = action.Parameters.SingleOrDefault<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => p.Name == parameterName));
            if (operationParameter1.Type.IsPrimitive())
            {
              dictionary[parameterName] = odataParameterReader.Value;
              continue;
            }
            ODataEdmTypeDeserializer typeDeserializer1 = this.DeserializerProvider.GetEdmTypeDeserializer(operationParameter1.Type);
            dictionary[parameterName] = typeDeserializer1.ReadInline(odataParameterReader.Value, operationParameter1.Type, readContext);
            continue;
          case ODataParameterReaderState.Collection:
            parameterName = odataParameterReader.Name;
            IEdmCollectionTypeReference type1 = action.Parameters.SingleOrDefault<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => p.Name == parameterName)).Type as IEdmCollectionTypeReference;
            ODataCollectionValue odataCollectionValue = ODataCollectionDeserializer.ReadCollection(odataParameterReader.CreateCollectionReader());
            ODataCollectionDeserializer typeDeserializer2 = (ODataCollectionDeserializer) this.DeserializerProvider.GetEdmTypeDeserializer((IEdmTypeReference) type1);
            dictionary[parameterName] = typeDeserializer2.ReadInline((object) odataCollectionValue, (IEdmTypeReference) type1, readContext);
            continue;
          case ODataParameterReaderState.Resource:
            parameterName = odataParameterReader.Name;
            IEdmOperationParameter operationParameter2 = action.Parameters.SingleOrDefault<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => p.Name == parameterName));
            object obj1 = (object) odataParameterReader.CreateResourceReader().ReadResourceOrResourceSet();
            ODataResourceDeserializer typeDeserializer3 = (ODataResourceDeserializer) this.DeserializerProvider.GetEdmTypeDeserializer(operationParameter2.Type);
            dictionary[parameterName] = typeDeserializer3.ReadInline(obj1, operationParameter2.Type, readContext);
            continue;
          case ODataParameterReaderState.ResourceSet:
            parameterName = odataParameterReader.Name;
            IEdmCollectionTypeReference type2 = action.Parameters.SingleOrDefault<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => p.Name == parameterName)).Type as IEdmCollectionTypeReference;
            object obj2 = (object) odataParameterReader.CreateResourceSetReader().ReadResourceOrResourceSet();
            object obj3 = this.DeserializerProvider.GetEdmTypeDeserializer((IEdmTypeReference) type2).ReadInline(obj2, (IEdmTypeReference) type2, readContext);
            IEdmTypeReference edmTypeReference = type2.ElementType();
            if (obj3 is IEnumerable enumerable1)
            {
              if (readContext.IsUntyped)
              {
                dictionary[parameterName] = (object) enumerable1.ConvertToEdmObject(type2);
                continue;
              }
              Type clrType = EdmLibHelpers.GetClrType(edmTypeReference, readContext.Model);
              IEnumerable enumerable = ODataActionPayloadDeserializer._castMethodInfo.MakeGenericMethod(clrType).Invoke((object) null, new object[1]
              {
                obj3
              }) as IEnumerable;
              dictionary[parameterName] = (object) enumerable;
              continue;
            }
            continue;
          default:
            continue;
        }
      }
      return (object) dictionary;
    }

    internal static IEdmAction GetAction(ODataDeserializerContext readContext)
    {
      Microsoft.AspNet.OData.Routing.ODataPath odataPath = readContext != null ? readContext.Path : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readContext));
      if (odataPath == null || odataPath.Segments.Count == 0)
        throw new SerializationException(SRResources.ODataPathMissing);
      IEdmAction edmAction = (IEdmAction) null;
      if (odataPath.PathTemplate == "~/unboundaction")
      {
        if (odataPath.Segments.Last<ODataPathSegment>() is OperationImportSegment operationImportSegment && operationImportSegment.OperationImports.First<IEdmOperationImport>() is IEdmActionImport edmActionImport)
          edmAction = edmActionImport.Action;
      }
      else if (odataPath.Segments.Last<ODataPathSegment>() is OperationSegment operationSegment)
        edmAction = operationSegment.Operations.First<IEdmOperation>() as IEdmAction;
      return edmAction != null ? edmAction : throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.RequestNotActionInvocation, (object) odataPath.ToString()));
    }
  }
}
