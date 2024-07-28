// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskDefinitionComponent8
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskDefinitionComponent8 : TaskDefinitionComponent7
  {
    public override async Task<IList<TaskDefinitionData>> GetTaskDefinitionsForExtensionAsync(
      string extensionIdentifier)
    {
      TaskDefinitionComponent8 component = this;
      IList<TaskDefinitionData> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetTaskDefinitionsForExtensionAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetTaskDefinitionsForExtension");
        component.BindString("@extensionIdentifier", extensionIdentifier, 2048, true, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskDefinitionData>((ObjectBinder<TaskDefinitionData>) new TaskDefinitionDataBinder3());
          items = (IList<TaskDefinitionData>) resultCollection.GetCurrent<TaskDefinitionData>().Items;
        }
      }
      return items;
    }
  }
}
