// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GetFileUtilityInstance
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.IO;
using Microsoft.CodeCoverage.IO.Coverage;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class GetFileUtilityInstance
  {
    public static ICoverageFileUtility GetCoverageFileUtilityInstance() => (ICoverageFileUtility) new CoverageFileUtility();

    public static ICoverageFileUtilityV2 GetCoverageFileUtilityV2Instance() => (ICoverageFileUtilityV2) new CoverageFileUtilityV2((ICoverageFileConfiguration) new CoverageFileConfiguration(true, true, true, true, true, true, Environment.ProcessorCount < 1 ? 2 : Environment.ProcessorCount, true));
  }
}
