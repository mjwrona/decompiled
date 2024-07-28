// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.HttpRequestStringRepresentationBuilder
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class HttpRequestStringRepresentationBuilder
  {
    private const string c_newLine = "\n";
    private List<HttpRequestStringRepresentationBuilder.Header> m_headers;
    private StringBuilder m_contentSB;
    private string m_confidentialRequestUri;
    private HttpRequestMessage m_httpRequestMessage;

    public HttpRequestStringRepresentationBuilder(HttpRequestMessage httpRequestMessage)
    {
      ArgumentUtility.CheckForNull<HttpRequestMessage>(httpRequestMessage, nameof (httpRequestMessage));
      this.m_headers = new List<HttpRequestStringRepresentationBuilder.Header>();
      this.m_confidentialRequestUri = (string) null;
      this.m_contentSB = new StringBuilder();
      this.m_httpRequestMessage = httpRequestMessage;
    }

    public HttpRequestStringRepresentationBuilder(
      HttpRequestMessage httpRequestMessage,
      string requestBody)
      : this(httpRequestMessage)
    {
      this.AppendContent(requestBody);
    }

    public string ConfidentialRequestUri
    {
      get => this.m_confidentialRequestUri;
      set => this.m_confidentialRequestUri = value;
    }

    public bool RemoveFirstHeader(string headerName)
    {
      HttpRequestStringRepresentationBuilder.Header header = this.m_headers.FirstOrDefault<HttpRequestStringRepresentationBuilder.Header>((Func<HttpRequestStringRepresentationBuilder.Header, bool>) (h => h.Key == headerName));
      if (header == null)
        return false;
      this.m_headers.Remove(header);
      return true;
    }

    public void AddHeader(string headerName, string headerValue, bool isConfidential) => this.m_headers.Add(new HttpRequestStringRepresentationBuilder.Header()
    {
      IsConfidential = isConfidential,
      Key = headerName,
      Values = (IEnumerable<string>) new List<string>()
      {
        headerValue
      }
    });

    public void AddHeader(string headerName, IEnumerable<string> headerValues, bool isConfidential) => this.m_headers.Add(new HttpRequestStringRepresentationBuilder.Header()
    {
      IsConfidential = isConfidential,
      Key = headerName,
      Values = (IEnumerable<string>) new List<string>(headerValues)
    });

    public void AppendContent(string content) => this.m_contentSB.Append(content);

    public void AppendContentWithFormat(string format, params string[] args) => this.m_contentSB.AppendFormat(format, (object[]) args);

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_MethodTemplate((object) this.m_httpRequestMessage.Method, (object) "\n"));
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_UriTemplate((object) (this.m_confidentialRequestUri ?? this.m_httpRequestMessage.RequestUri.ToString()), (object) "\n"));
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_HttpVersionTemplate((object) this.m_httpRequestMessage.Version, (object) "\n"));
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_HeadersStartTemplate((object) "\n"));
      foreach (HttpRequestStringRepresentationBuilder.Header header in this.m_headers)
        HttpRequestStringRepresentationBuilder.AppendHeaderValues(sb, header);
      if (this.m_httpRequestMessage.Content != null)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) this.m_httpRequestMessage.Content.Headers)
          HttpRequestStringRepresentationBuilder.AppendHeaderValues(sb, new HttpRequestStringRepresentationBuilder.Header()
          {
            IsConfidential = false,
            Key = header.Key,
            Values = header.Value
          });
      }
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_HeadersEndTemplate((object) "\n"));
      if (this.m_contentSB.Length != 0)
        sb.Append(ServiceHooksWebApiResources.HttpActionTask_ContentTemplate((object) "\n", (object) this.m_contentSB));
      return sb.ToString();
    }

    private static void AppendHeaderValues(
      StringBuilder sb,
      HttpRequestStringRepresentationBuilder.Header header)
    {
      foreach (string confidentialValue in header.Values)
        sb.Append(ServiceHooksWebApiResources.HttpActionTask_HeaderKeyValueTemplate((object) header.Key, header.IsConfidential ? (object) SecurityHelper.GetMaskedValue(confidentialValue) : (object) confidentialValue, (object) "\n"));
    }

    private class Header
    {
      public string Key { get; set; }

      public bool IsConfidential { get; set; }

      public IEnumerable<string> Values { get; set; }
    }
  }
}
