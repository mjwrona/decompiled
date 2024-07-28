// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search.SearchToken
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Search;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search
{
  public class SearchToken
  {
    public SearchToken(
      string originalTokenText,
      uint tokenStartPosition,
      string parsedTokenText,
      SearchParseError parseError)
    {
      this.OriginalTokenText = originalTokenText;
      this.TokenStartPosition = tokenStartPosition;
      this.ParsedTokenText = parsedTokenText;
      this.ParseError = (uint) parseError;
    }

    public string OriginalTokenText { get; private set; }

    public uint ParseError { get; private set; }

    public string ParsedTokenText { get; private set; }

    public uint TokenStartPosition { get; private set; }
  }
}
