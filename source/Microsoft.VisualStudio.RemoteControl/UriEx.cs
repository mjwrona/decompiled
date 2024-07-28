// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteControl.UriEx
// Assembly: Microsoft.VisualStudio.RemoteControl, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D9D0761-3208-49DD-A9E2-BF705DBE6B5D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.RemoteControl.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.RemoteControl
{
  internal static class UriEx
  {
    public static Tuple<Uri, string> SplitLastSegment(this Uri uri)
    {
      uri.RequiresArgumentNotNull<Uri>(nameof (uri));
      UriBuilder uriBuilder = new UriBuilder(uri);
      string[] source = uriBuilder.Path.Split('/');
      int count = source.Length - 1;
      uriBuilder.Path = ((IEnumerable<string>) source).Take<string>(count).Join("/");
      return Tuple.Create<Uri, string>(uriBuilder.Uri, source[count]);
    }

    public static Uri AddSegment(this Uri uri, string segment)
    {
      uri.RequiresArgumentNotNull<Uri>(nameof (uri));
      segment = segment ?? string.Empty;
      UriBuilder uriBuilder = new UriBuilder(uri);
      uriBuilder.Path = uriBuilder.Path.TrimEnd('/') + "/" + segment.TrimStart('/');
      return uriBuilder.Uri;
    }
  }
}
