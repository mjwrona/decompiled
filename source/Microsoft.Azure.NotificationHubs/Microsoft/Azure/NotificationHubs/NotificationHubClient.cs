// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationHubClient
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs
{
  public class NotificationHubClient
  {
    private NotificationHubManager manager;
    private string notificationHubPath;

    private NotificationHubClient(string connectionString, string notificationHubPath)
    {
      if (string.IsNullOrWhiteSpace(connectionString))
        throw new ArgumentNullException(nameof (connectionString));
      this.manager = !string.IsNullOrWhiteSpace(notificationHubPath) ? new NotificationHubManager(connectionString, notificationHubPath) : throw new ArgumentNullException(nameof (notificationHubPath));
      this.notificationHubPath = notificationHubPath;
    }

    public static NotificationHubClient CreateClientFromConnectionString(
      string connectionString,
      string notificationHubPath)
    {
      return new NotificationHubClient(connectionString, notificationHubPath);
    }

    public static NotificationHubClient CreateClientFromConnectionString(
      string connectionString,
      string notificationHubPath,
      bool enableTestSend)
    {
      return new NotificationHubClient(connectionString, notificationHubPath)
      {
        EnableTestSend = enableTestSend
      };
    }

    public bool EnableTestSend { get; private set; }

    internal NotificationOutcome SendWindowsNativeNotification(string windowsNativePayload) => this.SendWindowsNativeNotification(windowsNativePayload, string.Empty);

    internal NotificationOutcome SendWindowsNativeNotification(
      string windowsNativePayload,
      string tagExpression)
    {
      return this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendWindowsNativeNotificationAsync(windowsNativePayload, tagExpression)));
    }

    public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload) => this.SendWindowsNativeNotificationAsync(windowsNativePayload, string.Empty);

    public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(
      string windowsNativePayload,
      string tagExpression)
    {
      return this.SendNotificationAsync((Notification) new WindowsNotification(windowsNativePayload), tagExpression);
    }

    public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(
      string windowsNativePayload,
      IEnumerable<string> tags)
    {
      return this.SendNotificationAsync((Notification) new WindowsNotification(windowsNativePayload), tags);
    }

    internal NotificationOutcome SendAppleNativeNotification(string jsonPayload) => this.SendAppleNativeNotification(jsonPayload, string.Empty);

    internal NotificationOutcome SendAppleNativeNotification(
      string jsonPayload,
      string tagExpression)
    {
      return this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendAppleNativeNotificationAsync(jsonPayload, tagExpression)));
    }

    public Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId) => this.manager.GetNotificationHubJobAsync(jobId);

    public Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync() => this.manager.GetNotificationHubJobsAsync();

    public Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job) => this.manager.SubmitNotificationHubJobAsync(job);

    public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload) => this.SendAppleNativeNotificationAsync(jsonPayload, string.Empty);

    public Task<NotificationOutcome> SendAppleNativeNotificationAsync(
      string jsonPayload,
      string tagExpression)
    {
      return this.SendNotificationAsync((Notification) new AppleNotification(jsonPayload), tagExpression);
    }

    public Task<NotificationOutcome> SendAppleNativeNotificationAsync(
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SendNotificationAsync((Notification) new AppleNotification(jsonPayload), tags);
    }

    internal NotificationOutcome SendTemplateNotification(IDictionary<string, string> properties) => this.SendTemplateNotification(properties, string.Empty);

    internal NotificationOutcome SendTemplateNotification(
      IDictionary<string, string> properties,
      string tagExpression)
    {
      return this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendTemplateNotificationAsync(properties, tagExpression)));
    }

    public Task<NotificationOutcome> SendTemplateNotificationAsync(
      IDictionary<string, string> properties)
    {
      return this.SendTemplateNotificationAsync(properties, string.Empty);
    }

    public Task<NotificationOutcome> SendTemplateNotificationAsync(
      IDictionary<string, string> properties,
      string tagExpression)
    {
      return this.SendNotificationAsync((Notification) new TemplateNotification(properties), tagExpression);
    }

    public Task<NotificationOutcome> SendTemplateNotificationAsync(
      IDictionary<string, string> properties,
      IEnumerable<string> tags)
    {
      return this.SendNotificationAsync((Notification) new TemplateNotification(properties), tags);
    }

    internal NotificationOutcome SendGcmNativeNotification(string jsonPayload) => this.SendGcmNativeNotification(jsonPayload, string.Empty);

    internal NotificationOutcome SendGcmNativeNotification(string jsonPayload, string tagExpression) => this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendGcmNativeNotificationAsync(jsonPayload, tagExpression)));

    public Task<NotificationOutcome> SendGcmNativeNotificationAsync(string jsonPayload) => this.SendGcmNativeNotificationAsync(jsonPayload, string.Empty);

    public Task<NotificationOutcome> SendGcmNativeNotificationAsync(
      string jsonPayload,
      string tagExpression)
    {
      return this.SendNotificationAsync((Notification) new GcmNotification(jsonPayload), tagExpression);
    }

    public Task<NotificationOutcome> SendGcmNativeNotificationAsync(
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SendNotificationAsync((Notification) new GcmNotification(jsonPayload), tags);
    }

    internal NotificationOutcome SendNokiaXNativeNotification(string jsonload) => this.SendNokiaXNativeNotification(jsonload, string.Empty);

    internal NotificationOutcome SendNokiaXNativeNotification(
      string jsonPayload,
      string tagExpression)
    {
      return this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendNokiaXNativeNotificationAsync(jsonPayload, tagExpression)));
    }

    internal Task<NotificationOutcome> SendNokiaXNativeNotificationAsync(string jsonPayload) => this.SendNotificationAsync((Notification) new NokiaXNotification(jsonPayload), string.Empty);

    internal Task<NotificationOutcome> SendNokiaXNativeNotificationAsync(
      string jsonPayload,
      string tagExpression)
    {
      return this.SendNotificationAsync((Notification) new NokiaXNotification(jsonPayload), tagExpression);
    }

    internal Task<NotificationOutcome> SendNokiaXNativeNotificationAsync(
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SendNotificationAsync((Notification) new NokiaXNotification(jsonPayload), tags);
    }

    internal NotificationOutcome SendBaiduNativeNotification(string message) => this.SendBaiduNativeNotification(message, string.Empty);

    internal NotificationOutcome SendBaiduNativeNotification(string message, string tagExpression) => this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendBaiduNativeNotificationAsync(message, tagExpression)));

    public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message) => this.SendNotificationAsync((Notification) new BaiduNotification(message), string.Empty);

    public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(
      string message,
      string tagExpression)
    {
      return this.SendNotificationAsync((Notification) new BaiduNotification(message), tagExpression);
    }

    public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(
      string message,
      IEnumerable<string> tags)
    {
      return this.SendNotificationAsync((Notification) new BaiduNotification(message), tags);
    }

    internal NotificationOutcome SendAdmNativeNotification(string jsonPayload) => this.SendAdmNativeNotification(jsonPayload, string.Empty);

    internal NotificationOutcome SendAdmNativeNotification(string jsonPayload, string tagExpression) => this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendAdmNativeNotificationAsync(jsonPayload, tagExpression)));

    public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload) => this.SendAdmNativeNotificationAsync(jsonPayload, string.Empty);

    public Task<NotificationOutcome> SendAdmNativeNotificationAsync(
      string jsonPayload,
      string tagExpression)
    {
      return this.SendNotificationAsync((Notification) new AdmNotification(jsonPayload), tagExpression);
    }

    public Task<NotificationOutcome> SendAdmNativeNotificationAsync(
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SendNotificationAsync((Notification) new AdmNotification(jsonPayload), tags);
    }

    internal NotificationOutcome SendMpnsNativeNotification(string nativePayload) => this.SendMpnsNativeNotification(nativePayload, string.Empty);

    internal NotificationOutcome SendMpnsNativeNotification(
      string nativePayload,
      string tagExpression)
    {
      return this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendMpnsNativeNotificationAsync(nativePayload, tagExpression)));
    }

    public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload) => this.SendMpnsNativeNotificationAsync(nativePayload, string.Empty);

    public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(
      string nativePayload,
      string tagExpression)
    {
      return this.SendNotificationAsync((Notification) new MpnsNotification(nativePayload), tagExpression);
    }

    public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(
      string nativePayload,
      IEnumerable<string> tags)
    {
      return this.SendNotificationAsync((Notification) new MpnsNotification(nativePayload), tags);
    }

    internal NotificationOutcome SendNotification(Notification notification) => this.SyncOp<NotificationOutcome>((Func<Task<NotificationOutcome>>) (() => this.SendNotificationAsync(notification)));

    public Task<NotificationOutcome> SendNotificationAsync(Notification notification)
    {
      if (notification == null)
        throw new ArgumentNullException(nameof (notification));
      return this.manager.SendNotificationAsync(notification, this.EnableTestSend, notification.tag);
    }

    public Task<NotificationOutcome> SendNotificationAsync(
      Notification notification,
      string tagExpression)
    {
      if (notification == null)
        throw new ArgumentNullException(nameof (notification));
      if (notification.tag != null)
        throw new ArgumentException("notification.Tag property should be null");
      return this.manager.SendNotificationAsync(notification, this.EnableTestSend, tagExpression);
    }

    public Task<NotificationOutcome> SendNotificationAsync(
      Notification notification,
      IEnumerable<string> tags)
    {
      if (notification == null)
        throw new ArgumentNullException(nameof (notification));
      if (notification.tag != null)
        throw new ArgumentException("notification.Tag property should be null");
      if (tags == null)
        throw new ArgumentNullException(nameof (tags));
      string tagExpression = tags.Count<string>() != 0 ? string.Join("||", tags) : throw new ArgumentException("tags argument should contain atleat one tag");
      return this.manager.SendNotificationAsync(notification, this.EnableTestSend, tagExpression);
    }

    public Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId) => !string.IsNullOrWhiteSpace(notificationId) ? this.manager.GetNotificationAsync(notificationId) : throw new ArgumentNullException(nameof (notificationId));

    public Task<Uri> GetFeedbackContainerUriAsync() => this.manager.GetFeedbackContainerUriAsync();

    public void CreateOrUpdateInstallation(Installation installation) => this.SyncOp((Func<Task>) (() => this.CreateOrUpdateInstallationAsync(installation)));

    public Task CreateOrUpdateInstallationAsync(Installation installation)
    {
      if (installation == null)
        throw new ArgumentNullException(nameof (installation));
      return !string.IsNullOrWhiteSpace(installation.InstallationId) ? this.manager.CreateOrUpdateInstallationAsync(installation) : throw new InvalidOperationException("InstallationId must be specified");
    }

    public void PatchInstallation(string installationId, IList<PartialUpdateOperation> operations) => this.SyncOp((Func<Task>) (() => this.PatchInstallationAsync(installationId, operations)));

    public Task PatchInstallationAsync(
      string installationId,
      IList<PartialUpdateOperation> operations)
    {
      if (string.IsNullOrWhiteSpace(installationId))
        throw new ArgumentNullException(nameof (installationId));
      if (operations == null)
        throw new ArgumentNullException(nameof (operations));
      if (operations.Count == 0)
        throw new InvalidOperationException("Operations list is empty");
      return this.manager.PatchInstallationAsync(installationId, operations);
    }

    public void DeleteInstallation(string installationId) => this.SyncOp((Func<Task>) (() => this.DeleteInstallationAsync(installationId)));

    public Task DeleteInstallationAsync(string installationId) => !string.IsNullOrWhiteSpace(installationId) ? this.manager.DeleteInstallationAsync(installationId) : throw new ArgumentNullException(nameof (installationId));

    public Installation GetInstallation(string installationId) => this.SyncOp<Installation>((Func<Task<Installation>>) (() => this.GetInstallationAsync(installationId)));

    public Task<Installation> GetInstallationAsync(string installationId) => !string.IsNullOrWhiteSpace(installationId) ? this.manager.GetInstallationAsync(installationId) : throw new ArgumentNullException(nameof (installationId));

    public Task<string> CreateRegistrationIdAsync() => this.manager.CreateRegistrationIdAsync();

    internal WindowsRegistrationDescription CreateWindowsNativeRegistration(string channelUri) => this.CreateWindowsNativeRegistration(channelUri, (IEnumerable<string>) null);

    internal WindowsRegistrationDescription CreateWindowsNativeRegistration(
      string channelUri,
      IEnumerable<string> tags)
    {
      return this.SyncOp<WindowsRegistrationDescription>((Func<Task<WindowsRegistrationDescription>>) (() => this.CreateWindowsNativeRegistrationAsync(channelUri, tags)));
    }

    public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(
      string channelUri)
    {
      return this.CreateWindowsNativeRegistrationAsync(channelUri, (IEnumerable<string>) null);
    }

    public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(
      string channelUri,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<WindowsRegistrationDescription>(new WindowsRegistrationDescription(new Uri(channelUri), tags));
    }

    internal WindowsTemplateRegistrationDescription CreateWindowsTemplateRegistration(
      string channelUri,
      string xmlTemplate)
    {
      return this.CreateWindowsTemplateRegistration(channelUri, xmlTemplate, (IEnumerable<string>) null);
    }

    internal WindowsTemplateRegistrationDescription CreateWindowsTemplateRegistration(
      string channelUri,
      string xmlTemplate,
      IEnumerable<string> tags)
    {
      return this.SyncOp<WindowsTemplateRegistrationDescription>((Func<Task<WindowsTemplateRegistrationDescription>>) (() => this.CreateWindowsTemplateRegistrationAsync(channelUri, xmlTemplate, tags)));
    }

    public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(
      string channelUri,
      string xmlTemplate)
    {
      return this.CreateWindowsTemplateRegistrationAsync(channelUri, xmlTemplate, (IEnumerable<string>) null);
    }

    public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(
      string channelUri,
      string xmlTemplate,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<WindowsTemplateRegistrationDescription>(new WindowsTemplateRegistrationDescription(new Uri(channelUri), xmlTemplate, tags));
    }

    internal AppleRegistrationDescription CreateAppleNativeRegistration(string deviceToken) => this.CreateAppleNativeRegistration(deviceToken, (IEnumerable<string>) null);

    internal AppleRegistrationDescription CreateAppleNativeRegistration(
      string deviceToken,
      IEnumerable<string> tags)
    {
      return this.SyncOp<AppleRegistrationDescription>((Func<Task<AppleRegistrationDescription>>) (() => this.CreateAppleNativeRegistrationAsync(deviceToken, tags)));
    }

    public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken) => this.CreateAppleNativeRegistrationAsync(deviceToken, (IEnumerable<string>) null);

    public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(
      string deviceToken,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<AppleRegistrationDescription>(new AppleRegistrationDescription(deviceToken, tags));
    }

    internal AppleTemplateRegistrationDescription CreateAppleTemplateRegistration(
      string deviceToken,
      string jsonPayload)
    {
      return this.CreateAppleTemplateRegistration(deviceToken, jsonPayload, (IEnumerable<string>) null);
    }

    internal AppleTemplateRegistrationDescription CreateAppleTemplateRegistration(
      string deviceToken,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SyncOp<AppleTemplateRegistrationDescription>((Func<Task<AppleTemplateRegistrationDescription>>) (() => this.CreateAppleTemplateRegistrationAsync(deviceToken, jsonPayload, tags)));
    }

    public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(
      string deviceToken,
      string jsonPayload)
    {
      return this.CreateAppleTemplateRegistrationAsync(deviceToken, jsonPayload, (IEnumerable<string>) null);
    }

    public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(
      string deviceToken,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<AppleTemplateRegistrationDescription>(new AppleTemplateRegistrationDescription(deviceToken, jsonPayload, tags));
    }

    internal AdmRegistrationDescription CreateAdmNativeRegistration(string admRegistrationId) => this.CreateAdmNativeRegistration(admRegistrationId, (IEnumerable<string>) null);

    internal AdmRegistrationDescription CreateAdmNativeRegistration(
      string admRegistrationId,
      IEnumerable<string> tags)
    {
      return this.SyncOp<AdmRegistrationDescription>((Func<Task<AdmRegistrationDescription>>) (() => this.CreateAdmNativeRegistrationAsync(admRegistrationId, tags)));
    }

    public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(
      string admRegistrationId)
    {
      return this.CreateAdmNativeRegistrationAsync(admRegistrationId, (IEnumerable<string>) null);
    }

    public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(
      string admRegistrationId,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<AdmRegistrationDescription>(new AdmRegistrationDescription(admRegistrationId, tags));
    }

    internal AdmTemplateRegistrationDescription CreateAdmTemplateRegistration(
      string admRegistrationId,
      string jsonPayload)
    {
      return this.CreateAdmTemplateRegistration(admRegistrationId, jsonPayload, (IEnumerable<string>) null);
    }

    internal AdmTemplateRegistrationDescription CreateAdmTemplateRegistration(
      string admRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SyncOp<AdmTemplateRegistrationDescription>((Func<Task<AdmTemplateRegistrationDescription>>) (() => this.CreateAdmTemplateRegistrationAsync(admRegistrationId, jsonPayload, tags)));
    }

    public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(
      string admRegistrationId,
      string jsonPayload)
    {
      return this.CreateAdmTemplateRegistrationAsync(admRegistrationId, jsonPayload, (IEnumerable<string>) null);
    }

    public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(
      string admRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<AdmTemplateRegistrationDescription>(new AdmTemplateRegistrationDescription(admRegistrationId, jsonPayload, tags));
    }

    internal GcmRegistrationDescription CreateGcmNativeRegistration(string gcmRegistrationId) => this.CreateGcmNativeRegistration(gcmRegistrationId, (IEnumerable<string>) null);

    internal GcmRegistrationDescription CreateGcmNativeRegistration(
      string gcmRegistrationId,
      IEnumerable<string> tags)
    {
      return this.SyncOp<GcmRegistrationDescription>((Func<Task<GcmRegistrationDescription>>) (() => this.CreateGcmNativeRegistrationAsync(gcmRegistrationId, tags)));
    }

    public Task<GcmRegistrationDescription> CreateGcmNativeRegistrationAsync(
      string gcmRegistrationId)
    {
      return this.CreateGcmNativeRegistrationAsync(gcmRegistrationId, (IEnumerable<string>) null);
    }

    public Task<GcmRegistrationDescription> CreateGcmNativeRegistrationAsync(
      string gcmRegistrationId,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<GcmRegistrationDescription>(new GcmRegistrationDescription(gcmRegistrationId, tags));
    }

    internal GcmTemplateRegistrationDescription CreateGcmTemplateRegistration(
      string gcmRegistrationId,
      string jsonPayload)
    {
      return this.CreateGcmTemplateRegistration(gcmRegistrationId, jsonPayload, (IEnumerable<string>) null);
    }

    internal GcmTemplateRegistrationDescription CreateGcmTemplateRegistration(
      string gcmRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SyncOp<GcmTemplateRegistrationDescription>((Func<Task<GcmTemplateRegistrationDescription>>) (() => this.CreateGcmTemplateRegistrationAsync(gcmRegistrationId, jsonPayload, tags)));
    }

    public Task<GcmTemplateRegistrationDescription> CreateGcmTemplateRegistrationAsync(
      string gcmRegistrationId,
      string jsonPayload)
    {
      return this.CreateGcmTemplateRegistrationAsync(gcmRegistrationId, jsonPayload, (IEnumerable<string>) null);
    }

    public Task<GcmTemplateRegistrationDescription> CreateGcmTemplateRegistrationAsync(
      string gcmRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<GcmTemplateRegistrationDescription>(new GcmTemplateRegistrationDescription(gcmRegistrationId, jsonPayload, tags));
    }

    internal NokiaXRegistrationDescription CreateNokiaXNativeRegistration(
      string nokiaXRegistrationId,
      IEnumerable<string> tags)
    {
      return this.SyncOp<NokiaXRegistrationDescription>((Func<Task<NokiaXRegistrationDescription>>) (() => this.CreateNokiaXNativeRegistrationAsync(nokiaXRegistrationId, tags)));
    }

    internal NokiaXRegistrationDescription CreateNokiaXNativeRegistration(
      string nokiaXRegistrationId)
    {
      return this.CreateNokiaXNativeRegistration(nokiaXRegistrationId, (IEnumerable<string>) null);
    }

    internal Task<NokiaXRegistrationDescription> CreateNokiaXNativeRegistrationAsync(
      string nokiaXRegistrationId,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<NokiaXRegistrationDescription>(new NokiaXRegistrationDescription(nokiaXRegistrationId, tags));
    }

    internal Task<NokiaXRegistrationDescription> CreateNokiaXNativeRegistrationAsync(
      string nokiaXRegistrationId)
    {
      return this.CreateNokiaXNativeRegistrationAsync(nokiaXRegistrationId, (IEnumerable<string>) null);
    }

    internal Task<NokiaXTemplateRegistrationDescription> CreateNokiaXTemplateRegistrationAsync(
      string nokiaRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<NokiaXTemplateRegistrationDescription>(new NokiaXTemplateRegistrationDescription(nokiaRegistrationId, jsonPayload, tags));
    }

    internal Task<NokiaXTemplateRegistrationDescription> CreateNokiaXTemplateRegistrationAsync(
      string nokiaXRegistrationId,
      string jsonPayload)
    {
      return this.CreateNokiaXTemplateRegistrationAsync(nokiaXRegistrationId, jsonPayload, (IEnumerable<string>) null);
    }

    internal NokiaXTemplateRegistrationDescription CreateNokiaXTemplateRegistration(
      string nokiaXRegistrationId,
      string jsonPayload)
    {
      return this.CreateNokiaXTemplateRegistration(nokiaXRegistrationId, jsonPayload, (IEnumerable<string>) null);
    }

    internal NokiaXTemplateRegistrationDescription CreateNokiaXTemplateRegistration(
      string nokiaXRegistrationId,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SyncOp<NokiaXTemplateRegistrationDescription>((Func<Task<NokiaXTemplateRegistrationDescription>>) (() => this.CreateNokiaXTemplateRegistrationAsync(nokiaXRegistrationId, jsonPayload, tags)));
    }

    public Task<BaiduRegistrationDescription> CreateBaiduNativeRegistrationAsync(
      string userId,
      string channelId)
    {
      return this.CreateBaiduNativeRegistrationAsync(userId, channelId, (IEnumerable<string>) null);
    }

    public Task<BaiduRegistrationDescription> CreateBaiduNativeRegistrationAsync(
      string userId,
      string channelId,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<BaiduRegistrationDescription>(new BaiduRegistrationDescription(userId, channelId, tags));
    }

    internal BaiduRegistrationDescription CreateBaiduNativeRegistration(
      string userId,
      string channelId,
      IEnumerable<string> tags)
    {
      return this.SyncOp<BaiduRegistrationDescription>((Func<Task<BaiduRegistrationDescription>>) (() => this.CreateBaiduNativeRegistrationAsync(userId, channelId, tags)));
    }

    internal BaiduRegistrationDescription CreateBaiduNativeRegistration(
      string userId,
      string channelId)
    {
      return this.CreateBaiduNativeRegistration(userId, channelId, (IEnumerable<string>) null);
    }

    public Task<BaiduTemplateRegistrationDescription> CreateBaiduTemplateRegistrationAsync(
      string userId,
      string channelId,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<BaiduTemplateRegistrationDescription>(new BaiduTemplateRegistrationDescription(userId, channelId, jsonPayload, tags));
    }

    public Task<BaiduTemplateRegistrationDescription> CreateBaiduTemplateRegistrationAsync(
      string userId,
      string channelId,
      string jsonPayload)
    {
      return this.CreateBaiduTemplateRegistrationAsync(userId, channelId, jsonPayload, (IEnumerable<string>) null);
    }

    internal BaiduTemplateRegistrationDescription CreateBaiduTemplateRegistration(
      string userId,
      string channelId,
      string jsonPayload)
    {
      return this.CreateBaiduTemplateRegistration(userId, channelId, jsonPayload, (IEnumerable<string>) null);
    }

    internal BaiduTemplateRegistrationDescription CreateBaiduTemplateRegistration(
      string userId,
      string channelId,
      string jsonPayload,
      IEnumerable<string> tags)
    {
      return this.SyncOp<BaiduTemplateRegistrationDescription>((Func<Task<BaiduTemplateRegistrationDescription>>) (() => this.CreateBaiduTemplateRegistrationAsync(userId, channelId, jsonPayload, tags)));
    }

    internal MpnsRegistrationDescription CreateMpnsNativeRegistration(string channelUri) => this.CreateMpnsNativeRegistration(channelUri, (IEnumerable<string>) null);

    internal MpnsRegistrationDescription CreateMpnsNativeRegistration(
      string channelUri,
      IEnumerable<string> tags)
    {
      return this.SyncOp<MpnsRegistrationDescription>((Func<Task<MpnsRegistrationDescription>>) (() => this.CreateMpnsNativeRegistrationAsync(channelUri, tags)));
    }

    public Task<MpnsRegistrationDescription> CreateMpnsNativeRegistrationAsync(string channelUri) => this.CreateMpnsNativeRegistrationAsync(channelUri, (IEnumerable<string>) null);

    public Task<MpnsRegistrationDescription> CreateMpnsNativeRegistrationAsync(
      string channelUri,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<MpnsRegistrationDescription>(new MpnsRegistrationDescription(new Uri(channelUri), tags));
    }

    internal MpnsTemplateRegistrationDescription CreateMpnsTemplateRegistration(
      string channelUri,
      string xmlTemplate)
    {
      return this.CreateMpnsTemplateRegistration(channelUri, xmlTemplate, (IEnumerable<string>) null);
    }

    internal MpnsTemplateRegistrationDescription CreateMpnsTemplateRegistration(
      string channelUri,
      string xmlTemplate,
      IEnumerable<string> tags)
    {
      return this.SyncOp<MpnsTemplateRegistrationDescription>((Func<Task<MpnsTemplateRegistrationDescription>>) (() => this.CreateMpnsTemplateRegistrationAsync(channelUri, xmlTemplate, tags)));
    }

    public Task<MpnsTemplateRegistrationDescription> CreateMpnsTemplateRegistrationAsync(
      string channelUri,
      string xmlTemplate)
    {
      return this.CreateMpnsTemplateRegistrationAsync(channelUri, xmlTemplate, (IEnumerable<string>) null);
    }

    public Task<MpnsTemplateRegistrationDescription> CreateMpnsTemplateRegistrationAsync(
      string channelUri,
      string xmlTemplate,
      IEnumerable<string> tags)
    {
      return this.CreateRegistrationAsync<MpnsTemplateRegistrationDescription>(new MpnsTemplateRegistrationDescription(new Uri(channelUri), xmlTemplate, tags));
    }

    internal T CreateRegistration<T>(T registration) where T : RegistrationDescription => this.SyncOp<T>((Func<Task<T>>) (() => this.CreateRegistrationAsync<T>(registration)));

    public Task<T> CreateRegistrationAsync<T>(T registration) where T : RegistrationDescription
    {
      if (!string.IsNullOrWhiteSpace(registration.NotificationHubPath) && registration.NotificationHubPath != this.notificationHubPath)
        throw new ArgumentException("NotificationHubPath in RegistrationDescription is not valid.");
      return string.IsNullOrWhiteSpace(registration.RegistrationId) ? this.manager.CreateRegistrationAsync<T>(registration) : throw new ArgumentException("RegistrationId should be null or empty");
    }

    internal T UpdateRegistration<T>(T registration) where T : RegistrationDescription => this.SyncOp<T>((Func<Task<T>>) (() => this.UpdateRegistrationAsync<T>(registration)));

    public Task<T> UpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription
    {
      if (string.IsNullOrWhiteSpace(registration.RegistrationId))
        throw new ArgumentNullException("RegistrationId");
      return !string.IsNullOrWhiteSpace(registration.ETag) ? this.manager.UpdateRegistrationAsync<T>(registration) : throw new ArgumentNullException("ETag");
    }

    public Task<T> CreateOrUpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription => !string.IsNullOrWhiteSpace(registration.RegistrationId) ? this.manager.CreateOrUpdateRegistrationAsync<T>(registration) : throw new ArgumentNullException("RegistrationId");

    internal IEnumerable<RegistrationDescription> UpdateRegistrationsWithNewPnsHandle(
      string oldPnsHandle,
      string newPnsHandle)
    {
      return this.SyncOp<IEnumerable<RegistrationDescription>>((Func<Task<IEnumerable<RegistrationDescription>>>) (() => this.UpdateRegistrationsWithNewPnsHandleAsync(oldPnsHandle, newPnsHandle)));
    }

    internal Task<IEnumerable<RegistrationDescription>> UpdateRegistrationsWithNewPnsHandleAsync(
      string oldPnsHandle,
      string newPnsHandle)
    {
      return this.manager.UpdateRegistrationsWithNewPnsHandleAsync(oldPnsHandle, newPnsHandle);
    }

    internal TRegistrationDescription GetRegistration<TRegistrationDescription>(
      string registrationId)
      where TRegistrationDescription : RegistrationDescription
    {
      return this.SyncOp<TRegistrationDescription>((Func<Task<TRegistrationDescription>>) (() => this.GetRegistrationAsync<TRegistrationDescription>(registrationId)));
    }

    public Task<TRegistrationDescription> GetRegistrationAsync<TRegistrationDescription>(
      string registrationId)
      where TRegistrationDescription : RegistrationDescription
    {
      if (string.IsNullOrWhiteSpace(registrationId))
        throw new ArgumentNullException(nameof (registrationId));
      return this.manager.GetRegistrationAsync(registrationId).ContinueWith<TRegistrationDescription>((Func<Task<RegistrationDescription>, TRegistrationDescription>) (r => r.Result as TRegistrationDescription));
    }

    internal CollectionQueryResult<RegistrationDescription> GetAllRegistrations(int top) => this.SyncOp<CollectionQueryResult<RegistrationDescription>>((Func<Task<CollectionQueryResult<RegistrationDescription>>>) (() => this.manager.GetAllRegistrationsAsync(string.Empty, top)));

    internal CollectionQueryResult<RegistrationDescription> GetAllRegistrations(
      string continuationToken,
      int top)
    {
      return this.SyncOp<CollectionQueryResult<RegistrationDescription>>((Func<Task<CollectionQueryResult<RegistrationDescription>>>) (() => this.manager.GetAllRegistrationsAsync(continuationToken, top)));
    }

    public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(int top) => this.manager.GetAllRegistrationsAsync(string.Empty, top);

    public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(
      string continuationToken,
      int top)
    {
      return this.manager.GetAllRegistrationsAsync(continuationToken, top);
    }

    internal CollectionQueryResult<RegistrationDescription> GetRegistrationsByChannel(
      string pnsHandle,
      int top)
    {
      return this.GetRegistrationsByChannel(pnsHandle, string.Empty, top);
    }

    internal CollectionQueryResult<RegistrationDescription> GetRegistrationsByChannel(
      string pnsHandle,
      string continuationToken,
      int top)
    {
      return this.SyncOp<CollectionQueryResult<RegistrationDescription>>((Func<Task<CollectionQueryResult<RegistrationDescription>>>) (() => this.manager.GetRegistrationsByChannelAsync(pnsHandle, continuationToken, top)));
    }

    public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(
      string pnsHandle,
      int top)
    {
      return this.GetRegistrationsByChannelAsync(pnsHandle, string.Empty, top);
    }

    public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(
      string pnsHandle,
      string continuationToken,
      int top)
    {
      if (string.IsNullOrWhiteSpace(pnsHandle))
        throw new ArgumentNullException(nameof (pnsHandle));
      return this.manager.GetRegistrationsByChannelAsync(pnsHandle, continuationToken, top);
    }

    internal void DeleteRegistration(RegistrationDescription registration)
    {
      if (registration == null)
        throw new ArgumentNullException(nameof (registration));
      this.DeleteRegistration(registration.RegistrationId, registration.ETag);
    }

    public Task DeleteRegistrationAsync(RegistrationDescription registration)
    {
      if (registration == null)
        throw new ArgumentNullException(nameof (registration));
      return this.DeleteRegistrationAsync(registration.RegistrationId, registration.ETag);
    }

    internal void DeleteRegistration(string registrationId) => this.DeleteRegistration(registrationId, "*");

    internal void DeleteRegistration(string registrationId, string etag) => this.SyncOp((Func<Task>) (() => this.DeleteRegistrationAsync(registrationId, etag)));

    public Task DeleteRegistrationAsync(string registrationId) => this.DeleteRegistrationAsync(registrationId, "*");

    public Task DeleteRegistrationAsync(string registrationId, string etag)
    {
      if (string.IsNullOrWhiteSpace(registrationId))
        throw new ArgumentNullException(nameof (registrationId));
      return this.manager.DeleteRegistrationAsync(registrationId, etag);
    }

    internal void DeleteRegistrationsByChannel(string pnsHandle) => this.SyncOp((Func<Task>) (() => this.DeleteRegistrationsByChannelAsync(pnsHandle)));

    public Task DeleteRegistrationsByChannelAsync(string pnsHandle) => !string.IsNullOrWhiteSpace(pnsHandle) ? this.manager.DeleteRegistrationsByChannelAsync(pnsHandle) : throw new ArgumentNullException(nameof (pnsHandle));

    internal bool RegistrationExists(string registrationId) => this.SyncOp<bool>((Func<Task<bool>>) (() => this.RegistrationExistsAsync(registrationId)));

    public Task<bool> RegistrationExistsAsync(string registrationId) => this.manager.RegistrationExistsAsync(registrationId);

    internal CollectionQueryResult<RegistrationDescription> GetRegistrationsByTag(
      string tag,
      int top)
    {
      return this.GetRegistrationsByTag(tag, string.Empty, top);
    }

    public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(
      string tag,
      int top)
    {
      return this.GetRegistrationsByTagAsync(tag, string.Empty, top);
    }

    internal CollectionQueryResult<RegistrationDescription> GetRegistrationsByTag(
      string tag,
      string continuationToken,
      int top)
    {
      return this.SyncOp<CollectionQueryResult<RegistrationDescription>>((Func<Task<CollectionQueryResult<RegistrationDescription>>>) (() => this.GetRegistrationsByTagAsync(tag, continuationToken, top)));
    }

    public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(
      string tag,
      string continuationToken,
      int top)
    {
      if (string.IsNullOrWhiteSpace(tag))
        throw new ArgumentNullException(nameof (tag));
      return this.manager.GetRegistrationsByTagAsync(tag, continuationToken, top);
    }

    internal Task<RegistrationCounts> GetRegistrationCountsByTagAsync(string tag) => !string.IsNullOrWhiteSpace(tag) ? this.manager.GetRegistrationCountsByTagAsync(tag) : throw new ArgumentNullException(nameof (tag));

    internal Task<RegistrationCounts> GetRegistrationCountsAsync() => this.manager.GetRegistrationCountsAsync();

    public Task<ScheduledNotification> ScheduleNotificationAsync(
      Notification notification,
      DateTimeOffset scheduledTime)
    {
      return this.ScheduleNotificationAsyncInternal(notification, scheduledTime, string.Empty);
    }

    public Task<ScheduledNotification> ScheduleNotificationAsync(
      Notification notification,
      DateTimeOffset scheduledTime,
      IEnumerable<string> tags)
    {
      if (tags == null)
        throw new ArgumentNullException(nameof (tags));
      string tagExpression = tags.Count<string>() != 0 ? string.Join("||", tags) : throw new ArgumentException("tags argument should contain atleast one tag");
      return this.ScheduleNotificationAsyncInternal(notification, scheduledTime, tagExpression);
    }

    public Task<ScheduledNotification> ScheduleNotificationAsync(
      Notification notification,
      DateTimeOffset scheduledTime,
      string tagExpression)
    {
      return this.ScheduleNotificationAsyncInternal(notification, scheduledTime, tagExpression);
    }

    public Task CancelNotificationAsync(string scheduledNotificationId) => this.CancelNotificationAsyncInternal(scheduledNotificationId);

    public Task<NotificationOutcome> SendDirectNotificationAsync(
      Notification notification,
      string deviceHandle)
    {
      if (notification == null)
        throw new ArgumentNullException(nameof (notification));
      if (string.IsNullOrEmpty(deviceHandle))
        throw new ArgumentNullException(nameof (deviceHandle));
      return this.manager.SendDirectNotificationAsync(notification, deviceHandle);
    }

    public Task<NotificationOutcome> SendDirectNotificationAsync(
      Notification notification,
      IList<string> deviceHandles)
    {
      if (notification == null)
        throw new ArgumentNullException(nameof (notification));
      if (deviceHandles == null)
        throw new ArgumentNullException(nameof (deviceHandles));
      if (deviceHandles.Count == 0)
        throw new ArgumentException(nameof (deviceHandles));
      return this.manager.SendDirectNotificationAsync(notification, deviceHandles);
    }

    private T SyncOp<T>(Func<Task<T>> func)
    {
      try
      {
        return func().Result;
      }
      catch (AggregateException ex)
      {
        throw ex.Flatten().InnerException;
      }
    }

    private void SyncOp(Func<Task> action)
    {
      try
      {
        action().Wait();
      }
      catch (AggregateException ex)
      {
        throw ex.Flatten().InnerException;
      }
    }

    private Task<ScheduledNotification> ScheduleNotificationAsyncInternal(
      Notification notification,
      DateTimeOffset scheduledTime,
      string tagExpression)
    {
      if (notification == null)
        throw new ArgumentNullException(nameof (notification));
      return this.manager.ScheduleNotificationAsync(notification, scheduledTime, tagExpression);
    }

    private Task CancelNotificationAsyncInternal(string scheduledNotificationId)
    {
      if (string.IsNullOrWhiteSpace(scheduledNotificationId))
        throw new ArgumentNullException(nameof (scheduledNotificationId));
      if (scheduledNotificationId.Split('-').Length != 2)
        throw new ArgumentException(nameof (scheduledNotificationId));
      try
      {
        return this.manager.CancelScheduledNotificationAsync(scheduledNotificationId);
      }
      catch (UriFormatException ex)
      {
        throw new ArgumentException(nameof (scheduledNotificationId), (Exception) ex);
      }
    }

    public Uri GetBaseUri() => this.manager.baseUri;
  }
}
