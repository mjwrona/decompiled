// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.BacklogsService
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
  public class BacklogsService : IBacklogsService, IVssFrameworkService
  {
    private const string c_RequestContextBacklogConfigurationKey = "RequestContextBacklogConfigurationKey";

    private List<Process> GetProcesses(IVssRequestContext requestContext, Guid project)
    {
      using (AnalyticsMetadataComponent component = requestContext.CreateComponent<AnalyticsMetadataComponent>())
        return component.GetProcesses((IList<Guid>) new List<Guid>()
        {
          project
        });
    }

    public string GetRequirementBacklogName(IVssRequestContext requestContext, Guid project) => this.GetProcesses(requestContext, project).Where<Process>((Func<Process, bool>) (process => process.BacklogType == "RequirementBacklog")).Select<Process, string>((Func<Process, string>) (process => process.BacklogName)).FirstOrDefault<string>();

    public string GetIterationBacklogName(IVssRequestContext requestContext, Guid project) => this.GetProcesses(requestContext, project).Where<Process>((Func<Process, bool>) (process => process.BacklogType == "IterationBacklog")).Select<Process, string>((Func<Process, string>) (process => process.BacklogName)).FirstOrDefault<string>();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
