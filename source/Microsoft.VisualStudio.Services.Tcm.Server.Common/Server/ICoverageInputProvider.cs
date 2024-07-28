// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ICoverageInputProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public interface ICoverageInputProvider
  {
    IEnumerable<CodeCoverageFile> GetCoverageFiles();

    IEnumerable<CoverageScope> GetCoverageScopes();

    string GetUserDefinedCoverageScopesValue(CodeCoverageFile coverageFile);

    IEnumerable<CodeCoverageFile> GetCoverageFilesByScope(CoverageScope coverageScope);

    Dictionary<CoverageScope, Dictionary<CodeCoverageFile, string>> GetCoverageScopeIntermediateFilesMap(
      Dictionary<CodeCoverageFile, string> coverageDictionary,
      Dictionary<string, object> ciData);
  }
}
