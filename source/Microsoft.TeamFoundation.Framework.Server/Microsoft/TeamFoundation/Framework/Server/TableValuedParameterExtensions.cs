// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TableValuedParameterExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class TableValuedParameterExtensions
  {
    private static readonly SqlMetaData[] typ_StrongBoxItemTable = new SqlMetaData[6]
    {
      new SqlMetaData("LookupKey", SqlDbType.NVarChar, 512L),
      new SqlMetaData("ItemKind", SqlDbType.TinyInt),
      new SqlMetaData("EncryptedContent", SqlDbType.VarBinary, -1L),
      new SqlMetaData("ExpirationDate", SqlDbType.DateTime),
      new SqlMetaData("CredentialName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SigningKeyId", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_StrongBoxItemTable2 = new SqlMetaData[7]
    {
      new SqlMetaData("LookupKey", SqlDbType.NVarChar, 512L),
      new SqlMetaData("ItemKind", SqlDbType.TinyInt),
      new SqlMetaData("EncryptedContent", SqlDbType.VarBinary, -1L),
      new SqlMetaData("ExpirationDate", SqlDbType.DateTime),
      new SqlMetaData("CredentialName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SigningKeyId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SecureFileId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_StrongBoxItemTableForReencryption = new SqlMetaData[10]
    {
      new SqlMetaData("LookupKey", SqlDbType.NVarChar, 512L),
      new SqlMetaData("ItemKind", SqlDbType.TinyInt),
      new SqlMetaData("EncryptedContent", SqlDbType.VarBinary, -1L),
      new SqlMetaData("ExpirationDate", SqlDbType.DateTime),
      new SqlMetaData("CredentialName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SigningKeyId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SecureFileId", SqlDbType.Int),
      new SqlMetaData("DrawerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("PrevFileId", SqlDbType.Int),
      new SqlMetaData("PrevContent", SqlDbType.VarBinary, -1L)
    };

    public static SqlParameter BindStrongBoxItemInfoTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<StrongBoxItemInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<StrongBoxItemInfo>();
      System.Func<StrongBoxItemInfo, SqlDataRecord> selector = (System.Func<StrongBoxItemInfo, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(TableValuedParameterExtensions.typ_StrongBoxItemTable);
        record.SetString(0, row.LookupKey);
        record.SetByte(1, (byte) row.ItemKind);
        if (row.EncryptedContent != null)
          record.SetBytes(2, 0L, row.EncryptedContent, 0, row.EncryptedContent.Length);
        DateTime? expirationDate = row.ExpirationDate;
        if (expirationDate.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          expirationDate = row.ExpirationDate;
          DateTime dateTime = expirationDate.Value;
          sqlDataRecord.SetDateTime(3, dateTime);
        }
        record.SetNullableString(4, row.CredentialName);
        record.SetGuid(5, row.SigningKeyId);
        return record;
      });
      return component.BindTable(parameterName, "typ_StrongBoxItemTable", rows.Select<StrongBoxItemInfo, SqlDataRecord>(selector));
    }

    public static SqlParameter BindStrongBoxItemInfoTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<StrongBoxItemInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<StrongBoxItemInfo>();
      System.Func<StrongBoxItemInfo, SqlDataRecord> selector = (System.Func<StrongBoxItemInfo, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(TableValuedParameterExtensions.typ_StrongBoxItemTable2);
        record.SetString(0, row.LookupKey);
        record.SetByte(1, (byte) row.ItemKind);
        if (row.EncryptedContent != null)
          record.SetBytes(2, 0L, row.EncryptedContent, 0, row.EncryptedContent.Length);
        DateTime? expirationDate = row.ExpirationDate;
        if (expirationDate.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          expirationDate = row.ExpirationDate;
          DateTime dateTime = expirationDate.Value;
          sqlDataRecord.SetDateTime(3, dateTime);
        }
        record.SetNullableString(4, row.CredentialName);
        record.SetGuid(5, row.SigningKeyId);
        if (row.SecureFileId > 0)
          record.SetInt32(6, row.SecureFileId);
        return record;
      });
      return component.BindTable(parameterName, "typ_StrongBoxItemTable2", rows.Select<StrongBoxItemInfo, SqlDataRecord>(selector));
    }

    public static SqlParameter BindStrongBoxItemInfoTableForReencryption(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<TeamFoundationStrongBoxServiceBase.ReencryptionData> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationStrongBoxServiceBase.ReencryptionData>();
      System.Func<TeamFoundationStrongBoxServiceBase.ReencryptionData, SqlDataRecord> selector = (System.Func<TeamFoundationStrongBoxServiceBase.ReencryptionData, SqlDataRecord>) (itemData =>
      {
        SqlDataRecord record = new SqlDataRecord(TableValuedParameterExtensions.typ_StrongBoxItemTableForReencryption);
        StrongBoxItemInfo reencryptedItem = itemData.ReencryptedItem;
        record.SetString(0, reencryptedItem.LookupKey);
        record.SetByte(1, (byte) reencryptedItem.ItemKind);
        if (reencryptedItem.EncryptedContent != null)
          record.SetBytes(2, 0L, reencryptedItem.EncryptedContent, 0, reencryptedItem.EncryptedContent.Length);
        if (reencryptedItem.ExpirationDate.HasValue)
          record.SetDateTime(3, reencryptedItem.ExpirationDate.Value);
        record.SetNullableString(4, reencryptedItem.CredentialName);
        record.SetGuid(5, reencryptedItem.SigningKeyId);
        if (reencryptedItem.SecureFileId > 0)
          record.SetInt32(6, reencryptedItem.SecureFileId);
        record.SetGuid(7, reencryptedItem.DrawerId);
        if (itemData.PreviousFileId > 0)
          record.SetInt32(8, itemData.PreviousFileId);
        if (itemData.PreviousContent != null)
          record.SetBytes(9, 0L, itemData.PreviousContent, 0, itemData.PreviousContent.Length);
        return record;
      });
      return component.BindTable(parameterName, "typ_StrongBoxItemTableForReencryption", rows.Select<TeamFoundationStrongBoxServiceBase.ReencryptionData, SqlDataRecord>(selector));
    }
  }
}
