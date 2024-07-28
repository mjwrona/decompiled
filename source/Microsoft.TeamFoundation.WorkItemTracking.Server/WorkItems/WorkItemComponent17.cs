// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent17
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent17 : WorkItemComponent16
  {
    protected override WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset> GetWorkItemDataSetBinder(
      bool bindTitle,
      bool bindCountFields,
      IdentityDisplayType identityDisplayType)
    {
      return (WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset>) new WorkItemComponent.WorkItemDatasetBinder5<WorkItemDataset>(bindTitle, bindCountFields, identityDisplayType, new Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier));
    }

    protected override WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues> GetWorkItemFieldValuesBinder(
      IEnumerable<int> wideTableFields,
      IdentityDisplayType identityDisplayType,
      bool disableProjectionLevelThree)
    {
      return (WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValuesBinder5<WorkItemFieldValues>(wideTableFields, identityDisplayType, new Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), disableProjectionLevelThree);
    }
  }
}
