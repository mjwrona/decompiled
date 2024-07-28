// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Welcome.WelcomeResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Welcome, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B024A61-082C-4505-8523-CF030F6A8A5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Welcome.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Welcome
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class WelcomeResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal WelcomeResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (WelcomeResources.resourceMan == null)
          WelcomeResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Welcome.WelcomeResources", typeof (WelcomeResources).Assembly);
        return WelcomeResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => WelcomeResources.resourceCulture;
      set => WelcomeResources.resourceCulture = value;
    }

    internal static string CollapsedText => WelcomeResources.ResourceManager.GetString(nameof (CollapsedText), WelcomeResources.resourceCulture);

    internal static string ErrorText => WelcomeResources.ResourceManager.GetString(nameof (ErrorText), WelcomeResources.resourceCulture);

    internal static string HubTitle => WelcomeResources.ResourceManager.GetString(nameof (HubTitle), WelcomeResources.resourceCulture);

    internal static string ProjectInfoError => WelcomeResources.ResourceManager.GetString(nameof (ProjectInfoError), WelcomeResources.resourceCulture);

    internal static string ProjectTypeError => WelcomeResources.ResourceManager.GetString(nameof (ProjectTypeError), WelcomeResources.resourceCulture);
  }
}
