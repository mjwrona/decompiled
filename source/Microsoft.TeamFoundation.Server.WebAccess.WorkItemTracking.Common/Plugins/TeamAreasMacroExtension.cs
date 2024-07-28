// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Plugins.TeamAreasMacroExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Plugins
{
  public class TeamAreasMacroExtension : QueryMacroExtension
  {
    public override string Name => "TeamAreas";

    public override object DefaultValue => (object) string.Empty;

    public override DataType DataType => DataType.String;

    public override void Validate(
      IVssRequestContext requestContext,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters)
    {
      this.ParseParameters(requestContext, parameters, out string _);
    }

    private void ParseParameters(
      IVssRequestContext requestContext,
      NodeParameters parameters,
      out string teamName)
    {
      if (parameters.Arguments.Count != 1)
        throw new SyntaxException(Resources.TeamAreas_Usages);
      if (!(parameters.Arguments[0] is NodeString nodeString))
        throw new SyntaxException(Resources.TeamAreas_StringTeamParameter);
      teamName = nodeString.Value;
    }

    public override object GetValue(
      IVssRequestContext requestContext,
      string macro,
      ProjectInfo project,
      WebApiTeam team = null,
      NodeParameters parameters = null,
      TimeZone timeZone = null,
      CultureInfo cultureInfo = null)
    {
      return (object) null;
    }

    public override bool Rewrite(
      IVssRequestContext requestContext,
      NodeCondition condition,
      out Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node rewritten)
    {
      rewritten = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) null;
      NodeVariable teamAreasNode;
      List<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node> otherValues;
      if (!this.CanProcess(requestContext, condition, out teamAreasNode, out otherValues))
        return false;
      ICollection<string> areasWithChildren;
      ICollection<string> exactAreas;
      this.GetTeamAreas(requestContext, teamAreasNode.Parameters, out areasWithChildren, out exactAreas);
      List<NodeCondition> source = new List<NodeCondition>();
      if (otherValues.Any<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>() || exactAreas.Any<string>())
      {
        NodeValueList right = new NodeValueList();
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in otherValues)
          right.Add(node);
        foreach (string s in (IEnumerable<string>) exactAreas)
          right.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(s));
        source.Add(new NodeCondition(Condition.In, condition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right));
      }
      foreach (string s in (IEnumerable<string>) areasWithChildren)
        source.Add(new NodeCondition(Condition.Under, condition.Left, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(s)));
      if (source.Count == 1)
      {
        rewritten = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) source.First<NodeCondition>();
      }
      else
      {
        NodeOrOperator nodeOrOperator = new NodeOrOperator();
        foreach (NodeCondition nodeCondition in source)
          nodeOrOperator.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeCondition);
        rewritten = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeOrOperator;
      }
      if (condition.Condition == Condition.NotEquals)
        rewritten = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNotOperator(rewritten);
      return true;
    }

    private bool CanProcess(
      IVssRequestContext requestContext,
      NodeCondition condition,
      out NodeVariable teamAreasNode,
      out List<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node> otherValues)
    {
      teamAreasNode = (NodeVariable) null;
      otherValues = new List<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>();
      if (condition.Right is NodeVariable right1 && string.Equals(right1.Value, this.Name, StringComparison.OrdinalIgnoreCase))
      {
        if (condition.Condition != Condition.Equals && condition.Condition != Condition.NotEquals)
          throw new SyntaxException(Resources.TeamAreas_AllowedOperators);
        teamAreasNode = right1;
        return true;
      }
      if (!(condition.Right is NodeValueList right2))
        return false;
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right2)
      {
        if (teamAreasNode == null && node is NodeVariable)
        {
          NodeVariable nodeVariable = node as NodeVariable;
          if (string.Equals(nodeVariable.Value, this.Name, StringComparison.OrdinalIgnoreCase))
          {
            teamAreasNode = nodeVariable;
            continue;
          }
        }
        otherValues.Add(node);
      }
      if (teamAreasNode != null && condition.Condition != Condition.In)
        throw new SyntaxException(Resources.TeamAreas_AllowedOperators);
      return teamAreasNode != null;
    }

    private void GetTeamAreas(
      IVssRequestContext requestContext,
      NodeParameters parameters,
      out ICollection<string> areasWithChildren,
      out ICollection<string> exactAreas)
    {
      areasWithChildren = (ICollection<string>) new List<string>();
      exactAreas = (ICollection<string>) new List<string>();
      string teamName;
      this.ParseParameters(requestContext, parameters, out teamName);
      ProjectInfo project;
      WebApiTeam team;
      MacroArgumentHelper.ParseTeam(requestContext, teamName, out project, out team);
      if (!requestContext.GetService<ProjectConfigurationService>().IsTeamFieldAreaPath(requestContext, project.Uri))
        throw new WorkItemTrackingQueryException(string.Format(Resources.TeamAreas_AreaPathOnly, (object) project.Name));
      ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, false);
      if (teamSettings == null || teamSettings.TeamFieldConfig == null || teamSettings.TeamFieldConfig.TeamFieldValues.Length == 0)
        throw new WorkItemTrackingQueryException(string.Format(Resources.TeamAreas_TeamHasNoAreaPaths, (object) team.Name));
      areasWithChildren = (ICollection<string>) ((IEnumerable<ITeamFieldValue>) teamSettings.TeamFieldConfig.TeamFieldValues).Where<ITeamFieldValue>((Func<ITeamFieldValue, bool>) (v => v.IncludeChildren)).Select<ITeamFieldValue, string>((Func<ITeamFieldValue, string>) (fieldValue => fieldValue.Value)).ToList<string>();
      exactAreas = (ICollection<string>) ((IEnumerable<ITeamFieldValue>) teamSettings.TeamFieldConfig.TeamFieldValues).Where<ITeamFieldValue>((Func<ITeamFieldValue, bool>) (v => !v.IncludeChildren)).Select<ITeamFieldValue, string>((Func<ITeamFieldValue, string>) (fieldValue => fieldValue.Value)).ToList<string>();
    }
  }
}
