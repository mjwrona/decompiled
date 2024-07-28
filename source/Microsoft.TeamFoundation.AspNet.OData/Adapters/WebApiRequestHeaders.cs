// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiRequestHeaders
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiRequestHeaders : IWebApiHeaders
  {
    private HttpRequestHeaders innerCollection;

    public WebApiRequestHeaders(HttpRequestHeaders headers) => this.innerCollection = headers != null ? headers : throw Error.ArgumentNull(nameof (headers));

    public bool TryGetValues(string key, out IEnumerable<string> values) => this.innerCollection.TryGetValues(key, out values);
  }
}
