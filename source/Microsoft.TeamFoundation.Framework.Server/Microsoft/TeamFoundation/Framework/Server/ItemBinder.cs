// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ItemBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ItemBinder : ObjectBinder<StrongBoxItemInfo>
  {
    private SqlColumnBinder DrawerIdColumn = new SqlColumnBinder("DrawerId");
    private SqlColumnBinder LookupKeyColumn = new SqlColumnBinder("LookupKey");
    private SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
    private SqlColumnBinder ItemKindColumn = new SqlColumnBinder("ItemKind");
    private SqlColumnBinder ValueColumn = new SqlColumnBinder("Value");
    private SqlColumnBinder ExpirationDateColumn = new SqlColumnBinder("ExpirationDate");
    private SqlColumnBinder CredentialNameColumn = new SqlColumnBinder("CredentialName");
    private SqlColumnBinder SigningKeyIdColumn = new SqlColumnBinder("SigningKeyId");
    private SqlColumnBinder EncryptedContentColumn = new SqlColumnBinder("EncryptedContent");
    private SqlColumnBinder SecureFileIdColumn = new SqlColumnBinder("SecureFileId");
    private SqlColumnBinder LastUpdateTimeColumn = new SqlColumnBinder("LastUpdateTime");

    protected override StrongBoxItemInfo Bind()
    {
      StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo();
      strongBoxItemInfo.ItemKind = (StrongBoxItemKind) this.ItemKindColumn.GetByte((IDataReader) this.Reader);
      strongBoxItemInfo.DrawerId = this.DrawerIdColumn.GetGuid((IDataReader) this.Reader);
      strongBoxItemInfo.LookupKey = this.LookupKeyColumn.GetString((IDataReader) this.Reader, false);
      strongBoxItemInfo.FileId = this.FileIdColumn.GetInt32((IDataReader) this.Reader, -1);
      strongBoxItemInfo.Value = this.ValueColumn.GetString((IDataReader) this.Reader, true);
      if (this.ExpirationDateColumn.ColumnExists((IDataReader) this.Reader) && !this.ExpirationDateColumn.IsNull((IDataReader) this.Reader))
        strongBoxItemInfo.ExpirationDate = new DateTime?(this.ExpirationDateColumn.GetDateTime((IDataReader) this.Reader));
      if (this.CredentialNameColumn.ColumnExists((IDataReader) this.Reader))
        strongBoxItemInfo.CredentialName = this.CredentialNameColumn.GetString((IDataReader) this.Reader, true);
      if (this.SigningKeyIdColumn.ColumnExists((IDataReader) this.Reader))
        strongBoxItemInfo.SigningKeyId = this.SigningKeyIdColumn.GetGuid((IDataReader) this.Reader, true);
      if (strongBoxItemInfo.SigningKeyId == Guid.Empty)
        strongBoxItemInfo.SigningKeyId = strongBoxItemInfo.DrawerId;
      if (this.EncryptedContentColumn.ColumnExists((IDataReader) this.Reader))
        strongBoxItemInfo.EncryptedContent = this.EncryptedContentColumn.GetBytes((IDataReader) this.Reader, true);
      if (this.SecureFileIdColumn.ColumnExists((IDataReader) this.Reader))
        strongBoxItemInfo.SecureFileId = this.SecureFileIdColumn.GetInt32((IDataReader) this.Reader, -1);
      if (this.LastUpdateTimeColumn.ColumnExists((IDataReader) this.Reader) && !this.LastUpdateTimeColumn.IsNull((IDataReader) this.Reader))
        strongBoxItemInfo.LastUpdateTime = new DateTime?(this.LastUpdateTimeColumn.GetDateTime((IDataReader) this.Reader));
      return strongBoxItemInfo;
    }
  }
}
