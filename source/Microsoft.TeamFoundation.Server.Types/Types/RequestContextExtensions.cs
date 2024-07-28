// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.RequestContextExtensions
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.Types
{
  public static class RequestContextExtensions
  {
    public static ProjectInfo GetProjectInfo(
      this IVssRequestContext requestContext,
      string projFilter)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      Guid result;
      return !Guid.TryParse(projFilter, out result) ? service.GetProject(requestContext, projFilter) : service.GetProject(requestContext, result);
    }
  }
}
