// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskGroupExtensionsRetriever
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class TaskGroupExtensionsRetriever
  {
    private const string TemplatesContributionId = "ms.vss-taskgroup.tg-templates";

    public static List<TaskGroup> GetAllTaskGroupTemplates(IVssRequestContext requestContext)
    {
      List<TaskGroup> taskGroupTemplates = new List<TaskGroup>();
      foreach (Contribution templateContribution in TaskGroupExtensionsRetriever.GetInstalledTemplateContributions(requestContext))
      {
        try
        {
          List<TaskGroup> collection = JsonConvert.DeserializeObject<List<TaskGroup>>(templateContribution.Properties["values"].ToString());
          collection.ForEach((Action<TaskGroup>) (x => x.ContributionIdentifier = "ms.vss-taskgroup.tg-templates"));
          taskGroupTemplates.AddRange((IEnumerable<TaskGroup>) collection);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10015104, TraceLevel.Error, "TaskGroup", "TaskGroupFromExtensions", ex);
        }
      }
      return taskGroupTemplates;
    }

    private static IEnumerable<Contribution> GetInstalledTemplateContributions(
      IVssRequestContext requestContext)
    {
      try
      {
        return requestContext.GetService<IContributionService>().QueryContributionsForTarget(requestContext, "ms.vss-taskgroup.tg-templates");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015104, TraceLevel.Error, "TaskGroup", "TaskService", ex);
        return (IEnumerable<Contribution>) new List<Contribution>();
      }
    }
  }
}
