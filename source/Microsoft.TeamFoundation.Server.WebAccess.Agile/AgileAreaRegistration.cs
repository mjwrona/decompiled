// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class AgileAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Agile";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea("Agile/Scripts", "Agile/Scripts/Resources", "TFS").RegisterResource("AgileProductBacklog", (Func<ResourceManager>) (() => AgileProductBacklogResources.ResourceManager)).RegisterResource("AgileTaskboard", (Func<ResourceManager>) (() => AgileTaskboardResources.ResourceManager)).RegisterResource("AgileSprintPlanning", (Func<ResourceManager>) (() => AgileSprintPlanningResources.ResourceManager)).RegisterResource("AgileControls", (Func<ResourceManager>) (() => AgileControlsResources.ResourceManager)).RegisterResource("AgileEngagement", (Func<ResourceManager>) (() => AgileEngagementResources.ResourceManager)).RegisterResource("AgileNavigation", (Func<ResourceManager>) (() => AgileNavigationResources.ResourceManager)).RegisterResource("Agile", (Func<ResourceManager>) (() => AgileResources.ResourceManager)).RegisterResource("AgileAdmin", (Func<ResourceManager>) (() => AgileAdminResources.ResourceManager)).RegisterResource("BacklogsHub.BacklogView", (Func<ResourceManager>) (() => BacklogViewResources.ResourceManager)).RegisterResource("BoardsHub.BoardView", (Func<ResourceManager>) (() => BoardViewResources.ResourceManager)).RegisterResource("BoardsHub.BoardDirectory", (Func<ResourceManager>) (() => BoardDirectoryResources.ResourceManager)).RegisterResource("SprintsHub.BacklogPivot", (Func<ResourceManager>) (() => BacklogPivotResources.ResourceManager)).RegisterResource("SprintsHub.CapacityPivot", (Func<ResourceManager>) (() => CapacityPivotResources.ResourceManager)).RegisterResource("SprintsHub.Directory", (Func<ResourceManager>) (() => DirectoryResources.ResourceManager)).RegisterResource("SprintsHub.SprintEditor", (Func<ResourceManager>) (() => SprintEditorResources.ResourceManager)).RegisterResource("SprintsHub.WorkDetailsPanel", (Func<ResourceManager>) (() => WorkDetailsPanelResources.ResourceManager)).RegisterResource("SprintsHub", (Func<ResourceManager>) (() => SprintsHubResources.ResourceManager)).RegisterResource("AgileHubsShared", (Func<ResourceManager>) (() => AgileHubsSharedResources.ResourceManager)).RegisterResource("TeamPanel", (Func<ResourceManager>) (() => TeamPanelResources.ResourceManager)).RegisterResource("AgileExtensionsDuplicateWorkItem", (Func<ResourceManager>) (() => AgileExtensionsDuplicateWorkItem.ResourceManager));
      BuiltinPluginManager.RegisterPluginBase("TFS.Agile", "Agile/Scripts/");
    }
  }
}
