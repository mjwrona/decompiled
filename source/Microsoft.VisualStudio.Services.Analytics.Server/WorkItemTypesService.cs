// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemTypesService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class WorkItemTypesService : IWorkItemTypesService, IVssFrameworkService
  {
    private List<Process> GetProcesses(IVssRequestContext requestContext, IList<Guid> projects)
    {
      using (AnalyticsMetadataComponent component = requestContext.CreateComponent<AnalyticsMetadataComponent>())
        return component.GetProcesses(projects);
    }

    public List<string> GetWorkItemTypesForBacklogs(
      IVssRequestContext requestContext,
      IList<Guid> projects,
      IList<string> backlogs)
    {
      return this.GetProcesses(requestContext, projects).Where<Process>((Func<Process, bool>) (p => backlogs.Contains(p.BacklogName))).Select<Process, string>((Func<Process, string>) (p => p.WorkItemType)).Distinct<string>().ToList<string>();
    }

    public List<string> GetBugWorkItemTypes(IVssRequestContext requestContext, IList<Guid> projects) => this.GetProcesses(requestContext, projects).Where<Process>((Func<Process, bool>) (process => process.IsBugType)).Select<Process, string>((Func<Process, string>) (process => process.WorkItemType)).Distinct<string>().ToList<string>();

    public List<string> GetWorkItemTypesNotInTheHiddenCategory(
      IVssRequestContext requestContext,
      IList<Guid> projects)
    {
      return this.GetProcesses(requestContext, projects).Where<Process>((Func<Process, bool>) (process => !process.IsHiddenType)).Select<Process, string>((Func<Process, string>) (process => process.WorkItemType)).Distinct<string>().ToList<string>();
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
