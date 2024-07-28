// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.IAccountCacheConfiguration
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public interface IAccountCacheConfiguration
  {
    string InstanceName { get; set; }

    string FileName { get; set; }

    string DirectoryPath { get; set; }

    string FilePath { get; }

    string MacKeyChainService { get; set; }

    string MacKeyChainAccount { get; set; }

    string LinuxKeyRingSchema { get; set; }

    string LinuxKeyRingCollection { get; set; }

    string LinuxKeyRingLabel { get; set; }

    KeyValuePair<string, string> LinuxKeyRingAttr1 { get; set; }

    KeyValuePair<string, string> LinuxKeyRingAttr2 { get; set; }

    string LockfilePath { get; }

    string LockfileName { get; set; }
  }
}
