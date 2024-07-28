// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions
{
  public static class Constants
  {
    public const string FieldsId = "fields";
    public const string QueryId = "query";
    public const string AggregationsId = "aggs";
    public const string CustomQueryPluginId = "codeelement";
    public const string CustomQueryPluginSearchFieldParamId = "field";
    public const string CustomQueryPluginSearchStringParamId = "value";
    public const string CustomQueryPluginCodeTokenIdListParamId = "tokenKind";
    public const string CustomQueryScorerId = "codesearch_score_query";
    public const string FunctionScorerId = "function_score";
    public const string QueryRequestId = "query_id";
    public const int CodeTokenKindForPhrase = -1;
    public const string ShouldQueryMinimumMatch = "minimum_number_should_match";
    public const string AndQueryOperatorOperator = "and";
    public const string ContentField = "content";
    public const string NameField = "name";
    public const string ScoreModeSum = "sum";
    public const string TermQueryId = "term";
    public const string WildcardQueryId = "wildcard";
    public const string PhraseQueryId = "phrase";
    public const string MultiMatchQueryId = "multi_match";
    public const string TieBreakerId = "tie_breaker";
    public const string BoolQueryId = "bool";
    public const string MustQueryPartId = "must";
    public const string MustNotQueryPartId = "must_not";
    public const string FilterId = "filter";
    public const string QueryAnalyzer = "whitespace";
    public const string ContentAnalyzer = "contentanalyzer";
    public const string QueryRewriteFieldParamId = "rewrite";
    public const string QueryRewriteMethod = "top_terms_boost_100";
    public const string QueryRewriteMethodConstantScore = "constant_score";
    public const string FilterRewriteMethod = "constant_score";
    public const string TermsFilterId = "terms";
    public const string ExistsId = "exists";
    public const string FieldQueryId = "field";
    public const string ValidCharsInContentField = "\\w";
    internal const string CodeElementAggsId = "code_element_aggs";
    internal const string FilteredCodeElementAggsId = "filtered_code_element_aggs";
    internal const string TermAggsId = "term_aggs";
    internal const string FilteredAccountAggsId = "filtered_account_aggs";
    internal const string FilteredCollectionAggsId = "filtered_collection_aggs";
    internal const string FilteredProjectAggsId = "filtered_project_aggs";
    internal const string FilteredRepositoryAggsId = "filtered_repository_aggs";
    internal const string FilteredPathAggsId = "filtered_path_aggs";
    internal const string FilteredProjectTagsAggsId = "filtered_project_tags_aggs";
    internal const string FilteredLanguagesAggsId = "filtered_languages_aggs";
    internal const string FilteredProtocolTagsAggsId = "filtered_protocol_tags_aggs";
    internal const string FilteredFeedAggsId = "filtered_feed_aggs";
    internal const string FilteredViewAggsId = "filtered_view_aggs";
    internal const string FilteredVisibilityAggsId = "filtered_visibility_aggs";
    internal const string AccountAggsId = "account_aggs";
    internal const string CollectionAggsId = "collection_aggs";
    internal const string ProjectAggsId = "project_aggs";
    internal const string RepositoryAggsId = "repository_aggs";
    internal const string PathAggsId = "path_aggs";
    internal const string ProtocolAggsId = "protocol_aggs";
    internal const string FeedAggsId = "feed_aggs";
    internal const string ViewAggsId = "view_aggs";
    internal const string ProjectTagsAggsId = "project_tags_aggs";
    internal const string LanguagesAggsId = "languages_aggs";
    internal const string VisibilityAggsId = "visibility_aggs";
    internal const string PlatformConnectionTimeoutKey = "ConnectionTimeout";
    internal const int PlatformConnectionDefaultTimeout = 60;
    internal const int FileQueryScoreBoost = 10000000;
    internal const string ScriptLanguage = "almsearch_scripts";

    public static class WorkItemRecencyData
    {
      public static readonly string ProjectId = nameof (ProjectId);
      public static readonly string RecentWorkItemIds = nameof (RecentWorkItemIds);
      public static readonly string RecentAreaIds = nameof (RecentAreaIds);
    }
  }
}
