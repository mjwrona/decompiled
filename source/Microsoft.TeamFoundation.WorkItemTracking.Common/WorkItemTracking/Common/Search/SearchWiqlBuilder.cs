// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Search.SearchWiqlBuilder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Search
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SearchWiqlBuilder
  {
    private const string cInvariantDateFormat = "o";
    private ISearchCriteriaRecorder m_criteriaHelper;
    private ISearchTokenHelper m_tokenHelper;
    private IWiqlAdapterHelper m_wiqlHelper;
    private string m_projectName;

    [CLSCompliant(false)]
    public SearchWiqlBuilder(
      IWiqlAdapterHelper wiqlHelper,
      ISearchTokenHelper tokenHelper,
      string projectName,
      ISearchCriteriaRecorder criteriaHelper = null)
    {
      this.m_wiqlHelper = wiqlHelper;
      this.m_tokenHelper = tokenHelper;
      this.m_criteriaHelper = criteriaHelper;
      this.m_projectName = projectName;
    }

    public string BuildWiql(
      string orderWiql,
      string previewFieldsWiql = null,
      bool excludeCodeReviewResponse = false,
      bool includeTags = false,
      bool skipReproStepsFieldForSearch = false)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.BuildSelect(previewFieldsWiql, includeTags));
      stringBuilder.Append(this.BuildWhere(excludeCodeReviewResponse, skipReproStepsFieldForSearch));
      stringBuilder.Append(this.BuildOrder(orderWiql));
      return stringBuilder.ToString();
    }

    private string BuildSelect(string previewFieldsWiql, bool includeTags)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SELECT System.Id, ");
      if (!string.IsNullOrEmpty(previewFieldsWiql))
        stringBuilder.Append(previewFieldsWiql);
      else
        stringBuilder.Append(string.Join(",", new string[4]
        {
          "System.WorkItemType",
          "System.Title",
          "System.State",
          "System.AssignedTo"
        }));
      if (includeTags)
        stringBuilder.Append(",System.Tags");
      stringBuilder.Append(" FROM workitems");
      return stringBuilder.ToString();
    }

    private string BuildWhere(bool excludeCodeReviewResponse = false, bool skipReproStepsFieldForSearch = false)
    {
      StringBuilder wiql = new StringBuilder();
      wiql.Append(" WHERE System.TeamProject = @project");
      foreach (object token in this.m_tokenHelper.GetTokens())
      {
        wiql.Append(" AND ");
        if (this.m_tokenHelper.IsFilterToken(token))
          this.AppendFilterCondition(token, wiql);
        else
          this.AppendBasicCondition(token, wiql, skipReproStepsFieldForSearch);
      }
      if (excludeCodeReviewResponse)
      {
        wiql.Append(" ");
        wiql.Append(string.Join(" ", new string[4]
        {
          "AND",
          WiqlHelpers.GetEnclosedField("System.WorkItemType"),
          "NOT IN GROUP",
          WiqlHelpers.GetSingleQuotedValue("Microsoft.CodeReviewResponseCategory")
        }));
        wiql.Append(" ");
      }
      return wiql.ToString();
    }

    private string BuildOrder(string orderWiql)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(" ");
      stringBuilder.Append(orderWiql);
      return stringBuilder.ToString();
    }

    private void AppendFilterCondition(object filterToken, StringBuilder wiql)
    {
      object field = this.m_wiqlHelper.FindField(this.GetFieldReferenceName(this.m_tokenHelper.GetFilterField(filterToken)), (string) null, (object) null);
      wiql.Append(this.m_wiqlHelper.GetFieldReferenceName(field));
      wiql.Append(" ");
      string operation = this.GetOperator(field, (SearchFilterTokenType) this.m_tokenHelper.GetFilterTokenType(filterToken));
      wiql.Append(operation);
      wiql.Append(" ");
      wiql.Append(this.GetInvariantFieldValue(field, this.m_tokenHelper.GetFilterValue(filterToken)));
      if (this.m_criteriaHelper == null)
        return;
      string str = this.m_tokenHelper.GetFilterValue(filterToken);
      string invariantMacro = this.GetInvariantMacro(this.m_tokenHelper.GetFilterValue(filterToken));
      if (!string.IsNullOrEmpty(invariantMacro))
        str = this.EvaluateInvariantMacro(field, invariantMacro);
      this.m_criteriaHelper.RecordCriterion(this.m_wiqlHelper.GetFieldFriendlyName(field), operation, str);
    }

    private string GetOperator(object field, SearchFilterTokenType filterOp)
    {
      bool flag1 = filterOp.HasFlag((Enum) SearchFilterTokenType.SFTT_EXACTMATCH);
      bool flag2 = filterOp.HasFlag((Enum) SearchFilterTokenType.SFTT_EXCLUDE);
      switch (this.m_wiqlHelper.GetFieldType(field))
      {
        case (InternalFieldType) 0:
        case InternalFieldType.Integer:
        case InternalFieldType.DateTime:
        case InternalFieldType.Double:
        case InternalFieldType.Guid:
        case InternalFieldType.Boolean:
          return flag2 ? "<>" : "=";
        case InternalFieldType.String:
          return flag1 ? (flag2 ? "<>" : "=") : (flag2 ? "NOT CONTAINS" : "CONTAINS");
        case InternalFieldType.PlainText:
        case InternalFieldType.Html:
        case InternalFieldType.History:
          return flag2 ? "NOT CONTAINS" : "CONTAINS";
        case InternalFieldType.TreePath:
          return flag1 ? (flag2 ? "<>" : "=") : (flag2 ? "NOT UNDER" : "UNDER");
        default:
          return flag2 ? "<>" : "=";
      }
    }

    private string GetInvariantMacro(string localizedValue)
    {
      if (TFStringComparer.QueryOperator.StartsWith(localizedValue, WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroToday)))
        return WiqlOperators.GetInvariantTodayMacro(localizedValue);
      if (TFStringComparer.QueryOperator.Equals(localizedValue, WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroMe)))
        return "@me";
      if (TFStringComparer.QueryOperator.Equals(localizedValue, WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroProject)))
        return "@project";
      return TFStringComparer.QueryOperator.Equals(localizedValue, WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroCurrentIteration)) ? "@currentIteration" : (string) null;
    }

    private string GetInvariantFieldValue(object field, string localValue)
    {
      string invariantFieldValue = localValue;
      try
      {
        Type fieldSystemType = this.m_wiqlHelper.GetFieldSystemType(field);
        string invariantMacro = this.GetInvariantMacro(localValue);
        bool flag1 = localValue.Length == 0;
        if (invariantMacro != null)
          invariantFieldValue = invariantMacro;
        else if (fieldSystemType == typeof (DateTime) && !flag1)
          invariantFieldValue = WiqlHelpers.GetEscapedSingleQuotedValue(DateTime.Parse(localValue, (IFormatProvider) CultureInfo.CurrentCulture).ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
        else if (!flag1 && fieldSystemType == typeof (Decimal) || fieldSystemType == typeof (double) || fieldSystemType == typeof (bool))
        {
          invariantFieldValue = (string) Convert.ChangeType(Convert.ChangeType((object) localValue, fieldSystemType, (IFormatProvider) CultureInfo.CurrentCulture), typeof (string), (IFormatProvider) CultureInfo.InvariantCulture);
        }
        else
        {
          bool flag2 = fieldSystemType == typeof (string);
          if (fieldSystemType == typeof (Guid) | flag2 | flag1)
            invariantFieldValue = WiqlHelpers.GetEscapedSingleQuotedValue(localValue);
        }
      }
      catch (Exception ex)
      {
        throw new TeamFoundationServerException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, InternalsResourceStrings.Get("SearchPageFormatException"), (object) localValue, (object) this.m_wiqlHelper.GetFieldFriendlyName(field), (object) this.m_wiqlHelper.GetFieldType(field)));
      }
      return invariantFieldValue;
    }

    private string GetFieldReferenceName(string name)
    {
      if (name != null && name.Length == 1)
      {
        switch (name[0])
        {
          case 'A':
          case 'a':
            return "System.AssignedTo";
          case 'C':
          case 'c':
            return "System.CreatedBy";
          case 'S':
          case 's':
            return "System.State";
          case 'T':
          case 't':
            return "System.WorkItemType";
        }
      }
      return this.m_wiqlHelper.GetFieldReferenceName(this.m_wiqlHelper.FindField(name, (string) null, (object) null) ?? throw new TeamFoundationServerException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, InternalsResourceStrings.Get("ErrorSearchInvalidFieldName"), (object) name)));
    }

    private void AppendBasicCondition(
      object token,
      StringBuilder wiql,
      bool skipReproStepsFieldForSearch = false)
    {
      object field1 = this.m_wiqlHelper.FindField("System.Title", (string) null, (object) null);
      object field2 = this.m_wiqlHelper.FindField("System.Description", (string) null, (object) null);
      object field3 = skipReproStepsFieldForSearch ? (object) null : this.m_wiqlHelper.FindField("Microsoft.VSTS.TCM.ReproSteps", (string) null, (object) null);
      bool exclude = false;
      string str1 = this.m_tokenHelper.GetParsedTokenText(token);
      if (str1.Length > 1 && str1[0] == '-' && !this.IsQuoted(this.m_tokenHelper.GetOriginalTokenText(token)))
      {
        exclude = true;
        str1 = str1.Substring(1);
      }
      string str2 = exclude ? "AND" : "OR";
      string singleQuotedValue = WiqlHelpers.GetEscapedSingleQuotedValue(str1);
      bool useTextSearchIfAvailable = true;
      bool flag = useTextSearchIfAvailable && this.m_wiqlHelper.GetFieldSupportsTextQuery(field1);
      wiql.Append("(");
      wiql.Append(this.GetContainsClause(field1, singleQuotedValue, exclude, useTextSearchIfAvailable));
      wiql.Append(" " + str2 + " ");
      wiql.Append(this.GetContainsClause(field2, singleQuotedValue, exclude, useTextSearchIfAvailable));
      if (field3 != null)
      {
        wiql.Append(" " + str2 + " ");
        wiql.Append(this.GetContainsClause(field3, singleQuotedValue, exclude, useTextSearchIfAvailable));
      }
      wiql.Append(")");
      if (this.m_criteriaHelper == null)
        return;
      this.m_criteriaHelper.RecordCriterion(this.GetKeywordSearchHelpText(exclude, field1, field2, field3), exclude ? (flag ? "KeywordNotContainsWords" : "KeywordNotContains") : (flag ? "KeywordContainsWords" : "KeywordContains"), str1);
    }

    private bool IsQuoted(string text)
    {
      text = text.Trim();
      return text.Length > 1 && text[0] == '"' && text[text.Length - 1] == '"';
    }

    private string GetKeywordSearchHelpText(
      bool exclude,
      object titleField,
      object descriptionField,
      object reproStepsField)
    {
      return reproStepsField == null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, exclude ? InternalsResourceStrings.Get("SearchPageKeywordNotHelpTextWithoutRepro") : InternalsResourceStrings.Get("SearchPageKeywordHelpTextWithoutRepro"), (object) this.m_wiqlHelper.GetFieldFriendlyName(titleField), (object) this.m_wiqlHelper.GetFieldFriendlyName(descriptionField)) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, exclude ? InternalsResourceStrings.Get("SearchPageKeywordNotHelpTextWithRepro") : InternalsResourceStrings.Get("SearchPageKeywordHelpTextWithRepro"), (object) this.m_wiqlHelper.GetFieldFriendlyName(titleField), (object) this.m_wiqlHelper.GetFieldFriendlyName(descriptionField), (object) this.m_wiqlHelper.GetFieldFriendlyName(reproStepsField));
    }

    private string GetContainsClause(
      object field,
      string value,
      bool exclude,
      bool useTextSearchIfAvailable)
    {
      bool flag = useTextSearchIfAvailable && !this.m_wiqlHelper.GetFieldIsLongText(field) && this.m_wiqlHelper.GetFieldSupportsTextQuery(field);
      string str = exclude ? (flag ? "NOT CONTAINS WORDS" : "NOT CONTAINS") : (flag ? "CONTAINS WORDS" : "CONTAINS");
      return this.m_wiqlHelper.GetFieldReferenceName(field) + " " + str + " " + value;
    }

    private string EvaluateInvariantMacro(object field, string invariantMacro)
    {
      if (this.m_wiqlHelper.GetFieldType(field) == InternalFieldType.DateTime && TFStringComparer.QueryOperator.StartsWith(invariantMacro.Trim(), "@today"))
      {
        if (TFStringComparer.QueryOperator.Equals(invariantMacro.Trim(), "@today"))
          return DateTime.Today.ToShortDateString();
        string[] strArray = invariantMacro.Split('-');
        int result;
        return strArray.Length != 2 || !int.TryParse(strArray[1].Trim(), out result) ? invariantMacro : DateTime.Today.AddDays((double) -result).ToShortDateString();
      }
      if (TFStringComparer.QueryOperator.Equals(invariantMacro, "@me"))
        return this.m_wiqlHelper.UserDisplayName;
      return TFStringComparer.QueryOperator.Equals(invariantMacro, "@project") ? this.m_projectName : invariantMacro;
    }
  }
}
