// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions.AgileSettingsExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions
{
  public static class AgileSettingsExtensions
  {
    public static IEnumerable<string> GetDefaultBacklogFields(
      this IAgileSettings agileSettings,
      bool includeActivity)
    {
      List<string> defaultBacklogFields = new List<string>()
      {
        "System.Id",
        "System.TeamProject",
        "System.WorkItemType",
        "System.AssignedTo",
        "System.Title",
        "System.State",
        "System.Tags",
        agileSettings.Process.OrderByField.Name,
        agileSettings.Process.RemainingWorkField.Name,
        "System.AreaId",
        "System.IterationPath",
        "System.IterationId",
        agileSettings.Process.TeamField.Name
      };
      if (includeActivity && agileSettings.Process.ActivityField != null)
        defaultBacklogFields.Add(agileSettings.Process.ActivityField.Name);
      return (IEnumerable<string>) defaultBacklogFields;
    }
  }
}
