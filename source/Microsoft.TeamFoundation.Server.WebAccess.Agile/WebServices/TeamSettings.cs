// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.TeamSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class TeamSettings
  {
    private ITeamSettings m_teamSettings;
    private string m_backlogIterationPath;
    private string[] m_iterationPaths;
    private bool m_backlogIterationPathInitialized;

    internal TeamSettings() => this.m_iterationPaths = Array.Empty<string>();

    internal TeamSettings(
      IVssRequestContext requestContext,
      ITeamSettings teamSettings,
      string teamField)
    {
      this.TeamField = teamField;
      this.m_teamSettings = teamSettings;
      this.SetTeamFieldValues(teamSettings);
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string BacklogIterationPath
    {
      get
      {
        if (!this.m_backlogIterationPathInitialized)
          throw new InvalidOperationException("BacklogIterationPath must be populated prior to access");
        return this.m_backlogIterationPath;
      }
      set
      {
        this.m_backlogIterationPathInitialized = true;
        this.m_backlogIterationPath = value;
      }
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string[] IterationPaths
    {
      get => this.m_iterationPaths;
      set => this.m_iterationPaths = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public TeamFieldValue[] TeamFieldValues { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string TeamField { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string CurrentIterationPath { get; set; }

    internal void PopulateIterations(
      IVssRequestContext requestContext,
      string projectUri,
      IDictionary<Guid, CommonStructureNodeInfo> nodeMap)
    {
      CommonStructureNodeInfo node1 = (CommonStructureNodeInfo) null;
      if (this.m_teamSettings.BacklogIterationId != Guid.Empty && nodeMap.TryGetValue(this.m_teamSettings.BacklogIterationId, out node1))
        this.BacklogIterationPath = node1.GetWitPath();
      List<CommonStructureNodeInfo> list = this.m_teamSettings.Iterations.Select<ITeamIteration, CommonStructureNodeInfo>((Func<ITeamIteration, CommonStructureNodeInfo>) (i =>
      {
        CommonStructureNodeInfo structureNodeInfo = (CommonStructureNodeInfo) null;
        nodeMap.TryGetValue(i.IterationId, out structureNodeInfo);
        return structureNodeInfo;
      })).Where<CommonStructureNodeInfo>((Func<CommonStructureNodeInfo, bool>) (node => node != null)).ToList<CommonStructureNodeInfo>();
      this.IterationPaths = list.Select<CommonStructureNodeInfo, string>((Func<CommonStructureNodeInfo, string>) (node => node.GetWitPath())).ToArray<string>();
      if (list.Any<CommonStructureNodeInfo>())
      {
        Guid projectIdFromUri = CommonStructureUtils.GetProjectIdFromUri(projectUri);
        this.CurrentIterationPath = this.m_teamSettings.GetCurrentIterationNode(requestContext, projectIdFromUri)?.GetPath(requestContext);
      }
      this.m_backlogIterationPathInitialized = true;
    }

    internal void SetTeamFieldValues(ITeamSettings teamSettings)
    {
      if (teamSettings.TeamFieldConfig != null && teamSettings.TeamFieldConfig.TeamFieldValues != null && teamSettings.TeamFieldConfig.TeamFieldValues.Length != 0)
      {
        ITeamFieldValue[] teamFieldValues = teamSettings.TeamFieldConfig.TeamFieldValues;
        int defaultValueIndex = teamSettings.TeamFieldConfig.DefaultValueIndex;
        if (!teamSettings.TeamFieldConfig.IsDefaultIndexValid())
          defaultValueIndex = 0;
        this.TeamFieldValues = ((IEnumerable<TeamFieldValue>) new TeamFieldValue[1]
        {
          (TeamFieldValue) ((IEnumerable<ITeamFieldValue>) teamFieldValues).ElementAt<ITeamFieldValue>(defaultValueIndex)
        }).Concat<TeamFieldValue>(((IEnumerable<ITeamFieldValue>) teamFieldValues).Where<ITeamFieldValue>((Func<ITeamFieldValue, int, bool>) ((value, index) => index != defaultValueIndex)).Cast<TeamFieldValue>()).ToArray<TeamFieldValue>();
      }
      else
        this.TeamFieldValues = Array.Empty<TeamFieldValue>();
    }
  }
}
