// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssHttpRequestSettings
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Common
{
  public class VssHttpRequestSettings
  {
    public const string PropertyName = "MS.VS.RequestSettings";
    public const string HttpCompletionOptionPropertyName = "MS.VS.HttpCompletionOption";
    public const string LightweightHeader = "lightweight";
    public const string ExcludeUrlsHeader = "excludeUrls";
    private int m_maxContentBufferSize;
    private ICollection<CultureInfo> m_acceptLanguages = (ICollection<CultureInfo>) new List<CultureInfo>();
    private static Lazy<Encoding> s_encoding = new Lazy<Encoding>((Func<Encoding>) (() => (Encoding) new UTF8Encoding(false)), LazyThreadSafetyMode.PublicationOnly);
    private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromSeconds(100.0);
    private const int c_defaultMaxRetry = 3;
    private const int c_maxAllowedContentBufferSize = 1073741824;
    private const int c_defaultContentBufferSize = 536870912;

    public VssHttpRequestSettings()
      : this(Guid.NewGuid())
    {
    }

    public VssHttpRequestSettings(Guid sessionId)
    {
      this.AllowAutoRedirect = false;
      this.CompressionEnabled = true;
      this.ExpectContinue = true;
      this.BypassProxyOnLocal = true;
      this.MaxContentBufferSize = 536870912;
      this.SendTimeout = VssHttpRequestSettings.s_defaultTimeout;
      if (!string.IsNullOrEmpty(CultureInfo.CurrentUICulture.Name))
        this.AcceptLanguages.Add(CultureInfo.CurrentUICulture);
      this.SessionId = sessionId;
      this.SuppressFedAuthRedirects = true;
      this.ClientCertificateManager = (IVssClientCertificateManager) null;
      this.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback) null;
      if (!CultureInfo.CurrentCulture.Equals((object) CultureInfo.CurrentUICulture) && !string.IsNullOrEmpty(CultureInfo.CurrentCulture.Name))
        this.AcceptLanguages.Add(CultureInfo.CurrentCulture);
      this.MaxRetryRequest = 3;
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public VssHttpRequestSettings(Guid sessionId, Guid e2eId)
      : this(sessionId)
    {
    }

    protected VssHttpRequestSettings(VssHttpRequestSettings copy)
    {
      this.AllowAutoRedirect = copy.AllowAutoRedirect;
      this.CompressionEnabled = copy.CompressionEnabled;
      this.ExpectContinue = copy.ExpectContinue;
      this.BypassProxyOnLocal = copy.BypassProxyOnLocal;
      this.MaxContentBufferSize = copy.MaxContentBufferSize;
      this.SendTimeout = copy.SendTimeout;
      this.m_acceptLanguages = (ICollection<CultureInfo>) new List<CultureInfo>((IEnumerable<CultureInfo>) copy.AcceptLanguages);
      this.SessionId = copy.SessionId;
      this.AgentId = copy.AgentId;
      this.SuppressFedAuthRedirects = copy.SuppressFedAuthRedirects;
      this.UserAgent = new List<ProductInfoHeaderValue>((IEnumerable<ProductInfoHeaderValue>) copy.UserAgent);
      this.OperationName = copy.OperationName;
      this.ClientCertificateManager = copy.ClientCertificateManager;
      this.ServerCertificateValidationCallback = copy.ServerCertificateValidationCallback;
      this.MaxRetryRequest = copy.MaxRetryRequest;
      this.ReadConsistencyLevel = copy.ReadConsistencyLevel;
    }

    public bool AllowAutoRedirect { get; set; }

    [DefaultValue(true)]
    public bool CompressionEnabled { get; set; }

    [DefaultValue(true)]
    public bool ExpectContinue { get; set; }

    public bool BypassProxyOnLocal { get; set; }

    [DefaultValue(536870912)]
    public int MaxContentBufferSize
    {
      get => this.m_maxContentBufferSize;
      set
      {
        ArgumentUtility.CheckForOutOfRange(value, nameof (value), 0, 1073741824);
        this.m_maxContentBufferSize = value;
      }
    }

    public TimeSpan SendTimeout { get; set; }

    [DefaultValue(true)]
    public bool SuppressFedAuthRedirects { get; set; }

    public List<ProductInfoHeaderValue> UserAgent { get; set; }

    public ICollection<CultureInfo> AcceptLanguages => this.m_acceptLanguages;

    public Guid SessionId { get; set; }

    public Guid E2EId { get; set; }

    public string AgentId { get; set; }

    public string OperationName { get; set; }

    public IVssClientCertificateManager ClientCertificateManager { get; set; }

    public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }

    [DefaultValue(3)]
    public int MaxRetryRequest { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public VssReadConsistencyLevel? ReadConsistencyLevel { get; set; }

    protected internal virtual bool IsHostLocal(string hostName) => false;

    protected internal virtual bool ApplyTo(HttpRequestMessage request)
    {
      if (request.Properties.ContainsKey("MS.VS.RequestSettings"))
        return false;
      request.Properties.Add("MS.VS.RequestSettings", (object) this);
      if (this.AcceptLanguages != null && this.AcceptLanguages.Count > 0)
      {
        foreach (CultureInfo cultureInfo in this.AcceptLanguages.Where<CultureInfo>((Func<CultureInfo, bool>) (a => !string.IsNullOrEmpty(a.Name))))
          request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(cultureInfo.Name));
      }
      if (this.UserAgent != null)
      {
        foreach (ProductInfoHeaderValue productInfoHeaderValue in this.UserAgent)
        {
          if (!request.Headers.UserAgent.Contains(productInfoHeaderValue))
            request.Headers.UserAgent.Add(productInfoHeaderValue);
        }
      }
      if (this.SuppressFedAuthRedirects)
        request.Headers.Add("X-TFS-FedAuthRedirect", "Suppress");
      if (!request.Headers.Contains("X-TFS-Session"))
      {
        if (!string.IsNullOrEmpty(this.OperationName))
          request.Headers.Add("X-TFS-Session", this.SessionId.ToString("D") + ", " + this.OperationName);
        else
          request.Headers.Add("X-TFS-Session", this.SessionId.ToString("D"));
      }
      if (!string.IsNullOrEmpty(this.AgentId))
        request.Headers.Add("X-VSS-Agent", this.AgentId);
      if (this.ReadConsistencyLevel.HasValue)
        request.Headers.Add("X-VSS-ReadConsistencyLevel", this.ReadConsistencyLevel.ToString());
      return true;
    }

    public static Encoding Encoding => VssHttpRequestSettings.s_encoding.Value;
  }
}
