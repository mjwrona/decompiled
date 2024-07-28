// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search.SearchFilterToken
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Search;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search
{
  public class SearchFilterToken : SearchToken
  {
    public SearchFilterToken(
      string originalTokenText,
      uint tokenStartPosition,
      string parsedTokenText,
      string filterField,
      string filterValue,
      SearchFilterTokenType filterTokenType,
      uint filterSeparatorPosition,
      SearchParseError parseError)
      : base(originalTokenText, tokenStartPosition, parsedTokenText, parseError)
    {
      this.FilterField = filterField;
      this.FilterValue = filterValue;
      this.FilterSeparatorPosition = filterSeparatorPosition;
      this.FilterTokenType = (uint) filterTokenType;
    }

    public string FilterField { get; private set; }

    public uint FilterSeparatorPosition { get; private set; }

    public string FilterValue { get; private set; }

    public uint FilterTokenType { get; private set; }
  }
}
