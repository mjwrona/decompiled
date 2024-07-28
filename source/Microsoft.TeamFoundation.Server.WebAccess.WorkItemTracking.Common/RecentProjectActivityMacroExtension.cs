// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.RecentProjectActivityMacroExtension
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.RecentActivity;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class RecentProjectActivityMacroExtension : QueryMacroExtension
  {
    private const string Area = "RecentProjectActivityMacroExtension";
    private const string Layer = "WorkItemTracking.Common.Plugins";
    private const string EmptyList = "0";
    private const int RecentProjectActivityUnknownExceptionTracePoint = 15162005;
    private const string RecentProjectActivityMacro = "RecentProjectActivity";

    public override string Name => "RecentProjectActivity";

    public override DataType DataType => DataType.Numeric;

    public override object DefaultValue
    {
      get
      {
        NodeValueList defaultValue = new NodeValueList();
        defaultValue.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber("0"));
        return (object) defaultValue;
      }
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
      if (project == null)
        throw new WorkItemTrackingQueryException(string.Format(Resources.QueryFilterInvalidMacroNoProject, (object) ("@" + CommonClientResourceStrings.WiqlOperators_MacroRecentProjectActivity)));
      try
      {
        int limit = CommonWITUtils.HasCrossProjectQueryPermission(requestContext) ? requestContext.WitContext().ServerSettings.MaxQueryResultSize : requestContext.WitContext().ServerSettings.MaxQueryResultSizeForPublicUser;
        IEnumerable<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>> projectActivities = (IEnumerable<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>>) requestContext.GetService<ITeamFoundationRecentActivityService>().GetProjectActivities(requestContext, project.Id, WorkItemArtifactKinds.WorkItem, limit);
        if (projectActivities == null || !projectActivities.Any<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>>())
          return this.DefaultValue;
        NodeValueList nodeValueList = new NodeValueList();
        foreach (KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> keyValuePair in projectActivities)
          nodeValueList.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeNumber(keyValuePair.Key));
        return (object) nodeValueList;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15162005, nameof (RecentProjectActivityMacroExtension), "WorkItemTracking.Common.Plugins", ex);
        throw;
      }
    }
  }
}
