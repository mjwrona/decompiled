// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobTemplateCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobTemplateCacheService : VssBaseService, IVssFrameworkService
  {
    private Dictionary<Guid, TeamFoundationJobDefinitionTemplate> m_jobTemplates;
    private ILockName m_lock;
    private long m_sequenceId;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_lock = this.CreateLockName(systemRequestContext, nameof (JobTemplateCacheService));
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.JobTemplatesChanged, new SqlNotificationHandler(this.OnJobTemplatesChanged), false);
      this.RefreshCache(systemRequestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.JobTemplatesChanged, new SqlNotificationHandler(this.OnJobTemplatesChanged), false);

    public virtual Dictionary<Guid, TeamFoundationJobDefinitionTemplate> GetJobTemplates(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, TeamFoundationJobDefinitionTemplate> location1 = (Dictionary<Guid, TeamFoundationJobDefinitionTemplate>) null;
      Interlocked.Exchange<Dictionary<Guid, TeamFoundationJobDefinitionTemplate>>(ref location1, this.m_jobTemplates);
      return location1;
    }

    private void OnJobTemplatesChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      this.RefreshCache(requestContext);
    }

    internal void RefreshCache(IVssRequestContext requestContext)
    {
      long sequenceId;
      Dictionary<Guid, TeamFoundationJobDefinitionTemplate> dictionary;
      using (JobTemplateComponent component = requestContext.CreateComponent<JobTemplateComponent>())
        dictionary = component.QueryJobTemplates(false, out sequenceId).ToDictionary<TeamFoundationJobDefinitionTemplate, Guid>((Func<TeamFoundationJobDefinitionTemplate, Guid>) (x => x.JobId));
      using (requestContext.AcquireWriterLock(this.m_lock))
      {
        if (this.m_jobTemplates != null && this.m_sequenceId >= sequenceId)
          return;
        Interlocked.Exchange<Dictionary<Guid, TeamFoundationJobDefinitionTemplate>>(ref this.m_jobTemplates, dictionary);
        this.m_sequenceId = sequenceId;
      }
    }
  }
}
