// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataNullValueMessageHandler
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData
{
  public class ODataNullValueMessageHandler : DelegatingHandler
  {
    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);
      ObjectContent content = httpResponseMessage == null ? (ObjectContent) null : httpResponseMessage.Content as ObjectContent;
      if (request.Method == HttpMethod.Get && content != null && content.Value == null && httpResponseMessage.StatusCode == HttpStatusCode.OK)
      {
        HttpStatusCode? statusCodeOrNull = ODataNullValueMessageHandler.GetUpdatedResponseStatusCodeOrNull(request.ODataProperties().Path);
        if (statusCodeOrNull.HasValue)
          httpResponseMessage = request.CreateResponse(statusCodeOrNull.Value);
      }
      return httpResponseMessage;
    }

    internal Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => this.SendAsync(request, CancellationToken.None);

    internal static HttpStatusCode? GetUpdatedResponseStatusCodeOrNull(Microsoft.AspNet.OData.Routing.ODataPath oDataPath)
    {
      if (oDataPath == null || oDataPath.Segments == null || oDataPath.Segments.Count == 0)
        return new HttpStatusCode?();
      int index1 = oDataPath.Segments.Count - 1;
      ReadOnlyCollection<ODataPathSegment> segments = oDataPath.Segments;
      while (index1 >= 0 && segments[index1] is TypeSegment)
        --index1;
      if (index1 >= 0 && segments[index1] is ValueSegment)
        --index1;
      if (index1 < 0)
        return new HttpStatusCode?();
      if (segments[index1] is KeySegment)
      {
        int index2 = index1 - 1;
        while (index2 >= 0 && segments[index2] is TypeSegment)
          --index2;
        if (index2 < 0)
          return new HttpStatusCode?();
        if (segments[index2] is EntitySetSegment)
          return new HttpStatusCode?(HttpStatusCode.NotFound);
        return segments[index2] is NavigationPropertySegment ? new HttpStatusCode?(HttpStatusCode.NoContent) : new HttpStatusCode?();
      }
      if (segments[index1] is PropertySegment propertySegment)
        return ODataNullValueMessageHandler.GetChangedStatusCodeForProperty(propertySegment);
      if (segments[index1] is NavigationPropertySegment navigation)
        return ODataNullValueMessageHandler.GetChangedStatusCodeForNavigationProperty(navigation);
      return segments[index1] is SingletonSegment ? new HttpStatusCode?(HttpStatusCode.NotFound) : new HttpStatusCode?();
    }

    private static HttpStatusCode? GetChangedStatusCodeForNavigationProperty(
      NavigationPropertySegment navigation)
    {
      switch (navigation.NavigationProperty.TargetMultiplicity())
      {
        case EdmMultiplicity.ZeroOrOne:
        case EdmMultiplicity.One:
          return new HttpStatusCode?(HttpStatusCode.NoContent);
        default:
          return new HttpStatusCode?();
      }
    }

    private static HttpStatusCode? GetChangedStatusCodeForProperty(PropertySegment propertySegment)
    {
      IEdmTypeReference type = propertySegment.Property.Type;
      return !type.IsPrimitive() && !type.IsComplex() ? new HttpStatusCode?() : new HttpStatusCode?(HttpStatusCode.NoContent);
    }
  }
}
