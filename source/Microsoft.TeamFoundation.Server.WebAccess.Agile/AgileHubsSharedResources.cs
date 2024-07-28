// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.AgileHubsSharedResources
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
  public class AgileHubsSharedResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AgileHubsSharedResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AgileHubsSharedResources.resourceMan == null)
          AgileHubsSharedResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Agile.Resources.AgileHubsSharedResources", typeof (AgileHubsSharedResources).Assembly);
        return AgileHubsSharedResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AgileHubsSharedResources.resourceCulture;
      set => AgileHubsSharedResources.resourceCulture = value;
    }

    public static string NewBacklogLevelVisibilityNotSetNotificationLinkText_NewHub => AgileHubsSharedResources.ResourceManager.GetString(nameof (NewBacklogLevelVisibilityNotSetNotificationLinkText_NewHub), AgileHubsSharedResources.resourceCulture);

    public static string NewBacklogLevelVisibilityNotSetNotificationMessage_Left => AgileHubsSharedResources.ResourceManager.GetString(nameof (NewBacklogLevelVisibilityNotSetNotificationMessage_Left), AgileHubsSharedResources.resourceCulture);

    public static string NewBacklogLevelVisibilityNotSetNotificationMessage_Right => AgileHubsSharedResources.ResourceManager.GetString(nameof (NewBacklogLevelVisibilityNotSetNotificationMessage_Right), AgileHubsSharedResources.resourceCulture);
  }
}
