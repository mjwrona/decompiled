// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.SasQueryBuilder
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Core
{
  public class SasQueryBuilder : UriQueryBuilder
  {
    public SasQueryBuilder(string sasToken) => this.AddRange((IEnumerable<KeyValuePair<string, string>>) HttpWebUtility.ParseQueryString(sasToken));

    public bool RequireHttps { get; private set; }

    public override void Add(string name, string value)
    {
      if (value != null)
        value = Uri.EscapeDataString(value);
      if (string.CompareOrdinal(name, "spr") == 0 && string.CompareOrdinal(value, "https") == 0)
        this.RequireHttps = true;
      this.Parameters.Add(name, value);
    }

    public override Uri AddToUri(Uri uri)
    {
      CommonUtility.AssertNotNull(nameof (uri), (object) uri);
      if (this.RequireHttps && string.CompareOrdinal(uri.Scheme, Uri.UriSchemeHttps) != 0)
        throw new ArgumentException("Cannot transform a Uri object using a StorageCredentials object that is marked HTTPS only.");
      return this.AddToUriCore(uri);
    }
  }
}
