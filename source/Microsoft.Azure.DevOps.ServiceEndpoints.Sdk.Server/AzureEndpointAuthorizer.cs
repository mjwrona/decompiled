// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureEndpointAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public abstract class AzureEndpointAuthorizer : IEndpointAuthorizer
  {
    protected readonly IVssRequestContext _requestContext;
    public readonly ServiceEndpoint ServiceEndpoint;

    public bool SupportsAbsoluteEndpoint => true;

    public AzureEndpointAuthorizer(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(serviceEndpoint, nameof (serviceEndpoint));
      this._requestContext = requestContext;
      this.ServiceEndpoint = serviceEndpoint;
    }

    public string GetEndpointUrl() => this.ServiceEndpoint.GetEndpointUrl();

    public string GetServiceEndpointType() => this.ServiceEndpoint.Type;

    public abstract void AuthorizeRequest(HttpWebRequest request, string resourceUrl);
  }
}
