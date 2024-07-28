// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DrawerInfoColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DrawerInfoColumns : 
    ObjectBinder<(string Name, Guid DrawerId, Guid SigningKeyId, DateTime LastRotateDate)>
  {
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder DrawerIdColumn = new SqlColumnBinder("DrawerId");
    private SqlColumnBinder SigningKeyIdColumn = new SqlColumnBinder("SigningKeyId");
    private SqlColumnBinder LastRotateDate = new SqlColumnBinder(nameof (LastRotateDate));

    protected override (string Name, Guid DrawerId, Guid SigningKeyId, DateTime LastRotateDate) Bind()
    {
      string str = this.NameColumn.GetString((IDataReader) this.Reader, false);
      Guid guid1 = this.DrawerIdColumn.GetGuid((IDataReader) this.Reader, false);
      Guid guid2 = this.SigningKeyIdColumn.GetGuid((IDataReader) this.Reader, true);
      DateTime dateTime1 = this.LastRotateDate.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      Guid guid3 = guid1;
      Guid guid4 = guid2;
      DateTime dateTime2 = dateTime1;
      return (str, guid3, guid4, dateTime2);
    }
  }
}
