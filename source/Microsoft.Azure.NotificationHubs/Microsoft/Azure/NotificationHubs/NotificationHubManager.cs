// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationHubManager
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Common.Parallel;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Messaging.Configuration;
using Microsoft.Azure.NotificationHubs.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs
{
  internal class NotificationHubManager
  {
    internal const string TagHeaderName = "ServiceBusNotification-Tags";
    internal const string ScheduleTimeHeaderName = "ServiceBusNotification-ScheduleTime";
    internal string notificationHubPath;
    internal TokenProvider tokenProvider;
    internal NamespaceManager namespaceManager;
    internal Uri baseUri;

    public NotificationHubManager(string connectionString, string notificationHubPath)
    {
      this.notificationHubPath = notificationHubPath;
      KeyValueConfigurationManager manager = new KeyValueConfigurationManager(connectionString);
      this.namespaceManager = manager.CreateNamespaceManager();
      this.GetTokenProvider(manager);
      this.GetBaseUri(manager);
    }

    public NotificationHubManager(string connectionString)
    {
      KeyValueConfigurationManager manager = new KeyValueConfigurationManager(connectionString);
      this.notificationHubPath = manager.connectionProperties["EntityPath"];
      this.namespaceManager = manager.CreateNamespaceManager();
      this.GetTokenProvider(manager);
      this.GetBaseUri(manager);
    }

    public Task<NotificationOutcome> SendNotificationAsync(
      Notification notification,
      bool testSend,
      string tagExpression)
    {
      NotificationType notificationType = testSend ? NotificationType.DebugSend : NotificationType.Send;
      return TaskHelpers.CreateTask<NotificationOutcome>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginSendNotification(notification, notificationType, tagExpression, (string) null, c, s)), new Func<IAsyncResult, NotificationOutcome>(this.EndSendNotification));
    }

    public Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job) => this.namespaceManager.SubmitNotificationHubJobAsync(job, this.notificationHubPath);

    public Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId) => this.namespaceManager.GetNotificationHubJobAsync(jobId, this.notificationHubPath);

    public Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync() => this.namespaceManager.GetNotificationHubJobsAsync(this.notificationHubPath);

    public Task<T> CreateOrUpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription
    {
      registration = (T) registration.Clone();
      registration.NotificationHubPath = this.notificationHubPath;
      registration.ETag = (string) null;
      registration.ExpirationTime = new DateTime?();
      return this.namespaceManager.UpdateRegistrationAsync<T>(registration);
    }

    public Task<T> UpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription
    {
      registration = (T) registration.Clone();
      registration.NotificationHubPath = this.notificationHubPath;
      registration.ExpirationTime = new DateTime?();
      return this.namespaceManager.UpdateRegistrationAsync<T>(registration);
    }

    public Task<string> CreateRegistrationIdAsync() => this.namespaceManager.CreateRegistrationIdAsync(this.notificationHubPath);

    public Task<T> CreateRegistrationAsync<T>(T registration) where T : RegistrationDescription
    {
      registration = (T) registration.Clone();
      registration.NotificationHubPath = this.notificationHubPath;
      registration.ExpirationTime = new DateTime?();
      registration.ETag = (string) null;
      registration.RegistrationId = (string) null;
      return this.namespaceManager.CreateRegistrationAsync<T>(registration);
    }

    public Task<IEnumerable<RegistrationDescription>> UpdateRegistrationsWithNewPnsHandleAsync(
      string oldPnsHandle,
      string newPnsHandle)
    {
      return TaskHelpers.CreateTask<IEnumerable<RegistrationDescription>>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginUpdateRegistrationsWithNewPnsHandle(oldPnsHandle, newPnsHandle, c, s)), new Func<IAsyncResult, IEnumerable<RegistrationDescription>>(this.EndUpdateRegistrationsWithNewPnsHandle));
    }

    public Task<bool> RegistrationExistsAsync(string registrationId) => this.GetRegistrationAsync(registrationId).ContinueWith<bool>((Func<Task<RegistrationDescription>, bool>) (r => r.Result != null));

    public Task<RegistrationDescription> GetRegistrationAsync(string registrationId) => this.namespaceManager.GetRegistrationAsync<RegistrationDescription>(registrationId, this.notificationHubPath).ContinueWith<RegistrationDescription>((Func<Task<RegistrationDescription>, RegistrationDescription>) (r =>
    {
      if (!r.IsFaulted)
        return r.Result;
      if (r.Exception.Flatten().InnerException is MessagingEntityNotFoundException)
        return (RegistrationDescription) null;
      throw r.Exception;
    }));

    public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(
      string continuationToken,
      int top)
    {
      return this.namespaceManager.GetAllRegistrationsAsync(this.notificationHubPath, continuationToken, top);
    }

    public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(
      string pnsHandle,
      string continuationToken,
      int top)
    {
      return this.namespaceManager.GetRegistrationsByChannelAsync(pnsHandle, this.notificationHubPath, continuationToken, top);
    }

    public Task DeleteRegistrationAsync(string registrationId, string etag) => this.namespaceManager.DeleteRegistrationAsync(this.notificationHubPath, registrationId, etag);

    public Task DeleteRegistrationsByChannelAsync(string pnsHandle)
    {
      TaskCompletionSource<object> taskSource = new TaskCompletionSource<object>();
      this.DeleteRegistrationsByChannelAsyncInternal(pnsHandle, taskSource);
      return (Task) taskSource.Task;
    }

    public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(
      string tag,
      string continuationToken,
      int top)
    {
      return this.namespaceManager.GetRegistrationsByTagAsync(this.notificationHubPath, tag, continuationToken, top);
    }

    public Task<RegistrationCounts> GetRegistrationCountsByTagAsync(string tag) => TaskHelpers.CreateTask<RegistrationCounts>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => (IAsyncResult) new GetRegistrationCountsAsyncResult(this, tag, c, s)), (Func<IAsyncResult, RegistrationCounts>) (r => AsyncResult<GetRegistrationCountsAsyncResult>.End(r).Result));

    public Task<NotificationDetails> GetNotificationAsync(string notificationId) => TaskHelpers.CreateTask<NotificationDetails>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => (IAsyncResult) new GetNotificationAsyncResult(this, notificationId, c, s)), (Func<IAsyncResult, NotificationDetails>) (r => AsyncResult<GetNotificationAsyncResult>.End(r).Result));

    public Task<Uri> GetFeedbackContainerUriAsync() => TaskHelpers.CreateTask<Uri>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => (IAsyncResult) new GetFeedbackContainerUriAsyncResult(this, c, s)), (Func<IAsyncResult, Uri>) (r => AsyncResult<GetFeedbackContainerUriAsyncResult>.End(r).Result));

    public Task<Installation> GetInstallationAsync(string installationId) => this.namespaceManager.GetInstallationAsync(installationId, this.notificationHubPath).ContinueWith<Installation>((Func<Task<string>, Installation>) (t => Installation.FromJson(t.Result)));

    public Task CreateOrUpdateInstallationAsync(Installation installation) => (Task) this.namespaceManager.CreateOrUpdateInstallationAsync(installation.ToJson(), installation.InstallationId, "PUT", this.notificationHubPath);

    public Task PatchInstallationAsync(
      string installationId,
      IList<PartialUpdateOperation> operations)
    {
      return (Task) this.namespaceManager.CreateOrUpdateInstallationAsync(operations.ToJson(), installationId, "PATCH", this.notificationHubPath);
    }

    public Task DeleteInstallationAsync(string installationId) => this.namespaceManager.DeleteInstallationAsync(installationId, this.notificationHubPath);

    public Task<RegistrationCounts> GetRegistrationCountsAsync() => TaskHelpers.CreateTask<RegistrationCounts>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => (IAsyncResult) new GetRegistrationCountsAsyncResult(this, (string) null, c, s)), (Func<IAsyncResult, RegistrationCounts>) (r => AsyncResult<GetRegistrationCountsAsyncResult>.End(r).Result));

    private void DeleteRegistrationsByChannelAsyncInternal(
      string pnsHandle,
      TaskCompletionSource<object> taskSource)
    {
      this.namespaceManager.GetRegistrationsByChannelAsync(pnsHandle, this.notificationHubPath, (string) null, -1).ContinueWith((Action<Task<CollectionQueryResult<RegistrationDescription>>>) (t =>
      {
        if (t.IsFaulted)
        {
          taskSource.SetException(t.Exception.InnerException);
        }
        else
        {
          List<Task> list = t.Result.Select<RegistrationDescription, Task>((Func<RegistrationDescription, Task>) (item => this.namespaceManager.DeleteRegistrationAsync(this.notificationHubPath, item.RegistrationId, "*"))).ToList<Task>();
          if (list.Any<Task>())
            Task.Factory.ContinueWhenAll(list.ToArray(), (Action<Task[]>) (deleteTasks =>
            {
              if (((IEnumerable<Task>) deleteTasks).Any<Task>((Func<Task, bool>) (deleteTask => deleteTask.IsFaulted)))
                taskSource.SetException(((IEnumerable<Task>) deleteTasks).Where<Task>((Func<Task, bool>) (deleteTask => deleteTask.IsFaulted)).Select<Task, Exception>((Func<Task, Exception>) (deleteTask => deleteTask.Exception.InnerException)));
              else if (t.Result.ContinuationToken != null)
                this.DeleteRegistrationsByChannelAsyncInternal(pnsHandle, taskSource);
              else
                taskSource.SetResult((object) null);
            }));
          else
            taskSource.SetResult((object) null);
        }
      }));
    }

    public Task<ScheduledNotification> ScheduleNotificationAsync(
      Notification notification,
      DateTimeOffset scheduledTime,
      string tagExpression)
    {
      return TaskHelpers.CreateTask<ScheduledNotification>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginScheduleNotification(notification, scheduledTime, tagExpression, c, s)), new Func<IAsyncResult, ScheduledNotification>(this.EndScheduleNotification));
    }

    public Task<NotificationOutcome> SendDirectNotificationAsync(
      Notification nativeNotification,
      string deviceHandle)
    {
      if (!(nativeNotification is INativeNotification))
        throw new ArgumentException("Template notifications are not supported with direct send");
      return TaskHelpers.CreateTask<NotificationOutcome>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginSendNotification(nativeNotification, NotificationType.DirectSend, (string) null, deviceHandle, c, s)), new Func<IAsyncResult, NotificationOutcome>(this.EndSendNotification));
    }

    public Task<NotificationOutcome> SendDirectNotificationAsync(
      Notification nativeNotification,
      IList<string> deviceHandles)
    {
      if (!(nativeNotification is INativeNotification))
        throw new ArgumentException("Template notifications are not supported with direct send");
      nativeNotification.ValidateAndPopulateHeaders();
      return TaskHelpers.CreateTask<NotificationOutcome>((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => new SendBatchNotificationAsyncResult(this, nativeNotification, deviceHandles, c, s).Start()), (Func<IAsyncResult, NotificationOutcome>) (r => AsyncResult<SendBatchNotificationAsyncResult>.End(r).Result));
    }

    private IAsyncResult BeginScheduleNotification(
      Notification notification,
      DateTimeOffset scheduledTime,
      string tagExpression,
      AsyncCallback callback,
      object state)
    {
      notification.ValidateAndPopulateHeaders();
      ScheduleNotificationAsyncResult notificationAsyncResult = new ScheduleNotificationAsyncResult(this, notification, scheduledTime, tagExpression, callback, state);
      notificationAsyncResult.Start();
      return (IAsyncResult) notificationAsyncResult;
    }

    private ScheduledNotification EndScheduleNotification(IAsyncResult result) => AsyncResult<ScheduleNotificationAsyncResult>.End(result).Result;

    public Task CancelScheduledNotificationAsync(string scheduledNotificationId) => TaskHelpers.CreateTask((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => this.BeginCancelScheduledNotification(scheduledNotificationId, c, s)), new Action<IAsyncResult>(this.EndCancelScheduledNotification));

    private IAsyncResult BeginCancelScheduledNotification(
      string scheduledNotificationId,
      AsyncCallback callback,
      object state)
    {
      CancelScheduledNotificationAsyncResult notificationAsyncResult = new CancelScheduledNotificationAsyncResult(this, scheduledNotificationId, callback, state);
      notificationAsyncResult.Start();
      return (IAsyncResult) notificationAsyncResult;
    }

    private void EndCancelScheduledNotification(IAsyncResult result) => AsyncResult<CancelScheduledNotificationAsyncResult>.End(result);

    private IAsyncResult BeginSendNotification(
      Notification notification,
      NotificationType notificationType,
      string tagExpression,
      string deviceHandle,
      AsyncCallback callback,
      object state)
    {
      notification.ValidateAndPopulateHeaders();
      SendNotificationAsyncResult notificationAsyncResult = new SendNotificationAsyncResult(this, notification, notificationType, tagExpression, deviceHandle, callback, state);
      notificationAsyncResult.Start();
      return (IAsyncResult) notificationAsyncResult;
    }

    private NotificationOutcome EndSendNotification(IAsyncResult result) => AsyncResult<SendNotificationAsyncResult>.End(result).Result;

    private IAsyncResult BeginUpdateRegistrationsWithNewPnsHandle(
      string oldPnsHandle,
      string newPnsHandle,
      AsyncCallback callback,
      object state)
    {
      if (string.IsNullOrEmpty(oldPnsHandle))
        throw new ArgumentNullException(nameof (oldPnsHandle));
      if (string.IsNullOrEmpty(newPnsHandle))
        throw new ArgumentNullException(nameof (newPnsHandle));
      UpdatePnsHandleAsyncResult handleAsyncResult = new UpdatePnsHandleAsyncResult(this, oldPnsHandle, newPnsHandle, callback, state);
      handleAsyncResult.Start();
      return (IAsyncResult) handleAsyncResult;
    }

    private IEnumerable<RegistrationDescription> EndUpdateRegistrationsWithNewPnsHandle(
      IAsyncResult result)
    {
      return AsyncResult<UpdatePnsHandleAsyncResult>.End(result).UpdatedRegistrations;
    }

    private void GetTokenProvider(KeyValueConfigurationManager manager)
    {
      IEnumerable<Uri> endpointAddresses = (IEnumerable<Uri>) KeyValueConfigurationManager.GetEndpointAddresses(manager.connectionProperties["StsEndpoint"], (string) null);
      string connectionProperty1 = manager.connectionProperties["SharedSecretIssuer"];
      string connectionProperty2 = manager.connectionProperties["SharedSecretValue"];
      string connectionProperty3 = manager.connectionProperties["SharedAccessKeyName"];
      string connectionProperty4 = manager.connectionProperties["SharedAccessKey"];
      if (!string.IsNullOrEmpty(connectionProperty2))
      {
        if (endpointAddresses != null && endpointAddresses.Any<Uri>())
          this.tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(connectionProperty1, connectionProperty2, endpointAddresses.First<Uri>());
        else
          this.tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(connectionProperty1, connectionProperty2);
      }
      else
        this.tokenProvider = !string.IsNullOrEmpty(connectionProperty3) ? TokenProvider.CreateSharedAccessSignatureTokenProvider(connectionProperty3, connectionProperty4) : throw new ArgumentException("connectionString");
    }

    private void GetBaseUri(KeyValueConfigurationManager manager)
    {
      using (IEnumerator<Uri> enumerator = KeyValueConfigurationManager.GetEndpointAddresses(manager.connectionProperties["Endpoint"], manager.connectionProperties["ManagementPort"]).GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        this.baseUri = enumerator.Current;
      }
    }
  }
}
