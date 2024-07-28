// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.ExternalConnection
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  internal struct ExternalConnection
  {
    public ExternalConnection(ServiceEndpoint externalConnectionEndpoint)
    {
      this.Url = externalConnectionEndpoint.Url;
      this.Authorization = externalConnectionEndpoint.Authorization.Clone();
      this.Data = externalConnectionEndpoint.Data ?? (IDictionary<string, string>) new Dictionary<string, string>();
    }

    public Uri Url { get; internal set; }

    public EndpointAuthorization Authorization { get; internal set; }

    public IDictionary<string, string> Data { get; internal set; }

    public string GetBasicAuthParameter(string parameterName) => this.GetBasicAuthParameter(parameterName, string.Empty);

    public string GetBasicAuthParameter(string parameterName, string defaultValue)
    {
      string basicAuthParameter = string.Empty;
      if (this.Authorization != null && this.Authorization.Scheme.Equals("UsernamePassword"))
      {
        this.Authorization.Parameters.TryGetValue(parameterName, out basicAuthParameter);
        if (string.IsNullOrEmpty(basicAuthParameter))
          basicAuthParameter = defaultValue;
      }
      return basicAuthParameter;
    }
  }
}
