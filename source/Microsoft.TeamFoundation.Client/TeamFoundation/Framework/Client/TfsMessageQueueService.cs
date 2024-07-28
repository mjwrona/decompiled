// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TfsMessageQueueService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class TfsMessageQueueService : ITfsConnectionObject, IDisposable
  {
    private object m_thisLock;
    private TfsConnection m_connection;
    private Uri m_queueServiceLocation;
    private TfsMessageQueueService.PollTimeouts m_pollTimeouts;
    private TfsSoapMessageEncoder m_encoder;
    private TfsMessageQueueVersion m_version;
    private Dictionary<Uri, TfsMessageQueue> m_activeQueues;

    internal TfsMessageQueueService()
    {
      this.m_thisLock = new object();
      this.m_activeQueues = new Dictionary<Uri, TfsMessageQueue>();
    }

    internal VssCredentials Credentials => this.m_connection.ClientCredentials;

    public TimeSpan ReceiveTimeout => this.m_pollTimeouts.ReceiveTimeout;

    public TimeSpan ReconnectTimeout => this.m_pollTimeouts.ReconnectTimeout;

    public TimeSpan SleepTimeout => this.m_pollTimeouts.SleepTimeout;

    public TfsConnection Connection => this.m_connection;

    [Obsolete("This property has been deprecated. See TfsMessageQueueService.Connection instead.")]
    public TfsTeamProjectCollection ProjectCollection => this.m_connection as TfsTeamProjectCollection;

    internal TfsMessageQueueVersion Version => this.m_version;

    internal Uri QueueServiceLocation => this.m_queueServiceLocation;

    public void Close()
    {
      TfsMessageQueue[] array = (TfsMessageQueue[]) null;
      lock (this.m_thisLock)
      {
        if (this.m_activeQueues != null && this.m_activeQueues.Count > 0)
        {
          array = new TfsMessageQueue[this.m_activeQueues.Count];
          this.m_activeQueues.Values.CopyTo(array, 0);
        }
        this.m_activeQueues.Clear();
      }
      if (array == null)
        return;
      foreach (TfsMessageQueue tfsMessageQueue in array)
        tfsMessageQueue.Abort();
    }

    void IDisposable.Dispose() => this.Close();

    public TfsMessageQueue CreateQueue(Uri queueId) => this.CreateQueue(queueId, 1);

    public TfsMessageQueue CreateQueue(Uri queueId, int maxPendingCount) => this.CreateQueue(queueId, maxPendingCount, TimeSpan.FromSeconds(5.0));

    public TfsMessageQueue CreateQueue(
      Uri queueId,
      int maxPendingCount,
      TimeSpan maxAcknowledgementDelay)
    {
      return this.CreateQueue(queueId, maxPendingCount, maxAcknowledgementDelay, (Func<SoapException, Exception>) null);
    }

    public TfsMessageQueue CreateQueue(
      Uri queueId,
      int maxPendingCount,
      TimeSpan maxAcknowledgementDelay,
      Func<SoapException, Exception> convertException)
    {
      TfsMessageQueue queue;
      lock (this.m_thisLock)
      {
        queue = new TfsMessageQueue(queueId, maxPendingCount, maxAcknowledgementDelay, this, convertException);
        this.m_activeQueues.Add(queueId, queue);
      }
      return queue;
    }

    internal void Remove(TfsMessageQueue queue)
    {
      lock (this.m_thisLock)
        this.m_activeQueues.Remove(queue.Id);
    }

    void ITfsConnectionObject.Initialize(TfsConnection server)
    {
      ArgumentUtility.CheckForNull<TfsConnection>(server, nameof (server));
      this.m_connection = server;
      this.EnsureInitialized();
    }

    internal ITfsRequestChannel CreatePollChannel()
    {
      this.EnsureInitialized();
      if (this.m_queueServiceLocation == (Uri) null)
        throw new InvalidOperationException();
      TfsRequestSettings settings = TfsRequestSettings.GetSettings("MessageQueue").Clone();
      settings.SendTimeout = TimeSpan.FromMinutes(10.0);
      return (ITfsRequestChannel) new TfsHttpRequestChannel(this.m_queueServiceLocation, this.m_connection.SessionId, this.m_connection.Name, this.m_connection.Culture, (TfsMessageHeader[]) null, this.m_connection.ClientCredentials, this.m_connection.IdentityToImpersonate, settings, (TfsMessageEncoder) this.m_encoder, (ITfsRequestListener) null);
    }

    private void EnsureInitialized()
    {
      if (this.m_pollTimeouts != null)
        return;
      lock (this.m_thisLock)
      {
        if (this.m_pollTimeouts != null)
          return;
        TfsMessageQueueVersion messageQueueVersion = TfsMessageQueueVersion.V2;
        ILocationService service = this.m_connection.GetService<ILocationService>();
        string uriString = service.LocationForCurrentConnection("MessageQueueService2", FrameworkServiceIdentifiers.MessageQueue2);
        if (string.IsNullOrEmpty(uriString))
        {
          messageQueueVersion = TfsMessageQueueVersion.V1;
          uriString = service.LocationForCurrentConnection("MessageQueueService", FrameworkServiceIdentifiers.MessageQueue);
        }
        if (string.IsNullOrEmpty(uriString))
          throw new NotSupportedException(ClientResources.TfsmqNotSupportedOnProjectCollection((object) this.m_connection.Uri.AbsoluteUri));
        this.m_version = messageQueueVersion;
        this.m_queueServiceLocation = new Uri(uriString, UriKind.Absolute);
        this.m_pollTimeouts = new TfsMessageQueueService.PollTimeouts();
        this.m_pollTimeouts.ReceiveTimeout = TimeSpan.FromMinutes(20.0);
        this.m_pollTimeouts.ReconnectTimeout = TimeSpan.FromSeconds(10.0);
        this.m_pollTimeouts.SendTimeout = TimeSpan.FromMinutes(5.0);
        this.m_pollTimeouts.SleepTimeout = TimeSpan.FromSeconds(10.0);
        this.m_encoder = new TfsSoapMessageEncoder(TfsRequestSettings.RequestEncoding, XmlDictionaryReaderQuotas.Max, true);
      }
    }

    private sealed class PollTimeouts
    {
      public TimeSpan ReceiveTimeout { get; set; }

      public TimeSpan ReconnectTimeout { get; set; }

      public TimeSpan SendTimeout { get; set; }

      public TimeSpan SleepTimeout { get; set; }
    }
  }
}
