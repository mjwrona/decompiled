// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WitExporter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WitExporter
  {
    private IVssRequestContext m_requestContext;
    private IdentityService m_identityService;
    private ITreeDictionary m_treeService;
    private IWorkItemTypeService m_typeService;
    private IFieldTypeDictionary m_fieldTypeDictionary;
    private IWorkItemTrackingConfigurationInfo m_configurationInfo;
    private ConstantsSearchSession m_searchSession;
    private string m_projectGuidPrefix;
    private int m_treeId;
    private WorkItemType m_workItemType;
    private Guid m_projectGuid;
    private XmlDocument m_doc;
    private XmlElement m_typeElement;
    private XmlElement m_fieldsElement;
    private XmlElement m_statesElement;
    private XmlElement m_transitionsElement;
    private XmlElement m_formElement;
    private Dictionary<int, WitField> m_fieldsMap;
    private Dictionary<WorkflowKey, XmlElement> m_workflowMap;
    private SortedList<int, SetRecord> m_globalListsMap;
    private List<WitExporter.RuleFilter> m_ruleFilters;
    private Func<IEnumerable<RuleRecord>> m_getRuleRecords;
    private WitReadReplicaContext m_readReplicaContext;

    internal static XmlDocument ExportWorkItemType(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string typeName,
      ExportMask flags)
    {
      return new WitExporter(requestContext).ExportWorkItemType(projectGuid, typeName, flags);
    }

    internal static XmlDocument ExportGlobalWorkflow(
      IVssRequestContext requestContext,
      Guid projectGuid,
      ExportMask flags)
    {
      return new WitExporter(requestContext).ExportGlobalWorkflow(projectGuid, flags);
    }

    internal WitExporter(
      IVssRequestContext requestContext,
      Func<IEnumerable<RuleRecord>> getRuleRecords = null)
    {
      this.m_requestContext = requestContext;
      this.m_fieldTypeDictionary = this.m_requestContext.WitContext().FieldDictionary;
      this.m_typeService = this.m_requestContext.GetService<IWorkItemTypeService>();
      this.m_treeService = this.m_requestContext.WitContext().TreeService;
      this.m_identityService = this.m_requestContext.GetService<IdentityService>();
      this.m_searchSession = new ConstantsSearchSession(requestContext);
      this.m_configurationInfo = this.m_requestContext.WitContext().ServerSettings;
      this.m_getRuleRecords = getRuleRecords;
      this.m_readReplicaContext = new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica");
    }

    private void PopulateProjectAndTypeInfo(Guid projectGuid, string typeName)
    {
      this.m_projectGuid = projectGuid;
      if (projectGuid != Guid.Empty)
        this.m_treeId = this.m_treeService.GetTreeNode(projectGuid, projectGuid).Id;
      this.m_projectGuidPrefix = this.m_projectGuid.ToString() + "\\";
      if (!string.IsNullOrEmpty(typeName))
        this.m_workItemType = this.m_typeService.GetWorkItemTypeByReferenceName(this.m_requestContext, this.m_projectGuid, typeName);
      else
        this.m_workItemType = this.m_typeService.GetWorkItemTypeById(this.m_requestContext, projectGuid, -this.m_treeId);
    }

    private bool DoesConstantReferToIdentityGroup(int constId)
    {
      ConstantRecord constantRecord = this.m_searchSession.GetConstantRecord(constId, new WitReadReplicaContext?(this.m_readReplicaContext));
      if (constantRecord == null)
        return false;
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      try
      {
        identity = this.m_identityService.ReadIdentities(this.m_requestContext, (IList<Guid>) new Guid[1]
        {
          constantRecord.TeamFoundationId
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
      if (identity != null)
        return identity.IsContainer;
      return false;
    }

    internal XmlDocument ExportWorkItemType(Guid projectGuid, string typeName, ExportMask flags)
    {
      this.PopulateProjectAndTypeInfo(projectGuid, typeName);
      this.m_ruleFilters = new List<WitExporter.RuleFilter>();
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.FormFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.RuleScopeFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.StateValuesFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ReasonValuesFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.HelpTextFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.RequiredFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ValidUserFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.AllowedValuesFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.SuggestedValuesFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ProhibitedValuesFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ReadOnlyFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.CopyDefaultFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.EmptyFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.CannotLoseValueFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.NotSameAsFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.DefaultReasonFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.TransitionPermissionsFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.MatchFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.FrozenFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ServerDefaultFilter));
      this.m_doc = new XmlDocument();
      XmlElement xmlElement1 = (XmlElement) this.m_doc.AppendChild((XmlNode) this.m_doc.CreateElement(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) ProvisionTags.TypePrefix, (object) ProvisionTags.Root), ProvisionValues.TypesNamespace));
      xmlElement1.SetAttribute(ProvisionAttributes.Application, ProvisionValues.Application);
      xmlElement1.SetAttribute(ProvisionAttributes.AppVersion, ProvisionValues.AppVersion);
      this.m_typeElement = (XmlElement) xmlElement1.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.WorkItemType));
      this.m_typeElement.SetAttribute(ProvisionAttributes.WorkItemTypeName, this.m_workItemType.Name);
      this.m_fieldsElement = (XmlElement) this.m_typeElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.FieldDefinitions));
      XmlElement xmlElement2 = (XmlElement) this.m_typeElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Workflow));
      this.m_statesElement = (XmlElement) xmlElement2.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.States));
      this.m_transitionsElement = (XmlElement) xmlElement2.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Transitions));
      this.m_formElement = (XmlElement) this.m_typeElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Form));
      this.m_fieldsMap = new Dictionary<int, WitField>();
      this.m_workflowMap = new Dictionary<WorkflowKey, XmlElement>();
      this.ExportDescription(this.m_workItemType);
      if ((flags & ExportMask.ExportGlobalLists) != ExportMask.None)
        this.ExportGlobalLists(this.m_typeElement);
      this.ExportFields(FieldSource.WorkItemType);
      this.ExportRules();
      this.ExportActions();
      this.FixAllowExistingValue();
      this.FixDefaultReason();
      return this.m_doc;
    }

    private XmlDocument ExportGlobalWorkflow(Guid projectGuid, ExportMask flags)
    {
      this.PopulateProjectAndTypeInfo(projectGuid, (string) null);
      FieldSource targetFieldSource;
      if (projectGuid != Guid.Empty)
      {
        targetFieldSource = FieldSource.ProjectGlobalWorkflow;
      }
      else
      {
        targetFieldSource = FieldSource.CollectionGlobalWorkflow;
        this.m_projectGuidPrefix = Guid.NewGuid().ToString() + "\\";
      }
      this.m_ruleFilters = new List<WitExporter.RuleFilter>();
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.RuleGlobalScopeFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.RequiredFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ValidUserFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.AllowedValuesFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.SuggestedValuesFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ProhibitedValuesFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ReadOnlyFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.CopyDefaultFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.EmptyFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.CannotLoseValueFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.NotSameAsFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.MatchFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.FrozenFilter));
      this.m_ruleFilters.Add(new WitExporter.RuleFilter(this.ServerDefaultFilter));
      this.m_fieldsMap = new Dictionary<int, WitField>();
      this.m_doc = new XmlDocument();
      XmlElement root = (XmlElement) this.m_doc.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.GlobalWorkflow));
      this.m_fieldsElement = (XmlElement) root.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.FieldDefinitions));
      if ((flags & ExportMask.ExportGlobalLists) != ExportMask.None)
        this.ExportGlobalLists(root);
      this.ExportFields(targetFieldSource);
      this.ExportRules();
      this.FixAllowExistingValue();
      if (!this.m_fieldsElement.HasChildNodes)
        root.RemoveChild((XmlNode) this.m_fieldsElement);
      return this.m_doc;
    }

    private void ExportDescription(WorkItemType type)
    {
      string description = type.Description;
      if (string.IsNullOrEmpty(description))
        return;
      this.m_typeElement.PrependChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Description)).InnerText = description;
    }

    private void ExportFields(FieldSource targetFieldSource)
    {
      List<WitField> witFieldList = new List<WitField>();
      IReadOnlyCollection<FieldEntry> allFields = this.m_fieldTypeDictionary.GetAllFields();
      HashSet<int> typeFields = new HashSet<int>((IEnumerable<int>) this.m_workItemType.GetFieldIds(this.m_requestContext));
      Dictionary<int, FieldSource> dictionary = this.m_workItemType.GetFieldUsages().ToDictionary<FieldUsageEntry, int, FieldSource>((Func<FieldUsageEntry, int>) (x => x.FieldId), (Func<FieldUsageEntry, FieldSource>) (x => x.FieldSource));
      Func<FieldEntry, bool> predicate = (Func<FieldEntry, bool>) (x =>
      {
        if (x.IsIgnored || x.Usage != InternalFieldUsages.WorkItem)
          return false;
        return x.IsCore && targetFieldSource == FieldSource.WorkItemType || typeFields.Contains(x.FieldId);
      });
      foreach (FieldEntry f in allFields.Where<FieldEntry>(predicate))
      {
        FieldSource fieldSource;
        if (dictionary.TryGetValue(f.FieldId, out fieldSource))
        {
          if (!fieldSource.HasFlag((Enum) targetFieldSource))
            continue;
        }
        else if (targetFieldSource != FieldSource.WorkItemType)
          continue;
        WitField witField = new WitField(this.m_doc, f);
        if (!witField.IsIgnored)
        {
          witFieldList.Add(witField);
          this.m_fieldsMap[witField.Id] = witField;
        }
      }
      witFieldList.Sort();
      foreach (WitField witField in witFieldList)
        this.m_fieldsElement.AppendChild((XmlNode) witField.Element);
    }

    private void ExportGlobalLists(XmlElement root)
    {
      SortedList<int, SetRecord> globalLists = this.GlobalLists;
      if (globalLists.Count <= 0)
        return;
      XmlElement xmlElement = (XmlElement) root.InsertBefore((XmlNode) this.m_doc.CreateElement(ProvisionTags.GlobalLists), (XmlNode) this.m_fieldsElement);
      IDictionary<int, SetRecord[]> lists = this.GetLists(globalLists.Select<KeyValuePair<int, SetRecord>, int>((Func<KeyValuePair<int, SetRecord>, int>) (l => l.Key)).ToArray<int>());
      this.GetScopedConstants(lists.Values.SelectMany<SetRecord[], int>((Func<SetRecord[], IEnumerable<int>>) (r => ((IEnumerable<SetRecord>) r).Select<SetRecord, int>((Func<SetRecord, int>) (rec => rec.ItemId)))));
      for (int index = 0; index < globalLists.Count; ++index)
      {
        int key = globalLists.Keys[index];
        string str = globalLists.Values[index].Item;
        if (str.Length > 0 && str[0] == '*')
          str = str.Remove(0, 1);
        else
          Trace.WriteLine("Global list name doesn't begin with an asterisk!");
        XmlElement listElement = (XmlElement) xmlElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.GlobalList));
        listElement.SetAttribute(ProvisionAttributes.GlobalListName, str);
        this.ExportList((IEnumerable<SetRecord>) lists[key], 0, listElement);
      }
    }

    private IDictionary<int, SetRecord[]> GetLists(int[] constIds) => (IDictionary<int, SetRecord[]>) this.m_searchSession.ExpandConstantSets(constIds).ToDictionary<KeyValuePair<ConstantSetReference, SetRecord[]>, int, SetRecord[]>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, int>) (kvp => kvp.Key.Id), (Func<KeyValuePair<ConstantSetReference, SetRecord[]>, SetRecord[]>) (kvp => kvp.Value));

    private void ExportList(
      IEnumerable<SetRecord> setReferences,
      int fieldId,
      XmlElement listElement)
    {
      bool isGlobalList = string.Equals(listElement.LocalName, ProvisionTags.GlobalList, StringComparison.Ordinal);
      foreach (SetRecord setReference in setReferences)
        this.ExportConst(setReference.ItemId, setReference.Item, fieldId, listElement, isGlobalList);
    }

    private void ExportConst(
      int constId,
      string constName,
      int fieldId,
      XmlElement listElement,
      bool isGlobalList)
    {
      string str;
      string name1;
      string name2;
      if (!isGlobalList && this.GlobalLists.ContainsKey(constId))
      {
        str = constName;
        name1 = ProvisionTags.GlobalList;
        name2 = ProvisionAttributes.GlobalListName;
        if (str.Length > 0 && str[0] == '*')
          str = str.Remove(0, 1);
      }
      else
      {
        str = this.GetScopedConstant(constId);
        if (fieldId != 0)
          str = WitExporter.NormalizeValue(str, this.m_fieldsMap[fieldId].Type);
        name1 = ProvisionTags.ListItem;
        name2 = ProvisionAttributes.ListItemValue;
      }
      ((XmlElement) listElement.AppendChild((XmlNode) this.m_doc.CreateElement(name1))).SetAttribute(name2, str);
    }

    private void ExportRules()
    {
      IEnumerable<RuleRecord> ruleRecords = this.GetRuleRecords();
      this.CacheRuleConstants(ruleRecords);
      using (IEnumerator<RuleRecord> enumerator = ruleRecords.GetEnumerator())
      {
label_5:
        while (enumerator.MoveNext())
        {
          RuleRecord current = enumerator.Current;
          int index = 0;
          while (true)
          {
            if (index < this.m_ruleFilters.Count && !this.m_ruleFilters[index](current))
              ++index;
            else
              goto label_5;
          }
        }
      }
    }

    private void CacheRuleConstants(IEnumerable<RuleRecord> ruleRecords)
    {
      this.GetLists(ruleRecords.Where<RuleRecord>((Func<RuleRecord, bool>) (r => this.FIsValidRuleContext(r, this.m_treeId, this.m_workItemType))).Where<RuleRecord>((Func<RuleRecord, bool>) (r =>
      {
        if (this.IsAllowedValuesRule(r) || this.IsSuggestedValuesRule(r))
          return true;
        return this.IsProhibitedValuesRule(r) && !this.ProhibitedValuesRuleReferencesInternalList(r);
      })).Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.ThenConstID)).Where<int>((Func<int, bool>) (i => i != 0)).ToArray<int>());
      this.m_searchSession.GetConstantRecords(Enumerable.Empty<int>().Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.Fld1IsConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.Fld1WasConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.Fld2IsConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.Fld2WasConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.Fld3IsConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.Fld3WasConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.Fld4IsConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.Fld4WasConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.If2ConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.IfConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.ThenConstID))).Concat<int>(ruleRecords.Select<RuleRecord, int>((Func<RuleRecord, int>) (r => r.PersonID))).Where<int>((Func<int, bool>) (i => i != 0)).Distinct<int>().ToArray<int>(), new WitReadReplicaContext?(this.m_readReplicaContext));
    }

    private IEnumerable<RuleRecord> GetRuleRecords() => this.m_getRuleRecords != null ? this.m_getRuleRecords() : CompatibilityRulesGenerator.GetRules(this.m_requestContext, this.m_treeId, this.m_workItemType.Name);

    private void ExportActions()
    {
      Dictionary<ActionKey, List<string>> dictionary = new Dictionary<ActionKey, List<string>>();
      foreach (KeyValuePair<WorkItemTypeTransition, HashSet<string>> action in (IEnumerable<KeyValuePair<WorkItemTypeTransition, HashSet<string>>>) this.m_workItemType.GetAdditionalProperties(this.m_requestContext, new WitReadReplicaContext?(this.m_readReplicaContext)).Actions)
      {
        ActionKey key = new ActionKey(action.Key.From, action.Key.To);
        dictionary[key] = action.Value.ToList<string>();
      }
      foreach (KeyValuePair<ActionKey, List<string>> keyValuePair in dictionary)
      {
        XmlElement xmlElement = (XmlElement) this.FindAddTransition(keyValuePair.Key.fromState, keyValuePair.Key.toState).AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Actions));
        for (int index = 0; index < keyValuePair.Value.Count; ++index)
          ((XmlElement) xmlElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Action))).SetAttribute(ProvisionAttributes.ListItemValue, keyValuePair.Value[index]);
      }
    }

    private void FixAllowExistingValue()
    {
      XmlNodeList xmlNodeList = this.m_doc.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "//*[@{0}]", (object) InternalAttributes.AllowExistingValues));
      for (int i = 0; i < xmlNodeList.Count; ++i)
      {
        if (xmlNodeList[i] is XmlElement xmlElement)
        {
          XmlElement element = this.m_doc.CreateElement(ProvisionTags.AllowExistingValueRule);
          XmlElement refChild = (XmlElement) xmlElement.SelectSingleNode(ProvisionTags.HelpTextRule);
          xmlElement.InsertAfter((XmlNode) element, (XmlNode) refChild);
          xmlElement.RemoveAttribute(InternalAttributes.AllowExistingValues);
        }
      }
    }

    private void FixDefaultReason()
    {
      XmlNodeList xmlNodeList = this.m_doc.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "//{0}[@{1}]", (object) ProvisionTags.Reason, (object) InternalAttributes.DefaultReason));
      for (int i = 0; i < xmlNodeList.Count; ++i)
      {
        if (xmlNodeList[i] is XmlElement oldChild)
        {
          XmlElement parentNode = (XmlElement) oldChild.ParentNode;
          string attribute = oldChild.GetAttribute(ProvisionAttributes.ListItemValue);
          XmlElement element = this.m_doc.CreateElement(ProvisionTags.DefaultReason);
          while (oldChild.ChildNodes.Count > 0)
            element.AppendChild(oldChild.ChildNodes[0]);
          element.SetAttribute(ProvisionAttributes.ListItemValue, attribute);
          parentNode.RemoveChild((XmlNode) oldChild);
          parentNode.PrependChild((XmlNode) element);
        }
      }
    }

    private string ProjectGuid => this.m_projectGuidPrefix;

    private string GlobalGuid => "488bb442-0beb-4c1e-98b6-4eddc604bd9e\\";

    private string InstanceGuid => AccountHelper.GetInstanceId(this.m_requestContext);

    private SortedList<int, SetRecord> GlobalLists
    {
      get
      {
        if (this.m_globalListsMap == null)
        {
          this.m_globalListsMap = new SortedList<int, SetRecord>();
          foreach (SetRecord setRecord in this.m_searchSession.ExpandNonIdentityConstantSet("299f07ef-6201-41b3-90fc-03eeb3977587").SelectMany<KeyValuePair<ConstantSetReference, SetRecord[]>, SetRecord>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, IEnumerable<SetRecord>>) (x => (IEnumerable<SetRecord>) x.Value)))
            this.m_globalListsMap[setRecord.ItemId] = setRecord;
        }
        return this.m_globalListsMap;
      }
    }

    private WitField FindAddField(int fieldId)
    {
      WitField addField;
      FieldEntry field;
      if (!this.m_fieldsMap.TryGetValue(fieldId, out addField) && this.m_fieldTypeDictionary.TryGetField(fieldId, out field))
      {
        addField = new WitField(this.m_doc, field);
        if (!addField.IsIgnored)
        {
          this.m_fieldsMap[fieldId] = addField;
          this.m_fieldsElement.AppendChild((XmlNode) addField.Element);
        }
      }
      return addField;
    }

    private XmlElement FindAddState(string state)
    {
      WorkflowKey key = new WorkflowKey(state);
      XmlElement addState;
      if (!this.m_workflowMap.TryGetValue(key, out addState))
      {
        addState = (XmlElement) this.m_statesElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.State));
        addState.SetAttribute(ProvisionAttributes.ListItemValue, key.fromState);
        this.m_workflowMap[key] = addState;
      }
      return addState;
    }

    private XmlElement FindAddTransition(string fromState, string toState)
    {
      WorkflowKey key = new WorkflowKey(fromState, toState);
      XmlElement addTransition;
      if (!this.m_workflowMap.TryGetValue(key, out addTransition))
      {
        addTransition = (XmlElement) this.m_transitionsElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Transition));
        addTransition.SetAttribute(ProvisionAttributes.OriginalState, key.fromState);
        addTransition.SetAttribute(ProvisionAttributes.NewState, key.toState);
        addTransition.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Reasons));
        this.m_workflowMap[key] = addTransition;
      }
      return addTransition;
    }

    private XmlElement AddReason(XmlElement reasonsElement, WorkflowKey workflow)
    {
      XmlElement xmlElement = (XmlElement) reasonsElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Reason));
      xmlElement.SetAttribute(ProvisionAttributes.ListItemValue, workflow.reason);
      this.m_workflowMap[workflow] = xmlElement;
      return xmlElement;
    }

    private XmlElement FindAddReason(string fromState, string toState, string reason)
    {
      WorkflowKey key = new WorkflowKey(fromState, toState, reason);
      XmlElement addReason;
      if (!this.m_workflowMap.TryGetValue(key, out addReason))
      {
        addReason = (XmlElement) this.FindAddTransition(key.fromState, key.toState).SelectSingleNode(ProvisionTags.Reasons).PrependChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.Reason));
        addReason.SetAttribute(ProvisionAttributes.ListItemValue, key.reason);
        this.m_workflowMap[key] = addReason;
      }
      return addReason;
    }

    private XmlElement FindAddTargetElement(RuleRecord rule)
    {
      try
      {
        WitField addField = this.FindAddField(rule.ThenFldID);
        XmlElement addTargetElement;
        if (rule.Fld2ID != 2)
        {
          addTargetElement = addField.Element;
        }
        else
        {
          if (rule.Fld3ID == 22)
          {
            WorkflowKey workflowKey = new WorkflowKey(rule.Fld2Was, rule.Fld2Is, rule.Fld3Is);
            if (!this.m_workflowMap.TryGetValue(workflowKey, out addTargetElement))
              addTargetElement = this.AddReason((XmlElement) this.FindAddTransition(workflowKey.fromState, workflowKey.toState).SelectSingleNode(ProvisionTags.Reasons), workflowKey);
          }
          else
            addTargetElement = rule.Fld2WasConstID == 0 ? this.FindAddState(rule.Fld2Is) : this.FindAddTransition(rule.Fld2Was, rule.Fld2Is);
          addTargetElement = (XmlElement) addTargetElement.SelectSingleNode(ProvisionTags.FieldReferences) ?? (XmlElement) addTargetElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.FieldReferences));
          XmlElement xmlElement = (XmlElement) null;
          if (addTargetElement.HasChildNodes)
          {
            string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}[@{1}=\"{2}\"]", (object) ProvisionTags.FieldReference, (object) ProvisionAttributes.FieldReferenceName, (object) addField.ReferenceName);
            xmlElement = (XmlElement) addTargetElement.SelectSingleNode(xpath);
          }
          if (xmlElement == null)
          {
            xmlElement = (XmlElement) addTargetElement.AppendChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.FieldReference));
            xmlElement.SetAttribute(ProvisionAttributes.FieldReferenceName, addField.ReferenceName);
          }
          addTargetElement = xmlElement;
        }
        if (WitExporter.FIsConditionalRule(rule))
        {
          string fieldName = this.SafeGetFieldName(rule.IfFldID);
          string name;
          string x;
          if (rule.IfConstID == -10001)
          {
            if ((rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.IfNot) != Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None)
            {
              name = ProvisionTags.WhenChangedCondition;
              x = (string) null;
            }
            else
            {
              name = ProvisionTags.WhenNotChangedCondition;
              x = (string) null;
            }
          }
          else
          {
            name = (rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.IfNot) != Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None ? ProvisionTags.WhenNotCondition : ProvisionTags.WhenCondition;
            x = WitExporter.NormalizeValue(this.GetScopedConstant(rule.IfConstID), this.m_fieldsMap[rule.IfFldID].Type);
          }
          XmlElement xmlElement = (XmlElement) null;
          string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[@{1}=\"{2}\"]", (object) name, (object) ProvisionAttributes.FieldReference, (object) fieldName);
          foreach (XmlElement selectNode in addTargetElement.SelectNodes(xpath))
          {
            if (x == null)
            {
              xmlElement = selectNode;
              break;
            }
            string attribute = selectNode.GetAttribute(ProvisionAttributes.FieldValue);
            if (VssStringComparer.FieldName.Compare(x, attribute) == 0)
            {
              xmlElement = selectNode;
              break;
            }
          }
          if (xmlElement == null)
          {
            xmlElement = (XmlElement) addTargetElement.AppendChild((XmlNode) this.m_doc.CreateElement(name));
            xmlElement.SetAttribute(ProvisionAttributes.FieldReference, fieldName);
            if (x != null)
              xmlElement.SetAttribute(ProvisionAttributes.FieldValue, x);
          }
          addTargetElement = xmlElement;
        }
        return addTargetElement;
      }
      catch (KeyNotFoundException ex)
      {
        Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Rule {0} refers to a non-existing field.", (object) rule.RuleID));
        return this.m_doc.CreateElement(InternalTags.Ignore);
      }
    }

    private XmlElement AddRuleElement(string tagName, RuleRecord rule)
    {
      try
      {
        XmlElement addTargetElement = this.FindAddTargetElement(rule);
        XmlElement xmlElement = (XmlElement) addTargetElement.AppendChild((XmlNode) this.m_doc.CreateElement(tagName));
        if (rule.PersonID != -1)
        {
          string name = (rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.InversePerson) != Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None ? ProvisionAttributes.RuleDontApplyTo : ProvisionAttributes.RuleApplyTo;
          string scopedConstant = this.GetScopedConstant(rule.PersonID);
          xmlElement.SetAttribute(name, scopedConstant);
        }
        if ((rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.DenyWrite) != Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None && (rule.RuleFlags2 & RuleFlags2.ThenImplicitUnchanged) != RuleFlags2.None)
          addTargetElement.SetAttribute(InternalAttributes.AllowExistingValues, XmlConvert.ToString(true));
        return xmlElement;
      }
      catch (Exception ex)
      {
        WitField addField = this.FindAddField(rule.ThenFldID);
        XmlElement addTargetElement = this.FindAddTargetElement(rule);
        TeamFoundationTracingService.TraceRaw(921000, TraceLevel.Error, nameof (WitExporter), nameof (AddRuleElement), ex.Message + "\n" + "tag: " + tagName + " \n" + "Rule: " + rule?.ToDebugString() + " \n" + string.Format("field is: {0} \n", (object) addField.Element) + string.Format("parentElement is: {0} \n", (object) addTargetElement));
        return this.m_doc.CreateElement(InternalTags.Ignore);
      }
    }

    private XmlElement AddListRuleElement(string tagName, RuleRecord rule)
    {
      XmlElement listElement = this.AddRuleElement(tagName, rule);
      bool flag = (rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.ThenTwoPlusLevels) != 0;
      listElement.SetAttribute(ProvisionAttributes.ExpandItems, XmlConvert.ToString(flag));
      if ((rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.ThenInterior) == Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None)
        listElement.SetAttribute(ProvisionAttributes.FilterItems, ProvisionValues.ExcludeGroups);
      this.ExportList((IEnumerable<SetRecord>) this.GetLists(new int[1]
      {
        rule.ThenConstID
      }).Values.Single<SetRecord[]>(), rule.ThenFldID, listElement);
      return listElement;
    }

    private void AddCopyDefaultRule(string tagName, RuleRecord rule)
    {
      XmlElement xmlElement = this.AddRuleElement(tagName, rule);
      string str1 = (string) null;
      string name = (string) null;
      string str2 = (string) null;
      if (rule.ThenConstID > 0 || rule.ThenConstID == -10000)
      {
        str1 = ProvisionValues.SourceValue;
        name = ProvisionAttributes.RuleValue;
        str2 = WitExporter.NormalizeValue(this.GetScopedConstant(rule.ThenConstID), this.m_fieldsMap[rule.ThenFldID].Type);
      }
      else
      {
        switch (rule.ThenConstID)
        {
          case -10028:
            str1 = ProvisionValues.SourceClock;
            break;
          case -10012:
            str1 = ProvisionValues.SourceField;
            name = ProvisionAttributes.FieldReference;
            str2 = this.SafeGetFieldName(rule.If2FldID);
            break;
          case -10002:
            str1 = ProvisionValues.SourceCurrentUser;
            break;
        }
      }
      xmlElement.SetAttribute(ProvisionAttributes.RuleSource, str1);
      if (name == null)
        return;
      xmlElement.SetAttribute(name, str2);
    }

    private bool StateValuesFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 83886224L) != 83886224L || ((long) (uint) rule.RuleFlags & 416018432L) != 0L || ((long) (uint) rule.RuleFlags2 & 352L) != 0L || rule.Fld2ID != 0 || rule.IfFldID != 0 || rule.ThenConstID <= 0 || rule.ThenFldID != 2 || rule.If2FldID != 0)
        return false;
      foreach (string state in this.m_searchSession.ExpandConstantSets(new int[1]
      {
        rule.ThenConstID
      }).SelectMany<KeyValuePair<ConstantSetReference, SetRecord[]>, SetRecord>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, IEnumerable<SetRecord>>) (x => (IEnumerable<SetRecord>) x.Value)).Select<SetRecord, string>((Func<SetRecord, string>) (x => x.Item)))
        this.FindAddState(state);
      return true;
    }

    private bool ReasonValuesFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 83886224L) != 83886224L || ((long) (uint) rule.RuleFlags & 416018432L) != 0L || ((long) (uint) rule.RuleFlags2 & 352L) != 0L || rule.Fld2ID != 2 || rule.Fld2WasConstID == 0 || rule.Fld3ID != 0 || rule.ThenConstID <= 0 || rule.ThenFldID != 22 || rule.IfFldID != 0 || rule.If2FldID != 0)
        return false;
      string fld2Was = rule.Fld2Was;
      string fld2Is = rule.Fld2Is;
      XmlElement reasonsElement = (XmlElement) this.FindAddTransition(fld2Was, fld2Is).SelectSingleNode(ProvisionTags.Reasons);
      foreach (string reason in this.m_searchSession.ExpandConstantSets(new int[1]
      {
        rule.ThenConstID
      }).SelectMany<KeyValuePair<ConstantSetReference, SetRecord[]>, SetRecord>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, IEnumerable<SetRecord>>) (x => (IEnumerable<SetRecord>) x.Value)).Select<SetRecord, string>((Func<SetRecord, string>) (x => x.Item)))
      {
        WorkflowKey workflowKey = new WorkflowKey(fld2Was, fld2Is, reason);
        if (!this.m_workflowMap.ContainsKey(workflowKey))
          this.AddReason(reasonsElement, workflowKey);
      }
      return true;
    }

    private bool HelpTextFilter(RuleRecord rule)
    {
      try
      {
        if (((long) (uint) rule.RuleFlags & 536870912L) != 536870912L || ((long) (uint) rule.RuleFlags & 265023488L) != 0L || rule.PersonID != -1 || rule.IfFldID != 0 || rule.If2FldID != 0 || rule.Fld2ID != 0)
          return false;
        this.FindAddTargetElement(rule).PrependChild((XmlNode) this.m_doc.CreateElement(ProvisionTags.HelpTextRule)).InnerText = rule.Then;
        return true;
      }
      catch (Exception ex)
      {
        WitField addField = this.FindAddField(rule.ThenFldID);
        XmlElement addTargetElement = this.FindAddTargetElement(rule);
        TeamFoundationTracingService.TraceRaw(921000, TraceLevel.Error, nameof (WitExporter), "AddRuleElement", ex.Message + "\n" + "Rule: " + rule?.ToDebugString() + " \n" + string.Format("field is: {0} \n", (object) addField.Element) + string.Format("parentElement is: {0} \n", (object) addTargetElement));
        return false;
      }
    }

    private bool RequiredFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 4194448L) != 4194448L || ((long) (uint) rule.RuleFlags & 260046848L) != 0L || rule.ThenConstID != -10000 || rule.If2FldID != 0)
        return false;
      this.AddRuleElement(ProvisionTags.RequiredRule, rule);
      return true;
    }

    private bool IsAllowedValuesRule(RuleRecord rule) => ((long) (uint) rule.RuleFlags & 83886224L) == 83886224L && (rule.RuleFlags2 & RuleFlags2.ThenImplicitEmpty) != RuleFlags2.None && ((long) (uint) rule.RuleFlags & 281018368L) == 0L && rule.ThenConstID > 0 && rule.If2FldID == 0;

    private bool AllowedValuesFilter(RuleRecord rule)
    {
      if (!this.IsAllowedValuesRule(rule))
        return false;
      this.AddListRuleElement(ProvisionTags.AllowedValuesRule, rule);
      return true;
    }

    private bool IsSuggestedValuesRule(RuleRecord rule) => ((long) (uint) rule.RuleFlags & 83887104L) == 83887104L && ((long) (uint) rule.RuleFlags & 281018368L) == 0L && rule.ThenConstID > 0 && rule.If2FldID == 0;

    private bool SuggestedValuesFilter(RuleRecord rule)
    {
      if (!this.IsSuggestedValuesRule(rule))
        return false;
      this.AddListRuleElement(ProvisionTags.SuggestedValuesRule, rule);
      return true;
    }

    private bool IsProhibitedValuesRule(RuleRecord rule) => ((long) (uint) rule.RuleFlags & 71303312L) == 71303312L && ((long) (uint) rule.RuleFlags & 276824064L) == 0L && rule.ThenConstID > 0 && rule.If2FldID == 0;

    private bool ProhibitedValuesRuleReferencesInternalList(RuleRecord rule)
    {
      string then = rule.Then;
      return then.Length > 0 && then[0] == '-' && (rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.ThenInterior) != Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None;
    }

    private bool ProhibitedValuesFilter(RuleRecord rule)
    {
      if (!this.IsProhibitedValuesRule(rule))
        return false;
      if (this.ProhibitedValuesRuleReferencesInternalList(rule))
        return true;
      this.AddListRuleElement(ProvisionTags.ProhibitedValuesRule, rule);
      return true;
    }

    private bool ReadOnlyFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 144L) != 144L || ((long) (uint) rule.RuleFlags & 264241152L) != 0L || rule.ThenConstID != -10001 || rule.If2FldID != 0)
        return false;
      this.AddRuleElement(ProvisionTags.ReadOnlyRule, rule);
      return true;
    }

    private bool CopyDefaultFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 512L) != 512L || ((long) (uint) rule.RuleFlags & 264241152L) != 0L || rule.ThenFldID == 2 || rule.ThenFldID == 22 || rule.ThenConstID == -10013 || rule.ThenConstID == -10026 || rule.ThenConstID == -10031)
        return false;
      bool flag = false;
      if (rule.Fld4ID != 0)
        flag = rule.Fld4ID == rule.ThenFldID;
      else if (rule.Fld3ID != 0)
        flag = rule.Fld3ID == rule.ThenFldID;
      else if (rule.Fld2ID != 0)
        flag = rule.Fld2ID == rule.ThenFldID;
      else if (rule.Fld1ID != 0)
        flag = rule.Fld1ID == rule.ThenFldID;
      this.AddCopyDefaultRule(flag ? ProvisionTags.DefaultRule : ProvisionTags.CopyRule, rule);
      return true;
    }

    private bool ValidUserFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 251658384L) != 251658384L || (rule.RuleFlags2 & RuleFlags2.ThenImplicitEmpty) == RuleFlags2.None || ((long) (uint) rule.RuleFlags & 281018368L) != 0L || rule.If2FldID != 0 || rule.ThenConstID != -2 && !this.DoesConstantReferToIdentityGroup(rule.ThenConstID))
        return false;
      XmlElement xmlElement = this.AddRuleElement(ProvisionTags.ValidUserRule, rule);
      if (rule.ThenConstID != -2)
      {
        string scopedConstant = this.GetScopedConstant(rule.ThenConstID);
        xmlElement.SetAttribute(ProvisionAttributes.GroupReference, scopedConstant);
      }
      return true;
    }

    private bool EmptyFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 144L) != 144L || ((long) (uint) rule.RuleFlags & 264241152L) != 0L || rule.ThenConstID != -10000 || rule.If2FldID != 0)
        return false;
      this.AddRuleElement(ProvisionTags.EmptyRule, rule);
      return true;
    }

    private bool CannotLoseValueFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 4718736L) != 4718736L || ((long) (uint) rule.RuleFlags & 260046848L) != 0L || rule.If2FldID != rule.ThenFldID || rule.If2ConstID != -10006 || rule.ThenConstID != -10000)
        return false;
      this.AddRuleElement(ProvisionTags.CannotLoseValueRule, rule);
      return true;
    }

    private bool NotSameAsFilter(RuleRecord rule)
    {
      if (((long) rule.RuleFlags & 4194448L) != 4194448L || ((long) rule.RuleFlags & 260046848L) != 0L || rule.If2ConstID != -10025 || rule.ThenConstID != -10025)
        return false;
      string fieldName = this.SafeGetFieldName(rule.If2FldID);
      this.AddRuleElement(ProvisionTags.NotSameAsRule, rule).SetAttribute(ProvisionAttributes.FieldReference, fieldName);
      return true;
    }

    private bool FormFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 512L) != 512L || ((long) (uint) rule.RuleFlags & 265023488L) != 0L || (rule.RuleFlags2 & RuleFlags2.ThenConstLargetext) == RuleFlags2.None || rule.IfFldID != 25 || !TFStringComparer.WorkItemTypeName.Equals(rule.If, this.m_workItemType.Name) || rule.ThenFldID != -14)
        return false;
      string str = rule.Form ?? string.Empty;
      if (str.Length > 0)
      {
        XmlElement element = this.m_doc.CreateElement(InternalTags.Ignore);
        element.InnerXml = str;
        XmlElement formElement = (XmlElement) element.SelectSingleNode(ProvisionTags.Form);
        Lazy<IEnumerable<Contribution>> formContributions = new Lazy<IEnumerable<Contribution>>((Func<IEnumerable<Contribution>>) (() => FormExtensionsUtility.GetFilteredContributions(this.m_requestContext)));
        FieldEntry field;
        new FormTransformer(this.m_requestContext.RequestTracer).TransformForExport(formElement, formContributions, WellKnownProcessLayout.GetAgileBugLayout(this.m_requestContext), (ControlLabelResolver) (controlId => this.m_fieldTypeDictionary.TryGetField(controlId, out field) ? field.Name : (string) null), false);
        this.m_formElement.InnerXml = formElement.InnerXml;
      }
      return true;
    }

    private bool DefaultReasonFilter(RuleRecord rule)
    {
      if (rule.ThenFldID != 22 || ((long) (uint) rule.RuleFlags & 512L) != 512L || ((long) (uint) rule.RuleFlags & 265023488L) != 0L || rule.IfFldID != 0 || rule.If2FldID != 0 || rule.ThenConstID <= 0 || rule.Fld2ID != 2 || rule.Fld2WasConstID == 0 || rule.Fld2IsConstID == 0)
        return false;
      this.FindAddReason(rule.Fld2Was, rule.Fld2Is, rule.Then).SetAttribute(InternalAttributes.DefaultReason, XmlConvert.ToString(true));
      return true;
    }

    private bool TransitionPermissionsFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 16L) != 16L || ((long) (uint) rule.RuleFlags & 265023616L) != 0L || rule.Fld2ID != 2 || rule.IfFldID != 0 || rule.If2FldID != 0 || rule.ThenFldID != 2 || rule.Fld3ID != 0 || rule.PersonID == -1)
        return false;
      XmlElement addTransition = this.FindAddTransition(rule.Fld2Was, rule.Then);
      string scopedConstant = this.GetScopedConstant(rule.PersonID);
      string name = (rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.InversePerson) != Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None ? ProvisionAttributes.TransitionAllowedFor : ProvisionAttributes.TransitionProhibitedFor;
      string str = scopedConstant;
      addTransition.SetAttribute(name, str);
      return true;
    }

    private bool MatchFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 92274832L) != 92274832L || ((long) (uint) rule.RuleFlags2 & 32L) != 32L || ((long) (uint) rule.RuleFlags & 171966464L) != 0L || rule.ThenConstID <= 0 || rule.If2FldID != 0)
        return false;
      foreach (string str in this.m_searchSession.ExpandConstantSets(new int[1]
      {
        rule.ThenConstID
      }).SelectMany<KeyValuePair<ConstantSetReference, SetRecord[]>, SetRecord>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, IEnumerable<SetRecord>>) (x => (IEnumerable<SetRecord>) x.Value)).Select<SetRecord, string>((Func<SetRecord, string>) (x => x.Item)))
        this.AddRuleElement(ProvisionTags.MatchRule, rule).SetAttribute(ProvisionAttributes.MatchPattern, str);
      return true;
    }

    private bool FrozenFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 524432L) != 524432L || ((long) (uint) rule.RuleFlags & 264241152L) != 0L || rule.If2FldID != rule.ThenFldID || rule.If2ConstID != -10000 || rule.ThenConstID != -10022)
        return false;
      this.AddRuleElement(ProvisionTags.FrozenRule, rule);
      return true;
    }

    private bool ServerDefaultFilter(RuleRecord rule)
    {
      if (((long) (uint) rule.RuleFlags & 16L) != 16L || ((long) (uint) rule.RuleFlags & 265019392L) != 0L || rule.If2FldID != 0)
        return false;
      string str;
      if (rule.ThenConstID == -10013)
        str = ProvisionValues.SourceClock;
      else if (rule.ThenConstID == -10026)
      {
        str = ProvisionValues.SourceCurrentUser;
      }
      else
      {
        if (rule.ThenConstID != -10031)
          return false;
        str = ProvisionValues.SourceGuid;
      }
      this.AddRuleElement(ProvisionTags.ServerDefaultRule, rule).SetAttribute(ProvisionAttributes.RuleSource, str);
      return true;
    }

    private bool RuleScopeFilter(RuleRecord rule) => !this.FIsValidRuleContext(rule, this.m_treeId, this.m_workItemType);

    private bool RuleGlobalScopeFilter(RuleRecord rule) => ((long) (uint) rule.RuleFlags & 106L) != 0L || ((long) (uint) rule.RuleFlags & 1L) != 1L || rule.AreaID != this.m_treeId || rule.ThenFldID == -14 || rule.Fld1ID == 25;

    private bool FIsValidRuleContext(RuleRecord rule, int areaID, WorkItemType type) => rule.AreaID == areaID && rule.Fld1ID == 25 && rule.Fld1WasConstID == 0 && rule.Fld1IsConstID != 0 && TFStringComparer.WorkItemTypeName.Equals(this.m_searchSession.GetConstantDisplayPart(rule.Fld1IsConstID, new WitReadReplicaContext?(this.m_readReplicaContext)), type.Name) && (rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.FlowdownTree) != Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None && (rule.RuleFlags & Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.Reverse) == Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags.None && rule.ThenFldID != 0;

    private static bool FIsConditionalRule(RuleRecord rule) => rule.IfFldID != 0;

    private string SafeGetFieldName(int fieldId) => this.FindAddField(fieldId).Element.GetAttribute(ProvisionAttributes.FieldReferenceName);

    private string GetScopedConstant(int constId) => this.GetScopedConstants((IEnumerable<int>) new int[1]
    {
      constId
    }).Values.FirstOrDefault<string>() ?? string.Empty;

    private IDictionary<int, string> GetScopedConstants(IEnumerable<int> constIds)
    {
      IEnumerable<ConstantRecord> constantRecords = this.m_searchSession.GetConstantRecords(constIds.ToArray<int>(), new WitReadReplicaContext?(this.m_readReplicaContext));
      Dictionary<int, string> scopedConstants = new Dictionary<int, string>(constantRecords.Count<ConstantRecord>());
      foreach (ConstantRecord constantRecord in constantRecords)
      {
        string strA = constantRecord == null || constantRecord.StringValue == null ? "" : constantRecord.StringValue;
        if (strA.Length >= this.ProjectGuid.Length)
        {
          if (string.Compare(strA, 0, this.ProjectGuid, 0, this.ProjectGuid.Length, StringComparison.OrdinalIgnoreCase) == 0)
            strA = ProvisionValues.ConstScopeProject + strA.Remove(0, this.ProjectGuid.Length);
          else if (string.Compare(strA, 0, this.GlobalGuid, 0, this.GlobalGuid.Length, StringComparison.OrdinalIgnoreCase) == 0)
            strA = ProvisionValues.ConstScopeGlobal + strA.Remove(0, this.GlobalGuid.Length);
          else if (string.Compare(strA, 0, this.InstanceGuid, 0, this.InstanceGuid.Length, StringComparison.OrdinalIgnoreCase) == 0)
            strA = ProvisionValues.ConstScopeInstance + strA.Remove(0, this.InstanceGuid.Length);
        }
        scopedConstants[constantRecord.Id] = strA;
      }
      return (IDictionary<int, string>) scopedConstants;
    }

    private static string NormalizeValue(string value, PsFieldDefinitionTypeEnum type) => type == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedBoolean && !string.IsNullOrEmpty(value) ? XmlConvert.ToString(XmlConvert.ToBoolean(value)) : value;

    private delegate bool RuleFilter(RuleRecord rule);
  }
}
