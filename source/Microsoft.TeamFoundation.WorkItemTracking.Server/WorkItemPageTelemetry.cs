// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemPageTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemPageTelemetry : WorkItemTrackingTelemetry
  {
    private const string c_pageWorkItemFields = "PageWorkItemFields";
    private const string c_thresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemPageAboveThreshold";
    private const int c_defaultThresholdTime = 500;

    public WorkItemPageTelemetry(IVssRequestContext requestContext, string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemPageAboveThreshold", 500)
    {
    }

    public static string Feature => "PageWorkItemFields";

    public override void AddData(params object[] param)
    {
      if (param.Length != 1 || !(param[0] is WorkItemPageTelemetryParams pageTelemetryParams))
        return;
      if (pageTelemetryParams.Fields != null)
      {
        this.ClientTraceData.Add("FieldTypes", (object) ("[" + string.Join("][", pageTelemetryParams.Fields.GroupBy<FieldEntry, InternalFieldType>((Func<FieldEntry, InternalFieldType>) (f => f.FieldType)).Select<IGrouping<InternalFieldType, FieldEntry>, string>((Func<IGrouping<InternalFieldType, FieldEntry>, string>) (g => g.Key.ToString() + ":" + g.Count<FieldEntry>().ToString()))) + "]"));
        this.ClientTraceData.Add("Fields", (object) string.Join(",", pageTelemetryParams.Fields.Select<FieldEntry, string>((Func<FieldEntry, string>) (f => f.Name))));
        this.ClientTraceData.Add("StorageTargets", (object) ("[" + string.Join("][", pageTelemetryParams.Fields.GroupBy<FieldEntry, FieldStorageTarget>((Func<FieldEntry, FieldStorageTarget>) (f => f.StorageTarget)).Select<IGrouping<FieldStorageTarget, FieldEntry>, string>((Func<IGrouping<FieldStorageTarget, FieldEntry>, string>) (g => g.Key.ToString() + ":" + g.Count<FieldEntry>().ToString()))) + "]"));
        this.ClientTraceData.Add("FieldsCount", (object) pageTelemetryParams.Fields.Count<FieldEntry>());
      }
      ClientTraceData clientTraceData = this.ClientTraceData;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs = pageTelemetryParams.WorkItemIdRevPairs;
      // ISSUE: variable of a boxed type
      __Boxed<int?> local = (ValueType) (workItemIdRevPairs != null ? new int?(workItemIdRevPairs.Count<WorkItemIdRevisionPair>()) : new int?());
      clientTraceData.Add("GivenWorkItemsCount", (object) local);
      this.ClientTraceData.Add("WideTableProjectionLevel", (object) pageTelemetryParams.WideTableProjectionLevel);
      this.ClientTraceData.Add("WideFields", (object) pageTelemetryParams.WideFields);
      this.ClientTraceData.Add("LongFields", (object) pageTelemetryParams.LongFields);
      this.ClientTraceData.Add("TextFields", (object) pageTelemetryParams.TextFields);
      this.ClientTraceData.Add("ByRevision", (object) pageTelemetryParams.ByRevision);
      this.ClientTraceData.Add("AsOfDateTime", (object) pageTelemetryParams.AsOfDateTime);
      this.ClientTraceData.Add("MaxLongTextLength", (object) pageTelemetryParams.MaxLongTextLength);
      this.ClientTraceData.Add("IdentityDisplayType", (object) pageTelemetryParams.IdentityDisplayType.ToString());
      this.ClientTraceData.Add("WorkItemRetrievalMode", (object) pageTelemetryParams.WorkItemRetrievalMode.ToString());
      this.ClientTraceData.Add("FetchWorkItemsCount", (object) pageTelemetryParams.FetchWorkItemsCount);
      this.ClientTraceData.Add("ReturnWorkItemsCount", (object) pageTelemetryParams.ReturnWorkItemsCount);
    }
  }
}
