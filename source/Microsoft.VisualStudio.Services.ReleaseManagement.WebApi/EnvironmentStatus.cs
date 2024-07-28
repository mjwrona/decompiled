// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.EnvironmentStatus
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [Flags]
  [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Zero value is defined already")]
  public enum EnvironmentStatus
  {
    Undefined = 0,
    NotStarted = 1,
    InProgress = 2,
    Succeeded = 4,
    Canceled = 8,
    Rejected = 16, // 0x00000010
    Queued = 32, // 0x00000020
    Scheduled = 64, // 0x00000040
    PartiallySucceeded = 128, // 0x00000080
  }
}
