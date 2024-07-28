// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionsReconciliationTelemetry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTypeExtensionsReconciliationTelemetry : WorkItemTrackingTelemetry
  {
    private const string ThresholdRegistryPath = "/Service/WorkItemTracking/Settings/TraceWorkItemTypeExtensionsReconciliationAboveThreshold";
    private const int DefaultThresholdTimeMilliseconds = 500;
    private static readonly XmlSerializer WorkItemExtensionPredicateSerializer = new XmlSerializer(typeof (WorkItemExtensionPredicate));
    private static readonly XmlWriterSettings XmlWriterSettings = new XmlWriterSettings()
    {
      Indent = false,
      NewLineHandling = NewLineHandling.None
    };

    public WorkItemTypeExtensionsReconciliationTelemetry(
      IVssRequestContext requestContext,
      string feature)
      : base(requestContext, feature, "/Service/WorkItemTracking/Settings/TraceWorkItemTypeExtensionsReconciliationAboveThreshold", 500)
    {
    }

    public static string Feature => "WorkItemTypeExtensionsReconciliation";

    public override void AddData(params object[] param)
    {
      if (param.Length != 1 || !(param[0] is WorkItemTypeExtensionsReconciliationTelemetryParams reconciliationTelemetryParams))
        return;
      this.ClientTraceData.Add("ExtensionId", (object) reconciliationTelemetryParams.Extension.Id);
      this.ClientTraceData.Add("ExtensionProjectId", (object) reconciliationTelemetryParams.Extension.ProjectId);
      this.ClientTraceData.Add("ExtensionName", (object) reconciliationTelemetryParams.Extension.Name);
      this.ClientTraceData.Add("ExtensionDescription", (object) reconciliationTelemetryParams.Extension.Description);
      this.ClientTraceData.Add("ExtensionLastChangedDate", (object) reconciliationTelemetryParams.Extension.LastChangedDate);
      this.ClientTraceData.Add("ExtensionReconciliationStatus", (object) reconciliationTelemetryParams.Extension.ReconciliationStatus);
      this.ClientTraceData.Add("ExtensionReconciliationMessage", (object) reconciliationTelemetryParams.Extension.ReconciliationMessage);
      this.ClientTraceData.Add("ExtensionMarkerField", (object) (reconciliationTelemetryParams.Extension.MarkerField.Field.ReferenceName + ":" + reconciliationTelemetryParams.Extension.MarkerField.Field.FieldId.ToString()));
      this.ClientTraceData.Add("ExtensionPredicate", (object) WorkItemTypeExtensionsReconciliationTelemetry.SerializeXml<WorkItemExtensionPredicate>(WorkItemTypeExtensionsReconciliationTelemetry.WorkItemExtensionPredicateSerializer, reconciliationTelemetryParams.Extension.Predicate));
      this.ClientTraceData.Add("ExtensionRank", (object) reconciliationTelemetryParams.Extension.Rank);
      ClientTraceData clientTraceData = this.ClientTraceData;
      List<FieldEntry> pageFields = reconciliationTelemetryParams.PageFields;
      IEnumerable<string> strings = pageFields != null ? pageFields.Select<FieldEntry, string>((Func<FieldEntry, string>) (f => string.Format("{0}:{1}", (object) f.Name, (object) f.FieldId))) : (IEnumerable<string>) null;
      clientTraceData.Add("PageFields", (object) strings);
      this.ClientTraceData.Add("ExtensionFields", (object) reconciliationTelemetryParams.ExtensionFields);
      this.ClientTraceData.Add("Wiql", (object) reconciliationTelemetryParams.Wiql);
      this.ClientTraceData.Add("InitialActivateCount", (object) reconciliationTelemetryParams.InitialActivateCount);
      this.ClientTraceData.Add("ActualActivateCount", (object) reconciliationTelemetryParams.ActualActivateCount);
      this.ClientTraceData.Add("InitialDeactivateCount", (object) reconciliationTelemetryParams.InitialDeactivateCount);
      this.ClientTraceData.Add("ActualDeactivateCount", (object) reconciliationTelemetryParams.ActualDeactivateCount);
      this.ClientTraceData.Add("AlreadyActiveCount", (object) reconciliationTelemetryParams.AlreadyActiveCount);
      this.ClientTraceData.Add("ActivateRetryTimes", (object) reconciliationTelemetryParams.ActivateRetryTimes);
      this.ClientTraceData.Add("ActivateRetryCount", (object) reconciliationTelemetryParams.ActivateRetryCount);
      this.ClientTraceData.Add("DeactivateRetryTimes", (object) reconciliationTelemetryParams.DeactivateRetryTimes);
      this.ClientTraceData.Add("DeactivateRetryCount", (object) reconciliationTelemetryParams.DeactivateRetryCount);
      this.ClientTraceData.Add("TotalFetchCount", (object) reconciliationTelemetryParams.TotalFetchCount);
      this.ClientTraceData.Add("NewestWorkItemDateTime", (object) reconciliationTelemetryParams.NewestWorkItemDateTime);
      this.ClientTraceData.Add("OldestWorkItemDateTime", (object) reconciliationTelemetryParams.OldestWorkItemDateTime);
      this.ClientTraceData.Add("PageWorkItemsDbCount", (object) reconciliationTelemetryParams.PageWorkItemsDbCount);
      this.ClientTraceData.Add("UpdateWorkItemsDbCount", (object) reconciliationTelemetryParams.UpdateWorkItemsDbCount);
      this.ClientTraceData.Add("FieldsToEvaluate", (object) reconciliationTelemetryParams.FieldsToEvaluate);
      this.ClientTraceData.Add("QueryWorkItemsTimeMs", (object) reconciliationTelemetryParams.QueryWorkItemsTimeMs);
      this.ClientTraceData.Add("GetActiveWorkItemsTimeMs", (object) reconciliationTelemetryParams.GetActiveWorkItemsTimeMs);
      this.ClientTraceData.Add("PageWorkItemsTimeMs", (object) reconciliationTelemetryParams.PageWorkItemsTimeMs);
      this.ClientTraceData.Add("PageWorkItemsMaxTimeMs", (object) reconciliationTelemetryParams.PageWorkItemsMaxTimeMs);
      this.ClientTraceData.Add("UpdateWorkItemsTimeMs", (object) reconciliationTelemetryParams.UpdateWorkItemsTimeMs);
      this.ClientTraceData.Add("UpdateWorkItemsMaxTimeMs", (object) reconciliationTelemetryParams.UpdateWorkItemsMaxTimeMs);
      this.ClientTraceData.Add("LastStepName", (object) reconciliationTelemetryParams.LastStepName);
      this.ClientTraceData.Add("LeaseRenewCount", (object) reconciliationTelemetryParams.LeaseRenewCount);
      this.ClientTraceData.Add("ReconcileResult", reconciliationTelemetryParams.ReconcileResult ? (object) "Succeeded" : (object) "Canceled");
    }

    private static string SerializeXml<T>(XmlSerializer xmlSerializer, T obj)
    {
      if ((object) obj == null)
        return (string) null;
      using (MemoryStream output = new MemoryStream())
      {
        using (XmlWriter xmlWriter = XmlWriter.Create((Stream) output, WorkItemTypeExtensionsReconciliationTelemetry.XmlWriterSettings))
        {
          xmlSerializer.Serialize(xmlWriter, (object) obj);
          xmlWriter.Flush();
        }
        output.Seek(0L, SeekOrigin.Begin);
        using (StreamReader streamReader = new StreamReader((Stream) output))
          return streamReader.ReadToEnd();
      }
    }
  }
}
