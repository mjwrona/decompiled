// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.ErrorCode
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public enum ErrorCode
  {
    AccountIsBeingReindexed = 0,
    IndexingNotStarted = 1,
    InvalidRequest = 2,
    PrefixWildcardQueryNotSupported = 3,
    MultiWordWithCodeFacetNotSupported = 4,
    AccountIsBeingOnboarded = 5,
    TakeResultValueTrimmedToMaxResultAllowed = 7,
    BranchesAreBeingIndexed = 8,
    FacetingNotEnabledAtScaleUnit = 9,
    WorkItemsNotAccessible = 10, // 0x0000000A
    EmptyQueryNotSupported = 11, // 0x0000000B
    OnlyWildcardQueryNotSupported = 12, // 0x0000000C
    ZeroResultsWithWildcard = 13, // 0x0000000D
    ZeroResultsWithFilter = 14, // 0x0000000E
    ZeroResultsWithWildcardAndFilter = 15, // 0x0000000F
    ZeroResultsWithNoWildcardNoFilter = 16, // 0x00000010
    PartialResultsDueToSearchRequestTimeout = 17, // 0x00000011
    PhraseQueriesWithCEFacetsNotSupported = 18, // 0x00000012
    WildcardQueriesWithCEFacetsNotSupported = 19, // 0x00000013
    ClearedScrollSearchRequestParam = 20, // 0x00000014
    InvalidScrollSearchRequestParam = 21, // 0x00000015
    StopWarmerRequests = 22, // 0x00000016
    WildCardPartialResults = 23, // 0x00000017
    ReindexingCompleted = 24, // 0x00000018
    ReindexingInProgress = 25, // 0x00000019
    InvalidIndexingMode = 26, // 0x0000001A
    ReindexingPausedForPrimaryIndexingUnit = 27, // 0x0000001B
    WildcardSubstringTooShort = 28, // 0x0000001C
    UnsupportedProximitySearchTerm = 29, // 0x0000001D
    PrefixSuffixSubStringTooShort = 30, // 0x0000001E
    SubstringWithInfixWildcard = 31, // 0x0000001F
    SubstringWithInfixWildcardCEFFacets = 32, // 0x00000020
    SubstringSearchCEFFacets = 33, // 0x00000021
    QuestionMarkWildcardSubstring = 34, // 0x00000022
    MixedWildcardSubstring = 35, // 0x00000023
  }
}
