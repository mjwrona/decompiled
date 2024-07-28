// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsHttpRequestChannel
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Client.Channels
{
  internal sealed class TfsHttpRequestChannel : ITfsRequestChannel
  {
    private Uri m_uri;
    private Guid m_sessionId;
    private object m_thisLock;
    private string m_serverName;
    private CultureInfo m_cultureInfo;
    private TfsHttpClientState m_state;
    private TfsMessageHeader[] m_headers;
    private TfsRequestSettings m_settings;
    private ITfsRequestListener m_listener;
    private IdentityDescriptor m_impersonate;
    private TfsMessageEncoder m_messageEncoder;
    private VssCredentials m_credentials;
    private List<TfsHttpWebRequest> m_activeRequests;

    public TfsHttpRequestChannel(
      Uri uri,
      Guid sessionId,
      string serverName,
      CultureInfo cultureInfo,
      TfsMessageHeader[] headers,
      VssCredentials credentials,
      IdentityDescriptor impersonate,
      TfsRequestSettings settings,
      TfsMessageEncoder messageEncoder,
      ITfsRequestListener listener)
    {
      this.m_uri = uri;
      this.m_settings = settings;
      this.m_listener = listener;
      this.m_sessionId = sessionId;
      this.m_thisLock = new object();
      this.m_serverName = serverName;
      this.m_credentials = credentials;
      this.m_impersonate = impersonate;
      this.m_messageEncoder = messageEncoder;
      this.m_state = TfsHttpClientState.Opened;
      this.m_headers = headers ?? Array.Empty<TfsMessageHeader>();
      this.m_cultureInfo = cultureInfo ?? CultureInfo.CurrentUICulture;
    }

    public VssCredentials Credentials => this.m_credentials;

    public CultureInfo Culture => this.m_cultureInfo;

    public TfsMessageEncoder Encoder => this.m_messageEncoder;

    public TfsMessageHeader[] Headers => this.m_headers;

    public IdentityDescriptor Impersonate => this.m_impersonate;

    public ITfsRequestListener Listener => this.m_listener;

    public string ServerName => this.m_serverName;

    public Guid SessionId => this.m_sessionId;

    public TfsRequestSettings Settings => this.m_settings;

    public TfsHttpClientState State => this.m_state;

    public Uri Uri => this.m_uri;

    public void Abort()
    {
      TfsHttpWebRequest[] array = (TfsHttpWebRequest[]) null;
      lock (this.m_thisLock)
      {
        if (this.m_state != TfsHttpClientState.Opened)
          return;
        this.m_state = TfsHttpClientState.Aborted;
        if (this.m_activeRequests != null)
        {
          if (this.m_activeRequests.Count > 0)
          {
            array = new TfsHttpWebRequest[this.m_activeRequests.Count];
            this.m_activeRequests.CopyTo(array);
            this.m_activeRequests.Clear();
          }
        }
      }
      if (array == null)
        return;
      for (int index = 0; index < array.Length; ++index)
        array[index].Abort();
    }

    private void ApplyHeaders(TfsMessage message)
    {
      if (message.To == (Uri) null)
        message.To = this.Uri;
      for (int i = 0; i < this.m_headers.Length; i++)
      {
        if (!message.Headers.Any<TfsMessageHeader>((Func<TfsMessageHeader, bool>) (x => x.Name == this.m_headers[i].Name && x.Namespace == this.m_headers[i].Namespace)))
          message.Headers.Add(this.m_headers[i]);
      }
    }

    public TfsMessage Request(TfsMessage message) => this.Request(message, this.Settings.SendTimeout);

    public TfsMessage Request(TfsMessage message, TimeSpan timeout)
    {
      TfsHttpWebRequest tfsHttpWebRequest = (TfsHttpWebRequest) null;
      lock (this.m_thisLock)
      {
        this.ThrowIfAborted();
        if (this.m_activeRequests == null)
          this.m_activeRequests = new List<TfsHttpWebRequest>();
        this.ApplyHeaders(message);
        tfsHttpWebRequest = new TfsHttpWebRequest(this, message, timeout, this.m_listener, (AsyncCallback) null, (object) null);
        this.m_activeRequests.Add(tfsHttpWebRequest);
      }
      try
      {
        return tfsHttpWebRequest.SendRequest();
      }
      finally
      {
        lock (this.m_thisLock)
          this.m_activeRequests.Remove(tfsHttpWebRequest);
      }
    }

    public IAsyncResult BeginRequest(TfsMessage message, AsyncCallback callback, object state) => this.BeginRequest(message, this.Settings.SendTimeout, callback, state);

    public IAsyncResult BeginRequest(
      TfsMessage message,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      TfsHttpWebRequest tfsHttpWebRequest = (TfsHttpWebRequest) null;
      lock (this.m_thisLock)
      {
        this.ThrowIfAborted();
        if (this.m_activeRequests == null)
          this.m_activeRequests = new List<TfsHttpWebRequest>();
        this.ApplyHeaders(message);
        tfsHttpWebRequest = new TfsHttpWebRequest(this, message, timeout, this.m_listener, callback, state);
        this.m_activeRequests.Add(tfsHttpWebRequest);
      }
      tfsHttpWebRequest.Begin();
      return (IAsyncResult) tfsHttpWebRequest;
    }

    public TfsMessage EndRequest(IAsyncResult result)
    {
      TfsHttpWebRequest webRequest;
      Exception exception;
      bool flag = TfsHttpWebRequest.TryEnd(result, out webRequest, out exception);
      lock (this.m_thisLock)
        this.m_activeRequests.Remove(webRequest);
      if (!flag)
        throw exception;
      return webRequest.Response;
    }

    private void ThrowIfAborted()
    {
      if (this.State == TfsHttpClientState.Aborted)
        throw new System.OperationCanceledException(ClientResources.CommandCanceled());
    }
  }
}
