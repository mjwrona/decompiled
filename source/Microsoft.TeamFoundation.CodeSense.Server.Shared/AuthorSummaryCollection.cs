// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.AuthorSummaryCollection
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class AuthorSummaryCollection : List<AuthorSummary>
  {
    [JsonConstructor]
    public AuthorSummaryCollection()
    {
    }

    private AuthorSummaryCollection(IEnumerable<AuthorSummary> authors)
      : base(authors)
    {
    }

    public AuthorSummaryCollection WithChange(CodeElementChangeResult newChange)
    {
      string newAuthor = newChange.AuthorUniqueName;
      AuthorSummary currentSummary = this.FirstOrDefault<AuthorSummary>((Func<AuthorSummary, bool>) (i => i.AuthorUniqueName == newAuthor));
      AuthorSummary authorSummary = currentSummary != null ? new AuthorSummary(currentSummary, newChange) : new AuthorSummary(newChange);
      return new AuthorSummaryCollection(this.Except<AuthorSummary>((IEnumerable<AuthorSummary>) new AuthorSummary[1]
      {
        currentSummary
      }).Concat<AuthorSummary>((IEnumerable<AuthorSummary>) new AuthorSummary[1]
      {
        authorSummary
      }));
    }
  }
}
