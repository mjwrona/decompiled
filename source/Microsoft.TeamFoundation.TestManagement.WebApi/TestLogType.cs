// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestLogType
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public enum TestLogType
  {
    GeneralAttachment = 1,
    [ClientInternalUseOnly(false)] CodeCoverage = 2,
    [ClientInternalUseOnly(false)] TestImpact = 3,
    [ClientInternalUseOnly(false)] Intermediate = 4,
    [ClientInternalUseOnly(false)] System = 5,
    [ClientInternalUseOnly(false)] MergedCoverageFile = 6,
  }
}
