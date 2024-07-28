// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TVPBinders
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  public class TVPBinders
  {
    private static readonly SqlMetaData[] typ_CardRuleTable = new SqlMetaData[7]
    {
      new SqlMetaData("RevisedDate", SqlDbType.DateTime),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Type", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("IsEnabled", SqlDbType.Bit),
      new SqlMetaData("Filter", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("FilterExpression", SqlDbType.NVarChar, 1024L)
    };
    public const string CardRuleTypeName = "typ_CardRuleTable";
    private static readonly SqlMetaData[] typ_UpdateCardRuleTable = new SqlMetaData[6]
    {
      new SqlMetaData("RevisedDate", SqlDbType.DateTime),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("IsEnabled", SqlDbType.Bit),
      new SqlMetaData("Filter", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("FilterExpression", SqlDbType.NVarChar, 1024L)
    };
    public const string UpdateCardRuleTypeName = "typ_UpdateCardRuleTable";
    private static readonly SqlMetaData[] typ_CardRuleAttributeTable = new SqlMetaData[3]
    {
      new SqlMetaData("RuleName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Value", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    public const string CardRuleAttributeTypeName = "typ_CardRuleAttributeTable";
    private static readonly SqlMetaData[] typ_CardRuleAttributeTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("RuleName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("RuleType", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Value", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    public const string CardRuleAttributeTypeName2 = "typ_CardRuleAttributeTable2";
    private static readonly SqlMetaData[] typ_CardRuleTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("RuleType", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    public const string CardRuleTypeTableName = "typ_CardRuleTypeTable";

    public static IEnumerable<SqlDataRecord> BindCardRuleRows(IEnumerable<BoardCardRuleRow> rules)
    {
      foreach (BoardCardRuleRow rule in rules)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TVPBinders.typ_CardRuleTable);
        sqlDataRecord.SetDateTime(0, rule.RevisedDate);
        sqlDataRecord.SetInt32(1, rule.Order);
        sqlDataRecord.SetString(2, rule.Name);
        sqlDataRecord.SetString(3, rule.Type);
        sqlDataRecord.SetBoolean(4, rule.IsEnabled);
        sqlDataRecord.SetString(5, rule.Filter);
        sqlDataRecord.SetString(6, rule.FilterExpression);
        yield return sqlDataRecord;
      }
    }

    public static IEnumerable<SqlDataRecord> BindUpdateCardRuleRows(
      IEnumerable<BoardCardRuleRow> rules)
    {
      foreach (BoardCardRuleRow rule in rules)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TVPBinders.typ_UpdateCardRuleTable);
        sqlDataRecord.SetDateTime(0, rule.RevisedDate);
        sqlDataRecord.SetInt32(1, rule.Order);
        sqlDataRecord.SetString(2, rule.Name);
        sqlDataRecord.SetBoolean(3, rule.IsEnabled);
        sqlDataRecord.SetString(4, rule.Filter);
        sqlDataRecord.SetString(5, rule.FilterExpression);
        yield return sqlDataRecord;
      }
    }

    public static IEnumerable<SqlDataRecord> BindCardStyleRows(
      IEnumerable<RuleAttributeRow> attributes)
    {
      foreach (RuleAttributeRow attribute in attributes)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TVPBinders.typ_CardRuleAttributeTable);
        sqlDataRecord.SetString(0, attribute.RuleName);
        sqlDataRecord.SetString(1, attribute.Name);
        sqlDataRecord.SetString(2, attribute.Value);
        yield return sqlDataRecord;
      }
    }

    public static IEnumerable<SqlDataRecord> BindCardStyleRows2(
      IEnumerable<RuleAttributeRow> attributes)
    {
      foreach (RuleAttributeRow attribute in attributes)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TVPBinders.typ_CardRuleAttributeTable2);
        sqlDataRecord.SetString(0, attribute.RuleName);
        sqlDataRecord.SetString(1, attribute.Type);
        sqlDataRecord.SetString(2, attribute.Name);
        sqlDataRecord.SetString(3, attribute.Value);
        yield return sqlDataRecord;
      }
    }

    public static IEnumerable<SqlDataRecord> BindCardRuleType(List<string> types)
    {
      foreach (string type in types)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TVPBinders.typ_CardRuleTypeTable);
        sqlDataRecord.SetString(0, type);
        yield return sqlDataRecord;
      }
    }

    public enum CardRuleIndex
    {
      RevisedDate,
      Order,
      Name,
      Type,
      IsEnabled,
      Filter,
      FilterExpression,
    }

    public enum UpdateCardRuleIndex
    {
      RevisedDate,
      Order,
      Name,
      IsEnabled,
      Filter,
      FilterExpression,
    }

    public enum CardRuleAttributeIndex
    {
      RuleName,
      Name,
      Value,
    }

    public enum CardRuleAttributeIndex2
    {
      RuleName,
      RuleType,
      Name,
      Value,
    }

    public enum CardRuleTypeIndex
    {
      RuleType,
    }
  }
}
