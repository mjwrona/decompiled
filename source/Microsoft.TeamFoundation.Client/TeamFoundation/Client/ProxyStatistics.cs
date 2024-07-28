// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProxyStatistics
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Client
{
  public sealed class ProxyStatistics
  {
    private ProxyStatisticsWebService m_service;

    public ProxyStatistics(Uri url)
      : this((TfsConnection) null, url)
    {
    }

    public ProxyStatistics(TfsConnection tfs, Uri url)
    {
      ArgumentUtility.CheckForNull<Uri>(url, nameof (url));
      if (tfs == null)
        this.m_service = new ProxyStatisticsWebService((VssCredentials) new VssClientCredentials(), url);
      else
        this.m_service = new ProxyStatisticsWebService(tfs.ClientCredentials, url);
    }

    public int Timeout
    {
      get => !this.m_service.Timeout.HasValue ? (int) this.m_service.Settings.SendTimeout.TotalMilliseconds : this.m_service.Timeout.Value;
      set => this.m_service.Timeout = new int?(value);
    }

    public void Abort() => this.m_service.Abort();

    public ProxyStatisticsInfo[] QueryProxyStatistics() => this.m_service.QueryProxyStatistics();

    public IAsyncResult BeginQueryProxyStatistics(AsyncCallback callback, object state) => this.m_service.BeginQueryProxyStatistics(callback, state);

    public ProxyStatisticsInfo[] EndQueryProxyStatistics(IAsyncResult result) => this.m_service.EndQueryProxyStatistics(result);
  }
}
