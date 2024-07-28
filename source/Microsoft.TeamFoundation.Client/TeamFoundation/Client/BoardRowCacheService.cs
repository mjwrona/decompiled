// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.BoardRowCacheService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BoardRowCacheService : BoardCacheServiceBase
  {
    protected override Task<IReadOnlyList<string>> GetValueAsync(
      Guid scope,
      SuggestedValueCacheService.ScopeSuggestedValue scopeValues)
    {
      return ((WorkHttpClientBase) this.m_proxy).GetRowSuggestedValuesAsync(scope).ContinueWith<IReadOnlyList<string>>((Func<Task<List<BoardSuggestedValue>>, IReadOnlyList<string>>) (t => this.ProcessGetSuggestedValuesAsyncTaskResult((Task) t, scopeValues, (Action) (() => scopeValues.Values = t.Result.Select<BoardSuggestedValue, string>((Func<BoardSuggestedValue, string>) (row => row.Name)).ToList<string>()))));
    }
  }
}
