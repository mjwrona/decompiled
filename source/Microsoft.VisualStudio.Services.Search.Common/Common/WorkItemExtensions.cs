// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.WorkItemExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class WorkItemExtensions
  {
    public static void MarkAsPermanentlyDeleted(this WorkItem workItem)
    {
      if (workItem == null)
        throw new ArgumentNullException(nameof (workItem));
      if (!workItem.Id.HasValue)
        throw new ArgumentException("Id cannot be null.", nameof (workItem));
      workItem.Fields = (IDictionary<string, object>) new Dictionary<string, object>();
      workItem.Rev = new int?(0);
    }

    public static bool IsPermanentlyDeleted(this WorkItem workItem)
    {
      if (workItem == null)
        throw new ArgumentNullException(nameof (workItem));
      return workItem.Id.HasValue && workItem.Fields.Count == 0 && workItem.Rev.HasValue && workItem.Rev.Value == 0;
    }

    public static bool IsValid(this WorkItem workItem)
    {
      if (workItem == null)
        throw new ArgumentNullException(nameof (workItem));
      return workItem.Id.HasValue && workItem.Rev.HasValue;
    }
  }
}
