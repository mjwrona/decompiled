// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.IdFormatter
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Server;
using System;

namespace Microsoft.TeamFoundation
{
  internal class IdFormatter
  {
    private static string GetProjectName(string projectUri, ICommonStructureService css)
    {
      try
      {
        ProjectInfo project = css.GetProject(projectUri);
        return project == null ? string.Empty : project.Name;
      }
      catch (Exception ex)
      {
        return projectUri;
      }
    }

    internal static string FormatIdDisplay(Identity id, ICommonStructureService css)
    {
      if (string.IsNullOrEmpty(id.Domain))
        return "[SERVER]\\" + id.DisplayName;
      if (id.Type == IdentityType.WindowsGroup || id.Type == IdentityType.WindowsUser)
        return id.Domain + "\\" + id.AccountName;
      return id.Type == IdentityType.ApplicationGroup && css != null ? "[" + IdFormatter.GetProjectName(id.Domain, css) + "]\\" + id.DisplayName : id.DisplayName;
    }
  }
}
