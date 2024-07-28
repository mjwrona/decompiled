// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsFilterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  public abstract class RemoteSettingsFilterProvider
  {
    public virtual Guid GetMachineId() => Guid.Empty;

    public virtual Guid GetUserId() => Guid.Empty;

    public virtual async Task<string> GetVsIdAsync() => await Task.FromResult<string>(string.Empty);

    public virtual string GetCulture() => string.Empty;

    public virtual string GetBranchBuildFrom() => string.Empty;

    public virtual string GetApplicationName() => string.Empty;

    public virtual string GetApplicationVersion() => string.Empty;

    public virtual string GetVsSku() => string.Empty;

    public virtual int GetNotificationsCount() => 0;

    public virtual Guid GetAppIdPackageGuid() => Guid.Empty;

    public virtual string GetMacAddressHash() => string.Empty;

    public virtual string GetChannelId() => string.Empty;

    public virtual string GetChannelManifestId() => string.Empty;

    public virtual string GetManifestId() => string.Empty;

    public virtual string GetOsType() => string.Empty;

    public virtual string GetOsVersion() => string.Empty;

    public virtual bool GetIsUserInternal() => false;

    public virtual string GetSessionRole() => string.Empty;

    public virtual string GetClrVersion() => string.Empty;

    public virtual string GetProcessArchitecture() => string.Empty;

    public virtual string GetClientSourceType() => string.Empty;
  }
}
