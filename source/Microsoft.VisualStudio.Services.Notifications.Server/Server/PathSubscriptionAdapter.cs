// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.PathSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class PathSubscriptionAdapter : BaseSubscriptionAdapter
  {
    private Dictionary<string, string> m_basicAlertNames = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ExpressionFilterField> m_fieldsByInvariantName = new Dictionary<string, ExpressionFilterField>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ExpressionFilterField> m_fieldsByLocalizedName = new Dictionary<string, ExpressionFilterField>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, NotificationEventField> m_contributedFiledsByInvariantName = new Dictionary<string, NotificationEventField>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private bool m_contributedFieldsInitialized;
    protected const string c_Null = "null";
    private static Regex s_subscriptionNameRegEx = new Regex("(?<prename>.*<PT [^>]*N=\\\")(?<name>[^\\\"]+)(?<postname>\\\".*)", RegexOptions.Compiled);
    private static readonly string s_subscriptionNameFormat = "<PT N=\"{0}\" />";
    protected PathSubscriptionExpression m_pathSubscriptionExpression;

    public override string FilterType => "Expression";

    public void InitializeAdapter(IVssRequestContext requestContext) => this.LoadContributedFields(requestContext);

    protected void AddBasicAlertName(string eventTypeString, string alertName) => this.m_basicAlertNames[eventTypeString] = alertName;

    public string GetBasicAlertNameFromTag(string subscriptionTag)
    {
      string[] strArray = subscriptionTag.Split(new char[1]
      {
        ':'
      }, 2);
      string str1;
      if (strArray.Length == 2 && this.m_basicAlertNames.TryGetValue(strArray[1], out str1))
        return strArray[0] + ": " + str1;
      string str2;
      return !string.IsNullOrEmpty(subscriptionTag) && this.m_basicAlertNames.TryGetValue(subscriptionTag, out str2) ? str2 : (string) null;
    }

    protected IDictionary<string, string> GetUserUniqueNameMacroLookup() => (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
      {
        "@@MyUniqueName@@",
        CoreRes.MeMacroDisplayString()
      }
    };

    public virtual void EnsureFieldsLoaded(IVssRequestContext requestContext)
    {
      if (this.FieldInitialized)
        return;
      this.LoadFields(requestContext);
    }

    protected virtual void LoadFields(IVssRequestContext requestContext) => this.FieldInitialized = true;

    protected IDictionary<string, string> GetUserDisplayNameMacroLookup() => (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
      {
        "@@MyDisplayName@@",
        CoreRes.MeMacroDisplayString()
      }
    };

    protected static string MeMacroDisplayString => CoreRes.MeMacroDisplayString();

    public virtual NotificationEventField LoadPossibleFieldValues(
      IVssRequestContext requestContext,
      ExpressionFilterField field,
      SubscriptionScope scope)
    {
      NotificationEventField notificationEventField = new NotificationEventField();
      notificationEventField.Name = field.InvariantFieldName;
      notificationEventField.Id = field.InvariantFieldName;
      notificationEventField.Path = field.InvariantFieldName;
      NotificationEventFieldType notificationEventFieldType = new NotificationEventFieldType();
      notificationEventFieldType.Id = "String";
      notificationEventFieldType.Operators = SubscriptionFilterOperators.GetLocalizedFieldOperators(field.RawOperators);
      ValueDefinition valueDefinition = new ValueDefinition();
      valueDefinition.DataSource = new List<InputValue>();
      foreach (string str in field.GetValues(requestContext))
        valueDefinition.DataSource.Add(new InputValue()
        {
          Value = str,
          DisplayValue = str
        });
      notificationEventFieldType.Value = valueDefinition;
      notificationEventField.FieldType = notificationEventFieldType;
      return notificationEventField;
    }

    protected IDictionary<string, string> GetProjectNameMacroLookup() => (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
      {
        "@@MyProjectName@@",
        CoreRes.ProjectMacroDisplayString()
      }
    };

    protected static string ProjectMacroDisplayString => CoreRes.ProjectMacroDisplayString();

    protected bool FieldInitialized { get; set; }

    protected ExpressionFilterField GetFieldByInvariantName(Token fieldNameToken) => this.GetFieldByInvariantName(fieldNameToken.QueriableValue);

    protected ExpressionFilterField GetFieldByInvariantName(string invariantFieldName)
    {
      ExpressionFilterField expressionFilterField;
      return !string.IsNullOrEmpty(invariantFieldName) && this.m_fieldsByInvariantName.TryGetValue(invariantFieldName, out expressionFilterField) ? expressionFilterField : (ExpressionFilterField) null;
    }

    protected ExpressionFilterField GetFieldByLocalizedName(string localizedFieldName)
    {
      ExpressionFilterField expressionFilterField;
      return !string.IsNullOrEmpty(localizedFieldName) && this.m_fieldsByLocalizedName.TryGetValue(localizedFieldName, out expressionFilterField) ? expressionFilterField : (ExpressionFilterField) null;
    }

    protected ExpressionFilterField AddField(
      string invariantFieldName,
      string localizedValue,
      IEnumerable<byte> operators = null,
      Func<IVssRequestContext, IEnumerable<string>> getValuesMethod = null)
    {
      return this.AddField(SubscriptionFieldType.String, invariantFieldName, localizedValue, operators, getValuesMethod);
    }

    protected ExpressionFilterField AddField(
      SubscriptionFieldType fieldType,
      string invariantFieldName,
      string localizedValue,
      IEnumerable<byte> operators = null,
      Func<IVssRequestContext, IEnumerable<string>> getValuesMethod = null)
    {
      ExpressionFilterField field = new ExpressionFilterField(fieldType, invariantFieldName, localizedValue, operators, getValuesMethod);
      this.AddField(field);
      return field;
    }

    protected ExpressionFilterField AddFieldWithLookup(
      string invariantFieldName,
      string localizedValue,
      IEnumerable<byte> operators,
      Func<IVssRequestContext, IDictionary<string, string>> getValuesLookupMethod)
    {
      ExpressionFilterField field = new ExpressionFilterField(SubscriptionFieldType.String, invariantFieldName, localizedValue, operators, getValuesLookupMethod);
      this.AddField(field);
      return field;
    }

    protected void AddField(ExpressionFilterField field)
    {
      this.m_fieldsByInvariantName[field.InvariantFieldName] = field;
      this.m_fieldsByLocalizedName[this.GetLocalizedFieldName(field)] = field;
    }

    public abstract ExpressionFilterModel ParseCondition(
      IVssRequestContext requestcontext,
      string conditionString);

    public ExpressionFilterModel ParseCondition(
      IVssRequestContext requestcontext,
      string conditionString,
      string matcher)
    {
      int num = matcher == "XPathMatcher" ? 1 : 0;
      ExpressionFilterModel filter = new ExpressionFilterModel();
      string input = conditionString;
      TeamFoundationEventConditionParser parser = TeamFoundationEventConditionParser.GetParser((EventSerializerType) num, input);
      this.ParseCondition(requestcontext, filter, parser.Parse(), string.Empty);
      ExpressionFilterGroup.AssignLevels((IEnumerable<ExpressionFilterGroup>) filter.Groups);
      filter.MaxGroupLevel = ExpressionFilterGroup.AssignLevels((IEnumerable<ExpressionFilterGroup>) filter.Groups);
      return filter;
    }

    private void ParseCondition(
      IVssRequestContext requestContext,
      ExpressionFilterModel filter,
      Condition condition,
      string logicalOperator)
    {
      this.EnsureFieldsLoaded(requestContext);
      bool flag = false;
      int firstRow = 1 + filter.Clauses.Count;
      switch (condition)
      {
        case AndCondition _:
          AndCondition andCondition = (AndCondition) condition;
          if (andCondition.condition1 is FieldCondition && andCondition.condition2 is FieldCondition)
          {
            FieldCondition condition1 = (FieldCondition) andCondition.condition1;
            FieldCondition condition2 = (FieldCondition) andCondition.condition2;
            if (string.Equals(this.GetValueFromCondition(condition1), this.GetValueFromCondition(condition2), StringComparison.OrdinalIgnoreCase))
            {
              PathSubscriptionExpression expressionParser = this.GetExpressionParser();
              PathSubscriptionExpression path1 = expressionParser.ParsePath(condition1.FieldName.Spelling);
              PathSubscriptionExpression path2 = expressionParser.ParsePath(condition2.FieldName.Spelling);
              if (this.AreOldAndNewChangeFields(path1, path2))
              {
                ExpressionFilterField fieldByPath = this.GetFieldByPath(path1);
                if (fieldByPath != null)
                {
                  byte rawOperator = this.IsChangeFieldOldValue(path1) == (condition1.Operation == (byte) 12) ? (byte) 161 : (byte) 162;
                  string localizedValue = fieldByPath.GetLocalizedValue(requestContext, this.GetValueFromCondition(condition1));
                  string clauseValueForView = this.GetFilterFieldClauseValueForView(fieldByPath, localizedValue);
                  this.AddNewClause(filter, logicalOperator, this.GetLocalizedFieldName(fieldByPath), rawOperator, clauseValueForView);
                  return;
                }
              }
            }
          }
          this.ParseCondition(requestContext, filter, ((AndCondition) condition).condition1, logicalOperator);
          this.ParseCondition(requestContext, filter, ((AndCondition) condition).condition2, NotificationsWebApiResources.LogicalOperatorAnd());
          flag = ((AndCondition) condition).hasParens;
          break;
        case OrCondition _:
          this.ParseCondition(requestContext, filter, ((OrCondition) condition).Condition1, logicalOperator);
          this.ParseCondition(requestContext, filter, ((OrCondition) condition).Condition2, NotificationsWebApiResources.LogicalOperatorOr());
          flag = ((OrCondition) condition).m_hasParens;
          break;
        case FieldCondition _:
          this.AddNewClause(requestContext, filter, (FieldCondition) condition, logicalOperator);
          break;
      }
      if (!flag || firstRow >= filter.Clauses.Count)
        return;
      filter.Groups.Add(new ExpressionFilterGroup(firstRow, filter.Clauses.Count));
    }

    protected virtual string GetFilterFieldClauseValueForView(
      ExpressionFilterField filterField,
      string clauseValue)
    {
      return clauseValue;
    }

    protected virtual PathSubscriptionExpression ParsePathFromFieldName(string fieldName) => this.GetExpressionParser().ParsePath(fieldName);

    protected virtual void UpdateValuesFromPath(
      ExpressionFilterField field,
      PathSubscriptionExpression xpath,
      ref byte rawOperator,
      ref string value,
      IVssRequestContext requestcontext)
    {
      if (string.Equals(value, "null") && xpath.FilterType != FieldFilterType.None && xpath.FilterType != FieldFilterType.NoOperator)
      {
        value = xpath.FilterValue.Spelling;
        if (xpath.FilterType == FieldFilterType.Equals || xpath.FilterType == FieldFilterType.EndsWith)
          rawOperator = rawOperator == (byte) 12 ? (byte) 13 : (byte) 12;
        else if (xpath.FilterType == FieldFilterType.NotEquals)
          rawOperator = (byte) 13;
        else if (xpath.FilterType == FieldFilterType.StartsWith)
          rawOperator = rawOperator == (byte) 12 ? (byte) 163 : (byte) 14;
        else if (xpath.FilterType == FieldFilterType.NotStartsWith)
          rawOperator = (byte) 163;
        else if (xpath.FilterType == FieldFilterType.Contains)
          rawOperator = rawOperator == (byte) 12 ? (byte) 25 : (byte) 15;
        else if (xpath.FilterType == FieldFilterType.NotContains)
          rawOperator = (byte) 25;
      }
      value = field.GetLocalizedValue(requestcontext, value);
    }

    private ExpressionFilterClause AddNewClause(
      IVssRequestContext requestContext,
      ExpressionFilterModel filter,
      FieldCondition condition,
      string logicalOperator)
    {
      string str1 = condition.FieldName.Spelling;
      byte rawOperator = condition.Operation;
      string str2 = this.GetValueFromCondition(condition);
      PathSubscriptionExpression subscriptionExpression = this.ParsePathFromFieldName(str1);
      if (subscriptionExpression.FilterType == FieldFilterType.NotContains)
      {
        rawOperator = (byte) 25;
        str2 = subscriptionExpression.FilterValue.Spelling;
        string path = subscriptionExpression.Path;
        if (subscriptionExpression.FilterName.Spelling != null && subscriptionExpression.FilterName.Spelling.StartsWith("@"))
          path += string.Format("[{0}]", (object) subscriptionExpression.FilterName.Spelling);
        if (!string.IsNullOrEmpty(subscriptionExpression.PostFilterPath))
          path += subscriptionExpression.PostFilterPath;
        subscriptionExpression = this.GetExpressionParser().ParsePath(path);
      }
      ExpressionFilterField fieldByPath = this.GetFieldByPath(subscriptionExpression);
      if (fieldByPath != null)
      {
        PathSubscriptionExpression path = this.GetExpressionParser().ParsePath(str2);
        str1 = this.GetLocalizedFieldName(fieldByPath);
        if (this.AreOldAndNewChangeFields(subscriptionExpression, path))
        {
          rawOperator = (byte) 160;
          str2 = string.Empty;
        }
        else if (this.IsChangeFieldOldValue(subscriptionExpression))
        {
          rawOperator = (byte) 161;
          str2 = fieldByPath.GetLocalizedValue(requestContext, str2);
        }
        else if (this.IsChangeFieldNewValue(subscriptionExpression) && rawOperator == (byte) 12)
        {
          rawOperator = (byte) 162;
          str2 = fieldByPath.GetLocalizedValue(requestContext, str2);
        }
        else
          this.UpdateValuesFromPath(fieldByPath, subscriptionExpression, ref rawOperator, ref str2, requestContext);
        str2 = this.GetFilterFieldClauseValueForView(fieldByPath, str2);
      }
      return this.AddNewClause(filter, logicalOperator, str1, rawOperator, str2);
    }

    private ExpressionFilterClause AddNewClause(
      ExpressionFilterModel filter,
      string logicalOperator,
      string localizedFieldName,
      byte rawOperator,
      string value)
    {
      ExpressionFilterClause expressionFilterClause = new ExpressionFilterClause();
      expressionFilterClause.LogicalOperator = logicalOperator;
      expressionFilterClause.FieldName = localizedFieldName;
      expressionFilterClause.Operator = SubscriptionFilterOperators.GetLocalizedOperator(rawOperator);
      expressionFilterClause.Value = value;
      expressionFilterClause.Index = filter.Clauses.Count + 1;
      filter.Clauses.Add(expressionFilterClause);
      return expressionFilterClause;
    }

    private string GetValueFromCondition(FieldCondition condition)
    {
      string str = PathSubscriptionAdapter.UnQuoteString(condition.GetOperandString());
      if (condition.Operation == (byte) 15)
      {
        if (str.Length >= 4 && str.StartsWith(".*") && str.EndsWith(".*"))
          str = str.Substring(2, str.Length - 4);
        str = Regex.Unescape(str);
      }
      return str;
    }

    protected static string UnQuoteString(string quotedString)
    {
      if (quotedString != null && quotedString.Length > 1 && (quotedString[0] == '\'' && quotedString[quotedString.Length - 1] == '\'' || quotedString[0] == '"' && quotedString[quotedString.Length - 1] == '"'))
        quotedString = quotedString.Substring(1, quotedString.Length - 2);
      return quotedString;
    }

    public string GetFieldNameFromPathExpression(
      IVssRequestContext requestContext,
      PathSubscriptionExpression path)
    {
      this.EnsureFieldsLoaded(requestContext);
      return this.GetFieldByPath(path)?.InvariantFieldName;
    }

    protected virtual ExpressionFilterField GetFieldByPath(PathSubscriptionExpression xpath)
    {
      if (xpath.FilterType == FieldFilterType.None)
        return this.GetFieldByInvariantName(xpath.Path);
      if ("text()".Equals(xpath.FilterName.Spelling, StringComparison.OrdinalIgnoreCase))
        return this.GetFieldFromPathTextFilter(xpath);
      return this.GetFieldByInvariantName(xpath.FilterName.Spelling.TrimStart('@'));
    }

    protected virtual ExpressionFilterField GetFieldFromPathTextFilter(
      PathSubscriptionExpression xpath)
    {
      string[] strArray = xpath.Path.Split('/');
      return this.GetFieldByInvariantName(strArray[strArray.Length - 1]);
    }

    protected virtual bool AreOldAndNewChangeFields(
      PathSubscriptionExpression xpath1,
      PathSubscriptionExpression xpath2)
    {
      if (this.IsChangeFieldOldValue(xpath1) && this.IsChangeFieldNewValue(xpath2))
        return true;
      return this.IsChangeFieldNewValue(xpath1) && this.IsChangeFieldOldValue(xpath2);
    }

    protected abstract bool IsChangeFieldNewValue(PathSubscriptionExpression path);

    protected abstract bool IsChangeFieldOldValue(PathSubscriptionExpression path);

    public virtual string ToConditionString(
      IVssRequestContext requestcontext,
      ExpressionFilterModel filter)
    {
      this.EnsureFieldsLoaded(requestcontext);
      int num = 0;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ExpressionFilterClause clause1 in (IEnumerable<ExpressionFilterClause>) filter.Clauses)
      {
        ExpressionFilterClause clause = clause1;
        if (clause != null)
          clause.Validate(true);
        clause.Index = ++num;
        if (clause.Index > 1)
        {
          bool flag = string.Equals(clause.LogicalOperator, NotificationsWebApiResources.LogicalOperatorAnd(), StringComparison.CurrentCultureIgnoreCase) || "AND".Equals(clause.LogicalOperator, StringComparison.CurrentCultureIgnoreCase);
          stringBuilder.Append(flag ? " AND " : " OR ");
        }
        stringBuilder.Append('(', filter.Groups.Count<ExpressionFilterGroup>((Func<ExpressionFilterGroup, bool>) (g => g != null && g.Start == clause.Index)));
        stringBuilder.Append(this.GetFilterString(clause, requestcontext));
        stringBuilder.Append(')', filter.Groups.Count<ExpressionFilterGroup>((Func<ExpressionFilterGroup, bool>) (g => g != null && g.End == clause.Index)));
      }
      return stringBuilder.ToString();
    }

    protected virtual string GetFilterString(
      ExpressionFilterClause clause,
      IVssRequestContext requestContext)
    {
      ExpressionFilterField fieldByLocalizedName = this.GetFieldByLocalizedName(clause.FieldName);
      if (fieldByLocalizedName == null)
        return this.GetFilterString(clause.FieldName, this.GetRawOperator(clause), clause.Value, requestContext);
      clause.Value = fieldByLocalizedName.GetInvariantValue(requestContext, clause.Value);
      switch (this.GetRawOperator(clause))
      {
        case 160:
          return this.GetFilterString(this.GetChangeFieldExpression(fieldByLocalizedName, false).ToPath(), (byte) 13, this.GetChangeFieldExpression(fieldByLocalizedName, true).ToPath());
        case 161:
          string forConditionString1 = this.GetFilterFieldClauseValueForConditionString(fieldByLocalizedName, clause);
          return string.Format("({0} AND {1})", (object) this.GetFilterString(this.GetChangeFieldExpression(fieldByLocalizedName, true).ToPath(), (byte) 12, forConditionString1, requestContext), (object) this.GetFilterString(this.GetChangeFieldExpression(fieldByLocalizedName, false).ToPath(), (byte) 13, forConditionString1, requestContext));
        case 162:
          string forConditionString2 = this.GetFilterFieldClauseValueForConditionString(fieldByLocalizedName, clause);
          return string.Format("({0} AND {1})", (object) this.GetFilterString(this.GetChangeFieldExpression(fieldByLocalizedName, true).ToPath(), (byte) 13, forConditionString2, requestContext), (object) this.GetFilterString(this.GetChangeFieldExpression(fieldByLocalizedName, false).ToPath(), (byte) 12, forConditionString2, requestContext));
        default:
          return this.GetFilterString(fieldByLocalizedName, clause, requestContext);
      }
    }

    protected virtual string GetFilterString(
      ExpressionFilterField field,
      ExpressionFilterClause clause,
      IVssRequestContext requestContext = null)
    {
      return this.GetFilterString(field.InvariantFieldName, this.GetRawOperator(clause), clause.Value, requestContext);
    }

    protected virtual string GetFilterFieldClauseValueForConditionString(
      ExpressionFilterField field,
      ExpressionFilterClause filterClause)
    {
      return filterClause.Value;
    }

    protected virtual PathSubscriptionExpression GetChangeFieldExpression(
      ExpressionFilterField field,
      bool useOldChangeValue)
    {
      return this.GetExpressionParser();
    }

    protected abstract PathSubscriptionExpression GetExpressionParser();

    protected abstract string GetFilterString(
      string rawFieldName,
      byte rawOperator,
      string rawValue,
      IVssRequestContext requestContext = null);

    protected byte GetRawOperator(ExpressionFilterClause clause) => SubscriptionFilterOperators.GetRawOperator(clause.Operator);

    protected static string QuoteString(string rawValue) => rawValue != null && rawValue.Contains("'") ? "\"" + rawValue + "\"" : "'" + rawValue + "'";

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscription notificationSubscription)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(notificationSubscription.Filter.EventType, "FilterEventType");
      Subscription subscription = base.ToSubscription(requestContext, notificationSubscription);
      if (notificationSubscription.Description != null)
        subscription.Tag = string.Format(PathSubscriptionAdapter.s_subscriptionNameFormat, (object) notificationSubscription.Description);
      this.SetSubscriptionFilter(requestContext, subscription, notificationSubscription.Filter);
      return subscription;
    }

    public override Subscription ToSubscription(
      IVssRequestContext requestContext,
      NotificationSubscriptionCreateParameters createParameters)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(createParameters.Filter.EventType, "FilterEventType");
      Subscription subscription = base.ToSubscription(requestContext, createParameters);
      if (createParameters.Description != null)
        subscription.Tag = string.Format(PathSubscriptionAdapter.s_subscriptionNameFormat, (object) createParameters.Description);
      this.SetSubscriptionFilter(requestContext, subscription, createParameters.Filter);
      return subscription;
    }

    private void SetSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      ISubscriptionFilter filter)
    {
      if (!(filter is ExpressionFilter filter1))
        throw new InvalidSubscriptionException(CoreRes.InvalidSubscriptionFilter());
      string matcher = this.GetMatcher(requestContext, filter.EventType);
      subscription.Matcher = matcher;
      if (filter1.FilterModel == null)
        return;
      string validatedConditionString = this.GetValidatedConditionString(requestContext, filter1);
      subscription.SubscriptionScope = this.CurrentScope;
      subscription.ConditionString = validatedConditionString;
      subscription.Expression = validatedConditionString;
    }

    public string ParseTagData(
      IVssRequestContext requestContext,
      Subscription subscription,
      ref bool isBasicAlert)
    {
      this.EnsureFieldsLoaded(requestContext);
      return this.GetSubscriptionNameFromTag(subscription, subscription.Tag, ref isBasicAlert);
    }

    internal string GetSubscriptionNameFromTag(
      Subscription subscription,
      string subscriptionTag,
      ref bool isBasicAlert)
    {
      if (string.IsNullOrWhiteSpace(subscriptionTag))
        return (string) null;
      Match match = PathSubscriptionAdapter.s_subscriptionNameRegEx.Match(subscriptionTag);
      if (match.Success)
        return match.Groups["name"].Value.Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">");
      string alertNameFromTag = this.GetBasicAlertNameFromTag(subscriptionTag);
      if (string.IsNullOrEmpty(alertNameFromTag))
      {
        isBasicAlert = false;
        return subscriptionTag;
      }
      isBasicAlert = true;
      return alertNameFromTag;
    }

    public override NotificationSubscription ToNotificationSubscription(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Subscription>(subscription, nameof (subscription));
      bool isBasicAlert = false;
      NotificationSubscription notificationSubscription = base.ToNotificationSubscription(requestContext, subscription, queryFlags);
      if (string.IsNullOrEmpty(notificationSubscription.Description))
        notificationSubscription.Description = this.ParseTagData(requestContext, subscription, ref isBasicAlert);
      this.PopulateNotificationSubscriptionLinks(requestContext, subscription, notificationSubscription);
      return notificationSubscription;
    }

    public override void PrepareForDisplay(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter)
    {
      this.EnsureFieldsLoaded(requestContext);
      ExpressionFilter expressionFilter = filter as ExpressionFilter;
      if (filter == null)
        throw new InvalidSubscriptionException(CoreRes.InvalidSubscriptionFilter());
      if (expressionFilter.FilterModel == null)
        return;
      foreach (ExpressionFilterClause clause in (IEnumerable<ExpressionFilterClause>) expressionFilter.FilterModel.Clauses)
      {
        if (!string.IsNullOrEmpty(clause.LogicalOperator))
          clause.LogicalOperator = SubscriptionFilterOperators.GetLocalizedOperator(clause.LogicalOperator);
        clause.FieldName = this.GetLocalizedFieldName(clause.FieldName);
        clause.Operator = SubscriptionFilterOperators.GetLocalizedOperator(clause.Operator);
        if (!string.IsNullOrEmpty(clause.Value))
        {
          ExpressionFilterField fieldByLocalizedName = this.GetFieldByLocalizedName(clause.FieldName);
          if (fieldByLocalizedName != null)
            clause.Value = fieldByLocalizedName.GetLocalizedValue(requestContext, clause.Value);
        }
      }
    }

    public override void ApplyToSubscriptionLookup(
      IVssRequestContext requestContext,
      SubscriptionLookup lookup,
      ISubscriptionFilter filter)
    {
      ArgumentUtility.CheckForNull<SubscriptionLookup>(lookup, nameof (lookup));
      ArgumentUtility.CheckForNull<ISubscriptionFilter>(filter, nameof (filter));
      if ((filter as ExpressionFilter).FilterModel != null)
        throw new ArgumentException(NotificationsWebApiResources.Error_CannotSearchWithCriteria());
      lookup.Matcher = "PathMatcher";
      lookup.EventType = filter.EventType;
    }

    public override void PostBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      subscription.ConditionString = subscription.Expression;
    }

    public override void PreBindSubscription(
      IVssRequestContext requestContext,
      Subscription subscription)
    {
      subscription.Expression = subscription.ConditionString;
    }

    public override ISubscriptionFilter CreateSubscriptionFilter(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionQueryFlags queryFlags)
    {
      ExpressionFilterModel filterModel = !queryFlags.HasFlag((Enum) SubscriptionQueryFlags.IncludeFilterDetails) ? new ExpressionFilterModel() : this.ParseCondition(requestContext, subscription.ConditionString);
      return (ISubscriptionFilter) new ExpressionFilter(subscription.EventTypeName, filterModel);
    }

    public override void ApplyFilterUpdatesToSubscription(
      IVssRequestContext requestContext,
      Subscription subscriptionToPatch,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      base.ApplyFilterUpdatesToSubscription(requestContext, subscriptionToPatch, updateParameters);
      Guid? id = updateParameters.Scope?.Id;
      if (id.HasValue)
        subscriptionToPatch.SubscriptionScope.Id = updateParameters.Scope.Id;
      if (updateParameters.Filter == null)
        return;
      if (!(updateParameters.Filter is ExpressionFilter filter))
        throw new ArgumentException(CoreRes.InvalidSubscriptionFilter());
      if (filter.FilterModel?.Clauses == null)
        return;
      string validatedConditionString = this.GetValidatedConditionString(requestContext, filter);
      id = this.CurrentScope?.Id;
      Guid collectionScope = NotificationClientConstants.CollectionScope;
      if ((id.HasValue ? (id.HasValue ? (id.GetValueOrDefault() != collectionScope ? 1 : 0) : 0) : 1) != 0)
        subscriptionToPatch.SubscriptionScope.Id = this.CurrentScope.Id;
      subscriptionToPatch.ConditionString = validatedConditionString;
    }

    private void LoadContributedFields(IVssRequestContext requestContext)
    {
      if (requestContext == null || this.m_contributedFieldsInitialized)
        return;
      NotificationEventType eventType = requestContext.GetService<INotificationEventService>().GetEventType(requestContext, this.SubscriptionTypeName, EventTypeQueryFlags.IncludeFields);
      if (eventType != null && eventType.Fields != null)
      {
        foreach (NotificationEventField notificationEventField in eventType.Fields.Values)
          this.m_contributedFiledsByInvariantName.Add(notificationEventField.Path, notificationEventField);
      }
      this.m_contributedFieldsInitialized = true;
    }

    protected virtual string GetLocalizedFieldName(ExpressionFilterField field)
    {
      NotificationEventField notificationEventField;
      return this.m_contributedFiledsByInvariantName.TryGetValue(field.InvariantFieldName, out notificationEventField) ? notificationEventField.Name : field.LocalizedFieldName;
    }

    protected virtual string GetLocalizedFieldName(string fieldId)
    {
      NotificationEventField notificationEventField = this.m_contributedFiledsByInvariantName.Values.Where<NotificationEventField>((Func<NotificationEventField, bool>) (x => x.Id.Equals(fieldId))).FirstOrDefault<NotificationEventField>();
      return notificationEventField != null ? notificationEventField.Name : fieldId;
    }

    internal string GetValidatedConditionString(
      IVssRequestContext requestContext,
      ExpressionFilter filter)
    {
      EventSerializerType serializationFormatForEvent = requestContext.GetService<INotificationEventService>().GetSerializationFormatForEvent(requestContext, filter.EventType);
      string conditionString = this.ToConditionString(requestContext, filter.FilterModel);
      this.GetMatcher(requestContext, filter.EventType);
      this.ValidateConditionString(serializationFormatForEvent, conditionString);
      return conditionString;
    }

    private void ValidateConditionString(EventSerializerType type, string conditionString)
    {
      TeamFoundationEventConditionParser parser = TeamFoundationEventConditionParser.GetParser(type, conditionString);
      try
      {
        parser.Parse();
      }
      catch (Exception ex)
      {
        throw new ArgumentException(CoreRes.InvalidSubscriptionExpression((object) conditionString), ex);
      }
    }

    public virtual Dictionary<string, ExpressionFilterField> GetFields(
      IVssRequestContext requestContext,
      SubscriptionScope scope)
    {
      this.EnsureFieldsLoaded(requestContext);
      this.FieldInitialized = true;
      return this.m_fieldsByInvariantName;
    }
  }
}
