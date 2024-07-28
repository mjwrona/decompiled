// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.TswaHyperlinkBuilder
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Common
{
  public abstract class TswaHyperlinkBuilder
  {
    private const string c_PublicAccessMapping = "PublicAccessMapping";
    private const string c_VersionControlPathRoot = "$/";
    private const char c_VersionControlPathSeparator = '/';

    protected Guid CollectionId { get; set; }

    protected abstract Uri GetUrl(string serviceType, string accessMappingMoniker);

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual Uri GetUrl(
      string serviceType,
      string accessMappingMoniker,
      bool collectionLevel = false)
    {
      return this.GetUrl(serviceType, accessMappingMoniker);
    }

    private Uri GetPageUrl(string accessMappingMoniker, string relativePath, string queryString) => new UriBuilder(UriUtility.Combine(this.GetHomeUrl(accessMappingMoniker), relativePath, true))
    {
      Query = queryString
    }.Uri;

    public Uri GetHomeUrl() => this.GetHomeUrl("PublicAccessMapping");

    public Uri GetHomeUrl(string accessMappingMoniker) => this.GetUrl("TSWAHome", accessMappingMoniker);

    public Uri GetHomeUrl(Uri projectUri) => this.GetHomeUrl(projectUri, "PublicAccessMapping");

    public Uri GetTeamHomeUrl(Uri projectUri, string teamName)
    {
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri));
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (teamName));
      return this.GetPageUrl("PublicAccessMapping", "index.aspx", this.FormatQueryString(projectUri, "team", teamName));
    }

    public Uri GetHomeUrl(Uri projectUri, string accessMappingMoniker)
    {
      ArgumentUtility.CheckForNull<Uri>(projectUri, nameof (projectUri));
      return this.GetPageUrl(accessMappingMoniker, "index.aspx", this.FormatQueryString(projectUri));
    }

    public Uri GetSourceExplorerUrl(string serverItemPath) => this.GetSourceExplorerUrl(serverItemPath, "PublicAccessMapping");

    public Uri GetSourceExplorerUrl(string serverItemPath, string accessMappingMoniker)
    {
      if (string.IsNullOrEmpty(serverItemPath))
        serverItemPath = "$/";
      return this.FormatUrl("ExploreSourceControlPath", accessMappingMoniker, "sourceControlPath", serverItemPath);
    }

    public Uri GetChangesetDetailsUrl(int changesetId) => this.GetChangesetDetailsUrl(changesetId, "PublicAccessMapping");

    public Uri GetChangesetDetailsUrl(int changesetId, string accessMappingMoniker)
    {
      ArgumentUtility.CheckForOutOfRange(changesetId, nameof (changesetId), 1);
      return this.FormatUrl("ViewChangesetDetails", accessMappingMoniker, nameof (changesetId), changesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public Uri GetShelvesetDetailsUrl(string shelvesetName, string shelvesetOwner) => this.GetShelvesetDetailsUrl(shelvesetName, shelvesetOwner, "PublicAccessMapping");

    public Uri GetShelvesetDetailsUrl(
      string shelvesetName,
      string shelvesetOwner,
      string accessMappingMoniker)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(shelvesetName, nameof (shelvesetName));
      ArgumentUtility.CheckStringForNullOrEmpty(shelvesetOwner, nameof (shelvesetOwner));
      return this.FormatUrl("ViewShelvesetDetails", accessMappingMoniker, nameof (shelvesetName), shelvesetName, nameof (shelvesetOwner), shelvesetOwner);
    }

    public Uri GetViewSourceControlItemUrl(int itemId, string versionSpec) => this.GetViewSourceControlItemUrl(itemId, versionSpec, "PublicAccessMapping");

    public Uri GetViewSourceControlItemUrl(
      int itemId,
      string versionSpec,
      string accessMappingMoniker)
    {
      ArgumentUtility.CheckForOutOfRange(itemId, nameof (itemId), 0);
      string queryString;
      if (string.IsNullOrEmpty(versionSpec))
        queryString = this.FormatQueryString("item", itemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      else
        queryString = this.FormatQueryString("item", itemId.ToString((IFormatProvider) CultureInfo.InvariantCulture), "cs", versionSpec);
      return this.GetPageUrl(accessMappingMoniker, "view.aspx", queryString);
    }

    public Uri GetViewSourceControlItemUrl(string serverItemPath, int changesetId) => this.GetViewSourceControlItemUrl(serverItemPath, changesetId, "PublicAccessMapping");

    public Uri GetViewSourceControlItemUrl(
      string serverItemPath,
      int changesetId,
      string accessMappingMoniker)
    {
      TswaHyperlinkBuilder.ValidateServerItemPath(serverItemPath);
      return this.GetViewSourceControlItemUrl(serverItemPath, changesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture), accessMappingMoniker);
    }

    public Uri GetViewSourceControlItemUrl(string serverItemPath, string versionSpec) => this.GetViewSourceControlItemUrl(serverItemPath, versionSpec, "PublicAccessMapping");

    public Uri GetViewSourceControlItemUrl(
      string serverItemPath,
      string versionSpec,
      string accessMappingMoniker)
    {
      TswaHyperlinkBuilder.ValidateServerItemPath(serverItemPath);
      return this.FormatUrl("ViewSourceControlItem", accessMappingMoniker, "itemPath", serverItemPath, "itemChangeset", versionSpec);
    }

    public Uri GetViewSourceControlShelvedItemUrl(
      string serverItemPath,
      string shelvesetName,
      string shelvesetOwner)
    {
      return this.GetViewSourceControlShelvedItemUrl(serverItemPath, shelvesetName, shelvesetOwner, "PublicAccessMapping");
    }

    public Uri GetViewSourceControlShelvedItemUrl(
      string serverItemPath,
      string shelvesetName,
      string shelvesetOwner,
      string accessMappingMoniker)
    {
      ArgumentUtility.CheckForNull<string>(shelvesetName, nameof (shelvesetName));
      ArgumentUtility.CheckForNull<string>(shelvesetOwner, nameof (shelvesetOwner));
      TswaHyperlinkBuilder.ValidateServerItemPath(serverItemPath);
      return this.GetPageUrl(accessMappingMoniker, "view.aspx", this.FormatQueryString("path", serverItemPath, "ss", shelvesetName + ";" + shelvesetOwner));
    }

    public Uri GetHistorySourceControlItemUrl(string serverItemPath, string versionSpec) => this.GetHistorySourceControlItemUrl(serverItemPath, versionSpec, "PublicAccessMapping");

    public Uri GetHistorySourceControlItemUrl(
      string serverItemPath,
      string versionSpec,
      string accessMappingMoniker)
    {
      TswaHyperlinkBuilder.ValidateServerItemPath(serverItemPath);
      string queryString;
      if (string.IsNullOrEmpty(versionSpec))
        queryString = this.FormatQueryString("path", serverItemPath);
      else
        queryString = this.FormatQueryString("path", serverItemPath, "cs", versionSpec);
      return this.GetPageUrl(accessMappingMoniker, "history.aspx", queryString);
    }

    public Uri GetAnnotateSourceControlItemUrl(string serverItemPath, int changesetId) => this.GetAnnotateSourceControlItemUrl(serverItemPath, changesetId, "PublicAccessMapping");

    public Uri GetAnnotateSourceControlItemUrl(
      string serverItemPath,
      int changesetId,
      string accessMappingMoniker)
    {
      TswaHyperlinkBuilder.ValidateServerItemPath(serverItemPath);
      return this.GetAnnotateSourceControlItemUrl(serverItemPath, changesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture), accessMappingMoniker);
    }

    public Uri GetAnnotateSourceControlItemUrl(string serverItemPath, string versionSpec) => this.GetAnnotateSourceControlItemUrl(serverItemPath, versionSpec, "PublicAccessMapping");

    public Uri GetAnnotateSourceControlItemUrl(
      string serverItemPath,
      string versionSpec,
      string accessMappingMoniker)
    {
      TswaHyperlinkBuilder.ValidateServerItemPath(serverItemPath);
      return this.FormatUrl("AnnotateSourceControlItem", accessMappingMoniker, "itemPath", serverItemPath, "itemChangeset", versionSpec);
    }

    public Uri GetDifferenceSourceControlItemsUrl(
      string originalItemServerPath,
      int originalItemChangeset,
      string modifiedItemServerPath,
      int modifiedItemChangeset)
    {
      return this.GetDifferenceSourceControlItemsUrl(originalItemServerPath, originalItemChangeset, modifiedItemServerPath, modifiedItemChangeset, "PublicAccessMapping");
    }

    public Uri GetDifferenceSourceControlItemsUrl(
      string originalItemServerPath,
      int originalItemChangeset,
      string modifiedItemServerPath,
      int modifiedItemChangeset,
      string accessMappingMoniker)
    {
      return this.GetDifferenceSourceControlItemsUrl(originalItemServerPath, originalItemChangeset.ToString((IFormatProvider) CultureInfo.InvariantCulture), modifiedItemServerPath, modifiedItemChangeset.ToString((IFormatProvider) CultureInfo.InvariantCulture), accessMappingMoniker);
    }

    public Uri GetDifferenceSourceControlItemsUrl(
      string originalItemServerPath,
      string originalItemVersionSpec,
      string modifiedItemServerPath,
      string modifiedItemVersionSpec)
    {
      return this.GetDifferenceSourceControlItemsUrl(originalItemServerPath, originalItemVersionSpec, modifiedItemServerPath, modifiedItemVersionSpec, "PublicAccessMapping");
    }

    public Uri GetDifferenceSourceControlItemsUrl(
      string originalItemServerPath,
      string originalItemVersionSpec,
      string modifiedItemServerPath,
      string modifiedItemVersionSpec,
      string accessMappingMoniker)
    {
      TswaHyperlinkBuilder.ValidateServerItemPath(originalItemServerPath, nameof (originalItemServerPath));
      TswaHyperlinkBuilder.ValidateServerItemPath(modifiedItemServerPath, nameof (modifiedItemServerPath));
      return this.FormatUrl("DiffSourceControlItems", accessMappingMoniker, "originalItemPath", originalItemServerPath, "originalItemChangeset", originalItemVersionSpec, "modifiedItemPath", modifiedItemServerPath, "modifiedItemChangeset", modifiedItemVersionSpec);
    }

    public Uri GetDifferenceSourceControlShelvedItemUrl(
      string originalItemServerPath,
      int originalItemChangeset,
      string shelvedItemServerPath,
      string shelvesetName,
      string shelvesetOwner)
    {
      return this.GetDifferenceSourceControlShelvedItemUrl(originalItemServerPath, originalItemChangeset.ToString((IFormatProvider) CultureInfo.InvariantCulture), shelvedItemServerPath, shelvesetName, shelvesetOwner, "PublicAccessMapping");
    }

    public Uri GetDifferenceSourceControlShelvedItemUrl(
      string originalItemServerPath,
      string originalItemVersionSpec,
      string shelvedItemServerPath,
      string shelvesetName,
      string shelvesetOwner,
      string accessMappingMoniker)
    {
      TswaHyperlinkBuilder.ValidateServerItemPath(originalItemServerPath, nameof (originalItemServerPath));
      TswaHyperlinkBuilder.ValidateServerItemPath(shelvedItemServerPath, nameof (shelvedItemServerPath));
      return this.FormatUrl("DiffSourceControlShelvedItem", accessMappingMoniker, "originalItemPath", originalItemServerPath, "originalItemChangeset", originalItemVersionSpec, "shelvedItemPath", shelvedItemServerPath, nameof (shelvesetName), shelvesetName, nameof (shelvesetOwner), shelvesetOwner);
    }

    public Uri GetWorkItemEditorUrl(int workItemId) => this.GetWorkItemEditorUrl(workItemId, "PublicAccessMapping");

    public Uri GetWorkItemEditorUrl(Uri projectUri, int workItemId)
    {
      if (!(projectUri != (Uri) null))
        return this.GetWorkItemEditorUrl(workItemId);
      return this.FormatUrl("OpenWorkItemWithProjectContext", "PublicAccessMapping", true, false, "projectId", TswaHyperlinkBuilder.ArtifactToolSpecificIdToGuid(projectUri).ToString(), "id", workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public Uri GetWorkItemEditorUrl(int workItemId, string accessMappingMoniker)
    {
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      return this.FormatUrl("OpenWorkItem", accessMappingMoniker, nameof (workItemId), workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public Uri GetWorkItemQueryResultsUrl(Uri projectUri, string queryPath) => this.GetWorkItemQueryResultsUrl(projectUri, queryPath, "PublicAccessMapping");

    public Uri GetWorkItemQueryResultsUrl(
      Uri projectUri,
      string queryPath,
      string accessMappingMoniker)
    {
      ArgumentUtility.CheckForNull<string>(queryPath, nameof (queryPath));
      if (projectUri == (Uri) null)
      {
        string queryString = this.FormatQueryString(projectUri, "path", queryPath);
        return this.GetPageUrl(accessMappingMoniker, "qr.aspx", queryString);
      }
      return this.FormatUrl("ViewServerQueryResults", accessMappingMoniker, nameof (projectUri), projectUri.ToString(), "storedQueryPath", queryPath);
    }

    public Uri GetWorkItemQueryResultsUrl(Uri projectUri, Guid queryId) => this.GetWorkItemQueryResultsUrl(projectUri, queryId, "PublicAccessMapping");

    public Uri GetWorkItemQueryResultsUrl(
      Uri projectUri,
      Guid queryId,
      string accessMappingMoniker)
    {
      ArgumentUtility.CheckForEmptyGuid(queryId, nameof (queryId));
      return this.GetPageUrl(accessMappingMoniker, "qr.aspx", this.FormatQueryString(projectUri, "qid", queryId.ToString()));
    }

    public Uri GetViewBuildDetailsUrl(Uri buildUri) => this.GetViewBuildDetailsUrl(buildUri, Guid.Empty, "PublicAccessMapping");

    public Uri GetViewBuildDetailsUrl(Uri buildUri, Guid projectId) => this.GetViewBuildDetailsUrl(buildUri, projectId, "PublicAccessMapping");

    public Uri GetViewBuildDetailsUrl(Uri buildUri, Guid projectId, string accessMappingMoniker)
    {
      ArgumentUtility.CheckForNull<Uri>(buildUri, nameof (buildUri));
      return projectId != Guid.Empty ? this.FormatUrl("ViewBuildDetails", accessMappingMoniker, nameof (buildUri), buildUri.ToString(), nameof (projectId), projectId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture)) : this.FormatUrl("ViewBuildDetails", accessMappingMoniker, nameof (buildUri), buildUri.ToString());
    }

    public Uri GetAlertsAdminPageUrl(string teamProject) => this.FormatUrl("ProjectAlertsWeb", "PublicAccessMapping", true, true, "projectName", teamProject);

    public Uri GetArtifactViewerUrl(Uri artifactUri) => this.GetArtifactViewerUrl(artifactUri, "PublicAccessMapping");

    public Uri GetArtifactViewerUrl(Uri artifactUri, string accessMappingMoniker)
    {
      ArgumentUtility.CheckForNull<Uri>(artifactUri, nameof (artifactUri));
      ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri.AbsoluteUri);
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TeamProject"))
        return this.GetHomeUrl(artifactUri, accessMappingMoniker);
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "WorkItem"))
        return this.GetWorkItemEditorUrl(TswaHyperlinkBuilder.ArtifactToolSpecificIdToInt(artifactUri, artifactId), accessMappingMoniker);
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "Query"))
        return this.GetWorkItemQueryResultsUrl((Uri) null, TswaHyperlinkBuilder.ArtifactToolSpecificIdToGuid(artifactUri, artifactId), accessMappingMoniker);
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "VersionedItem"))
      {
        NameValueCollection queryString = UriUtility.ParseQueryString(UriUtility.UrlDecode(artifactId.ToolSpecificId));
        string folder = queryString[(string) null];
        if (folder == null)
          throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri));
        int result = 0;
        string s = queryString["changesetVersion"];
        if (!string.IsNullOrEmpty(s) && !int.TryParse(s, out result))
          throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri));
        return this.GetViewSourceControlItemUrl(TswaHyperlinkBuilder.PrependRootIfNeeded(folder), result, accessMappingMoniker);
      }
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "LatestItemVersion"))
        return this.GetViewSourceControlItemUrl(TswaHyperlinkBuilder.ArtifactToolSpecificIdToInt(artifactUri, artifactId), (string) null, accessMappingMoniker);
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "Changeset"))
        return this.GetChangesetDetailsUrl(TswaHyperlinkBuilder.ArtifactToolSpecificIdToInt(artifactUri, artifactId), accessMappingMoniker);
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "Shelveset"))
      {
        NameValueCollection queryString = UriUtility.ParseQueryString(UriUtility.UrlDecode(artifactId.ToolSpecificId));
        string shelvesetName = queryString[(string) null];
        if (string.IsNullOrEmpty(shelvesetName))
          throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri));
        string shelvesetOwner = queryString["shelvesetOwner"];
        if (string.IsNullOrEmpty(shelvesetOwner))
          throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri));
        return this.GetShelvesetDetailsUrl(shelvesetName, shelvesetOwner, accessMappingMoniker);
      }
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "ShelvedItem"))
      {
        NameValueCollection queryString = UriUtility.ParseQueryString(UriUtility.UrlDecode(artifactId.ToolSpecificId));
        string folder = queryString[(string) null];
        if (folder == null)
          throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri));
        string shelvesetName = queryString["shelvesetName"];
        if (string.IsNullOrEmpty(shelvesetName))
          throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri));
        string shelvesetOwner = queryString["shelvesetOwner"];
        if (string.IsNullOrEmpty(shelvesetOwner))
          throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri));
        return this.GetViewSourceControlShelvedItemUrl(TswaHyperlinkBuilder.PrependRootIfNeeded(folder), shelvesetName, shelvesetOwner, accessMappingMoniker);
      }
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "Build"))
        return this.GetViewBuildDetailsUrl(artifactUri, Guid.Empty, accessMappingMoniker);
      throw new NotSupportedException(TFCommonResources.WebAccess_UnSupportedArtifactType((object) artifactId.ArtifactType));
    }

    private Uri FormatUrl(string serviceType, string accessMappingMoniker, params string[] args) => this.FormatUrl(serviceType, accessMappingMoniker, false, false, args);

    private Uri FormatUrl(
      string serviceType,
      string accessMappingMoniker,
      bool variablePath,
      params string[] args)
    {
      return this.FormatUrl(serviceType, accessMappingMoniker, false, variablePath, args);
    }

    private Uri FormatUrl(
      string serviceType,
      string accessMappingMoniker,
      bool collectionLevel,
      bool variablePath,
      params string[] args)
    {
      string str1 = this.GetUrl(serviceType, accessMappingMoniker, collectionLevel).ToString();
      StringBuilder stringBuilder = new StringBuilder(str1);
      stringBuilder.Replace("tfs={tfsUrl}&", string.Empty);
      if (str1.IndexOf("{projectCollectionGuid}", StringComparison.OrdinalIgnoreCase) > 0)
      {
        if (this.CollectionId == Guid.Empty)
          throw new InvalidOperationException(TFCommonResources.CollectionIdIsNotInitialized());
        stringBuilder.Replace("{projectCollectionGuid}", this.CollectionId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
      }
      for (int index = 0; index < args.Length; index += 2)
      {
        string str2 = args[index];
        string str3 = args[index + 1];
        if (variablePath)
          stringBuilder.Replace("{" + str2 + "}", Uri.EscapeDataString(str3));
        else
          stringBuilder.Replace("{" + str2 + "}", UriUtility.UrlEncode(str3));
      }
      return new Uri(stringBuilder.ToString());
    }

    private string FormatQueryString(params string[] args) => this.FormatQueryString((Uri) null, args);

    private string FormatQueryString(Uri projectUri, params string[] args)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (projectUri != (Uri) null)
      {
        Guid guid = TswaHyperlinkBuilder.ArtifactToolSpecificIdToGuid(projectUri);
        stringBuilder.Append("pguid=");
        stringBuilder.Append(UriUtility.UrlEncode(guid.ToString()));
      }
      else
      {
        stringBuilder.Append("pcguid=");
        stringBuilder.Append(UriUtility.UrlEncode(this.CollectionId.ToString()));
      }
      for (int index = 0; index < args.Length; index += 2)
      {
        string str1 = args[index];
        string str2 = args[index + 1];
        if (str1 != null)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append('&');
          stringBuilder.Append(UriUtility.UrlEncode(str1));
        }
        if (str2 != null)
        {
          if (str1 != null)
            stringBuilder.Append('=');
          else if (stringBuilder.Length > 0)
            stringBuilder.Append('&');
          stringBuilder.Append(UriUtility.UrlEncode(str2));
        }
      }
      return stringBuilder.ToString();
    }

    private static void ValidateServerItemPath(string serverItemPath) => TswaHyperlinkBuilder.ValidateServerItemPath(serverItemPath, nameof (serverItemPath));

    private static void ValidateServerItemPath(string serverItemPath, string parameterName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serverItemPath, parameterName);
      if (!serverItemPath.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(TFCommonResources.InvalidPathMissingRoot((object) serverItemPath), nameof (serverItemPath));
    }

    private static string PrependRootIfNeeded(string folder)
    {
      if (!folder.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
        folder = "$/" + folder.TrimStart('/');
      return folder;
    }

    private static Guid ArtifactToolSpecificIdToGuid(Uri artifactUri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri.AbsoluteUri);
      return TswaHyperlinkBuilder.ArtifactToolSpecificIdToGuid(artifactUri, artifactId);
    }

    private static Guid ArtifactToolSpecificIdToGuid(Uri artifactUri, ArtifactId artifactId)
    {
      try
      {
        return new Guid(artifactId.ToolSpecificId);
      }
      catch (FormatException ex)
      {
        throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri), (Exception) ex);
      }
    }

    private static int ArtifactToolSpecificIdToInt(Uri artifactUri, ArtifactId artifactId)
    {
      int result;
      if (!int.TryParse(artifactId.ToolSpecificId, out result))
        throw new InvalidOperationException(CommonResources.MalformedArtifactId((object) artifactUri));
      return result;
    }
  }
}
