// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Backlog.OwnershipEvaluator
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;

namespace Microsoft.TeamFoundation.Agile.Server.Backlog
{
  internal static class OwnershipEvaluator
  {
    public static bool EvaluateOwnership(
      string teamFieldValue,
      ITeamFieldSettings teamFieldsettings)
    {
      foreach (ITeamFieldValue teamFieldValue1 in teamFieldsettings.TeamFieldValues)
      {
        if (TFStringComparer.CssTreePathName.Equals(teamFieldValue, teamFieldValue1.Value) || teamFieldValue1.IncludeChildren && TFStringComparer.CssTreePathName.StartsWith(teamFieldValue, teamFieldValue1.Value + "\\"))
          return true;
      }
      return false;
    }
  }
}
