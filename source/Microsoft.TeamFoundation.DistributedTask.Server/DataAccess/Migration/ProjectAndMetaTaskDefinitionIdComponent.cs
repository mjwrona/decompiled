// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.Migration.ProjectAndMetaTaskDefinitionIdComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.Migration
{
  internal class ProjectAndMetaTaskDefinitionIdComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ProjectAndMetaTaskDefinitionIdComponent>(1)
    }, "DistributedTaskProjectAndMetaTaskDefinitionId", "DistributedTask");

    public ProjectAndMetaTaskDefinitionIdComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public IEnumerable<ProjectAndMetaTaskDefinitionId> GetProjectAndMetaTaskDefinitionIds()
    {
      this.PrepareStoredProcedure("Task.prc_GetProjectAndMetaTaskDefinitionIds");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectAndMetaTaskDefinitionId>((ObjectBinder<ProjectAndMetaTaskDefinitionId>) new MetaTaskDefinitionIdsBinder());
        return (IEnumerable<ProjectAndMetaTaskDefinitionId>) resultCollection.GetCurrent<ProjectAndMetaTaskDefinitionId>().Items;
      }
    }
  }
}
