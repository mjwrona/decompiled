// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Extensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class Extensions
  {
    public static bool ContainsContentEncoding(
      this HttpContentHeaders @this,
      string contentEncoding)
    {
      return @this.ContentEncoding.Contains<string>(contentEncoding, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public static void SetContentEncoding(this HttpContentHeaders @this, string contentEncoding)
    {
      if (@this.ContainsContentEncoding(contentEncoding))
        return;
      @this.ContentEncoding.Add(contentEncoding);
    }

    public static bool HasXpressContentEncoding(this HttpContentHeaders headers)
    {
      if (headers == null)
        return false;
      bool? nullable = headers.ContentEncoding?.Contains("xpress");
      bool flag = true;
      return nullable.GetValueOrDefault() == flag & nullable.HasValue;
    }
  }
}
