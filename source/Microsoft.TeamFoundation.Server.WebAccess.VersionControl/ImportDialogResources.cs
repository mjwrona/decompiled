// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.ImportDialogResources
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
  internal class ImportDialogResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ImportDialogResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ImportDialogResources.resourceMan == null)
          ImportDialogResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Scenarios.Import.ImportDialog.ImportDialogResources", typeof (ImportDialogResources).Assembly);
        return ImportDialogResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ImportDialogResources.resourceCulture;
      set => ImportDialogResources.resourceCulture = value;
    }

    internal static string CloneUrlLabel => ImportDialogResources.ResourceManager.GetString(nameof (CloneUrlLabel), ImportDialogResources.resourceCulture);

    internal static string CloneUrlPlaceHolder => ImportDialogResources.ResourceManager.GetString(nameof (CloneUrlPlaceHolder), ImportDialogResources.resourceCulture);

    internal static string CloseLabel => ImportDialogResources.ResourceManager.GetString(nameof (CloseLabel), ImportDialogResources.resourceCulture);

    internal static string ImportAnExistingGitRepositoryLabel => ImportDialogResources.ResourceManager.GetString(nameof (ImportAnExistingGitRepositoryLabel), ImportDialogResources.resourceCulture);

    internal static string ImportAnExistingTfvcRepositoryLabel => ImportDialogResources.ResourceManager.GetString(nameof (ImportAnExistingTfvcRepositoryLabel), ImportDialogResources.resourceCulture);

    internal static string ImportHistoryDaysLabel => ImportDialogResources.ResourceManager.GetString(nameof (ImportHistoryDaysLabel), ImportDialogResources.resourceCulture);

    internal static string ImportOkButtonLabel => ImportDialogResources.ResourceManager.GetString(nameof (ImportOkButtonLabel), ImportDialogResources.resourceCulture);

    internal static string MigratehistoryLabel => ImportDialogResources.ResourceManager.GetString(nameof (MigratehistoryLabel), ImportDialogResources.resourceCulture);

    internal static string NamePlaceHolder => ImportDialogResources.ResourceManager.GetString(nameof (NamePlaceHolder), ImportDialogResources.resourceCulture);

    internal static string Password => ImportDialogResources.ResourceManager.GetString(nameof (Password), ImportDialogResources.resourceCulture);

    internal static string PasswordToolTip => ImportDialogResources.ResourceManager.GetString(nameof (PasswordToolTip), ImportDialogResources.resourceCulture);

    internal static string PathLabel => ImportDialogResources.ResourceManager.GetString(nameof (PathLabel), ImportDialogResources.resourceCulture);

    internal static string PathPlaceholderText => ImportDialogResources.ResourceManager.GetString(nameof (PathPlaceholderText), ImportDialogResources.resourceCulture);

    internal static string RepoLabel => ImportDialogResources.ResourceManager.GetString(nameof (RepoLabel), ImportDialogResources.resourceCulture);

    internal static string RequiresAuthLabel => ImportDialogResources.ResourceManager.GetString(nameof (RequiresAuthLabel), ImportDialogResources.resourceCulture);

    internal static string SourceLabel_Git => ImportDialogResources.ResourceManager.GetString(nameof (SourceLabel_Git), ImportDialogResources.resourceCulture);

    internal static string SourceLabel_Tfvc => ImportDialogResources.ResourceManager.GetString(nameof (SourceLabel_Tfvc), ImportDialogResources.resourceCulture);

    internal static string SourceTypeLabel => ImportDialogResources.ResourceManager.GetString(nameof (SourceTypeLabel), ImportDialogResources.resourceCulture);

    internal static string Tfvc_WarningMessage => ImportDialogResources.ResourceManager.GetString(nameof (Tfvc_WarningMessage), ImportDialogResources.resourceCulture);

    internal static string Username => ImportDialogResources.ResourceManager.GetString(nameof (Username), ImportDialogResources.resourceCulture);

    internal static string UsernameToolTip => ImportDialogResources.ResourceManager.GetString(nameof (UsernameToolTip), ImportDialogResources.resourceCulture);

    internal static string ValidationError_Auth_Private => ImportDialogResources.ResourceManager.GetString(nameof (ValidationError_Auth_Private), ImportDialogResources.resourceCulture);

    internal static string ValidationError_Auth_Public => ImportDialogResources.ResourceManager.GetString(nameof (ValidationError_Auth_Public), ImportDialogResources.resourceCulture);

    internal static string ValidationError_EmptySource => ImportDialogResources.ResourceManager.GetString(nameof (ValidationError_EmptySource), ImportDialogResources.resourceCulture);

    internal static string ValidationError_Header => ImportDialogResources.ResourceManager.GetString(nameof (ValidationError_Header), ImportDialogResources.resourceCulture);

    internal static string ValidationError_Tfvc_NoItem => ImportDialogResources.ResourceManager.GetString(nameof (ValidationError_Tfvc_NoItem), ImportDialogResources.resourceCulture);

    internal static string ValidationError_UnreachableUrl => ImportDialogResources.ResourceManager.GetString(nameof (ValidationError_UnreachableUrl), ImportDialogResources.resourceCulture);
  }
}
