// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.FrameworkUserService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  internal class FrameworkUserService : 
    IUserServiceInternal,
    IUserService,
    IVssFrameworkService,
    IUserMailConfirmationServiceInternal,
    IUserMailConfirmationService
  {
    private CommandSetter m_enableProfileSyncComandSetter;
    private FrameworkUserService.EnableProfileSyncCircuitBreakerSettings m_enableProfileSyncCircuitBreakerSettings;
    private static readonly string s_Area = "User";
    private static readonly string s_Layer = nameof (FrameworkUserService);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      Interlocked.CompareExchange<FrameworkUserService.EnableProfileSyncCircuitBreakerSettings>(ref this.m_enableProfileSyncCircuitBreakerSettings, new FrameworkUserService.EnableProfileSyncCircuitBreakerSettings(), (FrameworkUserService.EnableProfileSyncCircuitBreakerSettings) null);
      this.UpdateEnableProfileSyncCommandSeter();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnProfileSyncCircuitBreakerSettingsChanged), in FrameworkUserService.EnableProfileSyncCircuitBreakerSettings.RegistryPath);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnProfileSyncCircuitBreakerSettingsChanged));

    public User GetUser(IVssRequestContext requestContext, SubjectDescriptor descriptor) => this.GetUser(requestContext, descriptor, false);

    public User GetUser(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      bool createIfNotExists)
    {
      requestContext.TraceEnter(165511, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (GetUser));
      try
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return this.GetUserInternal(requestContext, descriptor, createIfNotExists);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (UserHelper.IsOtherAccountUser(requestContext, descriptor))
          vssRequestContext = vssRequestContext.Elevate();
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(vssRequestContext))
          return vssRequestContext.GetService<FrameworkUserService>().GetUser(vssRequestContext, descriptor, createIfNotExists);
      }
      finally
      {
        requestContext.TraceLeave(129499, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (GetUser));
      }
    }

    public User GetUser(IVssRequestContext requestContext, Guid userId) => this.GetUser(requestContext, FrameworkUserService.GetDescriptorForVsid(requestContext, userId), false);

    public User GetUser(IVssRequestContext requestContext, Guid userId, bool createIfNotExists) => this.GetUser(requestContext, FrameworkUserService.GetDescriptorForVsid(requestContext, userId), createIfNotExists);

    public User CreateUser(IVssRequestContext requestContext, CreateUserParameters userParameters)
    {
      if (!(userParameters is InternalCreateUserParameters userParameters1))
        userParameters1 = new InternalCreateUserParameters(userParameters);
      return this.CreateUser(requestContext, userParameters1);
    }

    public User CreateUser(
      IVssRequestContext requestContext,
      InternalCreateUserParameters userParameters)
    {
      requestContext.TraceEnter(474632, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (CreateUser));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserService>().CreateUser(vssRequestContext, userParameters);
        }
        requestContext.GetUserIdentity();
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
        {
          FrameworkUserHttpClient client = this.GetClient(requestContext);
          InternalCreateUserParameters userParameters1 = userParameters;
          CancellationToken cancellationToken1 = requestContext.CancellationToken;
          bool? createLocal = new bool?();
          CancellationToken cancellationToken2 = cancellationToken1;
          return client.CreateUserAsync((CreateUserParameters) userParameters1, createLocal, cancellationToken: cancellationToken2).SyncResult<User>();
        }
      }
      finally
      {
        requestContext.TraceLeave(960044, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (CreateUser));
      }
    }

    public User UpdateUser(IVssRequestContext requestContext, UpdateUserParameters userParameters)
    {
      requestContext.TraceEnter(257342, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (UpdateUser));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserService>().UpdateUser(vssRequestContext, userParameters);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return this.GetClient(requestContext).UpdateUserAsync(userParameters.Descriptor, userParameters, cancellationToken: requestContext.CancellationToken).SyncResult<User>();
      }
      finally
      {
        requestContext.TraceLeave(316778, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (UpdateUser));
      }
    }

    public User UpdateUser(
      IVssRequestContext requestContext,
      Guid userId,
      UpdateUserParameters userParameters)
    {
      if (!userParameters.Descriptor.IsClaimsUserType())
        userParameters.Descriptor = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      return this.UpdateUser(requestContext, userParameters);
    }

    public UserAttribute GetAttribute(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName)
    {
      requestContext.TraceEnter(375441, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (GetAttribute));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserService>().GetAttribute(vssRequestContext, descriptor, attributeName);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return this.GetClient(requestContext).GetAttributeAsync(descriptor, attributeName, cancellationToken: requestContext.CancellationToken).SyncResult<UserAttribute>();
      }
      finally
      {
        requestContext.TraceLeave(736850, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (GetAttribute));
      }
    }

    public UserAttribute GetAttribute(
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName)
    {
      SubjectDescriptor descriptorForVsid = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      return this.GetAttribute(requestContext, descriptorForVsid, attributeName);
    }

    public IList<UserAttribute> QueryAttributes(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null)
    {
      requestContext.TraceEnter(990748, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (QueryAttributes));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserService>().QueryAttributes(vssRequestContext, descriptor, queryPattern, modifiedAfter);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return this.GetClient(requestContext).QueryAttributes(descriptor, queryPattern, modifiedAfter, requestContext.CancellationToken);
      }
      finally
      {
        requestContext.TraceLeave(325075, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (QueryAttributes));
      }
    }

    public IList<UserAttribute> QueryAttributes(
      IVssRequestContext requestContext,
      Guid userId,
      string queryPattern,
      DateTimeOffset? modifiedAfter = null)
    {
      SubjectDescriptor descriptorForVsid = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      return this.QueryAttributes(requestContext, descriptorForVsid, queryPattern, modifiedAfter);
    }

    public IList<UserAttribute> SetAttributes(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      IList<SetUserAttributeParameters> attributeParametersList)
    {
      requestContext.TraceEnter(337910, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (SetAttributes));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<FrameworkUserService>().SetAttributes(vssRequestContext, descriptor, attributeParametersList);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return (IList<UserAttribute>) this.GetClient(requestContext).SetAttributesAsync(descriptor, (IEnumerable<SetUserAttributeParameters>) attributeParametersList, cancellationToken: requestContext.CancellationToken).SyncResult<List<UserAttribute>>();
      }
      finally
      {
        requestContext.TraceLeave(396862, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (SetAttributes));
      }
    }

    public IList<UserAttribute> SetAttributes(
      IVssRequestContext requestContext,
      Guid userId,
      IList<SetUserAttributeParameters> attributeParametersList)
    {
      SubjectDescriptor descriptorForVsid = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      return this.SetAttributes(requestContext, descriptorForVsid, attributeParametersList);
    }

    public void DeleteAttribute(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string attributeName)
    {
      requestContext.TraceEnter(862722, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (DeleteAttribute));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          vssRequestContext.GetService<FrameworkUserService>().DeleteAttribute(vssRequestContext, descriptor, attributeName);
        }
        else
        {
          using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
            this.GetClient(requestContext).DeleteAttributeAsync(descriptor, attributeName, cancellationToken: requestContext.CancellationToken).SyncResult();
        }
      }
      finally
      {
        requestContext.TraceLeave(707567, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (DeleteAttribute));
      }
    }

    public void DeleteAttribute(
      IVssRequestContext requestContext,
      Guid userId,
      string attributeName)
    {
      SubjectDescriptor descriptorForVsid = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      this.DeleteAttribute(requestContext, descriptorForVsid, attributeName);
    }

    public Avatar GetAvatar(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      AvatarSize size = AvatarSize.Medium)
    {
      requestContext.TraceEnter(271574, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (GetAvatar));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          if (UserHelper.IsOtherAccountUser(requestContext, descriptor))
            vssRequestContext = vssRequestContext.Elevate();
          return vssRequestContext.GetService<FrameworkUserService>().GetAvatar(vssRequestContext, descriptor, size);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return this.GetClient(requestContext).GetAvatarAsync(descriptor, new AvatarSize?(size), cancellationToken: requestContext.CancellationToken).SyncResult<Avatar>();
      }
      finally
      {
        requestContext.TraceLeave(920024, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (GetAvatar));
      }
    }

    public Avatar GetAvatar(IVssRequestContext requestContext, Guid userId, AvatarSize size = AvatarSize.Medium)
    {
      SubjectDescriptor descriptorForVsid = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      return this.GetAvatar(requestContext, descriptorForVsid, size);
    }

    public void UpdateAvatar(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      Avatar avatar)
    {
      requestContext.TraceEnter(231759, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (UpdateAvatar));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          vssRequestContext.GetService<FrameworkUserService>().UpdateAvatar(vssRequestContext, descriptor, avatar);
        }
        else
        {
          using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
            this.GetClient(requestContext).SetAvatarAsync(descriptor, avatar, cancellationToken: requestContext.CancellationToken).SyncResult();
        }
      }
      finally
      {
        requestContext.TraceLeave(69505, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (UpdateAvatar));
      }
    }

    public void UpdateAvatar(IVssRequestContext requestContext, Guid userId, Avatar avatar)
    {
      SubjectDescriptor descriptorForVsid = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      this.UpdateAvatar(requestContext, descriptorForVsid, avatar);
    }

    public void DeleteAvatar(IVssRequestContext requestContext, SubjectDescriptor descriptor)
    {
      requestContext.TraceEnter(107416, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (DeleteAvatar));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          vssRequestContext.GetService<FrameworkUserService>().DeleteAvatar(vssRequestContext, descriptor);
        }
        else
        {
          using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
            this.GetClient(requestContext).DeleteAvatarAsync(descriptor, cancellationToken: requestContext.CancellationToken).SyncResult();
        }
      }
      finally
      {
        requestContext.TraceLeave(180451, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (DeleteAvatar));
      }
    }

    public void DeleteAvatar(IVssRequestContext requestContext, Guid userId)
    {
      SubjectDescriptor descriptorForVsid = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      this.DeleteAvatar(requestContext, descriptorForVsid);
    }

    public Avatar CreateAvatarPreview(
      IVssRequestContext requestContext,
      Guid userId,
      Avatar avatar,
      AvatarSize size = AvatarSize.Medium,
      string displayName = null)
    {
      requestContext.TraceEnter(546602096, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (CreateAvatarPreview));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          bool isOtherAccountUser;
          int num1 = UserHelper.TranslateUserId(requestContext, ref userId, out isOtherAccountUser) ? 1 : 0;
          IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
          int num2 = isOtherAccountUser ? 1 : 0;
          if ((num1 | num2) != 0)
            context = context.Elevate();
          return context.GetService<FrameworkUserService>().CreateAvatarPreview(requestContext, userId, avatar, size, displayName);
        }
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          return this.GetClient(requestContext).CreateAvatarPreviewAsync(userId, avatar, new AvatarSize?(size), displayName, cancellationToken: requestContext.CancellationToken).SyncResult<Avatar>();
      }
      finally
      {
        requestContext.TraceLeave(1209374462, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (CreateAvatarPreview));
      }
    }

    public void ConfirmMail(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      MailConfirmationParameters confirmationParameters)
    {
      requestContext.TraceEnter(379506536, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (ConfirmMail));
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          vssRequestContext.GetService<FrameworkUserService>().ConfirmMail(vssRequestContext, descriptor, confirmationParameters);
        }
        else
        {
          using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
            this.GetClient(requestContext).ConfirmPreferredMailAsync(descriptor, confirmationParameters, cancellationToken: requestContext.CancellationToken).SyncResult();
        }
      }
      finally
      {
        requestContext.TraceLeave(232600678, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, nameof (ConfirmMail));
      }
    }

    public void ConfirmMail(
      IVssRequestContext requestContext,
      Guid userId,
      MailConfirmationParameters confirmationParameters)
    {
      SubjectDescriptor descriptorForVsid = FrameworkUserService.GetDescriptorForVsid(requestContext, userId);
      this.ConfirmMail(requestContext, descriptorForVsid, confirmationParameters);
    }

    public IList<AccessedHost> GetMostRecentlyAccessedHosts(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
        return (IList<AccessedHost>) this.GetAnyClient(requestContext.To(TeamFoundationHostType.Deployment)).GetMostRecentlyAccessedHostsAsync(descriptor, cancellationToken: requestContext.CancellationToken).SyncResult<List<AccessedHost>>();
    }

    public void UpdateMostRecentlyAccessedHosts(
      IVssRequestContext requestContext,
      IList<AccessedHostsParameters> parametersList)
    {
      using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
        this.GetAnyClient(requestContext.To(TeamFoundationHostType.Deployment)).UpdateMostRecentlyAccessedHostsAsync((IEnumerable<AccessedHostsParameters>) parametersList, cancellationToken: requestContext.CancellationToken).SyncResult();
    }

    public void EnableUserProfileDataSync(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      new CommandService(requestContext, this.m_enableProfileSyncComandSetter, (Action) (() => this.EnableUserProfileDataSyncFrameworkInternal(requestContext, descriptor))).Execute();
    }

    internal void EnableUserProfileDataSyncFrameworkInternal(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
        this.GetAnyClient(requestContext.To(TeamFoundationHostType.Deployment)).EnableUserProfileSyncAsync(descriptor, (object) null, requestContext.CancellationToken).SyncResult();
    }

    public void DisableUserProfileDataSync(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
        this.GetAnyClient(requestContext.To(TeamFoundationHostType.Deployment)).DisableUserProfileSyncAsync(descriptor, (object) null, requestContext.CancellationToken).SyncResult();
    }

    internal virtual User GetUserInternal(
      IVssRequestContext deploymentContext,
      SubjectDescriptor descriptor,
      bool createIfNotExists)
    {
      FrameworkUserHttpClient client = this.GetClient(deploymentContext);
      using (UserHelper.GetTemporaryUseDelegatedS2STokens(deploymentContext))
        return client.GetUserAsync(descriptor, new bool?(createIfNotExists), cancellationToken: deploymentContext.CancellationToken).SyncResult<User>();
    }

    private static SubjectDescriptor GetDescriptorForVsid(
      IVssRequestContext requestContext,
      Guid vsid)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetUserIdentity();
      if (identity == null || identity.Id != vsid)
      {
        identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          vsid
        }, QueryMembership.None, (IEnumerable<string>) new string[0])[0];
        if (identity == null)
          throw new IdentityNotFoundException(vsid);
      }
      return identity.SubjectDescriptor;
    }

    private FrameworkUserHttpClient GetClient(IVssRequestContext requestContext) => requestContext.GetClient<FrameworkUserHttpClient>();

    private FrameworkUserHttpClient GetAnyClient(IVssRequestContext requestContext) => requestContext.GetClient<FrameworkUserHttpClient>(FrameworkUserService.UserServicePrincipal);

    private void OnProfileSyncCircuitBreakerSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      try
      {
        Volatile.Write<FrameworkUserService.EnableProfileSyncCircuitBreakerSettings>(ref this.m_enableProfileSyncCircuitBreakerSettings, FrameworkUserService.EnableProfileSyncCircuitBreakerSettings.Load(requestContext));
        this.UpdateEnableProfileSyncCommandSeter();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(922061941, FrameworkUserService.s_Area, FrameworkUserService.s_Layer, ex);
      }
    }

    private void UpdateEnableProfileSyncCommandSeter() => this.m_enableProfileSyncComandSetter = CommandSetter.WithGroupKey((CommandGroupKey) this.m_enableProfileSyncCircuitBreakerSettings.CommandGroupKey).AndCommandKey((CommandKey) this.m_enableProfileSyncCircuitBreakerSettings.CommandKeyForEnableProfileSync).AndCommandPropertiesDefaults(new CommandPropertiesSetter((ICommandProperties) this.m_enableProfileSyncCircuitBreakerSettings));

    private static Guid UserServicePrincipal => new Guid("00000038-0000-8888-8000-000000000000");

    internal class EnableProfileSyncCircuitBreakerSettings : CommandPropertiesDefault
    {
      public static readonly RegistryQuery RegistryPath = (RegistryQuery) "FrameworkUserService/EnableProfileSyncCircuitBreakerSettings/**";

      internal string CommandGroupKey => "User.";

      internal string CommandKeyForEnableProfileSync => "FrameworkUserService-EnableUserProfileDataSync-";

      internal static FrameworkUserService.EnableProfileSyncCircuitBreakerSettings Load(
        IVssRequestContext requestContext)
      {
        requestContext.CheckDeploymentRequestContext();
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, FrameworkUserService.EnableProfileSyncCircuitBreakerSettings.RegistryPath);
        FrameworkUserService.EnableProfileSyncCircuitBreakerSettings circuitBreakerSettings = new FrameworkUserService.EnableProfileSyncCircuitBreakerSettings();
        circuitBreakerSettings.CircuitBreakerDisabled = registryEntryCollection.GetValueFromPath<bool>("CircuitBreakerDisabled", false);
        circuitBreakerSettings.FallbackDisabled = registryEntryCollection.GetValueFromPath<bool>("FallbackDisabled", true);
        circuitBreakerSettings.CircuitBreakerForceClosed = registryEntryCollection.GetValueFromPath<bool>("CircuitBreakerForceClosed", false);
        circuitBreakerSettings.CircuitBreakerForceOpen = registryEntryCollection.GetValueFromPath<bool>("CircuitBreakerForceOpen", false);
        circuitBreakerSettings.CircuitBreakerRequestVolumeThreshold = registryEntryCollection.GetValueFromPath<int>("CircuitBreakerRequestVolumeThreshold", 20);
        circuitBreakerSettings.CircuitBreakerErrorThresholdPercentage = registryEntryCollection.GetValueFromPath<int>("CircuitBreakerErrorThresholdPercentage", 50);
        circuitBreakerSettings.CircuitBreakerMinBackoff = registryEntryCollection.GetValueFromPath<TimeSpan>("CircuitBreakerMinBackoff", TimeSpan.FromSeconds(1.0));
        circuitBreakerSettings.CircuitBreakerMaxBackoff = registryEntryCollection.GetValueFromPath<TimeSpan>("CircuitBreakerMaxBackoff", TimeSpan.FromSeconds(30.0));
        circuitBreakerSettings.CircuitBreakerDeltaBackoff = registryEntryCollection.GetValueFromPath<TimeSpan>("CircuitBreakerDeltaBackoff", TimeSpan.FromMilliseconds(500.0));
        circuitBreakerSettings.ExecutionTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("ExecutionTimeout", TimeSpan.FromSeconds(10.0));
        circuitBreakerSettings.MetricsHealthSnapshotInterval = registryEntryCollection.GetValueFromPath<TimeSpan>("MetricsHealthSnapshotInterval", TimeSpan.FromSeconds(0.5));
        circuitBreakerSettings.MetricsRollingStatisticalWindowInMilliseconds = registryEntryCollection.GetValueFromPath<int>("MetricsRollingStatisticalWindowInMilliseconds", 10000);
        circuitBreakerSettings.MetricsRollingStatisticalWindowBuckets = registryEntryCollection.GetValueFromPath<int>("MetricsRollingStatisticalWindowBuckets", 10);
        return circuitBreakerSettings;
      }

      public EnableProfileSyncCircuitBreakerSettings()
        : base()
      {
      }
    }
  }
}
