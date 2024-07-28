// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTrackingCommonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class WorkItemTrackingCommonExtensions
  {
    public static IEnumerable<IWorkItemType> SelectByName(
      this IEnumerable<IWorkItemType> workItems,
      IEnumerable<string> names)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(names, nameof (names));
      return workItems.Join<IWorkItemType, string, string, IWorkItemType>(names, (Func<IWorkItemType, string>) (workItem => workItem.Name), (Func<string, string>) (name => name), (Func<IWorkItemType, string, IWorkItemType>) ((wi, name) => wi), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
    }
  }
}
