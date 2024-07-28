// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskDefinitionsControllerBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientInternalUseOnly(false)]
  public abstract class TaskDefinitionsControllerBase : DistributedTaskApiController
  {
    protected const int BufferSize = 32768;

    [HttpDelete]
    public void DeleteTaskDefinition(Guid taskId) => this.TaskService.DeleteTaskDefinition(this.TfsRequestContext, taskId);

    protected string GetTempFileName() => Path.Combine(FileSpec.GetTempDirectory(), Guid.NewGuid().ToString("N"));
  }
}
