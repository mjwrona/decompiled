// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.GenericEndpointAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class GenericEndpointAuthorizer : IEndpointAuthorizer
  {
    private readonly ServiceEndpoint serviceEndpoint;

    public bool SupportsAbsoluteEndpoint => true;

    public GenericEndpointAuthorizer(ServiceEndpoint serviceEndpoint) => this.serviceEndpoint = serviceEndpoint;

    public void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      if (!string.IsNullOrEmpty(resourceUrl))
        throw new HttpRequestException(ServiceEndpointSdkResources.ResourceUrlNotSupported((object) this.serviceEndpoint.Type, (object) this.serviceEndpoint.Authorization.Scheme));
      if (!this.serviceEndpoint.Authorization.Scheme.Equals("UsernamePassword"))
        throw new HttpRequestException(ServiceEndpointSdkResources.NoUsernamePassword());
      string parameter1 = this.serviceEndpoint.Authorization.Parameters["Username"];
      string parameter2 = this.serviceEndpoint.Authorization.Parameters["Password"];
      if (parameter1.IsNullOrEmpty<char>() && parameter2.IsNullOrEmpty<char>())
        return;
      request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", (object) parameter1, (object) parameter2))));
    }

    public string GetEndpointUrl() => this.serviceEndpoint.Url.AbsoluteUri;

    public string GetServiceEndpointType() => this.serviceEndpoint.Type;
  }
}
