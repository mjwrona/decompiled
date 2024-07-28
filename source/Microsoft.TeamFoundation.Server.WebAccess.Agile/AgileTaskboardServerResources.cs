// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileTaskboardServerResources
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
  internal class AgileTaskboardServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileTaskboardServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (AgileTaskboardServerResources.resourceMan == null)
          AgileTaskboardServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.AgileTaskboardServerResources", typeof (AgileTaskboardServerResources).Assembly);
        return AgileTaskboardServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => AgileTaskboardServerResources.resourceCulture;
      set => AgileTaskboardServerResources.resourceCulture = value;
    }

    internal static string LimitInput_Label => AgileTaskboardServerResources.ResourceManager.GetString(nameof (LimitInput_Label), AgileTaskboardServerResources.resourceCulture);

    internal static string LimitInput_Tooltip => AgileTaskboardServerResources.ResourceManager.GetString(nameof (LimitInput_Tooltip), AgileTaskboardServerResources.resourceCulture);

    internal static string Taskboard_NestedTasks_ReorderMessage => AgileTaskboardServerResources.ResourceManager.GetString(nameof (Taskboard_NestedTasks_ReorderMessage), AgileTaskboardServerResources.resourceCulture);

    internal static string TileContextMenu_DeleteWorkItem => AgileTaskboardServerResources.ResourceManager.GetString(nameof (TileContextMenu_DeleteWorkItem), AgileTaskboardServerResources.resourceCulture);
  }
}
