// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SigningKeyColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SigningKeyColumns : ObjectBinder<SigningComponent.SigningServiceKey>
  {
    private SqlColumnBinder PrivateKeyColumn = new SqlColumnBinder("PrivateKey");
    private SqlColumnBinder KeyTypeColumn = new SqlColumnBinder("KeyType");
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");

    protected override SigningComponent.SigningServiceKey Bind()
    {
      byte[] bytes = this.PrivateKeyColumn.GetBytes((IDataReader) this.Reader, false);
      SigningKeyType signingKeyType = SigningKeyType.RSAStored;
      Guid guid = Guid.Empty;
      if (this.IdColumn.ColumnExists((IDataReader) this.Reader))
        guid = this.IdColumn.GetGuid((IDataReader) this.Reader);
      if (this.KeyTypeColumn.ColumnExists((IDataReader) this.Reader))
        signingKeyType = (SigningKeyType) this.KeyTypeColumn.GetByte((IDataReader) this.Reader);
      return new SigningComponent.SigningServiceKey()
      {
        KeyType = signingKeyType,
        KeyData = bytes,
        Identifier = guid
      };
    }
  }
}
