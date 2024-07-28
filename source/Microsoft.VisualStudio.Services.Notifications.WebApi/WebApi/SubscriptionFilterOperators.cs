// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionFilterOperators
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public static class SubscriptionFilterOperators
  {
    private static Dictionary<byte, Func<string>> s_operatorTranslationTable = new Dictionary<byte, Func<string>>()
    {
      [(byte) 12] = new Func<string>(NotificationsWebApiResources.OperatorEqualTo),
      [(byte) 9] = new Func<string>(NotificationsWebApiResources.OperatorGT),
      [(byte) 11] = new Func<string>(NotificationsWebApiResources.OperatorGTE),
      [(byte) 16] = new Func<string>(NotificationsWebApiResources.OperatorLike),
      [(byte) 8] = new Func<string>(NotificationsWebApiResources.OperatorLT),
      [(byte) 10] = new Func<string>(NotificationsWebApiResources.OperatorLTE),
      [(byte) 15] = new Func<string>(NotificationsWebApiResources.OperatorMatch),
      [(byte) 13] = new Func<string>(NotificationsWebApiResources.OperatorNotEqualTo),
      [(byte) 14] = new Func<string>(NotificationsWebApiResources.OperatorUnder),
      [(byte) 17] = new Func<string>(NotificationsWebApiResources.OperatorDynamic),
      [(byte) 26] = new Func<string>(NotificationsWebApiResources.OperatorUnderPath),
      [(byte) 160] = new Func<string>(NotificationsWebApiResources.OperatorChanges),
      [(byte) 161] = new Func<string>(NotificationsWebApiResources.OperatorChangesFrom),
      [(byte) 162] = new Func<string>(NotificationsWebApiResources.OperatorChangesTo),
      [(byte) 163] = new Func<string>(NotificationsWebApiResources.OperatorNotUnder),
      [(byte) 25] = new Func<string>(NotificationsWebApiResources.OperatorNotContains),
      [(byte) 27] = new Func<string>(NotificationsWebApiResources.OperatorContainsValue),
      [(byte) 28] = new Func<string>(NotificationsWebApiResources.OperatorDoesNotContainValue),
      [(byte) 29] = new Func<string>(NotificationsWebApiResources.OperatorMemberOf)
    };
    private static Dictionary<string, Func<string>> s_contributedOperatorTranslationTable = new Dictionary<string, Func<string>>()
    {
      ["equalTo"] = new Func<string>(NotificationsWebApiResources.OperatorEqualTo),
      ["greaterThan"] = new Func<string>(NotificationsWebApiResources.OperatorGT),
      ["greaterThanOrEqualTo"] = new Func<string>(NotificationsWebApiResources.OperatorGTE),
      ["like"] = new Func<string>(NotificationsWebApiResources.OperatorLike),
      ["lessThan"] = new Func<string>(NotificationsWebApiResources.OperatorLT),
      ["lessThanOrEqualTo"] = new Func<string>(NotificationsWebApiResources.OperatorLTE),
      ["contains"] = new Func<string>(NotificationsWebApiResources.OperatorMatch),
      ["notEqualTo"] = new Func<string>(NotificationsWebApiResources.OperatorNotEqualTo),
      ["under"] = new Func<string>(NotificationsWebApiResources.OperatorUnder),
      ["dynamic"] = new Func<string>(NotificationsWebApiResources.OperatorDynamic),
      ["changes"] = new Func<string>(NotificationsWebApiResources.OperatorChanges),
      ["changesFrom"] = new Func<string>(NotificationsWebApiResources.OperatorChangesFrom),
      ["changesTo"] = new Func<string>(NotificationsWebApiResources.OperatorChangesTo),
      ["notUnder"] = new Func<string>(NotificationsWebApiResources.OperatorNotUnder),
      ["notContains"] = new Func<string>(NotificationsWebApiResources.OperatorNotContains),
      ["and"] = new Func<string>(NotificationsWebApiResources.LogicalOperatorAnd),
      ["or"] = new Func<string>(NotificationsWebApiResources.LogicalOperatorOr),
      ["underPath"] = new Func<string>(NotificationsWebApiResources.OperatorUnderPath),
      ["containsValue"] = new Func<string>(NotificationsWebApiResources.OperatorContainsValue),
      ["doesNotContainValue"] = new Func<string>(NotificationsWebApiResources.OperatorDoesNotContainValue),
      ["memberOf"] = new Func<string>(NotificationsWebApiResources.OperatorMemberOf)
    };
    public const byte c_OPChanges = 160;
    public const byte c_OPChangesFrom = 161;
    public const byte c_OPChangesTo = 162;
    public const byte c_OPNotUnder = 163;
    public const byte c_OPNone = 255;
    public const string c_OPEqualId = "equalTo";
    public const string c_OPNotEqualId = "notEqualTo";
    public const string c_OPGTId = "greaterThan";
    public const string c_OPGTEId = "greaterThanOrEqualTo";
    public const string c_OPLikeId = "like";
    public const string c_OPLTId = "lessThan";
    public const string c_OPLTEId = "lessThanOrEqualTo";
    public const string c_OPChangesId = "changes";
    public const string c_OPChangesFromId = "changesFrom";
    public const string c_OPChangesToId = "changesTo";
    public const string c_OPUnderId = "under";
    public const string c_OPNotUnderId = "notUnder";
    public const string c_OPMatchId = "contains";
    public const string c_OPNotContainsId = "notContains";
    public const string c_OPDynamicId = "dynamic";
    public const string c_OPAndId = "and";
    public const string c_OPOrId = "or";
    public const string c_OPUnderPathId = "underPath";
    public const string c_OPContainsValueId = "containsValue";
    public const string c_OPDoesNotContainValueId = "doesNotContainValue";
    public const string c_OPMemberOfId = "memberOf";

    public static byte GetRawOperator(string localizedOperator)
    {
      foreach (byte key in SubscriptionFilterOperators.s_operatorTranslationTable.Keys)
      {
        if (string.Equals(localizedOperator, SubscriptionFilterOperators.s_operatorTranslationTable[key](), StringComparison.CurrentCultureIgnoreCase))
          return key;
      }
      return 12;
    }

    public static string GetLocalizedOperator(byte rawOperator)
    {
      Func<string> func;
      SubscriptionFilterOperators.s_operatorTranslationTable.TryGetValue(rawOperator, out func);
      if (rawOperator == (byte) 0)
        return NotificationsWebApiResources.OperatorEqualTo();
      return func != null ? func() : Token.spellings[(int) rawOperator];
    }

    public static string GetLocalizedOperator(string operatorId)
    {
      Func<string> func;
      SubscriptionFilterOperators.s_contributedOperatorTranslationTable.TryGetValue(operatorId, out func);
      return func != null ? func() : NotificationsWebApiResources.OperatorUnknown((object) operatorId);
    }

    public static IEnumerable<string> GetLocalizedOperators(IEnumerable<byte> rawOperators) => rawOperators.Select<byte, string>(SubscriptionFilterOperators.\u003C\u003EO.\u003C0\u003E__GetLocalizedOperator ?? (SubscriptionFilterOperators.\u003C\u003EO.\u003C0\u003E__GetLocalizedOperator = new Func<byte, string>(SubscriptionFilterOperators.GetLocalizedOperator)));

    public static List<NotificationEventFieldOperator> GetLocalizedFieldOperators(
      IEnumerable<byte> rawOperators)
    {
      List<NotificationEventFieldOperator> localizedFieldOperators = new List<NotificationEventFieldOperator>();
      foreach (byte rawOperator in rawOperators)
        localizedFieldOperators.Add(new NotificationEventFieldOperator()
        {
          Id = rawOperator.ToString(),
          DisplayName = SubscriptionFilterOperators.GetLocalizedOperator(rawOperator)
        });
      return localizedFieldOperators;
    }

    public static bool IsChangeOperator(byte rawOperator) => rawOperator == (byte) 160 || rawOperator == (byte) 161 || rawOperator == (byte) 162;

    public static FieldFilterType GetFilterOperator(byte rawOperator)
    {
      switch (rawOperator)
      {
        case 13:
          return FieldFilterType.NotEquals;
        case 14:
          return FieldFilterType.StartsWith;
        case 15:
          return FieldFilterType.Contains;
        case 25:
          return FieldFilterType.NotContains;
        case 26:
          return FieldFilterType.UnderPath;
        case 29:
          return FieldFilterType.MemberOf;
        case 163:
          return FieldFilterType.NotStartsWith;
        default:
          return FieldFilterType.Equals;
      }
    }

    public static byte GetRawOperator(FieldFilterType filterOperator)
    {
      switch (filterOperator)
      {
        case FieldFilterType.NotEquals:
          return 13;
        case FieldFilterType.StartsWith:
          return 14;
        case FieldFilterType.NotStartsWith:
          return 163;
        case FieldFilterType.Contains:
          return 15;
        case FieldFilterType.NotContains:
          return 25;
        case FieldFilterType.UnderPath:
          return 26;
        case FieldFilterType.MemberOf:
          return 29;
        default:
          return 12;
      }
    }
  }
}
