// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestRunType
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  public enum TestRunType
  {
    Unspecified = 0,
    Normal = 1,
    Blocking = 2,
    Web = 4,
    MtrRunInitiatedFromWeb = 8,
    RunWithDtlEnv = 16, // 0x00000010
  }
}
