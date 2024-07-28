// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.ImportStatusResources
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
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ImportStatusResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ImportStatusResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ImportStatusResources.resourceMan == null)
          ImportStatusResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Scenarios.Import.ImportStatus.ImportStatusResources", typeof (ImportStatusResources).Assembly);
        return ImportStatusResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ImportStatusResources.resourceCulture;
      set => ImportStatusResources.resourceCulture = value;
    }

    internal static string Cancel => ImportStatusResources.ResourceManager.GetString(nameof (Cancel), ImportStatusResources.resourceCulture);

    internal static string ClickHere => ImportStatusResources.ResourceManager.GetString(nameof (ClickHere), ImportStatusResources.resourceCulture);

    internal static string Failed_FailureMessage => ImportStatusResources.ResourceManager.GetString(nameof (Failed_FailureMessage), ImportStatusResources.resourceCulture);

    internal static string Failed_Footer => ImportStatusResources.ResourceManager.GetString(nameof (Failed_Footer), ImportStatusResources.resourceCulture);

    internal static string Failed_ImportFailed => ImportStatusResources.ResourceManager.GetString(nameof (Failed_ImportFailed), ImportStatusResources.resourceCulture);

    internal static string Progress_Footer => ImportStatusResources.ResourceManager.GetString(nameof (Progress_Footer), ImportStatusResources.resourceCulture);

    internal static string Progress_ImportMessage => ImportStatusResources.ResourceManager.GetString(nameof (Progress_ImportMessage), ImportStatusResources.resourceCulture);

    internal static string Progress_OnItsWay => ImportStatusResources.ResourceManager.GetString(nameof (Progress_OnItsWay), ImportStatusResources.resourceCulture);

    internal static string Retry => ImportStatusResources.ResourceManager.GetString(nameof (Retry), ImportStatusResources.resourceCulture);

    internal static string Success_Footer => ImportStatusResources.ResourceManager.GetString(nameof (Success_Footer), ImportStatusResources.resourceCulture);

    internal static string Success_ImportSuccessful => ImportStatusResources.ResourceManager.GetString(nameof (Success_ImportSuccessful), ImportStatusResources.resourceCulture);

    internal static string Success_SuccessMessage => ImportStatusResources.ResourceManager.GetString(nameof (Success_SuccessMessage), ImportStatusResources.resourceCulture);
  }
}
