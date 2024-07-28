// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityExtensionTableValuedParametersBinder
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityExtensionTableValuedParametersBinder
  {
    private static readonly SqlMetaData[] IdentityExtensionTableSchema = new SqlMetaData[11]
    {
      new SqlMetaData("IdentityId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ResourceVersion", SqlDbType.TinyInt),
      new SqlMetaData("MetaTypeId", SqlDbType.TinyInt),
      new SqlMetaData("ExternalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ExternalRefreshToken", SqlDbType.VarChar, 4000L),
      new SqlMetaData("ExternalRefreshTokenValidFrom", SqlDbType.DateTime),
      new SqlMetaData("AuthenticationCredentialValidFrom", SqlDbType.DateTime),
      new SqlMetaData("ImageId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ImageType", SqlDbType.VarChar, 500L),
      new SqlMetaData("ConfirmedNotificationAddress", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CustomNotificationAddresses", SqlDbType.NVarChar, 4000L)
    };

    public virtual string NameOfTypIdentityextensionTable => "typ_IdentityExtensionTable";

    public virtual SqlMetaData[] TableTypeIdentityExtensionTableSchema => IdentityExtensionTableValuedParametersBinder.IdentityExtensionTableSchema;

    public virtual SqlParameter BindIdentityExtensionTable(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows)
    {
      return component.Bind<Microsoft.VisualStudio.Services.Identity.Identity>(parameterName, rows, this.TableTypeIdentityExtensionTableSchema, this.NameOfTypIdentityextensionTable, (Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity>) ((record, identity) =>
      {
        record.SetGuid(0, identity.Id);
        if (identity.ResourceVersion > 0)
          record.SetByte(1, (byte) identity.ResourceVersion);
        if (identity.MetaTypeId >= 0)
          record.SetByte(2, (byte) identity.MetaTypeId);
        if (identity.ResourceVersion <= 0)
          return;
        HashSet<string> modifiedProperties = identity.GetModifiedProperties();
        if (modifiedProperties == null)
          return;
        if (modifiedProperties.Contains("http://schemas.microsoft.com/identity/claims/objectidentifier"))
        {
          Guid guid = identity.Properties.GetValue<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
          record.SetGuid(3, guid);
        }
        if (modifiedProperties.Contains("AuthenticationCredentialValidFrom"))
        {
          long ticks = identity.Properties.GetValue<long>("AuthenticationCredentialValidFrom", 0L);
          DateTime dateTime = ticks == 0L ? new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc) : new DateTime(ticks, DateTimeKind.Utc);
          record.SetDateTime(6, dateTime);
        }
        if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Id"))
        {
          byte[] b = identity.Properties.GetValue<byte[]>("Microsoft.TeamFoundation.Identity.Image.Id", (byte[]) null);
          Guid guid = b == null ? Guid.Empty : new Guid(b);
          record.SetGuid(7, guid);
        }
        if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Type"))
        {
          string str = identity.Properties.GetValue<string>("Microsoft.TeamFoundation.Identity.Image.Type", string.Empty);
          record.SetNullableStringAsEmpty(8, str);
        }
        if (modifiedProperties.Contains("ConfirmedNotificationAddress"))
        {
          string str = identity.Properties.GetValue<string>("ConfirmedNotificationAddress", string.Empty);
          record.SetNullableStringAsEmpty(9, str);
        }
        if (!modifiedProperties.Contains("CustomNotificationAddresses"))
          return;
        string str1 = identity.Properties.GetValue<string>("CustomNotificationAddresses", string.Empty);
        record.SetNullableStringAsEmpty(10, str1);
      }));
    }

    public virtual SqlParameter BindIdentityExtensionTableForIdentityComponent11OrLater(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows)
    {
      return component.Bind<Microsoft.VisualStudio.Services.Identity.Identity>(parameterName, rows, this.TableTypeIdentityExtensionTableSchema, this.NameOfTypIdentityextensionTable, (Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity>) ((record, identity) =>
      {
        record.SetGuid(0, identity.MasterId == Guid.Empty ? identity.Id : identity.MasterId);
        if (identity.ResourceVersion > 0)
          record.SetByte(1, (byte) identity.ResourceVersion);
        if (identity.MetaTypeId >= 0 && identity.MetaTypeId < (int) byte.MaxValue)
          record.SetByte(2, (byte) identity.MetaTypeId);
        if (identity.ResourceVersion <= 0)
          return;
        HashSet<string> modifiedProperties = identity.GetModifiedProperties();
        if (modifiedProperties == null)
          return;
        if (modifiedProperties.Contains("http://schemas.microsoft.com/identity/claims/objectidentifier"))
        {
          Guid guid = identity.Properties.GetValue<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
          record.SetGuid(3, guid);
        }
        if (modifiedProperties.Contains("AuthenticationCredentialValidFrom"))
        {
          long ticks = identity.Properties.GetValue<long>("AuthenticationCredentialValidFrom", 0L);
          DateTime dateTime = ticks == 0L ? new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc) : new DateTime(ticks, DateTimeKind.Utc);
          record.SetDateTime(6, dateTime);
        }
        if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Id"))
        {
          byte[] b = identity.Properties.GetValue<byte[]>("Microsoft.TeamFoundation.Identity.Image.Id", (byte[]) null);
          Guid guid = b == null ? Guid.Empty : new Guid(b);
          record.SetGuid(7, guid);
        }
        if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Type"))
        {
          string str = identity.Properties.GetValue<string>("Microsoft.TeamFoundation.Identity.Image.Type", string.Empty);
          record.SetNullableStringAsEmpty(8, str);
        }
        if (modifiedProperties.Contains("ConfirmedNotificationAddress"))
        {
          string str = identity.Properties.GetValue<string>("ConfirmedNotificationAddress", string.Empty);
          record.SetNullableStringAsEmpty(9, str);
        }
        if (!modifiedProperties.Contains("CustomNotificationAddresses"))
          return;
        string str1 = identity.Properties.GetValue<string>("CustomNotificationAddresses", string.Empty);
        record.SetNullableStringAsEmpty(10, str1);
      }));
    }

    public virtual SqlParameter BindIdentityExtensionTableForIdentityComponent19OrLater(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> rows,
      bool allowMetadataUpdates)
    {
      return component.Bind<Microsoft.VisualStudio.Services.Identity.Identity>(parameterName, rows, this.TableTypeIdentityExtensionTableSchema, this.NameOfTypIdentityextensionTable, new Action<SqlDataRecord, Microsoft.VisualStudio.Services.Identity.Identity>(this.SetIdentityExtenionTableValue));
    }

    protected virtual void SetIdentityExtenionTableValue(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      record.SetGuid(0, identity.MasterId == Guid.Empty ? identity.Id : identity.MasterId);
      if (identity.ResourceVersion > 0)
        record.SetByte(1, (byte) identity.ResourceVersion);
      if (identity.MetaTypeId >= 0 && identity.MetaTypeId < (int) byte.MaxValue)
        record.SetByte(2, (byte) identity.MetaTypeId);
      if (identity.ResourceVersion <= 0)
        return;
      Guid guid1 = identity.Properties.GetValue<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
      if (guid1 != Guid.Empty)
        record.SetGuid(3, guid1);
      HashSet<string> modifiedProperties = identity.GetModifiedProperties();
      if (modifiedProperties == null)
        return;
      if (modifiedProperties.Contains("AuthenticationCredentialValidFrom"))
      {
        long ticks = identity.Properties.GetValue<long>("AuthenticationCredentialValidFrom", 0L);
        DateTime dateTime = ticks == 0L ? new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc) : new DateTime(ticks, DateTimeKind.Utc);
        record.SetDateTime(6, dateTime);
      }
      if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Id"))
      {
        byte[] b = identity.Properties.GetValue<byte[]>("Microsoft.TeamFoundation.Identity.Image.Id", (byte[]) null);
        Guid guid2 = b == null ? Guid.Empty : new Guid(b);
        record.SetGuid(7, guid2);
      }
      if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Type"))
      {
        string str = identity.Properties.GetValue<string>("Microsoft.TeamFoundation.Identity.Image.Type", string.Empty);
        record.SetNullableStringAsEmpty(8, str);
      }
      if (modifiedProperties.Contains("ConfirmedNotificationAddress"))
      {
        string str = identity.Properties.GetValue<string>("ConfirmedNotificationAddress", string.Empty);
        record.SetNullableStringAsEmpty(9, str);
      }
      if (!modifiedProperties.Contains("CustomNotificationAddresses"))
        return;
      string str1 = identity.Properties.GetValue<string>("CustomNotificationAddresses", string.Empty);
      record.SetNullableStringAsEmpty(10, str1);
    }
  }
}
