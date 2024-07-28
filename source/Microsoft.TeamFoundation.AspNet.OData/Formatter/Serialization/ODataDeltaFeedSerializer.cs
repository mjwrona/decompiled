// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataDeltaFeedSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataDeltaFeedSerializer : ODataEdmTypeSerializer
  {
    private const string DeltaFeed = "deltafeed";

    public ODataDeltaFeedSerializer(ODataSerializerProvider serializerProvider)
      : base(ODataPayloadKind.Delta, serializerProvider)
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
      if (graph == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotSerializerNull, (object) "deltafeed"));
      if (!(writeContext.NavigationSource is IEdmEntitySetBase navigationSource))
        throw new SerializationException(SRResources.EntitySetMissingDuringSerialization);
      IEdmTypeReference edmType = writeContext.GetEdmType(graph, type);
      IEdmEntityTypeReference type1 = ODataDeltaFeedSerializer.GetResourceType(edmType).AsEntity();
      ODataWriter resourceSetWriter = messageWriter.CreateODataDeltaResourceSetWriter(navigationSource, (IEdmStructuredType) type1.EntityDefinition());
      this.WriteDeltaFeedInline(graph, edmType, resourceSetWriter, writeContext);
    }

    public virtual void WriteDeltaFeedInline(
      object graph,
      IEdmTypeReference expectedType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      if (writer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writer));
      if (writeContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      if (expectedType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (expectedType));
      if (graph == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotSerializerNull, (object) "deltafeed"));
      if (!(graph is IEnumerable enumerable))
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) this.GetType().Name, (object) graph.GetType().FullName));
      this.WriteFeed(enumerable, expectedType, writer, writeContext);
    }

    private void WriteFeed(
      IEnumerable enumerable,
      IEdmTypeReference feedType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      IEdmStructuredTypeReference resourceType = ODataDeltaFeedSerializer.GetResourceType(feedType);
      if (resourceType.IsComplex())
      {
        ODataResourceSet odataResourceSet = new ODataResourceSet();
        odataResourceSet.TypeName = feedType.FullName();
        ODataResourceSet resourceSet = odataResourceSet;
        writer.WriteStart(resourceSet);
        if (!(this.SerializerProvider.GetEdmTypeSerializer((IEdmTypeReference) resourceType) is ODataResourceSerializer edmTypeSerializer))
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) resourceType.FullName()));
        foreach (object graph in enumerable)
          edmTypeSerializer.WriteDeltaObjectInline(graph, (IEdmTypeReference) resourceType, writer, writeContext);
      }
      else
      {
        ODataDeltaResourceSet odataDeltaFeed = this.CreateODataDeltaFeed(enumerable, feedType.AsCollection(), writeContext);
        if (odataDeltaFeed == null)
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotSerializerNull, (object) "deltafeed"));
        Func<object, Uri> nextLinkGenerator = ODataDeltaFeedSerializer.GetNextLinkGenerator(odataDeltaFeed, enumerable, writeContext);
        odataDeltaFeed.NextPageLink = (Uri) null;
        writer.WriteStart(odataDeltaFeed);
        object obj = (object) null;
        foreach (object graph in enumerable)
        {
          obj = graph != null ? graph : throw new SerializationException(SRResources.NullElementInCollection);
          if (!(graph is IEdmChangedObject edmChangedObject))
            throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) this.GetType().Name, (object) enumerable.GetType().FullName));
          switch (edmChangedObject.DeltaKind)
          {
            case EdmDeltaEntityKind.Entry:
              if (!(this.SerializerProvider.GetEdmTypeSerializer((IEdmTypeReference) resourceType) is ODataResourceSerializer edmTypeSerializer))
                throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) resourceType.FullName()));
              edmTypeSerializer.WriteDeltaObjectInline(graph, (IEdmTypeReference) resourceType, writer, writeContext);
              continue;
            case EdmDeltaEntityKind.DeletedEntry:
              this.WriteDeltaDeletedEntry(graph, writer, writeContext);
              continue;
            case EdmDeltaEntityKind.DeletedLinkEntry:
              this.WriteDeltaDeletedLink(graph, writer, writeContext);
              continue;
            case EdmDeltaEntityKind.LinkEntry:
              this.WriteDeltaLink(graph, writer, writeContext);
              continue;
            default:
              continue;
          }
        }
        odataDeltaFeed.NextPageLink = nextLinkGenerator(obj);
      }
      writer.WriteEnd();
    }

    internal static Func<object, Uri> GetNextLinkGenerator(
      ODataDeltaResourceSet deltaFeed,
      IEnumerable enumerable,
      ODataSerializerContext writeContext)
    {
      return ODataResourceSetSerializer.GetNextLinkGenerator((ODataResourceSetBase) deltaFeed, enumerable, writeContext);
    }

    public virtual ODataDeltaResourceSet CreateODataDeltaFeed(
      IEnumerable feedInstance,
      IEdmCollectionTypeReference feedType,
      ODataSerializerContext writeContext)
    {
      ODataDeltaResourceSet odataDeltaFeed = new ODataDeltaResourceSet();
      if (writeContext.ExpandedResource == null)
      {
        if (feedInstance is PageResult pageResult)
        {
          odataDeltaFeed.Count = pageResult.Count;
          odataDeltaFeed.NextPageLink = pageResult.NextPageLink;
        }
        else if (writeContext.Request != null)
        {
          odataDeltaFeed.NextPageLink = writeContext.InternalRequest.Context.NextLink;
          odataDeltaFeed.DeltaLink = writeContext.InternalRequest.Context.DeltaLink;
          long? totalCount = writeContext.InternalRequest.Context.TotalCount;
          if (totalCount.HasValue)
            odataDeltaFeed.Count = new long?(totalCount.Value);
        }
      }
      return odataDeltaFeed;
    }

    public virtual void WriteDeltaDeletedEntry(
      object graph,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      if (!(graph is EdmDeltaDeletedEntityObject deletedEntityObject))
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) this.GetType().Name, (object) graph.GetType().FullName));
      ODataDeletedResource odataDeletedResource = new ODataDeletedResource(ODataDeltaFeedSerializer.StringToUri(deletedEntityObject.Id), deletedEntityObject.Reason);
      if (deletedEntityObject.NavigationSource != null)
      {
        ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo()
        {
          NavigationSourceName = deletedEntityObject.NavigationSource.Name
        };
        odataDeletedResource.SetSerializationInfo(serializationInfo);
      }
      if (odataDeletedResource == null)
        return;
      writer.WriteStart(odataDeletedResource);
      writer.WriteEnd();
    }

    public virtual void WriteDeltaDeletedLink(
      object graph,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      if (!(graph is EdmDeltaDeletedLink deltaDeletedLink1))
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) this.GetType().Name, (object) graph.GetType().FullName));
      ODataDeltaDeletedLink deltaDeletedLink2 = new ODataDeltaDeletedLink(deltaDeletedLink1.Source, deltaDeletedLink1.Target, deltaDeletedLink1.Relationship);
      if (deltaDeletedLink2 == null)
        return;
      writer.WriteDeltaDeletedLink(deltaDeletedLink2);
    }

    public virtual void WriteDeltaLink(
      object graph,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      if (!(graph is EdmDeltaLink edmDeltaLink))
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) this.GetType().Name, (object) graph.GetType().FullName));
      ODataDeltaLink deltaLink = new ODataDeltaLink(edmDeltaLink.Source, edmDeltaLink.Target, edmDeltaLink.Relationship);
      if (deltaLink == null)
        return;
      writer.WriteDeltaLink(deltaLink);
    }

    private static IEdmStructuredTypeReference GetResourceType(IEdmTypeReference feedType)
    {
      IEdmTypeReference type = feedType.IsCollection() ? feedType.AsCollection().ElementType() : throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) typeof (ODataResourceSetSerializer).Name, (object) feedType.FullName()));
      if (type.IsEntity() || type.IsComplex())
        return type.AsStructured();
    }

    internal static Uri StringToUri(string uriString)
    {
      try
      {
        return new Uri(uriString, UriKind.RelativeOrAbsolute);
      }
      catch (FormatException ex)
      {
        return new Uri(uriString, UriKind.Relative);
      }
    }
  }
}
