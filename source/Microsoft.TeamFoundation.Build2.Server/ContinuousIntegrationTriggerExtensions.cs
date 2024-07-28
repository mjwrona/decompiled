// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTriggerExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ContinuousIntegrationTriggerExtensions
  {
    public static void ConvertFilterPathsToProjectId(
      this ContinuousIntegrationTrigger trigger,
      IVssRequestContext requestContext)
    {
      ContinuousIntegrationTriggerExtensions.ConvertPathsToProjectId(requestContext, trigger.BranchFilters);
      ContinuousIntegrationTriggerExtensions.ConvertPathsToProjectId(requestContext, trigger.PathFilters);
    }

    private static void ConvertPathsToProjectId(
      IVssRequestContext requestContext,
      List<string> paths)
    {
      for (int index = 0; index < paths.Count; ++index)
      {
        string path = paths[index];
        paths[index] = path.Substring(0, 1) + TFVCPathHelper.ConvertToPathWithProjectGuid(requestContext, path.Substring(1));
      }
    }

    public static void ConvertFilterPathsToProjectName(
      this ContinuousIntegrationTrigger trigger,
      IVssRequestContext requestContext)
    {
      ContinuousIntegrationTriggerExtensions.ConvertPathsToProjectName(requestContext, trigger.BranchFilters);
      ContinuousIntegrationTriggerExtensions.ConvertPathsToProjectName(requestContext, trigger.PathFilters);
    }

    private static void ConvertPathsToProjectName(
      IVssRequestContext requestContext,
      List<string> paths)
    {
      for (int index = 0; index < paths.Count; ++index)
      {
        string path = paths[index];
        paths[index] = path.Substring(0, 1) + TFVCPathHelper.ConvertToPathWithProjectName(requestContext, path.Substring(1));
      }
    }

    public static bool IsPollingEnabled(this ContinuousIntegrationTrigger trigger)
    {
      if (trigger != null && trigger.PollingInterval.HasValue)
      {
        int? pollingInterval1 = trigger.PollingInterval;
        int num1 = 60;
        if (pollingInterval1.GetValueOrDefault() >= num1 & pollingInterval1.HasValue)
        {
          int? pollingInterval2 = trigger.PollingInterval;
          int num2 = 86400;
          if (pollingInterval2.GetValueOrDefault() <= num2 & pollingInterval2.HasValue)
            return true;
        }
      }
      return false;
    }

    public static void GetLatestValuesFromPollingJob(
      this ContinuousIntegrationTrigger trigger,
      IVssRequestContext requestContext,
      out string lastSourceVersion,
      out string currentConnectionId,
      out string lastFailedBuildDate)
    {
      lastSourceVersion = string.Empty;
      currentConnectionId = string.Empty;
      lastFailedBuildDate = string.Empty;
      if (!trigger.IsPollingEnabled())
        return;
      TeamFoundationJobDefinition foundationJobDefinition = requestContext.Elevate().GetService<ITeamFoundationJobService>().QueryJobDefinition(requestContext, trigger.PollingJobId);
      if (foundationJobDefinition == null || foundationJobDefinition.Data == null || !(foundationJobDefinition.Data.LocalName == "BuildDefinition"))
        return;
      foreach (XmlNode selectNode in foundationJobDefinition.Data.SelectNodes("/*"))
      {
        if (string.Equals(selectNode.LocalName, "LastVersionEvaluated", StringComparison.OrdinalIgnoreCase))
          lastSourceVersion = selectNode.InnerText;
        else if (string.Equals(selectNode.LocalName, "CurrentConnectionId", StringComparison.OrdinalIgnoreCase))
          currentConnectionId = selectNode.InnerText;
        else if (string.Equals(selectNode.LocalName, "LastFailedBuildDateTime", StringComparison.OrdinalIgnoreCase))
          lastFailedBuildDate = selectNode.InnerText;
      }
    }
  }
}
