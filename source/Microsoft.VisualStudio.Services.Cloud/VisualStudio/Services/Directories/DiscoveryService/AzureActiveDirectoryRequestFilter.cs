// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectoryRequestFilter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class AzureActiveDirectoryRequestFilter
  {
    internal static bool AllowRequest(IVssRequestContext context, DirectoryInternalRequest request)
    {
      context.TraceConditionally(15003003, TraceLevel.Verbose, "VisualStudio.Services.DirectoryDiscovery", "Service", (Func<string>) (() => "AzureActiveDirectoryRequestFilter AllowRequest" + string.Join(";", new string[10]
      {
        context.ActivityId.ToString(),
        context.UniqueIdentifier.ToString(),
        context.ServiceName,
        context.ServiceHost.ToString(),
        context.GetAuthenticatedId().ToString(),
        context.GetUserId().ToString(),
        context.IsSystemContext.ToString(),
        context.IsServicingContext.ToString(),
        context.IsUserContext.ToString(),
        string.Join("-", request.Directories)
      })));
      return !context.ServiceHost.Is(TeamFoundationHostType.Deployment) && (request.Directories.Contains<string>("aad") || request.Directories.Contains<string>("src") && DirectoryUtils.IsOrganizationAadBacked(context));
    }
  }
}
