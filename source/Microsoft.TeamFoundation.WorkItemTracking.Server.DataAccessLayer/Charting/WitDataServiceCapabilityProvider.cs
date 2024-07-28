// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Charting.WitDataServiceCapabilityProvider
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Charting
{
  public class WitDataServiceCapabilityProvider : 
    IDataServiceCapabilityProvider,
    IDataServicesService
  {
    public IVssRequestContext RequestContext { get; set; }

    public virtual string GetScopeName() => FeatureProviderScopes.WorkItemQueries;

    public virtual string GetArtifactPluralName(IVssRequestContext requestContext) => DalResourceStrings.Get("WorkItemsPluralName");

    public IEnumerable<FixedIntervalDateRange> GetHistoryRanges(IVssRequestContext requestContext) => WitDataServiceCapabilityProvider.GetRangeOptions();

    internal static IEnumerable<FixedIntervalDateRange> GetRangeOptions() => DefaultDateRanges.GetDefaultOptions();

    public IEnumerable<FieldInfo> GetFields(IVssRequestContext requestContext)
    {
      List<FieldInfo> fields = new List<FieldInfo>();
      foreach (FieldEntry allField in this.GetAllFields(requestContext))
      {
        FieldInfo fieldInfo1 = new FieldInfo();
        fieldInfo1.LabelText = allField.Name;
        fieldInfo1.Name = allField.ReferenceName;
        fieldInfo1.IsAggregatable = this.IsAggregateable(allField);
        fieldInfo1.IsGroupable = this.IsGroupable(allField);
        FieldInfo fieldInfo2 = fieldInfo1;
        if (fieldInfo2.IsAggregatable || fieldInfo2.IsGroupable)
          fields.Add(fieldInfo2);
      }
      return (IEnumerable<FieldInfo>) fields;
    }

    protected virtual IEnumerable<FieldEntry> GetAllFields(IVssRequestContext requestContext) => (IEnumerable<FieldEntry>) requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext).GetAllFields();

    private bool IsGroupable(FieldEntry fieldEntry) => this.IsQueryableWorkItemField(fieldEntry) && !WitDataServiceCapabilityProvider.IsDisallowedGroupFieldType(fieldEntry.FieldType) && !WitDataServiceCapabilityProvider.IsDisallowedFieldId(fieldEntry.FieldId);

    private bool IsAggregateable(FieldEntry fieldEntry) => this.IsQueryableWorkItemField(fieldEntry) && WitDataServiceCapabilityProvider.IsSupportedAggregationFieldType(fieldEntry.FieldType) && !WitDataServiceCapabilityProvider.IsDisallowedFieldId(fieldEntry.FieldId);

    private bool IsQueryableWorkItemField(FieldEntry fieldEntry) => fieldEntry.Usage == InternalFieldUsages.WorkItem && fieldEntry.IsQueryable && fieldEntry.CanSortBy;

    private static bool IsDisallowedGroupFieldType(InternalFieldType fieldType) => fieldType == InternalFieldType.History || fieldType == InternalFieldType.Html || fieldType == InternalFieldType.PlainText;

    private static bool IsSupportedAggregationFieldType(InternalFieldType fieldType) => fieldType == InternalFieldType.Double || fieldType == InternalFieldType.Integer;

    private static bool IsDisallowedFieldId(int fieldId) => fieldId == 1 || fieldId == 80 || fieldId == -3;
  }
}
