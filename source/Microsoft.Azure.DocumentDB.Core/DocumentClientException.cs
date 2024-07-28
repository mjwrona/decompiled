// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentClientException
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.Azure.Documents
{
  [Serializable]
  public class DocumentClientException : Exception
  {
    private Error error;
    private SubStatusCodes? substatus;
    private INameValueCollection responseHeaders;

    internal DocumentClientException(
      Error errorResource,
      HttpResponseHeaders responseHeaders,
      HttpStatusCode? statusCode)
      : base(DocumentClientException.MessageWithActivityId(errorResource.Message, responseHeaders))
    {
      this.error = errorResource;
      this.responseHeaders = (INameValueCollection) new DictionaryNameValueCollection();
      this.StatusCode = statusCode;
      if (responseHeaders != null)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> responseHeader in (System.Net.Http.Headers.HttpHeaders) responseHeaders)
          this.responseHeaders.Add(responseHeader.Key, string.Join(",", responseHeader.Value));
      }
      Guid activityId = Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId;
      if (this.responseHeaders.Get("x-ms-activity-id") == null)
        this.responseHeaders.Set("x-ms-activity-id", Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId.ToString());
      this.LSN = -1L;
      this.PartitionKeyRangeId = (string) null;
      HttpStatusCode? statusCode1 = this.StatusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.Gone;
      if (statusCode1.GetValueOrDefault() == httpStatusCode & statusCode1.HasValue)
        return;
      object[] objArray = new object[3];
      statusCode1 = this.StatusCode;
      objArray[0] = (object) (HttpStatusCode) ((int) statusCode1 ?? 0);
      objArray[1] = (object) errorResource.Message;
      objArray[2] = (object) DocumentClientException.SerializeHTTPResponseHeaders(responseHeaders);
      DefaultTrace.TraceError("DocumentClientException with status code: {0}, message: {1}, and response headers: {2}", objArray);
    }

    internal DocumentClientException(
      string message,
      Exception innerException,
      HttpStatusCode? statusCode,
      Uri requestUri = null,
      string statusDescription = null)
      : this(DocumentClientException.MessageWithActivityId(message), innerException, (INameValueCollection) null, statusCode, requestUri)
    {
    }

    internal DocumentClientException(
      string message,
      Exception innerException,
      HttpResponseHeaders responseHeaders,
      HttpStatusCode? statusCode,
      Uri requestUri = null,
      SubStatusCodes? substatusCode = null)
      : base(DocumentClientException.MessageWithActivityId(message, responseHeaders), innerException)
    {
      this.responseHeaders = (INameValueCollection) new DictionaryNameValueCollection();
      this.StatusCode = statusCode;
      this.substatus = substatusCode;
      if (this.substatus.HasValue)
        this.responseHeaders["x-ms-substatus"] = ((int) this.substatus.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (responseHeaders != null)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> responseHeader in (System.Net.Http.Headers.HttpHeaders) responseHeaders)
          this.responseHeaders.Add(responseHeader.Key, string.Join(",", responseHeader.Value));
      }
      Guid activityId = Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId;
      this.responseHeaders.Set("x-ms-activity-id", Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId.ToString());
      this.RequestUri = requestUri;
      this.LSN = -1L;
      this.PartitionKeyRangeId = (string) null;
      HttpStatusCode? statusCode1 = this.StatusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.Gone;
      if (statusCode1.GetValueOrDefault() == httpStatusCode & statusCode1.HasValue)
        return;
      DefaultTrace.TraceError("DocumentClientException with status code {0}, message: {1}, inner exception: {2}, and response headers: {3}", (object) (HttpStatusCode) ((int) this.StatusCode ?? 0), (object) message, innerException != null ? (object) innerException.ToString() : (object) "null", (object) DocumentClientException.SerializeHTTPResponseHeaders(responseHeaders));
    }

    internal DocumentClientException(
      string message,
      Exception innerException,
      INameValueCollection responseHeaders,
      HttpStatusCode? statusCode,
      SubStatusCodes? substatusCode,
      Uri requestUri = null)
      : this(message, innerException, responseHeaders, statusCode, requestUri)
    {
      this.substatus = substatusCode;
      this.responseHeaders["x-ms-substatus"] = ((int) this.substatus.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    internal DocumentClientException(
      string message,
      Exception innerException,
      INameValueCollection responseHeaders,
      HttpStatusCode? statusCode,
      Uri requestUri = null)
      : base(DocumentClientException.MessageWithActivityId(message, responseHeaders), innerException)
    {
      this.responseHeaders = (INameValueCollection) new DictionaryNameValueCollection();
      this.StatusCode = statusCode;
      if (responseHeaders != null)
        this.responseHeaders.Add(responseHeaders);
      Guid activityId = Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId;
      this.responseHeaders.Set("x-ms-activity-id", Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId.ToString());
      this.RequestUri = requestUri;
      this.LSN = -1L;
      this.PartitionKeyRangeId = (string) null;
      HttpStatusCode? statusCode1 = this.StatusCode;
      HttpStatusCode httpStatusCode = HttpStatusCode.Gone;
      if (statusCode1.GetValueOrDefault() == httpStatusCode & statusCode1.HasValue)
        return;
      DefaultTrace.TraceError("DocumentClientException with status code {0}, message: {1}, inner exception: {2}, and response headers: {3}", (object) (HttpStatusCode) ((int) this.StatusCode ?? 0), (object) message, innerException != null ? (object) innerException.ToString() : (object) "null", (object) DocumentClientException.SerializeHTTPResponseHeaders(responseHeaders));
    }

    internal DocumentClientException(
      string message,
      HttpStatusCode statusCode,
      SubStatusCodes subStatusCode)
      : this(message, (Exception) null, new HttpStatusCode?(statusCode))
    {
      this.substatus = new SubStatusCodes?(subStatusCode);
      this.responseHeaders["x-ms-substatus"] = ((int) this.substatus.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public Error Error
    {
      get
      {
        if (this.error == null)
          this.error = new Error()
          {
            Code = this.StatusCode.ToString(),
            Message = this.Message
          };
        return this.error;
      }
      internal set => this.error = value;
    }

    public string ActivityId => this.responseHeaders != null ? this.responseHeaders["x-ms-activity-id"] : (string) null;

    public TimeSpan RetryAfter
    {
      get
      {
        if (this.responseHeaders != null)
        {
          string responseHeader = this.responseHeaders["x-ms-retry-after-ms"];
          if (!string.IsNullOrEmpty(responseHeader))
          {
            long result = 0;
            if (long.TryParse(responseHeader, NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture, out result))
              return TimeSpan.FromMilliseconds((double) result);
          }
        }
        return TimeSpan.Zero;
      }
    }

    public NameValueCollection ResponseHeaders => this.responseHeaders.ToNameValueCollection();

    internal INameValueCollection Headers
    {
      get => this.responseHeaders;
      set => this.responseHeaders = value;
    }

    public HttpStatusCode? StatusCode { get; internal set; }

    internal string StatusDescription { get; set; }

    public double RequestCharge => this.responseHeaders != null ? Helpers.GetHeaderValueDouble(this.responseHeaders, "x-ms-request-charge", 0.0) : 0.0;

    public string ScriptLog => Helpers.GetScriptLogHeader(this.Headers);

    public override string Message
    {
      get
      {
        string str = this.RequestStatistics == null ? string.Empty : this.RequestStatistics.ToString();
        return this.RequestUri != (Uri) null ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessageAddRequestUri, (object) base.Message, (object) this.RequestUri.PathAndQuery, (object) str, (object) CustomTypeExtensions.GenerateBaseUserAgentString()) : (string.IsNullOrEmpty(str) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}, {1}", (object) base.Message, (object) CustomTypeExtensions.GenerateBaseUserAgentString()) : string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "{0}, {1}, {2}", (object) base.Message, (object) str, (object) CustomTypeExtensions.GenerateBaseUserAgentString()));
      }
    }

    internal virtual string PublicMessage
    {
      get
      {
        string str = this.RequestStatistics == null ? string.Empty : this.RequestStatistics.ToString();
        return this.RequestUri != (Uri) null ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessageAddRequestUri, (object) base.Message, (object) this.RequestUri.PathAndQuery, (object) str, (object) CustomTypeExtensions.GenerateBaseUserAgentString()) : (string.IsNullOrEmpty(str) ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}, {1}", (object) base.Message, (object) CustomTypeExtensions.GenerateBaseUserAgentString()) : string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "{0}, {1}, {2}", (object) base.Message, (object) str, (object) CustomTypeExtensions.GenerateBaseUserAgentString()));
      }
    }

    internal string RawErrorMessage => base.Message;

    internal IClientSideRequestStatistics RequestStatistics { get; set; }

    internal long LSN { get; set; }

    internal string PartitionKeyRangeId { get; set; }

    internal string ResourceAddress { get; set; }

    internal Uri RequestUri { get; private set; }

    private static string MessageWithActivityId(
      string message,
      INameValueCollection responseHeaders)
    {
      string[] source = (string[]) null;
      if (responseHeaders != null)
        source = responseHeaders.GetValues("x-ms-activity-id");
      return source != null ? DocumentClientException.MessageWithActivityId(message, ((IEnumerable<string>) source).FirstOrDefault<string>()) : DocumentClientException.MessageWithActivityId(message);
    }

    private static string MessageWithActivityId(string message, HttpResponseHeaders responseHeaders)
    {
      IEnumerable<string> values = (IEnumerable<string>) null;
      return responseHeaders != null && responseHeaders.TryGetValues("x-ms-activity-id", out values) && values != null ? DocumentClientException.MessageWithActivityId(message, values.FirstOrDefault<string>()) : DocumentClientException.MessageWithActivityId(message);
    }

    private static string MessageWithActivityId(string message, string activityIdFromHeaders = null)
    {
      string str;
      if (!string.IsNullOrEmpty(activityIdFromHeaders))
      {
        str = activityIdFromHeaders;
      }
      else
      {
        if (!(Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId != Guid.Empty))
          return message;
        str = Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId.ToString();
      }
      return message.Contains(str) ? message : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}" + Environment.NewLine + "ActivityId: {1}", (object) message, (object) str);
    }

    private static string SerializeHTTPResponseHeaders(HttpResponseHeaders responseHeaders)
    {
      if (responseHeaders == null)
        return "null";
      StringBuilder stringBuilder = new StringBuilder("{");
      stringBuilder.Append(Environment.NewLine);
      foreach (KeyValuePair<string, IEnumerable<string>> responseHeader in (System.Net.Http.Headers.HttpHeaders) responseHeaders)
      {
        foreach (string str in responseHeader.Value)
          stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\": \"{1}\",{2}", (object) responseHeader.Key, (object) str, (object) Environment.NewLine));
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    internal SubStatusCodes GetSubStatus()
    {
      if (!this.substatus.HasValue)
      {
        this.substatus = new SubStatusCodes?(SubStatusCodes.Unknown);
        string s = this.responseHeaders.Get("x-ms-substatus");
        if (!string.IsNullOrEmpty(s))
        {
          uint result = 0;
          if (uint.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            this.substatus = new SubStatusCodes?((SubStatusCodes) result);
        }
      }
      return !this.substatus.HasValue ? SubStatusCodes.Unknown : this.substatus.Value;
    }

    private static string SerializeHTTPResponseHeaders(INameValueCollection responseHeaders)
    {
      if (responseHeaders == null)
        return "null";
      IEnumerable<Tuple<string, string>> tuples = ((IEnumerable<string>) responseHeaders.AllKeys()).SelectMany<string, string, Tuple<string, string>>(new Func<string, IEnumerable<string>>(responseHeaders.GetValues), (Func<string, string, Tuple<string, string>>) ((k, v) => new Tuple<string, string>(k, v)));
      StringBuilder stringBuilder = new StringBuilder("{");
      stringBuilder.Append(Environment.NewLine);
      foreach (Tuple<string, string> tuple in tuples)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\": \"{1}\",{2}", (object) tuple.Item1, (object) tuple.Item2, (object) Environment.NewLine));
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
  }
}
