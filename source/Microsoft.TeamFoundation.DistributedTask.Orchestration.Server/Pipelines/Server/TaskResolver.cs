// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.TaskResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class TaskResolver : ITaskResolver
  {
    private readonly IVssRequestContext m_requestContext;

    public TaskResolver(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public TaskDefinition Resolve(Guid taskId, string versionSpec)
    {
      ArgumentUtility.CheckForEmptyGuid(taskId, nameof (taskId));
      return this.m_requestContext.GetService<IDistributedTaskPoolService>().GetTaskDefinition(this.m_requestContext, taskId, versionSpec);
    }
  }
}
