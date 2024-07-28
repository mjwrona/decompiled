// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataCollectionSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataCollectionSerializer : ODataEdmTypeSerializer
  {
    public ODataCollectionSerializer(ODataSerializerProvider serializerProvider)
      : base(ODataPayloadKind.Collection, serializerProvider)
    {
    }

    public override void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      if (messageWriter == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageWriter));
      if (writeContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      IEdmTypeReference edmType = writeContext.GetEdmType(graph, type);
      IEdmTypeReference elementType = ODataCollectionSerializer.GetElementType(edmType);
      this.WriteCollection(messageWriter.CreateODataCollectionWriter(elementType), graph, (IEdmTypeReference) edmType.AsCollection(), writeContext);
    }

    public override sealed ODataValue CreateODataValue(
      object graph,
      IEdmTypeReference expectedType,
      ODataSerializerContext writeContext)
    {
      switch (graph)
      {
        case IEnumerable enumerable:
        case null:
          IEdmTypeReference elementType = expectedType != null ? ODataCollectionSerializer.GetElementType(expectedType) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (expectedType));
          return (ODataValue) this.CreateODataCollectionValue(enumerable, elementType, writeContext);
        default:
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (graph), SRResources.ArgumentMustBeOfType, (object) typeof (IEnumerable).Name);
      }
    }

    public virtual void WriteCollection(
      ODataCollectionWriter writer,
      object graph,
      IEdmTypeReference collectionType,
      ODataSerializerContext writeContext)
    {
      if (writer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writer));
      ODataCollectionStart collectionStart = new ODataCollectionStart()
      {
        Name = writeContext.RootElementName
      };
      if (writeContext.Request != null)
      {
        if (writeContext.InternalRequest.Context.NextLink != (Uri) null)
          collectionStart.NextPageLink = writeContext.InternalRequest.Context.NextLink;
        else if (writeContext.InternalRequest.Context.QueryOptions != null)
        {
          SkipTokenHandler skipTokenHandler = writeContext.QueryOptions.Context.GetSkipTokenHandler();
          collectionStart.NextPageLink = skipTokenHandler.GenerateNextPageLink(writeContext.InternalRequest.RequestUri, writeContext.InternalRequest.Context.PageSize, (object) null, writeContext);
        }
        if (writeContext.InternalRequest.Context.TotalCount.HasValue)
          collectionStart.Count = writeContext.InternalRequest.Context.TotalCount;
      }
      writer.WriteStart(collectionStart);
      if (graph != null && this.CreateODataValue(graph, collectionType, writeContext) is ODataCollectionValue odataValue)
      {
        foreach (object obj in odataValue.Items)
          writer.WriteItem(obj);
      }
      writer.WriteEnd();
    }

    public virtual ODataCollectionValue CreateODataCollectionValue(
      IEnumerable enumerable,
      IEdmTypeReference elementType,
      ODataSerializerContext writeContext)
    {
      if (writeContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      if (elementType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (elementType));
      ArrayList source = new ArrayList();
      if (enumerable != null)
      {
        ODataEdmTypeSerializer edmTypeSerializer = (ODataEdmTypeSerializer) null;
        foreach (object obj in enumerable)
        {
          if (obj == null)
          {
            if (!elementType.IsNullable)
              throw new SerializationException(SRResources.NullElementInCollection);
            source.Add((object) null);
          }
          else
          {
            IEdmTypeReference edmType = writeContext.GetEdmType(obj, obj.GetType());
            edmTypeSerializer = edmTypeSerializer ?? this.SerializerProvider.GetEdmTypeSerializer(edmType);
            if (edmTypeSerializer == null)
              throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) edmType.FullName()));
            source.Add(edmTypeSerializer.CreateODataValue(obj, edmType, writeContext).GetInnerValue());
          }
        }
      }
      string str = "Collection(" + elementType.FullName() + ")";
      ODataCollectionValue odataCollectionValue = new ODataCollectionValue();
      odataCollectionValue.Items = source.Cast<object>();
      odataCollectionValue.TypeName = str;
      ODataCollectionSerializer.AddTypeNameAnnotationAsNeeded(odataCollectionValue, writeContext.MetadataLevel);
      return odataCollectionValue;
    }

    internal override ODataProperty CreateProperty(
      object graph,
      IEdmTypeReference expectedType,
      string elementName,
      ODataSerializerContext writeContext)
    {
      ODataValue odataValue = this.CreateODataValue(graph, expectedType, writeContext);
      if (odataValue == null)
        return (ODataProperty) null;
      ODataProperty property = new ODataProperty();
      property.Name = elementName;
      property.Value = (object) odataValue;
      return property;
    }

    protected internal static void AddTypeNameAnnotationAsNeeded(
      ODataCollectionValue value,
      ODataMetadataLevel metadataLevel)
    {
      if (!ODataCollectionSerializer.ShouldAddTypeNameAnnotation(metadataLevel))
        return;
      string typeName = !ODataCollectionSerializer.ShouldSuppressTypeNameSerialization(metadataLevel) ? value.TypeName : (string) null;
      value.TypeAnnotation = new ODataTypeAnnotation(typeName);
    }

    internal static bool ShouldAddTypeNameAnnotation(ODataMetadataLevel metadataLevel)
    {
      switch (metadataLevel)
      {
        case ODataMetadataLevel.MinimalMetadata:
          return false;
        default:
          return true;
      }
    }

    internal static bool ShouldSuppressTypeNameSerialization(ODataMetadataLevel metadataLevel) => metadataLevel != ODataMetadataLevel.FullMetadata && metadataLevel == ODataMetadataLevel.NoMetadata;

    private static IEdmTypeReference GetElementType(IEdmTypeReference feedType) => feedType.IsCollection() ? feedType.AsCollection().ElementType() : throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) typeof (ODataResourceSetSerializer).Name, (object) feedType.FullName()));
  }
}
