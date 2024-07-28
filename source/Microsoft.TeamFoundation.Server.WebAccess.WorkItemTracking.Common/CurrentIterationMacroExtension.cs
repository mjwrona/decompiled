// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CurrentIterationMacroExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Plugins;
using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Globalization;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class CurrentIterationMacroExtension : QueryMacroExtension
  {
    public const string c_extName = "CurrentIteration";

    public override string Name => "CurrentIteration";

    public override object DefaultValue => (object) string.Empty;

    public override DataType DataType => DataType.String;

    public override bool DoesMacroExtensionHandleOffset => true;

    public override void Validate(
      IVssRequestContext requestContext,
      NodeTableName tableContext,
      NodeFieldName fieldContext,
      NodeParameters parameters)
    {
      this.ParseParameters(requestContext, parameters, out string _, out int _);
    }

    private bool ParseParameters(
      IVssRequestContext requestContext,
      NodeParameters parameters,
      out string team,
      out int offset)
    {
      team = (string) null;
      offset = 0;
      if (parameters.Arguments.Count == 0 && parameters.Offset == 0.0)
        return false;
      if (parameters.Arguments.Count > 1)
        throw new SyntaxException(Resources.CurrentIterationOnlyAccepts1Argument);
      if (parameters.Arguments.Count == 1)
      {
        if (!(parameters.Arguments[0] is NodeString))
          throw new SyntaxException(Resources.CurrentIterationArgumentMustBeAString);
        team = (parameters.Arguments[0] as NodeString).Value;
      }
      offset = (int) parameters.Offset;
      if ((double) offset != parameters.Offset)
        throw new SyntaxException(Resources.CurrentIterationOffsetMustBeAnInteger);
      return true;
    }

    private string FormatOffset(int offset)
    {
      if (offset == 0)
        return "";
      return offset >= 0 ? string.Format(" + {0}", (object) offset) : string.Format(" - {0}", (object) -offset);
    }

    public override object GetValue(
      IVssRequestContext requestContext,
      string macro,
      ProjectInfo project,
      WebApiTeam team,
      NodeParameters parameters,
      TimeZone timeZone = null,
      CultureInfo cultureInfo = null)
    {
      string projectName = project?.Name ?? "";
      string team1;
      int offset;
      if (this.ParseParameters(requestContext, parameters, out team1, out offset) && team1 != null)
      {
        MacroArgumentHelper.ParseTeam(requestContext, team1, out project, out team);
        projectName = project.Name;
      }
      if (team == null)
        throw new WorkItemTrackingQueryException(string.Format(Resources.QueryFilterInvalidMacroNoTeam, (object) ("@" + CommonClientResourceStrings.WiqlOperators_MacroCurrentIteration)));
      string iteration;
      if (!this.TryGetIteration(requestContext, team, project, offset, out iteration))
      {
        object adminIterationUrl = this.GetAdminIterationUrl(requestContext, projectName, team.Name);
        throw new WorkItemTrackingQueryException(string.Format(Resources.Validation_IterationNotSet, (object) projectName, (object) team.Name, (object) this.FormatOffset(offset), adminIterationUrl));
      }
      return (object) iteration;
    }

    private object GetAdminIterationUrl(
      IVssRequestContext requestContext,
      string projectName,
      string teamName)
    {
      return (object) requestContext.GetService<IContributionRoutingService>().RouteUrl(requestContext, "ms.vss-work-web.admin-work-route", new RouteValueDictionary()
      {
        ["_a"] = (object) "iterations",
        ["team"] = (object) teamName,
        ["project"] = (object) projectName
      });
    }

    private bool TryGetIteration(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ProjectInfo project,
      int offset,
      out string iteration)
    {
      ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, false);
      if (teamSettings == null || teamSettings.Iterations == null)
      {
        iteration = (string) null;
        return false;
      }
      SortedIterationSubscriptions iterationTimeline = teamSettings.GetIterationTimeline(requestContext, project.Id);
      if (!iterationTimeline.IsValidSubscription)
      {
        iteration = (string) null;
        return false;
      }
      int index = iterationTimeline.CurrentIterationIndex + offset;
      if (index < 0 || index > iterationTimeline.Iterations.Count - 1)
      {
        iteration = (string) null;
        return false;
      }
      iteration = iterationTimeline.Iterations[index].GetPath(requestContext);
      return true;
    }
  }
}
