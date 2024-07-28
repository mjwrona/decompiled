// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.ValidationUtils
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class ValidationUtils
  {
    public static GitVersionDescriptor ValidateAndGetWikiVersionForEditScenarios(
      IVssRequestContext TfsRequestContext,
      WikiV2 wiki,
      GitVersionDescriptor versionDescriptor,
      string errorMessageInvalidWikiVersion,
      string wikiPageOperationNotSupported,
      string wikiVersionInvalidOrDoesNotExist)
    {
      if (wiki.Type == WikiType.ProjectWiki)
      {
        GitVersionDescriptorComparer descriptorComparer = new GitVersionDescriptorComparer();
        GitVersionDescriptor version2 = wiki.Versions.ToList<GitVersionDescriptor>()[0];
        if (string.IsNullOrEmpty(versionDescriptor?.Version) || descriptorComparer.Equals(versionDescriptor, version2))
          return version2;
        throw new InvalidArgumentValueException(nameof (versionDescriptor), wikiVersionInvalidOrDoesNotExist);
      }
      if (!TfsRequestContext.IsFeatureEnabled("WebAccess.NewWiki.RichCodeWikiEditing"))
        throw new WikiOperationNotSupportedException(string.Format(wikiPageOperationNotSupported, (object) WikiType.CodeWiki));
      if (versionDescriptor?.Version == null || !versionDescriptor.VersionType.Equals((object) GitVersionType.Branch))
        throw new InvalidArgumentValueException(nameof (versionDescriptor), errorMessageInvalidWikiVersion);
      GitVersionDescriptorComparer versionComparer = new GitVersionDescriptorComparer();
      if (!wiki.Versions.Any<GitVersionDescriptor>((Func<GitVersionDescriptor, bool>) (x => versionComparer.Equals(x, versionDescriptor))))
        throw new InvalidArgumentValueException("Version", string.Format(wikiVersionInvalidOrDoesNotExist, (object) versionDescriptor));
      return versionDescriptor;
    }

    public static WikiV2 ValidateWiki(
      IVssRequestContext requestContext,
      Guid projectId,
      string wikiIdentifier)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(wikiIdentifier, nameof (wikiIdentifier));
      return WikiV2Helper.GetWikiByIdentifier(requestContext, projectId, wikiIdentifier) ?? throw new WikiNotFoundException(Resources.WikiNotFound);
    }
  }
}
