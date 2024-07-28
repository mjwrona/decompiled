// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TagsCacheService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TagsCacheService : SuggestedValueCacheService
  {
    protected override void OnInitialize() => this.m_proxy = (IVssHttpClient) this.m_projectCollection.GetClient<TaggingHttpClient>();

    public IReadOnlyList<string> GetCachedTags(Guid scope) => this.GetCachedValues(scope);

    protected override Task<IReadOnlyList<string>> GetValueAsync(
      Guid scope,
      SuggestedValueCacheService.ScopeSuggestedValue scopeValues)
    {
      return ((TaggingHttpClient) this.m_proxy).GetTagsAsync(scope).ContinueWith<IReadOnlyList<string>>((Func<Task<WebApiTagDefinitionList>, IReadOnlyList<string>>) (t => this.ProcessGetSuggestedValuesAsyncTaskResult((Task) t, scopeValues, (Action) (() =>
      {
        scopeValues.Values = t.Result.Select<WebApiTagDefinition, string>((Func<WebApiTagDefinition, string>) (tag => tag.Name)).ToList<string>();
        scopeValues.Values.Sort((IComparer<string>) VssStringComparer.TagName);
      }))));
    }
  }
}
