// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.InfoCodes
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public enum InfoCodes
  {
    Ok = 0,
    AccountIsBeingReindexed = 1,
    IndexingNotStarted = 2,
    InvalidRequest = 3,
    PrefixWildcardQueryNotSupported = 4,
    MultiWordWithCodeFacetNotSupported = 5,
    AccountIsBeingOnboarded = 6,
    TakeResultValueTrimmedToMaxResultAllowed = 8,
    BranchesAreBeingIndexed = 9,
    FacetingNotEnabledAtScaleUnit = 10, // 0x0000000A
    WorkItemsNotAccessible = 11, // 0x0000000B
    EmptyQueryNotSupported = 12, // 0x0000000C
    OnlyWildcardQueryNotSupported = 13, // 0x0000000D
    ZeroResultsWithWildcard = 14, // 0x0000000E
    ZeroResultsWithFilter = 15, // 0x0000000F
    ZeroResultsWithWildcardAndFilter = 16, // 0x00000010
    ZeroResultsWithNoWildcardNoFilter = 17, // 0x00000011
    PartialResultsDueToSearchRequestTimeout = 18, // 0x00000012
    PhraseQueriesWithCEFacetsNotSupported = 19, // 0x00000013
    WildcardQueriesWithCEFacetsNotSupported = 20, // 0x00000014
    ClearedScrollSearchRequestParam = 21, // 0x00000015
    InvalidScrollSearchRequestParam = 22, // 0x00000016
    StopWarmerRequests = 23, // 0x00000017
    WildCardPartialResults = 24, // 0x00000018
    ReindexingCompleted = 25, // 0x00000019
    ReindexingInProgress = 26, // 0x0000001A
    InvalidIndexingMode = 27, // 0x0000001B
    ReindexingPausedForPrimaryIndexingUnit = 28, // 0x0000001C
    WildcardSubstringTooShort = 29, // 0x0000001D
    UnsupportedProximitySearchTerm = 30, // 0x0000001E
    PrefixSuffixSubStringTooShort = 31, // 0x0000001F
    SubstringWithInfixWildcard = 32, // 0x00000020
    SubstringWithInfixWildcardCEFFacets = 33, // 0x00000021
    SubstringSearchCEFFacets = 34, // 0x00000022
    QuestionMarkWildcardSubstring = 35, // 0x00000023
    MixedWildcardSubstring = 36, // 0x00000024
  }
}
