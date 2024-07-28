// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssHttpRetryOptions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Common
{
  public class VssHttpRetryOptions
  {
    private int m_isReadOnly;
    private int m_maxRetries;
    private TimeSpan m_minBackoff;
    private TimeSpan m_maxBackoff;
    private TimeSpan m_backoffCoefficient;
    private ICollection<HttpStatusCode> m_retryableStatusCodes;
    private ICollection<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter> m_retryFilters;
    private static TimeSpan s_minBackoff = TimeSpan.FromSeconds(10.0);
    private static TimeSpan s_maxBackoff = TimeSpan.FromMinutes(10.0);
    private static TimeSpan s_backoffCoefficient = TimeSpan.FromSeconds(1.0);
    private static Lazy<VssHttpRetryOptions> s_defaultOptions = new Lazy<VssHttpRetryOptions>((Func<VssHttpRetryOptions>) (() => new VssHttpRetryOptions().MakeReadonly()));
    private static VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter s_hostShutdownFilter = (VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter) (response => response.Headers.Contains("X-VSS-HostOfflineError"));

    public VssHttpRetryOptions()
      : this((IEnumerable<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter>) new VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter[1]
      {
        VssHttpRetryOptions.s_hostShutdownFilter
      })
    {
    }

    public VssHttpRetryOptions(
      IEnumerable<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter> filters)
    {
      this.BackoffCoefficient = VssHttpRetryOptions.s_backoffCoefficient;
      this.MinBackoff = VssHttpRetryOptions.s_minBackoff;
      this.MaxBackoff = VssHttpRetryOptions.s_maxBackoff;
      this.MaxRetries = 5;
      this.RetryableStatusCodes = (ICollection<HttpStatusCode>) new HashSet<HttpStatusCode>()
      {
        HttpStatusCode.BadGateway,
        HttpStatusCode.GatewayTimeout,
        HttpStatusCode.ServiceUnavailable
      };
      this.m_retryFilters = (ICollection<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter>) new HashSet<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter>(filters);
    }

    public static VssHttpRetryOptions Default => VssHttpRetryOptions.s_defaultOptions.Value;

    public TimeSpan BackoffCoefficient
    {
      get => this.m_backoffCoefficient;
      set
      {
        this.ThrowIfReadonly();
        this.m_backoffCoefficient = value;
      }
    }

    public TimeSpan MinBackoff
    {
      get => this.m_minBackoff;
      set
      {
        this.ThrowIfReadonly();
        this.m_minBackoff = value;
      }
    }

    public TimeSpan MaxBackoff
    {
      get => this.m_maxBackoff;
      set
      {
        this.ThrowIfReadonly();
        this.m_maxBackoff = value;
      }
    }

    public int MaxRetries
    {
      get => this.m_maxRetries;
      set
      {
        this.ThrowIfReadonly();
        this.m_maxRetries = value;
      }
    }

    public ICollection<HttpStatusCode> RetryableStatusCodes
    {
      get => this.m_retryableStatusCodes;
      private set
      {
        this.ThrowIfReadonly();
        this.m_retryableStatusCodes = value;
      }
    }

    public bool IsRetryableResponse(HttpResponseMessage response)
    {
      if (!this.m_retryableStatusCodes.Contains(response.StatusCode))
        return false;
      foreach (VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter retryFilter in (IEnumerable<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter>) this.m_retryFilters)
      {
        if (retryFilter(response))
          return false;
      }
      return true;
    }

    public VssHttpRetryOptions MakeReadonly()
    {
      if (Interlocked.CompareExchange(ref this.m_isReadOnly, 1, 0) == 0)
      {
        this.m_retryableStatusCodes = (ICollection<HttpStatusCode>) new ReadOnlyCollection<HttpStatusCode>((IList<HttpStatusCode>) this.m_retryableStatusCodes.ToList<HttpStatusCode>());
        this.m_retryFilters = (ICollection<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter>) new ReadOnlyCollection<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter>((IList<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter>) this.m_retryFilters.ToList<VssHttpRetryOptions.VssHttpRetryableStatusCodeFilter>());
      }
      return this;
    }

    private void ThrowIfReadonly()
    {
      if (this.m_isReadOnly > 0)
        throw new InvalidOperationException();
    }

    public delegate bool VssHttpRetryableStatusCodeFilter(HttpResponseMessage response);
  }
}
