// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WorkDetailsPanelResources
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
  public class WorkDetailsPanelResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal WorkDetailsPanelResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (WorkDetailsPanelResources.resourceMan == null)
          WorkDetailsPanelResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.SprintsHub.WorkDetailsPanelResources", typeof (WorkDetailsPanelResources).Assembly);
        return WorkDetailsPanelResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => WorkDetailsPanelResources.resourceCulture;
      set => WorkDetailsPanelResources.resourceCulture = value;
    }

    public static string Capacity_Unassigned => WorkDetailsPanelResources.ResourceManager.GetString(nameof (Capacity_Unassigned), WorkDetailsPanelResources.resourceCulture);

    public static string ExpandCollapseSection_AriaLabel => WorkDetailsPanelResources.ResourceManager.GetString(nameof (ExpandCollapseSection_AriaLabel), WorkDetailsPanelResources.resourceCulture);

    public static string SectionTitle_Prefix => WorkDetailsPanelResources.ResourceManager.GetString(nameof (SectionTitle_Prefix), WorkDetailsPanelResources.resourceCulture);

    public static string SectionTitle_TeamCapacity => WorkDetailsPanelResources.ResourceManager.GetString(nameof (SectionTitle_TeamCapacity), WorkDetailsPanelResources.resourceCulture);

    public static string TeamCapacitySection_TeamProgressTitle => WorkDetailsPanelResources.ResourceManager.GetString(nameof (TeamCapacitySection_TeamProgressTitle), WorkDetailsPanelResources.resourceCulture);

    public static string WorkDetailsPanel_Title => WorkDetailsPanelResources.ResourceManager.GetString(nameof (WorkDetailsPanel_Title), WorkDetailsPanelResources.resourceCulture);
  }
}
