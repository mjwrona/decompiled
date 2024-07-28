// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataResourceSetSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataResourceSetSerializer : ODataEdmTypeSerializer
  {
    private const string ResourceSet = "ResourceSet";

    public ODataResourceSetSerializer(ODataSerializerProvider serializerProvider)
      : base(ODataPayloadKind.ResourceSet, serializerProvider)
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
      IEdmEntitySetBase entitySet = writeContext != null ? writeContext.NavigationSource as IEdmEntitySetBase : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      IEdmTypeReference edmType = writeContext.GetEdmType(graph, type);
      IEdmStructuredTypeReference resourceType = ODataResourceSetSerializer.GetResourceType(edmType);
      ODataWriter resourceSetWriter = messageWriter.CreateODataResourceSetWriter(entitySet, resourceType.StructuredDefinition());
      this.WriteObjectInline(graph, edmType, resourceSetWriter, writeContext);
    }

    public override void WriteObjectInline(
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
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotSerializerNull, (object) "ResourceSet"));
      if (!(graph is IEnumerable enumerable))
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) this.GetType().Name, (object) graph.GetType().FullName));
      this.WriteResourceSet(enumerable, expectedType, writer, writeContext);
    }

    private void WriteResourceSet(
      IEnumerable enumerable,
      IEdmTypeReference resourceSetType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      IEdmStructuredTypeReference resourceType = ODataResourceSetSerializer.GetResourceType(resourceSetType);
      ODataResourceSet resourceSet = this.CreateResourceSet(enumerable, resourceSetType.AsCollection(), writeContext);
      Func<object, Uri> nextLinkGenerator = ODataResourceSetSerializer.GetNextLinkGenerator((ODataResourceSetBase) resourceSet, enumerable, writeContext);
      if (resourceSet == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotSerializerNull, (object) "ResourceSet"));
      if (!(writeContext.NavigationSource is IEdmEntitySetBase))
        resourceSet.SetSerializationInfo(new ODataResourceSerializationInfo()
        {
          IsFromCollection = true,
          NavigationSourceEntityTypeName = resourceType.FullName(),
          NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet,
          NavigationSourceName = (string) null
        });
      ODataEdmTypeSerializer edmTypeSerializer = this.SerializerProvider.GetEdmTypeSerializer((IEdmTypeReference) resourceType);
      if (edmTypeSerializer == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) resourceType.FullName()));
      resourceSet.NextPageLink = (Uri) null;
      writer.WriteStart(resourceSet);
      object obj = (object) null;
      foreach (object graph in enumerable)
      {
        obj = graph;
        switch (graph)
        {
          case null:
          case NullEdmComplexObject _:
            if (resourceType.IsEntity())
              throw new SerializationException(SRResources.NullElementInCollection);
            writer.WriteStart((ODataResource) null);
            writer.WriteEnd();
            continue;
          default:
            edmTypeSerializer.WriteObjectInline(graph, (IEdmTypeReference) resourceType, writer, writeContext);
            continue;
        }
      }
      resourceSet.NextPageLink = nextLinkGenerator(obj);
      writer.WriteEnd();
    }

    public virtual ODataResourceSet CreateResourceSet(
      IEnumerable resourceSetInstance,
      IEdmCollectionTypeReference resourceSetType,
      ODataSerializerContext writeContext)
    {
      ODataResourceSet odataResourceSet = new ODataResourceSet();
      odataResourceSet.TypeName = resourceSetType.FullName();
      ODataResourceSet resourceSet = odataResourceSet;
      IEdmStructuredTypeReference type = ODataResourceSetSerializer.GetResourceType((IEdmTypeReference) resourceSetType).AsStructured();
      if (writeContext.NavigationSource != null && type.IsEntity())
      {
        ResourceSetContext resourceSetContext = ResourceSetContext.Create(writeContext, resourceSetInstance);
        IEdmEntityType entityType = type.AsEntity().EntityDefinition();
        foreach (ODataOperation odataOperation in this.CreateODataOperations(writeContext.Model.GetAvailableOperationsBoundToCollection(entityType), resourceSetContext, writeContext))
        {
          if (odataOperation is ODataAction action)
            resourceSet.AddAction(action);
          else
            resourceSet.AddFunction((ODataFunction) odataOperation);
        }
      }
      if (writeContext.ExpandedResource == null)
      {
        if (resourceSetInstance is PageResult pageResult)
        {
          resourceSet.Count = pageResult.Count;
          resourceSet.NextPageLink = pageResult.NextPageLink;
        }
        else if (writeContext.Request != null)
        {
          resourceSet.NextPageLink = writeContext.InternalRequest.Context.NextLink;
          resourceSet.DeltaLink = writeContext.InternalRequest.Context.DeltaLink;
          long? totalCount = writeContext.InternalRequest.Context.TotalCount;
          if (totalCount.HasValue)
            resourceSet.Count = new long?(totalCount.Value);
        }
      }
      else if (resourceSetInstance is ICountOptionCollection optionCollection && optionCollection.TotalCount.HasValue)
        resourceSet.Count = optionCollection.TotalCount;
      return resourceSet;
    }

    internal static Func<object, Uri> GetNextLinkGenerator(
      ODataResourceSetBase resourceSet,
      IEnumerable resourceSetInstance,
      ODataSerializerContext writeContext)
    {
      if (resourceSet != null && resourceSet.NextPageLink != (Uri) null)
      {
        Uri defaultUri = resourceSet.NextPageLink;
        return (Func<object, Uri>) (obj => defaultUri);
      }
      if (writeContext.ExpandedResource == null)
      {
        if (writeContext.InternalRequest != null && writeContext.QueryContext != null)
        {
          SkipTokenHandler handler = writeContext.QueryContext.GetSkipTokenHandler();
          return (Func<object, Uri>) (obj => handler.GenerateNextPageLink(writeContext.InternalRequest.RequestUri, writeContext.InternalRequest.Context.PageSize, obj, writeContext));
        }
      }
      else
      {
        ITruncatedCollection truncatedCollection = resourceSetInstance as ITruncatedCollection;
        if (truncatedCollection != null && truncatedCollection.IsTruncated)
          return (Func<object, Uri>) (obj => ODataResourceSetSerializer.GetNestedNextPageLink(writeContext, truncatedCollection.PageSize, obj));
      }
      return (Func<object, Uri>) (obj => (Uri) null);
    }

    public virtual ODataOperation CreateODataOperation(
      IEdmOperation operation,
      ResourceSetContext resourceSetContext,
      ODataSerializerContext writeContext)
    {
      if (operation == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (operation));
      if (resourceSetContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceSetContext));
      ODataMetadataLevel odataMetadataLevel = writeContext != null ? writeContext.MetadataLevel : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      IEdmModel model = writeContext.Model;
      if (odataMetadataLevel != ODataMetadataLevel.FullMetadata)
        return (ODataOperation) null;
      OperationLinkBuilder operationLinkBuilder = model.GetOperationLinkBuilder(operation);
      if (operationLinkBuilder == null)
        return (ODataOperation) null;
      Uri uri1 = operationLinkBuilder.BuildLink(resourceSetContext);
      if (uri1 == (Uri) null)
        return (ODataOperation) null;
      Uri uri2 = new Uri(new Uri(writeContext.InternalUrlHelper.CreateODataLink((ODataPathSegment) MetadataSegment.Instance)), "#" + operation.FullName());
      ODataOperation odataOperation = !(operation is IEdmAction) ? (ODataOperation) new ODataFunction() : (ODataOperation) new ODataAction();
      odataOperation.Metadata = uri2;
      ODataResourceSerializer.EmitTitle(model, operation, odataOperation);
      if (odataMetadataLevel == ODataMetadataLevel.FullMetadata || !operationLinkBuilder.FollowsConventions)
        odataOperation.Target = uri1;
      return odataOperation;
    }

    private IEnumerable<ODataOperation> CreateODataOperations(
      IEnumerable<IEdmOperation> operations,
      ResourceSetContext resourceSetContext,
      ODataSerializerContext writeContext)
    {
      foreach (IEdmOperation operation in operations)
      {
        ODataOperation odataOperation = this.CreateODataOperation(operation, resourceSetContext, writeContext);
        if (odataOperation != null)
          yield return odataOperation;
      }
    }

    private static Uri GetNestedNextPageLink(
      ODataSerializerContext writeContext,
      int pageSize,
      object obj)
    {
      IEdmNavigationSource navigationSource = writeContext.ExpandedResource.NavigationSource;
      Uri navigationLink = writeContext.Model.GetNavigationSourceLinkBuilder(navigationSource).BuildNavigationLink(writeContext.ExpandedResource, writeContext.NavigationProperty);
      Uri fromExpandedItem = ODataResourceSetSerializer.GenerateQueryFromExpandedItem(writeContext, navigationLink);
      SkipTokenHandler skipTokenHandler = (SkipTokenHandler) null;
      if (writeContext.QueryContext != null)
        skipTokenHandler = writeContext.QueryContext.GetSkipTokenHandler();
      if (!(fromExpandedItem != (Uri) null))
        return (Uri) null;
      return skipTokenHandler != null ? skipTokenHandler.GenerateNextPageLink(fromExpandedItem, pageSize, obj, writeContext) : GetNextPageHelper.GetNextPageLink(fromExpandedItem, pageSize);
    }

    private static Uri GenerateQueryFromExpandedItem(
      ODataSerializerContext writeContext,
      Uri navigationLink)
    {
      IWebApiUrlHelper internalUrlHelper = writeContext.InternalUrlHelper;
      if (internalUrlHelper == null)
        return navigationLink;
      Uri serviceRoot = new Uri(internalUrlHelper.CreateODataLink(writeContext.InternalRequest.Context.RouteName, writeContext.InternalRequest.PathHandler, (IList<ODataPathSegment>) new List<ODataPathSegment>()));
      ODataUri uri = new ODataUriParser(writeContext.Model, serviceRoot, navigationLink).ParseUri();
      uri.SelectAndExpand = writeContext.SelectExpandClause;
      if (writeContext.CurrentExpandedSelectItem != null)
      {
        uri.OrderBy = writeContext.CurrentExpandedSelectItem.OrderByOption;
        uri.Filter = writeContext.CurrentExpandedSelectItem.FilterOption;
        uri.Skip = writeContext.CurrentExpandedSelectItem.SkipOption;
        uri.Top = writeContext.CurrentExpandedSelectItem.TopOption;
        if (writeContext.CurrentExpandedSelectItem.CountOption.HasValue)
        {
          bool? countOption = writeContext.CurrentExpandedSelectItem.CountOption;
          if (countOption.HasValue)
          {
            ODataUri odataUri = uri;
            countOption = writeContext.CurrentExpandedSelectItem.CountOption;
            bool? nullable = new bool?(countOption.Value);
            odataUri.QueryCount = nullable;
          }
        }
        if (writeContext.CurrentExpandedSelectItem is ExpandedNavigationSelectItem expandedSelectItem)
          uri.SelectAndExpand = expandedSelectItem.SelectAndExpand;
      }
      ODataUrlKeyDelimiter urlKeyDelimiter = writeContext.InternalRequest.Options.UrlKeyDelimiter == ODataUrlKeyDelimiter.Slash ? ODataUrlKeyDelimiter.Slash : ODataUrlKeyDelimiter.Parentheses;
      return uri.BuildUri(urlKeyDelimiter);
    }

    private static IEdmStructuredTypeReference GetResourceType(IEdmTypeReference resourceSetType)
    {
      IEdmTypeReference type = resourceSetType.IsCollection() ? resourceSetType.AsCollection().ElementType() : throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) typeof (ODataResourceSetSerializer).Name, (object) resourceSetType.FullName()));
      if (type.IsEntity() || type.IsComplex())
        return type.AsStructured();
    }
  }
}
