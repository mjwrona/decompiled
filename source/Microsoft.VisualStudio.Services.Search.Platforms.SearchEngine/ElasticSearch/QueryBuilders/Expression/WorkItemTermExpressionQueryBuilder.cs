// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.WorkItemTermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Expression.WorkItem;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal class WorkItemTermExpressionQueryBuilder
  {
    [StaticSafe]
    private static readonly IDictionary<Operator, string> s_operatorNames = (IDictionary<Operator, string>) new FriendlyDictionary<Operator, string>()
    {
      [Operator.LessThan] = "lt",
      [Operator.LessThanOrEqual] = "lte",
      [Operator.GreaterThan] = "gt",
      [Operator.GreaterThanOrEqual] = "gte"
    };
    private const string AllFieldsRefName = "*.*";
    private const string WorkItemNonCompositeEligibleHighlightedFieldsList = "system.id;system.history";
    private static readonly string s_compositeStringFieldName = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.String).CompositePlatformFieldName;
    private static readonly string s_compositeIntegerFieldName = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Integer).CompositePlatformFieldName;
    private static readonly string s_compositeRealFieldName = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Real).CompositePlatformFieldName;
    private static readonly string s_compositeDateTimeFieldName = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.DateTime).CompositePlatformFieldName;
    private static readonly string s_compositePathFieldName = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Path).CompositePlatformFieldName;
    private static readonly string s_compositeHtmlFieldName = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Html).CompositePlatformFieldName;
    private static readonly string s_compositeIdentityFieldName = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Identity).CompositePlatformFieldName;
    private static readonly string s_compositeNameFieldName = WorkItemIndexedField.FromWitField("*.*", WorkItemContract.FieldType.Name).CompositePlatformFieldName;
    private static readonly string s_titlePlatformFieldName = WorkItemIndexedField.FromWitField("System.Title", WorkItemContract.FieldType.String).PlatformFieldName;
    private static readonly string s_descriptionPlatformFieldName = WorkItemIndexedField.FromWitField("System.Description", WorkItemContract.FieldType.Html).PlatformFieldName;
    private static readonly string s_assignedToPlatformFieldName = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.String).PlatformFieldName;
    private static readonly string s_assignedToIdentityPlatformFieldName = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.Identity).PlatformFieldName;
    private static readonly string s_assignedToNamePlatformFieldName = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.Name).PlatformFieldName;
    private static readonly string s_idPlatformFieldName = WorkItemIndexedField.FromWitField("System.Id", WorkItemContract.FieldType.IntegerAsString).PlatformFieldName;
    private List<WorkItemIndexedField> m_highlightableFields;
    private bool m_searchOnStemmedFields;
    private bool m_queryingOnIdentitiesEnabled;
    private WorkItemContract.FieldType m_identityFieldType = WorkItemContract.FieldType.Name;

    public string Build(IVssRequestContext requestContext, IExpression expression)
    {
      this.m_queryingOnIdentitiesEnabled = requestContext.IsFeatureEnabled("Search.Server.WorkItem.QueryIdentityFields");
      if (!(bool) requestContext.Items["isUserAnonymousKey"])
        this.m_identityFieldType = WorkItemContract.FieldType.Identity;
      this.m_highlightableFields = this.GetHighlightableFields(requestContext);
      this.m_searchOnStemmedFields = requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/WorkItemSearchOnlyOnStemmedFields", TeamFoundationHostType.Deployment);
      TermExpression termExpression = (TermExpression) expression;
      switch (termExpression.Operator)
      {
        case Operator.Equals:
        case Operator.NotEquals:
          return this.GetFilteredMultiMatchQueryStringForEqualityOperators(termExpression);
        case Operator.LessThan:
          return this.GetRangeQueryStringForLessThan(termExpression);
        case Operator.LessThanOrEqual:
          return this.GetRangeQueryStringForLessOrThanEqual(termExpression);
        case Operator.GreaterThan:
          return this.GetRangeQueryStringForGreaterThan(termExpression);
        case Operator.GreaterThanOrEqual:
          return this.GetRangeQueryStringForGreaterThanOrEqual(termExpression);
        case Operator.Matches:
          return this.GetMultiMatchQueryString(termExpression);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Operator [{0}] is not supported in work item search.", (object) termExpression.Operator)));
      }
    }

    private void ValidateAgainstDefaultExpressionType(TermExpression termExpression)
    {
      if (termExpression.IsOfType("*"))
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Term expression [{0}] with default type does not support conditional operators.", (object) termExpression)));
    }

    private string GetRangeQueryStringForLessOrThanEqual(TermExpression termExpression)
    {
      this.ValidateAgainstDefaultExpressionType(termExpression);
      if (ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value) != (ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String))
        return this.GetRangeQueryString(termExpression);
      int daysToAdd = 1;
      string dateTimeStringValue;
      this.TryNormalizeDateTimeAsStringAndAddDays(termExpression, daysToAdd, out dateTimeStringValue);
      return this.GetRangeQueryStringForValueOfDateTimeDataType(termExpression, dateTimeStringValue);
    }

    private string GetRangeQueryStringForLessThan(TermExpression termExpression)
    {
      this.ValidateAgainstDefaultExpressionType(termExpression);
      if (ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value) != (ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String))
        return this.GetRangeQueryString(termExpression);
      int daysToAdd = 0;
      string dateTimeStringValue;
      this.TryNormalizeDateTimeAsStringAndAddDays(termExpression, daysToAdd, out dateTimeStringValue);
      return this.GetRangeQueryStringForValueOfDateTimeDataType(termExpression, dateTimeStringValue);
    }

    private string GetRangeQueryStringForGreaterThanOrEqual(TermExpression termExpression)
    {
      this.ValidateAgainstDefaultExpressionType(termExpression);
      if (ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value) != (ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String))
        return this.GetRangeQueryString(termExpression);
      int daysToAdd = 0;
      string dateTimeStringValue;
      this.TryNormalizeDateTimeAsStringAndAddDays(termExpression, daysToAdd, out dateTimeStringValue);
      return this.GetRangeQueryStringForValueOfDateTimeDataType(termExpression, dateTimeStringValue);
    }

    private string GetRangeQueryStringForGreaterThan(TermExpression termExpression)
    {
      this.ValidateAgainstDefaultExpressionType(termExpression);
      if (ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value) != (ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String))
        return this.GetRangeQueryString(termExpression);
      int daysToAdd = 1;
      string dateTimeStringValue;
      this.TryNormalizeDateTimeAsStringAndAddDays(termExpression, daysToAdd, out dateTimeStringValue);
      return this.GetRangeQueryStringForValueOfDateTimeDataType(termExpression, dateTimeStringValue);
    }

    private bool TryNormalizeDateTimeAsStringAndAddDays(
      TermExpression termExpression,
      int daysToAdd,
      out string dateTimeStringValue)
    {
      DateTime result;
      if (DateTime.TryParse(termExpression.Value, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
      {
        dateTimeStringValue = result.ToUniversalTime().AddDays((double) daysToAdd).ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      dateTimeStringValue = string.Empty;
      return false;
    }

    private string GetRangeQueryString(TermExpression termExpression)
    {
      ExpressionBuilderUtils.DataType resolvableDataTypesOf = ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value);
      switch (resolvableDataTypesOf)
      {
        case ExpressionBuilderUtils.DataType.String:
          return string.Empty;
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
          return this.GetRangeQueryStringForValueOfDoubleDataType(termExpression, termExpression.Type);
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
          return this.GetRangeQueryStringForValueOfBothDateAndDoubleDataTypes(termExpression, termExpression.Type);
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.Int32 | ExpressionBuilderUtils.DataType.String:
          return this.GetRangeQueryStringForValueOfIntegerDataType(termExpression, termExpression.Type);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Type combination [{0}] for value [{1}] is not supported.", (object) resolvableDataTypesOf, (object) termExpression.Value)));
      }
    }

    private string GetRangeQueryStringForValueOfBothDateAndDoubleDataTypes(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      string result1;
      ExpressionBuilderUtils.TryNormalizeDateTimeAsString(termExpression.Value, out result1);
      string result2;
      ExpressionBuilderUtils.TryNormalizeDoubleAsString(termExpression.Value, out result2);
      double integerRange = WorkItemTermExpressionQueryBuilder.TrimDoubleToIntegerRange(result2);
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"bool\": {{\r\n                        \"should\": [\r\n                            {{\r\n                                \"range\": {{\r\n                                    \"{0}\": {{\r\n                                        \"{1}\": \"{2}\"\r\n                                    }}\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"range\": {{\r\n                                    \"{3}\": {{\r\n                                        \"{4}\": {5}\r\n                                    }}\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"range\": {{\r\n                                    \"{6}\": {{\r\n                                        \"{7}\": {8}\r\n                                    }}\r\n                                }}\r\n                            }}\r\n                        ]\r\n                    }}\r\n                }}", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.DateTime).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[termExpression.Operator], (object) result1, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[termExpression.Operator], (object) result2, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Integer).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[termExpression.Operator], (object) integerRange));
    }

    private string GetRangeQueryStringForValueOfIntegerDataType(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      string result;
      ExpressionBuilderUtils.TryNormalizeIntegerAsString(termExpression.Value, out result);
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"bool\": {{\r\n                        \"should\": [\r\n                            {{\r\n                                \"range\": {{\r\n                                    \"{0}\": {{\r\n                                        \"{1}\": {2}\r\n                                    }}\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"range\": {{\r\n                                    \"{3}\": {{\r\n                                        \"{4}\": {5}\r\n                                    }}\r\n                                }}\r\n                            }}\r\n                        ]\r\n                    }}\r\n                }}", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[termExpression.Operator], (object) result, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Integer).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[termExpression.Operator], (object) result));
    }

    private string GetRangeQueryStringForValueOfDoubleDataType(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      string result;
      ExpressionBuilderUtils.TryNormalizeDoubleAsString(termExpression.Value, out result);
      double integerRange = WorkItemTermExpressionQueryBuilder.TrimDoubleToIntegerRange(result);
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"bool\": {{\r\n                        \"should\": [\r\n                            {{\r\n                                \"range\": {{\r\n                                    \"{0}\": {{\r\n                                        \"{1}\": {2}\r\n                                    }}\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"range\": {{\r\n                                    \"{3}\": {{\r\n                                        \"{4}\": {5}\r\n                                    }}\r\n                                }}\r\n                            }}\r\n                        ]\r\n                    }}\r\n                }}", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[termExpression.Operator], (object) result, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Integer).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[termExpression.Operator], (object) integerRange));
    }

    private string GetRangeQueryStringForValueOfDateTimeDataType(
      TermExpression termExpression,
      string dateTimeStringValue)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                   \"range\": {{\r\n                        \"{0}\": {{\r\n                            \"{1}\": \"{2}\"\r\n                        }}\r\n                    }}\r\n                }}", (object) WorkItemIndexedField.FromWitField(termExpression.Type, WorkItemContract.FieldType.DateTime).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[termExpression.Operator], (object) dateTimeStringValue));
    }

    private string GetMultiMatchQueryString(TermExpression termExpression) => termExpression.Value.Contains("*") || termExpression.Value.Contains("?") ? (termExpression.IsOfType("*") ? this.GetFullTextMultiWildcardQueryString(termExpression) : this.GetFilteredMultiWildcardQueryString(termExpression)) : (termExpression.IsOfType("*") ? this.GetFullTextMultiMatchQueryString(termExpression) : this.GetFilteredMultiMatchQueryString(termExpression));

    private string GetFilteredMultiWildcardQueryString(TermExpression termExpression)
    {
      string type = termExpression.Type;
      return !this.m_queryingOnIdentitiesEnabled ? FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_wildcard\": {{\r\n                        \"value\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\",\r\n                            \"{2}.{3}\",\r\n                            \"{4}.{5}\",\r\n                            \"{6}\",\r\n                            \"{7}.{8}\",\r\n                            \"{9}.{10}\",\r\n                            \"{11}\"\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value.NormalizePath()), (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.String).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.String).PlatformFieldName, (object) "pattern", (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.Html).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "pattern", (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.Path).PlatformFieldName)) : FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_wildcard\": {{\r\n                        \"value\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\",\r\n                            \"{2}.{3}\",\r\n                            \"{4}.{5}\",\r\n                            \"{6}\",\r\n                            \"{7}.{8}\",\r\n                            \"{9}.{10}\",\r\n                            \"{11}\",\r\n                            \"{12}\"\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value.NormalizePath()), (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.String).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.String).PlatformFieldName, (object) "pattern", (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.Html).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "pattern", (object) WorkItemIndexedField.FromWitField(type, WorkItemContract.FieldType.Path).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(type, this.m_identityFieldType).PlatformFieldName));
    }

    private string GetFullTextMultiWildcardQueryString(TermExpression termExpression)
    {
      List<string> wildcardOrStringValues = this.GetPlatformFieldNamesToQueryForWildcardOrStringValues();
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_wildcard\": {{\r\n                        \"value\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value.NormalizePath()), (object) string.Join(",", wildcardOrStringValues.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFilteredMultiMatchQueryString(TermExpression termExpression)
    {
      string type = termExpression.Type;
      ExpressionBuilderUtils.DataType resolvableDataTypesOf = ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value);
      switch (resolvableDataTypesOf)
      {
        case ExpressionBuilderUtils.DataType.String:
          return this.GetFilteredMultiMatchQueryStringForValueOfStringDataType(termExpression, type);
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String:
          return this.GetFilteredMultiMatchQueryStringForValueOfDateTimeDataType(termExpression, type);
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
          return this.GetFilteredMultiMatchQueryStringForValueOfDoubleDataType(termExpression, type);
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
          return this.GetFilteredMultiMatchQueryStringForValueOfBothDateTimeAndDoubleDataTypes(termExpression, type);
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.Int32 | ExpressionBuilderUtils.DataType.String:
          return this.GetFilteredMultiMatchQueryStringForValueOfIntegerDataType(termExpression, type);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Type combination [{0}] for value [{1}] is not supported.", (object) resolvableDataTypesOf, (object) termExpression.Value)));
      }
    }

    private string GetFilteredMultiMatchQueryStringForEqualityOperators(
      TermExpression termExpression)
    {
      this.ValidateAgainstDefaultExpressionType(termExpression);
      string type = termExpression.Type;
      ExpressionBuilderUtils.DataType resolvableDataTypesOf = ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value);
      switch (resolvableDataTypesOf)
      {
        case ExpressionBuilderUtils.DataType.String:
          return string.Empty;
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String:
          return this.GetFilteredRangeQueryStringForDateTimeDataTypeAndEqualityOperators(termExpression, type);
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
          return this.GetFilteredMatchQueryStringForDoubleDataTypeAndEqualityOperators(termExpression, type);
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.Int32 | ExpressionBuilderUtils.DataType.String:
          return this.GetFilteredMatchQueryStringForIntegerDataTypeAndEqualityOperators(termExpression, type);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Type combination [{0}] for value [{1}] is not supported.", (object) resolvableDataTypesOf, (object) termExpression.Value)));
      }
    }

    private string GetFilteredMultiMatchQueryStringForValueOfBothDateTimeAndDoubleDataTypes(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      string result1;
      ExpressionBuilderUtils.TryNormalizeDateTimeAsString(termExpression.Value, out result1);
      string result2;
      ExpressionBuilderUtils.TryNormalizeDoubleAsString(termExpression.Value, out result2);
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"bool\": {{\r\n                        \"should\": [\r\n                            {{\r\n                                \"multi_match\": {{\r\n                                    \"query\": {0},\r\n                                    \"fields\": [\r\n                                        \"{1}.{2}\",\r\n                                        \"{3}.{4}\",\r\n                                        \"{5}\"\r\n                                    ],\r\n                                    \"type\": \"phrase\"\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"multi_match\": {{\r\n                                    \"query\": {6},\r\n                                    \"fields\": [\r\n                                        \"{7}\"\r\n                                    ]\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"multi_match\": {{\r\n                                    \"query\": {8},\r\n                                    \"fields\": [\r\n                                        \"{9}\"\r\n                                    ]\r\n                                }}\r\n                            }}\r\n                        ]\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Path).PlatformFieldName, (object) JsonConvert.SerializeObject((object) result1), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.DateTime).PlatformFieldName, (object) JsonConvert.SerializeObject((object) result2), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName));
    }

    private string GetFilteredMultiMatchQueryStringForValueOfStringDataType(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      return !this.m_queryingOnIdentitiesEnabled ? FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\",\r\n                            \"{2}.{3}\",\r\n                            \"{4}.{5}\",\r\n                            \"{6}\",\r\n                            \"{7}.{8}\",\r\n                            \"{9}.{10}\",\r\n                            \"{11}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value.NormalizePath()), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "pattern", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "pattern", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Path).PlatformFieldName)) : FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\",\r\n                            \"{2}.{3}\",\r\n                            \"{4}\",\r\n                            \"{5}.{6}\",\r\n                            \"{7}\",\r\n                            \"{8}.{9}\",\r\n                            \"{10}.{11}\",\r\n                            \"{12}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value.NormalizePath()), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, this.m_identityFieldType).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "pattern", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "pattern", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Path).PlatformFieldName));
    }

    private string GetFilteredMultiMatchQueryStringForValueOfIntegerDataType(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      ExpressionBuilderUtils.TryNormalizeIntegerAsString(termExpression.Value, out string _);
      return !this.m_queryingOnIdentitiesEnabled ? FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}.{2}\",\r\n                            \"{3}.{4}\",\r\n                            \"{5}\",\r\n                            \"{6}\",\r\n                            \"{7}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Path).PlatformFieldName)) : FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}.{2}\",\r\n                            \"{3}\",\r\n                            \"{4}.{5}\",\r\n                            \"{6}\",\r\n                            \"{7}\",\r\n                            \"{8}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, this.m_identityFieldType).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Path).PlatformFieldName));
    }

    private string GetFilteredMultiMatchQueryStringForValueOfDoubleDataType(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      ExpressionBuilderUtils.TryNormalizeDoubleAsString(termExpression.Value, out string _);
      return !this.m_queryingOnIdentitiesEnabled ? FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}.{2}\",\r\n                            \"{3}.{4}\",\r\n                            \"{5}\",\r\n                            \"{6}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Path).PlatformFieldName)) : FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}.{2}\",\r\n                            \"{3}\",\r\n                            \"{4}.{5}\",\r\n                            \"{6}\",\r\n                            \"{7}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, this.m_identityFieldType).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Path).PlatformFieldName));
    }

    private string GetFilteredMultiMatchQueryStringForValueOfDateTimeDataType(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      string result;
      ExpressionBuilderUtils.TryNormalizeDateTimeAsString(termExpression.Value, out result);
      string endDate = WorkItemTermExpressionQueryBuilder.GetEndDate(result);
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"bool\": {{\r\n                        \"should\": [\r\n                            {{\r\n                                \"multi_match\": {{\r\n                                    \"query\": {0},\r\n                                    \"fields\": [\r\n                                        \"{1}.{2}\",\r\n                                        \"{3}.{4}\",\r\n                                        \"{5}\"\r\n                                    ],\r\n                                    \"type\": \"phrase\"\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"range\": {{\r\n                                    \"{6}\": {{\r\n                                        \"{7}\": {8},\r\n                                        \"{9}\": {10}\r\n                                    }}\r\n                                }}\r\n                            }}\r\n                        ]\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.String).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Html).PlatformFieldName, (object) "stemmed", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Path).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.DateTime).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[Operator.GreaterThanOrEqual], (object) JsonConvert.SerializeObject((object) result), (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[Operator.LessThan], (object) JsonConvert.SerializeObject((object) endDate)));
    }

    private string GetFilteredRangeQueryStringForDateTimeDataTypeAndEqualityOperators(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      string result;
      ExpressionBuilderUtils.TryNormalizeDateTimeAsString(termExpression.Value, out result);
      string endDate = WorkItemTermExpressionQueryBuilder.GetEndDate(result);
      switch (termExpression.Operator)
      {
        case Operator.Equals:
          return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                                \"range\": {{\r\n                                    \"{0}\": {{\r\n                                        \"{1}\": {2},\r\n                                        \"{3}\": {4}\r\n                                    }}\r\n                                }}\r\n                            }}", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.DateTime).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[Operator.GreaterThanOrEqual], (object) JsonConvert.SerializeObject((object) result), (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[Operator.LessThan], (object) JsonConvert.SerializeObject((object) endDate)));
        case Operator.NotEquals:
          return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                            \"bool\": {{\r\n                                \"must_not\": [\r\n                                    {{\r\n                                        \"range\": {{\r\n                                            \"{0}\": {{\r\n                                                \"{1}\": {2},\r\n                                                \"{3}\": {4}\r\n                                            }}\r\n                                        }}\r\n                                    }},\r\n                                    {{\r\n                                        \"constant_score\": {{\r\n                                            \"filter\": {{\r\n                                                \"bool\": {{\r\n                                                    \"must_not\": [\r\n                                                        {{\r\n                                                            \"exists\": {{\r\n                                                                \"field\": \"{5}\"\r\n                                                            }}\r\n                                                        }}\r\n                                                    ]\r\n                                                }}\r\n                                            }}\r\n                                        }}\r\n                                    }}\r\n                                ]                                \r\n                            }}\r\n                        }}", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.DateTime).PlatformFieldName, (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[Operator.GreaterThanOrEqual], (object) JsonConvert.SerializeObject((object) result), (object) WorkItemTermExpressionQueryBuilder.s_operatorNames[Operator.LessThan], (object) JsonConvert.SerializeObject((object) endDate), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.DateTime).PlatformFieldName));
        default:
          return string.Empty;
      }
    }

    private static string GetEndDate(string startDateTimeStringValue) => DateTime.Parse(startDateTimeStringValue, (IFormatProvider) CultureInfo.InvariantCulture).AddDays(1.0).ToUniversalTime().ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);

    private string GetFilteredMatchQueryStringForDoubleDataTypeAndEqualityOperators(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      string result;
      ExpressionBuilderUtils.TryNormalizeDoubleAsString(termExpression.Value, out result);
      double num = double.Parse(result, (IFormatProvider) CultureInfo.InvariantCulture);
      bool includeIntegerFields = Math.Abs(num - Math.Round(num)) < double.Epsilon;
      switch (termExpression.Operator)
      {
        case Operator.Equals:
          return this.GetFilteredMultiMatchQueryStringForValueOfDoubleDataTypeWithEqualsOperator(includeIntegerFields, num, fieldReferenceName);
        case Operator.NotEquals:
          return this.GetFilteredMultiMatchQueryStringForValueOfDoubleDataTypeWithNotEqualsOperator(includeIntegerFields, num, fieldReferenceName);
        default:
          return string.Empty;
      }
    }

    private string GetFilteredMatchQueryStringForIntegerDataTypeAndEqualityOperators(
      TermExpression termExpression,
      string fieldReferenceName)
    {
      ExpressionBuilderUtils.TryNormalizeIntegerAsString(termExpression.Value, out string _);
      switch (termExpression.Operator)
      {
        case Operator.Equals:
          return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                                \"multi_match\": {{\r\n                                    \"query\": {0},\r\n                                    \"fields\": [\r\n                                        \"{1}\",\r\n                                        \"{2}\"\r\n                                    ]\r\n                                }}\r\n                            }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName));
        case Operator.NotEquals:
          return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                            \"bool\": {{\r\n                                \"should\": [\r\n                                    {{\r\n                                        \"bool\": {{\r\n                                            \"must_not\": [\r\n                                                {{\r\n                                                    \"multi_match\": {{\r\n                                                        \"query\": {0},\r\n                                                        \"fields\": [\r\n                                                            \"{1}\"\r\n                                                        ]\r\n                                                    }}\r\n                                                }},\r\n                                                {{\r\n                                                    \"constant_score\": {{\r\n                                                        \"filter\": {{\r\n                                                            \"bool\": {{\r\n                                                                \"must_not\": [\r\n                                                                    {{\r\n                                                                        \"exists\": {{\r\n                                                                            \"field\": \"{2}\"\r\n                                                                        }}\r\n                                                                    }}\r\n                                                                ]\r\n                                                            }}\r\n                                                        }}\r\n                                                    }}\r\n                                                }}\r\n                                            ]\r\n                                        }}\r\n                                    }},\r\n                                    {{\r\n                                        \"bool\": {{\r\n                                            \"must_not\": [\r\n                                                {{\r\n                                                    \"multi_match\": {{\r\n                                                        \"query\": {3},\r\n                                                        \"fields\": [\r\n                                                            \"{4}\"\r\n                                                        ]\r\n                                                    }}\r\n                                                }},\r\n                                                {{\r\n                                                    \"constant_score\": {{\r\n                                                        \"filter\": {{\r\n                                                            \"bool\": {{\r\n                                                                \"must_not\": [\r\n                                                                    {{\r\n                                                                        \"exists\": {{\r\n                                                                            \"field\": \"{5}\"\r\n                                                                        }}\r\n                                                                    }}\r\n                                                                ]\r\n                                                            }}\r\n                                                        }}\r\n                                                    }}\r\n                                                }}\r\n                                            ]\r\n                                        }}\r\n                                    }}\r\n                                ]\r\n                            }}\r\n                        }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName, (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName));
        default:
          return string.Empty;
      }
    }

    private string GetFilteredMultiMatchQueryStringForValueOfDoubleDataTypeWithEqualsOperator(
      bool includeIntegerFields,
      double doubleValue,
      string fieldReferenceName)
    {
      bool flag = (double) int.MinValue < doubleValue && doubleValue < (double) int.MaxValue;
      return includeIntegerFields & flag ? FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                        \"bool\": {{\r\n                            \"should\": [\r\n                                {{\r\n                                    \"multi_match\": {{\r\n                                        \"query\": {0},\r\n                                        \"fields\": [\r\n                                            \"{1}\"\r\n                                        ]\r\n                                    }}\r\n                                }},\r\n                                {{\r\n                                    \"multi_match\": {{\r\n                                        \"query\": {2},\r\n                                        \"fields\": [\r\n                                            \"{3}\"\r\n                                        ]\r\n                                    }}\r\n                                }}\r\n                            ]\r\n                        }}\r\n                     }}", (object) JsonConvert.SerializeObject((object) doubleValue), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) JsonConvert.SerializeObject((object) (int) doubleValue), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName)) : FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                        \"multi_match\": {{\r\n                            \"query\": {0},\r\n                            \"fields\": [\r\n                                \"{1}\"\r\n                            ]\r\n                        }}\r\n                      }}", (object) JsonConvert.SerializeObject((object) doubleValue), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName));
    }

    private string GetFilteredMultiMatchQueryStringForValueOfDoubleDataTypeWithNotEqualsOperator(
      bool includeIntegerFields,
      double doubleValue,
      string fieldReferenceName)
    {
      bool flag = (double) int.MinValue < doubleValue && doubleValue < (double) int.MaxValue;
      return includeIntegerFields & flag ? FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                        \"bool\": {{\r\n                            \"should\": [\r\n                                {{\r\n                                    \"bool\": {{\r\n                                        \"must_not\": [\r\n                                            {{\r\n                                                \"multi_match\": {{\r\n                                                    \"query\": {0},\r\n                                                    \"fields\": [                                                   \r\n                                                        \"{1}\"\r\n                                                    ]\r\n                                                 }}\r\n                                            }},\r\n                                            {{\r\n                                                \"constant_score\": {{\r\n                                                    \"filter\": {{\r\n                                                        \"bool\": {{\r\n                                                            \"must_not\": [\r\n                                                                {{\r\n                                                                    \"exists\": {{\r\n                                                                        \"field\": \"{2}\"\r\n                                                                    }}\r\n                                                                }}\r\n                                                            ]\r\n                                                        }}\r\n                                                    }}\r\n                                                }}\r\n                                            }}\r\n                                        ]\r\n                                    }}\r\n                                }},\r\n                                {{\r\n                                    \"bool\": {{\r\n                                        \"must_not\": [\r\n                                            {{\r\n                                                \"multi_match\": {{\r\n                                                    \"query\": {3},\r\n                                                    \"fields\": [                                                   \r\n                                                        \"{4}\"\r\n                                                    ]\r\n                                                }}\r\n                                            }},\r\n                                            {{\r\n                                                \"constant_score\": {{\r\n                                                    \"filter\": {{\r\n                                                        \"bool\": {{\r\n                                                            \"must_not\": [\r\n                                                                {{\r\n                                                                    \"exists\": {{\r\n                                                                        \"field\": \"{5}\"\r\n                                                                    }}\r\n                                                                }}\r\n                                                            ]\r\n                                                        }}\r\n                                                    }}\r\n                                                }}\r\n                                            }}\r\n                                        ]\r\n                                    }}\r\n                                }}\r\n                             ]\r\n                        }}\r\n                    }}", (object) JsonConvert.SerializeObject((object) doubleValue), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) JsonConvert.SerializeObject((object) (int) doubleValue), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName)) : FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                        \"bool\": {{\r\n                            \"should\": [\r\n                                {{\r\n                                    \"bool\": {{                                    \r\n                                        \"must_not\": [                                      \r\n                                            {{\r\n                                                \"constant_score\": {{\r\n                                                    \"filter\": {{\r\n                                                        \"bool\": {{\r\n                                                            \"must_not\": [\r\n                                                                {{\r\n                                                                    \"exists\": {{\r\n                                                                        \"field\": \"{0}\"\r\n                                                                    }}\r\n                                                                }}\r\n                                                            ]\r\n                                                        }}\r\n                                                    }}\r\n                                                }}\r\n                                            }}\r\n                                        ]\r\n                                    }}\r\n                                }},\r\n                                {{\r\n                                    \"bool\": {{\r\n                                        \"must_not\": [\r\n                                            {{\r\n                                                \"multi_match\": {{\r\n                                                    \"query\": {1},\r\n                                                    \"fields\": [                                                   \r\n                                                        \"{2}\"\r\n                                                    ]\r\n                                                 }}\r\n                                            }},\r\n                                            {{\r\n                                                \"constant_score\": {{\r\n                                                    \"filter\": {{\r\n                                                        \"bool\": {{\r\n                                                            \"must_not\": [\r\n                                                                {{\r\n                                                                    \"exists\": {{\r\n                                                                        \"field\": \"{3}\"\r\n                                                                    }}\r\n                                                                }}\r\n                                                            ]\r\n                                                        }}\r\n                                                    }}\r\n                                                }}\r\n                                            }}\r\n                                        ]\r\n                                    }}\r\n                                }}\r\n                            ]\r\n                        }}\r\n                    }}", (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.IntegerAsString).PlatformFieldName, (object) JsonConvert.SerializeObject((object) doubleValue), (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName, (object) WorkItemIndexedField.FromWitField(fieldReferenceName, WorkItemContract.FieldType.Real).PlatformFieldName));
    }

    private string GetFullTextMultiMatchQueryString(TermExpression termExpression)
    {
      ExpressionBuilderUtils.DataType resolvableDataTypesOf = ExpressionBuilderUtils.GetResolvableDataTypesOf(termExpression.Value);
      switch (resolvableDataTypesOf)
      {
        case ExpressionBuilderUtils.DataType.String:
          return this.GetFullTextMultiMatchQueryStringForValueOfStringDataType(termExpression);
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.String:
          return this.GetFullTextMultiMatchQueryStringForValueOfDateTimeDataType(termExpression);
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
        case ExpressionBuilderUtils.DataType.DateTime | ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.String:
          return this.GetFullTextMultiMatchQueryStringForValueOfDoubleDataType(termExpression);
        case ExpressionBuilderUtils.DataType.Double | ExpressionBuilderUtils.DataType.Int32 | ExpressionBuilderUtils.DataType.String:
          return this.GetFullTextMultiMatchQueryStringForValueOfIntegerDataType(termExpression);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Type combination [{0}] for value [{1}] is not supported.", (object) resolvableDataTypesOf, (object) termExpression.Value)));
      }
    }

    private string GetFullTextMultiMatchQueryStringForValueOfStringDataType(
      TermExpression termExpression)
    {
      List<string> wildcardOrStringValues = this.GetPlatformFieldNamesToQueryForWildcardOrStringValues();
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value.NormalizePath()), (object) string.Join(",", wildcardOrStringValues.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFullTextMultiMatchQueryStringForValueOfIntegerDataType(
      TermExpression termExpression)
    {
      List<string> stringList = new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositePathFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositeRealFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositeIntegerFieldName))
      };
      stringList.AddRange(this.m_highlightableFields.Select<WorkItemIndexedField, string>((Func<WorkItemIndexedField, string>) (f => f.Type != WorkItemContract.FieldType.IntegerAsString && f.Type != WorkItemContract.FieldType.Path ? FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) f.PlatformFieldName, (object) "stemmed")) : FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) f.PlatformFieldName)))));
      stringList.AddRange((IEnumerable<string>) new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}^100", (object) WorkItemTermExpressionQueryBuilder.s_idPlatformFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^4", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^4", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^3", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^3", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "pattern"))
      });
      if (this.m_queryingOnIdentitiesEnabled)
        this.AddIdentityFieldsToQueryOn(stringList, 2);
      else
        stringList.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^2", (object) WorkItemTermExpressionQueryBuilder.s_assignedToPlatformFieldName)));
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", stringList.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFullTextMultiMatchQueryStringForValueOfDoubleDataType(
      TermExpression termExpression)
    {
      List<string> stringList = new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositePathFieldName))
      };
      if (!ExpressionBuilderUtils.IsInfinityForFloat(termExpression.Value))
        stringList.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositeRealFieldName)));
      stringList.AddRange(this.m_highlightableFields.Where<WorkItemIndexedField>((Func<WorkItemIndexedField, bool>) (f => f.Type != WorkItemContract.FieldType.AllTypes)).Select<WorkItemIndexedField, string>((Func<WorkItemIndexedField, string>) (f => f.Type != WorkItemContract.FieldType.Path ? FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) f.PlatformFieldName, (object) "stemmed")) : FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) f.PlatformFieldName)))));
      stringList.AddRange((IEnumerable<string>) new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^10", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^5", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "stemmed"))
      });
      if (this.m_queryingOnIdentitiesEnabled)
        this.AddIdentityFieldsToQueryOn(stringList, 4);
      else
        stringList.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^4", (object) WorkItemTermExpressionQueryBuilder.s_assignedToPlatformFieldName)));
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", stringList.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFullTextMultiMatchQueryStringForValueOfDateTimeDataType(
      TermExpression termExpression)
    {
      string result;
      ExpressionBuilderUtils.TryNormalizeDateTimeAsString(termExpression.Value, out result);
      List<string> stringList = new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositePathFieldName))
      };
      stringList.AddRange(this.m_highlightableFields.Where<WorkItemIndexedField>((Func<WorkItemIndexedField, bool>) (f => f.Type == WorkItemContract.FieldType.String || f.Type == WorkItemContract.FieldType.Html || f.Type == WorkItemContract.FieldType.Path)).Select<WorkItemIndexedField, string>((Func<WorkItemIndexedField, string>) (f => f.Type != WorkItemContract.FieldType.Path ? FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) f.PlatformFieldName, (object) "stemmed")) : FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) f.PlatformFieldName)))));
      stringList.AddRange((IEnumerable<string>) new string[4]
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^10", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^10", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^5", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^5", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "pattern"))
      });
      if (this.m_queryingOnIdentitiesEnabled)
        this.AddIdentityFieldsToQueryOn(stringList, 4);
      else
        stringList.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^4", (object) WorkItemTermExpressionQueryBuilder.s_assignedToPlatformFieldName)));
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"bool\": {{\r\n                        \"should\": [\r\n                            {{\r\n                                \"multi_match\": {{\r\n                                    \"query\": {0},\r\n                                    \"fields\": [\r\n                                        {1}\r\n                                    ],\r\n                                    \"type\": \"phrase\"\r\n                                }}\r\n                            }},\r\n                            {{\r\n                                \"multi_match\": {{\r\n                                    \"query\": {2},\r\n                                    \"fields\": [\r\n                                        \"{3}\"\r\n                                    ]\r\n                                }}\r\n                            }}\r\n                        ]\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", stringList.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f))))), (object) JsonConvert.SerializeObject((object) result), (object) WorkItemTermExpressionQueryBuilder.s_compositeDateTimeFieldName));
    }

    private List<string> GetPlatformFieldNamesToQueryForWildcardOrStringValues()
    {
      if (this.m_searchOnStemmedFields)
        return this.GetPlatformFieldNamesWithStemmingToQueryForWildcardOrStringValues();
      List<string> fieldsToQuery = new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositePathFieldName))
      };
      fieldsToQuery.AddRange(this.m_highlightableFields.Where<WorkItemIndexedField>((Func<WorkItemIndexedField, bool>) (f => f.Type == WorkItemContract.FieldType.String || f.Type == WorkItemContract.FieldType.Html || f.Type == WorkItemContract.FieldType.Path)).SelectMany<WorkItemIndexedField, string>((Func<WorkItemIndexedField, IEnumerable<string>>) (f => (IEnumerable<string>) new string[3]
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) f.PlatformFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) f.PlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) f.PlatformFieldName, (object) "pattern"))
      })));
      fieldsToQuery.AddRange((IEnumerable<string>) new string[7]
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}^100", (object) WorkItemTermExpressionQueryBuilder.s_idPlatformFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}^10", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^10", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^10", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}^5", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^5", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^5", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "pattern"))
      });
      if (this.m_queryingOnIdentitiesEnabled)
        this.AddIdentityFieldsToQueryOn(fieldsToQuery, 4, true);
      else
        fieldsToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^4", (object) WorkItemTermExpressionQueryBuilder.s_assignedToPlatformFieldName)));
      return fieldsToQuery;
    }

    private List<string> GetPlatformFieldNamesWithStemmingToQueryForWildcardOrStringValues()
    {
      List<string> fieldsToQuery = new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeStringFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemTermExpressionQueryBuilder.s_compositeHtmlFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositePathFieldName))
      };
      fieldsToQuery.AddRange(this.m_highlightableFields.Where<WorkItemIndexedField>((Func<WorkItemIndexedField, bool>) (f => f.Type == WorkItemContract.FieldType.String || f.Type == WorkItemContract.FieldType.Html || f.Type == WorkItemContract.FieldType.Path)).SelectMany<WorkItemIndexedField, string>((Func<WorkItemIndexedField, IEnumerable<string>>) (f => (IEnumerable<string>) new string[2]
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) f.PlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) f.PlatformFieldName, (object) "pattern"))
      })));
      fieldsToQuery.AddRange((IEnumerable<string>) new string[5]
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}^100", (object) WorkItemTermExpressionQueryBuilder.s_idPlatformFieldName)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^10", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^10", (object) WorkItemTermExpressionQueryBuilder.s_titlePlatformFieldName, (object) "pattern")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^5", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "stemmed")),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^5", (object) WorkItemTermExpressionQueryBuilder.s_descriptionPlatformFieldName, (object) "pattern"))
      });
      if (this.m_queryingOnIdentitiesEnabled)
        this.AddIdentityFieldsToQueryOn(fieldsToQuery, 4, true);
      else
        fieldsToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^4", (object) WorkItemTermExpressionQueryBuilder.s_assignedToPlatformFieldName)));
      return fieldsToQuery;
    }

    private List<WorkItemIndexedField> GetHighlightableFields(IVssRequestContext requestContext)
    {
      List<WorkItemIndexedField> highlightableFields = new List<WorkItemIndexedField>();
      IList<string> stringList1;
      if (!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/WorkItemHighlightQueryOverride", TeamFoundationHostType.Deployment))
        stringList1 = WorkItemHighlightBuilder.GetWorkItemFieldsToHighlight(requestContext);
      else
        stringList1 = (IList<string>) ((IEnumerable<string>) "system.id;system.history".Split(';')).Select<string, string>((Func<string, string>) (p => p.Trim())).ToList<string>();
      IList<string> stringList2 = stringList1;
      if (stringList2.Count > 0)
      {
        IWorkItemFieldCacheService service = requestContext.GetService<IWorkItemFieldCacheService>();
        foreach (string fieldRefName in (IEnumerable<string>) stringList2)
        {
          WorkItemField fieldData;
          if (service.TryGetFieldByRefName(requestContext, fieldRefName, out fieldData))
          {
            if (fieldData.Type == Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Integer)
              highlightableFields.Add(WorkItemIndexedField.FromWitField(fieldData.ReferenceName, WorkItemContract.FieldType.IntegerAsString));
            else if (this.m_queryingOnIdentitiesEnabled && fieldData.IsIdentity)
              highlightableFields.Add(WorkItemIndexedField.FromWitField(fieldData.ReferenceName, this.m_identityFieldType));
            else
              highlightableFields.Add(WorkItemIndexedField.FromWitField(fieldData));
          }
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082215, "Search Engine", "Query Builder", FormattableString.Invariant(FormattableStringFactory.Create("Field [{0}] not found in field cache. It may not be searchable/highlightable. User query won't fail though.", (object) fieldRefName)));
        }
      }
      return highlightableFields;
    }

    private static double TrimDoubleToIntegerRange(string doubleStringValue)
    {
      double integerRange = Convert.ToDouble(doubleStringValue, (IFormatProvider) CultureInfo.InvariantCulture);
      if (integerRange <= (double) int.MinValue)
        integerRange = (double) int.MinValue;
      else if (integerRange >= (double) int.MaxValue)
        integerRange = (double) int.MaxValue;
      return integerRange;
    }

    private void AddIdentityFieldsToQueryOn(
      List<string> fieldsToQuery,
      int boostFactor,
      bool addHighlightableIdentityFields = false)
    {
      if (this.m_identityFieldType == WorkItemContract.FieldType.Name)
      {
        fieldsToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositeNameFieldName)));
        fieldsToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^{1}", (object) WorkItemTermExpressionQueryBuilder.s_assignedToNamePlatformFieldName, (object) boostFactor)));
      }
      else
      {
        fieldsToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) WorkItemTermExpressionQueryBuilder.s_compositeIdentityFieldName)));
        fieldsToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^{1}", (object) WorkItemTermExpressionQueryBuilder.s_assignedToIdentityPlatformFieldName, (object) boostFactor)));
      }
      if (!addHighlightableIdentityFields)
        return;
      fieldsToQuery.AddRange(this.m_highlightableFields.Where<WorkItemIndexedField>((Func<WorkItemIndexedField, bool>) (f => f.Type == this.m_identityFieldType)).Select<WorkItemIndexedField, string>((Func<WorkItemIndexedField, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) f.PlatformFieldName)))));
    }
  }
}
