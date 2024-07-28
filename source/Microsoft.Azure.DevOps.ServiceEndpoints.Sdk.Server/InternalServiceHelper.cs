// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.InternalServiceHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class InternalServiceHelper
  {
    private static readonly Dictionary<string, string> InternalServiceMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "devtestlabs",
        "0000000e-0000-8888-8000-000000000000"
      },
      {
        "teamfoundation",
        "00025394-6065-48CA-87D9-7F5672854EF7"
      },
      {
        "packaging",
        "00000030-0000-8888-8000-000000000000"
      },
      {
        "feed",
        "00000036-0000-8888-8000-000000000000"
      },
      {
        "governance",
        "00000049-0000-8888-8000-000000000000"
      },
      {
        "rm",
        "0000000D-0000-8888-8000-000000000000"
      },
      {
        "ems",
        "00000028-0000-8888-8000-000000000000"
      }
    };
    private const string InternalServicePrefix = "tfs:";

    public static bool IsInternalService(string connectionId) => !string.IsNullOrWhiteSpace(connectionId) && connectionId.StartsWith("tfs:", StringComparison.OrdinalIgnoreCase);

    public static string GetInternalServiceUrl(IVssRequestContext context, string endpoint)
    {
      string key = !string.IsNullOrWhiteSpace(endpoint) ? endpoint.Substring("tfs:".Length) : throw new ServiceEndpointNotFoundException(ServiceEndpointSdkResources.ServiceEndPointNotFound((object) endpoint));
      string input = InternalServiceHelper.InternalServiceMap.ContainsKey(key) ? InternalServiceHelper.InternalServiceMap[key] : key;
      ILocationService service = context.GetService<ILocationService>();
      Guid serviceAreaIdentifier;
      ref Guid local = ref serviceAreaIdentifier;
      if (Guid.TryParse(input, out local))
        return service.GetLocationServiceUrl(context, serviceAreaIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
    }
  }
}
