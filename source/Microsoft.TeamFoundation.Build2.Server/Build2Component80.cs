// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component80
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component80 : Build2Component79
  {
    public override async Task<List<CronSchedule>> DeleteCronSchedulesForDefinitions(
      Guid projectId,
      List<int> definitionIds)
    {
      Build2Component80 component = this;
      List<CronSchedule> list;
      using (component.TraceScope(method: nameof (DeleteCronSchedulesForDefinitions)))
      {
        component.PrepareStoredProcedure("Build.prc_DeleteCronSchedulesForDefinitions");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindUniqueInt32Table("@definitionIdTable", (IEnumerable<int>) definitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<CronSchedule>(component.GetCronScheduleBinder());
          list = resultCollection.GetCurrent<CronSchedule>().Items.Distinct<CronSchedule>().ToList<CronSchedule>();
        }
      }
      return list;
    }
  }
}
