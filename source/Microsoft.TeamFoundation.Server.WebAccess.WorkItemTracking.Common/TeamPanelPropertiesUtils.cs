// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamPanelPropertiesUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class TeamPanelPropertiesUtils
  {
    public static TeamPanelProperties GetTeamPanelProperties(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      TeamPanelProperties teamPanelProperties = new TeamPanelProperties();
      if (providerContext?.Properties == null)
        throw new ArgumentNullException(nameof (providerContext), "providerContext is null or has null properties");
      string stringPropertyValue1 = GetStringPropertyValue("teamId");
      string stringPropertyValue2 = GetStringPropertyValue("projectId");
      teamPanelProperties.TeamName = GetStringPropertyValue("teamName");
      teamPanelProperties.ProjectName = GetStringPropertyValue("projectName");
      Guid result1;
      if (!Guid.TryParse(stringPropertyValue1, out result1))
        throw new FormatException("providerContext.Properties[teamId] is in an invalid format");
      teamPanelProperties.TeamId = result1;
      Guid result2;
      if (!Guid.TryParse(stringPropertyValue2, out result2))
        throw new FormatException("providerContext.Properties[projectId] is in an invalid format");
      teamPanelProperties.ProjectId = result2;
      return teamPanelProperties;

      string GetStringPropertyValue(string key) => (providerContext.Properties.ContainsKey(key) ? providerContext.Properties[key] as string : (string) null) ?? throw new ArgumentNullException(key, "providerContext.Properties[" + key + "] is missing or null");
    }
  }
}
