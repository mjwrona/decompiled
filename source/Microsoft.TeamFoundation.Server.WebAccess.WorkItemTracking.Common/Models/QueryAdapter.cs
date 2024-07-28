// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.QueryAdapter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  internal class QueryAdapter
  {
    internal const string ListSeparator = ",";
    public static readonly string DefaultLinkFilter = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] <> ''", (object) "System.Links.LinkType");
    public static readonly string DefaultTargetFilter = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] <> ''", (object) "System.WorkItemType");
    protected List<string> m_filterFields;
    private string[] m_workItemTypeCategories;
    private List<WorkItemLinkTypeEnd> m_oneHopLinkTypes;
    private List<WorkItemLinkTypeEnd> m_recursiveLinkTypes;
    private List<string> m_treeLinkTypeNames;
    private List<string> m_linkTypeNames;
    private List<string> m_allOneHopLinkSet;
    private List<string> m_allRecursiveLinkSet;
    private IEnumerable<string> m_emptyLinkSet;
    private readonly CultureInfo m_cultureInfo;
    private bool? m_fieldComparisonSupported;
    private bool? m_workItemLinksSupported;
    private bool? m_useIsoDateFormat;
    private IVssRequestContext m_tfsRequestContext;
    private WebAccessWorkItemService m_witService;
    private WorkItemTrackingFieldService m_witFieldService;

    public QueryAdapter(IVssRequestContext tfsRequestContext, bool useIsoDateFormat = false)
    {
      this.m_tfsRequestContext = tfsRequestContext;
      this.m_witService = tfsRequestContext.GetService<WebAccessWorkItemService>();
      this.m_witFieldService = tfsRequestContext.GetService<WorkItemTrackingFieldService>();
      this.Operators = new WiqlOperators();
      this.PopulateLinkTypes();
      this.PopulateQueriableFieldNames();
      this.m_cultureInfo = this.m_tfsRequestContext.GetService<IUserPreferencesService>().GetUserPreferences(this.m_tfsRequestContext).Culture ?? CultureInfo.CurrentCulture;
      this.m_useIsoDateFormat = new bool?(useIsoDateFormat);
    }

    public WiqlOperators Operators { get; private set; }

    public IEnumerable<WorkItemLinkTypeEnd> OneHopLinkTypes => (IEnumerable<WorkItemLinkTypeEnd>) this.m_oneHopLinkTypes;

    public IEnumerable<WorkItemLinkTypeEnd> RecursiveLinkTypes => (IEnumerable<WorkItemLinkTypeEnd>) this.m_recursiveLinkTypes;

    public List<string> TreeLinkTypeNames => this.m_treeLinkTypeNames;

    public List<string> LinkTypeNames => this.m_linkTypeNames;

    public IEnumerable<string> AllRecursiveLinkSet => (IEnumerable<string>) this.m_allRecursiveLinkSet;

    public IEnumerable<string> AllOneHopLinkSet => (IEnumerable<string>) this.m_allOneHopLinkSet;

    public IEnumerable<string> EmptyLinkSet
    {
      get
      {
        if (this.m_emptyLinkSet == null)
          this.m_emptyLinkSet = (IEnumerable<string>) Array.Empty<string>();
        return this.m_emptyLinkSet;
      }
    }

    public bool IsFieldComparisonSupported
    {
      get
      {
        if (this.m_fieldComparisonSupported.HasValue)
          return this.m_fieldComparisonSupported.Value;
        this.m_fieldComparisonSupported = new bool?(true);
        return this.m_fieldComparisonSupported.Value;
      }
    }

    public bool IsWorkItemLinksSupported
    {
      get
      {
        if (this.m_workItemLinksSupported.HasValue)
          return this.m_workItemLinksSupported.Value;
        this.m_workItemLinksSupported = new bool?(true);
        return this.m_workItemLinksSupported.Value;
      }
    }

    public bool UseIsoDateFormat => this.m_useIsoDateFormat.HasValue && this.m_useIsoDateFormat.Value;

    private IEnumerable<string> GetWorkItemTypeCategories()
    {
      this.m_tfsRequestContext.TraceEnter(518000, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetWorkItemTypeCategories));
      if (this.m_workItemTypeCategories == null)
      {
        HashSet<string> source = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (Project project in this.m_witService.GetProjects(this.m_tfsRequestContext))
          source.UnionWith(this.m_witService.GetWorkItemTypeCategories(this.m_tfsRequestContext, project.Name).Select<WorkItemTypeCategory, string>((Func<WorkItemTypeCategory, string>) (x => x.ReferenceName)));
        this.m_workItemTypeCategories = source.OrderBy<string, string>((Func<string, string>) (c => c), (IComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<string>();
      }
      this.m_tfsRequestContext.TraceLeave(518005, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetWorkItemTypeCategories));
      return (IEnumerable<string>) this.m_workItemTypeCategories;
    }

    private FieldEntry TryGetFieldEntry(string fieldName)
    {
      FieldEntry field = (FieldEntry) null;
      this.m_witFieldService.TryGetField(this.m_tfsRequestContext, fieldName, out field);
      return field;
    }

    public virtual InternalFieldType GetFieldType(string fieldName)
    {
      this.m_tfsRequestContext.Trace(518015, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "GetFieldType: fieldName: {0}", (object) fieldName);
      InternalFieldType fieldType = InternalFieldType.String;
      FieldEntry fieldEntry = this.TryGetFieldEntry(fieldName);
      if (fieldEntry != null)
        fieldType = fieldEntry.FieldId == -42 ? (InternalFieldType) 0 : fieldEntry.FieldType;
      return fieldType;
    }

    public bool IsWorkitemTypeField(string fieldName)
    {
      this.m_tfsRequestContext.Trace(518020, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "IsWorkitemTypeField: fieldName: " + fieldName);
      return this.IsWorkitemTypeField(this.TryGetFieldEntry(fieldName));
    }

    public bool IsWorkitemTypeField(FieldEntry fieldEntry)
    {
      this.m_tfsRequestContext.Trace(518025, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "IsWorkitemTypeField: fieldName: " + fieldEntry?.Name);
      return fieldEntry != null && fieldEntry.FieldId == 25;
    }

    public bool IsTeamProjectField(string fieldName)
    {
      this.m_tfsRequestContext.Trace(518027, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "IsTeamProjectField: fieldName: {0}", (object) fieldName);
      FieldEntry fieldEntry = this.TryGetFieldEntry(fieldName);
      return fieldEntry != null && fieldEntry.FieldId == -42;
    }

    public string GetLocalizedFieldName(string fieldName)
    {
      this.m_tfsRequestContext.Trace(518030, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "GetLocalizedFieldName: fieldName: {0}", (object) fieldName);
      FieldEntry fieldEntry = this.TryGetFieldEntry(fieldName);
      return fieldEntry != null ? fieldEntry.Name : fieldName;
    }

    public string GetInvariantFieldName(string fieldName)
    {
      this.m_tfsRequestContext.Trace(518035, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "GetInvariantFieldName: fieldName: {0}", (object) fieldName);
      FieldEntry fieldEntry = this.TryGetFieldEntry(fieldName);
      return fieldEntry != null ? fieldEntry.ReferenceName : fieldName;
    }

    public virtual bool CanQuery(FieldEntry fieldEntry)
    {
      this.m_tfsRequestContext.Trace(518040, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "CanQuery: fieldName: {0}", (object) fieldEntry?.Name);
      return fieldEntry.IsQueryable && (fieldEntry.Usage & InternalFieldUsages.WorkItem) > InternalFieldUsages.None;
    }

    public bool IsUsernameField(string fieldName)
    {
      this.m_tfsRequestContext.Trace(518045, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "IsUsernameField: fieldName: " + fieldName);
      return this.IsUsernameField(this.TryGetFieldEntry(fieldName));
    }

    public bool IsUsernameField(FieldEntry fieldEntry)
    {
      this.m_tfsRequestContext.Trace(518050, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "IsUsernameField: fieldName: " + fieldEntry?.Name);
      if (fieldEntry == null)
        return false;
      return fieldEntry.FieldId == 24 || fieldEntry.FieldId == 33 || fieldEntry.FieldId == 9 || fieldEntry.FieldId == -1;
    }

    public bool FieldSupportsAnySyntax(string fieldName)
    {
      this.m_tfsRequestContext.Trace(518055, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "FieldSupportsAnySyntax: fieldName: {0}", (object) fieldName);
      return this.FieldSupportsAnySyntax(this.TryGetFieldEntry(fieldName));
    }

    public bool FieldSupportsAnySyntax(FieldEntry fieldEntry)
    {
      this.m_tfsRequestContext.Trace(518060, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "FieldSupportsAnySyntax: fieldName: {0}", (object) fieldEntry?.Name);
      return fieldEntry != null && WiqlAdapter.IsNonNullableField(fieldEntry.FieldId);
    }

    public string GetLocalizedOperator(string op) => this.Operators.GetLocalizedOperator(op);

    public string GetInvariantOperator(string op) => this.Operators.GetInvariantOperator(op);

    public List<string> GetLocalizedOperatorList(IEnumerable<string> invariantOperatorList) => this.Operators.GetLocalizedOperatorList(invariantOperatorList);

    public bool IsFieldComparisonOperator(string localizedOperator) => WiqlOperators.IsFieldComparisonOperator(localizedOperator);

    internal string GetFieldComparisonOperator(string invariantOperator) => this.Operators.GetFieldComparisonOperator(invariantOperator);

    private void PopulateLinkTypes()
    {
      this.m_oneHopLinkTypes = new List<WorkItemLinkTypeEnd>();
      this.m_recursiveLinkTypes = new List<WorkItemLinkTypeEnd>();
      this.m_linkTypeNames = new List<string>();
      this.m_treeLinkTypeNames = new List<string>();
      this.m_allOneHopLinkSet = new List<string>();
      this.m_allRecursiveLinkSet = new List<string>();
      foreach (WorkItemLinkTypeEnd linkTypeEnd in (ReadOnlyCollection<WorkItemLinkTypeEnd>) this.m_witService.GetLinkTypes(this.m_tfsRequestContext).LinkTypeEnds)
      {
        this.m_oneHopLinkTypes.Add(linkTypeEnd);
        this.m_allOneHopLinkSet.Add(linkTypeEnd.ImmutableName);
        this.m_linkTypeNames.Add(linkTypeEnd.Name);
        if (linkTypeEnd.LinkType.LinkTopology == WorkItemLinkType.Topology.Tree && linkTypeEnd.IsForwardLink)
        {
          this.m_recursiveLinkTypes.Add(linkTypeEnd);
          this.m_allRecursiveLinkSet.Add(linkTypeEnd.ImmutableName);
          this.m_treeLinkTypeNames.Add(linkTypeEnd.Name);
        }
      }
      this.m_linkTypeNames.Sort((IComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      this.m_linkTypeNames.Insert(0, CommonClientResourceStrings.WiqlOperators_Any);
    }

    private void PopulateQueriableFieldNames()
    {
      this.m_filterFields = new List<string>();
      foreach (FieldEntry allField in this.m_witFieldService.GetAllFields(this.m_tfsRequestContext))
      {
        if (this.CanQuery(allField))
          this.m_filterFields.Add(allField.Name);
      }
      this.m_filterFields.Sort((IComparer<string>) StringComparer.CurrentCultureIgnoreCase);
    }

    public virtual List<string> GetQueriableFieldNames()
    {
      try
      {
        this.m_tfsRequestContext.TraceEnter(518065, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetQueriableFieldNames));
        return this.m_filterFields;
      }
      finally
      {
        this.m_tfsRequestContext.TraceLeave(518070, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetQueriableFieldNames));
      }
    }

    public virtual List<string> GetAvailableOperators(string localizedFieldName)
    {
      try
      {
        this.m_tfsRequestContext.TraceEnter(518075, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetAvailableOperators));
        List<string> invariantOperatorList = new List<string>();
        FieldEntry fieldEntry = this.TryGetFieldEntry(localizedFieldName);
        InternalFieldType fieldType = this.GetFieldType(localizedFieldName);
        switch (fieldType)
        {
          case (InternalFieldType) 0:
            invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.ProjectOperators);
            break;
          case InternalFieldType.String:
            invariantOperatorList.AddRange(fieldEntry == null || !fieldEntry.SupportsTextQuery ? (IEnumerable<string>) WiqlOperators.StringOperators : (IEnumerable<string>) WiqlOperators.StringWithTextSupportOperators);
            break;
          case InternalFieldType.Integer:
          case InternalFieldType.DateTime:
          case InternalFieldType.Double:
            invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.ComparisonOperators);
            break;
          case InternalFieldType.PlainText:
          case InternalFieldType.Html:
          case InternalFieldType.History:
            invariantOperatorList.AddRange(fieldEntry == null || !fieldEntry.SupportsTextQuery ? (IEnumerable<string>) WiqlOperators.TextOperators : (IEnumerable<string>) WiqlOperators.TextWithTextSupportOperators);
            break;
          case InternalFieldType.TreePath:
            invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.TreePathOperators);
            break;
          case InternalFieldType.Guid:
          case InternalFieldType.Boolean:
            invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.EqualityOperators);
            break;
        }
        if ((fieldEntry == null ? 0 : (fieldEntry.IsComputed ? 1 : 0)) == 0 && ((IEnumerable<InternalFieldType>) WiqlOperators.FieldTypesSupportingEver).Contains<InternalFieldType>(fieldType))
          invariantOperatorList.Add("EVER");
        List<string> localizedOperatorList = this.GetLocalizedOperatorList((IEnumerable<string>) invariantOperatorList);
        if (this.IsFieldComparisonSupported && ((IEnumerable<InternalFieldType>) WiqlOperators.FieldTypesSupportingFieldComparison).Contains<InternalFieldType>(fieldType))
        {
          foreach (string invariantOperator in invariantOperatorList)
          {
            if (((IEnumerable<string>) WiqlOperators.OperatorsSupportingFieldComparison).Contains<string>(invariantOperator))
              localizedOperatorList.Add(this.GetFieldComparisonOperator(invariantOperator));
          }
        }
        return localizedOperatorList;
      }
      finally
      {
        this.m_tfsRequestContext.TraceLeave(518080, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetAvailableOperators));
      }
    }

    public virtual List<string> GetAvailableValues(
      Project currentProject,
      string localizedFieldName,
      string localizedOperator)
    {
      return this.GetAvailableValues(currentProject, localizedFieldName, localizedOperator, true);
    }

    public virtual List<string> GetAvailableValues(
      Project currentProject,
      string localizedFieldName,
      string localizedOperator,
      bool includePredefinedValues)
    {
      try
      {
        this.m_tfsRequestContext.TraceEnter(518085, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetAvailableValues));
        int projectId = 0;
        if (currentProject != null)
          projectId = currentProject.Id;
        List<string> availableValues = new List<string>();
        FieldEntry fieldEntry = this.TryGetFieldEntry(localizedFieldName);
        if (fieldEntry == null)
          return availableValues;
        InternalFieldType fieldType = this.GetFieldType(localizedFieldName);
        if (this.IsFieldComparisonOperator(localizedOperator))
        {
          foreach (string queriableFieldName in this.GetQueriableFieldNames())
          {
            if (!StringComparer.OrdinalIgnoreCase.Equals(localizedFieldName, queriableFieldName) && fieldType == this.GetFieldType(queriableFieldName))
              availableValues.Add(queriableFieldName);
          }
        }
        else
        {
          string invariantOperator = this.GetInvariantOperator(localizedOperator);
          int num = ((IEnumerable<string>) WiqlOperators.GroupOperators).Contains<string>(invariantOperator) ? 1 : 0;
          bool flag = this.IsWorkitemTypeField(fieldEntry);
          if (num != 0)
          {
            if (flag)
              availableValues.AddRange(this.GetWorkItemTypeCategories());
            else
              availableValues.AddRange(this.m_witService.GetGlobalAndProjectGroups(this.m_tfsRequestContext, projectId, true));
          }
          else
          {
            foreach (string allowedValue in this.m_witService.GetAllowedValues(this.m_tfsRequestContext, fieldEntry.FieldId))
              availableValues.Add(allowedValue);
            if (includePredefinedValues)
              availableValues.AddRange(this.GetPredefinedFieldValues(fieldEntry, fieldType));
          }
        }
        return availableValues;
      }
      finally
      {
        this.m_tfsRequestContext.TraceLeave(518090, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetAvailableValues));
      }
    }

    private IEnumerable<string> GetPredefinedFieldValues(
      FieldEntry fieldEntry,
      InternalFieldType fieldType)
    {
      List<string> predefinedFieldValues = new List<string>();
      switch (fieldType)
      {
        case (InternalFieldType) 0:
          predefinedFieldValues.Add(this.GetLocalizedOperator("@project"));
          return (IEnumerable<string>) predefinedFieldValues;
        case InternalFieldType.String:
          if (this.IsUsernameField(fieldEntry))
          {
            predefinedFieldValues.Add(this.GetLocalizedOperator("@me"));
            break;
          }
          if (this.FieldSupportsAnySyntax(fieldEntry))
          {
            predefinedFieldValues.Add(CommonClientResourceStrings.WiqlOperators_Any);
            break;
          }
          break;
        case InternalFieldType.DateTime:
          predefinedFieldValues.Add(this.GetLocalizedOperator("@today"));
          predefinedFieldValues.Add(this.Operators.GetLocalizedTodayMinusMacro(1));
          predefinedFieldValues.Add(this.Operators.GetLocalizedTodayMinusMacro(7));
          predefinedFieldValues.Add(this.Operators.GetLocalizedTodayMinusMacro(30));
          return (IEnumerable<string>) predefinedFieldValues;
        case InternalFieldType.Boolean:
          List<string> stringList1 = predefinedFieldValues;
          bool flag = true;
          string str1 = flag.ToString((IFormatProvider) this.m_cultureInfo);
          stringList1.Add(str1);
          List<string> stringList2 = predefinedFieldValues;
          flag = false;
          string str2 = flag.ToString((IFormatProvider) this.m_cultureInfo);
          stringList2.Add(str2);
          break;
      }
      return (IEnumerable<string>) predefinedFieldValues;
    }

    public virtual string GetLocalizedFieldValue(
      string invariantFieldName,
      string invariantOperator,
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node valueNode)
    {
      string empty = string.Empty;
      string localizedFieldValue;
      try
      {
        if (valueNode.NodeType == NodeType.ValueList)
        {
          NodeValueList nodeValueList = valueNode as NodeValueList;
          List<string> stringList = new List<string>();
          for (int i = 0; i < nodeValueList.Count; ++i)
          {
            string str = nodeValueList[i] is NodeArithmetic ? this.BuildArithmeticNodeString(invariantFieldName, nodeValueList[i]) : this.GetLocalizedFieldValueFromValueNode(invariantFieldName, nodeValueList[i] as NodeItem, true);
            stringList.Add(str);
          }
          localizedFieldValue = string.Join(",", stringList.ToArray());
        }
        else
          localizedFieldValue = valueNode.NodeType != NodeType.Arithmetic ? this.GetLocalizedFieldValueFromValueNode(invariantFieldName, valueNode as NodeItem, false) : this.BuildArithmeticNodeString(invariantFieldName, valueNode);
      }
      catch (NullReferenceException ex)
      {
        this.m_tfsRequestContext.TraceAlways(518106, TraceLevel.Error, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, string.Format("NullReferenceException in GetLocalizedFieldValue, field: {0}, operator: {1}, value: {2}", (object) invariantFieldName, (object) invariantOperator, (object) valueNode));
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) this.m_cultureInfo, Resources.UnableToProcessValueNode, (object) valueNode.ToString()), (Exception) ex);
      }
      this.m_tfsRequestContext.Trace(518105, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "GetLocalizedFieldValue: result: {0}", (object) localizedFieldValue);
      return localizedFieldValue;
    }

    private string BuildArithmeticNodeString(string invariantFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node valueNode)
    {
      NodeArithmetic nodeArithmetic = valueNode as NodeArithmetic;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2}", (object) this.GetLocalizedFieldValueFromValueNode(invariantFieldName, nodeArithmetic.Left as NodeItem, false), (object) ArithmeticalOperators.GetString(nodeArithmetic.Arithmetic), (object) this.GetLocalizedFieldValueFromValueNode(invariantFieldName, nodeArithmetic.Right as NodeItem, false));
    }

    private string GetLocalizedFieldValueFromValueNode(
      string invariantFieldname,
      NodeItem valueNode,
      bool valueInList)
    {
      string valueFromValueNode = valueNode.Value;
      if (valueNode.NodeType == NodeType.Variable)
        return this.GetLocalizedOperator(valueNode.ToString());
      if (valueNode.NodeType == NodeType.String && DateTime.TryParseExact(valueNode.Value, "o", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _))
        return this.UseIsoDateFormat ? valueFromValueNode : DateTime.ParseExact(valueFromValueNode, "o", (IFormatProvider) CultureInfo.InvariantCulture).ToString("d", (IFormatProvider) this.m_cultureInfo);
      if (valueNode.NodeType == NodeType.Number && valueFromValueNode.Contains(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator))
        return double.Parse(valueFromValueNode, (IFormatProvider) CultureInfo.InvariantCulture).ToString((IFormatProvider) this.m_cultureInfo);
      if (valueNode.NodeType == NodeType.String)
      {
        WorkItemLinkTypeEnd linkTypeEnd;
        if (StringComparer.OrdinalIgnoreCase.Equals(invariantFieldname, "System.Links.LinkType") && this.m_witService.GetLinkTypes(this.m_tfsRequestContext).LinkTypeEnds.TryGetByName(valueFromValueNode, out linkTypeEnd))
          valueFromValueNode = linkTypeEnd.Name;
        if (valueInList)
        {
          if (valueFromValueNode.Contains("\""))
            valueFromValueNode = valueFromValueNode.Replace("\"", "\"\"");
          if (valueFromValueNode.Contains(","))
            valueFromValueNode = string.Format((IFormatProvider) this.m_cultureInfo, "\"{0}\"", (object) valueFromValueNode);
        }
      }
      else if (valueNode.NodeType == NodeType.BoolConst)
        valueFromValueNode = bool.Parse(valueFromValueNode).ToString((IFormatProvider) this.m_cultureInfo);
      return valueFromValueNode;
    }

    public virtual string GetInvariantFieldValue(
      string invariantFieldName,
      string invariantOperator,
      string localizedValue)
    {
      try
      {
        this.m_tfsRequestContext.TraceEnter(518110, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetInvariantFieldValue));
        if (localizedValue == null)
          localizedValue = string.Empty;
        bool flag1 = localizedValue.Length == 0;
        if (StringComparer.OrdinalIgnoreCase.Equals(invariantOperator, "IN") || StringComparer.OrdinalIgnoreCase.Equals(invariantOperator, "NOT IN"))
        {
          string[] strArray = this.SplitListValue(localizedValue);
          for (int index = 0; index < strArray.Length; ++index)
            strArray[index] = this.GetInvariantFieldValue(invariantFieldName, string.Empty, strArray[index]);
          return "(" + string.Join(",", strArray) + ")";
        }
        if (StringComparer.OrdinalIgnoreCase.Equals(invariantFieldName, "System.Links.LinkType"))
        {
          WorkItemLinkTypeEnd linkTypeEnd;
          return this.m_witService.GetLinkTypes(this.m_tfsRequestContext).LinkTypeEnds.TryGetByName(localizedValue, out linkTypeEnd) ? WiqlHelper.QuoteStringValue(linkTypeEnd.ImmutableName) : WiqlHelper.QuoteStringValue(localizedValue);
        }
        InternalFieldType fieldType = this.GetFieldType(invariantFieldName);
        int num = localizedValue.StartsWith("@", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        bool flag2 = num == 0 && fieldType == InternalFieldType.DateTime;
        bool flag3 = num == 0 && fieldType == InternalFieldType.Double;
        bool flag4 = num == 0 && fieldType == InternalFieldType.Boolean;
        bool flag5 = num == 0 && (fieldType == (InternalFieldType) 0 || fieldType == InternalFieldType.String || fieldType == InternalFieldType.PlainText || fieldType == InternalFieldType.TreePath || fieldType == InternalFieldType.Html || fieldType == InternalFieldType.History);
        if (num != 0)
          localizedValue = this.GetInvariantMacro(localizedValue);
        else if (flag2 && !flag1)
        {
          DateTime dateTime = DateTime.Parse(localizedValue, (IFormatProvider) this.m_cultureInfo);
          if (dateTime < new DateTime(1800, 1, 1))
            dateTime = new DateTime(1800, 1, 1);
          if (dateTime > new DateTime(9999, 12, 31))
            dateTime = new DateTime(9999, 12, 31);
          localizedValue = dateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
        }
        else if (flag3 && !flag1)
          localizedValue = double.Parse(localizedValue, (IFormatProvider) this.m_cultureInfo).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        else if (flag4)
        {
          StringComparer cultureIgnoreCase1 = StringComparer.CurrentCultureIgnoreCase;
          string x1 = localizedValue;
          bool flag6 = true;
          string y1 = flag6.ToString((IFormatProvider) this.m_cultureInfo);
          bool flag7;
          if (cultureIgnoreCase1.Equals(x1, y1))
          {
            flag7 = true;
          }
          else
          {
            StringComparer cultureIgnoreCase2 = StringComparer.CurrentCultureIgnoreCase;
            string x2 = localizedValue;
            flag6 = false;
            string y2 = flag6.ToString((IFormatProvider) this.m_cultureInfo);
            if (!cultureIgnoreCase2.Equals(x2, y2))
              throw new FormatException(string.Format((IFormatProvider) this.m_cultureInfo, "InvalidBooleanValue", (object) localizedValue));
            flag7 = false;
          }
          localizedValue = flag7.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
        if (flag5 | flag2 | flag1)
          localizedValue = WiqlHelper.QuoteStringValue(localizedValue);
        return localizedValue;
      }
      finally
      {
        this.m_tfsRequestContext.TraceLeave(518115, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetInvariantFieldValue));
      }
    }

    public string GetInvariantMacro(string localizedMacro) => this.Operators.IsValidMacro(localizedMacro, true) ? this.Operators.GetInvariantOperator(localizedMacro) : throw new LegacyValidationException(string.Format((IFormatProvider) this.m_cultureInfo, Resources.QueryFilterInvalidMacro, (object) localizedMacro));

    internal string[] SplitListValue(string value)
    {
      string str = ",";
      char ch1 = '\n';
      char ch2 = '"';
      bool flag = false;
      StringBuilder stringBuilder = new StringBuilder(value);
      int num = 0;
      while (num < stringBuilder.Length)
      {
        if ((int) stringBuilder[num] == (int) ch2)
        {
          if (num + 1 < stringBuilder.Length && (int) stringBuilder[num + 1] == (int) ch2)
          {
            stringBuilder.Remove(num + 1, 1);
            ++num;
          }
          else
          {
            flag = !flag;
            stringBuilder.Remove(num, 1);
          }
        }
        else
        {
          if (!flag && stringBuilder.ToString(num, str.Length).Equals(str, StringComparison.Ordinal))
          {
            stringBuilder.Remove(num, str.Length);
            stringBuilder.Insert(num, ch1);
          }
          ++num;
        }
      }
      return stringBuilder.ToString().Split(new char[1]
      {
        ch1
      }, StringSplitOptions.RemoveEmptyEntries);
    }

    public virtual void OnLoadFromWiql(
      string fieldName,
      ref string localizedOperatorName,
      ref string value)
    {
      this.m_tfsRequestContext.Trace(518125, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "OnLoadFromWiql: fieldName: {0} op: {1} value: {2}", (object) fieldName, (object) localizedOperatorName, (object) value);
      if (this.FieldSupportsAnySyntax(fieldName))
        this.ConvertToSpecialAnySyntax(ref localizedOperatorName, ref value);
      string invariantOperator = this.GetInvariantOperator(localizedOperatorName);
      if (((IEnumerable<string>) WiqlOperators.ContainsOperators).Contains<string>(invariantOperator, (IEqualityComparer<string>) TFStringComparer.QueryOperator))
      {
        FieldEntry fieldEntry = this.TryGetFieldEntry(fieldName);
        if (fieldEntry == null || !fieldEntry.IsLongText || !fieldEntry.SupportsTextQuery)
          return;
        string containsWords = WiqlOperators.ConvertToContainsWords(invariantOperator);
        localizedOperatorName = this.GetLocalizedOperator(containsWords);
      }
      else
      {
        if (!((IEnumerable<string>) WiqlOperators.ContainsWordsOperators).Contains<string>(invariantOperator, (IEqualityComparer<string>) TFStringComparer.QueryOperator))
          return;
        FieldEntry fieldEntry = this.TryGetFieldEntry(fieldName);
        if (fieldEntry == null || fieldEntry.SupportsTextQuery)
          return;
        string contains = WiqlOperators.ConvertToContains(invariantOperator);
        localizedOperatorName = this.GetLocalizedOperator(contains);
      }
    }

    public virtual void OnSaveToWiql(string fieldName, ref string operatorName, ref string value)
    {
      this.m_tfsRequestContext.Trace(518130, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "OnSaveToWiql: fieldName: {0} op: {1} value: {2}", (object) fieldName, (object) operatorName, (object) value);
      if (this.FieldSupportsAnySyntax(fieldName))
        this.ConvertFromSpecialAnySyntax(ref operatorName, ref value);
      if (!((IEnumerable<string>) WiqlOperators.ContainsWordsOperators).Contains<string>(operatorName, (IEqualityComparer<string>) TFStringComparer.QueryOperator))
        return;
      FieldEntry fieldEntry = this.TryGetFieldEntry(fieldName);
      if (fieldEntry == null || !fieldEntry.IsLongText)
        return;
      operatorName = WiqlOperators.ConvertToContains(operatorName);
    }

    internal void ConvertToSpecialAnySyntax(ref string op, ref string value)
    {
      if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value.Trim()) || string.IsNullOrEmpty(op) || !StringComparer.OrdinalIgnoreCase.Equals(op.Trim(), this.GetLocalizedOperator("<>")))
        return;
      op = this.GetLocalizedOperator("=");
      value = CommonClientResourceStrings.WiqlOperators_Any;
    }

    internal void ConvertFromSpecialAnySyntax(ref string op, ref string value)
    {
      if (string.IsNullOrEmpty(value))
        return;
      if (!StringComparer.CurrentCultureIgnoreCase.Equals(value.Trim(' ', '\'', '"', '(', ')'), CommonClientResourceStrings.WiqlOperators_Any))
        return;
      if (string.IsNullOrEmpty(op) || !StringComparer.OrdinalIgnoreCase.Equals(op.Trim(), this.GetLocalizedOperator("=")))
        throw new LegacyValidationException(string.Format((IFormatProvider) this.m_cultureInfo, Resources.QueryEditorInvalidOperatorForSpecialValue, (object) CommonClientResourceStrings.WiqlOperators_Any, (object) this.GetLocalizedOperator("=")));
      op = this.GetLocalizedOperator("<>");
      value = string.Empty;
    }

    public string GetSeparatedLinkTypeNames(string filter, bool treeFilter)
    {
      this.m_tfsRequestContext.Trace(518135, TraceLevel.Verbose, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "GetSeparatedLinkTypeNames: filter: {0} treeFilter: {1} ", (object) filter, (object) treeFilter);
      IEnumerable<string> linkFilter = this.ParseLinkFilter(filter, treeFilter);
      if (linkFilter == null)
        return string.Empty;
      List<string> stringList = new List<string>();
      foreach (string referenceName in linkFilter)
      {
        WorkItemLinkTypeEnd linkTypeEnd;
        if (this.m_witService.GetLinkTypes(this.m_tfsRequestContext).LinkTypeEnds.TryGetByName(referenceName, out linkTypeEnd))
          stringList.Add(linkTypeEnd.Name);
      }
      return string.Join(",", stringList.ToArray());
    }

    private IEnumerable<string> ParseLinkFilter(string filter, bool treeFilter)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node whereNode;
      if (!WiqlHelper.ParseFilter(filter, string.Empty, out string _, out whereNode))
        return (IEnumerable<string>) null;
      return whereNode == null && whereNode.Count == 0 ? (IEnumerable<string>) null : this.ParseLinkNode(whereNode, treeFilter);
    }

    private IEnumerable<string> ParseLinkNode(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node, bool treeFilter)
    {
      switch (node.NodeType)
      {
        case NodeType.FieldCondition:
          if (node is NodeCondition nodeCondition && StringComparer.OrdinalIgnoreCase.Equals("System.Links.LinkType", nodeCondition.Left.Value.Trim()))
          {
            switch (nodeCondition.Condition)
            {
              case Condition.Equals:
                string linkTypeName1 = this.GetLinkTypeName(nodeCondition.Right as NodeItem, treeFilter);
                if (linkTypeName1 == null)
                  return this.EmptyLinkSet;
                return (IEnumerable<string>) new string[1]
                {
                  linkTypeName1
                };
              case Condition.NotEquals:
                string linkTypeName2 = this.GetLinkTypeName(nodeCondition.Right as NodeItem, treeFilter);
                if (linkTypeName2 == null)
                  return (IEnumerable<string>) null;
                return this.Not((IEnumerable<string>) new string[1]
                {
                  linkTypeName2
                }, treeFilter);
              case Condition.In:
                NodeValueList right = nodeCondition.Right as NodeValueList;
                List<string> linkNode = new List<string>();
                for (int i = 0; i < right.Count; ++i)
                {
                  string linkTypeName3 = this.GetLinkTypeName(right[i] as NodeItem, treeFilter);
                  if (linkTypeName3 != null)
                    linkNode.Add(linkTypeName3);
                }
                return (IEnumerable<string>) linkNode;
            }
          }
          else
            break;
          break;
        case NodeType.Not:
          return this.Not(this.ParseLinkNode(node[0], treeFilter), treeFilter);
        case NodeType.And:
          IEnumerable<string> setA1 = (IEnumerable<string>) null;
          for (int i = 0; i < node.Count; ++i)
            setA1 = this.Intersect(setA1, this.ParseLinkNode(node[i], treeFilter));
          return setA1;
        case NodeType.Or:
          IEnumerable<string> setA2 = this.EmptyLinkSet;
          for (int i = 0; i < node.Count; ++i)
            setA2 = this.Union(setA2, this.ParseLinkNode(node[i], treeFilter));
          return setA2;
      }
      throw new TeamFoundationServiceException(string.Format((IFormatProvider) this.m_cultureInfo, Resources.UnexpectedNode, (object) node.ToString()));
    }

    private string GetLinkTypeName(NodeItem nodeItem, bool treeFilter)
    {
      string referenceName = nodeItem.Value.Trim(' ', '(', ')');
      if (string.IsNullOrEmpty(referenceName))
        return (string) null;
      WorkItemLinkTypeEnd linkTypeEnd;
      if (!this.m_witService.GetLinkTypes(this.m_tfsRequestContext).LinkTypeEnds.TryGetByName(referenceName, out linkTypeEnd))
        return (string) null;
      return (treeFilter ? this.AllRecursiveLinkSet : this.AllOneHopLinkSet).Contains<string>(linkTypeEnd.ImmutableName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? linkTypeEnd.ImmutableName : (string) null;
    }

    private IEnumerable<string> Not(IEnumerable<string> set, bool treeFilter)
    {
      IEnumerable<string> first = treeFilter ? this.AllRecursiveLinkSet : this.AllOneHopLinkSet;
      return set == null ? this.EmptyLinkSet : first.Except<string>(set, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private IEnumerable<string> Union(IEnumerable<string> setA, IEnumerable<string> setB)
    {
      if (setA == null)
        return (IEnumerable<string>) null;
      return setB == null ? (IEnumerable<string>) null : setA.Union<string>(setB, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private IEnumerable<string> Intersect(IEnumerable<string> setA, IEnumerable<string> setB)
    {
      if (setA == null)
        return setB;
      return setB == null ? setA : setA.Intersect<string>(setB, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    internal string BuildLinkFilter(string linkTypes)
    {
      List<string> stringList = new List<string>();
      string str1 = linkTypes;
      string[] separator = new string[1]{ "," };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        string str3 = str2.Trim();
        if (StringComparer.OrdinalIgnoreCase.Equals(str3, CommonClientResourceStrings.WiqlOperators_Any))
          return QueryAdapter.DefaultLinkFilter;
        WorkItemLinkTypeEnd linkTypeEnd;
        if (this.m_witService.GetLinkTypes(this.m_tfsRequestContext).LinkTypeEnds.TryGetByName(str3, out linkTypeEnd))
          stringList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] = {1}", (object) "System.Links.LinkType", (object) WiqlHelper.QuoteStringValue(linkTypeEnd.ImmutableName)));
      }
      return string.Join(" OR ", stringList.ToArray());
    }
  }
}
