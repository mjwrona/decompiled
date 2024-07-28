// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.EntitySearchSuggestPlatformResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities
{
  public class EntitySearchSuggestPlatformResponse
  {
    public EntitySearchSuggestPlatformResponse(
      int count,
      IEnumerable<SuggestOption> suggestions,
      bool isTimedOut,
      string suggestText)
    {
      this.Count = count;
      this.Suggestions = suggestions;
      this.SearchTimedOut = isTimedOut;
      this.SuggestText = suggestText;
    }

    public int Count { get; protected set; }

    public IEnumerable<SuggestOption> Suggestions { get; protected set; }

    public bool SearchTimedOut { get; protected set; }

    public string SuggestText { get; protected set; }
  }
}
