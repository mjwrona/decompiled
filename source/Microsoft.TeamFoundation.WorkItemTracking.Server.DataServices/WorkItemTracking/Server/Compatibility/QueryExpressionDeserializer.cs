// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.QueryExpressionDeserializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  public static class QueryExpressionDeserializer
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression Deserialize(
      IVssRequestContext requestContext,
      XmlElement psQueryXml,
      IEnumerable<QuerySortOrderEntry> sortFields = null,
      DateTime? asOfDateTime = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<XmlElement>(psQueryXml, nameof (psQueryXml));
      XElement element1 = XElement.Parse(psQueryXml.OuterXml, LoadOptions.PreserveWhitespace);
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = new Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression();
      queryExpression.Wiql = psQueryXml.OuterXml;
      if (element1.Attribute<string>((XName) "Ever") != null)
        throw new NotImplementedException();
      if (!asOfDateTime.HasValue)
      {
        string s = element1.Attribute<string>((XName) "AsOf");
        if (!string.IsNullOrWhiteSpace(s))
        {
          DateTime result;
          if (!DateTime.TryParse(s, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
            throw new ArgumentException(DalResourceStrings.Get("QueryInvalidAsOfDate"), "queryXml");
          asOfDateTime = new DateTime?(result);
        }
      }
      queryExpression.AsOfDateTime = asOfDateTime;
      IFieldTypeDictionary fieldsSnapshot = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
      if (element1.Name == (XName) "LinksQuery")
      {
        string s = element1.Attribute<string>((XName) "RecursionID");
        bool flag = !string.IsNullOrWhiteSpace(s);
        if (flag)
        {
          short result = 0;
          if (!short.TryParse(s, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            throw new ArgumentException(DalResourceStrings.Format("InvalidLinkQueryRecursionID", (object) s), "queryXml");
          queryExpression.RecursionLinkTypeId = result;
          string str = element1.Attribute<string>((XName) "ReturnMatchingParents");
          queryExpression.RecursionOption = string.IsNullOrWhiteSpace(str) || QueryExpressionDeserializer.ConvertToBoolean(str) ? QueryRecursionOption.ParentFirst : QueryRecursionOption.ChildFirst;
        }
        QueryType queryType = QueryType.WorkItems;
        string str1 = element1.Attribute<string>((XName) "Type");
        if (!string.IsNullOrWhiteSpace(str1))
        {
          str1 = str1.Trim();
          if (str1.Equals("mustcontain", StringComparison.OrdinalIgnoreCase))
            queryType = flag ? QueryType.LinksRecursiveMustContain : QueryType.LinksOneHopMustContain;
          else if (str1.Equals("maycontain", StringComparison.OrdinalIgnoreCase))
            queryType = flag ? QueryType.LinksRecursiveMayContain : QueryType.LinksOneHopMayContain;
          else if (str1.Equals("doesnotcontain", StringComparison.OrdinalIgnoreCase))
            queryType = flag ? QueryType.LinksRecursiveDoesNotContain : QueryType.LinksOneHopDoesNotContain;
        }
        queryExpression.QueryType = queryType != QueryType.WorkItems ? queryType : throw new ArgumentException(DalResourceStrings.Format("InvalidLinkQueryInvalidType", (object) str1), "queryXml");
        foreach (XElement element2 in element1.Elements())
        {
          if (element2.Elements().Count<XElement>() != 1)
            throw new ArgumentException(DalResourceStrings.Format("InvalidLinkSubQuery", (object) element1.Name.LocalName), "queryXml");
          if (element2.Name == (XName) "LeftQuery")
            queryExpression.LeftGroup = QueryExpressionDeserializer.ParseQueryExpression(fieldsSnapshot, element2.Elements().First<XElement>());
          else if (element2.Name == (XName) "LinkQuery")
            queryExpression.LinkGroup = QueryExpressionDeserializer.ParseQueryExpression(fieldsSnapshot, element2.Elements().First<XElement>());
          else
            queryExpression.RightGroup = element2.Name == (XName) "RightQuery" ? QueryExpressionDeserializer.ParseQueryExpression(fieldsSnapshot, element2.Elements().First<XElement>()) : throw new ArgumentException(DalResourceStrings.Format("InvalidLinkSubQuery", (object) element2.Name.LocalName), "queryXml");
        }
      }
      else
      {
        queryExpression.QueryType = QueryType.WorkItems;
        if (element1.Elements().Count<XElement>() != 1 || element1.Elements().Any<XElement>((Func<XElement, bool>) (x => x.Name != (XName) "Expression" && x.Name != (XName) "Group")))
          throw new ArgumentException(DalResourceStrings.Get("QueryMissingGroupsOrExpressionsException"), "queryXml");
        queryExpression.LeftGroup = QueryExpressionDeserializer.ParseQueryExpression(fieldsSnapshot, element1.Elements().First<XElement>());
      }
      queryExpression.SortFields = (IEnumerable<QuerySortField>) QueryExpressionDeserializer.ConvertSortFields(fieldsSnapshot, queryExpression.QueryType, sortFields ?? Enumerable.Empty<QuerySortOrderEntry>()).ToArray<QuerySortField>();
      return queryExpression;
    }

    internal static IEnumerable<QuerySortField> ConvertSortFields(
      IFieldTypeDictionary fieldTypes,
      QueryType queryType,
      IEnumerable<QuerySortOrderEntry> sortFields)
    {
      foreach (QuerySortOrderEntry sortField in sortFields)
      {
        QuerySortField querySortField = QueryExpressionDeserializer.ConvertToQuerySortField(fieldTypes, sortField);
        if (queryType != QueryType.LinksOneHopDoesNotContain || querySortField.TableAlias == QueryTableAlias.Left)
          yield return querySortField;
      }
    }

    private static QuerySortField ConvertToQuerySortField(
      IFieldTypeDictionary fieldTypes,
      QuerySortOrderEntry sortField)
    {
      string[] strArray = !string.IsNullOrWhiteSpace(sortField.ColumnName) ? sortField.ColumnName.Trim().Split(new string[1]
      {
        "].["
      }, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentException(DalResourceStrings.Get("QueryIDsInvalidColumnName"), "sortFields");
      string x = (string) null;
      string name;
      if (strArray.Length == 1)
      {
        name = strArray[0];
      }
      else
      {
        x = strArray.Length == 2 && strArray[0].StartsWith("[", StringComparison.OrdinalIgnoreCase) && strArray[1].EndsWith("]", StringComparison.OrdinalIgnoreCase) ? strArray[0].Substring(1, strArray[0].Length - 1) : throw new ArgumentException(DalResourceStrings.Format("InvalidLinkQuerySortOrder", (object) sortField.ColumnName), "sortFields");
        name = strArray[1].Substring(0, strArray[1].Length - 1);
      }
      QuerySortField querySortField = new QuerySortField();
      querySortField.TableAlias = QueryTableAlias.Left;
      querySortField.Descending = !sortField.Ascending;
      if (x != null)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(x, "links") || StringComparer.OrdinalIgnoreCase.Equals(x, "linktypes"))
          querySortField.TableAlias = QueryTableAlias.Link;
        else if (StringComparer.OrdinalIgnoreCase.Equals(x, "rhs"))
          querySortField.TableAlias = QueryTableAlias.Right;
        else if (!StringComparer.OrdinalIgnoreCase.Equals(x, "lhs"))
          throw new ArgumentException(DalResourceStrings.Format("InvalidLinkQuerySortOrder", (object) sortField.ColumnName), "sortFields");
      }
      FieldEntry field;
      if (!fieldTypes.TryGetField(name, out field))
        throw new ArgumentException(DalResourceStrings.Get("QueryIDsInvalidColumnName"), "sortFields");
      if (field.FieldId == 100)
        querySortField.TableAlias = QueryTableAlias.Link;
      querySortField.Field = field;
      return querySortField;
    }

    private static QueryExpressionNode ParseQueryExpression(
      IFieldTypeDictionary fieldTypes,
      XElement xmlExpression)
    {
      if (xmlExpression.Name == (XName) "Expression")
      {
        string str = xmlExpression.Attribute<string>((XName) "Column");
        string name = !string.IsNullOrWhiteSpace(str) ? str.Trim() : throw new ArgumentException(DalResourceStrings.Get("QueryMissingColumnNameForExpressionException"), "queryXml");
        FieldEntry field;
        if (!fieldTypes.TryGetField(name, out field))
          throw new ArgumentException(DalResourceStrings.Get("QueryIDsInvalidColumnName"), "queryXml");
        QueryExpressionOperator queryOperator = QueryExpressionDeserializer.ParseQueryOperator(xmlExpression);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        bool flag = xmlExpression.Attribute<bool>((XName) "ExpandConstant", QueryExpressionDeserializer.\u003C\u003EO.\u003C0\u003E__ConvertToBoolean ?? (QueryExpressionDeserializer.\u003C\u003EO.\u003C0\u003E__ConvertToBoolean = new Func<string, bool>(QueryExpressionDeserializer.ConvertToBoolean)));
        QueryExpressionValue queryExpressionValue = QueryExpressionDeserializer.ParseQueryExpressionValue(fieldTypes, xmlExpression);
        return (QueryExpressionNode) new QueryComparisonExpressionNode()
        {
          Field = field,
          Operator = queryOperator,
          Value = queryExpressionValue,
          ExpandConstant = flag
        };
      }
      if (!(xmlExpression.Name == (XName) "Group"))
        return (QueryExpressionNode) null;
      QueryExpressionNode[] array = xmlExpression.Elements().Select<XElement, QueryExpressionNode>((Func<XElement, QueryExpressionNode>) (e => QueryExpressionDeserializer.ParseQueryExpression(fieldTypes, e))).Where<QueryExpressionNode>((Func<QueryExpressionNode, bool>) (n => n != null)).ToArray<QueryExpressionNode>();
      QueryLogicalExpressionOperator result = QueryLogicalExpressionOperator.And;
      string str1 = xmlExpression.Attribute<string>((XName) "GroupOperator");
      if (string.IsNullOrWhiteSpace(str1) || !Enum.TryParse<QueryLogicalExpressionOperator>(str1.Trim(), true, out result) || array.Length == 0)
        throw new ArgumentException(DalResourceStrings.Get("QueryMissingGroupsOrExpressionsException"), "queryXml");
      return (QueryExpressionNode) new QueryLogicalExpressionNode()
      {
        Operator = result,
        Children = (IEnumerable<QueryExpressionNode>) array
      };
    }

    private static QueryExpressionValue ParseQueryExpressionValue(
      IFieldTypeDictionary fieldTypes,
      XElement element)
    {
      if (element.Elements().Count<XElement>() != 1)
        throw new ArgumentException(DalResourceStrings.Get("QueryInvalidNumberOfValuesInExpressionException"), "queryXml");
      QueryExpressionValue queryExpressionValue = new QueryExpressionValue();
      string s1;
      if ((s1 = element.Element<string>((XName) "Number")) != null)
      {
        queryExpressionValue.ValueType = QueryExpressionValueType.Number;
        if (!string.IsNullOrWhiteSpace(s1))
        {
          int result;
          if (!int.TryParse(s1, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            throw new ArgumentException(DalResourceStrings.Get("QueryInvalidNumberValueException"), "queryXml");
          queryExpressionValue.NumberValue = result;
          queryExpressionValue.IsNull = false;
        }
      }
      else
      {
        string s2;
        if ((s2 = element.Element<string>((XName) "Double")) != null)
        {
          queryExpressionValue.ValueType = QueryExpressionValueType.Double;
          if (!string.IsNullOrWhiteSpace(s2))
          {
            double result;
            if (!double.TryParse(s2, NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
              throw new ArgumentException(DalResourceStrings.Get("QueryInvalidDoubleValueException"), "queryXml");
            queryExpressionValue.DoubleValue = result;
            queryExpressionValue.IsNull = false;
          }
        }
        else
        {
          string s3;
          if ((s3 = element.Element<string>((XName) "DateTime")) != null)
          {
            queryExpressionValue.ValueType = QueryExpressionValueType.DateTime;
            if (!string.IsNullOrWhiteSpace(s3))
            {
              DateTime result;
              if (!DateTime.TryParse(s3, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
                throw new ArgumentException(DalResourceStrings.Get("QueryInvalidDateValueException"), "queryXml");
              queryExpressionValue.DateValue = result;
              queryExpressionValue.IsNull = false;
            }
          }
          else
          {
            string str1;
            if ((str1 = element.Element<string>((XName) "Boolean")) != null)
            {
              queryExpressionValue.ValueType = QueryExpressionValueType.Boolean;
              if (!string.IsNullOrWhiteSpace(str1))
              {
                queryExpressionValue.BoolValue = QueryExpressionDeserializer.ConvertToBoolean(str1.Trim());
                queryExpressionValue.IsNull = false;
              }
            }
            else
            {
              string input;
              if ((input = element.Element<string>((XName) "Guid")) != null)
              {
                queryExpressionValue.ValueType = QueryExpressionValueType.UniqueIdentifier;
                if (!string.IsNullOrWhiteSpace(input))
                {
                  Guid result;
                  if (!Guid.TryParse(input, out result))
                    throw new ArgumentException(DalResourceStrings.Get("QueryInvalidGuidValueException"), "queryXml");
                  queryExpressionValue.GuidValue = result;
                  queryExpressionValue.IsNull = false;
                }
              }
              else
              {
                string str2;
                if ((str2 = element.Element<string>((XName) "Column")) != null)
                {
                  queryExpressionValue.ValueType = QueryExpressionValueType.Column;
                  string name = str2.TrimStart(' ', '[').TrimEnd(' ', ']');
                  FieldEntry field;
                  if (!fieldTypes.TryGetField(name, out field))
                    throw new ArgumentException(DalResourceStrings.Get("QueryIDsInvalidColumnName"), "queryXml");
                  queryExpressionValue.ColumnValue = field;
                  queryExpressionValue.IsNull = false;
                }
                else
                {
                  string str3;
                  if ((str3 = element.Element<string>((XName) "String")) == null)
                    throw new ArgumentException(DalResourceStrings.Get("QueryInvalidNumberOfValuesInExpressionException"), "queryXml");
                  queryExpressionValue.ValueType = QueryExpressionValueType.String;
                  queryExpressionValue.StringValue = str3;
                  queryExpressionValue.IsNull = string.IsNullOrEmpty(str3);
                }
              }
            }
          }
        }
      }
      return queryExpressionValue;
    }

    private static QueryExpressionOperator ParseQueryOperator(XElement xmlExpression)
    {
      string str = xmlExpression.Attribute<string>((XName) "Operator");
      string x = !string.IsNullOrWhiteSpace(str) ? str.Trim() : throw new ArgumentException(DalResourceStrings.Get("QueryInvalidExpressionOperatorException"), "queryXml");
      if (TFStringComparer.QueryOperator.Equals(x, "equals"))
        return QueryExpressionOperator.Equals;
      if (TFStringComparer.QueryOperator.Equals(x, "notequals"))
        return QueryExpressionOperator.NotEquals;
      if (TFStringComparer.QueryOperator.Equals(x, "under"))
        return QueryExpressionOperator.Under;
      if (TFStringComparer.QueryOperator.Equals(x, "notunder"))
        return QueryExpressionOperator.NotUnder;
      if (TFStringComparer.QueryOperator.Equals(x, "less"))
        return QueryExpressionOperator.Less;
      if (TFStringComparer.QueryOperator.Equals(x, "equalsless"))
        return QueryExpressionOperator.LessEquals;
      if (TFStringComparer.QueryOperator.Equals(x, "greater"))
        return QueryExpressionOperator.Greater;
      if (TFStringComparer.QueryOperator.Equals(x, "equalsgreater"))
        return QueryExpressionOperator.GreaterEquals;
      if (TFStringComparer.QueryOperator.Equals(x, "ever"))
        return QueryExpressionOperator.Ever;
      if (TFStringComparer.QueryOperator.Equals(x, "contains"))
        return QueryExpressionOperator.Contains;
      if (TFStringComparer.QueryOperator.Equals(x, "notcontains"))
        return QueryExpressionOperator.NotContains;
      if (TFStringComparer.QueryOperator.Equals(x, "containswords"))
        return QueryExpressionOperator.ContainsWords;
      if (TFStringComparer.QueryOperator.Equals(x, "notcontainswords"))
        return QueryExpressionOperator.NotContainsWords;
      if (TFStringComparer.QueryOperator.Equals(x, "evercontains"))
        return QueryExpressionOperator.EverContains;
      if (TFStringComparer.QueryOperator.Equals(x, "nevercontains"))
        return QueryExpressionOperator.NeverContains;
      if (TFStringComparer.QueryOperator.Equals(x, "evercontainswords"))
        return QueryExpressionOperator.EverContainsWords;
      if (TFStringComparer.QueryOperator.Equals(x, "nevercontainswords"))
        return QueryExpressionOperator.NeverContainsWords;
      if (TFStringComparer.QueryOperator.Equals(x, "ftcontains"))
        return QueryExpressionOperator.FTContains;
      if (TFStringComparer.QueryOperator.Equals(x, "notftcontains"))
        return QueryExpressionOperator.NotFTContains;
      if (TFStringComparer.QueryOperator.Equals(x, "everftcontains"))
        return QueryExpressionOperator.EverFTContains;
      if (TFStringComparer.QueryOperator.Equals(x, "neverftcontains"))
        return QueryExpressionOperator.NeverFTContains;
    }

    private static bool ConvertToBoolean(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return false;
      if (StringComparer.Ordinal.Equals(value, "1"))
        return true;
      if (StringComparer.Ordinal.Equals(value, "0") || StringComparer.Ordinal.Equals(value, "-1"))
        return false;
      bool result;
      if (bool.TryParse(value, out result))
        return result;
      throw new ArgumentException(DalResourceStrings.Get("QueryInvalidBooleanValueException"), "queryXml");
    }
  }
}
