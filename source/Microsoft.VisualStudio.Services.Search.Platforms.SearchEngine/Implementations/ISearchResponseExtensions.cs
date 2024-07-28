// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ISearchResponseExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations
{
  public static class ISearchResponseExtensions
  {
    public static string SerializeResponseProfile<T>(this ISearchResponse<T> queryResponse) where T : class
    {
      try
      {
        return queryResponse.Profile == null ? string.Empty : JsonConvert.SerializeObject((object) queryResponse.Profile).CompactJson();
      }
      catch (Exception ex)
      {
        return FormattableString.Invariant(FormattableStringFactory.Create("{0} failed with exception [{1}].", (object) nameof (SerializeResponseProfile), (object) ex));
      }
    }
  }
}
