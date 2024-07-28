// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.GalleryTableValuedParameterExtensions
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal static class GalleryTableValuedParameterExtensions
  {
    private static SqlMetaData[] typ_ExtensionFileTable = new SqlMetaData[5]
    {
      new SqlMetaData("Version", SqlDbType.VarChar, 25L),
      new SqlMetaData("AssetType", SqlDbType.VarChar, 100L),
      new SqlMetaData("ContentType", SqlDbType.VarChar, 100L),
      new SqlMetaData("FileId", SqlDbType.Int),
      new SqlMetaData("ShortDescription", SqlDbType.NVarChar, 200L)
    };
    private static SqlMetaData[] typ_ExtensionFileTable2 = new SqlMetaData[5]
    {
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("AssetType", SqlDbType.VarChar, 100L),
      new SqlMetaData("ContentType", SqlDbType.VarChar, 100L),
      new SqlMetaData("FileId", SqlDbType.Int),
      new SqlMetaData("ShortDescription", SqlDbType.NVarChar, 200L)
    };
    private static SqlMetaData[] typ_ExtensionFileTable3 = new SqlMetaData[5]
    {
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("AssetType", SqlDbType.VarChar, 260L),
      new SqlMetaData("ContentType", SqlDbType.VarChar, 100L),
      new SqlMetaData("FileId", SqlDbType.Int),
      new SqlMetaData("ShortDescription", SqlDbType.NVarChar, 200L)
    };
    private static SqlMetaData[] typ_QueryFilterTable = new SqlMetaData[4]
    {
      new SqlMetaData("QueryIndex", SqlDbType.Int),
      new SqlMetaData("FilterCount", SqlDbType.Int),
      new SqlMetaData("PageSize", SqlDbType.Int),
      new SqlMetaData("Direction", SqlDbType.Int)
    };
    private static SqlMetaData[] typ_QueryFiltersTable = new SqlMetaData[6]
    {
      new SqlMetaData("QueryIndex", SqlDbType.Int),
      new SqlMetaData("FilterCount", SqlDbType.Int),
      new SqlMetaData("PageSize", SqlDbType.Int),
      new SqlMetaData("PageNumber", SqlDbType.Int),
      new SqlMetaData("SortByType", SqlDbType.Int),
      new SqlMetaData("SortOrderType", SqlDbType.Int)
    };
    private static SqlMetaData[] typ_CategoryTitleLangTable = new SqlMetaData[3]
    {
      new SqlMetaData("Lang", SqlDbType.NVarChar, 10L),
      new SqlMetaData("Title", SqlDbType.NVarChar, 100L),
      new SqlMetaData("Lcid", SqlDbType.Int)
    };
    private static SqlMetaData[] typ_SearchFilterValueTable = new SqlMetaData[4]
    {
      new SqlMetaData("QueryIndex", SqlDbType.Int),
      new SqlMetaData("SearchFilterType", SqlDbType.Int),
      new SqlMetaData("Operator", SqlDbType.Int),
      new SqlMetaData("FilterValue", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    private static SqlMetaData[] typ_QueryFilterValueTable = new SqlMetaData[4]
    {
      new SqlMetaData("QueryIndex", SqlDbType.Int),
      new SqlMetaData("FilterIndex", SqlDbType.Int),
      new SqlMetaData("FilterValueType", SqlDbType.Int),
      new SqlMetaData("FilterValue", SqlDbType.NVarChar, 50L)
    };
    private static SqlMetaData[] typ_QueryFilterValuesTable = new SqlMetaData[4]
    {
      new SqlMetaData("QueryIndex", SqlDbType.Int),
      new SqlMetaData("FilterIndex", SqlDbType.Int),
      new SqlMetaData("FilterValueType", SqlDbType.Int),
      new SqlMetaData("FilterValue", SqlDbType.NVarChar, 201L)
    };
    private static SqlMetaData[] typ_ExtensionInstallationTarget = new SqlMetaData[6]
    {
      new SqlMetaData("Target", SqlDbType.NVarChar, 128L),
      new SqlMetaData("VersionRange", SqlDbType.NVarChar, 128L),
      new SqlMetaData("MinInclusive", SqlDbType.Bit),
      new SqlMetaData("MinVersion", SqlDbType.NVarChar, 50L),
      new SqlMetaData("MaxVersion", SqlDbType.NVarChar, 50L),
      new SqlMetaData("MaxInclusive", SqlDbType.Bit)
    };
    private static SqlMetaData[] typ_ExtensionInstallationTarget3 = new SqlMetaData[9]
    {
      new SqlMetaData("Target", SqlDbType.NVarChar, 128L),
      new SqlMetaData("VersionRange", SqlDbType.NVarChar, 128L),
      new SqlMetaData("MinInclusive", SqlDbType.Bit),
      new SqlMetaData("MinVersion", SqlDbType.NVarChar, 50L),
      new SqlMetaData("MaxVersion", SqlDbType.NVarChar, 50L),
      new SqlMetaData("MaxInclusive", SqlDbType.Bit),
      new SqlMetaData("ProductArchitecture", SqlDbType.NVarChar, 30L),
      new SqlMetaData("ExtensionVersion", SqlDbType.NVarChar, 43L),
      new SqlMetaData("TargetPlatform", SqlDbType.NVarChar, 100L)
    };
    private static SqlMetaData[] typ_ExtensionInstallationTarget2 = new SqlMetaData[7]
    {
      new SqlMetaData("Target", SqlDbType.NVarChar, 128L),
      new SqlMetaData("VersionRange", SqlDbType.NVarChar, 128L),
      new SqlMetaData("MinInclusive", SqlDbType.Bit),
      new SqlMetaData("MinVersion", SqlDbType.NVarChar, 50L),
      new SqlMetaData("MaxVersion", SqlDbType.NVarChar, 50L),
      new SqlMetaData("MaxInclusive", SqlDbType.Bit),
      new SqlMetaData("ProductArchitecture", SqlDbType.NVarChar, 30L)
    };
    private static SqlMetaData[] typ_DailyStatsUpdateDataTable = new SqlMetaData[4]
    {
      new SqlMetaData("ExtensionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("DailyStatType", SqlDbType.Int),
      new SqlMetaData("StatDate", SqlDbType.DateTime)
    };
    private static SqlMetaData[] typ_DailyStatsUpdateDataTable2 = new SqlMetaData[5]
    {
      new SqlMetaData("ExtensionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("DailyStatType", SqlDbType.Int),
      new SqlMetaData("StatDate", SqlDbType.DateTime),
      new SqlMetaData("TargetPlatform", SqlDbType.VarChar, 30L)
    };
    private static SqlMetaData[] typ_DailyStatsUpdateDataTable3 = new SqlMetaData[5]
    {
      new SqlMetaData("ExtensionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Version", SqlDbType.VarChar, 43L),
      new SqlMetaData("DailyStatType", SqlDbType.Int),
      new SqlMetaData("StatDate", SqlDbType.DateTime),
      new SqlMetaData("TargetPlatform", SqlDbType.VarChar, 100L)
    };
    private static SqlMetaData[] typ_AuditLogEntriesTable = new SqlMetaData[6]
    {
      new SqlMetaData("ChangedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AuditAction", SqlDbType.VarChar, 100L),
      new SqlMetaData("ActionDate", SqlDbType.DateTime),
      new SqlMetaData("ResourceId", SqlDbType.VarChar, 100L),
      new SqlMetaData("ResourceType", SqlDbType.VarChar, 100L),
      new SqlMetaData("LogData", SqlDbType.NVarChar, 2048L)
    };

    public static SqlParameter BindExtensionFileTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ExtensionFile> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionFile>();
      System.Func<ExtensionFile, SqlDataRecord> selector = (System.Func<ExtensionFile, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_ExtensionFileTable);
        AssetInfo assetInfo = new AssetInfo(row.AssetType, row.Language);
        record.SetString(0, row.Version, BindStringBehavior.EmptyStringToNull);
        record.SetString(1, assetInfo.ToString(), BindStringBehavior.EmptyStringToNull);
        record.SetString(2, row.ContentType, BindStringBehavior.EmptyStringToNull);
        record.SetInt32(3, row.FileId);
        record.SetString(4, row.ShortDescription, BindStringBehavior.EmptyStringToNull);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_AssetTable", rows.Select<ExtensionFile, SqlDataRecord>(selector));
    }

    public static SqlParameter BindExtensionFileTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ExtensionFile> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionFile>();
      System.Func<ExtensionFile, SqlDataRecord> selector = (System.Func<ExtensionFile, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_ExtensionFileTable2);
        AssetInfo assetInfo = new AssetInfo(row.AssetType, row.Language);
        record.SetString(0, row.Version, BindStringBehavior.EmptyStringToNull);
        record.SetString(1, assetInfo.ToString(), BindStringBehavior.EmptyStringToNull);
        record.SetString(2, row.ContentType, BindStringBehavior.EmptyStringToNull);
        record.SetInt32(3, row.FileId);
        record.SetString(4, row.ShortDescription, BindStringBehavior.EmptyStringToNull);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_AssetTable2", rows.Select<ExtensionFile, SqlDataRecord>(selector));
    }

    public static SqlParameter BindExtensionFileTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ExtensionFile> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionFile>();
      System.Func<ExtensionFile, SqlDataRecord> selector = (System.Func<ExtensionFile, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_ExtensionFileTable3);
        AssetInfo assetInfo = new AssetInfo(row.AssetType, row.Language);
        record.SetString(0, row.Version, BindStringBehavior.EmptyStringToNull);
        record.SetString(1, assetInfo.ToString(), BindStringBehavior.EmptyStringToNull);
        record.SetString(2, row.ContentType, BindStringBehavior.EmptyStringToNull);
        record.SetInt32(3, row.FileId);
        record.SetString(4, row.ShortDescription, BindStringBehavior.EmptyStringToNull);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_AssetTable3", rows.Select<ExtensionFile, SqlDataRecord>(selector));
    }

    public static SqlParameter BindQueryFilterTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<QueryFilter> rows)
    {
      rows = rows ?? Enumerable.Empty<QueryFilter>();
      System.Func<QueryFilter, SqlDataRecord> selector = (System.Func<QueryFilter, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_QueryFilterTable);
        sqlDataRecord.SetInt32(0, row.QueryIndex);
        sqlDataRecord.SetInt32(1, row.Criteria.Count);
        sqlDataRecord.SetInt32(2, row.PageSize);
        sqlDataRecord.SetInt32(3, (int) row.Direction);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "Gallery.typ_QueryFilterTable", rows.Select<QueryFilter, SqlDataRecord>(selector));
    }

    public static SqlParameter BindQueryFiltersTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<QueryFilter> rows)
    {
      rows = rows ?? Enumerable.Empty<QueryFilter>();
      System.Func<QueryFilter, SqlDataRecord> selector = (System.Func<QueryFilter, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_QueryFiltersTable);
        sqlDataRecord.SetInt32(0, row.QueryIndex);
        sqlDataRecord.SetInt32(1, row.Criteria.Count);
        sqlDataRecord.SetInt32(2, row.PageSize);
        sqlDataRecord.SetInt32(3, row.PageNumber);
        sqlDataRecord.SetInt32(4, row.SortBy);
        sqlDataRecord.SetInt32(5, row.SortOrder);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "Gallery.typ_QueryFiltersTable", rows.Select<QueryFilter, SqlDataRecord>(selector));
    }

    public static SqlParameter BindCategoryTitleLangTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<CategoryLanguageTitle> rows)
    {
      rows = rows ?? Enumerable.Empty<CategoryLanguageTitle>();
      System.Func<CategoryLanguageTitle, SqlDataRecord> selector = (System.Func<CategoryLanguageTitle, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_CategoryTitleLangTable);
        sqlDataRecord.SetString(0, row.Lang);
        sqlDataRecord.SetString(1, row.Title);
        sqlDataRecord.SetInt32(2, row.Lcid);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "Gallery.typ_CategoryTitleLangTable", rows.Select<CategoryLanguageTitle, SqlDataRecord>(selector));
    }

    public static SqlParameter BindSearchFilterValueTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<SearchCriteria> rows)
    {
      rows = rows ?? Enumerable.Empty<SearchCriteria>();
      System.Func<SearchCriteria, SqlDataRecord> selector = (System.Func<SearchCriteria, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_SearchFilterValueTable);
        record.SetInt32(0, 0);
        record.SetInt32(1, (int) row.FilterType);
        record.SetInt32(2, (int) row.OperatorType);
        record.SetNullableStringAsEmpty(3, row.FilterValue);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_SearchFilterValueTable", rows.Select<SearchCriteria, SqlDataRecord>(selector));
    }

    public static SqlParameter BindQueryFilterValueTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<QueryFilterValue> rows)
    {
      rows = rows ?? Enumerable.Empty<QueryFilterValue>();
      System.Func<QueryFilterValue, SqlDataRecord> selector = (System.Func<QueryFilterValue, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_QueryFilterValueTable);
        record.SetInt32(0, row.QueryIndex);
        record.SetInt32(1, row.FilterIndex);
        record.SetInt32(2, row.FilterValueType);
        record.SetNullableStringAsEmpty(3, row.FilterValue);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_QueryFilterValueTable", rows.Select<QueryFilterValue, SqlDataRecord>(selector));
    }

    public static SqlParameter BindQueryFilterValuesTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<QueryFilterValue> rows)
    {
      rows = rows ?? Enumerable.Empty<QueryFilterValue>();
      System.Func<QueryFilterValue, SqlDataRecord> selector = (System.Func<QueryFilterValue, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_QueryFilterValuesTable);
        record.SetInt32(0, row.QueryIndex);
        record.SetInt32(1, row.FilterIndex);
        record.SetInt32(2, row.FilterValueType);
        record.SetNullableStringAsEmpty(3, row.FilterValue);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_QueryFilterValuesTable", rows.Select<QueryFilterValue, SqlDataRecord>(selector));
    }

    public static SqlParameter BindExtensionInstallationTargetTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<InstallationTarget> rows)
    {
      rows = rows ?? Enumerable.Empty<InstallationTarget>();
      System.Func<InstallationTarget, SqlDataRecord> selector = (System.Func<InstallationTarget, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_ExtensionInstallationTarget);
        record.SetString(0, row.Target);
        record.SetNullableStringAsEmpty(1, row.TargetVersion);
        record.SetBoolean(2, row.MinInclusive);
        record.SetNullableStringAsEmpty(3, row.MinVersion?.ToString());
        record.SetNullableStringAsEmpty(4, row.MaxVersion?.ToString());
        record.SetBoolean(5, row.MaxInclusive);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_ExtensionInstallationTarget", rows.Select<InstallationTarget, SqlDataRecord>(selector));
    }

    public static SqlParameter BindExtensionInstallationTargetTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<InstallationTarget> rows)
    {
      rows = rows ?? Enumerable.Empty<InstallationTarget>();
      System.Func<InstallationTarget, SqlDataRecord> selector = (System.Func<InstallationTarget, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_ExtensionInstallationTarget2);
        record.SetString(0, row.Target);
        record.SetNullableStringAsEmpty(1, row.TargetVersion);
        record.SetBoolean(2, row.MinInclusive);
        record.SetNullableStringAsEmpty(3, row.MinVersion?.ToString());
        record.SetNullableStringAsEmpty(4, row.MaxVersion?.ToString());
        record.SetBoolean(5, row.MaxInclusive);
        record.SetNullableString(6, row.ProductArchitecture);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_ExtensionInstallationTarget2", rows.Select<InstallationTarget, SqlDataRecord>(selector));
    }

    public static SqlParameter BindExtensionInstallationTargetTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<InstallationTarget> rows)
    {
      rows = rows ?? Enumerable.Empty<InstallationTarget>();
      System.Func<InstallationTarget, SqlDataRecord> selector = (System.Func<InstallationTarget, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_ExtensionInstallationTarget3);
        record.SetString(0, row.Target);
        record.SetNullableStringAsEmpty(1, row.TargetVersion);
        record.SetBoolean(2, row.MinInclusive);
        record.SetNullableStringAsEmpty(3, row.MinVersion?.ToString());
        record.SetNullableStringAsEmpty(4, row.MaxVersion?.ToString());
        record.SetBoolean(5, row.MaxInclusive);
        record.SetNullableString(6, row.ProductArchitecture);
        record.SetNullableString(7, row.ExtensionVersion);
        record.SetNullableString(8, row.TargetPlatform);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_ExtensionInstallationTarget3", rows.Select<InstallationTarget, SqlDataRecord>(selector));
    }

    public static SqlParameter BindDailyStatsUpdateDataTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ExtensionDailyStatsUpdateData> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionDailyStatsUpdateData>();
      System.Func<ExtensionDailyStatsUpdateData, SqlDataRecord> selector = (System.Func<ExtensionDailyStatsUpdateData, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_DailyStatsUpdateDataTable);
        sqlDataRecord.SetGuid(0, row.ExtensionId);
        sqlDataRecord.SetString(1, row.Version);
        sqlDataRecord.SetInt32(2, (int) row.StatType);
        sqlDataRecord.SetDateTime(3, row.StatDate.Date);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "Gallery.typ_DailyStatsUpdateDataTable", rows.Select<ExtensionDailyStatsUpdateData, SqlDataRecord>(selector));
    }

    public static SqlParameter BindDailyStatsUpdateDataTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ExtensionDailyStatsUpdateData> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionDailyStatsUpdateData>();
      System.Func<ExtensionDailyStatsUpdateData, SqlDataRecord> selector = (System.Func<ExtensionDailyStatsUpdateData, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_DailyStatsUpdateDataTable2);
        sqlDataRecord.SetGuid(0, row.ExtensionId);
        sqlDataRecord.SetString(1, row.Version);
        sqlDataRecord.SetInt32(2, (int) row.StatType);
        sqlDataRecord.SetDateTime(3, row.StatDate.Date);
        if (row.TargetPlatform != null)
          sqlDataRecord.SetString(4, row.TargetPlatform);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "Gallery.typ_DailyStatsUpdateDataTable2", rows.Select<ExtensionDailyStatsUpdateData, SqlDataRecord>(selector));
    }

    public static SqlParameter BindDailyStatsUpdateDataTable3(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<ExtensionDailyStatsUpdateData> rows)
    {
      rows = rows ?? Enumerable.Empty<ExtensionDailyStatsUpdateData>();
      System.Func<ExtensionDailyStatsUpdateData, SqlDataRecord> selector = (System.Func<ExtensionDailyStatsUpdateData, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_DailyStatsUpdateDataTable3);
        sqlDataRecord.SetGuid(0, row.ExtensionId);
        sqlDataRecord.SetString(1, row.Version);
        sqlDataRecord.SetInt32(2, (int) row.StatType);
        sqlDataRecord.SetDateTime(3, row.StatDate.Date);
        if (row.TargetPlatform != null)
          sqlDataRecord.SetString(4, row.TargetPlatform);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "Gallery.typ_DailyStatsUpdateDataTable3", rows.Select<ExtensionDailyStatsUpdateData, SqlDataRecord>(selector));
    }

    public static SqlParameter BindAuditLogEntriesTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<AuditLogEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<AuditLogEntry>();
      System.Func<AuditLogEntry, SqlDataRecord> selector = (System.Func<AuditLogEntry, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(GalleryTableValuedParameterExtensions.typ_AuditLogEntriesTable);
        record.SetGuid(0, row.ChangedByIdentity);
        record.SetString(1, row.AuditAction);
        record.SetDateTime(2, row.ActionDate);
        record.SetString(3, row.ResourceId);
        record.SetString(4, row.ResourceType);
        record.SetNullableStringAsEmpty(5, row.Data);
        return record;
      });
      return component.BindTable(parameterName, "Gallery.typ_AuditLogEntriesTable", rows.Select<AuditLogEntry, SqlDataRecord>(selector));
    }
  }
}
