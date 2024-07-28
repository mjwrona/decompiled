// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseCredentialBinder2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseCredentialBinder2 : DatabaseCredentialBinder
  {
    protected SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
    protected SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
    protected SqlColumnBinder SigningKeyId = new SqlColumnBinder(nameof (SigningKeyId));

    protected override TeamFoundationDatabaseCredential Bind()
    {
      TeamFoundationDatabaseCredential databaseCredential = base.Bind();
      databaseCredential.Name = this.Name.GetString((IDataReader) this.Reader, true);
      databaseCredential.Description = this.Description.GetString((IDataReader) this.Reader, true);
      if (this.SigningKeyId.ColumnExists((IDataReader) this.Reader))
        databaseCredential.SigningKeyId = this.SigningKeyId.GetGuid((IDataReader) this.Reader);
      databaseCredential.Updated();
      return databaseCredential;
    }
  }
}
