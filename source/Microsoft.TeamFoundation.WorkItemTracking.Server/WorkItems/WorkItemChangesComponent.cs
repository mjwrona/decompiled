// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemChangesComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemChangesComponent : WorkItemTrackingResourceComponent
  {
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<WorkItemChangesComponent>(1)
    }, "WorkItemChanges");

    public virtual WorkItemChangesResult GetWorkItemsAndLinksChangedDate(int linkType)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemsAndLinksChangeDate");
      this.BindInt("@linkType", linkType);
      WorkItemChangesResult linksChangedDate = new WorkItemChangesResult();
      IDataReader dataReader = this.ExecuteReader();
      if (dataReader.Read())
      {
        object obj = dataReader.GetValue(0);
        linksChangedDate.WorkItemsChangedDate = obj != DBNull.Value ? (DateTime?) obj : new DateTime?();
      }
      dataReader.NextResult();
      if (dataReader.Read())
      {
        object obj = dataReader.GetValue(0);
        linksChangedDate.WorkItemLinksChangedDate = obj != DBNull.Value ? (DateTime?) obj : new DateTime?();
      }
      return linksChangedDate;
    }
  }
}
