// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsHttpClientBase
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TfsHttpClientBase : IServiceProvider, ITfsRequestListener
  {
    private Uri m_url;
    private object m_thisLock;
    private ITfsRequestChannel m_channel;
    private TfsRequestSettings m_settings;
    private IdentityDescriptor m_impersonate;
    private List<TfsMessageHeader> m_headers;
    private static readonly Guid s_defaultSessionId = Guid.NewGuid();

    protected TfsHttpClientBase(
      Uri uri,
      Guid? sessionId,
      CultureInfo culture,
      VssCredentials credentials)
      : this(uri, sessionId, culture, credentials, (IdentityDescriptor) null)
    {
    }

    protected TfsHttpClientBase(
      Uri uri,
      Guid? sessionId,
      CultureInfo culture,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate)
    {
      TFUtil.CheckForNull((object) credentials, nameof (credentials));
      this.m_thisLock = new object();
      this.m_impersonate = identityToImpersonate;
      this.m_headers = new List<TfsMessageHeader>();
      this.Url = uri;
      this.Culture = culture;
      this.Credentials = credentials;
      this.SessionId = sessionId ?? TfsHttpClientBase.s_defaultSessionId;
      if (credentials.Federated != null && credentials.Federated.TokenStorageUrl == (Uri) null)
        credentials.Federated.TokenStorageUrl = uri;
      if (credentials.Windows == null || !(credentials.Windows.TokenStorageUrl == (Uri) null))
        return;
      credentials.Windows.TokenStorageUrl = uri;
    }

    protected ITfsRequestChannel Channel
    {
      get
      {
        this.EnsureOpened();
        return this.m_channel;
      }
    }

    protected abstract string ComponentName { get; }

    public VssCredentials Credentials { get; private set; }

    public CultureInfo Culture { get; private set; }

    public ReadOnlyCollection<TfsMessageHeader> Headers => this.m_headers.AsReadOnly();

    public string RemoteServerName { get; protected set; }

    public Guid SessionId { get; private set; }

    public TfsRequestSettings Settings
    {
      get
      {
        this.EnsureOpened();
        return this.m_settings;
      }
    }

    public TfsHttpClientState State { get; private set; }

    public Uri Url
    {
      get
      {
        this.EnsureOpened();
        return this.m_url;
      }
      private set => this.m_url = value;
    }

    protected object ThisLock => this.m_thisLock;

    public void Abort()
    {
      ITfsRequestChannel tfsRequestChannel = (ITfsRequestChannel) null;
      lock (this.m_thisLock)
      {
        if (this.State == TfsHttpClientState.Opened)
        {
          tfsRequestChannel = this.m_channel;
          this.State = TfsHttpClientState.Aborted;
        }
      }
      tfsRequestChannel?.Abort();
    }

    public void AddMessageHeader(TfsMessageHeader header)
    {
      ArgumentUtility.CheckForNull<TfsMessageHeader>(header, nameof (header));
      this.ThrowIfAborted();
      lock (this.ThisLock)
      {
        if (this.State != TfsHttpClientState.Created)
          throw new InvalidOperationException(ClientResources.HttpClientAlreadyOpened());
        this.m_headers.Add(header);
      }
    }

    protected virtual TfsRequestSettings ApplyCustomSettings(TfsRequestSettings settings) => settings;

    protected virtual TfsMessageEncoder CreateMessageEncoder() => (TfsMessageEncoder) new TfsSoapMessageEncoder(TfsRequestSettings.RequestEncoding, XmlDictionaryReaderQuotas.Max, false);

    public void Open() => this.Open(TimeSpan.FromSeconds(30.0));

    public void Open(TimeSpan timeout)
    {
      this.ThrowIfAborted();
      if (this.State == TfsHttpClientState.Opened)
        return;
      bool flag1 = true;
      bool flag2 = false;
      bool flag3 = false;
      try
      {
        flag3 = Monitor.TryEnter(this.ThisLock, timeout);
        if (!flag3)
          throw new TimeoutException(ClientResources.HttpClientOpenTimeout((object) timeout));
        if (this.State == TfsHttpClientState.Opened)
        {
          flag2 = true;
        }
        else
        {
          this.State = TfsHttpClientState.Opening;
          if (this.Url == (Uri) null)
            this.Url = this.GetServiceLocation();
          this.m_settings = this.ApplyCustomSettings(TfsRequestSettings.GetSettings(this.ComponentName));
          this.m_channel = this.OnCreateChannel((ITfsRequestChannel) new TfsHttpRequestChannel(this.Url, this.SessionId, this.RemoteServerName, this.Culture, this.m_headers.ToArray(), this.Credentials, this.m_impersonate, this.Settings, this.CreateMessageEncoder(), (ITfsRequestListener) this));
          flag1 = false;
        }
      }
      finally
      {
        if (flag3)
        {
          if (!flag2)
            this.State = flag1 ? TfsHttpClientState.Created : TfsHttpClientState.Opened;
          Monitor.Exit(this.ThisLock);
        }
      }
    }

    protected virtual ITfsRequestChannel OnCreateChannel(ITfsRequestChannel innerChannel) => innerChannel;

    protected virtual Exception ConvertException(SoapException e) => (Exception) e;

    public virtual object GetService(Type serviceType) => (object) null;

    public T GetService<T>() where T : class => (T) this.GetService(typeof (T));

    protected virtual Uri GetServiceLocation() => this.Url;

    protected object Invoke(TfsClientOperation operation, object[] parameters) => this.Invoke(operation, parameters, out object[] _);

    protected object Invoke(
      TfsClientOperation operation,
      object[] parameters,
      out object[] outputs)
    {
      return this.Invoke(operation, parameters, this.Settings.SendTimeout, out outputs);
    }

    protected object Invoke(
      TfsClientOperation operation,
      object[] parameters,
      TimeSpan timeout,
      out object[] outputs)
    {
      this.ThrowIfAborted();
      this.EnsureOpened();
      TfsMessage message = this.Channel.Request(TfsMessage.CreateMessage(operation.SoapAction, operation.CreateBodyWriter(parameters)), timeout);
      return this.HandleReply(operation, message, out outputs);
    }

    protected IAsyncResult BeginInvoke(
      TfsClientOperation operation,
      object[] parameters,
      AsyncCallback callback,
      object state)
    {
      return this.BeginInvoke(operation, parameters, this.Settings.SendTimeout, callback, state);
    }

    protected IAsyncResult BeginInvoke(
      TfsClientOperation operation,
      object[] parameters,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      this.ThrowIfAborted();
      this.EnsureOpened();
      return (IAsyncResult) new TfsHttpClientBase.InvokeOperation(this, operation, parameters, timeout, callback, state);
    }

    protected object EndInvoke(IAsyncResult result) => this.EndInvoke(out object[] _, result);

    protected object EndInvoke(out object[] outputs, IAsyncResult result) => TfsHttpClientBase.InvokeOperation.End(out outputs, result);

    void ITfsRequestListener.AfterReceiveReply(
      long requestId,
      string methodName,
      HttpWebResponse response)
    {
      this.OnAfterReceiveReply(requestId, methodName, response);
    }

    void ITfsRequestListener.BeforeSendRequest(
      long requestId,
      string methodName,
      HttpWebRequest request)
    {
      this.OnBeforeSendRequest(requestId, methodName, request);
    }

    long ITfsRequestListener.BeginRequest() => this.OnBeginRequest();

    void ITfsRequestListener.EndRequest(long requestId, Exception exception) => this.OnEndRequest(requestId, exception);

    protected virtual void OnAfterReceiveReply(
      long requestId,
      string methodName,
      HttpWebResponse response)
    {
      TfsConnection.OnWebServiceCallEnd((TfsConnection) null, requestId, methodName, this.ComponentName, response);
    }

    protected virtual void OnBeforeSendRequest(
      long requestId,
      string methodName,
      HttpWebRequest request)
    {
      TfsConnection.OnWebServiceCallBegin((TfsConnection) null, requestId, methodName, this.ComponentName, request);
    }

    protected virtual long OnBeginRequest() => TfsConnection.OnBeginWebRequest();

    protected virtual void OnEndRequest(long requestId, Exception exception)
    {
    }

    private object HandleReply(
      TfsClientOperation operation,
      TfsMessage message,
      out object[] outputs)
    {
      try
      {
        operation.OutputHeaders = new ReadOnlyCollection<TfsMessageHeader>((IList<TfsMessageHeader>) message.Headers);
        if (message.IsFault)
        {
          Exception e = message.CreateException();
          e = e is SoapException ? this.ConvertException((SoapException) e) : throw e;
        }
        else
        {
          if (!operation.HasOutputs)
          {
            outputs = Array.Empty<object>();
            return (object) null;
          }
          object obj = operation.InitializeOutputs(out outputs);
          if (message.IsEmpty)
            return obj;
          using (XmlDictionaryReader bodyReader = message.GetBodyReader())
          {
            int num = bodyReader.IsEmptyElement ? 1 : 0;
            bodyReader.Read();
            if (num == 0)
            {
              if (!string.IsNullOrEmpty(operation.ResultName) && string.Equals(bodyReader.LocalName, operation.ResultName, StringComparison.Ordinal) && string.Equals(bodyReader.NamespaceURI, operation.SoapNamespace, StringComparison.Ordinal))
                obj = operation.ReadResult((IServiceProvider) this, (XmlReader) bodyReader);
              while (bodyReader.NodeType == XmlNodeType.Element)
                operation.ReadOutput((IServiceProvider) this, (XmlReader) bodyReader, outputs);
            }
          }
          return obj;
        }
      }
      finally
      {
        message?.Close();
      }
    }

    private void ThrowIfAborted()
    {
      if (this.State == TfsHttpClientState.Aborted)
        throw new System.OperationCanceledException(ClientResources.CommandCanceled());
    }

    private void EnsureOpened()
    {
      if (this.State == TfsHttpClientState.Opened)
        return;
      lock (this.m_thisLock)
      {
        if (this.State != TfsHttpClientState.Created)
          return;
        this.Open();
      }
    }

    private sealed class InvokeOperation : Microsoft.TeamFoundation.Framework.Common.AsyncOperation
    {
      private object m_result;
      private object[] m_outputs;
      private TfsHttpClientBase m_client;
      private TfsClientOperation m_operation;

      public InvokeOperation(
        TfsHttpClientBase client,
        TfsClientOperation operation,
        object[] parameters,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.m_client = client;
        this.m_operation = operation;
        IAsyncResult result = this.m_client.Channel.BeginRequest(TfsMessage.CreateMessage(operation.SoapAction, operation.CreateBodyWriter(parameters)), timeout, new AsyncCallback(TfsHttpClientBase.InvokeOperation.EndRequest), (object) this);
        if (!result.CompletedSynchronously)
          return;
        this.CompleteRequest(result);
      }

      private static void EndRequest(IAsyncResult result)
      {
        if (result.CompletedSynchronously)
          return;
        TfsHttpClientBase.InvokeOperation asyncState = (TfsHttpClientBase.InvokeOperation) result.AsyncState;
        bool flag = true;
        Exception exception = (Exception) null;
        try
        {
          flag = asyncState.CompleteRequest(result);
        }
        catch (Exception ex)
        {
          exception = ex;
        }
        if (!flag)
          return;
        asyncState.Complete(false, exception);
      }

      private bool CompleteRequest(IAsyncResult result)
      {
        this.m_result = this.m_client.HandleReply(this.m_operation, this.m_client.Channel.EndRequest(result), out this.m_outputs);
        return true;
      }

      public static object End(out object[] outputs, IAsyncResult result)
      {
        TfsHttpClientBase.InvokeOperation invokeOperation = Microsoft.TeamFoundation.Framework.Common.AsyncOperation.End<TfsHttpClientBase.InvokeOperation>(result);
        outputs = invokeOperation.m_outputs;
        return invokeOperation.m_result;
      }
    }
  }
}
