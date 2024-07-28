// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsODataRequestMessage
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class AnalyticsODataRequestMessage : IODataRequestMessage
  {
    private Dictionary<string, string> _headers;

    public AnalyticsODataRequestMessage(HttpRequestMessage request) => this._headers = request.Headers.Concat<KeyValuePair<string, IEnumerable<string>>>((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) request.Content.Headers).Select<KeyValuePair<string, IEnumerable<string>>, KeyValuePair<string, string>>((Func<KeyValuePair<string, IEnumerable<string>>, KeyValuePair<string, string>>) (kvp => new KeyValuePair<string, string>(kvp.Key, string.Join(";", kvp.Value.Where<string>((Func<string, bool>) (val => !string.IsNullOrWhiteSpace(val))))))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, string>, string>) (kvp => kvp.Value));

    public IEnumerable<KeyValuePair<string, string>> Headers => throw new NotImplementedException();

    public Uri Url
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public string Method
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public string GetHeader(string headerName)
    {
      string str;
      return this._headers.TryGetValue(headerName, out str) ? str : (string) null;
    }

    public Stream GetStream() => throw new NotImplementedException();

    public void SetHeader(string headerName, string headerValue) => throw new NotImplementedException();
  }
}
