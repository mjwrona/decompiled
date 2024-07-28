// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureClientBase
// Assembly: Microsoft.VisualStudio.Services.Commerce.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A442E579-88AD-441C-B92A-FDB0C6C9E30B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public abstract class AzureClientBase
  {
    private static CommerceAntiXssEncoder antiXssEncoder = new CommerceAntiXssEncoder();

    protected Uri PrepareUri(
      Uri baseUri,
      string apiVersion,
      IDictionary<string, string> queryParameters = null,
      string urlPath = "")
    {
      if (queryParameters == null)
        queryParameters = (IDictionary<string, string>) new Dictionary<string, string>();
      queryParameters.TryAdd<string, string>("api-version", apiVersion);
      string str = HttpUtil.ConstructQueryParameters(HttpUtil.TrimUrl(baseUri.ToString()) + urlPath, queryParameters);
      return new Uri(AzureClientBase.antiXssEncoder.UrlPathEncode(str));
    }
  }
}
