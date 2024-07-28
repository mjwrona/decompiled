// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.SqlColumnBinderExtensions
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal static class SqlColumnBinderExtensions
  {
    public static LockLevel GetLockLevel(ref this SqlColumnBinder binder, SqlDataReader reader) => (LockLevel) binder.GetByte((IDataReader) reader, (byte) 0);

    public static string GetServerItem(
      ref this SqlColumnBinder binder,
      SqlDataReader reader,
      bool allowNulls)
    {
      return DBPath.DatabaseToServerPath(binder.GetString((IDataReader) reader, allowNulls));
    }

    public static string GetLocalItem(
      ref this SqlColumnBinder binder,
      SqlDataReader reader,
      bool allowNulls)
    {
      return DBPath.DatabaseToLocalPath(binder.GetString((IDataReader) reader, allowNulls));
    }
  }
}
