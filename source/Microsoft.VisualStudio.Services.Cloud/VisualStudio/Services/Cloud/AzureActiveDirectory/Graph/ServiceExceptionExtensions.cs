// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.ServiceExceptionExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using System.Net;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  internal static class ServiceExceptionExtensions
  {
    internal static bool IsResourceNotFoundError(this ServiceException e) => e.StatusCode == HttpStatusCode.NotFound && "Request_ResourceNotFound".Equals(e?.Error?.Code);

    internal static bool IsBadRequest(this ServiceException e) => e.StatusCode == HttpStatusCode.BadRequest && "Request_BadRequest".Equals(e?.Error?.Code);

    internal static bool IsUnauthorizedError(this ServiceException e) => e.StatusCode == HttpStatusCode.Unauthorized && "InvalidAuthenticationToken".Equals(e?.Error?.Code);

    internal static bool IsRequestDeniedError(this ServiceException e) => e.StatusCode == HttpStatusCode.Forbidden && "Authorization_RequestDenied".Equals(e?.Error?.Code);
  }
}
