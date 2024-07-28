// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.HttpClientHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Partitioning;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  public static class HttpClientHelper
  {
    private const string c_area = "Location";
    private const string c_layer = "HttpClientHelper";

    public static T CreateSpsClient<T>(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid targetServicePrincipal = default (Guid))
      where T : class, IVssHttpClient
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      Uri spsUriForHostId;
      try
      {
        spsUriForHostId = PartitionedClientHelper.GetSpsUriForHostId(requestContext, hostId);
      }
      catch (PartitionNotFoundException ex)
      {
        requestContext.TraceException(610015, "Location", nameof (HttpClientHelper), (Exception) ex);
        throw new HostDoesNotExistException(hostId);
      }
      return HttpClientHelper.CreateClient<T>(requestContext, spsUriForHostId, targetServicePrincipal);
    }

    public static T CreateClient<T>(
      IVssRequestContext requestContext,
      Uri baseUri,
      Guid targetServicePrincipal = default (Guid))
      where T : class, IVssHttpClient
    {
      ArgumentUtility.CheckForNull<Uri>(baseUri, nameof (baseUri));
      return (requestContext.ClientProvider as ICreateClient).CreateClient<T>(requestContext, baseUri, typeof (T).Name, (ApiResourceLocationCollection) null, targetServicePrincipal: targetServicePrincipal);
    }
  }
}
