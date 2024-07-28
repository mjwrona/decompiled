// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemLinkUpdateResultExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal static class WorkItemLinkUpdateResultExtensions
  {
    public static Tuple<int, int, int> ToKey(this WorkItemLinkUpdateResult linkUpdateResult)
    {
      if (linkUpdateResult == null)
        throw new ArgumentNullException(nameof (linkUpdateResult));
      return new Tuple<int, int, int>(linkUpdateResult.SourceWorkItemId, linkUpdateResult.TargetWorkItemId, linkUpdateResult.LinkType);
    }

    public static Tuple<int, int, int> ToReverseKey(
      this WorkItemLinkUpdateResult linkUpdateResult,
      MDWorkItemLinkType linkType)
    {
      if (linkUpdateResult == null)
        throw new ArgumentNullException(nameof (linkUpdateResult));
      if (linkType == null)
        throw new ArgumentNullException(nameof (linkType));
      return new Tuple<int, int, int>(linkUpdateResult.TargetWorkItemId, linkUpdateResult.SourceWorkItemId, linkType.ForwardId);
    }
  }
}
