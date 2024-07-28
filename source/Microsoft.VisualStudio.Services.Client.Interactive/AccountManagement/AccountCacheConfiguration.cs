// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountCacheConfiguration
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  internal class AccountCacheConfiguration : IAccountCacheConfiguration
  {
    private string path;
    internal const string DefaultBaseCachePath = "Microsoft\\VSCommon\\VSAccountManagement\\";

    public string InstanceName { get; set; }

    public string FileName { get; set; } = "AccountCache.cache";

    public string DirectoryPath
    {
      get => this.path != null && Directory.Exists(this.path) ? this.path : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), string.IsNullOrEmpty(this.InstanceName) ? "Microsoft\\VSCommon\\VSAccountManagement\\" : Path.Combine("Microsoft\\VSCommon\\VSAccountManagement\\", this.InstanceName));
      set => this.path = value;
    }

    public string FilePath => Path.Combine(this.DirectoryPath, this.FileName);

    public string MacKeyChainService { get; set; } = "account_cache_service";

    public string MacKeyChainAccount { get; set; } = "account_cache_account";

    public string LinuxKeyRingSchema { get; set; } = "com.microsoft.visualstudio.accountcache";

    public string LinuxKeyRingCollection { get; set; } = "default";

    public string LinuxKeyRingLabel { get; set; } = "Account token cache for all Visual Studio services.";

    public KeyValuePair<string, string> LinuxKeyRingAttr1 { get; set; } = new KeyValuePair<string, string>("Version", "1");

    public KeyValuePair<string, string> LinuxKeyRingAttr2 { get; set; } = new KeyValuePair<string, string>("ProductGroup", "VisualStudio");

    public string LockfileName { get; set; } = "AccountCache.lockfile";

    public string LockfilePath => Path.Combine(this.DirectoryPath, this.LockfileName);
  }
}
