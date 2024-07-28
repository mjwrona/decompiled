// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.WiqlOperators
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WiqlOperators
  {
    private IDictionary<string, string> m_localizedOperatorLookup;
    private IDictionary<string, string> m_invariantOperatorLookup;
    private IDictionary<string, string> m_supportedInvariantOperationsLookup;
    private IDictionary<string, string> m_supportedLocalizedOperationsLookup;
    public const string cAnd = "AND";
    public const string cOr = "OR";
    public const string cNot_ = "NOT ";
    public const string cContains = "CONTAINS";
    public const string cContainsWords = "CONTAINS WORDS";
    public const string cNotContains = "NOT CONTAINS";
    public const string cNotContainsWords = "NOT CONTAINS WORDS";
    public const string cUnder = "UNDER";
    public const string cNotUnder = "NOT UNDER";
    public const string cIn = "IN";
    public const string cNotIn = "NOT IN";
    public const string cEver = "EVER";
    public const string cNotEver = "NOT EVER";
    public const string cEqualTo = "=";
    public const string cNotEqualTo = "<>";
    public const string cGreaterThan = ">";
    public const string cLessThan = "<";
    public const string cGreaterThanOrEqualTo = ">=";
    public const string cLessThanOrEqualTo = "<=";
    public const string cInGroup = "IN GROUP";
    public const string cNotInGroup = "NOT IN GROUP";
    public const string cIsEmpty = "IS EMPTY";
    public const string cIsNotEmpty = "IS NOT EMPTY";
    public const string cMacroStart = "@";
    public const string cMacroToday = "@today";
    public const string cMacroMe = "@me";
    public const string cProjectContextKey = "project";
    public const string cTeamNameContextKey = "team";
    public const string cMacroProject = "@project";
    public const string cCurrentIterationContextKey = "currentIteration";
    public const string cMacroCurrentIteration = "@currentIteration";
    public const string cMacroFollows = "@follows";
    public const string cRecentMentions = "@recentMentions";
    public const string cMyRecentActivity = "@myRecentActivity";
    public const string cRecentProjectActivity = "@recentProjectActivity";
    public const string cStartOfYearName = "startOfYear";
    public const string cStartOfMonthName = "startOfMonth";
    public const string cStartOfWeekName = "startOfWeek";
    public const string cStartOfDayName = "startOfDay";
    public const string cStartOfYear = "@startOfYear";
    public const string cStartOfMonth = "@startOfMonth";
    public const string cStartOfWeek = "@startOfWeek";
    public const string cStartOfDay = "@startOfDay";
    public const string refNameContains = "SupportedOperations.Contains";
    public const string refNameContainsWords = "SupportedOperations.ContainsWords";
    public const string refNameNotContains = "SupportedOperations.NotContains";
    public const string refNameNotContainsWords = "SupportedOperations.NotContainsWords";
    public const string refNameUnder = "SupportedOperations.Under";
    public const string refNameNotUnder = "SupportedOperations.NotUnder";
    public const string refNameIn = "SupportedOperations.In";
    public const string refNameEver = "SupportedOperations.Ever";
    public const string refNameNotEver = "SupportedOperations.NotEver";
    public const string refNameEqualTo = "SupportedOperations.Equals";
    public const string refNameNotEqualTo = "SupportedOperations.NotEquals";
    public const string refNameGreaterThan = "SupportedOperations.GreaterThan";
    public const string refNameLessThan = "SupportedOperations.LessThan";
    public const string refNameGreaterThanOrEqualTo = "SupportedOperations.GreaterThanEquals";
    public const string refNameLessThanOrEqualTo = "SupportedOperations.LessThanEquals";
    public const string refNameInGroup = "SupportedOperations.InGroup";
    public const string refNameNotInGroup = "SupportedOperations.NotInGroup";
    public static readonly string[] ProjectOperators = new string[4]
    {
      "=",
      "<>",
      "IN",
      "NOT IN"
    };
    public static readonly string[] StringOperators = new string[10]
    {
      "=",
      "<>",
      ">",
      "<",
      ">=",
      "<=",
      "CONTAINS",
      "NOT CONTAINS",
      "IN",
      "NOT IN"
    };
    public static readonly string[] StringWithTextSupportOperators = new string[12]
    {
      "=",
      "<>",
      ">",
      "<",
      ">=",
      "<=",
      "CONTAINS",
      "NOT CONTAINS",
      "CONTAINS WORDS",
      "NOT CONTAINS WORDS",
      "IN",
      "NOT IN"
    };
    public static readonly string[] TextOperators = new string[2]
    {
      "CONTAINS",
      "NOT CONTAINS"
    };
    public static readonly string[] TextWithTextSupportOperators = new string[2]
    {
      "CONTAINS WORDS",
      "NOT CONTAINS WORDS"
    };
    public static readonly string[] ContainsOperators = new string[2]
    {
      "CONTAINS",
      "NOT CONTAINS"
    };
    public static readonly string[] ContainsWordsOperators = new string[2]
    {
      "CONTAINS WORDS",
      "NOT CONTAINS WORDS"
    };
    public static readonly string[] TreePathOperators = new string[6]
    {
      "UNDER",
      "NOT UNDER",
      "=",
      "<>",
      "IN",
      "NOT IN"
    };
    public static readonly string[] ComparisonOperators = new string[8]
    {
      "=",
      "<>",
      ">",
      "<",
      ">=",
      "<=",
      "IN",
      "NOT IN"
    };
    public static readonly string[] EqualityOperators = new string[2]
    {
      "=",
      "<>"
    };
    public static readonly string[] LogicalOperators = new string[2]
    {
      "AND",
      "OR"
    };
    public static readonly string[] GroupOperators = new string[2]
    {
      "IN GROUP",
      "NOT IN GROUP"
    };
    public static readonly string[] OperatorsSupportingFieldComparison = new string[6]
    {
      "=",
      "<>",
      ">",
      "<",
      ">=",
      "<="
    };
    public static readonly string[] IdentityOperators = new string[6]
    {
      "=",
      "<>",
      "IN",
      "NOT IN",
      "IN GROUP",
      "NOT IN GROUP"
    };
    public static readonly InternalFieldType[] FieldTypesSupportingInGroup = new InternalFieldType[1]
    {
      InternalFieldType.String
    };
    public static readonly InternalFieldType[] FieldTypesSupportingEver = new InternalFieldType[5]
    {
      InternalFieldType.String,
      InternalFieldType.Integer,
      InternalFieldType.Double,
      InternalFieldType.Boolean,
      InternalFieldType.Guid
    };
    public static readonly InternalFieldType[] FieldTypesSupportingFieldComparison = new InternalFieldType[7]
    {
      InternalFieldType.String,
      InternalFieldType.DateTime,
      InternalFieldType.Integer,
      InternalFieldType.Double,
      InternalFieldType.Boolean,
      InternalFieldType.Guid,
      InternalFieldType.Identity
    };

    public WiqlOperators(WiqlOperatorsOptions options = null)
    {
      this.m_localizedOperatorLookup = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_invariantOperatorLookup = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      this.m_supportedInvariantOperationsLookup = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_supportedLocalizedOperationsLookup = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      string[] strArray1 = new string[68]
      {
        "AND",
        CommonClientResourceStrings.WiqlOperators_And,
        "OR",
        CommonClientResourceStrings.WiqlOperators_Or,
        "CONTAINS",
        CommonClientResourceStrings.WiqlOperators_Contains,
        "CONTAINS WORDS",
        CommonClientResourceStrings.WiqlOperators_ContainsWords,
        "NOT CONTAINS",
        CommonClientResourceStrings.WiqlOperators_NotContains,
        "NOT CONTAINS WORDS",
        CommonClientResourceStrings.WiqlOperators_NotContainsWords,
        "IN",
        CommonClientResourceStrings.WiqlOperators_In,
        "NOT IN",
        CommonClientResourceStrings.WiqlOperators_NotIn,
        "EVER",
        CommonClientResourceStrings.WiqlOperators_Ever,
        "NOT EVER",
        CommonClientResourceStrings.WiqlOperators_NotEver,
        "UNDER",
        CommonClientResourceStrings.WiqlOperators_Under,
        "NOT UNDER",
        CommonClientResourceStrings.WiqlOperators_NotUnder,
        "=",
        CommonClientResourceStrings.WiqlOperators_EqualTo,
        "<>",
        CommonClientResourceStrings.WiqlOperators_NotEqualTo,
        ">",
        CommonClientResourceStrings.WiqlOperators_GreaterThan,
        "<",
        CommonClientResourceStrings.WiqlOperators_LessThan,
        ">=",
        CommonClientResourceStrings.WiqlOperators_GreaterThanOrEqualTo,
        "<=",
        CommonClientResourceStrings.WiqlOperators_LessThanOrEqualTo,
        "IN GROUP",
        CommonClientResourceStrings.WiqlOperators_InGroup,
        "NOT IN GROUP",
        CommonClientResourceStrings.WiqlOperators_NotInGroup,
        "IS EMPTY",
        CommonClientResourceStrings.WiqlOperators_IsEmpty,
        "IS NOT EMPTY",
        CommonClientResourceStrings.WiqlOperators_IsNotEmpty,
        "@today",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroToday),
        "@me",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroMe),
        "@project",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroProject),
        "@currentIteration",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroCurrentIteration),
        "@follows",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroFollows),
        "@recentMentions",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroRecentMentions),
        "@myRecentActivity",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroMyRecentActivity),
        "@recentProjectActivity",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroRecentProjectActivity),
        "@startOfYear",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroStartOfYear),
        "@startOfMonth",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroStartOfMonth),
        "@startOfWeek",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroStartOfWeek),
        "@startOfDay",
        WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroStartOfDay)
      };
      string[] strArray2 = new string[34]
      {
        "CONTAINS",
        "SupportedOperations.Contains",
        "CONTAINS WORDS",
        "SupportedOperations.ContainsWords",
        "NOT CONTAINS",
        "SupportedOperations.NotContains",
        "NOT CONTAINS WORDS",
        "SupportedOperations.NotContainsWords",
        "IN",
        "SupportedOperations.In",
        "EVER",
        "SupportedOperations.Ever",
        "NOT EVER",
        "SupportedOperations.NotEver",
        "UNDER",
        "SupportedOperations.Under",
        "NOT UNDER",
        "SupportedOperations.NotUnder",
        "=",
        "SupportedOperations.Equals",
        "<>",
        "SupportedOperations.NotEquals",
        ">",
        "SupportedOperations.GreaterThan",
        "<",
        "SupportedOperations.LessThan",
        ">=",
        "SupportedOperations.GreaterThanEquals",
        "<=",
        "SupportedOperations.LessThanEquals",
        "IN GROUP",
        "SupportedOperations.InGroup",
        "NOT IN GROUP",
        "SupportedOperations.NotInGroup"
      };
      IEnumerable<string> filteredOperators = options?.FilteredOperators;
      for (int index = 0; index < strArray1.Length; index += 2)
      {
        if (filteredOperators == null || !filteredOperators.Contains<string>(strArray1[index], (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
          this.AddOperatorPair(strArray1[index], strArray1[index + 1]);
      }
      for (int index = 0; index < strArray2.Length; index += 2)
      {
        if (filteredOperators == null || !filteredOperators.Contains<string>(strArray2[index], (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
          this.AddSupportedOperatorPair(strArray2[index], strArray2[index + 1]);
      }
    }

    public IEnumerable<string> GetLocalizedMacros() => this.m_localizedOperatorLookup.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => p.Key.StartsWith("@", StringComparison.InvariantCulture))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (p => p.Value));

    public IEnumerable<string> GetInvariantMacros() => this.m_localizedOperatorLookup.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => p.Key.StartsWith("@", StringComparison.InvariantCulture))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (p => p.Key));

    public string GetInvariantOperator(string localizedOperator)
    {
      if (localizedOperator != null)
      {
        if (this.m_invariantOperatorLookup.ContainsKey(localizedOperator))
          return this.m_invariantOperatorLookup[localizedOperator];
        if (this.IsTodayMacro(localizedOperator, true))
          return WiqlOperators.GetInvariantTodayMacro(localizedOperator);
        if (this.IsComplexMacro(localizedOperator, true))
          return this.GetComplexMacro(localizedOperator, true);
      }
      return localizedOperator;
    }

    public string GetLocalizedOperator(string invariantOperator)
    {
      if (invariantOperator != null && this.m_localizedOperatorLookup.ContainsKey(invariantOperator))
        return this.m_localizedOperatorLookup[invariantOperator];
      if (this.IsTodayMacro(invariantOperator, true))
        return this.GetLocalizedTodayMacro(invariantOperator);
      return this.IsComplexMacro(invariantOperator, false) ? this.GetComplexMacro(invariantOperator, false) : invariantOperator;
    }

    public List<string> GetLocalizedOperatorList(IEnumerable<string> invariantOperatorList)
    {
      List<string> localizedOperatorList = new List<string>();
      foreach (string invariantOperator in invariantOperatorList)
        localizedOperatorList.Add(this.GetLocalizedOperator(invariantOperator));
      return localizedOperatorList;
    }

    public WiqlOperators.LogicalOperator GetLogicalOperator(string logicalOperatorText)
    {
      if (this.GetInvariantOperator(logicalOperatorText) == "AND")
        return WiqlOperators.LogicalOperator.And;
      return this.GetInvariantOperator(logicalOperatorText) == "OR" ? WiqlOperators.LogicalOperator.Or : WiqlOperators.LogicalOperator.None;
    }

    public string GetLocalizedTodayMinusMacro(int number) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} - {1}", (object) this.GetLocalizedOperator("@today"), (object) number);

    public bool IsValidMacro(string macro, bool localized)
    {
      if (!macro.StartsWith("@", StringComparison.OrdinalIgnoreCase))
        return false;
      if (this.IsTodayMacro(macro, localized))
        return true;
      bool flag = !localized ? this.m_localizedOperatorLookup.ContainsKey(macro) : this.m_invariantOperatorLookup.ContainsKey(macro);
      if (!flag)
        flag = this.IsComplexMacro(macro, localized);
      return flag;
    }

    public static bool IsFieldComparisonOperator(string operatorName) => operatorName.EndsWith(CommonClientResourceStrings.WiqlOperators_FieldComparisonQualifier, StringComparison.CurrentCulture);

    public string GetFieldComparisonOperator(string invariantOperator) => WiqlOperators.AppendFieldComparisonQualifier(this.GetLocalizedOperator(invariantOperator));

    public static string StripFieldComparisonQualifier(string operatorName)
    {
      if (WiqlOperators.IsFieldComparisonOperator(operatorName))
        operatorName = operatorName.Substring(0, operatorName.Length - CommonClientResourceStrings.WiqlOperators_FieldComparisonQualifier.Length);
      return operatorName;
    }

    public static string AppendFieldComparisonQualifier(string operatorName) => operatorName + CommonClientResourceStrings.WiqlOperators_FieldComparisonQualifier;

    public static string AppendFieldComparisonReferenceName(string operatorReferenceName) => operatorReferenceName + CommonClientResourceStrings.WiqlOperators_FieldComparisonReferenceName;

    public static string GetMacro(string stringFromResource) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "@{0}", (object) stringFromResource);

    private bool IsTodayMacro(string macroText, bool localized) => localized ? macroText.StartsWith(WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroToday), StringComparison.CurrentCultureIgnoreCase) : macroText.StartsWith("@today", StringComparison.OrdinalIgnoreCase);

    private string[] GetComplexMacroParts(string macroText)
    {
      string str = macroText;
      if ((str != null ? (!str.StartsWith("@", StringComparison.OrdinalIgnoreCase) ? 1 : 0) : 1) != 0)
        return (string[]) null;
      int length1 = str.IndexOf('(');
      if (length1 > 0)
      {
        str = str.Substring(0, length1);
      }
      else
      {
        int length2 = str.IndexOfAny(new char[2]{ '+', '-' });
        if (length2 > 0)
          str = str.Substring(0, length2);
      }
      return new string[2]
      {
        str.Trim(),
        macroText.Substring(str.Length)
      };
    }

    private bool IsComplexMacro(string macroText, bool localized)
    {
      string[] complexMacroParts = this.GetComplexMacroParts(macroText);
      if (complexMacroParts == null)
        return false;
      string key = complexMacroParts[0];
      return localized ? this.m_invariantOperatorLookup.ContainsKey(key) : this.m_localizedOperatorLookup.ContainsKey(key);
    }

    private string GetComplexMacro(string macroText, bool localized)
    {
      string[] complexMacroParts = this.GetComplexMacroParts(macroText);
      if (complexMacroParts == null)
        return macroText;
      string key = complexMacroParts[0];
      string str1 = complexMacroParts[1];
      if (localized)
      {
        string str2;
        if (this.m_invariantOperatorLookup.TryGetValue(key, out str2))
          return str2 + str1;
      }
      else
      {
        string str3;
        if (this.m_localizedOperatorLookup.TryGetValue(key, out str3))
          return str3 + str1;
      }
      return macroText;
    }

    public bool IsProjectMacro(string macro, bool localized) => localized ? TFStringComparer.QueryOperator.Equals(macro, this.GetLocalizedOperator("@project")) : TFStringComparer.QueryOperator.Equals(macro, "@project");

    public static bool IsCurrentIterationMacro(string macro, bool localized) => localized ? TFStringComparer.QueryOperator.Equals(macro, WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroCurrentIteration)) : TFStringComparer.QueryOperator.Equals(macro, "@currentIteration");

    public static bool IsFollowsMacro(string macro, bool localized) => localized ? TFStringComparer.QueryOperator.Equals(macro, WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroFollows)) : TFStringComparer.QueryOperator.Equals(macro, "@follows");

    public static string GetInvariantTodayMacro(string localizedMacro) => "@today" + localizedMacro.Substring(WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroToday).Length);

    private string GetLocalizedTodayMacro(string invariantMacro) => WiqlOperators.GetMacro(CommonClientResourceStrings.WiqlOperators_MacroToday) + invariantMacro.Substring("@today".Length);

    private void AddOperatorPair(string invariantValue, string localizedValue)
    {
      this.AddOperatorPair(this.m_localizedOperatorLookup, invariantValue, localizedValue);
      this.AddOperatorPair(this.m_invariantOperatorLookup, localizedValue, invariantValue);
    }

    private bool AddOperatorPair(IDictionary<string, string> hash, string key, string value)
    {
      if (hash.ContainsKey(key))
        return false;
      hash.Add(key, value);
      return true;
    }

    private void AddSupportedOperatorPair(string invariantOperator, string supportedOperator)
    {
      this.AddOperatorPair(this.m_supportedLocalizedOperationsLookup, invariantOperator, supportedOperator);
      this.AddOperatorPair(this.m_supportedInvariantOperationsLookup, supportedOperator, this.GetLocalizedOperator(invariantOperator));
    }

    public static string ConvertToContainsWords(string containsOperator) => TFStringComparer.QueryOperator.Equals(containsOperator, "CONTAINS") ? "CONTAINS WORDS" : "NOT CONTAINS WORDS";

    public static string ConvertToContains(string containsWordsOperator) => TFStringComparer.QueryOperator.Equals(containsWordsOperator, "CONTAINS WORDS") ? "CONTAINS" : "NOT CONTAINS";

    public IDictionary<string, string> GetSupportedOperationLookup(bool isInvariant) => isInvariant ? this.m_supportedInvariantOperationsLookup : this.m_supportedLocalizedOperationsLookup;

    public enum LogicalOperator
    {
      None = -1, // 0xFFFFFFFF
      And = 0,
      Or = 1,
    }
  }
}
