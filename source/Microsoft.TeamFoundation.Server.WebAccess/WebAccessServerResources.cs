// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebAccessServerResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

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
  internal class WebAccessServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal WebAccessServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (WebAccessServerResources.resourceMan == null)
          WebAccessServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.WebAccessServerResources", typeof (WebAccessServerResources).Assembly);
        return WebAccessServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => WebAccessServerResources.resourceCulture;
      set => WebAccessServerResources.resourceCulture = value;
    }

    internal static string AccountHubGroupsMyText => WebAccessServerResources.ResourceManager.GetString(nameof (AccountHubGroupsMyText), WebAccessServerResources.resourceCulture);

    internal static string BrowseOrganizationFromCollectionLabel => WebAccessServerResources.ResourceManager.GetString(nameof (BrowseOrganizationFromCollectionLabel), WebAccessServerResources.resourceCulture);

    internal static string HeaderSignInText => WebAccessServerResources.ResourceManager.GetString(nameof (HeaderSignInText), WebAccessServerResources.resourceCulture);

    internal static string HeaderSignInTitle => WebAccessServerResources.ResourceManager.GetString(nameof (HeaderSignInTitle), WebAccessServerResources.resourceCulture);

    internal static string Help => WebAccessServerResources.ResourceManager.GetString(nameof (Help), WebAccessServerResources.resourceCulture);

    internal static string IdentityImageFailureExceptionMessage => WebAccessServerResources.ResourceManager.GetString(nameof (IdentityImageFailureExceptionMessage), WebAccessServerResources.resourceCulture);

    internal static string ManageFeatures => WebAccessServerResources.ResourceManager.GetString(nameof (ManageFeatures), WebAccessServerResources.resourceCulture);

    internal static string ManageFeaturesOnPrem => WebAccessServerResources.ResourceManager.GetString(nameof (ManageFeaturesOnPrem), WebAccessServerResources.resourceCulture);

    internal static string ManageFeaturesTooltip => WebAccessServerResources.ResourceManager.GetString(nameof (ManageFeaturesTooltip), WebAccessServerResources.resourceCulture);

    internal static string ManageThemesText => WebAccessServerResources.ResourceManager.GetString(nameof (ManageThemesText), WebAccessServerResources.resourceCulture);

    internal static string ManageThemesTitle => WebAccessServerResources.ResourceManager.GetString(nameof (ManageThemesTitle), WebAccessServerResources.resourceCulture);

    internal static string SendAFrown => WebAccessServerResources.ResourceManager.GetString(nameof (SendAFrown), WebAccessServerResources.resourceCulture);

    internal static string SendASmile => WebAccessServerResources.ResourceManager.GetString(nameof (SendASmile), WebAccessServerResources.resourceCulture);

    internal static string NotFoundException_Message => WebAccessServerResources.ResourceManager.GetString(nameof (NotFoundException_Message), WebAccessServerResources.resourceCulture);
  }
}
