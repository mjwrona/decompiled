// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ResourceLinkingTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ResourceLinkingTelemetry : WorkItemTrackingTelemetry
  {
    public ResourceLinkingTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, string.Empty, 0)
    {
    }

    public static string Feature { get; } = "ResourceLinking";

    public override void AddData(params object[] param)
    {
      if (param.Length != 1 || !(param[0] is ICollection<WorkItemUpdateState> source1))
        return;
      this.ClientTraceData.Add("WorkItemUpdateCount", (object) source1.Count);
      foreach (IGrouping<LinkUpdateType, WorkItemResourceLinkUpdate> source2 in source1.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (u => u.HasResourceLinkUpdates)).SelectMany<WorkItemUpdateState, WorkItemResourceLinkUpdate>((Func<WorkItemUpdateState, IEnumerable<WorkItemResourceLinkUpdate>>) (u => u.Update.ResourceLinkUpdates)).GroupBy<WorkItemResourceLinkUpdate, LinkUpdateType>((Func<WorkItemResourceLinkUpdate, LinkUpdateType>) (rlu => rlu.UpdateType)))
      {
        Dictionary<string, int> dictionary = source2.GroupBy<WorkItemResourceLinkUpdate, string>((Func<WorkItemResourceLinkUpdate, string>) (rlu => !string.IsNullOrEmpty(rlu.Name) ? rlu.Name : rlu.Type.ToString())).ToDictionary<IGrouping<string, WorkItemResourceLinkUpdate>, string, int>((Func<IGrouping<string, WorkItemResourceLinkUpdate>, string>) (rlu => rlu.Key), (Func<IGrouping<string, WorkItemResourceLinkUpdate>, int>) (rlu => rlu.Count<WorkItemResourceLinkUpdate>()));
        this.ClientTraceData.Add(string.Format("ResourceLinkUpdateTypes-{0}", (object) source2.Key), (object) dictionary);
      }
    }
  }
}
