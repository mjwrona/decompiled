// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileNavigationResources
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
  public class AgileNavigationResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileNavigationResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AgileNavigationResources.resourceMan == null)
          AgileNavigationResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.AgileNavigationResources", typeof (AgileNavigationResources).Assembly);
        return AgileNavigationResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AgileNavigationResources.resourceCulture;
      set => AgileNavigationResources.resourceCulture = value;
    }

    public static string BacklogsL2HubDisplayName => AgileNavigationResources.ResourceManager.GetString(nameof (BacklogsL2HubDisplayName), AgileNavigationResources.resourceCulture);

    public static string NoFutureIterationsMenuItemTitle => AgileNavigationResources.ResourceManager.GetString(nameof (NoFutureIterationsMenuItemTitle), AgileNavigationResources.resourceCulture);
  }
}
