// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NamespaceManager
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Common.Parallel;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Messaging.Configuration;
using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class NamespaceManager
  {
    public const string ProtocolVersion = "2016-07";
    internal readonly IEnumerable<Uri> addresses;
    private readonly NamespaceManagerSettings settings;

    public NamespaceManager(string address)
      : this(new Uri(address), (TokenProvider) null)
    {
    }

    public NamespaceManager(IEnumerable<string> addresses)
      : this(addresses, (TokenProvider) null)
    {
    }

    public NamespaceManager(Uri address)
      : this(address, (TokenProvider) null)
    {
    }

    public NamespaceManager(IEnumerable<Uri> addresses)
      : this(addresses, (TokenProvider) null)
    {
    }

    public NamespaceManager(string address, TokenProvider tokenProvider)
      : this(new Uri(address), tokenProvider)
    {
    }

    public NamespaceManager(IEnumerable<string> addresses, TokenProvider tokenProvider)
      : this(MessagingUtilities.GetUriList(addresses), tokenProvider)
    {
    }

    public NamespaceManager(Uri address, TokenProvider tokenProvider)
    {
      MessagingUtilities.ThrowIfNullAddressOrPathExists(address, nameof (address));
      this.addresses = (IEnumerable<Uri>) new List<Uri>()
      {
        address
      };
      this.settings = new NamespaceManagerSettings()
      {
        TokenProvider = tokenProvider
      };
    }

    public NamespaceManager(IEnumerable<Uri> addresses, TokenProvider tokenProvider)
    {
      MessagingUtilities.ThrowIfNullAddressesOrPathExists(addresses, nameof (addresses));
      this.addresses = (IEnumerable<Uri>) addresses.ToList<Uri>();
      this.settings = new NamespaceManagerSettings()
      {
        TokenProvider = tokenProvider
      };
    }

    public NamespaceManager(string address, NamespaceManagerSettings settings)
      : this(new Uri(address), settings)
    {
    }

    public NamespaceManager(IEnumerable<string> addresses, NamespaceManagerSettings settings)
      : this(MessagingUtilities.GetUriList(addresses), settings)
    {
    }

    public NamespaceManager(Uri address, NamespaceManagerSettings settings)
      : this((IEnumerable<Uri>) new List<Uri>() { address }, settings)
    {
    }

    public NamespaceManager(IEnumerable<Uri> addresses, NamespaceManagerSettings settings)
    {
      MessagingUtilities.ThrowIfNullAddressesOrPathExists(addresses, nameof (addresses));
      if (settings == null)
        throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentNull(nameof (settings));
      TimeoutHelper.ThrowIfNonPositiveArgument(settings.OperationTimeout);
      this.addresses = (IEnumerable<Uri>) addresses.ToList<Uri>();
      this.settings = settings;
    }

    public Uri Address => this.addresses.First<Uri>();

    public NamespaceManagerSettings Settings => this.settings;

    public static NamespaceManager Create() => new KeyValueConfigurationManager().CreateNamespaceManager();

    public static NamespaceManager CreateFromConnectionString(string connectionString) => new KeyValueConfigurationManager(connectionString).CreateNamespaceManager();

    public string GetVersionInfo() => this.EndGetVersionInfo(this.BeginGetVersionInfo((AsyncCallback) null, (object) null));

    public Task<string> GetVersionInfoAsync() => TaskHelpers.CreateTask<string>(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetVersionInfo), new Func<IAsyncResult, string>(this.EndGetVersionInfo));

    public IAsyncResult BeginGetVersionInfo(AsyncCallback callback, object state)
    {
      NamespaceManager.GetVersionInfoAsyncResult versionInfo = new NamespaceManager.GetVersionInfoAsyncResult(this, callback, state);
      versionInfo.Start();
      return (IAsyncResult) versionInfo;
    }

    public string EndGetVersionInfo(IAsyncResult result) => AsyncResult<NamespaceManager.GetVersionInfoAsyncResult>.End(result).Version;

    internal NotificationHubDescription CreateNotificationHub(string path) => this.EndCreateNotificationHub(this.BeginCreateNotificationHub(new NotificationHubDescription(path), (AsyncCallback) null, (object) null));

    public NotificationHubDescription CreateNotificationHub(NotificationHubDescription description) => this.EndCreateNotificationHub(this.BeginCreateNotificationHub(description, (AsyncCallback) null, (object) null));

    public Task<NotificationHubDescription> CreateNotificationHubAsync(
      NotificationHubDescription description)
    {
      return TaskHelpers.CreateTask<NotificationHubDescription>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginCreateNotificationHub(description, c, s)), new Func<IAsyncResult, NotificationHubDescription>(this.EndCreateNotificationHub));
    }

    private IAsyncResult BeginCreateNotificationHub(
      NotificationHubDescription description,
      AsyncCallback callback,
      object state)
    {
      NamespaceManager.CreateOrUpdateNotificationHubAsyncResult notificationHub = new NamespaceManager.CreateOrUpdateNotificationHubAsyncResult(this, description, false, callback, state);
      notificationHub.Start();
      return (IAsyncResult) notificationHub;
    }

    private NotificationHubDescription EndCreateNotificationHub(IAsyncResult result) => AsyncResult<NamespaceManager.CreateOrUpdateNotificationHubAsyncResult>.End(result).NotificationHub;

    public NotificationHubDescription UpdateNotificationHub(NotificationHubDescription description) => this.EndUpdateNotificationHub(this.BeginUpdateNotificationHub(description, (AsyncCallback) null, (object) null));

    public Task<NotificationHubDescription> UpdateNotificationHubAsync(
      NotificationHubDescription description)
    {
      return TaskHelpers.CreateTask<NotificationHubDescription>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginUpdateNotificationHub(description, c, s)), new Func<IAsyncResult, NotificationHubDescription>(this.EndUpdateNotificationHub));
    }

    private IAsyncResult BeginUpdateNotificationHub(
      NotificationHubDescription description,
      AsyncCallback callback,
      object state)
    {
      NamespaceManager.CreateOrUpdateNotificationHubAsyncResult notificationHubAsyncResult = new NamespaceManager.CreateOrUpdateNotificationHubAsyncResult(this, description, true, callback, state);
      notificationHubAsyncResult.Start();
      return (IAsyncResult) notificationHubAsyncResult;
    }

    private NotificationHubDescription EndUpdateNotificationHub(IAsyncResult result) => AsyncResult<NamespaceManager.CreateOrUpdateNotificationHubAsyncResult>.End(result).NotificationHub;

    internal void DeleteRegistration(
      string notificationHubPath,
      string registrationId,
      string etag)
    {
      this.EndDeleteRegistration(this.BeginDeleteRegistration(notificationHubPath, registrationId, etag, (AsyncCallback) null, (object) null));
    }

    internal Task DeleteRegistrationAsync(
      string notificationHubPath,
      string registrationId,
      string etag)
    {
      return TaskHelpers.CreateTask((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginDeleteRegistration(notificationHubPath, registrationId, etag, c, s)), new Action<IAsyncResult>(this.EndDeleteRegistration));
    }

    internal IAsyncResult BeginDeleteRegistration(
      string notificationHubPath,
      string registrationId,
      string etag,
      AsyncCallback callback,
      object state)
    {
      Dictionary<string, string> additionalHeaders = new Dictionary<string, string>();
      if (string.IsNullOrEmpty(etag))
        additionalHeaders.Add("If-Match", "*");
      else
        additionalHeaders.Add("If-Match", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
        {
          (object) etag
        }));
      return ServiceBusResourceOperations.BeginDelete(TrackingContext.GetInstance(Guid.NewGuid(), notificationHubPath), new IResourceDescription[1]
      {
        (IResourceDescription) new GenericDescription("Registrations")
      }, new string[2]
      {
        notificationHubPath,
        registrationId
      }, this.addresses, additionalHeaders, this.settings, this.settings.InternalOperationTimeout, callback, state);
    }

    public void EndDeleteRegistration(IAsyncResult result) => ServiceBusResourceOperations.EndDelete(result);

    public void DeleteNotificationHub(string path) => this.EndDeleteNotificationHub(this.BeginDeleteNotificationHub(path, (AsyncCallback) null, (object) null));

    public Task DeleteNotificationHubAsync(string path) => TaskHelpers.CreateTask((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginDeleteNotificationHub(path, c, s)), new Action<IAsyncResult>(this.EndDeleteNotificationHub));

    private IAsyncResult BeginDeleteNotificationHub(
      string path,
      AsyncCallback callback,
      object state)
    {
      return ServiceBusResourceOperations.BeginDelete(TrackingContext.GetInstance(Guid.NewGuid(), path), (IResourceDescription[]) null, new string[1]
      {
        path
      }, this.addresses, this.settings, this.settings.InternalOperationTimeout, callback, state);
    }

    private void EndDeleteNotificationHub(IAsyncResult result) => ServiceBusResourceOperations.EndDelete(result);

    public Task<NotificationHubJob> SubmitNotificationHubJobAsync(
      NotificationHubJob job,
      string notificationHubPath)
    {
      NamespaceManager.CheckValidEntityName(notificationHubPath, 290, false, nameof (notificationHubPath));
      if (job == null)
        throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentNull(nameof (job));
      if (job.OutputContainerUri == (Uri) null)
        throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentNull("OutputContainerUri");
      return ServiceBusResourceOperations.CreateAsync<NotificationHubJob>(job, new IResourceDescription[1]
      {
        (IResourceDescription) new NotificationHubJob()
      }, new string[2]{ notificationHubPath, string.Empty }, this);
    }

    public Task<NotificationHubJob> GetNotificationHubJobAsync(
      string jobId,
      string notificationHubPath)
    {
      NamespaceManager.CheckValidEntityName(jobId, 128, false, nameof (jobId));
      NamespaceManager.CheckValidEntityName(notificationHubPath, 290, false, nameof (notificationHubPath));
      return ServiceBusResourceOperations.GetAsync<NotificationHubJob>(new IResourceDescription[1]
      {
        (IResourceDescription) new NotificationHubJob()
      }, new string[2]{ notificationHubPath, jobId }, this);
    }

    public Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(
      string notificationHubPath)
    {
      NamespaceManager.CheckValidEntityName(notificationHubPath, 290, false, nameof (notificationHubPath));
      TaskCompletionSource<IEnumerable<NotificationHubJob>> getNotificationHubJobsSource = new TaskCompletionSource<IEnumerable<NotificationHubJob>>();
      ServiceBusResourceOperations.GetAllAsync(new IResourceDescription[1]
      {
        (IResourceDescription) new NotificationHubJob()
      }, new string[1]{ notificationHubPath }, this).ContinueWith((Action<Task<SyndicationFeed>>) (getAllTask =>
      {
        if (getAllTask.IsFaulted)
          getNotificationHubJobsSource.TrySetException(getAllTask.Exception.InnerException);
        else if (getAllTask.IsCanceled)
          getNotificationHubJobsSource.TrySetCanceled();
        else
          getNotificationHubJobsSource.TrySetResult(new NamespaceManager.EntityDescriptionSyndicationFeed<NotificationHubJob>(getAllTask.Result, (Action<NotificationHubJob, string>) ((description, path) => { })).Entities);
      }));
      return getNotificationHubJobsSource.Task;
    }

    public NotificationHubDescription GetNotificationHub(string path) => this.EndGetNotificationHub(this.BeginGetNotificationHub(path, (AsyncCallback) null, (object) null));

    public Task<NotificationHubDescription> GetNotificationHubAsync(string path) => TaskHelpers.CreateTask<NotificationHubDescription>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetNotificationHub(path, c, s)), new Func<IAsyncResult, NotificationHubDescription>(this.EndGetNotificationHub));

    private IAsyncResult BeginGetNotificationHub(string path, AsyncCallback callback, object state)
    {
      NamespaceManager.CheckValidEntityName(path, 260, nameof (path));
      return ServiceBusResourceOperations.BeginGet<NotificationHubDescription>(TrackingContext.GetInstance(Guid.NewGuid(), path), (IResourceDescription[]) null, new string[1]
      {
        path
      }, this.addresses, this.settings, this.settings.InternalOperationTimeout, callback, state);
    }

    private NotificationHubDescription EndGetNotificationHub(IAsyncResult result)
    {
      string[] resourceNames;
      NotificationHubDescription notificationHub = ServiceBusResourceOperations.EndGet<NotificationHubDescription>(result, out resourceNames);
      notificationHub.Path = resourceNames[0];
      notificationHub.IsReadOnly = false;
      return notificationHub;
    }

    public bool NotificationHubExists(string path) => this.EndNotificationHubExists(this.BeginNotificationHubExists(path, (AsyncCallback) null, (object) null));

    public Task<bool> NotificationHubExistsAsync(string path) => TaskHelpers.CreateTask<bool>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginNotificationHubExists(path, c, s)), new Func<IAsyncResult, bool>(this.EndNotificationHubExists));

    private IAsyncResult BeginNotificationHubExists(
      string path,
      AsyncCallback callback,
      object state)
    {
      NamespaceManager.CheckValidEntityName(path, 260, nameof (path));
      return ServiceBusResourceOperations.BeginGet<NotificationHubDescription>(TrackingContext.GetInstance(Guid.NewGuid(), path), (IResourceDescription[]) null, new string[1]
      {
        path
      }, this.addresses, this.settings, this.settings.InternalOperationTimeout, callback, state);
    }

    private bool EndNotificationHubExists(IAsyncResult result)
    {
      try
      {
        return ServiceBusResourceOperations.EndGet<NotificationHubDescription>(result) != null;
      }
      catch (MessagingEntityNotFoundException ex)
      {
        return false;
      }
    }

    public IEnumerable<NotificationHubDescription> GetNotificationHubs() => this.EndGetNotificationHubs(this.BeginGetNotificationHubs((AsyncCallback) null, (object) null));

    public Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync() => TaskHelpers.CreateTask<IEnumerable<NotificationHubDescription>>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetNotificationHubs(c, s)), new Func<IAsyncResult, IEnumerable<NotificationHubDescription>>(this.EndGetNotificationHubs));

    private IAsyncResult BeginGetNotificationHubs(AsyncCallback callback, object state) => ServiceBusResourceOperations.BeginGetAll(TrackingContext.GetInstance(Guid.NewGuid()), new IResourceDescription[1]
    {
      (IResourceDescription) new NotificationHubDescription()
    }, (string[]) null, this.addresses, this.settings, callback, state);

    private IEnumerable<NotificationHubDescription> EndGetNotificationHubs(IAsyncResult result) => new NamespaceManager.NotificationHubSyndicationFeed(ServiceBusResourceOperations.EndGetAll(result)).NotificationHubs;

    internal Task<string> CreateRegistrationIdAsync(string notificationHubPath) => Task.Factory.FromAsync<string[], NamespaceManager, string>(new Func<string[], NamespaceManager, AsyncCallback, object, IAsyncResult>(ServiceBusResourceOperations.BeginCreateRegistrationId), new Func<IAsyncResult, string>(ServiceBusResourceOperations.EndCreateRegistrationId), new string[1]
    {
      notificationHubPath
    }, this, (object) null);

    internal Task<string> CreateOrUpdateInstallationAsync(
      string jsonPayload,
      string installationId,
      string methog,
      string hubPath)
    {
      return TaskHelpers.CreateTask<string>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginCreateOrUpdateInstallation(jsonPayload, installationId, methog, hubPath, c, s)), new Func<IAsyncResult, string>(this.EndCreateOrUpdateInstallation));
    }

    private IAsyncResult BeginCreateOrUpdateInstallation(
      string jsonPayload,
      string installationId,
      string methog,
      string hubPath,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new NamespaceManager.CreateOrUpdateInstallationAsyncResult(this, jsonPayload, installationId, methog, hubPath, callback, state);
    }

    private string EndCreateOrUpdateInstallation(IAsyncResult result) => NamespaceManager.CreateOrUpdateInstallationAsyncResult.End(result).Result;

    internal Task<string> GetInstallationAsync(string installationId, string hubPath) => TaskHelpers.CreateTask<string>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetInstallation(installationId, hubPath, c, s)), new Func<IAsyncResult, string>(this.EndGetInstallation));

    private IAsyncResult BeginGetInstallation(
      string installationId,
      string hubPath,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new NamespaceManager.GetInstallationAsyncResult(this, installationId, hubPath, callback, state);
    }

    private string EndGetInstallation(IAsyncResult result) => NamespaceManager.GetInstallationAsyncResult.End(result).Result;

    internal Task DeleteInstallationAsync(string installationId, string notificationHubPath) => TaskHelpers.CreateTask((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginDeleteInstallation(notificationHubPath, installationId, c, s)), new Action<IAsyncResult>(this.EndDeleteInstallation));

    private IAsyncResult BeginDeleteInstallation(
      string notificationHubPath,
      string installationId,
      AsyncCallback callback,
      object state)
    {
      return ServiceBusResourceOperations.BeginDeleteInstallation(TrackingContext.GetInstance(Guid.NewGuid(), notificationHubPath), installationId, notificationHubPath, this.addresses, this.settings, this.settings.InternalOperationTimeout, callback, state);
    }

    public void EndDeleteInstallation(IAsyncResult result) => ServiceBusResourceOperations.EndDeleteInstallation(result);

    internal TRegistrationDescription CreateRegistration<TRegistrationDescription>(
      TRegistrationDescription registrationDescription)
      where TRegistrationDescription : RegistrationDescription
    {
      return this.EndCreateRegistration<TRegistrationDescription>(this.BeginCreateRegistration<TRegistrationDescription>(registrationDescription, (AsyncCallback) null, (object) null));
    }

    internal Task<TRegistrationDescription> CreateRegistrationAsync<TRegistrationDescription>(
      TRegistrationDescription registrationDescription)
      where TRegistrationDescription : RegistrationDescription
    {
      return TaskHelpers.CreateTask<TRegistrationDescription>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginCreateRegistration<TRegistrationDescription>(registrationDescription, c, s)), new Func<IAsyncResult, TRegistrationDescription>(this.EndCreateRegistration<TRegistrationDescription>));
    }

    private IAsyncResult BeginCreateRegistration<TRegistrationDescription>(
      TRegistrationDescription registrationDescription,
      AsyncCallback callback,
      object state)
      where TRegistrationDescription : RegistrationDescription
    {
      RegistrationSDKHelper.ValidateRegistration((RegistrationDescription) registrationDescription);
      return (IAsyncResult) new NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>(this, registrationDescription, false, callback, state);
    }

    private TRegistrationDescription EndCreateRegistration<TRegistrationDescription>(
      IAsyncResult result)
      where TRegistrationDescription : RegistrationDescription
    {
      return NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.End(result).Registration;
    }

    internal TRegistrationDescription UpdateRegistration<TRegistrationDescription>(
      TRegistrationDescription registrationDescription)
      where TRegistrationDescription : RegistrationDescription
    {
      return this.EndUpdateRegistration<TRegistrationDescription>(this.BeginUpdateRegistration<TRegistrationDescription>(registrationDescription, (AsyncCallback) null, (object) null));
    }

    internal Task<TRegistrationDescription> UpdateRegistrationAsync<TRegistrationDescription>(
      TRegistrationDescription registrationDescription)
      where TRegistrationDescription : RegistrationDescription
    {
      return TaskHelpers.CreateTask<TRegistrationDescription>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginUpdateRegistration<TRegistrationDescription>(registrationDescription, c, s)), new Func<IAsyncResult, TRegistrationDescription>(this.EndUpdateRegistration<TRegistrationDescription>));
    }

    private IAsyncResult BeginUpdateRegistration<TRegistrationDescription>(
      TRegistrationDescription registrationDescription,
      AsyncCallback callback,
      object state)
      where TRegistrationDescription : RegistrationDescription
    {
      RegistrationSDKHelper.ValidateRegistration((RegistrationDescription) registrationDescription);
      return (IAsyncResult) new NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>(this, registrationDescription, true, callback, state);
    }

    private TRegistrationDescription EndUpdateRegistration<TRegistrationDescription>(
      IAsyncResult result)
      where TRegistrationDescription : RegistrationDescription
    {
      return NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.End(result).Registration;
    }

    internal CollectionQueryResult<RegistrationDescription> GetRegistrationsByChannel(
      string pnsHandle,
      string notificationHubPath,
      string continuationToken,
      int top)
    {
      return this.EndGetRegistrationsByChannel(this.BeginGetRegistrationsByChannel(pnsHandle, notificationHubPath, continuationToken, top, (AsyncCallback) null, (object) null));
    }

    internal Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(
      string pnsHandle,
      string notificationHubPath,
      string continuationToken,
      int top)
    {
      if (string.IsNullOrWhiteSpace(pnsHandle))
        throw new ArgumentNullException(nameof (pnsHandle));
      if (string.IsNullOrWhiteSpace(notificationHubPath))
        throw new ArgumentNullException(nameof (notificationHubPath));
      return TaskHelpers.CreateTask<CollectionQueryResult<RegistrationDescription>>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetRegistrationsByChannel(pnsHandle, notificationHubPath, continuationToken, top, c, s)), new Func<IAsyncResult, CollectionQueryResult<RegistrationDescription>>(this.EndGetRegistrationsByChannel));
    }

    private IAsyncResult BeginGetRegistrationsByChannel(
      string pnsHandle,
      string notificationHubPath,
      string continuationToken,
      int top,
      AsyncCallback callback,
      object state)
    {
      return ServiceBusResourceOperations.BeginGetAll(TrackingContext.GetInstance(Guid.NewGuid(), notificationHubPath), "ChannelUri eq '" + pnsHandle + "'", new IResourceDescription[1]
      {
        (IResourceDescription) new GenericDescription("Registrations")
      }, new string[1]{ notificationHubPath }, this.addresses, this.settings, continuationToken, top, callback, state);
    }

    private CollectionQueryResult<RegistrationDescription> EndGetRegistrationsByChannel(
      IAsyncResult result)
    {
      try
      {
        string continuationToken;
        return new CollectionQueryResult<RegistrationDescription>(new NamespaceManager.RegistrationSyndicationFeed(ServiceBusResourceOperations.EndGetAll(result, out continuationToken)).Registrations, continuationToken);
      }
      catch (MessagingEntityNotFoundException ex)
      {
        return new CollectionQueryResult<RegistrationDescription>((IEnumerable<RegistrationDescription>) null, string.Empty);
      }
    }

    internal CollectionQueryResult<RegistrationDescription> GetAllRegistrations(
      string notificationHubPath,
      string continuationToken,
      int top)
    {
      return this.EndGetAllRegistrations(this.BeginGetAllRegistrations(notificationHubPath, continuationToken, top, (AsyncCallback) null, (object) null));
    }

    internal Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(
      string notificationHubPath,
      string continuationToken,
      int top)
    {
      if (string.IsNullOrWhiteSpace(notificationHubPath))
        throw new ArgumentNullException(nameof (notificationHubPath));
      return TaskHelpers.CreateTask<CollectionQueryResult<RegistrationDescription>>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetAllRegistrations(notificationHubPath, continuationToken, top, c, s)), new Func<IAsyncResult, CollectionQueryResult<RegistrationDescription>>(this.EndGetAllRegistrations));
    }

    private IAsyncResult BeginGetAllRegistrations(
      string notificationHubPath,
      string continuationToken,
      int top,
      AsyncCallback callback,
      object state)
    {
      return ServiceBusResourceOperations.BeginGetAll(TrackingContext.GetInstance(Guid.NewGuid(), notificationHubPath), string.Empty, new IResourceDescription[1]
      {
        (IResourceDescription) new GenericDescription("Registrations")
      }, new string[1]{ notificationHubPath }, this.addresses, this.settings, continuationToken, top, callback, state);
    }

    private CollectionQueryResult<RegistrationDescription> EndGetAllRegistrations(
      IAsyncResult result)
    {
      try
      {
        string continuationToken;
        return new CollectionQueryResult<RegistrationDescription>(new NamespaceManager.RegistrationSyndicationFeed(ServiceBusResourceOperations.EndGetAll(result, out continuationToken)).Registrations, continuationToken);
      }
      catch (MessagingEntityNotFoundException ex)
      {
        return new CollectionQueryResult<RegistrationDescription>((IEnumerable<RegistrationDescription>) null, string.Empty);
      }
    }

    internal CollectionQueryResult<RegistrationDescription> GetRegistrationsByTag(
      string notificationHubPath,
      string tag,
      string continuationToken,
      int top)
    {
      return this.EndGetRegistrationsByTag(this.BeginGetRegistrationsByTag(notificationHubPath, tag, continuationToken, top, (AsyncCallback) null, (object) null));
    }

    internal Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(
      string notificationHubPath,
      string tag,
      string continuationToken,
      int top)
    {
      return TaskHelpers.CreateTask<CollectionQueryResult<RegistrationDescription>>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetRegistrationsByTag(notificationHubPath, tag, continuationToken, top, c, s)), new Func<IAsyncResult, CollectionQueryResult<RegistrationDescription>>(this.EndGetRegistrationsByTag));
    }

    private IAsyncResult BeginGetRegistrationsByTag(
      string notificationHubPath,
      string tag,
      string continuationToken,
      int top,
      AsyncCallback callback,
      object state)
    {
      if (string.IsNullOrWhiteSpace(tag))
        throw new ArgumentNullException(nameof (tag));
      return ServiceBusResourceOperations.BeginGetAll(TrackingContext.GetInstance(Guid.NewGuid(), notificationHubPath), string.Empty, new IResourceDescription[2]
      {
        (IResourceDescription) new GenericDescription("Tags"),
        (IResourceDescription) new GenericDescription("Registrations")
      }, new string[2]{ notificationHubPath, tag }, this.addresses, this.settings, continuationToken, top, callback, state);
    }

    private CollectionQueryResult<RegistrationDescription> EndGetRegistrationsByTag(
      IAsyncResult result)
    {
      try
      {
        string continuationToken;
        return new CollectionQueryResult<RegistrationDescription>(new NamespaceManager.RegistrationSyndicationFeed(ServiceBusResourceOperations.EndGetAll(result, out continuationToken)).Registrations, continuationToken);
      }
      catch (MessagingEntityNotFoundException ex)
      {
        return new CollectionQueryResult<RegistrationDescription>((IEnumerable<RegistrationDescription>) null, string.Empty);
      }
    }

    internal TRegistrationDescription GetRegistration<TRegistrationDescription>(
      string registrationId,
      string notificationHubPath)
      where TRegistrationDescription : RegistrationDescription
    {
      return this.EndGetRegistration<TRegistrationDescription>(this.BeginGetRegistration<TRegistrationDescription>(registrationId, notificationHubPath, (AsyncCallback) null, (object) null));
    }

    internal Task<TRegistrationDescription> GetRegistrationAsync<TRegistrationDescription>(
      string registrationId,
      string notificationHubPath)
      where TRegistrationDescription : RegistrationDescription
    {
      return TaskHelpers.CreateTask<TRegistrationDescription>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginGetRegistration<TRegistrationDescription>(registrationId, notificationHubPath, c, s)), new Func<IAsyncResult, TRegistrationDescription>(this.EndGetRegistration<TRegistrationDescription>));
    }

    private IAsyncResult BeginGetRegistration<TRegistrationDescription>(
      string registrationId,
      string notificationHubPath,
      AsyncCallback callback,
      object state)
      where TRegistrationDescription : RegistrationDescription
    {
      return ServiceBusResourceOperations.BeginGet<RegistrationDescription>(TrackingContext.GetInstance(Guid.NewGuid(), notificationHubPath), new IResourceDescription[1]
      {
        (IResourceDescription) new GenericDescription("Registrations")
      }, new string[2]
      {
        notificationHubPath,
        registrationId
      }, this.addresses, this.settings, this.settings.InternalOperationTimeout, callback, state);
    }

    private TRegistrationDescription EndGetRegistration<TRegistrationDescription>(
      IAsyncResult result)
      where TRegistrationDescription : RegistrationDescription
    {
      string[] resourceNames;
      TRegistrationDescription registration = (TRegistrationDescription) ServiceBusResourceOperations.EndGet<RegistrationDescription>(result, out resourceNames);
      registration.NotificationHubPath = resourceNames[0];
      registration.IsReadOnly = false;
      return registration;
    }

    private static void CheckValidEntityName(
      string entityName,
      int maxEntityNameLength,
      string paramName)
    {
      NamespaceManager.CheckValidEntityName(entityName, maxEntityNameLength, true, paramName);
    }

    private static void CheckValidEntityName(
      string entityName,
      int maxEntityNameLength,
      bool allowSeparator,
      string paramName)
    {
      string actualValue = !string.IsNullOrWhiteSpace(entityName) ? entityName.Replace("\\", "/") : throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentNullOrEmpty(paramName);
      if (actualValue.Length > maxEntityNameLength)
        throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentOutOfRange(paramName, (object) actualValue, SRClient.EntityNameLengthExceedsLimit((object) actualValue, (object) maxEntityNameLength));
      if (actualValue.StartsWith("/", StringComparison.OrdinalIgnoreCase) || actualValue.EndsWith("/", StringComparison.Ordinal))
        throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.Argument(paramName, SRClient.InvalidEntityNameFormatWithSlash((object) actualValue));
      if (!allowSeparator && actualValue.Contains("/"))
        throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.Argument(paramName, SRClient.InvalidCharacterInEntityName((object) "/", (object) actualValue));
      MessagingUtilities.CheckUriSchemeKey(entityName, paramName);
    }

    private sealed class GetVersionInfoAsyncResult : 
      IteratorAsyncResult<NamespaceManager.GetVersionInfoAsyncResult>
    {
      private readonly TrackingContext trackingContext;
      private readonly NamespaceManager manager;

      public GetVersionInfoAsyncResult(
        NamespaceManager manager,
        AsyncCallback callback,
        object state)
        : base(manager.settings.InternalOperationTimeout, callback, state)
      {
        this.trackingContext = TrackingContext.GetInstance(Guid.NewGuid());
        this.manager = manager;
        this.Version = string.Empty;
      }

      public string Version { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<NamespaceManager.GetVersionInfoAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        yield return this.CallAsync((IteratorAsyncResult<NamespaceManager.GetVersionInfoAsyncResult>.BeginCall) ((thisPtr, t, c, s) => ServiceBusResourceOperations.BeginGetInformation(thisPtr.trackingContext, thisPtr.manager.addresses, thisPtr.manager.settings, t, c, s)), (IteratorAsyncResult<NamespaceManager.GetVersionInfoAsyncResult>.EndCall) ((thisPtr, r) =>
        {
          IDictionary<string, string> information = ServiceBusResourceOperations.EndGetInformation(r);
          thisPtr.Version = information.ContainsKey("MaxProtocolVersion") ? information["MaxProtocolVersion"] : string.Empty;
        }), IteratorAsyncResult<NamespaceManager.GetVersionInfoAsyncResult>.ExceptionPolicy.Transfer);
      }
    }

    private sealed class CreateOrUpdateNotificationHubAsyncResult : 
      IteratorAsyncResult<NamespaceManager.CreateOrUpdateNotificationHubAsyncResult>
    {
      private readonly TrackingContext trackingContext;
      private readonly NamespaceManager manager;
      private readonly bool isUpdate;

      public CreateOrUpdateNotificationHubAsyncResult(
        NamespaceManager manager,
        NotificationHubDescription description,
        bool isUpdate,
        AsyncCallback callback,
        object state)
        : base(manager.settings.InternalOperationTimeout, callback, state)
      {
        if (description == null)
          throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentNull(nameof (description));
        NamespaceManager.CheckValidEntityName(description.Path, 260, "description.Path");
        this.trackingContext = TrackingContext.GetInstance(Guid.NewGuid(), description.Path);
        this.manager = manager;
        this.NotificationHub = description;
        this.isUpdate = isUpdate;
      }

      public NotificationHubDescription NotificationHub { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<NamespaceManager.CreateOrUpdateNotificationHubAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        yield return this.CallAsync((IteratorAsyncResult<NamespaceManager.CreateOrUpdateNotificationHubAsyncResult>.BeginCall) ((thisPtr, t, c, s) => ServiceBusResourceOperations.BeginCreateOrUpdate<NotificationHubDescription>(thisPtr.trackingContext, thisPtr.NotificationHub, (IResourceDescription[]) null, new string[1]
        {
          thisPtr.NotificationHub.Path
        }, thisPtr.manager.addresses, t, (thisPtr.NotificationHub.IsAnonymousAccessible ? 1 : 0) != 0, (thisPtr.isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, thisPtr.manager.settings, c, s)), (IteratorAsyncResult<NamespaceManager.CreateOrUpdateNotificationHubAsyncResult>.EndCall) ((thisPtr, r) => this.CreateDescription(thisPtr, r)), IteratorAsyncResult<NamespaceManager.CreateOrUpdateNotificationHubAsyncResult>.ExceptionPolicy.Transfer);
      }

      private void CreateDescription(
        NamespaceManager.CreateOrUpdateNotificationHubAsyncResult thisPtr,
        IAsyncResult r)
      {
        string path = thisPtr.NotificationHub.Path;
        thisPtr.NotificationHub = ServiceBusResourceOperations.EndCreate<NotificationHubDescription>(r);
        thisPtr.NotificationHub.Path = path;
        thisPtr.NotificationHub.IsReadOnly = false;
      }
    }

    private sealed class GetInstallationAsyncResult : AsyncResult
    {
      private readonly TrackingContext trackingContext;
      private readonly NamespaceManager manager;
      private readonly string installationId;
      private readonly string hubPath;
      private TimeoutHelper timeoutHelper;

      public GetInstallationAsyncResult(
        NamespaceManager manager,
        string installationId,
        string hubPath,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.installationId = installationId;
        this.hubPath = hubPath;
        this.trackingContext = TrackingContext.GetInstance(Guid.NewGuid(), hubPath);
        this.manager = manager;
        this.timeoutHelper = new TimeoutHelper(this.manager.settings.InternalOperationTimeout);
        if (!this.SyncContinue(this.BeginCreateOrUpdateOperation()))
          return;
        this.Complete(true);
      }

      public string Result { get; private set; }

      public static NamespaceManager.GetInstallationAsyncResult End(IAsyncResult result) => AsyncResult.End<NamespaceManager.GetInstallationAsyncResult>(result);

      private IAsyncResult BeginCreateOrUpdateOperation() => ServiceBusResourceOperations.BeginGetInstallation(this.trackingContext, this.installationId, this.hubPath, this.manager.addresses, this.timeoutHelper.RemainingTime(), this.manager.settings, this.PrepareAsyncCompletion(new AsyncResult.AsyncCompletion(NamespaceManager.GetInstallationAsyncResult.OnDone)), (object) this);

      private static bool OnDone(IAsyncResult result)
      {
        ((NamespaceManager.GetInstallationAsyncResult) result.AsyncState).Result = ServiceBusResourceOperations.EndGetInstallation(result);
        return true;
      }
    }

    private sealed class CreateOrUpdateInstallationAsyncResult : AsyncResult
    {
      private readonly TrackingContext trackingContext;
      private readonly NamespaceManager manager;
      private readonly string jsonPayload;
      private readonly string method;
      private readonly string installationId;
      private readonly string hubPath;
      private TimeoutHelper timeoutHelper;

      public CreateOrUpdateInstallationAsyncResult(
        NamespaceManager manager,
        string jsonPayload,
        string installationId,
        string method,
        string hubPath,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.jsonPayload = jsonPayload;
        this.installationId = installationId;
        this.hubPath = hubPath;
        this.method = method;
        this.trackingContext = TrackingContext.GetInstance(Guid.NewGuid(), hubPath);
        this.manager = manager;
        this.timeoutHelper = new TimeoutHelper(this.manager.settings.InternalOperationTimeout);
        if (!this.SyncContinue(this.BeginCreateOrUpdateOperation()))
          return;
        this.Complete(true);
      }

      public string Result { get; private set; }

      public static NamespaceManager.CreateOrUpdateInstallationAsyncResult End(IAsyncResult result) => AsyncResult.End<NamespaceManager.CreateOrUpdateInstallationAsyncResult>(result);

      private IAsyncResult BeginCreateOrUpdateOperation() => ServiceBusResourceOperations.BeginCreateOrUpdateInstallation(this.trackingContext, this.jsonPayload, this.installationId, this.method, this.hubPath, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(new AsyncResult.AsyncCompletion(NamespaceManager.CreateOrUpdateInstallationAsyncResult.OnDone)), (object) this);

      private static bool OnDone(IAsyncResult result)
      {
        ((NamespaceManager.CreateOrUpdateInstallationAsyncResult) result.AsyncState).Result = ServiceBusResourceOperations.EndCreateOrUpdateInstallation(result);
        return true;
      }
    }

    private sealed class CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription> : 
      AsyncResult
      where TRegistrationDescription : RegistrationDescription
    {
      private static readonly AsyncResult.AsyncCompletion onCreateRegistration = new AsyncResult.AsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.OnCreateRegistration);
      private readonly TrackingContext trackingContext;
      private readonly NamespaceManager manager;
      private TimeoutHelper timeoutHelper;

      public CreateOrUpdateRegistrationAsyncResult(
        NamespaceManager manager,
        TRegistrationDescription description,
        bool isUpdate,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.trackingContext = (object) description != null ? TrackingContext.GetInstance(Guid.NewGuid(), description.NotificationHubPath) : throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentNull(nameof (description));
        this.manager = manager;
        this.Registration = description;
        this.timeoutHelper = new TimeoutHelper(this.manager.settings.InternalOperationTimeout);
        string[] strArray;
        if (!isUpdate)
          strArray = new string[2]
          {
            this.Registration.NotificationHubPath,
            string.Empty
          };
        else
          strArray = new string[2]
          {
            this.Registration.NotificationHubPath,
            this.Registration.RegistrationId
          };
        string[] resourceNames = strArray;
        this.Registration.RegistrationId = (string) null;
        this.Registration.ExpirationTime = new DateTime?();
        if (!this.SyncContinue(this.BeginCreateOrUpdateOperation(resourceNames, isUpdate)))
          return;
        this.Complete(true);
      }

      public TRegistrationDescription Registration { get; private set; }

      public static NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription> End(
        IAsyncResult result)
      {
        return AsyncResult.End<NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>>(result);
      }

      private IAsyncResult BeginCreateOrUpdateOperation(string[] resourceNames, bool isUpdate)
      {
        if (this.Registration.GetType().Name == typeof (AppleTemplateRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<AppleTemplateRegistrationDescription>(this.trackingContext, (object) this.Registration as AppleTemplateRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (AppleRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<AppleRegistrationDescription>(this.trackingContext, (object) this.Registration as AppleRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (WindowsTemplateRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<WindowsTemplateRegistrationDescription>(this.trackingContext, (object) this.Registration as WindowsTemplateRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (WindowsRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<WindowsRegistrationDescription>(this.trackingContext, (object) this.Registration as WindowsRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (GcmTemplateRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<GcmTemplateRegistrationDescription>(this.trackingContext, (object) this.Registration as GcmTemplateRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (GcmRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<GcmRegistrationDescription>(this.trackingContext, (object) this.Registration as GcmRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (MpnsTemplateRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<MpnsTemplateRegistrationDescription>(this.trackingContext, (object) this.Registration as MpnsTemplateRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (MpnsRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<MpnsRegistrationDescription>(this.trackingContext, (object) this.Registration as MpnsRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (EmailRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<EmailRegistrationDescription>(this.trackingContext, (object) this.Registration as EmailRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (AdmRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<AdmRegistrationDescription>(this.trackingContext, (object) this.Registration as AdmRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (AdmTemplateRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<AdmTemplateRegistrationDescription>(this.trackingContext, (object) this.Registration as AdmTemplateRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (NokiaXTemplateRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<NokiaXTemplateRegistrationDescription>(this.trackingContext, (object) this.Registration as NokiaXTemplateRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (NokiaXRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<NokiaXRegistrationDescription>(this.trackingContext, (object) this.Registration as NokiaXRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (this.Registration.GetType().Name == typeof (BaiduTemplateRegistrationDescription).Name)
          return ServiceBusResourceOperations.BeginCreateOrUpdate<BaiduTemplateRegistrationDescription>(this.trackingContext, (object) this.Registration as BaiduTemplateRegistrationDescription, new IResourceDescription[1]
          {
            (IResourceDescription) this.Registration
          }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
        if (!(this.Registration.GetType().Name == typeof (BaiduRegistrationDescription).Name))
          throw new InvalidOperationException("Unknow RegistrationDescription type");
        return ServiceBusResourceOperations.BeginCreateOrUpdate<BaiduRegistrationDescription>(this.trackingContext, (object) this.Registration as BaiduRegistrationDescription, new IResourceDescription[1]
        {
          (IResourceDescription) this.Registration
        }, resourceNames, this.manager.addresses, this.timeoutHelper.RemainingTime(), false, (isUpdate ? 1 : 0) != 0, (IDictionary<string, string>) null, this.manager.settings, this.PrepareAsyncCompletion(NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>.onCreateRegistration), (object) this);
      }

      private static bool OnCreateRegistration(IAsyncResult result)
      {
        NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription> asyncState = (NamespaceManager.CreateOrUpdateRegistrationAsyncResult<TRegistrationDescription>) result.AsyncState;
        string notificationHubPath = asyncState.Registration.NotificationHubPath;
        if (asyncState.Registration.GetType().Name == typeof (AppleTemplateRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<AppleTemplateRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (AppleRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<AppleRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (WindowsTemplateRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<WindowsTemplateRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (WindowsRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<WindowsRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (GcmTemplateRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<GcmTemplateRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (GcmRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<GcmRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (NokiaXTemplateRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<NokiaXTemplateRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (NokiaXRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<NokiaXRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (BaiduRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<BaiduRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (BaiduTemplateRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<BaiduTemplateRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (MpnsTemplateRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<MpnsTemplateRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (MpnsRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<MpnsRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (EmailRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<EmailRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (AdmRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<AdmRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (AdmTemplateRegistrationDescription).Name)
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<AdmTemplateRegistrationDescription>(result) as TRegistrationDescription;
        else if (asyncState.Registration.GetType().Name == typeof (BaiduRegistrationDescription).Name)
        {
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<BaiduRegistrationDescription>(result) as TRegistrationDescription;
        }
        else
        {
          if (!(asyncState.Registration.GetType().Name == typeof (BaiduTemplateRegistrationDescription).Name))
            throw new InvalidOperationException("Unknow RegistrationDescription type");
          asyncState.Registration = ServiceBusResourceOperations.EndCreate<BaiduTemplateRegistrationDescription>(result) as TRegistrationDescription;
        }
        asyncState.Registration.NotificationHubPath = notificationHubPath;
        asyncState.Registration.IsReadOnly = false;
        return true;
      }
    }

    private sealed class GetAllAsyncResult : IteratorAsyncResult<NamespaceManager.GetAllAsyncResult>
    {
      private readonly TrackingContext trackingContext;
      private readonly NamespaceManagerSettings settings;
      private readonly string filter;

      public GetAllAsyncResult(
        TrackingContext trackingContext,
        IResourceDescription[] descriptions,
        string[] resourceNames,
        IEnumerable<Uri> addresses,
        NamespaceManagerSettings settings,
        AsyncCallback callback,
        object state)
        : base(TimeSpan.MaxValue, callback, state)
      {
        this.trackingContext = trackingContext;
        this.settings = settings;
        this.Descriptions = descriptions;
        this.ResourceNames = resourceNames;
        this.Addresses = addresses;
        this.Start();
      }

      public GetAllAsyncResult(
        TrackingContext trackingContext,
        IResourceDescription[] descriptions,
        string[] resourceNames,
        string filter,
        IEnumerable<Uri> addresses,
        NamespaceManagerSettings settings,
        AsyncCallback callback,
        object state)
        : base(TimeSpan.MaxValue, callback, state)
      {
        this.trackingContext = trackingContext;
        this.filter = filter;
        this.settings = settings;
        this.Descriptions = descriptions;
        this.ResourceNames = resourceNames;
        this.Addresses = addresses;
        this.Start();
      }

      public IResourceDescription[] Descriptions { get; private set; }

      public SyndicationFeed Feed { get; private set; }

      public string[] ResourceNames { get; private set; }

      private IEnumerable<Uri> Addresses { get; set; }

      protected override IEnumerator<IteratorAsyncResult<NamespaceManager.GetAllAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        yield return this.CallAsync((IteratorAsyncResult<NamespaceManager.GetAllAsyncResult>.BeginCall) ((thisPtr, t, c, s) => ServiceBusResourceOperations.BeginGetAll(thisPtr.trackingContext, thisPtr.filter, thisPtr.Descriptions, thisPtr.ResourceNames, thisPtr.Addresses, thisPtr.settings, c, s)), (IteratorAsyncResult<NamespaceManager.GetAllAsyncResult>.EndCall) ((thisPtr, r) => thisPtr.Feed = ServiceBusResourceOperations.EndGetAll(r)), IteratorAsyncResult<NamespaceManager.GetAllAsyncResult>.ExceptionPolicy.Transfer);
      }
    }

    private class EntityDescriptionSyndicationFeed<TEntityDescription> where TEntityDescription : EntityDescription, IResourceDescription
    {
      private readonly Action<TEntityDescription, string> onFeedEntry;

      public EntityDescriptionSyndicationFeed(
        SyndicationFeed feed,
        Action<TEntityDescription, string> onFeedEntry)
      {
        this.Feed = feed;
        this.onFeedEntry = onFeedEntry;
      }

      public SyndicationFeed Feed { get; private set; }

      public IEnumerable<TEntityDescription> Entities
      {
        get
        {
          if (this.Feed != null)
          {
            foreach (SyndicationItem syndicationItem in this.Feed.Items)
            {
              string text = syndicationItem.Title.Text;
              if (string.IsNullOrEmpty(text))
                throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.AsError((Exception) new MessagingException("EntityDescription: Atom Xml Title property is empty."));
              if (!(syndicationItem.Content is XmlSyndicationContent content))
                throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.AsError((Exception) new MessagingException("EntityDescription: Unable to read Atom XML content"));
              TEntityDescription entity = content.ReadContent<TEntityDescription>();
              if (this.onFeedEntry != null)
                this.onFeedEntry(entity, text);
              entity.IsReadOnly = false;
              yield return entity;
            }
          }
        }
      }
    }

    private class NotificationHubSyndicationFeed
    {
      public NotificationHubSyndicationFeed(SyndicationFeed feed) => this.Feed = feed;

      public SyndicationFeed Feed { get; private set; }

      public IEnumerable<NotificationHubDescription> NotificationHubs
      {
        get
        {
          if (this.Feed != null)
          {
            foreach (SyndicationItem syndicationItem in this.Feed.Items)
            {
              string text = syndicationItem.Title.Text;
              if (string.IsNullOrEmpty(text))
                throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.AsError((Exception) new MessagingException("NotificationHubDescription: Atom Xml Title property is empty."));
              if (!(syndicationItem.Content is XmlSyndicationContent content))
                throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.AsError((Exception) new MessagingException("NotificationHubDescription: Unable to read Atom XML content"));
              NotificationHubDescription notificationHub = content.ReadContent<NotificationHubDescription>();
              notificationHub.Path = text;
              notificationHub.IsReadOnly = false;
              yield return notificationHub;
            }
          }
        }
      }
    }

    internal class RegistrationSyndicationFeed
    {
      public RegistrationSyndicationFeed(SyndicationFeed feed) => this.Feed = feed;

      public SyndicationFeed Feed { get; private set; }

      public IEnumerable<RegistrationDescription> Registrations
      {
        get
        {
          if (this.Feed != null)
          {
            foreach (SyndicationItem syndicationItem in this.Feed.Items)
            {
              if (string.IsNullOrEmpty(syndicationItem.Title.Text))
                throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.AsError((Exception) new MessagingException("RegistrationDescription: Atom Xml Title property is empty."));
              if (!(syndicationItem.Content is XmlSyndicationContent syndicationContent))
                throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.AsError((Exception) new MessagingException("RegistrationDescription: Unable to read Atom XML content"));
              XmlDictionaryReader reader = syndicationContent.GetReaderAtContent();
              reader.Read();
              if (reader.Name == typeof (WindowsRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<WindowsRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (WindowsTemplateRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<WindowsTemplateRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (AppleRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<AppleRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (AppleTemplateRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<AppleTemplateRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (GcmRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<GcmRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (GcmTemplateRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<GcmTemplateRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (MpnsRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<MpnsRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (MpnsTemplateRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<MpnsTemplateRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (AdmRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<AdmRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (AdmTemplateRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<AdmTemplateRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (NokiaXRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<NokiaXRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (NokiaXTemplateRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<NokiaXTemplateRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (BaiduRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<BaiduRegistrationDescription>(syndicationContent);
              else if (reader.Name == typeof (BaiduTemplateRegistrationDescription).Name)
                yield return (RegistrationDescription) this.TryReadContent<BaiduTemplateRegistrationDescription>(syndicationContent);
              syndicationContent = (XmlSyndicationContent) null;
              reader = (XmlDictionaryReader) null;
            }
          }
        }
      }

      private TContent TryReadContent<TContent>(XmlSyndicationContent syndicationContent) where TContent : RegistrationDescription
      {
        try
        {
          return syndicationContent.ReadContent<TContent>();
        }
        catch (SerializationException ex)
        {
        }
        return default (TContent);
      }
    }
  }
}
