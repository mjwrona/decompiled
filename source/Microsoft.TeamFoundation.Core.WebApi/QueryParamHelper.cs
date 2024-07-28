// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.QueryParamHelper
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  internal static class QueryParamHelper
  {
    internal static void AddNonNullParam(
      List<KeyValuePair<string, string>> queryParams,
      string paramName,
      object paramValue)
    {
      if (paramValue == null)
        return;
      queryParams.Add(paramName, paramValue.ToString());
    }
  }
}
