// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.SchemeBasedAuthorizerFactory
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class SchemeBasedAuthorizerFactory
  {
    public static IEndpointAuthorizer GetEndpointAuthorizer(
      ServiceEndpoint serviceEndpoint,
      List<AuthorizationHeader> authorizationHeaders)
    {
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "Token", StringComparison.OrdinalIgnoreCase) || string.Equals(serviceEndpoint.Authorization.Scheme, "UsernamePassword", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) new TokenBasedEndpointAuthorizer(serviceEndpoint, authorizationHeaders);
      if (string.Equals(serviceEndpoint.Authorization.Scheme, "None", StringComparison.OrdinalIgnoreCase))
        return (IEndpointAuthorizer) new NoneEndpointAuthorizer(serviceEndpoint);
      throw new NotSupportedException(Resources.InvalidEndpointAuthorizer((object) serviceEndpoint.Type));
    }
  }
}
