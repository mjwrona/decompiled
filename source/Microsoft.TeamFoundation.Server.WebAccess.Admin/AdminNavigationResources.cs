// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminNavigationResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class AdminNavigationResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AdminNavigationResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AdminNavigationResources.resourceMan == null)
          AdminNavigationResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Admin.Resources.AdminNavigationResources", typeof (AdminNavigationResources).Assembly);
        return AdminNavigationResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AdminNavigationResources.resourceCulture;
      set => AdminNavigationResources.resourceCulture = value;
    }

    public static string AccountSettings => AdminNavigationResources.ResourceManager.GetString(nameof (AccountSettings), AdminNavigationResources.resourceCulture);

    public static string CollectionSettings => AdminNavigationResources.ResourceManager.GetString(nameof (CollectionSettings), AdminNavigationResources.resourceCulture);

    public static string OrganizationSettings => AdminNavigationResources.ResourceManager.GetString(nameof (OrganizationSettings), AdminNavigationResources.resourceCulture);

    public static string ProjectSettings => AdminNavigationResources.ResourceManager.GetString(nameof (ProjectSettings), AdminNavigationResources.resourceCulture);

    public static string ServerSettings => AdminNavigationResources.ResourceManager.GetString(nameof (ServerSettings), AdminNavigationResources.resourceCulture);

    public static string TeamSettings => AdminNavigationResources.ResourceManager.GetString(nameof (TeamSettings), AdminNavigationResources.resourceCulture);

    public static string TurnOffFeatureDialogTitleFormat => AdminNavigationResources.ResourceManager.GetString(nameof (TurnOffFeatureDialogTitleFormat), AdminNavigationResources.resourceCulture);

    public static string TurnOffFeatureReasonLabel => AdminNavigationResources.ResourceManager.GetString(nameof (TurnOffFeatureReasonLabel), AdminNavigationResources.resourceCulture);
  }
}
