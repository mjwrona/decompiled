// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.InstalledExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Extension;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components;
using Microsoft.VisualStudio.Services.ExtensionManagement.Server.Telemetry;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class InstalledExtensionService : 
    VssBaseService,
    IInstalledExtensionService,
    IVssFrameworkService
  {
    private static readonly ExtensionFile[] s_emptyFileList = Array.Empty<ExtensionFile>();
    public static readonly Guid m_processExtensionStateJobId = new Guid("F964D952-C431-484D-9975-5AD24C2AC43E");
    private Guid m_hostId;
    private ILockName m_messageBusLockName;
    private List<ServiceEvent> m_messageBusQueue = new List<ServiceEvent>();
    private bool m_messageBusTaskQueued;
    private TimeSpan m_messageBusTaskDelay;
    private TimeSpan m_directUpdateMessageBusTaskDelay;
    private ILockName m_versionCheckLockName;
    private int m_minVersionCheckTaskDelay;
    private int m_maxVersionCheckTaskDelay;
    private TimeSpan m_forceRecheckIntervalDays;
    private bool m_versionCheckTaskQueued;
    private ILockName m_stateDeletionLockName;
    private HashSet<ExtensionState> m_stateDeletionQueue = new HashSet<ExtensionState>();
    private bool m_stateDeletionTaskQueued;
    private int m_asyncVersionCheckDelay = 300;
    private TimeSpan m_stateDeletionTaskDelay;
    private List<ExtensionState> m_extensionStates;
    private ConcurrentDictionary<string, bool> m_extensionsAutoUpgrading = new ConcurrentDictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static List<PublishedExtensionWithState> s_emptyExtensionStates = new List<PublishedExtensionWithState>();
    private static readonly List<string> s_validInstallationTargets = new List<string>()
    {
      "Microsoft.VisualStudio.Services",
      "Microsoft.VisualStudio.Services.Cloud",
      "Microsoft.TeamFoundation.Server"
    };
    private static readonly TimeSpan s_recheckInterval = TimeSpan.FromMinutes(1.0);
    private static readonly HashSet<string> s_bypassDeletionCheck = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-build-agentcloud"
    };
    private const string c_serviceMustacheHandlerStart = "{{ServiceUrl";
    private const int c_defaultForceRecheckInDays = 28;
    private const int c_defaultVersionCheckTaskDelayInSeconds = 120;
    private const int c_defaultMaxVersionCheckTaskDelayInSeconds = 600;
    private const int c_defaultMessageBusTaskDelayInSeconds = 5;
    private const int c_defaultDirectUpdateMessageBusTaskDelayInSeconds = 3;
    private const int c_defaultStateDeletionTaskDelayInSeconds = 5;
    private const int c_maxApproversInExtensionEvents = 100;
    private const string c_installationIssuesMonikerFormat = "{0}_InstallationIssues";
    private const string c_sourceDemandIssue = "Demands";
    private const string c_TeamFoundationRequestContext = "IVssRequestContext";
    private const string s_area = "InstalledExtensionService";
    private const string s_layer = "Service";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_messageBusLockName = this.CreateLockName(requestContext, "messagebus");
      this.m_stateDeletionLockName = this.CreateLockName(requestContext, "stateDeletion");
      this.m_versionCheckLockName = this.CreateLockName(requestContext, "performVersionCheck");
      this.m_hostId = requestContext.ServiceHost.InstanceId;
      this.LoadConfiguration(requestContext);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<CachedRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), ExtensionManagementConstants.ExtensionManagementSettingsRoot + "/...");
      TeamFoundationSqlNotificationService service = requestContext.GetService<TeamFoundationSqlNotificationService>();
      service.RegisterNotification(requestContext, "Default", ExtensionManagementSqlNotificationClasses.InstalledExtensionChanged, new SqlNotificationHandler(this.OnExtensionStateChanged), requestContext.ExecutionEnvironment.IsHostedDeployment);
      service.RegisterNotification(vssRequestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), true);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      TeamFoundationSqlNotificationService service = requestContext.GetService<TeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "Default", ExtensionManagementSqlNotificationClasses.InstalledExtensionChanged, new SqlNotificationHandler(this.OnExtensionStateChanged), false);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      service.UnregisterNotification(vssRequestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false);
      vssRequestContext.GetService<CachedRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void LoadConfiguration(IVssRequestContext requestContext)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      this.m_messageBusTaskDelay = TimeSpan.FromSeconds((double) service.GetValue<uint>(requestContext, (RegistryQuery) ExtensionManagementConstants.ExtensionMessageBusDelay, true, 5U));
      this.m_directUpdateMessageBusTaskDelay = TimeSpan.FromSeconds((double) service.GetValue<uint>(requestContext, (RegistryQuery) ExtensionManagementConstants.DirectUpdateExtensionMessageBusDelay, true, 3U));
      this.m_stateDeletionTaskDelay = TimeSpan.FromSeconds((double) service.GetValue<uint>(requestContext, (RegistryQuery) ExtensionManagementConstants.ExtensionStateDeletionDelay, true, 5U));
      this.m_minVersionCheckTaskDelay = service.GetValue<int>(requestContext, (RegistryQuery) ExtensionManagementConstants.PerformVersionCheckDelay, true, 120);
      if (this.m_minVersionCheckTaskDelay < 1)
        this.m_minVersionCheckTaskDelay = 1;
      this.m_maxVersionCheckTaskDelay = service.GetValue<int>(requestContext, (RegistryQuery) ExtensionManagementConstants.MaxVersionCheckDelay, true, 600);
      if (this.m_maxVersionCheckTaskDelay <= this.m_minVersionCheckTaskDelay)
        this.m_maxVersionCheckTaskDelay = this.m_minVersionCheckTaskDelay + 2;
      this.m_forceRecheckIntervalDays = TimeSpan.FromDays((double) service.GetValue<uint>(requestContext, (RegistryQuery) ExtensionManagementConstants.ForceVersionCheckSpan, true, 28U));
      string str1 = service.GetValue<string>(requestContext, (RegistryQuery) ExtensionManagementConstants.BypassGalleryDeletionCheck, true, (string) null);
      if (!string.IsNullOrEmpty(str1))
      {
        string str2 = str1;
        char[] chArray = new char[1]{ ';' };
        foreach (string str3 in str2.Split(chArray))
          InstalledExtensionService.s_bypassDeletionCheck.Add(str3);
      }
      this.m_asyncVersionCheckDelay = Math.Abs(this.m_hostId.GetHashCode() % (this.m_maxVersionCheckTaskDelay - this.m_minVersionCheckTaskDelay)) + this.m_minVersionCheckTaskDelay;
    }

    internal List<PublishedExtensionWithState> GetExtensionStates(
      IVssRequestContext requestContext,
      bool includeInstallationIssues = false,
      bool allowAsyncVersionCheck = true,
      bool publishMessageBusEvents = true)
    {
      requestContext.TraceEnter(10013130, nameof (InstalledExtensionService), "Service", nameof (GetExtensionStates));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.Elevate();
        List<PublishedExtensionWithState> extensionStates = this.LoadExtensionStates(requestContext, includeInstallationIssues);
        int count = extensionStates.Count;
        Dictionary<string, ExtensionState> currentContextExtensionIds = new Dictionary<string, ExtensionState>();
        List<PublishedExtension> versionCheckNeeded = new List<PublishedExtension>();
        bool flag1 = !allowAsyncVersionCheck;
        bool flag2 = false;
        foreach (PublishedExtensionWithState extensionWithState in extensionStates)
        {
          PublishedExtension publishedExtension = extensionWithState.LatestPublishedExtension;
          ExtensionState extensionState = extensionWithState.ExtensionState;
          requestContext.Trace(10013465, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetExtensionStates:: Processing extensions looking for non-builtins: {0}.{1}", (object) extensionState.PublisherName, (object) extensionState.ExtensionName);
          string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extensionState.PublisherName, extensionState.ExtensionName);
          currentContextExtensionIds.Add(fullyQualifiedName, extensionState);
          if (!extensionState.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn))
          {
            try
            {
              requestContext.Trace(10013465, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetExtensionStates:: Processing extensions found non-builtin: {0}.{1}", (object) extensionState.PublisherName, (object) extensionState.ExtensionName);
              bool forceVersionCheck = false;
              if (!this.ShouldSkipExtensionAutoUpdateCheck(requestContext, extensionState))
              {
                if (this.IsVersionCheckNeeded(requestContext, extensionState, publishedExtension, out forceVersionCheck))
                {
                  requestContext.Trace(10013466, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Version check needed for non BuiltIn extensions: {0}.{1}", (object) extensionState.PublisherName, (object) extensionState.ExtensionName);
                  if (!flag1)
                    flag1 = forceVersionCheck;
                  if (publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.MultiVersion))
                  {
                    versionCheckNeeded.Add(publishedExtension);
                  }
                  else
                  {
                    string latestVersion = this.GetLatestVersion(publishedExtension.Versions);
                    if (!string.IsNullOrEmpty(latestVersion))
                    {
                      if (new Version(latestVersion).CompareTo(new Version(extensionState.Version)) > 0)
                      {
                        if (extensionState.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization))
                          requestContext.Trace(10013469, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Skipped updating version of non BuiltIn extension: {0}.{1}. Requires re-authorization", (object) extensionState.PublisherName, (object) extensionState.ExtensionName);
                        else if (this.m_extensionsAutoUpgrading.TryAdd(fullyQualifiedName, true))
                        {
                          try
                          {
                            IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
                            IContributionManifestService service = context.GetService<IContributionManifestService>();
                            bool flag3 = true;
                            IVssRequestContext requestContext1 = context;
                            string publisherName = extensionState.PublisherName;
                            string extensionName = extensionState.ExtensionName;
                            string version = latestVersion;
                            ExtensionManifest extensionManifest;
                            ref ExtensionManifest local = ref extensionManifest;
                            if (service.TryGetManifest(requestContext1, publisherName, extensionName, version, (string) null, out local))
                            {
                              ExtensionDemandsResolutionResult demandsResult = InstalledExtensionService.ValidateDemands(requestContext, extensionState.PublisherName, extensionState.ExtensionName, extensionManifest);
                              if (demandsResult.Status == DemandsResolutionStatus.Error)
                              {
                                requestContext.Trace(10013505, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Preventing auto-upgrade version of non BuiltIn extensions: {0}.{1}.  New version: {2}. Demands failing", (object) extensionState.PublisherName, (object) extensionState.ExtensionName, (object) latestVersion);
                                flag3 = false;
                                extensionState.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.AutoUpgradeError;
                                extensionState.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
                                this.ManageExtension(requestContext, true, publishedExtension, extensionState.Version, extensionState.Flags, new DateTime?(DateTime.UtcNow));
                                flag2 = true;
                                InstalledExtensionService.ScheduleTaskToSaveInstallationIssues(requestContext, extensionState.PublisherName, extensionState.ExtensionName, demandsResult);
                              }
                              else
                              {
                                extensionState.Flags &= ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.AutoUpgradeError;
                                if (demandsResult.Status == DemandsResolutionStatus.Success)
                                {
                                  extensionState.Flags &= ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
                                  InstalledExtensionService.ScheduleTaskToDeleteInstallationIssues(requestContext, extensionState.PublisherName, extensionState.ExtensionName);
                                }
                                else
                                {
                                  extensionState.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
                                  InstalledExtensionService.ScheduleTaskToSaveInstallationIssues(requestContext, extensionState.PublisherName, extensionState.ExtensionName, demandsResult);
                                }
                              }
                            }
                            if (flag3)
                            {
                              requestContext.Trace(10013467, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Updating version of non BuiltIn extensions: {0}.{1}.  New version: {2}", (object) extensionState.PublisherName, (object) extensionState.ExtensionName, (object) latestVersion);
                              this.ManageExtension(requestContext, true, publishedExtension, latestVersion, extensionState.Flags, new DateTime?(DateTime.UtcNow));
                              this.FireAuditEvent(requestContext, ExtensionUpdateType.VersionUpdated, extensionWithState.PublishedExtension, latestVersion, extensionState.Version);
                              flag2 = true;
                            }
                          }
                          finally
                          {
                            this.m_extensionsAutoUpgrading.TryRemove(fullyQualifiedName, out bool _);
                          }
                        }
                      }
                      else
                        this.ManageExtension(requestContext, false, publishedExtension, latestVersion, extensionState.Flags, new DateTime?(DateTime.UtcNow));
                    }
                  }
                }
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException(10013468, nameof (InstalledExtensionService), "Service", ex);
              extensionState.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
            }
          }
        }
        List<PublishedExtension> publishedExtensionList = vssRequestContext.GetService<IBuiltinExtensionService>().QueryBuiltInExtensions(vssRequestContext);
        if (publishedExtensionList.Count > 0)
        {
          foreach (PublishedExtension publishedExtension in publishedExtensionList)
          {
            requestContext.Trace(10013600, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetExtensionStates:: Processing builtin extensions: {0}.{1}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName);
            ExtensionState extensionState1;
            if (currentContextExtensionIds.TryGetValue(GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName), out extensionState1))
            {
              try
              {
                requestContext.Trace(10013600, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetExtensionStates:: Processing builtin extensions.  Already installed checking to see if version check needed: {0}.{1}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName);
                bool forceVersionCheck = false;
                if (this.IsVersionCheckNeeded(requestContext, extensionState1, publishedExtension, out forceVersionCheck))
                {
                  if (!flag1)
                    flag1 = forceVersionCheck;
                  if (publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.MultiVersion))
                  {
                    requestContext.Trace(10013133, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Version check needed for MultiVersion BuiltIn: {0}.{1}", (object) extensionState1.PublisherName, (object) extensionState1.ExtensionName);
                    extensionState1.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn;
                    versionCheckNeeded.Add(publishedExtension);
                  }
                  else
                  {
                    requestContext.Trace(10013134, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Version check needed for NonMultiVersion BuiltIn: {0}.{1}", (object) extensionState1.PublisherName, (object) extensionState1.ExtensionName);
                    string latestVersion = this.GetLatestVersion(publishedExtension.Versions);
                    if (extensionState1.Version != latestVersion)
                    {
                      extensionState1.Version = latestVersion;
                      this.ManageExtension(requestContext, true, publishedExtension, extensionState1.Version, extensionState1.Flags, new DateTime?(DateTime.UtcNow));
                    }
                  }
                }
                else
                  requestContext.Trace(10013605, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetExtensionStates:: Processing builtin extensions.  No version check needed: {0}.{1}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName);
              }
              catch (Exception ex)
              {
                requestContext.TraceException(10013470, nameof (InstalledExtensionService), "Service", ex);
                extensionState1.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
              }
            }
            else if (publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.MultiVersion))
            {
              flag1 = true;
              requestContext.Trace(10013475, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Adding new MultiVersion BuiltIn extension: {0}.{1}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName);
              versionCheckNeeded.Add(publishedExtension);
            }
            else
            {
              requestContext.Trace(10013476, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Adding new NonMultiVersion BuiltIn extension: {0}.{1}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName);
              ExtensionState extensionState2 = new ExtensionState();
              extensionState2.Version = this.GetLatestVersion(publishedExtension.Versions);
              extensionState2.Flags = Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn;
              extensionState2.LastUpdated = DateTime.UtcNow;
              try
              {
                this.ManageExtension(requestContext, true, publishedExtension, extensionState2.Version, extensionState2.Flags, new DateTime?(DateTime.UtcNow));
              }
              catch (Exception ex)
              {
                requestContext.TraceException(10013477, nameof (InstalledExtensionService), "Service", ex);
                extensionState2.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
              }
              extensionStates.Add(new PublishedExtensionWithState()
              {
                LatestPublishedExtension = publishedExtension,
                PublishedExtension = publishedExtension,
                ExtensionState = extensionState2
              });
            }
          }
        }
        if (versionCheckNeeded.Count > 0)
        {
          requestContext.Trace(10013478, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "{0} extensions need a MultiVersion check", (object) versionCheckNeeded.Count);
          if (((!requestContext.IsFeatureEnabled(ExtensionManagementFeatureFlags.AsyncVersionChecks) ? 0 : (!flag1 ? 1 : 0)) & (allowAsyncVersionCheck ? 1 : 0)) != 0)
          {
            this.EnqueueVersionCheck(requestContext);
          }
          else
          {
            this.PerformVersionChecks(requestContext, currentContextExtensionIds, versionCheckNeeded, false, publishMessageBusEvents);
            flag2 = true;
          }
        }
        if (flag2)
          extensionStates = this.LoadExtensionStates(requestContext, includeInstallationIssues);
        if (count != extensionStates.Count)
          requestContext.Trace(10013610, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetExtensionStates:: Returning different count of extensions: Started with: {0} Ended with: {1}", (object) count, (object) extensionStates.Count);
        else
          requestContext.Trace(10013611, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetExtensionStates:: Processed {0} states.", (object) count);
        return extensionStates;
      }
      finally
      {
        requestContext.TraceLeave(10013135, nameof (InstalledExtensionService), "Service", nameof (GetExtensionStates));
      }
    }

    private bool ShouldSkipExtensionAutoUpdateCheck(
      IVssRequestContext requestContext,
      ExtensionState extensionState)
    {
      bool flag = false;
      if (!requestContext.IsFeatureEnabled(ExtensionManagementFeatureFlags.DisableVSSReadConsistencyLevelExtensionUpdateFiltering) && this.IsVssReadConsistencyLevelEventual(requestContext))
      {
        flag = true;
        requestContext.Trace(10013471, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Skip checking extension " + extensionState.PublisherName + "." + extensionState.ExtensionName + " for update");
      }
      return flag;
    }

    private bool IsVssReadConsistencyLevelEventual(IVssRequestContext requestContext)
    {
      VssReadConsistencyLevel consistencyLevel;
      return !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.Items.TryGetValue<VssReadConsistencyLevel>("ReadConsistencyLevel", out consistencyLevel) && consistencyLevel == VssReadConsistencyLevel.Eventual;
    }

    private bool IsVersionCheckNeeded(
      IVssRequestContext requestContext,
      ExtensionState extensionState,
      PublishedExtension extension,
      out bool forceVersionCheck)
    {
      forceVersionCheck = false;
      DateTime? lastVersionCheck;
      int num1;
      if (extension != null && !extensionState.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled) && !extensionState.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.UnInstalled))
      {
        if (extensionState.LastVersionCheck.HasValue && extensionState.LastVersionCheck.Value.CompareTo(extension.LastUpdated) >= 0)
        {
          if (extensionState.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError))
          {
            DateTime utcNow = DateTime.UtcNow;
            lastVersionCheck = extensionState.LastVersionCheck;
            DateTime dateTime = lastVersionCheck.Value;
            num1 = utcNow - dateTime > InstalledExtensionService.s_recheckInterval ? 1 : 0;
          }
          else
            num1 = 0;
        }
        else
          num1 = 1;
      }
      else
        num1 = 0;
      if (num1 == 0)
        return num1 != 0;
      requestContext.Trace(10013515, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Version check needed for extensions: {0}.{1}", (object) extensionState.PublisherName, (object) extensionState.ExtensionName);
      ref bool local = ref forceVersionCheck;
      lastVersionCheck = extensionState.LastVersionCheck;
      int num2 = !lastVersionCheck.HasValue ? 1 : 0;
      local = num2 != 0;
      if (forceVersionCheck)
        return num1 != 0;
      if (!extensionState.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn))
        return num1 != 0;
      DateTime utcNow1 = DateTime.UtcNow;
      lastVersionCheck = extensionState.LastVersionCheck;
      DateTime dateTime1 = lastVersionCheck.Value;
      if (!(utcNow1 - dateTime1 > this.m_forceRecheckIntervalDays))
        return num1 != 0;
      forceVersionCheck = true;
      return num1 != 0;
    }

    private void PerformVersionCheckTask(IVssRequestContext requestContext, object taskArgs)
    {
      using (requestContext.Elevate().Lock(this.m_versionCheckLockName))
        this.m_versionCheckTaskQueued = false;
      this.GetExtensionStates(requestContext, allowAsyncVersionCheck: false);
    }

    internal virtual Dictionary<string, ExtensionState> PerformVersionChecks(
      IVssRequestContext requestContext,
      Dictionary<string, ExtensionState> currentContextExtensionIds,
      List<PublishedExtension> versionCheckNeeded,
      bool preview,
      bool publishMessageBusEvents = true)
    {
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      IContributionManifestService service1 = vssRequestContext1.GetService<IContributionManifestService>();
      object replacementValues = new object();
      Dictionary<string, ExtensionState> dictionary1 = new Dictionary<string, ExtensionState>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, List<PublishedExtension>> dictionary2 = new Dictionary<string, List<PublishedExtension>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      try
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          dictionary2.Add("onprem", versionCheckNeeded);
        }
        else
        {
          Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
          Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
          dictionary4.TryAdd<string, object>("IVssRequestContext", (object) requestContext);
          foreach (PublishedExtension publishedExtension in versionCheckNeeded)
          {
            try
            {
              string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName);
              ExtensionManifest extensionManifest;
              service1.TryGetManifest(vssRequestContext1, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, this.GetLatestVersion(publishedExtension.Versions), (string) null, out extensionManifest);
              if (extensionManifest != null && extensionManifest.EventCallbacks != null && extensionManifest.EventCallbacks.VersionCheck != null && !string.IsNullOrEmpty(extensionManifest.EventCallbacks.VersionCheck.Uri))
              {
                if (extensionManifest.EventCallbacks.VersionCheck.Uri.IndexOf("{{ServiceUrl") <= -1)
                {
                  try
                  {
                    string uri = extensionManifest.EventCallbacks.VersionCheck.Uri;
                    string templateUriProperty;
                    if (!dictionary3.TryGetValue(uri, out templateUriProperty))
                    {
                      templateUriProperty = extensionManifest.GetTemplateUriProperty(uri, replacementValues, PlatformMustacheExtensions.Parser, dictionary4);
                      dictionary3.Add(uri, templateUriProperty);
                    }
                    List<PublishedExtension> publishedExtensionList;
                    if (!dictionary2.TryGetValue(templateUriProperty, out publishedExtensionList))
                    {
                      publishedExtensionList = new List<PublishedExtension>();
                      dictionary2.Add(templateUriProperty, publishedExtensionList);
                    }
                    requestContext.Trace(10013490, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Version check callback url for extension: {0}.{1} is {2}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName, (object) templateUriProperty);
                    publishedExtensionList.Add(publishedExtension);
                    continue;
                  }
                  catch (Exception ex)
                  {
                    requestContext.Trace(10013261, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "Failed to resolve version check uri for extension: {0}.{1}   Template: {2}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName, (object) extensionManifest.EventCallbacks.VersionCheck.Uri);
                    requestContext.TraceException(10013262, nameof (InstalledExtensionService), "Service", ex);
                    continue;
                  }
                }
              }
              Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags extensionStateFlags = Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError | Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
              if (publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
                extensionStateFlags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn;
              if (publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Trusted))
                extensionStateFlags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Trusted;
              requestContext.Trace(10013495, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "Unable to find version check callback url for extension: {0}.{1}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName);
              ExtensionState extensionState1;
              if (extensionManifest != null && currentContextExtensionIds.TryGetValue(GalleryUtil.CreateFullyQualifiedName(publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName), out extensionState1))
                extensionStateFlags |= extensionState1.Flags;
              ExtensionState extensionState2 = new ExtensionState();
              extensionState2.PublisherName = publishedExtension.Publisher.PublisherName;
              extensionState2.ExtensionName = publishedExtension.ExtensionName;
              extensionState2.Version = (string) null;
              extensionState2.Flags = extensionStateFlags;
              extensionState2.LastVersionCheck = new DateTime?(DateTime.UtcNow);
              if (!dictionary1.TryAdd<string, ExtensionState>(fullyQualifiedName, extensionState2))
                requestContext.Trace(10013625, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "PerformVersionChecks::Extension being updated more than once: {0}. ", (object) fullyQualifiedName);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(10013480, nameof (InstalledExtensionService), "Service", ex);
            }
          }
        }
        ISupportedExtensionService service2 = vssRequestContext1.GetService<ISupportedExtensionService>();
        bool flag1 = false;
        foreach (string key in dictionary2.Keys)
        {
          IDictionary<string, SupportedExtension> versions = (IDictionary<string, SupportedExtension>) null;
          bool isCheckSuccessful;
          if (!service2.TryGetSupportedVersions(vssRequestContext1, key, out versions))
          {
            versions = service2.FetchSupportedVersions(requestContext, key, out isCheckSuccessful);
          }
          else
          {
            requestContext.Trace(10013263, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Supported versions found in cache for url: {0}", (object) key);
            isCheckSuccessful = true;
          }
          IVssRequestContext vssRequestContext2 = vssRequestContext1.Elevate();
          IPublishedExtensionCache service3 = vssRequestContext2.GetService<IPublishedExtensionCache>();
          foreach (PublishedExtension publishedExtension1 in dictionary2[key])
          {
            try
            {
              requestContext.Trace(10013495, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Checking for supported version for extension: {0}.{1}", (object) publishedExtension1.Publisher.PublisherName, (object) publishedExtension1.ExtensionName);
              bool flag2 = false;
              string str = (string) null;
              Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags extensionStateFlags = Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.None;
              string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publishedExtension1.Publisher.PublisherName, publishedExtension1.ExtensionName);
              ExtensionState extensionState3;
              if (!currentContextExtensionIds.TryGetValue(fullyQualifiedName, out extensionState3))
              {
                extensionState3 = new ExtensionState();
                extensionState3.PublisherName = publishedExtension1.Publisher.PublisherName;
                extensionState3.ExtensionName = publishedExtension1.ExtensionName;
                if (publishedExtension1.Flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
                  extensionState3.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn;
                if (publishedExtension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Trusted))
                  extensionState3.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Trusted;
              }
              else
              {
                str = extensionState3.Version;
                extensionStateFlags = extensionState3.Flags;
              }
              extensionState3.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.MultiVersion;
              SupportedExtension supportedExtension;
              if (versions.TryGetValue(GalleryUtil.CreateFullyQualifiedName(publishedExtension1.Publisher.PublisherName, publishedExtension1.ExtensionName), out supportedExtension))
              {
                requestContext.Trace(10013266, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Supported Version macthed Publisher and extension.  Checking version match: {0}.{1}", (object) supportedExtension.Publisher, (object) supportedExtension.Extension);
                Version result;
                if (Version.TryParse(supportedExtension.Version, out result))
                {
                  PublishedExtension publishedExtension2 = service3.GetPublishedExtension(vssRequestContext2, publishedExtension1.Publisher.PublisherName, publishedExtension1.ExtensionName, supportedExtension.Version);
                  if (publishedExtension2 != null && publishedExtension2.Versions != null && publishedExtension2.Versions.Count == 1)
                  {
                    if (string.Equals(publishedExtension2.Versions[0].Version, supportedExtension.Version, StringComparison.OrdinalIgnoreCase))
                    {
                      requestContext.Trace(10013500, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Found supported version for extension: {0}.{1}  verion: {2}", (object) publishedExtension1.Publisher.PublisherName, (object) publishedExtension1.ExtensionName, (object) result);
                      extensionState3.Flags &= ~(Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError | Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error);
                      extensionState3.Version = supportedExtension.Version;
                      flag2 = true;
                    }
                    else
                      requestContext.Trace(10013267, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "No matching version found.: {0}.{1} {2}", (object) publishedExtension1.Publisher.PublisherName, (object) publishedExtension1.ExtensionName, (object) supportedExtension.Version);
                  }
                  else
                  {
                    foreach (ExtensionVersion version in publishedExtension1.Versions)
                    {
                      requestContext.Trace(10013267, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Comparing Published Extension Version: {0}.{1} {2}", (object) publishedExtension1.Publisher.PublisherName, (object) publishedExtension1.ExtensionName, (object) version.Version);
                      if (string.Equals(version.Version, supportedExtension.Version, StringComparison.OrdinalIgnoreCase))
                      {
                        requestContext.Trace(10013500, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Found supported version for extension: {0}.{1}  verion: {2}", (object) publishedExtension1.Publisher.PublisherName, (object) publishedExtension1.ExtensionName, (object) result);
                        extensionState3.Flags &= ~(Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError | Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error);
                        extensionState3.Version = supportedExtension.Version;
                        flag2 = true;
                        break;
                      }
                    }
                  }
                }
                else
                  requestContext.Trace(10013269, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Unable to parse version for supported Version. {0}.{1} {2}", (object) supportedExtension.Publisher, (object) supportedExtension.Extension, (object) supportedExtension.Version);
              }
              else
                requestContext.Trace(10013270, TraceLevel.Info, nameof (InstalledExtensionService), "Service", " Supported Version not found in supported version map: {0}.{1}", (object) publishedExtension1.Publisher.PublisherName, (object) publishedExtension1.ExtensionName);
              if (!flag2)
              {
                requestContext.Trace(10013271, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "No supported version found for extension: {0}.{1}", (object) publishedExtension1.Publisher.PublisherName, (object) publishedExtension1.ExtensionName);
                extensionState3.Version = isCheckSuccessful ? (string) null : extensionState3.Version;
                extensionState3.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError | Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
              }
              ExtensionState extensionState4 = new ExtensionState();
              extensionState4.PublisherName = publishedExtension1.Publisher.PublisherName;
              extensionState4.ExtensionName = publishedExtension1.ExtensionName;
              extensionState4.Version = extensionState3.Version;
              extensionState4.Flags = extensionState3.Flags;
              extensionState4.LastVersionCheck = new DateTime?(DateTime.UtcNow);
              if (!dictionary1.TryAdd<string, ExtensionState>(fullyQualifiedName, extensionState4))
                requestContext.Trace(10013626, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "PerformVersionChecks::Extension being updated more than once: {0}. ", (object) fullyQualifiedName);
              if (!flag1 && (str == null || !str.Equals(extensionState4.Version) || extensionStateFlags != extensionState4.Flags))
                flag1 = true;
              this.PublishVersionCheckCiData(requestContext, extensionState4);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(10013485, nameof (InstalledExtensionService), "Service", ex);
            }
          }
        }
        if (!preview)
          this.ManageExtensions(requestContext, publishMessageBusEvents & flag1, dictionary1.Values.ToList<ExtensionState>());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013486, nameof (InstalledExtensionService), "Service", ex);
        throw;
      }
      return dictionary1;
    }

    private void PublishVersionCheckCiData(
      IVssRequestContext requestContext,
      ExtensionState extensionState)
    {
      if (extensionState == null)
        return;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("PublisherName", extensionState.PublisherName);
      properties.Add("ExtensionName", extensionState.ExtensionName);
      properties.Add("Version", extensionState.Version != null ? extensionState.Version.ToString() : string.Empty);
      properties.Add("Flags", (object) extensionState.Flags);
      properties.Add("LastVersionCheck", (object) extensionState.LastVersionCheck);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Contributions, ExtensionManagementCustomerIntelligenceFeature.VersionCheck, properties);
    }

    private string GetLatestVersion(List<ExtensionVersion> versions)
    {
      string latestVersion = (string) null;
      if (versions != null && versions.Count > 0)
        latestVersion = versions[0].Version;
      return latestVersion;
    }

    internal virtual List<PublishedExtensionWithState> LoadExtensionStates(
      IVssRequestContext requestContext,
      bool includeInstallationIssues = false)
    {
      requestContext.TraceEnter(10013140, nameof (InstalledExtensionService), "Service", "GetCachedStates");
      try
      {
        List<PublishedExtensionWithState> source;
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          source = InstalledExtensionService.s_emptyExtensionStates;
        }
        else
        {
          List<ExtensionState> extensionStateList;
          if ((extensionStateList = this.m_extensionStates) == null)
          {
            requestContext.Trace(10013480, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Extension states not cached. Loading from database.");
            using (InstalledExtensionComponent component = requestContext.CreateComponent<InstalledExtensionComponent>())
            {
              extensionStateList = component.QueryInstalledExtensions().GetCurrent<ExtensionState>().Items;
              this.m_extensionStates = extensionStateList;
            }
          }
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
          IPublishedExtensionCache service1 = vssRequestContext.GetService<IPublishedExtensionCache>();
          source = new List<PublishedExtensionWithState>();
          int count = extensionStateList.Count;
          foreach (ExtensionState extensionState1 in extensionStateList)
          {
            try
            {
              requestContext.Trace(10013465, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "LoadExtensionStates:: Processing extension state: {0}.{1}", (object) extensionState1.PublisherName, (object) extensionState1.ExtensionName);
              PublishedExtension publishedExtension1 = service1.GetPublishedExtension(vssRequestContext, extensionState1.PublisherName, extensionState1.ExtensionName, "latest");
              if (publishedExtension1 == null)
              {
                requestContext.Trace(10013469, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "LoadExtensionStates:: Extension does not exist in gallery {0}.{1}", (object) extensionState1.PublisherName, (object) extensionState1.ExtensionName);
                this.EnqueueStateDeletion(requestContext, extensionState1);
              }
              else if (publishedExtension1.Flags.HasFlag((Enum) PublishedExtensionFlags.Disabled))
              {
                requestContext.Trace(10013466, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "LoadExtensionStates:: Extension state is disabled in gallery {0}.{1}", (object) extensionState1.PublisherName, (object) extensionState1.ExtensionName);
              }
              else
              {
                PublishedExtension publishedExtension2 = (PublishedExtension) null;
                if (extensionState1.Version != null)
                {
                  publishedExtension2 = service1.GetPublishedExtension(vssRequestContext, extensionState1.PublisherName, extensionState1.ExtensionName, extensionState1.Version);
                  if (publishedExtension2 != null)
                  {
                    requestContext.Trace(10013467, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "LoadExtensionStates:: Extension version is not null.  Updating properties from PublishedExtension: {0}.{1} {2}", (object) extensionState1.PublisherName, (object) extensionState1.ExtensionName, (object) extensionState1.Version);
                    if (publishedExtension2.Flags.HasFlag((Enum) PublishedExtensionFlags.Trusted))
                      extensionState1.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Trusted;
                    Guid result1;
                    if (Guid.TryParse(publishedExtension2.GetProperty(extensionState1.Version, "RegistrationId"), out result1))
                    {
                      extensionState1.RegistrationId = result1;
                      if (!extensionState1.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Trusted))
                      {
                        string latestVersion = this.GetLatestVersion(publishedExtension1.Versions);
                        Guid result2;
                        if (this.CompareExtensionVersions(requestContext, extensionState1.Version, latestVersion) > 0 && Guid.TryParse(publishedExtension1.GetProperty(latestVersion, "RegistrationId"), out result2))
                        {
                          if (!result2.Equals(result1))
                          {
                            if (!extensionState1.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization))
                            {
                              requestContext.Trace(10013468, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "LoadExtensionStates:: Extension needs reauthorization. Updating flags: {0}.{1} {2} {3}", (object) extensionState1.PublisherName, (object) extensionState1.ExtensionName, (object) extensionState1.Version, (object) extensionState1.Flags);
                              extensionState1.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization;
                              this.ManageExtension(requestContext, true, publishedExtension1, extensionState1.Version, extensionState1.Flags, new DateTime?(DateTime.UtcNow));
                            }
                          }
                          else
                            extensionState1.Flags &= ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization;
                        }
                      }
                    }
                  }
                }
                source.Add(new PublishedExtensionWithState()
                {
                  LatestPublishedExtension = publishedExtension1,
                  PublishedExtension = publishedExtension2,
                  ExtensionState = new ExtensionState(extensionState1)
                });
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException(10013468, nameof (InstalledExtensionService), "Service", ex);
              ExtensionState extensionState2 = new ExtensionState(extensionState1);
              extensionState2.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
              source.Add(new PublishedExtensionWithState()
              {
                ExtensionState = extensionState2
              });
            }
          }
          if (includeInstallationIssues)
          {
            Dictionary<string, \u003C\u003Ef__AnonymousType0<ArtifactSpec, ExtensionState>> extensionsWithInstallationIssues = source.Where<PublishedExtensionWithState>((Func<PublishedExtensionWithState, bool>) (e => e.ExtensionState.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning))).ToDictionary((Func<PublishedExtensionWithState, string>) (k => InstalledExtensionService.GetInstallationIssuesArtifactMoniker(k.ExtensionState.PublisherName, k.ExtensionState.ExtensionName)), v => new
            {
              ArtifactSpec = InstalledExtensionService.GetInstallationIssuesArtifactSpec(v.ExtensionState.PublisherName, v.ExtensionState.ExtensionName),
              ExtensionState = v.ExtensionState
            });
            if (extensionsWithInstallationIssues.Count > 0)
              requestContext.TraceBlock(10013506, 10013507, nameof (InstalledExtensionService), "Service", "LoadExtensionStates.GetInstallationIssues", (Action) (() =>
              {
                ITeamFoundationPropertyService service2 = requestContext.GetService<ITeamFoundationPropertyService>();
                IEnumerable<ArtifactSpec> artifactSpecs1 = extensionsWithInstallationIssues.Values.Select(v => v.ArtifactSpec);
                IVssRequestContext requestContext1 = requestContext;
                IEnumerable<ArtifactSpec> artifactSpecs2 = artifactSpecs1;
                string[] propertyNameFilters = new string[1]
                {
                  "Demands"
                };
                using (TeamFoundationDataReader properties = service2.GetProperties(requestContext1, artifactSpecs2, (IEnumerable<string>) propertyNameFilters))
                {
                  foreach (ArtifactPropertyValue artifactPropertyValue in properties)
                  {
                    foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
                    {
                      if (propertyValue.PropertyName == "Demands")
                      {
                        try
                        {
                          List<InstalledExtensionStateIssue> extensionStateIssueList = JsonUtilities.Deserialize<List<InstalledExtensionStateIssue>>(propertyValue.Value?.ToString());
                          extensionsWithInstallationIssues[artifactPropertyValue.Spec.Moniker].ExtensionState.InstallationIssues = extensionStateIssueList;
                        }
                        catch (Exception ex)
                        {
                          requestContext.TraceException(10013508, nameof (InstalledExtensionService), "Service", ex);
                        }
                      }
                    }
                  }
                }
              }));
          }
          if (count != source.Count)
            requestContext.Trace(10013640, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "LoadExtensionStates:: Returning different count of extensions: Started with: {0} Ended with: {1}", (object) count, (object) source.Count);
        }
        return source;
      }
      finally
      {
        requestContext.TraceLeave(10013145, nameof (InstalledExtensionService), "Service", "GetCachedStates");
      }
    }

    internal int CompareExtensionVersions(
      IVssRequestContext requestContext,
      string previousVersion,
      string latestVersion)
    {
      if (!previousVersion.IsNullOrEmpty<char>() && !latestVersion.IsNullOrEmpty<char>())
        return Version.Parse(latestVersion).CompareTo(Version.Parse(previousVersion));
      requestContext.Trace(10013626, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "Either the previous or the latest extensions version is empty or not defined. Defaulting to reauthorization.");
      return 1;
    }

    public virtual InstalledExtension GetInstalledExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      IEnumerable<string> assetTypes = null,
      bool includeInstallationIssues = false)
    {
      requestContext.TraceEnter(10013150, nameof (InstalledExtensionService), "Service", nameof (GetInstalledExtension));
      try
      {
        return this.GetInstalledExtensions(requestContext, true, assetTypes: assetTypes, includeInstallationIssues: includeInstallationIssues).FirstOrDefault<InstalledExtension>((Func<InstalledExtension, bool>) (a => a.PublisherName.Equals(publisherName, StringComparison.OrdinalIgnoreCase) && a.ExtensionName.Equals(extensionName, StringComparison.OrdinalIgnoreCase))) ?? throw new InstalledExtensionNotFoundException(publisherName, extensionName);
      }
      finally
      {
        requestContext.TraceLeave(10013155, nameof (InstalledExtensionService), "Service", nameof (GetInstalledExtension));
      }
    }

    public List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      InstalledExtensionQuery query)
    {
      requestContext.TraceEnter(10013150, nameof (InstalledExtensionService), "Service", nameof (GetInstalledExtensions));
      try
      {
        ArgumentUtility.CheckForNull<InstalledExtensionQuery>(query, nameof (query));
        ArgumentUtility.CheckForNull<List<ExtensionIdentifier>>(query.Monikers, "query.Monikers");
        List<InstalledExtension> installedExtensions1 = new List<InstalledExtension>();
        if (query.Monikers.Count > 0)
        {
          List<InstalledExtension> installedExtensions2 = this.GetInstalledExtensions(requestContext, true, assetTypes: (IEnumerable<string>) query.AssetTypes);
          foreach (ExtensionIdentifier moniker1 in query.Monikers)
          {
            ExtensionIdentifier moniker = moniker1;
            if (!string.IsNullOrEmpty(moniker.PublisherName) && !string.IsNullOrEmpty(moniker.ExtensionName))
            {
              InstalledExtension installedExtension = installedExtensions2.FirstOrDefault<InstalledExtension>((Func<InstalledExtension, bool>) (a => a.PublisherName.Equals(moniker.PublisherName, StringComparison.OrdinalIgnoreCase) && a.ExtensionName.Equals(moniker.ExtensionName, StringComparison.OrdinalIgnoreCase)));
              if (installedExtension != null)
                installedExtensions1.Add(installedExtension);
            }
          }
        }
        return installedExtensions1;
      }
      finally
      {
        requestContext.TraceLeave(10013155, nameof (InstalledExtensionService), "Service", nameof (GetInstalledExtensions));
      }
    }

    public virtual List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      bool includeDisabledExtensions = false,
      bool includeErrors = false,
      IEnumerable<string> assetTypes = null,
      bool includeInstallationIssues = false)
    {
      requestContext.TraceEnter(10013160, nameof (InstalledExtensionService), "Service", "GetInstalledExtension");
      try
      {
        List<InstalledExtension> installedExtensions = new List<InstalledExtension>();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IContributionManifestService service = vssRequestContext.GetService<IContributionManifestService>();
        foreach (PublishedExtensionWithState extensionState1 in (IEnumerable<PublishedExtensionWithState>) this.GetExtensionStates(requestContext, includeInstallationIssues))
        {
          InstalledExtension installedExtension1 = (InstalledExtension) null;
          PublishedExtension publishedExtension = extensionState1.PublishedExtension;
          ExtensionState extensionState2 = extensionState1.ExtensionState;
          if (publishedExtension == null)
          {
            publishedExtension = extensionState1.LatestPublishedExtension;
            if (publishedExtension == null)
            {
              requestContext.Trace(10013614, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "GetInstalledExtensions:: Processing extension: {0}.{1}.  Did not find Published Extension.  Skipping extension.", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName);
              continue;
            }
          }
          requestContext.Trace(10013615, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetInstalledExtensions:: Processing extension: {0}.{1}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName);
          if (extensionState2.Version == null || extensionState2.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error))
          {
            requestContext.Trace(10013616, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetInstalledExtensions:: Processing extension: {0}.{1}. Version: {2}  Flags: {3}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
            if (includeErrors)
            {
              installedExtension1 = new InstalledExtension();
              extensionState2.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
            }
          }
          else if (includeDisabledExtensions || !extensionState2.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled))
          {
            requestContext.Trace(10013617, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetInstalledExtensions:: Processing extension: {0}.{1}. Version: {2}  Flags: {3}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
            ExtensionManifest extensionManifest;
            if (service.TryGetManifest(vssRequestContext, extensionState2.PublisherName, extensionState2.ExtensionName, extensionState2.Version, (string) null, out extensionManifest))
            {
              requestContext.Trace(10013136, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Loading contributions from extension: {0}.{1}#{2}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, (object) extensionState2.Version);
              InstalledExtension installedExtension2 = new InstalledExtension();
              installedExtension2.BaseUri = extensionManifest.BaseUri;
              installedExtension2.FallbackBaseUri = extensionManifest.FallbackBaseUri;
              installedExtension2.Constraints = extensionManifest.Constraints;
              installedExtension2.Contributions = extensionManifest.Contributions;
              installedExtension2.ContributionTypes = extensionManifest.ContributionTypes;
              installedExtension2.EventCallbacks = extensionManifest.EventCallbacks;
              installedExtension2.ManifestVersion = extensionManifest.ManifestVersion;
              installedExtension2.Scopes = extensionManifest.Scopes;
              installedExtension2.ServiceInstanceType = extensionManifest.ServiceInstanceType;
              installedExtension2.Demands = extensionManifest.Demands;
              installedExtension2.Licensing = extensionManifest.Licensing;
              installedExtension1 = installedExtension2;
            }
            else if (includeErrors)
            {
              requestContext.Trace(10013164, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "GetInstalledExtensions:: Processing extension: {0}.{1}.  Did not find manifest.  Adding to result in error state", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName);
              installedExtension1 = new InstalledExtension();
              extensionState2.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
            }
            else
              requestContext.Trace(10013617, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "GetInstalledExtensions:: Processing extension: {0}.{1}.  Did not find manifest.  Not adding to results", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName);
          }
          else
            requestContext.Trace(10013618, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "GetInstalledExtensions:: Processing extension: {0}.{1} Version: {2}  Flags: {3}. Not adding to results", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
          if ((object) installedExtension1 != null)
          {
            installedExtension1.PublisherName = publishedExtension.Publisher.PublisherName;
            installedExtension1.PublisherDisplayName = publishedExtension.Publisher.DisplayName;
            installedExtension1.ExtensionName = publishedExtension.ExtensionName;
            installedExtension1.ExtensionDisplayName = publishedExtension.DisplayName;
            installedExtension1.LastPublished = publishedExtension.LastUpdated;
            installedExtension1.Version = extensionState2.Version;
            installedExtension1.RegistrationId = extensionState2.RegistrationId;
            installedExtension1.Files = this.GetRequestedAssets(requestContext, publishedExtension, extensionState2.Version, assetTypes);
            installedExtension1.InstallState = new InstalledExtensionState()
            {
              Flags = extensionState2.Flags,
              LastUpdated = extensionState2.LastUpdated,
              InstallationIssues = extensionState2.InstallationIssues
            };
            if (extensionState2.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn))
              installedExtension1.Flags = ExtensionFlags.BuiltIn | ExtensionFlags.Trusted;
            if (extensionState2.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Trusted))
              installedExtension1.Flags |= ExtensionFlags.Trusted;
            installedExtensions.Add(installedExtension1);
          }
          else
            requestContext.Trace(10013619, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "GetInstalledExtensions:: Processing extension: {0}.{1} Version: {2}  Flags: {3}. Not adding to results", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
        }
        return installedExtensions;
      }
      finally
      {
        requestContext.TraceLeave(10013165, nameof (InstalledExtensionService), "Service", "GetInstalledExtension");
      }
    }

    public List<ExtensionState> GetInstalledExtensionStates(
      IVssRequestContext requestContext,
      bool includeDisabledExtensions,
      bool includeErrors,
      bool includeInstallationIssues = false,
      bool forceRefresh = false)
    {
      requestContext.TraceEnter(10013200, nameof (InstalledExtensionService), "Service", nameof (GetInstalledExtensionStates));
      try
      {
        List<ExtensionState> installedExtensionStates = new List<ExtensionState>();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IContributionManifestService service = vssRequestContext.GetService<IContributionManifestService>();
        foreach (PublishedExtensionWithState extensionState1 in this.GetExtensionStates(requestContext, includeInstallationIssues, !forceRefresh, !forceRefresh))
        {
          ExtensionState extensionState2 = extensionState1.ExtensionState;
          requestContext.Trace(10013620, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetInstalledExtensionStates:: Processing extension: {0}.{1}. Version: {2}  Flags: {3}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
          if (extensionState2.Version == null || extensionState2.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error))
          {
            if (includeErrors)
            {
              requestContext.Trace(10013623, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetInstalledExtensionStates:: Version null or error state, but included as error.  Adding to results: {0}.{1}. Version: {2}  Flags: {3}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
              extensionState2.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
              installedExtensionStates.Add(extensionState2);
            }
            else
              requestContext.Trace(10013624, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "GetInstalledExtensionStates:: Version null or error state.  Not added to results: {0}.{1}. Version: {2}  Flags: {3}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
          }
          else if (includeDisabledExtensions || !extensionState2.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled))
          {
            if (service.TryGetManifest(vssRequestContext, extensionState2.PublisherName, extensionState2.ExtensionName, extensionState2.Version, (string) null, out ExtensionManifest _))
            {
              requestContext.Trace(10013621, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "GetInstalledExtensionStates:: Manifest found.  Adding to results: {0}.{1}. Version: {2}  Flags: {3}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
              installedExtensionStates.Add(extensionState2);
            }
            else if (includeErrors)
            {
              requestContext.Trace(10013622, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "GetInstalledExtensionStates:: Manifest not found but including as error.  Adding to results: {0}.{1}. Version: {2}  Flags: {3}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
              extensionState2.Flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error;
              installedExtensionStates.Add(extensionState2);
            }
            else
              requestContext.Trace(10013625, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "GetInstalledExtensionStates::Manifest not found and not including errors: {0}.{1}. Version: {2}  Flags: {3}", (object) extensionState2.PublisherName, (object) extensionState2.ExtensionName, extensionState2.Version == null ? (object) "null" : (object) extensionState2.Version, (object) extensionState2.Flags);
          }
        }
        return installedExtensionStates;
      }
      finally
      {
        requestContext.TraceLeave(10013205, nameof (InstalledExtensionService), "Service", nameof (GetInstalledExtensionStates));
      }
    }

    public InstalledExtension InstallExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version = null)
    {
      requestContext.Trace(10013470, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Installing extension Id: {0}.{1}", (object) publisherName, (object) extensionName);
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.AlreadyInstallCheck"))
      {
        if (this.IsExtensionAlreadyInstalled(requestContext, publisherName, extensionName))
        {
          requestContext.Trace(10013470, TraceLevel.Error, nameof (InstalledExtensionService), "Service", "Extension already installed: {0}.{1}", (object) publisherName, (object) extensionName);
          throw new ExtensionAlreadyInstalledException(string.Format("{0}.{1}", (object) publisherName, (object) extensionName));
        }
      }
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.CheckPermission"))
        this.CheckPermission(requestContext);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string token;
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.GetToken"))
        token = requestContext.GetService<IAccountTokenService>().GetToken(requestContext);
      PublishedExtension publishedExtension;
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.LoadBuildInExtention"))
        publishedExtension = vssRequestContext.GetService<IGalleryService>().GetExtension(vssRequestContext, publisherName, extensionName, (string) null, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets, token);
      if (!string.IsNullOrEmpty(version) && publishedExtension.Versions != null)
      {
        bool flag = false;
        foreach (ExtensionVersion version1 in publishedExtension.Versions)
        {
          if (!string.IsNullOrEmpty(version1.Version) && version1.Version.Equals(version, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          publishedExtension = (PublishedExtension) null;
      }
      if (publishedExtension == null)
        throw new ExtensionDoesNotExistException(string.Format("{0}.{1}", (object) publisherName, (object) extensionName));
      publishedExtension.IsMarketExtension();
      this.CheckInstallationTarget(publishedExtension);
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags = Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.None;
      if (publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.MultiVersion))
      {
        version = (string) null;
        flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.MultiVersion;
        ExtensionState extensionState;
        if (this.PerformVersionChecks(requestContext, new Dictionary<string, ExtensionState>(), new List<PublishedExtension>()
        {
          publishedExtension
        }, true).TryGetValue(new ExtensionIdentifier(publisherName, extensionName).ToString(), out extensionState))
          version = extensionState.Version;
      }
      else
      {
        if (publishedExtension.Versions == null || publishedExtension.Versions.Count == 0)
        {
          requestContext.PublishAppInsightsPerExtensionTelemetryHelper(publishedExtension, CustomerIntelligenceActions.InstallError);
          throw new ExtensionDoesNotExistException(ExtensionResources.ExtensionDoesNotExistAtVersion());
        }
        if (version != null)
        {
          if (!publishedExtension.Versions.Any<ExtensionVersion>((Func<ExtensionVersion, bool>) (v => v.Version.Equals(version.ToString()))))
          {
            requestContext.PublishAppInsightsPerExtensionTelemetryHelper(publishedExtension, CustomerIntelligenceActions.InstallError);
            throw new ExtensionDoesNotExistException(ExtensionResources.ExtensionDoesNotExistAtVersion());
          }
        }
        else
          version = publishedExtension.Versions[0].Version;
      }
      if (publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
        flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn;
      Guid result = Guid.Empty;
      IExtensionEventCallbackService service1 = requestContext.GetService<IExtensionEventCallbackService>();
      IContributionManifestService service2 = vssRequestContext.GetService<IContributionManifestService>();
      ExtensionManifest extensionManifest;
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.TryGetManifest"))
        service2.TryGetManifest(vssRequestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, this.GetLatestVersion(publishedExtension.Versions), (string) null, out extensionManifest);
      ExtensionDemandsResolutionResult demandsResult;
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.ResolveDemands"))
        demandsResult = InstalledExtensionService.ValidateDemands(requestContext, publisherName, extensionName, extensionManifest, true);
      try
      {
        using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.AuthorizeDelegatedAuthApp"))
        {
          if (version != null)
          {
            string property = publishedExtension.GetProperty(version, "RegistrationId");
            requestContext.Trace(10013136, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "AuthorizeDelegatedAuthApp: {0}", (object) property);
            if (Guid.TryParse(property, out result))
            {
              if (result != Guid.Empty)
                this.AuthorizeDelegatedAuthApp(requestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, result);
            }
          }
        }
        using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.PerformPreEventCallbacks"))
          service1.PerformEventCallbacks(requestContext, publisherName, extensionName, version, extensionManifest, publishedExtension, ExtensionOperation.PreInstall, result);
      }
      catch
      {
        if (result != Guid.Empty)
        {
          try
          {
            this.RevokeDelegatedHostAuthorization(requestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, result);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10013222, nameof (InstalledExtensionService), "Service", ex);
          }
        }
        requestContext.PublishAppInsightsPerExtensionTelemetryHelper(publishedExtension, CustomerIntelligenceActions.InstallError);
        throw;
      }
      if (demandsResult.Status != DemandsResolutionStatus.Success)
      {
        flags |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
        InstalledExtensionService.SaveInstallationIssues(requestContext, publisherName, extensionName, demandsResult);
      }
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      bool failIfInstalled = publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public);
      ExtensionState extensionState1;
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.ManageExtension"))
        extensionState1 = this.ManageExtension(requestContext, true, publishedExtension, version, flags, processMessageNow: true, failIfInstalled: failIfInstalled, updatedBy: userIdentity.Id);
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.GetStates"))
        this.GetExtensionStates(requestContext, allowAsyncVersionCheck: false);
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.PerformPostEventCallback"))
        service1.PerformEventCallbacks(requestContext, publisherName, extensionName, version, extensionManifest, publishedExtension, ExtensionOperation.PostInstall, result);
      requestContext.Trace(10013475, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Installed extension Id: {0}.{1}", (object) publishedExtension.Publisher.PublisherName, (object) publishedExtension.ExtensionName);
      using (PerformanceTimer.StartMeasure(requestContext, "InstallExtension.FireALLEvents"))
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          requestContext.PublishAppInsightsPerExtensionTelemetryHelper(publishedExtension, CustomerIntelligenceActions.Install);
        this.FireExtensionChangeEvent(requestContext, publishedExtension, version, ExtensionUpdateType.Installed, true, true);
        this.FireAuditEvent(requestContext, ExtensionUpdateType.Installed, publishedExtension.Publisher.DisplayName, publishedExtension.DisplayName, version);
        this.FireCiInstallEvent(requestContext, publishedExtension, version);
        requestContext.GetService<IExtensionRequestService>().ResolveRequests(requestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, (string) null, (string) null, ExtensionRequestState.Accepted);
        PostInstallArgs taskArgs = new PostInstallArgs()
        {
          PublishedExtension = publishedExtension,
          Version = version,
          ActivityId = requestContext.ActivityId
        };
        vssRequestContext.GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.PostInstalltask), (object) taskArgs, 0));
      }
      PerformanceTimer.SendCustomerIntelligenceData(requestContext);
      return new InstalledExtension()
      {
        PublisherName = publishedExtension.Publisher.PublisherName,
        PublisherDisplayName = publishedExtension.Publisher.DisplayName,
        ExtensionName = publishedExtension.ExtensionName,
        ExtensionDisplayName = publishedExtension.DisplayName,
        LastPublished = publishedExtension.LastUpdated,
        Version = version,
        InstallState = new InstalledExtensionState()
        {
          Flags = extensionState1.Flags,
          LastUpdated = extensionState1.LastUpdated,
          InstallationIssues = InstalledExtensionService.ConvertToInstallationIssues(demandsResult.DemandIssues)
        }
      };
    }

    private void PostInstalltask(IVssRequestContext requestContext, object taskArgs)
    {
      PostInstallArgs postInstallArgs = taskArgs as PostInstallArgs;
      PublishedExtension publishedExtension = postInstallArgs.PublishedExtension;
      string version = postInstallArgs.Version;
      Guid activityId = postInstallArgs.ActivityId;
      postInstallArgs.ActivityId.ToString();
      this.PublishGalleryEvent(requestContext, "install", publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, version);
    }

    public void UpdateExtensionInstallCount(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IGalleryService>().UpdateExtensionInstallCount(vssRequestContext, publisherName, extensionName);
    }

    private void CheckInstallationTarget(PublishedExtension publishedExtension)
    {
      if (publishedExtension.InstallationTargets != null && publishedExtension.InstallationTargets.Count > 0)
      {
        foreach (InstallationTarget installationTarget in publishedExtension.InstallationTargets)
        {
          if (InstalledExtensionService.s_validInstallationTargets.Contains<string>(installationTarget.Target, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            return;
        }
        throw new InvalidInstallationTargetException(string.Join(", ", publishedExtension.InstallationTargets.Select<InstallationTarget, string>((Func<InstallationTarget, string>) (x => x.Target)).Distinct<string>()), publishedExtension.Publisher.PublisherName + "." + publishedExtension.ExtensionName);
      }
    }

    private static ExtensionDemandsResolutionResult ValidateDemands(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      ExtensionManifest extensionManifest,
      bool throwOnError = false)
    {
      ExtensionDemandsResolutionResult resolutionResult = requestContext.To(TeamFoundationHostType.Deployment).GetService<IExtensionDemandsResolutionService>().ResolveDemands(requestContext, publisherName, extensionName, extensionManifest, DemandsResolutionType.Installing);
      if (throwOnError && resolutionResult.Status == DemandsResolutionStatus.Error)
        throw new ExtensionDemandsNotSupportedException(publisherName, extensionName, resolutionResult.DemandIssues.Select<DemandIssue, string>((Func<DemandIssue, string>) (r => r.Message)));
      return resolutionResult;
    }

    public InstalledExtension UpdateExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags,
      string version)
    {
      requestContext.Trace(10013212, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Updating extension Id: {0}.{1} Flags: {2}, Version: {3}", (object) publisherName, (object) extensionName, (object) flags, (object) version);
      this.CheckPermission(requestContext);
      InstalledExtension installedExtension = this.GetInstalledExtension(requestContext, publisherName, extensionName, (IEnumerable<string>) null, false);
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags1 = installedExtension.InstallState.Flags;
      if (flags1.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn))
        throw new ArgumentException(ExtensionResources.CannotUpdateBuiltinExtension((object) installedExtension.ExtensionName));
      IExtensionEventCallbackService service = requestContext.GetService<IExtensionEventCallbackService>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (version != installedExtension.Version && !string.IsNullOrEmpty(version))
      {
        if (flags1.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization))
          throw new ArgumentException(ExtensionResources.CannotUpdateExtensionNeedingReauthorization((object) installedExtension.ExtensionName));
        Version version1 = new Version(installedExtension.Version);
        if (new Version(version).CompareTo(version1) > 0)
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
          ExtensionManifest extensionManifest;
          if (vssRequestContext.GetService<IContributionManifestService>().TryGetManifest(vssRequestContext, publisherName, extensionName, version, (string) null, out extensionManifest))
          {
            ExtensionDemandsResolutionResult demandsResult = InstalledExtensionService.ValidateDemands(requestContext, publisherName, extensionName, extensionManifest, true);
            Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags extensionStateFlags = flags1 & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.AutoUpgradeError;
            if (demandsResult.Status == DemandsResolutionStatus.Success)
            {
              flags1 = extensionStateFlags & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
              InstalledExtensionService.DeleteInstallationIssues(requestContext, publisherName, extensionName);
            }
            else
            {
              flags1 = extensionStateFlags | Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
              InstalledExtensionService.SaveInstallationIssues(requestContext, publisherName, extensionName, demandsResult);
              installedExtension.InstallState.InstallationIssues = InstalledExtensionService.ConvertToInstallationIssues(demandsResult.DemandIssues);
            }
          }
          this.ManageExtension(requestContext, true, installedExtension.PublisherName, installedExtension.ExtensionName, version, flags1, directMessageUpdate: true, updatedBy: userIdentity.Id);
          installedExtension.Version = version;
          installedExtension.InstallState.Flags = flags1;
        }
      }
      if (flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled) && !flags1.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled))
      {
        flags1 |= Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled;
        this.ManageExtension(requestContext, true, installedExtension.PublisherName, installedExtension.ExtensionName, installedExtension.Version, flags1, directMessageUpdate: true, updatedBy: userIdentity.Id);
        service.PerformEventCallbacks(requestContext, publisherName, extensionName, installedExtension.Version, (ExtensionManifest) installedExtension, (PublishedExtension) null, ExtensionOperation.PostDisable, installedExtension.RegistrationId);
        installedExtension.InstallState.Flags = flags1;
        this.FireExtensionChangeEvent(requestContext, installedExtension.PublisherName, installedExtension.ExtensionName, installedExtension.Version, ExtensionUpdateType.Disabled, true, true);
        this.FireAuditEvent(requestContext, ExtensionUpdateType.Disabled, installedExtension);
      }
      if (!flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled) && flags1.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled))
      {
        ExtensionDemandsResolutionResult demandsResult = InstalledExtensionService.ValidateDemands(requestContext, installedExtension.PublisherName, installedExtension.ExtensionName, (ExtensionManifest) installedExtension, true);
        Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags extensionStateFlags = flags1 & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled;
        Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags2;
        if (demandsResult.Status == DemandsResolutionStatus.Success)
        {
          flags2 = extensionStateFlags & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
          if (!flags2.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.AutoUpgradeError))
            InstalledExtensionService.DeleteInstallationIssues(requestContext, publisherName, extensionName);
        }
        else
        {
          flags2 = extensionStateFlags | Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
          if (!flags2.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.AutoUpgradeError))
            InstalledExtensionService.SaveInstallationIssues(requestContext, publisherName, extensionName, demandsResult);
          installedExtension.InstallState.InstallationIssues = InstalledExtensionService.ConvertToInstallationIssues(demandsResult.DemandIssues);
        }
        this.ManageExtension(requestContext, true, installedExtension.PublisherName, installedExtension.ExtensionName, installedExtension.Version, flags2, directMessageUpdate: true, updatedBy: userIdentity.Id);
        service.PerformEventCallbacks(requestContext, publisherName, extensionName, installedExtension.Version, (ExtensionManifest) installedExtension, (PublishedExtension) null, ExtensionOperation.PostEnable, installedExtension.RegistrationId);
        installedExtension.InstallState.Flags = flags2;
        this.FireExtensionChangeEvent(requestContext, installedExtension.PublisherName, installedExtension.ExtensionName, installedExtension.Version, ExtensionUpdateType.Enabled, true, true);
        this.FireAuditEvent(requestContext, ExtensionUpdateType.Enabled, installedExtension);
      }
      CustomerIntelligenceData data = new CustomerIntelligenceData();
      data.Add(CustomerIntelligenceProperty.Action, CustomerIntelligenceActions.Update);
      data.Add("PublisherName", publisherName);
      data.Add("ExtensionName", extensionName);
      data.Add("Version", installedExtension.Version != null ? installedExtension.Version.ToString() : string.Empty);
      data.Add("Enabled", !flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled));
      this.PublishCustomerIntelligenceEvent(requestContext, CustomerIntelligenceActions.Update, data, true);
      return installedExtension;
    }

    public ExtensionAuthorization AuthorizeExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid registrationId)
    {
      ExtensionAuthorization extensionAuthorization = new ExtensionAuthorization();
      extensionAuthorization.Id = registrationId;
      this.CheckPermission(requestContext);
      InstalledExtension installedExtension = this.GetInstalledExtension(requestContext, publisherName, extensionName, (IEnumerable<string>) null, false);
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags1 = installedExtension.InstallState.Flags;
      if (flags1.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Trusted) || !this.AuthorizeDelegatedAuthApp(requestContext, publisherName, extensionName, registrationId))
        throw new ArgumentException(ExtensionResources.CannotAuthorizeTrustedExtension());
      string forRegistrationId = this.GetVersionForRegistrationId(requestContext, publisherName, extensionName, registrationId);
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags2 = flags1 & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      this.ManageExtension(requestContext, true, installedExtension.PublisherName, installedExtension.ExtensionName, forRegistrationId, flags2, directMessageUpdate: true, updatedBy: userIdentity.Id);
      return extensionAuthorization;
    }

    internal virtual ExtensionState ManageExtension(
      IVssRequestContext requestContext,
      bool publishMessageBusEvent,
      PublishedExtension publishedExtension,
      string version,
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags,
      DateTime? lastVersionCheck = null,
      bool processMessageNow = false,
      bool failIfInstalled = false,
      Guid updatedBy = default (Guid))
    {
      return this.ManageExtension(requestContext, publishMessageBusEvent, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, version, flags, lastVersionCheck, processMessageNow, failIfInstalled, updatedBy);
    }

    private IEnumerable<ExtensionFile> GetRequestedAssets(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      string version,
      IEnumerable<string> assetTypes)
    {
      IEnumerable<ExtensionFile> requestedAssets = (IEnumerable<ExtensionFile>) InstalledExtensionService.s_emptyFileList;
      if (assetTypes != null)
      {
        List<ExtensionFile> extensionFileList = new List<ExtensionFile>();
        HashSet<string> stringSet = new HashSet<string>(assetTypes, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        bool flag = stringSet.Contains("*");
        foreach (ExtensionVersion version1 in publishedExtension.Versions)
        {
          if (version1.Version.Equals(version))
          {
            using (List<ExtensionFile>.Enumerator enumerator = version1.Files.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                ExtensionFile current = enumerator.Current;
                if (flag || stringSet.Contains(current.AssetType))
                  extensionFileList.Add(current);
              }
              break;
            }
          }
        }
        requestedAssets = (IEnumerable<ExtensionFile>) extensionFileList;
      }
      return requestedAssets;
    }

    internal virtual void ManageExtensions(
      IVssRequestContext requestContext,
      bool publishMessageBusEvent,
      List<ExtensionState> states)
    {
      using (InstalledExtensionComponent component = requestContext.CreateComponent<InstalledExtensionComponent>())
        component.InstallExtensions((IEnumerable<ExtensionState>) states);
      this.RefreshCache(requestContext);
      if (!publishMessageBusEvent)
        return;
      bool flag = false;
      foreach (InstalledExtensionState state in states)
      {
        if (!state.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return;
      InstalledExtensionMessage message1 = new InstalledExtensionMessage()
      {
        ChangeType = Microsoft.VisualStudio.Services.Extension.InstalledExtensionMessageChangeType.Installed,
        HostId = requestContext.ServiceHost.InstanceId,
        StateFlags = Microsoft.VisualStudio.Services.Extension.ExtensionStateFlags.None
      };
      ServiceEvent message2 = this.CreateMessage(requestContext, "extension.bulkinstall", message1);
      this.EnqueueCacheInvalidation(requestContext, message2);
    }

    private ExtensionState ManageExtension(
      IVssRequestContext requestContext,
      bool publishMessageBusEvent,
      string publisherName,
      string extensionName,
      string version,
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags,
      DateTime? lastVersionCheck = null,
      bool directMessageUpdate = false,
      bool failIfInstalled = false,
      Guid updatedBy = default (Guid))
    {
      requestContext.TraceEnter(10013210, nameof (InstalledExtensionService), "Service", nameof (ManageExtension));
      try
      {
        bool flag = !flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled);
        requestContext.Trace(10013211, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Installing extension Id: {0}.{1} Version: {2} Enabled: {3}", (object) publisherName, (object) extensionName, version != null ? (object) version.ToString() : (object) string.Empty, (object) flag);
        ExtensionState extensionState = (ExtensionState) null;
        string previousVersion;
        Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags? previousFlags;
        using (InstalledExtensionComponent component = requestContext.CreateComponent<InstalledExtensionComponent>())
          extensionState = component.InstallExtension(version, flags, lastVersionCheck, publisherName, extensionName, failIfInstalled, updatedBy, out previousVersion, out previousFlags);
        requestContext.Trace(10013212, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Installed extension Id: {0}.{1} Version: {2} Enabled: {3}", (object) publisherName, (object) extensionName, version != null ? (object) version.ToString() : (object) string.Empty, (object) flag);
        this.RefreshCache(requestContext);
        if (publishMessageBusEvent)
        {
          InstalledExtensionMessage installEventData = this.GetInstallEventData(requestContext, publisherName, extensionName, version, flags);
          if (requestContext.IsFeatureEnabled(ExtensionManagementFeatureFlags.UseJobForInstallEvents))
          {
            XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) installEventData);
            requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, string.Format("Publish extension event: {0}.{1}", (object) publisherName, (object) extensionName), ExtensionManagementJobs.PublishExtensionEventJob, xml, JobPriorityLevel.Normal, this.m_directUpdateMessageBusTaskDelay);
          }
          else
          {
            ServiceEvent message = this.CreateMessage(requestContext, "extension.install", installEventData);
            this.EnqueueCacheInvalidation(requestContext, message, directMessageUpdate);
          }
        }
        if (!string.IsNullOrEmpty(previousVersion))
        {
          if (previousFlags.HasValue)
          {
            if (!previousFlags.Value.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization) && flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization))
              this.ScheduleTaskToFireNotificationEvent(requestContext, publisherName, extensionName, version, ExtensionUpdateType.ActionRequired, false, false);
            else if (previousFlags.Value.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization) && !flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization))
              this.ScheduleTaskToFireNotificationEvent(requestContext, publisherName, extensionName, version, ExtensionUpdateType.ActionResolved, false, false);
          }
          if (!string.Equals(previousVersion, version) && !flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn))
            this.ScheduleTaskToFireNotificationEvent(requestContext, publisherName, extensionName, version, ExtensionUpdateType.VersionUpdated, false, false);
        }
        return extensionState;
      }
      finally
      {
        requestContext.TraceLeave(10013215, nameof (InstalledExtensionService), "Service", nameof (ManageExtension));
      }
    }

    public void UninstallExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string reason = null,
      string reasonCode = null)
    {
      requestContext.TraceEnter(10013220, nameof (InstalledExtensionService), "Service", nameof (UninstallExtension));
      try
      {
        requestContext.Trace(10013122, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "UninstallExtension: {0}.{1}", (object) publisherName, (object) extensionName);
        this.CheckPermission(requestContext);
        InstalledExtension installedExtension = (InstalledExtension) null;
        try
        {
          installedExtension = this.GetInstalledExtension(requestContext, publisherName, extensionName, (IEnumerable<string>) null, false);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013220, nameof (InstalledExtensionService), "Service", ex);
        }
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        IPublishedExtensionCache service = context.GetService<IPublishedExtensionCache>();
        string version1 = installedExtension == null ? "latest" : installedExtension.Version;
        IVssRequestContext requestContext1 = context;
        string publisherName1 = publisherName;
        string extensionName1 = extensionName;
        string version2 = version1;
        PublishedExtension publishedExtension = service.GetPublishedExtension(requestContext1, publisherName1, extensionName1, version2);
        Guid result;
        if (publishedExtension != null && installedExtension != null && installedExtension.Version != null && Guid.TryParse(publishedExtension.GetProperty(installedExtension.Version, "RegistrationId"), out result) && result != Guid.Empty)
        {
          requestContext.GetService<IExtensionEventCallbackService>().PerformEventCallbacks(requestContext, publisherName, extensionName, installedExtension.Version, (ExtensionManifest) installedExtension, publishedExtension, ExtensionOperation.PreUninstall, result);
          try
          {
            this.RevokeDelegatedHostAuthorization(requestContext, publishedExtension.Publisher.PublisherName, publishedExtension.ExtensionName, result);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10013221, nameof (InstalledExtensionService), "Service", ex);
          }
        }
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        using (InstalledExtensionComponent component = requestContext.CreateComponent<InstalledExtensionComponent>())
          component.UninstallExtension(publisherName, extensionName, userIdentity.Id);
        if (installedExtension != null)
          requestContext.GetService<IExtensionEventCallbackService>().PerformEventCallbacks(requestContext, publisherName, extensionName, installedExtension.Version, (ExtensionManifest) installedExtension, publishedExtension, ExtensionOperation.PostUninstall, installedExtension.RegistrationId);
        this.RefreshCache(requestContext);
        try
        {
          ServiceEvent message = this.CreateMessage(requestContext, "extension.uninstall", this.GetUninstallEventData(requestContext, publisherName, extensionName));
          this.EnqueueCacheInvalidation(requestContext, message, true);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013123, nameof (InstalledExtensionService), "Service", ex);
        }
        this.PublishGalleryEvent(requestContext, "uninstall", publisherName, extensionName, version1, reason, reasonCode);
        CustomerIntelligenceData data = new CustomerIntelligenceData();
        data.Add(CustomerIntelligenceProperty.Action, CustomerIntelligenceActions.Uninstall);
        data.Add("PublisherName", publisherName);
        data.Add("ExtensionName", extensionName);
        data.Add("Reason", reason != null ? reason : string.Empty);
        data.Add("ReasonCode", reasonCode != null ? reasonCode : string.Empty);
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          string action = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName) + "/" + CustomerIntelligenceActions.Uninstall;
          requestContext.PublishAppInsightsPerExtensionTelemetryIncrement(action, false);
        }
        this.PublishCustomerIntelligenceEvent(requestContext, CustomerIntelligenceActions.Uninstall, data, true);
        this.FireAuditEvent(requestContext, ExtensionUpdateType.Uninstalled, publishedExtension?.Publisher?.DisplayName ?? publisherName, publishedExtension?.DisplayName ?? extensionName);
        this.FireExtensionChangeEvent(requestContext, publishedExtension, installedExtension?.Version, ExtensionUpdateType.Uninstalled, true, true);
      }
      finally
      {
        requestContext.TraceLeave(10013225, nameof (InstalledExtensionService), "Service", nameof (UninstallExtension));
      }
    }

    private void PublishGalleryEvent(
      IVssRequestContext requestContext,
      string extensionEventType,
      string publisherName,
      string extensionName,
      string version,
      string reason = null,
      string reasonCode = null)
    {
      try
      {
        ExtensionEvents extensionEvents = new ExtensionEvents();
        extensionEvents.PublisherName = publisherName;
        extensionEvents.ExtensionName = extensionName;
        extensionEvents.Events = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent>>) new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent>>();
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        string name = requestContext.ServiceHost.Name;
        List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent> extensionEventList = new List<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent>();
        Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent extensionEvent = new Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent()
        {
          Version = version,
          StatisticDate = DateTime.UtcNow,
          Properties = new JObject()
        };
        extensionEvent.Properties.Add("hostId", (JToken) instanceId);
        extensionEvent.Properties.Add("hostName", (JToken) name);
        extensionEvent.Properties.Add("vsid", (JToken) requestContext.GetUserId());
        extensionEvent.Properties.Add("reasonText", (JToken) reason);
        extensionEvent.Properties.Add(nameof (reasonCode), (JToken) reasonCode);
        extensionEventList.Add(extensionEvent);
        extensionEvents.Events.Add(extensionEventType, (IEnumerable<Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionEvent>) extensionEventList);
        IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IGalleryService>().PublishExtensionEvents(vssRequestContext, (IEnumerable<ExtensionEvents>) new List<ExtensionEvents>()
        {
          extensionEvents
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013220, nameof (InstalledExtensionService), "Service", ex);
      }
    }

    private List<ServiceEvent> CreateMessages(
      IVssRequestContext requestContext,
      string eventType,
      List<ExtensionState> states)
    {
      List<ServiceEvent> messages = new List<ServiceEvent>();
      foreach (ExtensionState state in states)
      {
        if (!state.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError))
          messages.Add(this.CreateMessage(requestContext, eventType, this.GetInstallEventData(requestContext, state.PublisherName, state.ExtensionName, state.Version, state.Flags)));
      }
      return messages;
    }

    internal ServiceEvent CreateMessage(
      IVssRequestContext requestContext,
      string eventType,
      InstalledExtensionMessage message)
    {
      return new ServiceEvent()
      {
        EventType = eventType,
        Publisher = new Microsoft.VisualStudio.Services.WebApi.Publisher()
        {
          Name = "Extension",
          ServiceOwnerId = ExtensionConstants.ServiceOwner
        },
        Resource = (object) message,
        ResourceVersion = "1.0-preview.1",
        ResourceContainers = this.GetResourceContainers(requestContext)
      };
    }

    private InstalledExtensionMessage GetInstallEventData(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags flags)
    {
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags extensionStateFlags = flags & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.NeedsReauthorization & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.AutoUpgradeError & ~Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning;
      return new InstalledExtensionMessage()
      {
        ChangeType = Microsoft.VisualStudio.Services.Extension.InstalledExtensionMessageChangeType.Installed,
        HostId = requestContext.ServiceHost.InstanceId,
        PublisherName = publisherName,
        ExtensionName = extensionName,
        Version = version != null ? new Version(version) : (Version) null,
        StateFlags = (Microsoft.VisualStudio.Services.Extension.ExtensionStateFlags) extensionStateFlags
      };
    }

    private InstalledExtensionMessage GetUninstallEventData(
      IVssRequestContext requestContext,
      InstalledExtension installedExtension)
    {
      return this.GetUninstallEventData(requestContext, installedExtension.PublisherName, installedExtension.ExtensionName);
    }

    private InstalledExtensionMessage GetUninstallEventData(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      return new InstalledExtensionMessage()
      {
        ChangeType = Microsoft.VisualStudio.Services.Extension.InstalledExtensionMessageChangeType.Uninstalled,
        HostId = requestContext.ServiceHost.InstanceId,
        PublisherName = publisherName,
        ExtensionName = extensionName
      };
    }

    private Dictionary<string, object> GetResourceContainers(IVssRequestContext requestContext)
    {
      Dictionary<string, object> resourceContainers = new Dictionary<string, object>();
      IVssServiceHost organizationServiceHost = requestContext.ServiceHost.OrganizationServiceHost;
      resourceContainers.Add("Account", (object) (organizationServiceHost == null || !organizationServiceHost.IsOnly(TeamFoundationHostType.Application) ? Guid.Empty : organizationServiceHost.InstanceId));
      return resourceContainers;
    }

    private bool AuthorizeDelegatedAuthApp(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid registrationId)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        HostAuthorizationDecision authorizationDecision = vssRequestContext.GetService<IDelegatedAuthorizationService>().AuthorizeHost(vssRequestContext, registrationId);
        if (authorizationDecision != null)
        {
          if (!authorizationDecision.HasError)
          {
            requestContext.GetService<IExtensionHostAuthorizationService>().SetHostAuthorization(requestContext, publisherName, extensionName, authorizationDecision.HostAuthorizationId);
            return true;
          }
        }
      }
      catch (HostAuthorizationCreateException ex)
      {
        requestContext.TraceException(10013240, nameof (InstalledExtensionService), "Service", (Exception) ex);
        throw;
      }
      return false;
    }

    private void RevokeDelegatedHostAuthorization(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid registrationId)
    {
      requestContext.Trace(10013137, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "RevokeDelegatedHostAuthorization: {0}", (object) registrationId);
      requestContext.To(TeamFoundationHostType.Application);
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IDelegatedAuthorizationService>().RevokeHostAuthorization(vssRequestContext, registrationId, new Guid?(requestContext.ServiceHost.InstanceId));
      requestContext.GetService<IExtensionHostAuthorizationService>().RemoveHostAuthorization(requestContext, publisherName, extensionName);
    }

    private string GetVersionForRegistrationId(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid registrationId)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IGalleryService service = context.GetService<IGalleryService>();
      ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.ExcludeNonValidated;
      IVssRequestContext requestContext1 = context;
      string publisherName1 = publisherName;
      string extensionName1 = extensionName;
      int flags = (int) extensionQueryFlags;
      PublishedExtension extension = service.GetExtension(requestContext1, publisherName1, extensionName1, (string) null, (ExtensionQueryFlags) flags);
      string forRegistrationId = (string) null;
      foreach (ExtensionVersion version in extension.Versions)
      {
        if (version.Flags.HasFlag((Enum) ExtensionVersionFlags.Validated))
        {
          string property = extension.GetProperty(version.Version, "RegistrationId");
          Guid result;
          if (!string.IsNullOrEmpty(property) && Guid.TryParse(property, out result) && result.Equals(registrationId))
          {
            forRegistrationId = version.Version;
            break;
          }
        }
      }
      return forRegistrationId;
    }

    private bool IsExtensionAlreadyInstalled(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      try
      {
        this.GetInstalledExtension(requestContext, publisherName, extensionName, (IEnumerable<string>) null, false);
        return true;
      }
      catch (InstalledExtensionNotFoundException ex)
      {
        return false;
      }
    }

    internal virtual void EnqueueStateDeletion(
      IVssRequestContext requestContext,
      ExtensionState state)
    {
      if (state == null || requestContext.IsServicingContext)
        return;
      requestContext.Trace(10013480, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Queueing a deletion for extension with id {0}.{1}", (object) state.PublisherName, (object) state.ExtensionName);
      IVssRequestContext context = requestContext.Elevate();
      TeamFoundationTaskService service = context.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
      using (context.Lock(this.m_stateDeletionLockName))
      {
        this.m_stateDeletionQueue.Add(state);
        if (this.m_stateDeletionTaskQueued)
          return;
        DateTime startTime = DateTime.UtcNow + this.m_stateDeletionTaskDelay;
        service.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.StateDeletionTask), (object) null, startTime, 0));
        this.m_stateDeletionTaskQueued = true;
      }
    }

    internal virtual void EnqueueVersionCheck(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      if (requestContext.IsFeatureEnabled(ExtensionManagementFeatureFlags.UseJobForAsyncStateProcessing))
      {
        requestContext.Trace(10013510, TraceLevel.Verbose, nameof (InstalledExtensionService), "Service", "using job service for processing version checks");
        requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          InstalledExtensionService.m_processExtensionStateJobId
        }, this.m_asyncVersionCheckDelay);
      }
      else
      {
        requestContext.Trace(10013510, TraceLevel.Verbose, nameof (InstalledExtensionService), "Service", "using task service for processing version checks");
        TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<TeamFoundationTaskService>();
        using (vssRequestContext.Lock(this.m_versionCheckLockName))
        {
          if (this.m_versionCheckTaskQueued)
            return;
          DateTime startTime = DateTime.UtcNow + TimeSpan.FromSeconds((double) this.m_asyncVersionCheckDelay);
          service.AddTask(vssRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.PerformVersionCheckTask), (object) null, startTime, 0));
          this.m_versionCheckTaskQueued = true;
        }
      }
    }

    internal virtual void EnqueueCacheInvalidation(
      IVssRequestContext requestContext,
      List<ServiceEvent> messages,
      bool directMessageUpdate = false)
    {
      if (messages == null || messages.Count <= 0)
        return;
      TeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<TeamFoundationTaskService>();
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      if (directMessageUpdate)
      {
        DateTime startTime = DateTime.UtcNow + this.m_directUpdateMessageBusTaskDelay;
        service.AddTask(vssRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessServiceEvents), (object) messages, startTime, 0));
      }
      else
      {
        using (vssRequestContext.Lock(this.m_messageBusLockName))
        {
          this.m_messageBusQueue.AddRange((IEnumerable<ServiceEvent>) messages);
          if (this.m_messageBusTaskQueued)
            return;
          DateTime startTime = DateTime.UtcNow + this.m_messageBusTaskDelay;
          service.AddTask(vssRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishMessageBusTask), (object) null, startTime, 0));
          this.m_messageBusTaskQueued = true;
        }
      }
    }

    internal virtual void EnqueueCacheInvalidation(
      IVssRequestContext requestContext,
      ServiceEvent message,
      bool directMessageUpdate = false)
    {
      if (message == null)
        return;
      this.EnqueueCacheInvalidation(requestContext, new List<ServiceEvent>()
      {
        message
      }, directMessageUpdate);
    }

    private void ProcessServiceEvents(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(1048096, nameof (InstalledExtensionService), "Service", nameof (ProcessServiceEvents));
      if (taskArgs is List<ServiceEvent> serviceEventList && serviceEventList.Any<ServiceEvent>())
        this.ProcessMessageBusEvents(requestContext, serviceEventList);
      requestContext.TraceLeave(1048099, nameof (InstalledExtensionService), "Service", nameof (ProcessServiceEvents));
    }

    private void PublishMessageBusTask(IVssRequestContext requestContext, object taskArgs)
    {
      List<ServiceEvent> toDeliver = (List<ServiceEvent>) null;
      using (requestContext.Elevate().Lock(this.m_messageBusLockName))
      {
        toDeliver = this.m_messageBusQueue;
        this.m_messageBusQueue = new List<ServiceEvent>();
        this.m_messageBusTaskQueued = false;
      }
      if (toDeliver.Count <= 0)
        return;
      this.ProcessMessageBusEvents(requestContext, toDeliver);
    }

    private void ProcessMessageBusEvents(
      IVssRequestContext requestContext,
      List<ServiceEvent> toDeliver)
    {
      IVssRequestContext vssRequestContext1 = requestContext.Elevate();
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        vssRequestContext1.GetService<IMessageBusPublisherService>().Publish(vssRequestContext1, "Microsoft.VisualStudio.Services.Extension", (object[]) toDeliver.ToArray());
      }
      else
      {
        IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        TeamFoundationSqlNotificationService service1 = vssRequestContext2.GetService<TeamFoundationSqlNotificationService>();
        ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
        foreach (ServiceEvent serviceEvent in toDeliver)
        {
          InstalledExtensionMessage resource = serviceEvent.Resource as InstalledExtensionMessage;
          string eventData = TeamFoundationSerializationUtility.SerializeToString<InstalledExtensionMessage>(resource);
          service1.SendNotification(vssRequestContext2, ExtensionManagementSdkSqlNotificationClasses.InstalledExtensionChanged, eventData);
          service2.PublishNotification(requestContext, (object) resource);
        }
      }
    }

    private void StateDeletionTask(IVssRequestContext requestContext, object taskArgs)
    {
      List<ExtensionState> extensionStateList = new List<ExtensionState>();
      using (requestContext.Elevate().Lock(this.m_stateDeletionLockName))
      {
        if (this.m_stateDeletionQueue != null)
          extensionStateList.AddRange((IEnumerable<ExtensionState>) this.m_stateDeletionQueue);
        this.m_stateDeletionQueue = new HashSet<ExtensionState>();
        this.m_stateDeletionTaskQueued = false;
      }
      if (extensionStateList.Count <= 0)
        return;
      requestContext.Trace(10013485, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Processing state deletions");
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IGalleryService service = vssRequestContext.GetService<IGalleryService>();
      List<ExtensionState> extensionStates = new List<ExtensionState>();
      foreach (ExtensionState extensionState in extensionStateList)
      {
        try
        {
          if (InstalledExtensionService.s_bypassDeletionCheck.Contains(string.Format("{0}.{1}", (object) extensionState.PublisherName, (object) extensionState.ExtensionName)))
          {
            extensionStates.Add(extensionState);
          }
          else
          {
            PublishedExtension extension = service.GetExtension(vssRequestContext, extensionState.PublisherName, extensionState.ExtensionName, (string) null, ExtensionQueryFlags.None);
            if (extension != null)
            {
              if (!extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated))
                extensionStates.Add(extensionState);
            }
          }
        }
        catch (ExtensionDoesNotExistException ex)
        {
          extensionStates.Add(extensionState);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013490, nameof (InstalledExtensionService), "Service", ex);
        }
      }
      if (extensionStates.Count <= 0)
        return;
      using (InstalledExtensionComponent component = requestContext.CreateComponent<InstalledExtensionComponent>())
        component.DeleteExtensionStates(extensionStates);
      extensionStates.ForEach((Action<ExtensionState>) (state =>
      {
        if (!state.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Warning))
          return;
        InstalledExtensionService.DeleteInstallationIssues(requestContext, state.PublisherName, state.ExtensionName);
      }));
      extensionStates.ForEach((Action<ExtensionState>) (state =>
      {
        if (state.Flags.HasFlag((Enum) Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn))
          return;
        this.EnqueueCacheInvalidation(requestContext, this.CreateMessage(requestContext, "extension.uninstall", this.GetUninstallEventData(requestContext, state.PublisherName, state.ExtensionName)), true);
      }));
      this.RefreshCache(requestContext);
    }

    private void CheckPermission(IVssRequestContext requestContext) => requestContext.GetService<IExtensionPoliciesService>().CheckManagePermission(requestContext);

    private void OnExtensionStateChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      requestContext.Trace(10013287, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "OnExtensionStateChanged: Received message that states have changed.  Clearing cache.");
      this.RefreshCache(requestContext);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadConfiguration(requestContext);
    }

    private void OnPublishedExtensionChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        ExtensionChangeNotification changeNotification = TeamFoundationSerializationUtility.Deserialize<ExtensionChangeNotification>(eventData);
        requestContext.Trace(10013288, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "OnPublishedExtensionChanged: Received message that extension has been updated.  {0}.{1}. EventType: {2}.", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName, (object) changeNotification.EventType);
        if (changeNotification.EventType != ExtensionEventType.ExtensionDisabled && changeNotification.EventType != ExtensionEventType.ExtensionEnabled)
          return;
        this.RefreshCache(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013285, nameof (InstalledExtensionService), "Service", ex);
      }
    }

    private void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      string action,
      CustomerIntelligenceData data,
      bool logTelemetryForOnPremise = false)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (!logTelemetryForOnPremise)
          return;
        requestContext.PublishAppInsightsTelemetry(action, false);
      }
      else
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.Contributions, ExtensionManagementCustomerIntelligenceFeature.ExtensionManagement, data);
    }

    private void RefreshCache(IVssRequestContext requestContext)
    {
      requestContext.Trace(10013285, TraceLevel.Info, nameof (InstalledExtensionService), "Service", "Clearing cached states.");
      this.m_extensionStates = (List<ExtensionState>) null;
      requestContext.GetService<InstalledExtensionManager>().InvalidateHost(requestContext, requestContext.ServiceHost.InstanceId);
    }

    private void FireExtensionChangeEvent(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string extensionVersion,
      ExtensionUpdateType updateType,
      bool includeInitiator,
      bool suppressIfSystem)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      PublishedExtension publishedExtension = vssRequestContext.GetService<IPublishedExtensionCache>().GetPublishedExtension(vssRequestContext, publisherName, extensionName, "latest");
      if (publishedExtension == null)
        return;
      this.FireExtensionChangeEvent(requestContext, publishedExtension, extensionVersion, updateType, includeInitiator, suppressIfSystem);
    }

    private void FireExtensionChangeEvent(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      string extensionVersion,
      ExtensionUpdateType updateType,
      bool includeInitiator,
      bool suppressIfSystem)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (suppressIfSystem && (requestContext.IsServicingContext || requestContext.IsSystemContext || IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity)) || publishedExtension == null)
        return;
      PublishedExtension publishedExtension1 = new PublishedExtension()
      {
        ExtensionName = publishedExtension.ExtensionName,
        DisplayName = publishedExtension.DisplayName,
        ShortDescription = publishedExtension.ShortDescription,
        LongDescription = publishedExtension.LongDescription,
        Publisher = publishedExtension.Publisher,
        Flags = publishedExtension.Flags
      };
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> extensionManagers = vssRequestContext.GetService<IExtensionPoliciesService>().GetExtensionManagers(vssRequestContext, 100);
      INotificationEventService service = requestContext.GetService<INotificationEventService>();
      Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionEvent extensionEvent = new Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionEvent()
      {
        Extension = publishedExtension1,
        ExtensionVersion = extensionVersion,
        Host = new ExtensionHost()
        {
          Id = requestContext.ServiceHost.InstanceId,
          Name = requestContext.ServiceHost.Name
        },
        UpdateType = updateType,
        Links = this.GetExtensionUrls(requestContext, publishedExtension)
      };
      VssNotificationEvent theEvent = new VssNotificationEvent()
      {
        ItemId = publishedExtension1.ExtensionId.ToString()
      };
      theEvent.EventType = "ms.vss-extmgmt-web.extension-event";
      theEvent.Data = (object) extensionEvent;
      if (includeInitiator)
      {
        theEvent.AddActor(VssNotificationEvent.Roles.Initiator, userIdentity.Id);
        extensionEvent.ModifiedBy = new IdentityRef()
        {
          Id = userIdentity.Id.ToString(),
          DisplayName = userIdentity.DisplayName
        };
      }
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) extensionManagers)
        theEvent.AddActor("manager", identity.Id);
      service.PublishSystemEvent(requestContext, theEvent);
    }

    private void FireAuditEvent(
      IVssRequestContext requestContext,
      ExtensionUpdateType updateType,
      PublishedExtension publishedExtension,
      string version = null,
      string fromVersion = null)
    {
      if (publishedExtension == null)
        return;
      this.FireAuditEvent(requestContext, updateType, publishedExtension.Publisher != null ? publishedExtension.Publisher.DisplayName : string.Empty, publishedExtension.DisplayName, version, fromVersion);
    }

    private void FireCiInstallEvent(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      string version)
    {
      CustomerIntelligenceData data = new CustomerIntelligenceData();
      data.Add(CustomerIntelligenceProperty.Action, CustomerIntelligenceActions.Install);
      data.Add("PublisherName", publishedExtension.Publisher.PublisherName);
      data.Add("ExtensionName", publishedExtension.ExtensionName);
      data.Add("Version", version != null ? version.ToString() : string.Empty);
      data.Add("Enabled", true);
      data.Add("VerifiedPublisher", publishedExtension.Publisher.Flags.HasFlag((Enum) PublisherFlags.Verified));
      data.Add("PublicExtension", publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public));
      data.Add("PreviewExtension", publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Preview));
      data.Add("IsMarketplaceExtension", publishedExtension.IsMarketExtension());
      data.Add("ActivityId", (object) requestContext.ActivityId);
      this.PublishCustomerIntelligenceEvent(requestContext, CustomerIntelligenceActions.Install, data, true);
    }

    private void FireAuditEvent(
      IVssRequestContext requestContext,
      ExtensionUpdateType updateType,
      InstalledExtension installedExtension)
    {
      if (installedExtension == null)
        return;
      this.FireAuditEvent(requestContext, updateType, installedExtension.PublisherDisplayName, installedExtension.ExtensionDisplayName, installedExtension.Version);
    }

    private void FireAuditEvent(
      IVssRequestContext requestContext,
      ExtensionUpdateType updateType,
      string publisherName,
      string extensionName,
      string version = null,
      string fromVersion = null)
    {
      string actionId;
      switch (updateType)
      {
        case ExtensionUpdateType.Uninstalled:
          actionId = ExtensionAuditConstants.Uninstalled;
          break;
        case ExtensionUpdateType.Enabled:
          actionId = ExtensionAuditConstants.Enabled;
          break;
        case ExtensionUpdateType.Disabled:
          actionId = ExtensionAuditConstants.Disabled;
          break;
        case ExtensionUpdateType.VersionUpdated:
          actionId = ExtensionAuditConstants.VersionUpdated;
          break;
        default:
          actionId = ExtensionAuditConstants.Installed;
          break;
      }
      Dictionary<string, object> data = new Dictionary<string, object>()
      {
        {
          ExtensionAuditConstants.PublisherName,
          (object) publisherName
        },
        {
          ExtensionAuditConstants.ExtensionName,
          (object) extensionName
        }
      };
      if (!string.IsNullOrEmpty(version))
        data.Add(ExtensionAuditConstants.Version, (object) version);
      if (!string.IsNullOrEmpty(fromVersion))
        data.Add(ExtensionAuditConstants.FromVersion, (object) fromVersion);
      requestContext.LogAuditEvent(actionId, data);
    }

    private ExtensionEventUrls GetExtensionUrls(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension)
    {
      ExtensionEventUrls extensionUrls = new ExtensionEventUrls();
      IVssRequestContext context = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.ClientAccessMappingMoniker);
      if (locationServiceUrl != null)
        extensionUrls.ManageExtensionsPage = Path.Combine(locationServiceUrl, "_admin/_extensions");
      if (publishedExtension.Publisher != null)
      {
        string str1 = context.GetClient<GalleryHttpClient>().BaseAddress.ToString().TrimEnd('/');
        string str2 = str1;
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          str2 += "/_gallery";
        extensionUrls.ExtensionPage = str2 + "/items/" + publishedExtension.Publisher.PublisherName + "." + publishedExtension.ExtensionName;
        if ((publishedExtension.Flags & PublishedExtensionFlags.Public) != PublishedExtensionFlags.None)
          extensionUrls.ExtensionIcon = str1 + "/_apis/public/gallery/publisher/" + publishedExtension.Publisher.PublisherName + "/extension/" + publishedExtension.ExtensionName + "/latest/assetbyname/Microsoft.VisualStudio.Services.Icons.Default";
      }
      return extensionUrls;
    }

    private static ArtifactSpec GetInstallationIssuesArtifactSpec(
      string publisherName,
      string extensionName)
    {
      return new ArtifactSpec(ExtensionManagementPropertyServiceConstants.ExtensionDataPropertiesArtifactKind, InstalledExtensionService.GetInstallationIssuesArtifactMoniker(publisherName, extensionName), 0);
    }

    private static string GetInstallationIssuesArtifactMoniker(
      string publisherName,
      string extensionName)
    {
      return string.Format("{0}_InstallationIssues", (object) GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName));
    }

    private static void ScheduleTaskToDeleteInstallationIssues(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      requestContext.Elevate().To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask((TeamFoundationTaskCallback) ((requestContextTask, args) => InstalledExtensionService.DeleteInstallationIssues(requestContextTask, publisherName, extensionName)), (object) null, DateTime.UtcNow, 0));
    }

    private static void DeleteInstallationIssues(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      requestContext.GetService<ITeamFoundationPropertyService>().DeleteArtifacts(requestContext, (IEnumerable<ArtifactSpec>) new ArtifactSpec[1]
      {
        InstalledExtensionService.GetInstallationIssuesArtifactSpec(publisherName, extensionName)
      });
    }

    private static void ScheduleTaskToSaveInstallationIssues(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      ExtensionDemandsResolutionResult demandsResult)
    {
      requestContext.Elevate().To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask((TeamFoundationTaskCallback) ((requestContextTask, args) => InstalledExtensionService.SaveInstallationIssues(requestContextTask, publisherName, extensionName, demandsResult)), (object) null, DateTime.UtcNow, 0));
    }

    private void ScheduleTaskToFireNotificationEvent(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string extensionVersion,
      ExtensionUpdateType updateType,
      bool includeInitiator,
      bool suppressIfSystem)
    {
      requestContext.Elevate().To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask((TeamFoundationTaskCallback) ((requestContextTask, args) => this.FireExtensionChangeEvent(requestContextTask, publisherName, extensionName, extensionVersion, updateType, includeInitiator, suppressIfSystem)), (object) null, DateTime.UtcNow, 0));
    }

    private static void SaveInstallationIssues(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      ExtensionDemandsResolutionResult demandsResult)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      ArtifactPropertyValue[] artifactPropertyValueArray = new ArtifactPropertyValue[1]
      {
        new ArtifactPropertyValue(InstalledExtensionService.GetInstallationIssuesArtifactSpec(publisherName, extensionName), (IEnumerable<PropertyValue>) new PropertyValue[1]
        {
          new PropertyValue("Demands", (object) InstalledExtensionService.ConvertToInstallationIssues(demandsResult.DemandIssues).Serialize<List<InstalledExtensionStateIssue>>())
        })
      };
      service.SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueArray);
    }

    private static List<InstalledExtensionStateIssue> ConvertToInstallationIssues(
      List<DemandIssue> demandIssues)
    {
      return demandIssues == null ? (List<InstalledExtensionStateIssue>) null : demandIssues.Select<DemandIssue, InstalledExtensionStateIssue>((Func<DemandIssue, InstalledExtensionStateIssue>) (i => new InstalledExtensionStateIssue()
      {
        Type = i.Type == DemandIssueType.Warning ? InstalledExtensionStateIssueType.Warning : InstalledExtensionStateIssueType.Error,
        Source = "Demands",
        Message = i.Message
      })).ToList<InstalledExtensionStateIssue>();
    }
  }
}
