// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Project.WebServer.ProjectsContinuationTokenExtensions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Server.Core.Project.WebServer
{
  internal static class ProjectsContinuationTokenExtensions
  {
    internal static List<TeamProjectReference> ApplyPagination(
      this List<TeamProjectReference> projects,
      ProjectsContinuationToken continuationToken)
    {
      if (!continuationToken.IsValid)
        return projects;
      (int num1, int num2) = continuationToken.CalculatePaginationValues();
      return projects.Skip<TeamProjectReference>(num2).Take<TeamProjectReference>(num1).ToList<TeamProjectReference>();
    }

    internal static HttpResponseMessage WithContinuationTokenHeader(
      this HttpResponseMessage response,
      ProjectsContinuationToken continuationToken)
    {
      if (!continuationToken.IsValid || continuationToken.AllTheProjectsBeenFetched)
        return response;
      response.Headers.Add("x-ms-continuationtoken", continuationToken.NextToken.ToString());
      return response;
    }
  }
}
