// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ClientToolController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [VersionedApiControllerCustomName(Area = "clienttools", ResourceName = "release", ResourceVersion = 1)]
  [SetActivityLogAnonymousIdentifier]
  [ClientInternalUseOnly(true)]
  public sealed class ClientToolController : BlobControllerBase
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BlobExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ControllerMethodTraceFilter(5709030)]
    public ClientToolReleaseInfo GetRelease(
      [FromUri] string toolName,
      [ClientQueryParameter] string version = null,
      [ClientQueryParameter] string osName = null,
      [ClientQueryParameter] string osRelease = null,
      [ClientQueryParameter] string osVersion = null,
      [ClientQueryParameter] string arch = null,
      [ClientQueryParameter] string distroName = null,
      [ClientQueryParameter] string distroVersion = null,
      [ClientQueryParameter] bool netfx = false)
    {
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ClientTool toolName1 = (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ClientTool) System.Enum.Parse(typeof (Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ClientTool), toolName, true);
      ClientPlatformInfo platformInfo = new ClientPlatformInfo()
      {
        Architecture = arch,
        OSInfo = new OSInfo()
        {
          Name = osName,
          Release = osRelease,
          Version = osVersion,
          DistributionName = distroName,
          DistributionVersion = distroVersion
        },
        IsNetfx = netfx
      };
      return this.GetReleaseForPlatform(platformInfo, toolName1, version) ?? throw ClientToolNotFoundException.Create(toolName, platformInfo.OSInfo.Name, platformInfo.Architecture);
    }

    private ClientToolReleaseInfo GetReleaseForPlatform(
      ClientPlatformInfo platformInfo,
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ClientTool toolName,
      string version = null,
      bool allowEdge = true)
    {
      RuntimeIdentifier? nullable = new RuntimeIdentifierResolver().TryResolveRuntimeIdentifier(platformInfo);
      if (!nullable.HasValue)
        return (ClientToolReleaseInfo) null;
      EdgeCache edgeCache = allowEdge ? EdgeCache.Allowed : EdgeCache.NotAllowed;
      return this.TfsRequestContext.GetService<IClientToolReleaseQueryService>().GetRelease(this.TfsRequestContext, toolName, nullable.Value, edgeCache, version);
    }
  }
}
