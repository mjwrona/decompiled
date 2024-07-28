// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpRequestHeadersExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HttpRequestHeadersExtensions
  {
    public static string GetSingleValue(
      this HttpRequestHeaders headers,
      string name,
      bool splitOnComma = true)
    {
      IEnumerable<string> values;
      if (!headers.TryGetValues(name, out values) || !values.Any<string>())
        throw new MissingRequiredHeaderException(FrameworkResources.MissingRequiredHeaderError((object) name));
      if (values.Count<string>() == 1 & splitOnComma)
        values = (IEnumerable<string>) values.Single<string>().Split(',');
      return values.Count<string>() <= 1 ? values.Single<string>() : throw new MultipleHeaderValuesException(FrameworkResources.MultipleHeaderValuesError((object) name));
    }
  }
}
