// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent8
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent8 : StrongBoxComponent7
  {
    public override void AddStrongBoxItem(Guid drawerId, StrongBoxItemInfo item)
    {
      this.PrepareStoredProcedure("prc_AddStrongBoxItem");
      this.BindGuid("@drawerId", item.DrawerId);
      this.BindString("@lookupKey", item.LookupKey, 512, false, SqlDbType.NVarChar);
      this.BindByte("@itemKind", (byte) item.ItemKind);
      this.BindNullableDateTime("@expirationDate", item.ExpirationDate);
      this.BindString("@credentialName", item.CredentialName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@signingKeyId", item.SigningKeyId);
      if (item.EncryptedContent == null)
        this.BindNullValue("@encryptedContent", SqlDbType.VarBinary);
      else
        this.BindBinary("@encryptedContent", item.EncryptedContent, SqlDbType.VarBinary);
      if (item.SecureFileId > 0)
        this.BindInt("@secureFileId", item.SecureFileId);
      else
        this.BindNullValue("@secureFileId", SqlDbType.Int);
      this.ExecuteNonQuery();
    }

    public override void AddStrongBoxItems(Guid drawerId, IEnumerable<StrongBoxItemInfo> items)
    {
      this.PrepareStoredProcedure("prc_AddStrongBoxItems");
      this.BindGuid("@drawerId", drawerId);
      this.BindStrongBoxItemInfoTable2("@items", items);
      this.ExecuteNonQuery();
    }
  }
}
