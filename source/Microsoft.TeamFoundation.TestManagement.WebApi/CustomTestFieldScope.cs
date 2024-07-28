// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.CustomTestFieldScope
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [Flags]
  public enum CustomTestFieldScope
  {
    None = 0,
    TestRun = 1,
    TestResult = 2,
    System = 4,
    All = System | TestResult | TestRun, // 0x00000007
  }
}
