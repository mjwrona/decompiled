// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search.SearchTokenHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Search;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search
{
  public class SearchTokenHelper : ISearchTokenHelper
  {
    private SearchToken[] m_tokens;

    public SearchTokenHelper(SearchToken[] tokens) => this.m_tokens = tokens;

    public object[] GetTokens() => (object[]) this.m_tokens;

    public bool IsFilterToken(object token) => token is SearchFilterToken;

    public string GetOriginalTokenText(object token) => ((SearchToken) token).OriginalTokenText;

    public string GetParsedTokenText(object token) => ((SearchToken) token).ParsedTokenText;

    public uint GetTokenStartPosition(object token) => ((SearchToken) token).TokenStartPosition;

    public uint GetParseError(object token) => ((SearchToken) token).ParseError;

    public string GetFilterField(object token) => ((SearchFilterToken) token).FilterField;

    public string GetFilterValue(object token) => ((SearchFilterToken) token).FilterValue;

    public uint GetFilterSeperatorPosition(object token) => ((SearchFilterToken) token).FilterSeparatorPosition;

    public uint GetFilterTokenType(object token) => ((SearchFilterToken) token).FilterTokenType;
  }
}
