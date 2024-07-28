// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.ServiceEndpointDetailsExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public static class ServiceEndpointDetailsExtensions
  {
    public static ServiceEndpoint ToServiceEndpoint(
      this ServiceEndpointDetails serviceEndpointDetails,
      string endpointId)
    {
      if (serviceEndpointDetails == null)
        return new ServiceEndpoint() { Type = endpointId };
      Guid result;
      Guid.TryParse(endpointId, out result);
      ServiceEndpoint serviceEndpoint = new ServiceEndpoint()
      {
        Id = result,
        Type = serviceEndpointDetails.Type,
        Url = serviceEndpointDetails.Url
      };
      if (serviceEndpointDetails.Authorization != null)
        serviceEndpoint.Authorization = serviceEndpointDetails.Authorization.Clone();
      if (serviceEndpointDetails.Data != null)
        serviceEndpoint.Data = serviceEndpointDetails.Data;
      return serviceEndpoint;
    }

    public static bool Validate(
      this ServiceEndpointDetails serviceEndpointDetails,
      out string errorMessage)
    {
      errorMessage = string.Empty;
      if (serviceEndpointDetails == null)
      {
        errorMessage = "serviceEndpointDetails: null";
        return false;
      }
      if (string.IsNullOrEmpty(serviceEndpointDetails.Type))
      {
        errorMessage = string.Format("{0}:{1}", (object) CommonResources.EmptyStringNotAllowed(), (object) "serviceEndpointDetails.Type");
        return false;
      }
      if (serviceEndpointDetails.Url == (Uri) null)
      {
        errorMessage = "serviceEndpointDetails.Url: null";
        return false;
      }
      if (serviceEndpointDetails.Authorization != null)
        return true;
      errorMessage = "serviceEndpointDetails.Authorization: null";
      return false;
    }
  }
}
