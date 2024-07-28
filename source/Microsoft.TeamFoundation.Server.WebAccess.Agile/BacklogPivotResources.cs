// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogPivotResources
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
  public class BacklogPivotResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal BacklogPivotResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (BacklogPivotResources.resourceMan == null)
          BacklogPivotResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.SprintsHub.BacklogPivotResources", typeof (BacklogPivotResources).Assembly);
        return BacklogPivotResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => BacklogPivotResources.resourceCulture;
      set => BacklogPivotResources.resourceCulture = value;
    }

    public static string NoWorkScheduled => BacklogPivotResources.ResourceManager.GetString(nameof (NoWorkScheduled), BacklogPivotResources.resourceCulture);

    public static string ScheduleWork => BacklogPivotResources.ResourceManager.GetString(nameof (ScheduleWork), BacklogPivotResources.resourceCulture);

    public static string ServerNoResponse => BacklogPivotResources.ResourceManager.GetString(nameof (ServerNoResponse), BacklogPivotResources.resourceCulture);
  }
}
