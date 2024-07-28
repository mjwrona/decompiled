// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ImpactedPoint
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class ImpactedPoint
  {
    private static IBuildServiceHelper m_buildServiceHelper;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string BuildUri { get; set; }

    [XmlAttribute]
    public int TestCaseId { get; set; }

    [XmlAttribute]
    public int PointId { get; set; }

    [XmlAttribute]
    public byte Confidence { get; set; }

    [XmlAttribute]
    public byte State { get; set; }

    [XmlAttribute]
    public string SuiteName { get; set; }

    internal static List<ImpactedPoint> QueryImpactedPointsForPlan(
      TestManagementRequestContext context,
      string teamProjectName,
      int planId,
      string buildUri)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, teamProjectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new List<ImpactedPoint>();
      Microsoft.TeamFoundation.Build.WebApi.Build build = ImpactedPoint.BuildServiceHelper.QueryBuildByUri(context.RequestContext, projectFromName.GuidId, buildUri, true);
      if (build == null)
        return new List<ImpactedPoint>();
      string testPlanName = ImpactedPoint.GetTestPlanName(context, teamProjectName, planId);
      using (TestPlanningDatabase planningDatabase1 = TestPlanningDatabase.Create(context))
      {
        TestPlanningDatabase planningDatabase2 = planningDatabase1;
        Guid guidId = projectFromName.GuidId;
        int planId1 = planId;
        string planName = testPlanName;
        string buildDefinitionUri = build.Definition.Uri.ToString();
        DateTime? startTime = build.StartTime;
        DateTime buildStartDate;
        if (!startTime.HasValue)
        {
          buildStartDate = DateTime.UtcNow;
        }
        else
        {
          startTime = build.StartTime;
          buildStartDate = startTime.Value.ToUniversalTime();
        }
        return planningDatabase2.QueryImpactedPointsForPlan(guidId, planId1, planName, buildDefinitionUri, buildStartDate);
      }
    }

    internal static IBuildServiceHelper BuildServiceHelper
    {
      get => ImpactedPoint.m_buildServiceHelper ?? (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
      set => ImpactedPoint.m_buildServiceHelper = value;
    }

    private static string GetTestPlanName(
      TestManagementRequestContext context,
      string projectName,
      int planId)
    {
      List<WorkItem> list = context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, new List<int>()
      {
        planId
      }, new List<string>() { "System.Title" }).ToList<WorkItem>();
      string str;
      return list != null && list.Count == 1 && list.First<WorkItem>().Fields.TryGetValue<string, string>("System.Title", out str) ? str : string.Empty;
    }
  }
}
