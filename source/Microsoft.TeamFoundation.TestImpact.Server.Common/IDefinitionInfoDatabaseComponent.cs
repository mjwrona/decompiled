// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.IDefinitionInfoDatabaseComponent
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  public interface IDefinitionInfoDatabaseComponent
  {
    List<DefinitionInfo> QueryDefinitionInfoList(bool isDeleted);

    void QueueDeleteDefinitionInfo(Guid projectId, int definitionType, int definitionId);

    int DeleteDefinitionInfo(
      int dataspaceId,
      int definitionInfoId,
      int resumeStage,
      int deletionBatchSize);
  }
}
