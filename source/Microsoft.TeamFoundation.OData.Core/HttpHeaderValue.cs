// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.HttpHeaderValue
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal sealed class HttpHeaderValue : Dictionary<string, HttpHeaderValueElement>
  {
    internal HttpHeaderValue()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    public override string ToString() => this.Count != 0 ? string.Join(",", this.Values.Select<HttpHeaderValueElement, string>((Func<HttpHeaderValueElement, string>) (element => element.ToString())).ToArray<string>()) : (string) null;
  }
}
