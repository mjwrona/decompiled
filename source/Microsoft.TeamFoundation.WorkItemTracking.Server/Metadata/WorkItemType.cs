// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemType : ProcessReadSecuredObject, ILegacyMetadataCompatibilityParticipant
  {
    private IReadOnlyCollection<FieldUsageEntry> m_usageFields;
    private string m_icon;
    private string m_color;
    private string m_description;

    internal WorkItemType(WorkItemTypeEntry workItemTypeEntry, Guid projectId)
    {
      this.Id = workItemTypeEntry.Id;
      this.ProjectId = projectId;
      this.ReferenceName = workItemTypeEntry.ReferenceName;
      this.Name = workItemTypeEntry.Name;
      this.Description = workItemTypeEntry.Description;
      this.Form = workItemTypeEntry.Form;
      this.Color = workItemTypeEntry.Color;
      this.Icon = workItemTypeEntry.Icon;
      this.m_usageFields = (IReadOnlyCollection<FieldUsageEntry>) workItemTypeEntry.UsageFields.ToList<FieldUsageEntry>();
    }

    private WorkItemType(WorkItemType source)
    {
      this.Id = source.Id;
      this.ProjectId = source.ProjectId;
      this.ReferenceName = source.ReferenceName;
      this.InheritedWorkItemType = source.InheritedWorkItemType;
      this.Name = source.Name;
      this.Description = source.Description;
      this.m_usageFields = source.m_usageFields;
      this.Form = source.Form;
      this.Color = source.Color;
      this.Icon = source.Icon;
      this.Source = source.Source;
    }

    protected WorkItemType()
    {
    }

    internal static WorkItemType Create(
      WorkItemTrackingRequestContext witRequestContext,
      ProcessWorkItemType workItemType,
      Guid projectId,
      int? legacyId = null)
    {
      return new WorkItemType()
      {
        Source = workItemType,
        Id = legacyId,
        ProjectId = projectId,
        Name = workItemType.Name,
        ReferenceName = workItemType.ReferenceName,
        Description = workItemType.Description,
        Form = new FormTransformer().ConvertToNewFormXml(workItemType.GetCombinedForm(witRequestContext.RequestContext)),
        m_usageFields = (IReadOnlyCollection<FieldUsageEntry>) workItemType.GetCombinedFields(witRequestContext.RequestContext).Select<FieldEntry, FieldUsageEntry>((Func<FieldEntry, FieldUsageEntry>) (f => new FieldUsageEntry(f.FieldId, FieldSource.WorkItemType))).ToList<FieldUsageEntry>(),
        Color = workItemType.Color,
        Icon = workItemType.Icon
      };
    }

    internal static WorkItemType Create(
      IVssRequestContext requestContext,
      ComposedWorkItemType workItemType,
      Guid projectId,
      int legacyId)
    {
      return new WorkItemType()
      {
        OutOfBoxWorkItemType = (BaseWorkItemType) workItemType,
        Id = new int?(legacyId),
        ProjectId = projectId,
        Name = workItemType.Name,
        ReferenceName = workItemType.ReferenceName,
        Description = workItemType.Description,
        Form = new FormTransformer().ConvertToNewFormXml(workItemType.GetFormLayout(requestContext)),
        Color = workItemType.Color,
        Icon = workItemType.Icon,
        m_usageFields = (IReadOnlyCollection<FieldUsageEntry>) workItemType.GetLegacyFields(requestContext).Select<ProcessFieldResult, FieldUsageEntry>((Func<ProcessFieldResult, FieldUsageEntry>) (pf => new FieldUsageEntry(requestContext.WitContext().FieldDictionary.GetFieldByNameOrId(pf.ReferenceName).FieldId, FieldSource.WorkItemType))).ToList<FieldUsageEntry>()
      };
    }

    public int? Id { get; set; }

    public Guid ProjectId { get; protected set; }

    public string Name { get; protected set; }

    public string ReferenceName { get; protected set; }

    public string Description
    {
      get => this.IsDerived ? this.InheritedWorkItemType.Description ?? this.m_description : this.m_description;
      protected set => this.m_description = value;
    }

    public string Color
    {
      get => this.IsDerived ? this.InheritedWorkItemType.Color ?? this.m_color : this.m_color;
      protected set => this.m_color = value;
    }

    public string Icon
    {
      get => this.IsDerived ? this.InheritedWorkItemType.Icon ?? this.m_icon ?? WorkItemTypeIconUtils.GetDefaultIcon() : this.m_icon;
      protected set => this.m_icon = value;
    }

    public virtual bool IsDisabled
    {
      get
      {
        bool isDisabled = false;
        if (this.IsDerived)
          isDisabled = this.InheritedWorkItemType.IsDisabled;
        else if (this.IsCustomType)
          isDisabled = this.Source.IsDisabled;
        return isDisabled;
      }
    }

    public bool IsDerived => this.InheritedWorkItemType != null;

    public bool IsCustomType => this.Source != null;

    public bool IsMissingOutOfBoxType => this.OutOfBoxWorkItemType != null;

    public ProcessWorkItemType InheritedWorkItemType { get; internal set; }

    private string Form { get; set; }

    internal ProcessWorkItemType Source { get; set; }

    internal BaseWorkItemType OutOfBoxWorkItemType { get; set; }

    public virtual AdditionalWorkItemTypeProperties GetAdditionalProperties(
      IVssRequestContext requestContext,
      WitReadReplicaContext? readReplicaContext = null)
    {
      IWorkItemTypePropertiesService service = requestContext.GetService<IWorkItemTypePropertiesService>();
      return !WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) || !this.IsCustomType ? service.GetWorkItemTypeDetails(requestContext, this, readReplicaContext) : service.GetCustomWorkItemTypeDetails(requestContext, this, this.Source);
    }

    internal virtual IEnumerable<FieldUsageEntry> GetFieldUsages() => (IEnumerable<FieldUsageEntry>) this.m_usageFields;

    public virtual IReadOnlyCollection<int> GetFieldIds(
      IVssRequestContext requestContext,
      bool includeCalculatedFields = true)
    {
      IEnumerable<int> second1 = Enumerable.Empty<int>();
      if (this.IsDerived && this.InheritedWorkItemType.Fields != null)
        second1 = this.InheritedWorkItemType.Fields.Select<WorkItemTypeExtensionFieldEntry, int>((Func<WorkItemTypeExtensionFieldEntry, int>) (f => f.Field.FieldId));
      List<int> second2 = new List<int>();
      if (includeCalculatedFields)
      {
        second2.Add(50);
        second2.Add(37);
        second2.Add(51);
        second2.Add(58);
        second2.Add(-57);
        second2.Add(-404);
      }
      return (IReadOnlyCollection<int>) requestContext.WitContext().FieldDictionary.GetCoreFields().Where<FieldEntry>((Func<FieldEntry, bool>) (fld =>
      {
        if (fld.Usage != InternalFieldUsages.WorkItem)
          return false;
        return !fld.IsIgnored || fld.IsTreeNode;
      })).Select<FieldEntry, int>((Func<FieldEntry, int>) (fld => fld.FieldId)).Union<int>(this.m_usageFields.Select<FieldUsageEntry, int>((Func<FieldUsageEntry, int>) (x => x.FieldId))).Union<int>((IEnumerable<int>) second2).Union<int>(second1).ToList<int>();
    }

    public IEnumerable<FieldEntry> GetFields(
      IVssRequestContext requestContext,
      bool includeCalculatedFields = true)
    {
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      return this.GetFieldIds(requestContext, includeCalculatedFields).Select<int, FieldEntry>((Func<int, FieldEntry>) (x => fieldDict.GetField(x)));
    }

    public Layout GetFormLayout(IVssRequestContext requestContext, bool resolveContributions = true) => requestContext.TraceBlock<Layout>(900823, 900824, "Services", nameof (WorkItemType), "WorkItemType.GetFormLayout", (Func<Layout>) (() =>
    {
      WorkItemTrackingRequestContext witContext = requestContext.WitContext();
      requestContext.GetService<IFormLayoutService>();
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
      if (WorkItemTrackingFeatureFlags.IsSharedProcessEnabled(requestContext) && service.TryGetLatestProjectProcessDescriptor(requestContext, this.ProjectId, out processDescriptor))
      {
        IReadOnlyCollection<ComposedWorkItemType> allWorkItemTypes = requestContext.GetService<IProcessWorkItemTypeService>().GetAllWorkItemTypes(requestContext, processDescriptor.TypeId);
        ComposedWorkItemType composedWorkItemType = allWorkItemTypes.FirstOrDefault<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (t => TFStringComparer.WorkItemTypeReferenceName.Equals(t.ReferenceName, this.ReferenceName) || TFStringComparer.WorkItemTypeReferenceName.Equals(t.ParentTypeRefName, this.ReferenceName)));
        if (composedWorkItemType != null)
          return composedWorkItemType.GetFormLayout(requestContext, resolveContributions);
        string[] strArray = new string[6]
        {
          "composed workitemtype not found from process service for WIT, Name: ",
          this.Name,
          ", RefName: ",
          this.ReferenceName,
          ", Alltypes from process service are: ",
          string.Join(",", allWorkItemTypes.Select<ComposedWorkItemType, string>((Func<ComposedWorkItemType, string>) (t => t.ReferenceName + ":" + t.ParentTypeRefName)))
        };
        requestContext.Trace(900831, TraceLevel.Error, "Services", nameof (WorkItemType), string.Concat(strArray) + string.Format("process info. ProcessName: {0}, processId: {1}", (object) processDescriptor.Name, (object) processDescriptor.TypeId));
      }
      if (processDescriptor == null && WorkItemTrackingFeatureFlags.IsSharedProcessEnabled(requestContext))
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Message", "We are reading form from xml and not from composed process for shared process enabled account");
        properties.Add("ReferenceName", this.ReferenceName);
        properties.Add("ProjectId", (object) this.ProjectId);
        properties.Add("IsDerived", this.IsDerived);
        properties.Add("IsCustomType", this.IsCustomType);
        properties.Add("Id", (object) this.Id);
        properties.Add("IsHosted", requestContext.ExecutionEnvironment.IsHostedDeployment);
        properties.Add("IsOnPremises", requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
        properties.Add("IsDeploymentContext", requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment));
        properties.Add("IsInheritedProcessCustomizationOnlyAccount", WorkItemTrackingFeatureFlags.IsInheritedProcessCustomizationOnlyAccount(requestContext));
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ILegacyMetadataCompatibilityParticipant.GetForm", "NoFormForProjectWithoutProcess", properties);
      }
      IFieldTypeDictionary fieldDict = (IFieldTypeDictionary) null;
      return new FormTransformer(requestContext.RequestTracer).ConvertToNewFormLayout(this.Form, (string) null, this.Name, new Lazy<IEnumerable<Contribution>>((Func<IEnumerable<Contribution>>) (() => !resolveContributions ? Enumerable.Empty<Contribution>() : FormExtensionsUtility.GetFilteredContributions(requestContext))), WellKnownProcessLayout.GetAgileBugLayout(requestContext), (ControlLabelResolver) (controlId =>
      {
        if (fieldDict == null)
          fieldDict = witContext.FieldDictionary;
        FieldEntry field;
        return fieldDict.TryGetField(controlId, out field) ? field.Name : (string) null;
      }));
    }));

    public Layout ApplyHideFieldRuleOnLayout(
      IVssRequestContext requestContext,
      IWorkItemRuleFilter filter,
      Layout layout)
    {
      if (layout == null)
        return (Layout) null;
      layout = layout.Clone();
      ProcessDescriptor processDescriptor;
      if (requestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(requestContext, this.ProjectId, out processDescriptor) && processDescriptor.IsDerived)
      {
        IEnumerable<WorkItemFieldRule> source = Enumerable.Empty<WorkItemFieldRule>();
        if (this.InheritedWorkItemType != null)
          source = this.InheritedWorkItemType.FieldRules ?? source;
        else if (this.Source != null && this.Source.IsCustomType)
          source = this.Source.FieldRules ?? source;
        IEnumerable<WorkItemFieldRule> list = (IEnumerable<WorkItemFieldRule>) source.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (fldrule => fldrule.SelectRules<HideFieldRule>().Where<HideFieldRule>((Func<HideFieldRule, bool>) (hideFldRule => !hideFldRule.IsDisabled)).Any<HideFieldRule>() && fldrule.IsApplicable(filter))).ToList<WorkItemFieldRule>();
        if (list.Any<WorkItemFieldRule>())
        {
          HashSet<string> fieldsToHide = new HashSet<string>(list.Select<WorkItemFieldRule, string>((Func<WorkItemFieldRule, string>) (fldrule => fldrule.Field)), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
          foreach (LayoutNode layoutNode in layout.GetDescendants<Control>().Where<Control>((Func<Control, bool>) (ctrl => fieldsToHide.Contains(ctrl.Id))))
            layoutNode.Visible = new bool?(false);
        }
      }
      return layout;
    }

    internal WorkItemType Clone() => new WorkItemType(this);

    IReadOnlyCollection<WorkItemFieldRule> ILegacyMetadataCompatibilityParticipant.GetRules(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<WorkItemFieldRule>) requestContext.TraceBlock<List<WorkItemFieldRule>>(900818, 900819, "DataAccessLayer", "MetadataCompatibility", "ILegacyMetadataCompatibilityParticipant.GetRules", (Func<List<WorkItemFieldRule>>) (() =>
      {
        List<WorkItemFieldRule> rules = new List<WorkItemFieldRule>();
        if (this.IsDerived)
          rules.AddRange(this.InheritedWorkItemType.FieldRules ?? Enumerable.Empty<WorkItemFieldRule>());
        if (this.IsCustomType)
          rules.AddRange(this.Source.FieldRules ?? Enumerable.Empty<WorkItemFieldRule>());
        return rules;
      }));
    }

    IReadOnlyCollection<FieldEntry> ILegacyMetadataCompatibilityParticipant.GetFields(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<FieldEntry>) requestContext.TraceBlock<List<FieldEntry>>(900820, 900821, "DataAccessLayer", "MetadataCompatibility", "ILegacyMetadataCompatibilityParticipant.GetFields", (Func<List<FieldEntry>>) (() =>
      {
        List<FieldEntry> fields = new List<FieldEntry>();
        fields.AddRange(this.GetFields(requestContext, true));
        return fields;
      }));
    }

    string ILegacyMetadataCompatibilityParticipant.GetForm(IVssRequestContext requestContext) => requestContext.TraceBlock<string>(900821, 900822, "DataAccessLayer", "MetadataCompatibility", "ILegacyMetadataCompatibilityParticipant.GetForm", (Func<string>) (() =>
    {
      requestContext.WitContext();
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      if (this.IsCustomType)
      {
        FormTransformer formTransformer = new FormTransformer(requestContext.RequestTracer);
        Layout layout1 = LayoutHelper.RemoveDeploymentsControls(this.GetFormLayout(requestContext, false));
        Layout layout2 = this.ApplyHideFieldRuleOnLayout(requestContext, (IWorkItemRuleFilter) requestContext.WitContext().RuleMembershipFilter, layout1);
        return formTransformer.ConvertToNewFormXml(layout2);
      }
      ProcessDescriptor processDescriptor;
      if (WorkItemTrackingFeatureFlags.AreXMLProcessesEnabled(requestContext) && service.TryGetLatestProjectProcessDescriptor(requestContext, this.ProjectId, out processDescriptor) && processDescriptor.IsCustom)
        return this.Form;
      if (this.Form == null)
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Name", this.Name);
        properties.Add("ReferenceName", this.ReferenceName);
        properties.Add("ProjectId", (object) this.ProjectId);
        properties.Add("IsDerived", this.IsDerived);
        properties.Add("IsCustomType", this.IsCustomType);
        properties.Add("Id", (object) this.Id);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ILegacyMetadataCompatibilityParticipant.GetForm", "NullFormEncountered", properties);
      }
      Layout layout = LayoutHelper.RemoveDeploymentsControls(this.GetFormLayout(requestContext, false));
      return new FormTransformer(requestContext.RequestTracer).ConvertToNewFormXml(this.ApplyHideFieldRuleOnLayout(requestContext, (IWorkItemRuleFilter) requestContext.WitContext().RuleMembershipFilter, layout));
    }));
  }
}
