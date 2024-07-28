// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Resources
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Microsoft.TeamFoundation.Wiki.Server.Resources.resourceMan == null)
          Microsoft.TeamFoundation.Wiki.Server.Resources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Wiki.Server.Resources", typeof (Microsoft.TeamFoundation.Wiki.Server.Resources).Assembly);
        return Microsoft.TeamFoundation.Wiki.Server.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture;
      set => Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture = value;
    }

    internal static string ErrorMessage_PageNotFound => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessage_PageNotFound), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ErrorMessageAncestorPageNotFound => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessageAncestorPageNotFound), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ErrorMessageInsufficientRepositoryPermission => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessageInsufficientRepositoryPermission), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ErrorMessageNoVersionContributePermission => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessageNoVersionContributePermission), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ErrorMessagePageAlreadyExists => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessagePageAlreadyExists), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ErrorMessagePageContentUnavailable => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessagePageContentUnavailable), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ErrorMessagePageHasConflicts => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessagePageHasConflicts), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ErrorMessagePageRenameSourceNotFound => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessagePageRenameSourceNotFound), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ErrorMessagePreConditionHeaderInvalid => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ErrorMessagePreConditionHeaderInvalid), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string GetWikiByName => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (GetWikiByName), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string GetWikiByNameProjectScopeRequired => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (GetWikiByNameProjectScopeRequired), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string InvalidParametersOrNull => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (InvalidParametersOrNull), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string InvalidWikiVersionType => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (InvalidWikiVersionType), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ParameterCannotBeNullOrEmpty => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ParameterCannotBeNullOrEmpty), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ParameterNotExpectedForProjectWiki => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ParameterNotExpectedForProjectWiki), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ProjectWikiUpdateErrorMessage => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ProjectWikiUpdateErrorMessage), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string ScopeNotSupportedExceptionMessage => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (ScopeNotSupportedExceptionMessage), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiAlreadyExistsInProject => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiAlreadyExistsInProject), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiAlreadyExistsWithName => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiAlreadyExistsWithName), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiAttachmentExtensionTypeNotSupported => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiAttachmentExtensionTypeNotSupported), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiAttachmentNameHasReservedCharacters => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiAttachmentNameHasReservedCharacters), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiCommentArtifactFriendlyName => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiCommentArtifactFriendlyName), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiCommentInvalidProjectId => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiCommentInvalidProjectId), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiDeleteNotSupported => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiDeleteNotSupported), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiInitializationComment => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiInitializationComment), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiMappedPathInvalidOrDoesNotExist => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiMappedPathInvalidOrDoesNotExist), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiNameIsInvalid => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiNameIsInvalid), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiNotFound => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiNotFound), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiPageCommentInvalidArtifactId => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiPageCommentInvalidArtifactId), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiPageIdNotFound => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiPageIdNotFound), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiPageNameIsInvalid => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiPageNameIsInvalid), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiPageOrderNegative => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiPageOrderNegative), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiPagePathOrNameIsNullOrEmpty => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiPagePathOrNameIsNullOrEmpty), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiPagePathTooLong => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiPagePathTooLong), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiPermissionErrorMessage => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiPermissionErrorMessage), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiPublishOperationPathAlreadyPublished => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiPublishOperationPathAlreadyPublished), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiRenameFail => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiRenameFail), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiVersionInvalidOrDoesNotExist => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiVersionInvalidOrDoesNotExist), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);

    internal static string WikiVersionsMaximumThresholdReached => Microsoft.TeamFoundation.Wiki.Server.Resources.ResourceManager.GetString(nameof (WikiVersionsMaximumThresholdReached), Microsoft.TeamFoundation.Wiki.Server.Resources.resourceCulture);
  }
}
