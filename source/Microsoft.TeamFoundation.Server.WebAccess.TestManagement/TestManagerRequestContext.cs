// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagerRequestContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestManagerRequestContext
  {
    private TfsTestManagementRequestContext m_testContext;
    private CachedIdentitiesCollection m_cachedIdentities;
    private string m_ProjectName;
    private WebApiTeam m_team;
    private Guid m_ProjectGuid;

    internal TestManagerRequestContext(TfsController controller)
    {
      this.Controller = controller;
      this.TfsWebContext = controller.TfsWebContext;
      this.TfsRequestContext = controller.TfsRequestContext;
    }

    internal TestManagerRequestContext(
      IVssRequestContext tfsRequestContext,
      string projectName,
      Guid projectGuid,
      WebApiTeam team)
    {
      this.TfsRequestContext = tfsRequestContext;
      this.m_testContext = new TfsTestManagementRequestContext(this.TfsRequestContext);
      this.ProjectName = projectName;
      this.m_ProjectGuid = projectGuid;
      this.m_team = team;
    }

    public TestManagerRequestContext()
    {
    }

    public virtual TfsTestManagementRequestContext TestRequestContext
    {
      get
      {
        if (this.m_testContext == null)
          this.m_testContext = new TfsTestManagementRequestContext(this.TfsRequestContext);
        return this.m_testContext;
      }
    }

    public virtual IVssRequestContext TfsRequestContext { get; private set; }

    public virtual WebApiTeam Team
    {
      get
      {
        if (this.m_team == null)
          this.m_team = this.TfsWebContext?.Team;
        if (this.m_team == null && !this.TfsRequestContext.IsHostProcessType(HostProcessType.JobAgent))
        {
          IWebTeamContext context;
          this.TfsRequestContext.TryGetWebTeamContextWithoutGlobalContext(out context);
          this.m_team = context?.Team;
        }
        return this.m_team;
      }
      private set => this.m_team = value;
    }

    public virtual TfsWebContext TfsWebContext { get; private set; }

    internal CachedIdentitiesCollection Identities
    {
      get
      {
        if (this.m_cachedIdentities == null)
          this.m_cachedIdentities = new CachedIdentitiesCollection(this);
        return this.m_cachedIdentities;
      }
    }

    internal TfsController Controller { get; private set; }

    public virtual string ProjectName
    {
      get
      {
        if (this.m_ProjectName == null && this.TfsWebContext != null)
          this.m_ProjectName = this.TfsWebContext.ProjectContext == null ? (string) null : this.TfsWebContext.ProjectContext.Name;
        return this.m_ProjectName;
      }
      private set => this.m_ProjectName = value;
    }

    public virtual Guid CurrentProjectGuid
    {
      get
      {
        if (this.m_ProjectGuid == Guid.Empty)
          this.m_ProjectGuid = this.TfsWebContext.CurrentProjectGuid == Guid.Empty ? Guid.Empty : this.TfsWebContext.CurrentProjectGuid;
        return this.m_ProjectGuid;
      }
      private set => this.m_ProjectGuid = value;
    }

    internal ResultsStoreQuery GetResultsStoreQuery(string queryText) => new ResultsStoreQuery()
    {
      TeamProjectName = this.ProjectName,
      TimeZone = TimeZoneInfo.Utc.ToSerializedString(),
      QueryText = queryText
    };
  }
}
