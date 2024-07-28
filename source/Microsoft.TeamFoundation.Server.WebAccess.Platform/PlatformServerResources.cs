// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PlatformServerResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class PlatformServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal PlatformServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (PlatformServerResources.resourceMan == null)
          PlatformServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.PlatformServerResources", typeof (PlatformServerResources).Assembly);
        return PlatformServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => PlatformServerResources.resourceCulture;
      set => PlatformServerResources.resourceCulture = value;
    }

    public static string SecondLevelNavigationLabel => PlatformServerResources.ResourceManager.GetString(nameof (SecondLevelNavigationLabel), PlatformServerResources.resourceCulture);

    public static string TopLevelNavigationLabel => PlatformServerResources.ResourceManager.GetString(nameof (TopLevelNavigationLabel), PlatformServerResources.resourceCulture);
  }
}
