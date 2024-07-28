// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.ContextTagKeys
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.CodeDom.Compiler;
using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class ContextTagKeys
  {
    private static ContextTagKeys keys;

    internal static ContextTagKeys Keys => LazyInitializer.EnsureInitialized<ContextTagKeys>(ref ContextTagKeys.keys);

    public string ApplicationVersion { get; set; }

    public string ApplicationBuild { get; set; }

    public string DeviceId { get; set; }

    public string DeviceIp { get; set; }

    public string DeviceLanguage { get; set; }

    public string DeviceLocale { get; set; }

    public string DeviceModel { get; set; }

    public string DeviceNetwork { get; set; }

    public string DeviceOEMName { get; set; }

    public string DeviceOS { get; set; }

    public string DeviceOSVersion { get; set; }

    public string DeviceRoleInstance { get; set; }

    public string DeviceRoleName { get; set; }

    public string DeviceScreenResolution { get; set; }

    public string DeviceType { get; set; }

    public string DeviceMachineName { get; set; }

    public string LocationIp { get; set; }

    public string OperationId { get; set; }

    public string OperationName { get; set; }

    public string OperationParentId { get; set; }

    public string OperationRootId { get; set; }

    public string OperationSyntheticSource { get; set; }

    public string OperationIsSynthetic { get; set; }

    public string SessionId { get; set; }

    public string SessionIsFirst { get; set; }

    public string SessionIsNew { get; set; }

    public string UserAccountAcquisitionDate { get; set; }

    public string UserAccountId { get; set; }

    public string UserAgent { get; set; }

    public string UserId { get; set; }

    public string UserStoreRegion { get; set; }

    public string SampleRate { get; set; }

    public string InternalSdkVersion { get; set; }

    public string InternalAgentVersion { get; set; }

    public ContextTagKeys()
      : this("AI.ContextTagKeys", nameof (ContextTagKeys))
    {
    }

    protected ContextTagKeys(string fullName, string name)
    {
      this.ApplicationVersion = "ai.application.ver";
      this.ApplicationBuild = "ai.application.build";
      this.DeviceId = "ai.device.id";
      this.DeviceIp = "ai.device.ip";
      this.DeviceLanguage = "ai.device.language";
      this.DeviceLocale = "ai.device.locale";
      this.DeviceModel = "ai.device.model";
      this.DeviceNetwork = "ai.device.network";
      this.DeviceOEMName = "ai.device.oemName";
      this.DeviceOS = "ai.device.os";
      this.DeviceOSVersion = "ai.device.osVersion";
      this.DeviceRoleInstance = "ai.device.roleInstance";
      this.DeviceRoleName = "ai.device.roleName";
      this.DeviceScreenResolution = "ai.device.screenResolution";
      this.DeviceType = "ai.device.type";
      this.DeviceMachineName = "ai.device.machineName";
      this.LocationIp = "ai.location.ip";
      this.OperationId = "ai.operation.id";
      this.OperationName = "ai.operation.name";
      this.OperationParentId = "ai.operation.parentId";
      this.OperationRootId = "ai.operation.rootId";
      this.OperationSyntheticSource = "ai.operation.syntheticSource";
      this.OperationIsSynthetic = "ai.operation.isSynthetic";
      this.SessionId = "ai.session.id";
      this.SessionIsFirst = "ai.session.isFirst";
      this.SessionIsNew = "ai.session.isNew";
      this.UserAccountAcquisitionDate = "ai.user.accountAcquisitionDate";
      this.UserAccountId = "ai.user.accountId";
      this.UserAgent = "ai.user.userAgent";
      this.UserId = "ai.user.id";
      this.UserStoreRegion = "ai.user.storeRegion";
      this.SampleRate = "ai.sample.sampleRate";
      this.InternalSdkVersion = "ai.internal.sdkVersion";
      this.InternalAgentVersion = "ai.internal.agentVersion";
    }
  }
}
