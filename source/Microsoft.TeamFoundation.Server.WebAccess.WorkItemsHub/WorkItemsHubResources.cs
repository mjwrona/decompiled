// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub.WorkItemsHubResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF10A6C0-53C4-4480-9084-156ADE56D4B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class WorkItemsHubResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal WorkItemsHubResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (WorkItemsHubResources.resourceMan == null)
          WorkItemsHubResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub.WorkItemsHubResources", typeof (WorkItemsHubResources).Assembly);
        return WorkItemsHubResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => WorkItemsHubResources.resourceCulture;
      set => WorkItemsHubResources.resourceCulture = value;
    }

    public static string BackToWorkItems => WorkItemsHubResources.ResourceManager.GetString(nameof (BackToWorkItems), WorkItemsHubResources.resourceCulture);

    public static string KeyboardShortcutDescription_BackToWorkItemsHub => WorkItemsHubResources.ResourceManager.GetString(nameof (KeyboardShortcutDescription_BackToWorkItemsHub), WorkItemsHubResources.resourceCulture);

    public static string KeyboardShortcutGroup_WorkItems => WorkItemsHubResources.ResourceManager.GetString(nameof (KeyboardShortcutGroup_WorkItems), WorkItemsHubResources.resourceCulture);

    public static string NavBarBackToWorkItemsShortcut => WorkItemsHubResources.ResourceManager.GetString(nameof (NavBarBackToWorkItemsShortcut), WorkItemsHubResources.resourceCulture);

    public static string TriageBackToTabTooltip => WorkItemsHubResources.ResourceManager.GetString(nameof (TriageBackToTabTooltip), WorkItemsHubResources.resourceCulture);

    public static string TriageNextWorkItemTooltip => WorkItemsHubResources.ResourceManager.GetString(nameof (TriageNextWorkItemTooltip), WorkItemsHubResources.resourceCulture);

    public static string TriagePreviousWorkItemTooltip => WorkItemsHubResources.ResourceManager.GetString(nameof (TriagePreviousWorkItemTooltip), WorkItemsHubResources.resourceCulture);

    public static string WorkItemsHubTitle => WorkItemsHubResources.ResourceManager.GetString(nameof (WorkItemsHubTitle), WorkItemsHubResources.resourceCulture);
  }
}
