// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Machine.ServiceProxies.ServiceProxy`1
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;

namespace Microsoft.TeamFoundation.Build.Machine.ServiceProxies
{
  internal abstract class ServiceProxy<T> : IDisposable
  {
    private T m_channel;
    private Guid m_applicationId;
    private Guid m_collectionId;
    private Binding m_binding;
    private string m_url;
    private Uri m_uri;
    private bool m_requireClientCertificates;
    private System.ServiceModel.ChannelFactory<T> m_channelFactory;

    private ServiceProxy(
      Guid applicationId,
      Guid collectionId,
      string url,
      bool requireClientCertificates)
      : this(applicationId, collectionId, url, requireClientCertificates, TimeSpan.FromMinutes(5.0))
    {
    }

    private ServiceProxy(
      Guid applicationId,
      Guid collectionId,
      string url,
      bool requireClientCertificates,
      TimeSpan sendTimeout)
    {
      Binding binding = CommunicationHelpers.CreateBinding(ServiceProxy<T>.DetermineSecurityMode(url), requireClientCertificates);
      binding.SendTimeout = sendTimeout;
      this.m_applicationId = applicationId;
      this.m_collectionId = collectionId;
      this.m_url = url;
      this.m_uri = new Uri(url);
      this.m_binding = binding;
      this.m_requireClientCertificates = requireClientCertificates;
    }

    protected System.ServiceModel.ChannelFactory<T> ChannelFactory
    {
      get
      {
        if (this.m_channelFactory == null)
        {
          this.m_channelFactory = new System.ServiceModel.ChannelFactory<T>(this.m_binding, new EndpointAddress(this.m_url));
          if (ServiceProxy<T>.DetermineSecurityMode(this.m_uri) == SecurityMode.Transport)
          {
            this.m_channelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            if (this.m_requireClientCertificates)
              this.m_channelFactory.Endpoint.Behaviors.Add((IEndpointBehavior) new BuildServiceClientCredentials());
          }
          this.m_channelFactory.Endpoint.Behaviors.Add((IEndpointBehavior) new CollectionIdentifierBehavior(this.m_applicationId, this.m_collectionId));
        }
        return this.m_channelFactory;
      }
    }

    protected T Channel
    {
      get
      {
        if ((object) this.m_channel == null)
          this.m_channel = this.ChannelFactory.CreateChannel();
        return this.m_channel;
      }
    }

    private static SecurityMode DetermineSecurityMode(string url) => ServiceProxy<T>.DetermineSecurityMode(new Uri(url));

    private static SecurityMode DetermineSecurityMode(Uri uri) => !uri.Scheme.Equals(Uri.UriSchemeHttps) ? SecurityMode.None : SecurityMode.Transport;

    protected IAsyncResult BeginAsyncInvoke(
      Func<T, AsyncCallback, object, IAsyncResult> begin,
      Action<T, IAsyncResult> end,
      AsyncCallback callback,
      object state)
    {
      ServiceProxy<T>.AsyncOperation operation = new ServiceProxy<T>.AsyncOperation();
      operation.Begin = begin;
      operation.End = end;
      operation.CertificateIndex = -1;
      operation.InnerCallback = callback;
      operation.InnerState = state;
      return this.BeginOperation((ServiceProxy<T>.AsyncOperationBase) operation);
    }

    protected IAsyncResult BeginAsyncInvoke<TResult>(
      Func<T, AsyncCallback, object, IAsyncResult> begin,
      Func<T, IAsyncResult, TResult> end,
      AsyncCallback callback,
      object state)
    {
      ServiceProxy<T>.AsyncOperationWithResult<TResult> operation = new ServiceProxy<T>.AsyncOperationWithResult<TResult>();
      operation.Begin = begin;
      operation.End = end;
      operation.CertificateIndex = -1;
      operation.InnerCallback = callback;
      operation.InnerState = state;
      return this.BeginOperation((ServiceProxy<T>.AsyncOperationBase) operation);
    }

    protected void EndAsyncInvoke(IAsyncResult result)
    {
      if (!(result is ServiceProxy<T>.AsyncOperationBase asyncOperationBase))
        return;
      asyncOperationBase.AsyncWaitHandle.WaitOne();
      asyncOperationBase.AsyncWaitHandle.Dispose();
      if (asyncOperationBase.InnerException != null)
        throw asyncOperationBase.InnerException;
    }

    protected TResult EndAsyncInvoke<TResult>(IAsyncResult result)
    {
      this.EndAsyncInvoke(result);
      return !(result is ServiceProxy<T>.AsyncOperationWithResult<TResult> operationWithResult) ? default (TResult) : operationWithResult.Result;
    }

    private IAsyncResult BeginOperation(ServiceProxy<T>.AsyncOperationBase operation)
    {
      BuildServiceClientCredentials clientCredentials = this.ChannelFactory.Endpoint.Behaviors.Find<BuildServiceClientCredentials>();
      if (clientCredentials != null)
      {
        operation.UsingTransportSecurity = true;
        X509Certificate2 cachedCertificate = (X509Certificate2) null;
        if (operation.CertificateIndex == -1)
        {
          if (ClientCertificateChoiceCache.TryGet(this.m_uri, out cachedCertificate))
          {
            if (!clientCredentials.ClientCertificates.Contains(cachedCertificate))
            {
              cachedCertificate = (X509Certificate2) null;
            }
            else
            {
              clientCredentials.CurrentCertificate = cachedCertificate;
              operation.UsingCachedCertificate = true;
            }
          }
          if (cachedCertificate == null)
            operation.CertificateIndex = 0;
        }
        if (cachedCertificate == null)
        {
          if (operation.CertificateIndex < clientCredentials.ClientCertificates.Count)
          {
            clientCredentials.CurrentCertificate = clientCredentials.ClientCertificates[operation.CertificateIndex];
          }
          else
          {
            operation.InnerException = (Exception) new MessageSecurityException(this.ServiceCallFailedAuthMessage());
            operation.Complete();
          }
        }
      }
      operation.Channel = this.m_channelFactory.CreateChannel();
      operation.InnerResult = operation.Begin(operation.Channel, new AsyncCallback(this.EndRequest), (object) operation);
      return (IAsyncResult) operation;
    }

    private void EndRequest(IAsyncResult result)
    {
      if (!(result.AsyncState is ServiceProxy<T>.AsyncOperationBase asyncState))
        return;
      try
      {
        asyncState.EndOperation(result);
        if (!asyncState.UsingCachedCertificate && asyncState.UsingTransportSecurity)
        {
          BuildServiceClientCredentials clientCredentials = this.ChannelFactory.Endpoint.Behaviors.Find<BuildServiceClientCredentials>();
          if (clientCredentials != null)
            ClientCertificateChoiceCache.Set(this.m_uri, clientCredentials.CurrentCertificate);
        }
        asyncState.Complete();
      }
      catch (MessageSecurityException ex)
      {
        if (!ServiceProxy<T>.IsAuthenticationFailure(ex.InnerException as FaultException))
        {
          asyncState.InnerException = (Exception) ex;
          asyncState.Complete();
        }
      }
      catch (CryptographicException ex)
      {
      }
      catch (Exception ex)
      {
        asyncState.InnerException = ex;
        asyncState.Complete();
      }
      if (asyncState.IsCompleted)
        return;
      if (!asyncState.UsingTransportSecurity)
      {
        asyncState.Complete();
      }
      else
      {
        if (asyncState.UsingCachedCertificate)
        {
          asyncState.UsingCachedCertificate = false;
          ClientCertificateChoiceCache.Invalidate(this.m_uri);
        }
        ++asyncState.CertificateIndex;
        this.BeginOperation(asyncState);
      }
    }

    protected void Do(Action<T> action) => this.Do<bool>((Func<T, bool>) (channel =>
    {
      action(channel);
      return true;
    }));

    protected TResult Do<TResult>(Func<T, TResult> action)
    {
      BuildServiceClientCredentials clientCredentials = this.ChannelFactory.Endpoint.Behaviors.Find<BuildServiceClientCredentials>();
      if (clientCredentials == null || !this.m_requireClientCertificates)
        return action(this.ChannelFactory.CreateChannel());
      X509Certificate2 cachedCertificate;
      if (ClientCertificateChoiceCache.TryGet(this.m_uri, out cachedCertificate))
      {
        if (!clientCredentials.ClientCertificates.Contains(cachedCertificate))
        {
          cachedCertificate = (X509Certificate2) null;
        }
        else
        {
          clientCredentials.CurrentCertificate = cachedCertificate;
          TResult result;
          if (this.TryDo<TResult>(action, this.ChannelFactory.CreateChannel(), out result))
            return result;
          ClientCertificateChoiceCache.Invalidate(this.m_uri);
        }
      }
      foreach (X509Certificate2 clientCertificate in clientCredentials.ClientCertificates)
      {
        clientCredentials.CurrentCertificate = clientCertificate;
        TResult result;
        if (this.TryDo<TResult>(action, this.ChannelFactory.CreateChannel(), out result))
        {
          ClientCertificateChoiceCache.Set(this.m_uri, clientCertificate);
          return result;
        }
      }
      throw new MessageSecurityException(this.ServiceCallFailedAuthMessage());
    }

    private bool TryDo<TResult>(Func<T, TResult> action, T channel, out TResult result)
    {
      try
      {
        result = action(channel);
        return true;
      }
      catch (MessageSecurityException ex)
      {
        if (!ServiceProxy<T>.IsAuthenticationFailure(ex.InnerException as FaultException))
          throw;
      }
      catch (CryptographicException ex)
      {
      }
      result = default (TResult);
      return false;
    }

    private static bool IsAuthenticationFailure(FaultException exception) => exception != null && exception.Code != null && exception.Code.IsSenderFault && exception.Code.SubCode != null && exception.Code.SubCode.Name.Equals("FailedAuthentication") && exception.Code.SubCode.Namespace.Equals("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");

    public void Dispose()
    {
      CommunicationHelpers.Shutdown((ICommunicationObject) this.m_channelFactory);
      GC.SuppressFinalize((object) this);
    }

    public ServiceProxy(
      IVssRequestContext requestContext,
      string url,
      bool requireClientCertificates)
      : this(ServiceProxy<T>.GetApplicationInstanceId(requestContext), ServiceProxy<T>.GetCollectionInstanceId(requestContext), url, requireClientCertificates)
    {
    }

    public ServiceProxy(
      IVssRequestContext requestContext,
      string url,
      bool requireClientCertificates,
      TimeSpan sendTimeout)
      : this(ServiceProxy<T>.GetApplicationInstanceId(requestContext), ServiceProxy<T>.GetCollectionInstanceId(requestContext), url, requireClientCertificates, sendTimeout)
    {
    }

    private string ServiceCallFailedAuthMessage() => ResourceStrings.ServiceCallFailedAuthentication();

    private static Guid GetApplicationInstanceId(IVssRequestContext requestContext)
    {
      IVssServiceHost serviceHost = requestContext.ServiceHost;
      IVssServiceHost vssServiceHost = serviceHost == null ? requestContext.ServiceHost : serviceHost.OrganizationServiceHost;
      return vssServiceHost == null ? Guid.Empty : vssServiceHost.InstanceId;
    }

    private static Guid GetCollectionInstanceId(IVssRequestContext requestContext)
    {
      IVssServiceHost serviceHost = requestContext.ServiceHost;
      return serviceHost == null ? Guid.Empty : serviceHost.InstanceId;
    }

    private abstract class AsyncOperationBase : IAsyncResult
    {
      private bool m_isCompleted;
      private ManualResetEvent m_waitHandle = new ManualResetEvent(false);

      object IAsyncResult.AsyncState => this.InnerState;

      public WaitHandle AsyncWaitHandle => (WaitHandle) this.m_waitHandle;

      bool IAsyncResult.CompletedSynchronously => false;

      public bool IsCompleted => this.m_isCompleted;

      public T Channel { get; set; }

      public int CertificateIndex { get; set; }

      public AsyncCallback InnerCallback { get; set; }

      public IAsyncResult InnerResult { get; set; }

      public object InnerState { get; set; }

      public Exception InnerException { get; set; }

      public Func<T, AsyncCallback, object, IAsyncResult> Begin { get; set; }

      public bool UsingCachedCertificate { get; set; }

      public bool UsingTransportSecurity { get; set; }

      public void Complete()
      {
        this.m_waitHandle.Set();
        this.m_isCompleted = true;
        if (this.InnerCallback == null)
          return;
        this.InnerCallback((IAsyncResult) this);
      }

      public abstract void EndOperation(IAsyncResult result);
    }

    private class AsyncOperation : ServiceProxy<T>.AsyncOperationBase
    {
      public Action<T, IAsyncResult> End { get; set; }

      public override void EndOperation(IAsyncResult result) => this.End(this.Channel, result);
    }

    private class AsyncOperationWithResult<TResult> : ServiceProxy<T>.AsyncOperationBase
    {
      public TResult Result { get; set; }

      public Func<T, IAsyncResult, TResult> End { get; set; }

      public override void EndOperation(IAsyncResult result) => this.Result = this.End(this.Channel, result);
    }
  }
}
