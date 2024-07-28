// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WitBatchResponse
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WitBatchResponse
  {
    [DataMember(EmitDefaultValue = false)]
    public int Code { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, string> Headers { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Body { get; set; }

    public T ParseBody<T>(MediaTypeFormatter formatter = null) where T : class
    {
      string key = "Content-Type";
      if (string.IsNullOrEmpty(this.Body) || !this.Headers.ContainsKey(key))
        return default (T);
      string mediaType;
      string charSet;
      this.ParseHeaderValues(this.Headers[key], out mediaType, out charSet);
      if (string.IsNullOrEmpty(mediaType) || string.IsNullOrEmpty(charSet))
        return default (T);
      MediaTypeFormatter mediaTypeFormatter = formatter == null ? (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true) : formatter;
      return new StringContent(this.Body, Encoding.GetEncoding(charSet), mediaType).ReadAsAsync<T>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
      {
        mediaTypeFormatter
      }).Result;
    }

    private void ParseHeaderValues(string header, out string mediaType, out string charSet)
    {
      mediaType = (string) null;
      charSet = (string) null;
      string[] source1 = header.Split(';');
      if (source1.Length < 2)
        return;
      mediaType = source1[0];
      IEnumerable<string> source2 = ((IEnumerable<string>) source1).Where<string>((Func<string, bool>) (v => v.Trim().StartsWith("charset", StringComparison.OrdinalIgnoreCase)));
      if (!source2.Any<string>())
        return;
      charSet = source2.First<string>();
      charSet = charSet.Substring(charSet.IndexOf('=') + 1);
    }
  }
}
