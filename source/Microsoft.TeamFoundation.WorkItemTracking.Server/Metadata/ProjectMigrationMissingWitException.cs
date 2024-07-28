// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProjectMigrationMissingWitException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [Serializable]
  public class ProjectMigrationMissingWitException : ProcessServiceException
  {
    public ProjectMigrationMissingWitException(
      string projectName,
      string sourceProcessName,
      string targetProcessName,
      string quotedTypeNameList)
      : base(ServerResources.ProjectMigrateMissingWits((object) projectName, (object) sourceProcessName, (object) targetProcessName, (object) quotedTypeNameList), 402396)
    {
    }
  }
}
