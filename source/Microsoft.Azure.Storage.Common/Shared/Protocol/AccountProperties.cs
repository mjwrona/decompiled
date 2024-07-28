// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.AccountProperties
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  public sealed class AccountProperties
  {
    public string SkuName { get; private set; }

    public string AccountKind { get; private set; }

    internal static AccountProperties FromHttpResponseHeaders(
      HttpResponseHeaders httpResponseHeaders)
    {
      IEnumerable<string> values;
      return new AccountProperties()
      {
        SkuName = httpResponseHeaders.TryGetValues("x-ms-sku-name", out values) ? values.Single<string>() : (string) null,
        AccountKind = httpResponseHeaders.TryGetValues("x-ms-account-kind", out values) ? values.Single<string>() : (string) null
      };
    }
  }
}
