// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessTemplateExtensionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessTemplateExtensionService : 
    IProcessTemplateExtensionService,
    IVssFrameworkService
  {
    private IDisposableReadOnlyList<ProcessTemplateExtension> m_processTemplateExtensions;

    public void ServiceStart(IVssRequestContext requestContext) => this.m_processTemplateExtensions = requestContext.To(TeamFoundationHostType.Deployment).GetExtensions<ProcessTemplateExtension>();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      if (this.m_processTemplateExtensions == null)
        return;
      this.m_processTemplateExtensions.Dispose();
      this.m_processTemplateExtensions = (IDisposableReadOnlyList<ProcessTemplateExtension>) null;
    }

    public void RunProjectProcessMigrationReadinessChecks(
      IVssRequestContext requestContext,
      ProjectInfo projectToMigrate,
      ProcessDescriptor sourceProcess,
      ProcessDescriptor targetProcess)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProjectInfo>(projectToMigrate, nameof (projectToMigrate));
      if (this.m_processTemplateExtensions == null || !this.m_processTemplateExtensions.Any<ProcessTemplateExtension>())
        return;
      foreach (ProcessTemplateExtension templateExtension in (IEnumerable<ProcessTemplateExtension>) this.m_processTemplateExtensions)
        templateExtension.CheckProjectIsReadyForMigration(requestContext, projectToMigrate, sourceProcess, targetProcess);
    }

    public void QueueDeleteTemplateOperations(IVssRequestContext requestContext, Guid processTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_processTemplateExtensions == null || !this.m_processTemplateExtensions.Any<ProcessTemplateExtension>())
        return;
      foreach (ProcessTemplateExtension templateExtension in (IEnumerable<ProcessTemplateExtension>) this.m_processTemplateExtensions)
      {
        ProcessTemplateExtension extension = templateExtension;
        this.QueueInTaskService(requestContext, (TeamFoundationTaskCallback) ((ctx, arg) => extension.OnDeleteTemplate(ctx, (Guid) arg)), (object) processTypeId);
      }
    }

    public void QueueMigrateProjectsProcessOperations(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> migratedProjects)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ProjectInfo>>(migratedProjects, nameof (migratedProjects));
      if (this.m_processTemplateExtensions == null || !this.m_processTemplateExtensions.Any<ProcessTemplateExtension>())
        return;
      foreach (ProcessTemplateExtension templateExtension in (IEnumerable<ProcessTemplateExtension>) this.m_processTemplateExtensions)
      {
        ProcessTemplateExtension extension = templateExtension;
        this.QueueInTaskService(requestContext, (TeamFoundationTaskCallback) ((ctx, arg) => extension.OnMigrateProjectsProcess(ctx, arg as IEnumerable<ProjectInfo>)), (object) migratedProjects);
      }
    }

    private void QueueInTaskService(
      IVssRequestContext requestContext,
      TeamFoundationTaskCallback callback,
      object taskArgs)
    {
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(callback, taskArgs, 0));
    }
  }
}
