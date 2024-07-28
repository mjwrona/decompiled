// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ETagMessageHandler
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.AspNet.OData
{
  public class ETagMessageHandler : DelegatingHandler
  {
    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      HttpConfiguration configuration = request != null ? request.GetConfiguration() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestMustContainConfiguration);
      HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);
      ObjectContent content = httpResponseMessage == null ? (ObjectContent) null : httpResponseMessage.Content as ObjectContent;
      if (content != null)
      {
        EntityTagHeaderValue etag = ETagMessageHandler.GetETag(httpResponseMessage == null ? new int?() : new int?((int) httpResponseMessage.StatusCode), request.ODataProperties().Path, request.GetModel(), content.Value, configuration.GetETagHandler());
        if (etag != null)
          httpResponseMessage.Headers.ETag = etag;
      }
      return httpResponseMessage;
    }

    internal Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => this.SendAsync(request, CancellationToken.None);

    private static EntityTagHeaderValue GetETag(
      int? statusCode,
      Microsoft.AspNet.OData.Routing.ODataPath path,
      IEdmModel model,
      object value,
      IETagHandler etagHandler)
    {
      if (path == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (path));
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (model));
      if (etagHandler == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (etagHandler));
      if (!statusCode.HasValue || statusCode.Value < 200 || statusCode.Value >= 300 || statusCode.Value == 204)
        return (EntityTagHeaderValue) null;
      IEdmEntityType entityEntityType = ETagMessageHandler.GetSingleEntityEntityType(path);
      IEdmEntityTypeReference typeReference = ETagMessageHandler.GetTypeReference(model, entityEntityType, value);
      if (typeReference == null)
        return (EntityTagHeaderValue) null;
      ResourceContext instanceContext = ETagMessageHandler.CreateInstanceContext(model, typeReference, value);
      instanceContext.EdmModel = model;
      instanceContext.NavigationSource = path.NavigationSource;
      return ETagMessageHandler.CreateETag(instanceContext, etagHandler);
    }

    private static IEdmEntityTypeReference GetTypeReference(
      IEdmModel model,
      IEdmEntityType edmType,
      object value)
    {
      if (model == null || edmType == null || value == null)
        return (IEdmEntityTypeReference) null;
      IEdmObject edmObject = (IEdmObject) (value as IEdmEntityObject);
      if (edmObject != null)
        return edmObject.GetEdmType().AsEntity();
      IEdmTypeReference edmTypeReference = model.GetEdmTypeReference(value.GetType());
      return edmTypeReference != null && edmTypeReference.Definition.IsOrInheritsFrom((IEdmType) edmType) ? (IEdmEntityTypeReference) edmTypeReference : (IEdmEntityTypeReference) null;
    }

    private static EntityTagHeaderValue CreateETag(
      ResourceContext resourceContext,
      IETagHandler handler)
    {
      IEdmModel edmModel = resourceContext.EdmModel;
      IEnumerable<IEdmStructuralProperty> structuralProperties = edmModel == null || resourceContext.NavigationSource == null ? Enumerable.Empty<IEdmStructuralProperty>() : (IEnumerable<IEdmStructuralProperty>) edmModel.GetConcurrencyProperties(resourceContext.NavigationSource).OrderBy<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (c => c.Name));
      IDictionary<string, object> properties = (IDictionary<string, object>) new Dictionary<string, object>();
      foreach (IEdmStructuralProperty structuralProperty in structuralProperties)
        properties.Add(structuralProperty.Name, resourceContext.GetPropertyValue(structuralProperty.Name));
      return handler.CreateETag(properties);
    }

    private static ResourceContext CreateInstanceContext(
      IEdmModel model,
      IEdmEntityTypeReference reference,
      object value)
    {
      return new ResourceContext(new ODataSerializerContext()
      {
        Model = model
      }, (IEdmStructuredTypeReference) reference, value);
    }

    internal static IEdmEntityType GetSingleEntityEntityType(Microsoft.AspNet.OData.Routing.ODataPath path)
    {
      if (path == null || path.Segments.Count == 0)
        return (IEdmEntityType) null;
      int index = path.Segments.Count - 1;
      while (index >= 0 && path.Segments[index] is TypeSegment)
        --index;
      if (index < 0)
        return (IEdmEntityType) null;
      switch (path.Segments[index])
      {
        case SingletonSegment _:
        case KeySegment _:
          return (IEdmEntityType) path.EdmType;
        case NavigationPropertySegment navigationPropertySegment:
          if (navigationPropertySegment.NavigationProperty.TargetMultiplicity() == EdmMultiplicity.ZeroOrOne || navigationPropertySegment.NavigationProperty.TargetMultiplicity() == EdmMultiplicity.One)
            return (IEdmEntityType) path.EdmType;
          break;
      }
      return (IEdmEntityType) null;
    }
  }
}
