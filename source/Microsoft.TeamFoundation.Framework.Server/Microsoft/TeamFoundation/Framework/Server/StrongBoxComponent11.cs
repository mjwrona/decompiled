// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent11 : StrongBoxComponent10
  {
    public override bool ReplaceItem(
      StrongBoxItemInfo item,
      byte[] prevEncryptedContent,
      int prevFileId)
    {
      this.PrepareStoredProcedure("prc_ReplaceStrongBoxItem");
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
      if (prevEncryptedContent == null)
        this.BindNullValue("@prevEncryptedContent", SqlDbType.VarBinary);
      else
        this.BindBinary("@prevEncryptedContent", prevEncryptedContent, SqlDbType.VarBinary);
      if (prevFileId > 0)
        this.BindInt("@prevFileId", prevFileId);
      else
        this.BindNullValue("@prevFileId", SqlDbType.Int);
      return (int) this.ExecuteScalar() == 1;
    }
  }
}
