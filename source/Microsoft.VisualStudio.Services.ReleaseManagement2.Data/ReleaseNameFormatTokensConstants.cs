// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.ReleaseNameFormatTokensConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  public static class ReleaseNameFormatTokensConstants
  {
    public const string DefinitionName = "Release.DefinitionName";
    public const string ReleaseId = "Release.ReleaseId";
    public const string ReleaseDescription = "Release.ReleaseDescription";
    public const string TeamProject = "System.TeamProject";
    public const string DateTimePrefix = "Date:";
    public const string RevisionPrefix = "Rev:";
    public const string PartiallyExpandedRevisionTokenFormat = "{0}$$REV{1}$$";
    public const string PartiallyExpandedReleaseIdToken = "$$RELEASEID$$";
    public const string DefaultReleaseNameFormat = "Release-$(Rev:r)";
    public const string PartiallyExpandedDefaultReleaseNameFormat = "Release-$$REV1$$";
    public const string DefaultDraftReleaseNameFormat = "Draft-$(Rev:r)";
    private static readonly char[] ReleaseNameFormatInvalidCharactersArray = ((IEnumerable<char>) FileSpec.IllegalNtfsCharsAndWildcards).Union<char>((IEnumerable<char>) new char[1]
    {
      '@'
    }).ToArray<char>();

    public static ReadOnlyCollection<char> ReleaseNameFormatInvalidCharacters => new ReadOnlyCollection<char>((IList<char>) ReleaseNameFormatTokensConstants.ReleaseNameFormatInvalidCharactersArray);
  }
}
