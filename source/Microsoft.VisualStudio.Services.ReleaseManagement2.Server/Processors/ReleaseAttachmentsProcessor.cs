// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ReleaseAttachmentsProcessor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public class ReleaseAttachmentsProcessor
  {
    private readonly Func<IVssRequestContext, Guid, DeployPhaseTypes, IDistributedTaskOrchestrator> getDistributedTaskOrchestrator;

    public ReleaseAttachmentsProcessor()
      : this(ReleaseAttachmentsProcessor.\u003C\u003EO.\u003C0\u003E__CreateDistributedTaskOrchestrator ?? (ReleaseAttachmentsProcessor.\u003C\u003EO.\u003C0\u003E__CreateDistributedTaskOrchestrator = new Func<IVssRequestContext, Guid, DeployPhaseTypes, IDistributedTaskOrchestrator>(DistributedTaskOrchestrator.CreateDistributedTaskOrchestrator)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected ReleaseAttachmentsProcessor(
      Func<IVssRequestContext, Guid, DeployPhaseTypes, IDistributedTaskOrchestrator> getDistributedTaskOrchestratorFunc)
    {
      this.getDistributedTaskOrchestrator = getDistributedTaskOrchestratorFunc;
    }

    public IEnumerable<TaskAttachment> GetAttachments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      DeployPhaseTypes phaseType,
      string type)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return this.getDistributedTaskOrchestrator(requestContext, projectId, phaseType).GetAttachments(planId, type);
    }

    public Stream GetAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      DeployPhaseTypes phaseType,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return this.getDistributedTaskOrchestrator(requestContext, projectId, phaseType).GetAttachment(planId, new TaskAttachment(type, name)
      {
        TimelineId = timelineId,
        RecordId = recordId
      });
    }
  }
}
