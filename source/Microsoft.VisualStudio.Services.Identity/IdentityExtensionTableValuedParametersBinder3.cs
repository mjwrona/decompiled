// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityExtensionTableValuedParametersBinder3
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityExtensionTableValuedParametersBinder3 : 
    IdentityExtensionTableValuedParametersBinder2
  {
    private static readonly SqlMetaData[] IdentityExtensionTableSchema3 = new SqlMetaData[10]
    {
      new SqlMetaData("IdentityId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ResourceVersion", SqlDbType.TinyInt),
      new SqlMetaData("MetaTypeId", SqlDbType.TinyInt),
      new SqlMetaData("ExternalId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AuthenticationCredentialValidFrom", SqlDbType.DateTime),
      new SqlMetaData("ImageId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ImageType", SqlDbType.VarChar, 500L),
      new SqlMetaData("ConfirmedNotificationAddress", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CustomNotificationAddresses", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ApplicationId", SqlDbType.UniqueIdentifier)
    };

    public override string NameOfTypIdentityextensionTable => "typ_IdentityExtensionTable3";

    public override SqlMetaData[] TableTypeIdentityExtensionTableSchema => IdentityExtensionTableValuedParametersBinder3.IdentityExtensionTableSchema3;

    protected override void SetIdentityExtenionTableValue(SqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      base.SetIdentityExtenionTableValue(record, identity);
      HashSet<string> modifiedProperties = identity.GetModifiedProperties();
      if (modifiedProperties == null || !modifiedProperties.Contains("ApplicationId"))
        return;
      Guid guid = identity.Properties.GetValue<Guid>("ApplicationId", Guid.Empty);
      record.SetGuid(this.IdentityExtensionTableSchemaColumnOrdinalMapping["ApplicationId"], guid);
    }
  }
}
