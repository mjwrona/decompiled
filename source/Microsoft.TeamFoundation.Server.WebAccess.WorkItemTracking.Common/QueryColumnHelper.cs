// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.QueryColumnHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class QueryColumnHelper
  {
    private static readonly string[] RequiredColumns = new string[7]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.TeamProject,
      CoreFieldReferenceNames.Rev,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AssignedTo
    };

    public static string[] GetRequiredColumns(IEnumerable<string> additionalColumns) => additionalColumns == null ? QueryColumnHelper.RequiredColumns : ((IEnumerable<string>) QueryColumnHelper.RequiredColumns).Concat<string>(additionalColumns).Distinct<string>().ToArray<string>();

    private static string GetColumnOptionsPersistenceKey(Guid projectGuid, Guid persistenceId) => string.Format("/Projects/{0}/Queries/{1}/ColumnOptions", (object) projectGuid, (object) persistenceId);

    public static void PersistColumnSizes(
      WebUserSettingsHive hive,
      Guid queryId,
      Guid projectGuid,
      string columnSizes)
    {
      hive.WriteValue(QueryColumnHelper.GetColumnOptionsPersistenceKey(projectGuid, queryId), columnSizes);
    }

    public static string GetPersistedColumnSizes(
      WebUserSettingsHive hive,
      Guid queryId,
      Guid projectGuid)
    {
      return hive.ReadSetting<string>(QueryColumnHelper.GetColumnOptionsPersistenceKey(projectGuid, queryId), (string) null);
    }
  }
}
