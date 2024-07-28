// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsSeedFileResources
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
  public class TfsSeedFileResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal TfsSeedFileResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (TfsSeedFileResources.resourceMan == null)
          TfsSeedFileResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.TfsSeedFileResources", typeof (TfsSeedFileResources).Assembly);
        return TfsSeedFileResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => TfsSeedFileResources.resourceCulture;
      set => TfsSeedFileResources.resourceCulture = value;
    }

    public static string CircularDependencyErrorFormat => TfsSeedFileResources.ResourceManager.GetString(nameof (CircularDependencyErrorFormat), TfsSeedFileResources.resourceCulture);

    public static string CloseErrorWindow => TfsSeedFileResources.ResourceManager.GetString(nameof (CloseErrorWindow), TfsSeedFileResources.resourceCulture);

    public static string Dismiss => TfsSeedFileResources.ResourceManager.GetString(nameof (Dismiss), TfsSeedFileResources.resourceCulture);

    public static string ErrorPaneCloseLink => TfsSeedFileResources.ResourceManager.GetString(nameof (ErrorPaneCloseLink), TfsSeedFileResources.resourceCulture);

    public static string ErrorPaneHeader => TfsSeedFileResources.ResourceManager.GetString(nameof (ErrorPaneHeader), TfsSeedFileResources.resourceCulture);

    public static string ModuleScriptErrorFormat => TfsSeedFileResources.ResourceManager.GetString(nameof (ModuleScriptErrorFormat), TfsSeedFileResources.resourceCulture);

    public static string SessionInfoMessage => TfsSeedFileResources.ResourceManager.GetString(nameof (SessionInfoMessage), TfsSeedFileResources.resourceCulture);

    public static string UnknownErrorMessage => TfsSeedFileResources.ResourceManager.GetString(nameof (UnknownErrorMessage), TfsSeedFileResources.resourceCulture);

    public static string UnknownServerErrorMessage => TfsSeedFileResources.ResourceManager.GetString(nameof (UnknownServerErrorMessage), TfsSeedFileResources.resourceCulture);
  }
}
