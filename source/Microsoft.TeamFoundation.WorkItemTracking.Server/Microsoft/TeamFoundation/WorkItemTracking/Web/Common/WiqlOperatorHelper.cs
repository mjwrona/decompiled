// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.WiqlOperatorHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public class WiqlOperatorHelper
  {
    private static readonly WiqlOperators s_wiqlOperator = new WiqlOperators();
    private static readonly IDictionary<string, string> s_supportedInvariantOperations;
    private static readonly IDictionary<string, string> s_supportedLocalizedOperations = WiqlOperatorHelper.s_wiqlOperator.GetSupportedOperationLookup(false);
    private static IDictionary<QueryExpressionOperator, string> s_supportedExpressionOperatorLookup;

    static WiqlOperatorHelper()
    {
      WiqlOperatorHelper.s_supportedInvariantOperations = WiqlOperatorHelper.s_wiqlOperator.GetSupportedOperationLookup(true);
      WiqlOperatorHelper.AddSupportedExpressionOperatorPair();
    }

    public static IEnumerable<string> GetAvailableOperators(
      IVssRequestContext requestContext,
      InternalFieldType fieldType,
      FieldEntry fieldEntry)
    {
      bool fullTextEnabled = requestContext.WitContext().ServerSettings.FullTextEnabled;
      List<string> invariantOperatorList = new List<string>();
      switch (fieldType)
      {
        case (InternalFieldType) 0:
          invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.ProjectOperators);
          break;
        case InternalFieldType.String:
          invariantOperatorList.AddRange(((fieldEntry == null ? 0 : (fieldEntry.SupportsTextQuery ? 1 : 0)) & (fullTextEnabled ? 1 : 0)) != 0 ? (IEnumerable<string>) WiqlOperators.StringWithTextSupportOperators : (IEnumerable<string>) WiqlOperators.StringOperators);
          break;
        case InternalFieldType.Integer:
        case InternalFieldType.DateTime:
        case InternalFieldType.Double:
          invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.ComparisonOperators);
          break;
        case InternalFieldType.PlainText:
        case InternalFieldType.Html:
        case InternalFieldType.History:
          invariantOperatorList.AddRange(((fieldEntry == null ? 0 : (fieldEntry.SupportsTextQuery ? 1 : 0)) & (fullTextEnabled ? 1 : 0)) != 0 ? (IEnumerable<string>) WiqlOperators.TextWithTextSupportOperators : (IEnumerable<string>) WiqlOperators.TextOperators);
          break;
        case InternalFieldType.TreePath:
          invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.TreePathOperators);
          break;
        case InternalFieldType.Guid:
        case InternalFieldType.Boolean:
          invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.EqualityOperators);
          break;
        case InternalFieldType.Identity:
          invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.IdentityOperators);
          break;
      }
      if (((IEnumerable<InternalFieldType>) WiqlOperators.FieldTypesSupportingInGroup).Contains<InternalFieldType>(fieldType))
        invariantOperatorList.AddRange((IEnumerable<string>) WiqlOperators.GroupOperators);
      if ((fieldEntry == null ? 0 : (fieldEntry.IsComputed ? 1 : 0)) == 0 && ((IEnumerable<InternalFieldType>) WiqlOperators.FieldTypesSupportingEver).Contains<InternalFieldType>(fieldType))
        invariantOperatorList.Add("EVER");
      List<string> localizedOperatorList = WiqlOperatorHelper.s_wiqlOperator.GetLocalizedOperatorList((IEnumerable<string>) invariantOperatorList);
      if (((IEnumerable<InternalFieldType>) WiqlOperators.FieldTypesSupportingFieldComparison).Contains<InternalFieldType>(fieldType))
      {
        foreach (string str in invariantOperatorList)
        {
          if (((IEnumerable<string>) WiqlOperators.OperatorsSupportingFieldComparison).Contains<string>(str))
          {
            string comparisonOperator = WiqlOperatorHelper.s_wiqlOperator.GetFieldComparisonOperator(str);
            localizedOperatorList.Add(comparisonOperator);
            string localizedOperation = WiqlOperatorHelper.s_supportedLocalizedOperations[str];
            WiqlOperatorHelper.AddOperatorPair(WiqlOperatorHelper.s_supportedLocalizedOperations, comparisonOperator, WiqlOperators.AppendFieldComparisonReferenceName(localizedOperation));
            WiqlOperatorHelper.AddOperatorPair(WiqlOperatorHelper.s_supportedInvariantOperations, WiqlOperators.AppendFieldComparisonReferenceName(localizedOperation), comparisonOperator);
          }
        }
      }
      return (IEnumerable<string>) localizedOperatorList;
    }

    public static string GetSupportedOperationReferenceName(string localizedOperator)
    {
      string str = (string) null;
      string invariantOperator = WiqlOperatorHelper.s_wiqlOperator.GetInvariantOperator(localizedOperator);
      return WiqlOperatorHelper.s_supportedLocalizedOperations.ContainsKey(invariantOperator) ? WiqlOperatorHelper.s_supportedLocalizedOperations[invariantOperator] : str;
    }

    public static string GetSupportedOperationReferenceName(
      QueryExpressionOperator expressionOperator,
      bool expandConst)
    {
      string operationReferenceName = (string) null;
      if (WiqlOperatorHelper.s_supportedExpressionOperatorLookup.ContainsKey(expressionOperator) && !expandConst)
        return WiqlOperatorHelper.s_supportedExpressionOperatorLookup[expressionOperator];
      if (!(WiqlOperatorHelper.s_supportedExpressionOperatorLookup.ContainsKey(expressionOperator) & expandConst))
        return operationReferenceName;
      return expressionOperator == QueryExpressionOperator.Equals ? "SupportedOperations.InGroup" : "SupportedOperations.NotInGroup";
    }

    public static string GetSupportedOperationName(
      QueryExpressionOperator expressionOperator,
      bool expandConst)
    {
      string supportedOperationName = (string) null;
      if (WiqlOperatorHelper.s_supportedExpressionOperatorLookup.ContainsKey(expressionOperator) && !expandConst)
        return WiqlOperatorHelper.s_supportedInvariantOperations[WiqlOperatorHelper.s_supportedExpressionOperatorLookup[expressionOperator]];
      if (!(WiqlOperatorHelper.s_supportedExpressionOperatorLookup.ContainsKey(expressionOperator) & expandConst))
        return supportedOperationName;
      return expressionOperator == QueryExpressionOperator.Equals ? WiqlOperatorHelper.s_supportedInvariantOperations["SupportedOperations.InGroup"] : WiqlOperatorHelper.s_supportedInvariantOperations["SupportedOperations.NotInGroup"];
    }

    private static bool AddOperatorPair(IDictionary<string, string> hash, string key, string value)
    {
      if (hash.ContainsKey(key))
        return false;
      hash.Add(key, value);
      return true;
    }

    private static void AddSupportedExpressionOperatorPair() => WiqlOperatorHelper.s_supportedExpressionOperatorLookup = (IDictionary<QueryExpressionOperator, string>) new Dictionary<QueryExpressionOperator, string>()
    {
      {
        QueryExpressionOperator.Contains,
        "SupportedOperations.Contains"
      },
      {
        QueryExpressionOperator.ContainsWords,
        "SupportedOperations.ContainsWords"
      },
      {
        QueryExpressionOperator.NotContains,
        "SupportedOperations.NotContains"
      },
      {
        QueryExpressionOperator.NotContainsWords,
        "SupportedOperations.NotContainsWords"
      },
      {
        QueryExpressionOperator.In,
        "SupportedOperations.In"
      },
      {
        QueryExpressionOperator.Ever,
        "SupportedOperations.Ever"
      },
      {
        QueryExpressionOperator.Under,
        "SupportedOperations.Under"
      },
      {
        QueryExpressionOperator.NotUnder,
        "SupportedOperations.NotUnder"
      },
      {
        QueryExpressionOperator.Equals,
        "SupportedOperations.Equals"
      },
      {
        QueryExpressionOperator.NotEquals,
        "SupportedOperations.NotEquals"
      },
      {
        QueryExpressionOperator.Greater,
        "SupportedOperations.GreaterThan"
      },
      {
        QueryExpressionOperator.Less,
        "SupportedOperations.LessThan"
      },
      {
        QueryExpressionOperator.GreaterEquals,
        "SupportedOperations.GreaterThanEquals"
      },
      {
        QueryExpressionOperator.LessEquals,
        "SupportedOperations.LessThanEquals"
      },
      {
        QueryExpressionOperator.EverContains,
        "SupportedOperations.Contains"
      },
      {
        QueryExpressionOperator.EverContainsWords,
        "SupportedOperations.ContainsWords"
      },
      {
        QueryExpressionOperator.NeverContains,
        "SupportedOperations.NotContains"
      },
      {
        QueryExpressionOperator.NeverContainsWords,
        "SupportedOperations.NotContainsWords"
      }
    };
  }
}
