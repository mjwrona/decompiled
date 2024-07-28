// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.AgileBaseQueryBuilder
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Agile.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class AgileBaseQueryBuilder
  {
    protected IAgileSettings m_settings;

    public static string GetWorkItemTypeFilter(BacklogLevelConfiguration backlogLevel) => AgileBaseQueryBuilder.GetWorkItemTypeFilter((IEnumerable<BacklogLevelConfiguration>) new BacklogLevelConfiguration[1]
    {
      backlogLevel
    });

    public static string GetWorkItemTypeFilter(
      IEnumerable<BacklogLevelConfiguration> backlogLevels)
    {
      return string.Join(",", backlogLevels.SelectMany<BacklogLevelConfiguration, string>((Func<BacklogLevelConfiguration, IEnumerable<string>>) (x => (IEnumerable<string>) x.WorkItemTypes)).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName).Select<string, string>((Func<string, string>) (x => WiqlHelpers.GetEscapedSingleQuotedValue(x))));
    }

    protected AgileBaseQueryBuilder()
    {
    }

    public AgileBaseQueryBuilder(IAgileSettings settings)
    {
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      this.m_settings = settings;
    }

    public int ParentId { get; set; }

    public abstract string GetQuery();

    protected string ParentFilterCriteria => this.ParentId != 0 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[Source].[{0}] = '{1}' AND", (object) "System.Id", (object) this.ParentId) : string.Empty;

    public static string GetWiqlTeamFilter(IAgileSettings settings, string fieldPrefix = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = settings.Process.IsTeamFieldAreaPath();
      fieldPrefix = string.IsNullOrWhiteSpace(fieldPrefix) ? "" : fieldPrefix + ".";
      for (int index = 0; index < settings.TeamSettings.TeamFieldConfig.TeamFieldValues.Length; ++index)
      {
        if (index > 0)
          stringBuilder.Append(" OR ");
        string str = !flag || !settings.TeamSettings.TeamFieldConfig.TeamFieldValues[index].IncludeChildren ? "=" : "UNDER";
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}] {2} '{3}'", (object) fieldPrefix, (object) settings.Process.TeamField.Name, (object) str, (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(settings.TeamSettings.TeamFieldConfig.TeamFieldValues[index].Value));
      }
      return settings.TeamSettings.TeamFieldConfig.TeamFieldValues.Length > 1 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0})", (object) stringBuilder.ToString()) : stringBuilder.ToString();
    }
  }
}
