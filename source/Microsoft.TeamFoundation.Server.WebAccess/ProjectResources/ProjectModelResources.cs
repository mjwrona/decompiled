// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectResources.ProjectModelResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectResources
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ProjectModelResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ProjectModelResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ProjectModelResources.resourceMan == null)
          ProjectModelResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.ProjectResources.ProjectModelResources", typeof (ProjectModelResources).Assembly);
        return ProjectModelResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ProjectModelResources.resourceCulture;
      set => ProjectModelResources.resourceCulture = value;
    }

    internal static string ProfileImageAltText => ProjectModelResources.ResourceManager.GetString(nameof (ProfileImageAltText), ProjectModelResources.resourceCulture);

    internal static string ProjectStateCreating => ProjectModelResources.ResourceManager.GetString(nameof (ProjectStateCreating), ProjectModelResources.resourceCulture);

    internal static string ProjectStateDeleting => ProjectModelResources.ResourceManager.GetString(nameof (ProjectStateDeleting), ProjectModelResources.resourceCulture);

    internal static string ProjectStateOnline => ProjectModelResources.ResourceManager.GetString(nameof (ProjectStateOnline), ProjectModelResources.resourceCulture);
  }
}
