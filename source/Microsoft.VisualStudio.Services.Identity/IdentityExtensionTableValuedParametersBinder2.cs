// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityExtensionTableValuedParametersBinder2
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityExtensionTableValuedParametersBinder2 : 
    IdentityExtensionTableValuedParametersBinder
  {
    private static readonly SqlMetaData[] IdentityExtensionTableSchema2 = new SqlMetaData[9]
    {
      new SqlMetaData("IdentityId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ResourceVersion", SqlDbType.TinyInt),
      new SqlMetaData("MetaTypeId", SqlDbType.TinyInt),
      new SqlMetaData("ExternalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AuthenticationCredentialValidFrom", SqlDbType.DateTime),
      new SqlMetaData("ImageId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ImageType", SqlDbType.VarChar, 500L),
      new SqlMetaData("ConfirmedNotificationAddress", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CustomNotificationAddresses", SqlDbType.NVarChar, 4000L)
    };

    public override string NameOfTypIdentityextensionTable => "typ_IdentityExtensionTable2";

    public override SqlMetaData[] TableTypeIdentityExtensionTableSchema => IdentityExtensionTableValuedParametersBinder2.IdentityExtensionTableSchema2;

    public virtual Dictionary<string, int> IdentityExtensionTableSchemaColumnOrdinalMapping => this.GenerateIdentityExtensionTableSchemaColumnOrdinalMapping();

    protected virtual Dictionary<string, int> GenerateIdentityExtensionTableSchemaColumnOrdinalMapping()
    {
      Dictionary<string, int> columnOrdinalMapping = new Dictionary<string, int>();
      for (int index = 0; index < this.TableTypeIdentityExtensionTableSchema.Length; ++index)
        columnOrdinalMapping.Add(this.TableTypeIdentityExtensionTableSchema[index].Name, index);
      return columnOrdinalMapping;
    }

    protected override void SetIdentityExtenionTableValue(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      record.SetGuid(this.IdentityExtensionTableSchemaColumnOrdinalMapping["IdentityId"], identity.MasterId == Guid.Empty ? identity.Id : identity.MasterId);
      if (identity.ResourceVersion > 0)
        record.SetByte(this.IdentityExtensionTableSchemaColumnOrdinalMapping["ResourceVersion"], (byte) identity.ResourceVersion);
      if (identity.MetaTypeId >= 0 && identity.MetaTypeId < (int) byte.MaxValue)
        record.SetByte(this.IdentityExtensionTableSchemaColumnOrdinalMapping["MetaTypeId"], (byte) identity.MetaTypeId);
      if (identity.ResourceVersion <= 0)
        return;
      Guid guid1 = identity.Properties.GetValue<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
      if (guid1 != Guid.Empty || identity.Properties.ContainsKey("http://schemas.microsoft.com/identity/claims/objectidentifier"))
        record.SetGuid(this.IdentityExtensionTableSchemaColumnOrdinalMapping["ExternalId"], guid1);
      HashSet<string> modifiedProperties = identity.GetModifiedProperties();
      if (modifiedProperties == null)
        return;
      if (modifiedProperties.Contains("AuthenticationCredentialValidFrom"))
      {
        long ticks = identity.Properties.GetValue<long>("AuthenticationCredentialValidFrom", 0L);
        DateTime dateTime = ticks == 0L ? new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc) : new DateTime(ticks, DateTimeKind.Utc);
        record.SetDateTime(this.IdentityExtensionTableSchemaColumnOrdinalMapping["AuthenticationCredentialValidFrom"], dateTime);
      }
      if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Id"))
      {
        byte[] b = identity.Properties.GetValue<byte[]>("Microsoft.TeamFoundation.Identity.Image.Id", (byte[]) null);
        Guid guid2 = b == null ? Guid.Empty : new Guid(b);
        record.SetGuid(this.IdentityExtensionTableSchemaColumnOrdinalMapping["ImageId"], guid2);
      }
      if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Type"))
      {
        string str = identity.Properties.GetValue<string>("Microsoft.TeamFoundation.Identity.Image.Type", string.Empty);
        record.SetNullableStringAsEmpty(this.IdentityExtensionTableSchemaColumnOrdinalMapping["ImageType"], str);
      }
      if (modifiedProperties.Contains("ConfirmedNotificationAddress"))
      {
        string str = identity.Properties.GetValue<string>("ConfirmedNotificationAddress", string.Empty);
        record.SetNullableStringAsEmpty(this.IdentityExtensionTableSchemaColumnOrdinalMapping["ConfirmedNotificationAddress"], str);
      }
      if (!modifiedProperties.Contains("CustomNotificationAddresses"))
        return;
      string str1 = identity.Properties.GetValue<string>("CustomNotificationAddresses", string.Empty);
      record.SetNullableStringAsEmpty(this.IdentityExtensionTableSchemaColumnOrdinalMapping["CustomNotificationAddresses"], str1);
    }
  }
}
