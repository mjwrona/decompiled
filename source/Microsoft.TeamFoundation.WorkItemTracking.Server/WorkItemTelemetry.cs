// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceGetWorkItemAboveThreshold";
    private const int c_defaultThresholdTime = 1000;
    private const int c_DateTimeSize = 20;
    private const string c_feature = "GetWorkItem";

    public WorkItemTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceGetWorkItemAboveThreshold", 1000)
    {
    }

    public static string Feature => "GetWorkItem";

    private static int GetDataSize(object obj) => obj is DateTime ? 20 : obj.ToString().Length;

    public override void AddData(params object[] param)
    {
      if (param.Length != 5)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (!(param[0] is IEnumerable<WorkItem> source))
        return;
      bool? nullable1 = param[1] as bool?;
      bool? nullable2 = param[2] as bool?;
      bool? nullable3 = param[3] as bool?;
      bool? nullable4 = param[4] as bool?;
      this.ClientTraceData.Add("IncludeResourceLinks", (object) nullable1);
      this.ClientTraceData.Add("IncludeWorkItemLinks", (object) nullable2);
      this.ClientTraceData.Add("IncludeHistory", (object) nullable3);
      this.ClientTraceData.Add("IncludeTags", (object) nullable4);
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      List<int> intList3 = new List<int>();
      List<int> intList4 = new List<int>();
      List<int> intList5 = new List<int>();
      List<int> intList6 = new List<int>();
      List<int> intList7 = new List<int>();
      this.ClientTraceData.Add("Count", (object) source.Count<WorkItem>());
      foreach (WorkItem workItem in source.Where<WorkItem>((Func<WorkItem, bool>) (wi => wi != null)))
      {
        intList1.Add(workItem.Id);
        intList2.Add(workItem.Revision);
        intList3.Add(workItem.WorkItemLinks != null ? workItem.WorkItemLinks.Count<WorkItemLinkInfo>() : 0);
        intList4.Add(workItem.ResourceLinks != null ? workItem.ResourceLinks.Count : 0);
        List<int> intList8 = intList5;
        int num1;
        if (workItem.LatestData == null || !workItem.LatestData.ContainsKey(80) || workItem.LatestData[80] == null)
          num1 = 0;
        else
          num1 = workItem.LatestData[80].ToString().Split(';').Length;
        intList8.Add(num1);
        intList6.Add(workItem.Revisions != null ? workItem.Revisions.Count<WorkItemRevision>((Func<WorkItemRevision, bool>) (r => r.LatestData != null && r.LatestData.ContainsKey(54))) : 0);
        int num2 = 0;
        if (workItem.LatestData != null)
        {
          foreach (KeyValuePair<int, object> keyValuePair in workItem.LatestData.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (k => k.Value != null)))
            num2 += WorkItemTelemetry.GetDataSize(keyValuePair.Value);
        }
        if (workItem.Revisions != null)
        {
          foreach (WorkItemFieldData workItemFieldData in workItem.Revisions.Where<WorkItemRevision>((Func<WorkItemRevision, bool>) (r => r.LatestData != null)))
          {
            foreach (KeyValuePair<int, object> keyValuePair in workItemFieldData.LatestData.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (k => k.Value != null)))
              num2 += WorkItemTelemetry.GetDataSize(keyValuePair.Value);
          }
        }
        intList7.Add(num2);
      }
      this.ClientTraceData.Add("Ids", (object) intList1);
      this.ClientTraceData.Add("Revisions", (object) intList2);
      if (nullable1.HasValue && nullable1.Value)
        this.ClientTraceData.Add("ResourceLinkCounts", (object) intList4);
      if (nullable2.HasValue && nullable2.Value)
        this.ClientTraceData.Add("WorkItemLinkCounts", (object) intList3);
      if (nullable3.HasValue && nullable3.Value)
        this.ClientTraceData.Add("HistoryCounts", (object) intList6);
      if (nullable4.HasValue && nullable4.Value)
        this.ClientTraceData.Add("Tags", (object) intList5);
      this.ClientTraceData.Add("WorkItemSizes", (object) intList7);
    }
  }
}
