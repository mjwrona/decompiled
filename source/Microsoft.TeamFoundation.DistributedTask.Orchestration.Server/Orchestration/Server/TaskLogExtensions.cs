// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskLogExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class TaskLogExtensions
  {
    public static string GetPagePath(this TaskLog log, TaskLogPage page) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) log.Path, (object) page.PageId);

    public static void UpdateLocations(
      this TaskLog log,
      IVssRequestContext requestContext,
      Guid planId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      log.Location = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.Logs_Compat, (object) new
      {
        planId = planId,
        logId = log.Id
      });
    }
  }
}
