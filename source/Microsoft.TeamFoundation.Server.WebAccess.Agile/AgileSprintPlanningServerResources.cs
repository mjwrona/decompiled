// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileSprintPlanningServerResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class AgileSprintPlanningServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileSprintPlanningServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (AgileSprintPlanningServerResources.resourceMan == null)
          AgileSprintPlanningServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.AgileSprintPlanningServerResources", typeof (AgileSprintPlanningServerResources).Assembly);
        return AgileSprintPlanningServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => AgileSprintPlanningServerResources.resourceCulture;
      set => AgileSprintPlanningServerResources.resourceCulture = value;
    }

    internal static string Capacity_AADErrorDuringAddMissingTeamMembers => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (Capacity_AADErrorDuringAddMissingTeamMembers), AgileSprintPlanningServerResources.resourceCulture);

    internal static string Capacity_AddDaysOff => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (Capacity_AddDaysOff), AgileSprintPlanningServerResources.resourceCulture);

    internal static string Capacity_CannotGetTeamCapacity => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (Capacity_CannotGetTeamCapacity), AgileSprintPlanningServerResources.resourceCulture);

    internal static string Capacity_CapacityLabel => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (Capacity_CapacityLabel), AgileSprintPlanningServerResources.resourceCulture);

    internal static string Capacity_Others => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (Capacity_Others), AgileSprintPlanningServerResources.resourceCulture);

    internal static string Capacity_Save_Failure => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (Capacity_Save_Failure), AgileSprintPlanningServerResources.resourceCulture);

    internal static string DaysOffDialog_TeamTitle => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (DaysOffDialog_TeamTitle), AgileSprintPlanningServerResources.resourceCulture);

    internal static string Sprint_Planning_Info_RefreshRequired => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (Sprint_Planning_Info_RefreshRequired), AgileSprintPlanningServerResources.resourceCulture);

    internal static string StatusMessage_FatalError => AgileSprintPlanningServerResources.ResourceManager.GetString(nameof (StatusMessage_FatalError), AgileSprintPlanningServerResources.resourceCulture);
  }
}
