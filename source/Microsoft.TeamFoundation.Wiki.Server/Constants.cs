// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Constants
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class Constants
  {
    public const string WikiRootPath = "/";
    public const string WikiRepoNameSubstring = "wiki";
    public const string WikiPathSeparator = "/";
    public static readonly char[] WikiPathSeparatorCharacters = new char[1]
    {
      '/'
    };
    public const char WikiPathSeparatorCharacter = '/';
    private const string BaseWikiProjectPropertyPrefix = "System.Wiki";
    public const string V1ProjectPropertyKey = "System.WikiRepoId";
    public const string WikiProjectPropertyWildCardKey = "System.Wiki*";
    public const string V2ProjectPropertyKeyPrefix = "System.Wiki.";
    public const string V2ProjectPropertyKey = "System.Wiki.{0}";
    public const string WikiRenameFF = "WebAccess.Wiki.RenameWiki";
    public const string WikiPropertiesCleanupFF = "WebAccess.Wiki.WikiPropertiesCleanup";
    public const string WikiArtifactVisitPublishFF = "Wiki.Artifact.Visit.Publish";
    public const string WikiPageVisitArtifactKindId = "e9a7d0f1-3460-4118-89cc-bf15847656ae";
    public const string WikiPageVisitPagePathProperty = "PagePath";
    public const string WikiVersionPushWaterMarkRegistryKeyPrefix = "/WikiVersion/PushWaterMark";
  }
}
