// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.EntitySearchPhraseSuggesterHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class EntitySearchPhraseSuggesterHelper
  {
    private EntitySearchSuggestPlatformResponse m_suggestResponse;

    public EntitySearchPhraseSuggesterHelper(
      EntitySearchSuggestPlatformResponse suggestResponse)
    {
      this.m_suggestResponse = suggestResponse;
    }

    public bool ReceivedValidSuggestions() => this.m_suggestResponse != null && this.m_suggestResponse.Count > 0;

    public string GetTopSuggestion() => this.m_suggestResponse.Suggestions.First<SuggestOption>().Text;
  }
}
