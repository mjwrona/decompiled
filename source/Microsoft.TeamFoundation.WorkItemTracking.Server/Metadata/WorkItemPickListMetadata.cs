// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickListMetadata
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemPickListMetadata
  {
    protected bool m_isSuggested;

    public Guid Id { get; protected set; }

    public string Name { get; protected set; }

    public int ConstId { get; protected set; }

    public WorkItemPickListType Type { get; protected set; }

    public static WorkItemPickListMetadata Create(WorkItemPickListMetadataRecord metadata)
    {
      WorkItemPickList workItemPickList = new WorkItemPickList();
      workItemPickList.Id = metadata.Id;
      workItemPickList.Name = metadata.Name;
      workItemPickList.ConstId = metadata.ConstId;
      workItemPickList.Type = (WorkItemPickListType) metadata.Type;
      workItemPickList.m_isSuggested = metadata.IsSuggested;
      return (WorkItemPickListMetadata) workItemPickList;
    }

    public bool IsSuggested(IVssRequestContext requestContext, Guid typeletId, int fieldId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<bool>(909931, 909932, "PickLists", "ProcessWorkItemPickListService", nameof (IsSuggested), (Func<bool>) (() =>
      {
        WorkItemTypeletFieldProperties properties;
        if (requestContext.GetService<WorkItemTypeletFieldPropertiesCacheService>().TryGetWorkItemTypeletFieldProperties(requestContext, typeletId, fieldId, out properties) && properties != null)
        {
          bool? isSuggested = properties.IsSuggested;
          if (isSuggested.HasValue)
          {
            isSuggested = properties.IsSuggested;
            return isSuggested.Value;
          }
        }
        return this.m_isSuggested;
      }));
    }
  }
}
