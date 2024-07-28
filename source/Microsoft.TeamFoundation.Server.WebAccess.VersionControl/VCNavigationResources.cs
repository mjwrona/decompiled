// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VCNavigationResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class VCNavigationResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal VCNavigationResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (VCNavigationResources.resourceMan == null)
          VCNavigationResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VCNavigationResources", typeof (VCNavigationResources).Assembly);
        return VCNavigationResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => VCNavigationResources.resourceCulture;
      set => VCNavigationResources.resourceCulture = value;
    }

    public static string CloneActionText => VCNavigationResources.ResourceManager.GetString(nameof (CloneActionText), VCNavigationResources.resourceCulture);

    public static string ContributedTabError => VCNavigationResources.ResourceManager.GetString(nameof (ContributedTabError), VCNavigationResources.resourceCulture);

    public static string ForkActionText => VCNavigationResources.ResourceManager.GetString(nameof (ForkActionText), VCNavigationResources.resourceCulture);

    public static string ManageRepositoriesActionText => VCNavigationResources.ResourceManager.GetString(nameof (ManageRepositoriesActionText), VCNavigationResources.resourceCulture);

    public static string NewPullRequestActionText => VCNavigationResources.ResourceManager.GetString(nameof (NewPullRequestActionText), VCNavigationResources.resourceCulture);

    public static string NewPullRequestNoPermission => VCNavigationResources.ResourceManager.GetString(nameof (NewPullRequestNoPermission), VCNavigationResources.resourceCulture);

    public static string NewRepositoryActionText => VCNavigationResources.ResourceManager.GetString(nameof (NewRepositoryActionText), VCNavigationResources.resourceCulture);

    public static string RepoSettingsNoPermission => VCNavigationResources.ResourceManager.GetString(nameof (RepoSettingsNoPermission), VCNavigationResources.resourceCulture);
  }
}
