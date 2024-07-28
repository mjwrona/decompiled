// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Results.ResultHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Builder.Conventions;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Results
{
  internal static class ResultHelpers
  {
    public const string EntityIdHeaderName = "OData-EntityId";

    public static Uri GenerateODataLink(HttpRequestMessage request, object entity, bool isEntityId)
    {
      IEdmModel model = request.GetModel();
      if (model == null)
        throw new InvalidOperationException(SRResources.RequestMustHaveModel);
      Microsoft.AspNet.OData.Routing.ODataPath path = request.ODataProperties().Path;
      return ResultHelpers.GenerateODataLink(new ResourceContext(new ODataSerializerContext()
      {
        NavigationSource = (path != null ? path.NavigationSource : throw new InvalidOperationException(SRResources.ODataPathMissing)) ?? throw new InvalidOperationException(SRResources.NavigationSourceMissingDuringSerialization),
        Model = model,
        Url = request.GetUrlHelper() ?? new UrlHelper(request),
        MetadataLevel = ODataMetadataLevel.FullMetadata,
        Request = request,
        Path = path
      }, (IEdmStructuredTypeReference) ResultHelpers.GetEntityType(model, entity), entity), isEntityId);
    }

    public static void AddEntityId(HttpResponseMessage response, Func<Uri> entityId)
    {
      if (response.StatusCode != HttpStatusCode.NoContent)
        return;
      response.Headers.TryAddWithoutValidation("OData-EntityId", entityId().ToString());
    }

    public static void AddServiceVersion(HttpResponseMessage response, Func<string> version)
    {
      if (response.StatusCode != HttpStatusCode.NoContent)
        return;
      response.Headers.TryAddWithoutValidation("OData-Version", version());
    }

    internal static ODataVersion GetODataResponseVersion(HttpRequestMessage request)
    {
      if (request == null)
        return ODataVersion.V4;
      HttpRequestMessageProperties messageProperties = request.ODataProperties();
      return messageProperties.ODataMaxServiceVersion ?? messageProperties.ODataMinServiceVersion ?? messageProperties.ODataServiceVersion.GetValueOrDefault();
    }

    public static Uri GenerateODataLink(ResourceContext resourceContext, bool isEntityId)
    {
      if (resourceContext.NavigationSource.NavigationSourceKind() == EdmNavigationSourceKind.ContainedEntitySet)
        return ResultHelpers.GenerateContainmentODataPathSegments(resourceContext, isEntityId);
      NavigationSourceLinkBuilderAnnotation sourceLinkBuilder = resourceContext.EdmModel.GetNavigationSourceLinkBuilder(resourceContext.NavigationSource);
      Uri uri = sourceLinkBuilder.BuildIdLink(resourceContext);
      if (isEntityId)
        return !(uri == (Uri) null) ? uri : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.IdLinkNullForEntityIdHeader, (object) resourceContext.NavigationSource.Name);
      Uri odataLink = sourceLinkBuilder.BuildEditLink(resourceContext);
      if (!(odataLink == (Uri) null))
        return odataLink;
      return uri != (Uri) null ? uri : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EditLinkNullForLocationHeader, (object) resourceContext.NavigationSource.Name);
    }

    private static Uri GenerateContainmentODataPathSegments(
      ResourceContext resourceContext,
      bool isEntityId)
    {
      List<ODataPathSegment> list = new ContainmentPathBuilder().TryComputeCanonicalContainingPath(resourceContext.InternalRequest.Context.Path ?? throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ODataPathMissing)).Segments.ToList<ODataPathSegment>();
      if (!(resourceContext.NavigationSource is IEdmEntitySet entitySet))
        entitySet = (IEdmEntitySet) new EdmEntitySet((IEdmEntityContainer) new EdmEntityContainer("NS", "Default"), resourceContext.NavigationSource.Name, resourceContext.NavigationSource.EntityType());
      list.Add((ODataPathSegment) new EntitySetSegment(entitySet));
      list.Add((ODataPathSegment) new KeySegment(ConventionsHelpers.GetEntityKey(resourceContext), resourceContext.StructuredType as IEdmEntityType, resourceContext.NavigationSource));
      if (!isEntityId && resourceContext.StructuredType != resourceContext.NavigationSource.EntityType())
        list.Add((ODataPathSegment) new TypeSegment((IEdmType) resourceContext.StructuredType, resourceContext.NavigationSource));
      string odataLink = resourceContext.InternalUrlHelper.CreateODataLink((IList<ODataPathSegment>) list);
      return odataLink != null ? new Uri(odataLink) : (Uri) null;
    }

    private static IEdmEntityTypeReference GetEntityType(IEdmModel model, object entity)
    {
      Type type = entity.GetType();
      IEdmTypeReference edmTypeReference = model.GetEdmTypeReference(type);
      if (edmTypeReference == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ResourceTypeNotInModel, (object) type.FullName);
      return edmTypeReference.IsEntity() ? edmTypeReference.AsEntity() : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.TypeMustBeEntity, (object) edmTypeReference.FullName());
    }
  }
}
