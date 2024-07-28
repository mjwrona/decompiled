// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Enums.CodeSearchTag
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Common.Enums
{
  [Flags]
  [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags", Justification = "Expectation is to assign enums for all possible bit positions which is not necessary here")]
  public enum CodeSearchTag : long
  {
    None = 0,
    And = 1,
    Or = 2,
    Not = 4,
    PathFilter = 8,
    ProjFilter = 16, // 0x0000000000000010
    RepoFilter = 32, // 0x0000000000000020
    ExtFilter = 64, // 0x0000000000000040
    FileFilter = 128, // 0x0000000000000080
    CEFilter = 256, // 0x0000000000000100
    Phrase = 512, // 0x0000000000000200
    ProjFacet = 1024, // 0x0000000000000400
    RepoFacet = 2048, // 0x0000000000000800
    CEFacet = 4096, // 0x0000000000001000
    WildcardQuestion = 8192, // 0x0000000000002000
    WildcardAsterisk = 16384, // 0x0000000000004000
    SingleWord = 32768, // 0x0000000000008000
    MultiWord = 65536, // 0x0000000000010000
    PathFacet = 131072, // 0x0000000000020000
    AccountFacet = 262144, // 0x0000000000040000
    CodePrefixWildcard = 524288, // 0x0000000000080000
    CodePostfixWildcard = 1048576, // 0x0000000000100000
    CollectionFacet = 2097152, // 0x0000000000200000
    BranchFacet = 4194304, // 0x0000000000400000
    BranchFilter = 8388608, // 0x0000000000800000
    DefaultBranch = 16777216, // 0x0000000001000000
    AllBranch = 33554432, // 0x0000000002000000
    CodeWildcardQuestion = 67108864, // 0x0000000004000000
    CodeWildcardAsterisk = 134217728, // 0x0000000008000000
    CodeElementPrefixWildcard = 268435456, // 0x0000000010000000
    CodePrefixSuffixWildcardTerm = 536870912, // 0x0000000020000000
    CodeInfixSuffixWildcardTerm = 1073741824, // 0x0000000040000000
    CodeElementInfixSuffixWildcardTerm = 2147483648, // 0x0000000080000000
    CodeSubStringTerm = 4294967296, // 0x0000000100000000
    CodeNearTerm = 8589934592, // 0x0000000200000000
    CodeBeforeTerm = 17179869184, // 0x0000000400000000
    CodeAfterTerm = 34359738368, // 0x0000000800000000
    CodeWildcardSubstringTooShort = 68719476736, // 0x0000001000000000
    CodeElementWildcardSubstringTooShort = 137438953472, // 0x0000002000000000
    UnsupportedProximityTerm = 274877906944, // 0x0000004000000000
    CodeElementWildcardSubstring = 549755813888, // 0x0000008000000000
    CodeElementPrefixSuffixWildcardSubstring = 1099511627776, // 0x0000010000000000
    CodeSubstringWithInfixWildcard = 2199023255552, // 0x0000020000000000
    CodeSubstringTooShort = 4398046511104, // 0x0000040000000000
    CodeElementSubStringWithInfixWildcard = 8796093022208, // 0x0000080000000000
    CodeSubstringWithQuestionMarkWildcard = 17592186044416, // 0x0000100000000000
    CodeSubstringWithMixedWildcard = 35184372088832, // 0x0000200000000000
  }
}
