// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IBuildReferenceDatabase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IBuildReferenceDatabase
  {
    void DeleteTestBuild(
      Guid projectGuid,
      string[] buildUris,
      Guid lastUpdatedBy,
      bool deleteOnlyAutomatedRuns,
      bool isTcmService);

    BuildConfiguration QueryBuildConfigurationById(int buildConfigurationid, out Guid projectGuid);

    IList<string> QueryBuildsByProject(Guid projectId, bool? queryDeletedBuild, int batchSize);

    void UpdateBuildDeletionState(Guid projectId, Dictionary<string, bool> buildUriToDeletionState);
  }
}
