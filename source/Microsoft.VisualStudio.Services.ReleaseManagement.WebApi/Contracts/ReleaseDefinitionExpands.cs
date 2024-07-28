// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionExpands
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [Flags]
  public enum ReleaseDefinitionExpands
  {
    None = 0,
    Environments = 2,
    Artifacts = 4,
    Triggers = 8,
    Variables = 16, // 0x00000010
    Tags = 32, // 0x00000020
    LastRelease = 64, // 0x00000040
  }
}
