// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.Search.SearchServiceClientException
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Net;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.Search
{
  public sealed class SearchServiceClientException : Exception
  {
    public SearchServiceClientException(HttpStatusCode statusCode, string reasonPhrase)
      : base(reasonPhrase)
    {
      if (statusCode != HttpStatusCode.BadRequest)
        return;
      this.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
    }

    public int TracePoint => 12062082;
  }
}
