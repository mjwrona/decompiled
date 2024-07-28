// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnnotatedODataResourceSerializer
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class AnnotatedODataResourceSerializer : ODataResourceSetSerializer
  {
    private const string ResourceSet = "ResourceSet";

    public AnnotatedODataResourceSerializer(ODataSerializerProvider serializerProvider)
      : base(serializerProvider)
    {
    }

    public override ODataResourceSet CreateResourceSet(
      IEnumerable resourceSetInstance,
      IEdmCollectionTypeReference resourceSetType,
      ODataSerializerContext writeContext)
    {
      ODataResourceSet resourceSet = base.CreateResourceSet(resourceSetInstance, resourceSetType, writeContext);
      if (writeContext.ExpandedResource == null)
      {
        List<string> stringList = writeContext.Request.ODataWarnings();
        if (stringList.Count > 0)
        {
          ODataCollectionValue odataCollectionValue = new ODataCollectionValue()
          {
            TypeName = "Collection(Edm.String)",
            Items = (IEnumerable<object>) stringList
          };
          resourceSet.InstanceAnnotations.Add(new ODataInstanceAnnotation("vsts.warnings", (ODataValue) odataCollectionValue));
        }
      }
      return resourceSet;
    }

    public override void WriteObjectInline(
      object graph,
      IEdmTypeReference expectedType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      if (writeContext == null)
        throw new ArgumentNullException(nameof (writeContext));
      if (expectedType == null)
        throw new ArgumentNullException(nameof (expectedType));
      if (graph == null)
        throw new SerializationException(AnalyticsResources.ODATA_CannotSerializerNull((object) "ResourceSet"));
      if (!(graph is IEnumerable enumerable))
        throw new SerializationException(AnalyticsResources.ODATA_CannotWriteType((object) this.GetType().Name, (object) graph.GetType().FullName));
      this.WriteResourceSet(enumerable, expectedType, writer, writeContext);
    }

    private void WriteResourceSet(
      IEnumerable enumerable,
      IEdmTypeReference resourceSetType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      IEdmStructuredTypeReference resourceType = AnnotatedODataResourceSerializer.GetResourceType(resourceSetType);
      ODataResourceSet resourceSet = this.CreateResourceSet(enumerable, resourceSetType.AsCollection(), writeContext);
      if (resourceSet == null)
        throw new SerializationException(AnalyticsResources.ODATA_CannotSerializerNull((object) "ResourceSet"));
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
        throw new SerializationException(AnalyticsResources.ODATA_TYPE_CANNOT_BE_SERIALIZED((object) resourceType.FullName(), (object) typeof (ODataMediaTypeFormatter).Name));
      writer.WriteStart(resourceSet);
      foreach (object graph in enumerable)
      {
        if (graph == null || graph is NullEdmComplexObject)
        {
          if (resourceType.IsEntity())
            throw new SerializationException(AnalyticsResources.ODATA_NullElementInCollection());
          writer.WriteStart((ODataResource) null);
          writer.WriteEnd();
        }
        else
        {
          if (graph.GetType().FullName.Contains("Microsoft.AspNet.OData.Query.Expressions.SelectExpandBinder") && graph.GetType().GetProperty("Container") != (PropertyInfo) null && graph.GetType().GetProperty("Container").GetValue(graph) == null && graph.GetType().GetProperty("Instance") != (PropertyInfo) null && graph.GetType().GetProperty("Instance").GetValue(graph).ToString() == "Microsoft.AspNet.OData.Query.Expressions.AggregationWrapper")
            throw new SerializationException(AnalyticsResources.ODATA_SELECT_NOT_VALID());
          edmTypeSerializer.WriteObjectInline(graph, (IEdmTypeReference) resourceType, writer, writeContext);
        }
      }
      if (enumerable is IODataEnumerable odataEnumerable)
        resourceSet.NextPageLink = odataEnumerable.NextPageLink();
      writer.WriteEnd();
    }

    private static IEdmStructuredTypeReference GetResourceType(IEdmTypeReference resourceSetType)
    {
      IEdmTypeReference type = resourceSetType.IsCollection() ? resourceSetType.AsCollection().ElementType() : throw new SerializationException(AnalyticsResources.ODATA_CannotWriteType((object) typeof (ODataResourceSetSerializer).Name, (object) resourceSetType.FullName()));
      if (type.IsEntity() || type.IsComplex())
        return type.AsStructured();
    }
  }
}
