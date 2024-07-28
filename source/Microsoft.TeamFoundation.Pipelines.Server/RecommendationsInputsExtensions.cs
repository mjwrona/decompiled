// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.RecommendationsInputsExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class RecommendationsInputsExtensions
  {
    public static RecommendationsInputs ToServer(this Microsoft.TeamFoundation.Pipelines.WebApi.RecommendationsInputs recommendationsInputs)
    {
      if (recommendationsInputs == null)
        return (RecommendationsInputs) null;
      RecommendationsInputs server = new RecommendationsInputs(recommendationsInputs.ArchiveLink, recommendationsInputs.Callback.ToServer());
      IList<Microsoft.TeamFoundation.Pipelines.WebApi.LanguageInfo> languages = recommendationsInputs.Languages;
      server.Languages = languages != null ? (IReadOnlyList<LanguageInfo>) languages.Select<Microsoft.TeamFoundation.Pipelines.WebApi.LanguageInfo, LanguageInfo>((Func<Microsoft.TeamFoundation.Pipelines.WebApi.LanguageInfo, LanguageInfo>) (l => l.ToServer())).ToList<LanguageInfo>() : (IReadOnlyList<LanguageInfo>) null;
      return server;
    }

    public static ResultCallbackInfo ToServer(this Microsoft.TeamFoundation.Pipelines.WebApi.ResultCallbackInfo resultCallbackInfo) => resultCallbackInfo == null ? (ResultCallbackInfo) null : new ResultCallbackInfo(resultCallbackInfo.Url, resultCallbackInfo.SignatureKey);

    public static LanguageInfo ToServer(this Microsoft.TeamFoundation.Pipelines.WebApi.LanguageInfo languageInfo) => languageInfo == null ? (LanguageInfo) null : new LanguageInfo(languageInfo.Name, languageInfo.Percentage);
  }
}
