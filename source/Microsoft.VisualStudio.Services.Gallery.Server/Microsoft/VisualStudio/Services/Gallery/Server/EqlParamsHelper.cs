// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.EqlParamsHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class EqlParamsHelper
  {
    [StaticSafe]
    private static readonly IDictionary<string, SearchFilterType> EqlParamsToFilterTypeMap = (IDictionary<string, SearchFilterType>) new Dictionary<string, SearchFilterType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "name:",
        SearchFilterType.Name
      },
      {
        "publisher:",
        SearchFilterType.Publisher
      },
      {
        "tag:",
        SearchFilterType.TagName
      },
      {
        "category:",
        SearchFilterType.Category
      },
      {
        "target:",
        SearchFilterType.InstallationTarget
      }
    };

    public string GetApplicableEqlParam(string queryPart)
    {
      foreach (string key in (IEnumerable<string>) EqlParamsHelper.EqlParamsToFilterTypeMap.Keys)
      {
        if (queryPart.StartsWith(key, StringComparison.OrdinalIgnoreCase))
          return key;
      }
      return (string) null;
    }

    public string GetValueFromQueryForEqlParam(string queryPart, string eqlParam) => queryPart.StartsWith(eqlParam, StringComparison.OrdinalIgnoreCase) ? queryPart.Substring(eqlParam.Length) : (string) null;

    public SearchFilterType GetFilterTypeForEqlParam(string eqlParam)
    {
      SearchFilterType filterTypeForEqlParam;
      EqlParamsHelper.EqlParamsToFilterTypeMap.TryGetValue(eqlParam, out filterTypeForEqlParam);
      return filterTypeForEqlParam;
    }
  }
}
