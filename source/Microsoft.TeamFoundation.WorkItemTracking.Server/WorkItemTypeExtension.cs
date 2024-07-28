// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtension
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTypeExtension : WorkItemTypelet, IWorkItemTypeExtension
  {
    private IEnumerable<int> m_referencedFieldsByPredicate;
    private WorkItemTypeExtensionPredicateNode[][] m_explodedPredicate;

    internal WorkItemTypeExtension()
    {
    }

    internal static WorkItemTypeExtension Create(
      IVssRequestContext requestContext,
      WorkItemTypeletRecord record,
      WorkItemTrackingFieldService fieldDict)
    {
      WorkItemTypeExtension itemTypeExtension = new WorkItemTypeExtension();
      itemTypeExtension.SetupInternal(requestContext, record, fieldDict);
      return itemTypeExtension;
    }

    protected void SetupInternal(
      IVssRequestContext requestContext,
      WorkItemTypeletRecord record,
      WorkItemTrackingFieldService fieldDict,
      WorkItemExtensionPredicate predicate = null,
      IEnumerable<WorkItemFieldRule> fieldRules = null)
    {
      this.Id = record.Id;
      this.ProjectId = record.ProjectId;
      this.OwnerId = record.OwnerId;
      this.Name = record.Name;
      this.Description = record.Description;
      this.LastChangedDate = record.LastChangeDate;
      this.Form = new Layout();
      this.ReconciliationStatus = (WorkItemTypeExtensionReconciliationStatus) record.ReconciliationStatus;
      this.ReconciliationMessage = record.ReconciliationMessage;
      this.Rank = record.Rank;
      if (record.Fields == null)
        throw new InvalidOperationException(string.Format("Extension {0} was initialized without fields", (object) this.Id));
      WorkItemTypeExtensionFieldEntry[] array = ((IEnumerable<WorkItemTypeletFieldRecord>) record.Fields).Select<WorkItemTypeletFieldRecord, WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeletFieldRecord, WorkItemTypeExtensionFieldEntry>) (field =>
      {
        return new WorkItemTypeExtensionFieldEntry(this.Id, fieldDict.GetFieldById(requestContext, field.FieldId, new bool?(true)) ?? throw new WorkItemTrackingFieldDefinitionNotFoundException(field.FieldId));
      })).ToArray<WorkItemTypeExtensionFieldEntry>();
      this.MarkerField = ((IEnumerable<WorkItemTypeExtensionFieldEntry>) array).FirstOrDefault<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.FieldId == record.MarkerField));
      if (this.MarkerField == null)
        throw new InvalidOperationException(string.Format("Extension {0} was initialized without a matching marker field", (object) this.Id));
      this.Fields = (IEnumerable<WorkItemTypeExtensionFieldEntry>) ((IEnumerable<WorkItemTypeExtensionFieldEntry>) array).Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.FieldId != record.MarkerField)).ToArray<WorkItemTypeExtensionFieldEntry>();
      if (predicate != null)
        this.Predicate = predicate;
      else
        this.Predicate = !string.IsNullOrEmpty(record.Predicate) ? TeamFoundationSerializationUtility.Deserialize<WorkItemExtensionPredicate>(record.Predicate) : throw new InvalidOperationException(string.Format("Extension {0} was initialized without a predicate", (object) this.Id));
      if (fieldRules != null)
        this.FieldRules = fieldRules;
      else
        this.FieldRules = !string.IsNullOrEmpty(record.Rules) ? (IEnumerable<WorkItemFieldRule>) TeamFoundationSerializationUtility.Deserialize<WorkItemFieldRule[]>(record.Rules, new XmlRootAttribute("rules")) : throw new InvalidOperationException(string.Format("Extension {0} was initialized without rules", (object) this.Id));
    }

    public Guid ProjectId { get; private set; }

    public Guid OwnerId { get; private set; }

    public WorkItemTypeExtensionReconciliationStatus ReconciliationStatus { get; private set; }

    public Guid ReconciliationWatermark { get; private set; }

    public string ReconciliationMessage { get; private set; }

    public virtual WorkItemTypeExtensionFieldEntry MarkerField { get; private set; }

    public virtual WorkItemExtensionPredicate Predicate { get; private set; }

    public int Rank { get; private set; }

    internal void Update(
      WorkItemTypeExtensionReconciliationStatusRecord rStatus)
    {
      this.ReconciliationStatus = (WorkItemTypeExtensionReconciliationStatus) rStatus.ReconciliationStatus;
      this.ReconciliationWatermark = rStatus.ReconciliationWatermark;
      this.ReconciliationMessage = rStatus.ReconciliationMessage;
    }

    internal void UpdateReconciliationStatus(
      WorkItemTypeExtensionReconciliationStatus reconciliationStatus)
    {
      this.ReconciliationStatus = reconciliationStatus;
    }

    internal void UpdateReconciliationWatermark(Guid reconciliationWatermark) => this.ReconciliationWatermark = reconciliationWatermark;

    public IEnumerable<int> GetReferencedFieldsByPredicate()
    {
      if (this.m_referencedFieldsByPredicate == null)
        this.m_referencedFieldsByPredicate = (IEnumerable<int>) this.Predicate.GetReferencedFields().ToArray<int>();
      return this.m_referencedFieldsByPredicate;
    }

    public override IEnumerable<int> GetReferencedFields()
    {
      if (this.m_referencedFields == null)
        this.m_referencedFields = (IEnumerable<int>) this.GetReferencedFieldsByRules().Concat<int>(this.Fields.Select<WorkItemTypeExtensionFieldEntry, int>((Func<WorkItemTypeExtensionFieldEntry, int>) (efe => efe.Field.FieldId))).Concat<int>((IEnumerable<int>) new int[1]
        {
          this.MarkerField.Field.FieldId
        }).Concat<int>(this.GetReferencedFieldsByPredicate()).Distinct<int>().ToArray<int>();
      return this.m_referencedFields;
    }

    public bool IsPredicateMatch(IPredicateEvaluationHelper predicateEvaluator) => this.Predicate.Evaluate(predicateEvaluator);

    internal WorkItemTypeExtensionPredicateNode[][] GetNormalizedPredicate()
    {
      if (this.m_explodedPredicate == null)
        this.m_explodedPredicate = new WorkItemTypeExtensionPredicateNode((PredicateOperator) this.Predicate).OrsOfAnds();
      return this.m_explodedPredicate;
    }
  }
}
