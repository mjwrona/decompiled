// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.IterationQueryBuilder
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public class IterationQueryBuilder : AgileBaseQueryBuilder
  {
    private const string TaskBoard = "\r\nSELECT [{0}]\r\nFROM WorkItemLinks\r\nWHERE {1}\r\n    {2} AND\r\n    Source.[System.IterationPath] UNDER '{3}' AND\r\n    {4} AND\r\n    [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward' AND\r\n    {5} AND\r\n    Target.[System.IterationPath] UNDER '{6}' AND\r\n    {7}\r\nORDER BY [{8}] ASC\r\nMODE (Recursive, ReturnMatchingChildren)";
    private IEnumerable<BacklogLevelConfiguration> m_sourceConfigurations;
    private IEnumerable<BacklogLevelConfiguration> m_targetConfigurations;
    private WorkItemStateCategory[] m_stateTypes;
    private IEnumerable<string> m_fields;
    private string m_iterationPath;
    private string m_backlogIterationPath;

    public IterationQueryBuilder(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      IEnumerable<BacklogLevelConfiguration> sourceConfigurations,
      IEnumerable<BacklogLevelConfiguration> targetConfigurations,
      WorkItemStateCategory[] stateTypes,
      IEnumerable<string> fields,
      string iterationPath,
      string backlogIterationPath)
      : base(settings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForNull<IEnumerable<BacklogLevelConfiguration>>(sourceConfigurations, nameof (sourceConfigurations));
      ArgumentUtility.CheckForNull<IEnumerable<BacklogLevelConfiguration>>(targetConfigurations, nameof (targetConfigurations));
      ArgumentUtility.CheckForNull<WorkItemStateCategory[]>(stateTypes, nameof (stateTypes));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(fields, nameof (fields));
      ArgumentUtility.CheckStringForNullOrEmpty(iterationPath, "requestIteration");
      ArgumentUtility.CheckStringForNullOrEmpty(backlogIterationPath, nameof (backlogIterationPath));
      this.m_sourceConfigurations = sourceConfigurations;
      this.m_targetConfigurations = targetConfigurations;
      this.m_stateTypes = stateTypes;
      this.m_fields = fields;
      this.m_iterationPath = iterationPath;
      this.m_backlogIterationPath = backlogIterationPath;
    }

    public override string GetQuery()
    {
      string str1 = string.Join("],[", this.m_fields);
      string str2 = string.Join("] ASC,[", ((IEnumerable<string>) new string[2]
      {
        this.m_settings.Process.OrderByField.Name,
        "System.Id"
      }).Distinct<string>());
      string parentFilterCriteria = this.ParentFilterCriteria;
      string configurationClause1 = this.GetWorkItemTypeAndStatePerBacklogConfigurationClause(this.m_stateTypes, "Source", this.m_sourceConfigurations);
      string configurationClause2 = this.GetWorkItemTypeAndStatePerBacklogConfigurationClause(this.m_stateTypes, "Target", this.m_targetConfigurations);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\nSELECT [{0}]\r\nFROM WorkItemLinks\r\nWHERE {1}\r\n    {2} AND\r\n    Source.[System.IterationPath] UNDER '{3}' AND\r\n    {4} AND\r\n    [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward' AND\r\n    {5} AND\r\n    Target.[System.IterationPath] UNDER '{6}' AND\r\n    {7}\r\nORDER BY [{8}] ASC\r\nMODE (Recursive, ReturnMatchingChildren)", (object) str1, (object) parentFilterCriteria, (object) configurationClause1, (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(this.m_backlogIterationPath), (object) AgileBaseQueryBuilder.GetWiqlTeamFilter(this.m_settings, "Source"), (object) configurationClause2, (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(this.m_iterationPath), (object) AgileBaseQueryBuilder.GetWiqlTeamFilter(this.m_settings, "Target"), (object) str2);
    }

    private string GetWorkItemTypeAndStatePerBacklogConfigurationClause(
      WorkItemStateCategory[] stateTypes,
      string fieldPrefix,
      IEnumerable<BacklogLevelConfiguration> configurationList)
    {
      int num = configurationList.Count<BacklogLevelConfiguration>();
      string str1 = num > 1 ? "(" : "";
      fieldPrefix = string.IsNullOrWhiteSpace(fieldPrefix) ? "" : fieldPrefix.Trim() + ".";
      for (int index = 0; index < num; ++index)
      {
        BacklogLevelConfiguration backlogLevel = configurationList.ElementAt<BacklogLevelConfiguration>(index);
        string str2 = this.EscapeWiqlFieldValues((IEnumerable<string>) backlogLevel.WorkItemTypes);
        string str3 = this.EscapeWiqlFieldValues((IEnumerable<string>) this.m_settings.BacklogConfiguration.GetWorkItemStates(backlogLevel, stateTypes));
        if (index > 0)
          str1 += " OR ";
        str1 = str1 + "(" + fieldPrefix + "[System.WorkItemType] IN ('" + str2 + "') AND " + fieldPrefix + "[System.State] IN ('" + str3 + "'))";
      }
      return str1 + (num > 1 ? ")" : "");
    }

    private string EscapeWiqlFieldValues(IEnumerable<string> values) => string.Join("','", (IEnumerable<string>) WorkItemTrackingUtils.EscapeWiqlFieldValues(values));
  }
}
