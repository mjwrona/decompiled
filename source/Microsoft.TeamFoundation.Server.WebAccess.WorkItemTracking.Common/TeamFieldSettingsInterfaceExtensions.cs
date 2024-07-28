// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamFieldSettingsInterfaceExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class TeamFieldSettingsInterfaceExtensions
  {
    public static bool IsDefaultIndexValid(this ITeamFieldSettings settings)
    {
      ArgumentUtility.CheckForNull<ITeamFieldSettings>(settings, nameof (settings));
      if (settings.TeamFieldValues == null)
        return false;
      int defaultValueIndex = settings.DefaultValueIndex;
      int length = settings.TeamFieldValues.Length;
      if (length == 0 && defaultValueIndex == 0)
        return true;
      return defaultValueIndex >= 0 && defaultValueIndex < length;
    }
  }
}
