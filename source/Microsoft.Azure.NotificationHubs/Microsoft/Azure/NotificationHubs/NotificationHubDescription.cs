// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.NotificationHubDescription
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "NotificationHubDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public sealed class NotificationHubDescription : EntityDescription, IResourceDescription
  {
    public const string DefaultListenSasRuleName = "DefaultListenSharedAccessSignature";
    public const string DefaultFullSasRuleName = "DefaultFullSharedAccessSignature";
    private string path;
    private bool? internalStatus;

    internal NotificationHubDescription()
    {
    }

    public NotificationHubDescription(string path) => this.Path = path;

    public string Path
    {
      get => this.path;
      set
      {
        this.ThrowIfReadOnly();
        this.path = !string.IsNullOrWhiteSpace(value) ? value : throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentNullOrEmpty(nameof (Path));
      }
    }

    string IResourceDescription.CollectionName => "NotificationHubs";

    public void SetAccessPasswords(
      string fullAccessRuleName,
      string fullAccessPassword,
      string listenAccessRuleName,
      string listenAccessPassword)
    {
      if (string.IsNullOrWhiteSpace(fullAccessRuleName))
        throw new ArgumentNullException(nameof (fullAccessRuleName));
      if (string.IsNullOrWhiteSpace(fullAccessPassword))
        throw new ArgumentNullException(nameof (fullAccessPassword));
      if (string.IsNullOrWhiteSpace(listenAccessRuleName))
        throw new ArgumentNullException(nameof (listenAccessRuleName));
      if (string.IsNullOrWhiteSpace(listenAccessPassword))
        throw new ArgumentNullException(nameof (listenAccessPassword));
      this.SetAccessPassword(fullAccessRuleName, fullAccessPassword, (IEnumerable<AccessRights>) new AccessRights[3]
      {
        AccessRights.Listen,
        AccessRights.Send,
        AccessRights.Manage
      });
      this.SetAccessPassword(listenAccessRuleName, listenAccessPassword, (IEnumerable<AccessRights>) new AccessRights[1]
      {
        AccessRights.Listen
      });
    }

    public void SetDefaultAccessPasswords(string fullAccessPassword, string listenAccessPassword)
    {
      if (string.IsNullOrWhiteSpace(fullAccessPassword))
        throw new ArgumentNullException(nameof (fullAccessPassword));
      if (string.IsNullOrWhiteSpace(listenAccessPassword))
        throw new ArgumentNullException(nameof (listenAccessPassword));
      this.SetAccessPassword("DefaultFullSharedAccessSignature", fullAccessPassword, (IEnumerable<AccessRights>) new AccessRights[3]
      {
        AccessRights.Listen,
        AccessRights.Send,
        AccessRights.Manage
      });
      this.SetAccessPassword("DefaultListenSharedAccessSignature", listenAccessPassword, (IEnumerable<AccessRights>) new AccessRights[1]
      {
        AccessRights.Listen
      });
    }

    public AuthorizationRules Authorization
    {
      get
      {
        if (this.InternalAuthorization == null)
          this.InternalAuthorization = new AuthorizationRules();
        return this.InternalAuthorization;
      }
    }

    public TimeSpan? RegistrationTtl
    {
      get
      {
        if (!this.InternalRegistrationTtl.HasValue)
          this.InternalRegistrationTtl = new TimeSpan?(Constants.DefaultRegistrationTtl);
        return this.InternalRegistrationTtl;
      }
      set
      {
        this.ThrowIfReadOnly();
        if (value.Value < Constants.MinimumRegistrationTtl)
          throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentOutOfRange(nameof (value), (object) value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Registration Ttl must be at least {0}", new object[1]
          {
            (object) Constants.MinimumRegistrationTtl
          }));
        this.InternalRegistrationTtl = value;
      }
    }

    [DataMember(Name = "Status", IsRequired = false, EmitDefaultValue = false, Order = 1016)]
    private bool? InternalStatus
    {
      get => this.internalStatus;
      set => this.internalStatus = value;
    }

    [IgnoreDataMember]
    public bool IsDisabled
    {
      get => this.internalStatus ?? false;
      set => this.internalStatus = new bool?(value);
    }

    public bool IsAnonymousAccessible => false;

    [DataMember(Name = "ApnsCredential", IsRequired = false, EmitDefaultValue = false, Order = 1001)]
    public ApnsCredential ApnsCredential { get; set; }

    [DataMember(Name = "RegistrationTtl", IsRequired = false, EmitDefaultValue = false, Order = 1002)]
    internal TimeSpan? InternalRegistrationTtl { get; set; }

    [DataMember(Name = "WnsCredential", IsRequired = false, EmitDefaultValue = false, Order = 1003)]
    public WnsCredential WnsCredential { get; set; }

    [DataMember(Name = "AuthorizationRules", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
    internal AuthorizationRules InternalAuthorization { get; set; }

    [DataMember(Name = "GcmCredential", IsRequired = false, EmitDefaultValue = false, Order = 1005)]
    public GcmCredential GcmCredential { get; set; }

    [DataMember(Name = "MpnsCredential", IsRequired = false, EmitDefaultValue = false, Order = 1006)]
    public MpnsCredential MpnsCredential { get; set; }

    [DataMember(Name = "DailyOperations", IsRequired = false, EmitDefaultValue = false, Order = 1007)]
    public long DailyOperations { get; internal set; }

    [DataMember(Name = "DailyMaxActiveDevices", IsRequired = false, EmitDefaultValue = false, Order = 1008)]
    public long DailyMaxActiveDevices { get; internal set; }

    [DataMember(Name = "DailyMaxActiveRegistrations", IsRequired = false, EmitDefaultValue = false, Order = 1009)]
    public long DailyMaxActiveRegistrations { get; internal set; }

    [DataMember(Name = "UserMetadata", IsRequired = false, EmitDefaultValue = false, Order = 1010)]
    internal string InternalUserMetadata { get; set; }

    [DataMember(Name = "SmtpCredential", IsRequired = false, EmitDefaultValue = false, Order = 1011)]
    internal SmtpCredential SmtpCredential { get; set; }

    [DataMember(Name = "DailyPushes", IsRequired = false, EmitDefaultValue = false, Order = 1012)]
    public long DailyPushes { get; internal set; }

    [DataMember(Name = "DailyApiCalls", IsRequired = false, EmitDefaultValue = false, Order = 1013)]
    public long DailyApiCalls { get; internal set; }

    [DataMember(Name = "AdmCredential", IsRequired = false, EmitDefaultValue = false, Order = 1014)]
    public AdmCredential AdmCredential { get; set; }

    [DataMember(Name = "NokiaXCredential", IsRequired = false, EmitDefaultValue = false, Order = 1015)]
    internal NokiaXCredential NokiaXCredential { get; set; }

    [DataMember(Name = "BaiduCredential", IsRequired = false, EmitDefaultValue = false, Order = 1016)]
    public BaiduCredential BaiduCredential { get; set; }

    public string UserMetadata
    {
      get => this.InternalUserMetadata;
      set
      {
        this.ThrowIfReadOnly();
        if (string.IsNullOrWhiteSpace(value))
          this.InternalUserMetadata = (string) null;
        else
          this.InternalUserMetadata = value.Length <= 1024 ? value : throw Microsoft.Azure.NotificationHubs.Messaging.FxTrace.Exception.ArgumentOutOfRange(nameof (UserMetadata), (object) value.Length, SRClient.ArgumentOutOfRange((object) 0, (object) 1024));
      }
    }

    internal override bool RequiresEncryption => true;

    internal override bool IsValidForVersion(ApiVersion version) => base.IsValidForVersion(version) && (version >= ApiVersion.Four || this.GcmCredential == null) && (version >= MinimalApiVersionFor.AdmSupport || this.AdmCredential == null) && (version >= MinimalApiVersionFor.BaiduSupport || this.BaiduCredential == null) && (version >= MinimalApiVersionFor.DisableNotificationHub || !this.InternalStatus.HasValue);

    internal override void UpdateForVersion(
      ApiVersion version,
      EntityDescription existingDescription = null)
    {
      NotificationHubDescription notificationHubDescription = existingDescription as NotificationHubDescription;
      base.UpdateForVersion(version, existingDescription);
      if (version < ApiVersion.Four)
      {
        this.GcmCredential = notificationHubDescription == null ? (GcmCredential) null : notificationHubDescription.GcmCredential;
        this.MpnsCredential = notificationHubDescription == null ? (MpnsCredential) null : notificationHubDescription.MpnsCredential;
      }
      if (version < ApiVersion.Five)
      {
        this.DailyMaxActiveDevices = 0L;
        this.DailyMaxActiveRegistrations = 0L;
        this.DailyOperations = 0L;
      }
      if (version < ApiVersion.Seven)
      {
        this.DailyPushes = 0L;
        this.DailyApiCalls = 0L;
      }
      if (version < MinimalApiVersionFor.AdmSupport)
        this.AdmCredential = notificationHubDescription == null ? (AdmCredential) null : notificationHubDescription.AdmCredential;
      if (version < MinimalApiVersionFor.BaiduSupport)
        this.BaiduCredential = notificationHubDescription == null ? (BaiduCredential) null : notificationHubDescription.BaiduCredential;
      if (version >= MinimalApiVersionFor.DisableNotificationHub)
        return;
      this.InternalStatus = notificationHubDescription == null ? new bool?() : notificationHubDescription.InternalStatus;
    }

    private void SetAccessPassword(
      string accessKeyName,
      string password,
      IEnumerable<AccessRights> rights)
    {
      lock (this.Authorization)
      {
        SharedAccessAuthorizationRule rule;
        if (this.Authorization.TryGetSharedAccessAuthorizationRule(accessKeyName, out rule))
        {
          rule.PrimaryKey = password;
          rule.Rights = rights;
        }
        else
          this.Authorization.Add((AuthorizationRule) new SharedAccessAuthorizationRule(accessKeyName, password, rights));
      }
    }
  }
}
