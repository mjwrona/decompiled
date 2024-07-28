// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssResponseContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class VssResponseContext
  {
    internal VssResponseContext(HttpStatusCode statusCode, HttpResponseHeaders headers)
    {
      if (headers.Contains(nameof (ActivityId)))
      {
        Guid result;
        Guid.TryParse(headers.GetValues(nameof (ActivityId)).FirstOrDefault<string>(), out result);
        this.ActivityId = result;
      }
      IEnumerable<string> values;
      if (headers.TryGetValues("X-VSS-PerfData", out values))
        this.Timings = JsonConvert.DeserializeObject<IDictionary<string, PerformanceTimingGroup>>(values.First<string>());
      this.HttpStatusCode = statusCode;
      this.Headers = headers;
    }

    public bool TryGetException(out Exception value)
    {
      value = this.Exception;
      return this.Exception != null;
    }

    public bool TryGetErrorCode(out string value)
    {
      value = (string) null;
      if (this.Exception == null)
        return false;
      string message = this.Exception.Message;
      Match match1 = Regex.Match(message, "(TF[0-9]+)");
      if (match1.Success)
      {
        value = match1.Value;
        return true;
      }
      Match match2 = Regex.Match(message, "(VSS[0-9]+)");
      if (!match2.Success)
        return false;
      value = match2.Value;
      return true;
    }

    public HttpStatusCode HttpStatusCode { get; private set; }

    public Guid ActivityId { get; private set; }

    public Exception Exception { get; internal set; }

    public IDictionary<string, PerformanceTimingGroup> Timings { get; private set; }

    public HttpResponseHeaders Headers { get; private set; }
  }
}
