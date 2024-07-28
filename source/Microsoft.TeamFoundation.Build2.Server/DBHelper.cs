// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DBHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class DBHelper
  {
    private static readonly DateTime s_minAllowedDateTime = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime s_maxAllowedDateTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

    internal static DateTime GetDateTime(DateTime value)
    {
      if (value < DBHelper.s_minAllowedDateTime)
        value = DBHelper.s_minAllowedDateTime;
      if (value > DBHelper.s_maxAllowedDateTime)
        value = DBHelper.s_maxAllowedDateTime;
      return value;
    }

    internal static DateTime? GetDateTime(DateTime? value) => !value.HasValue ? new DateTime?() : new DateTime?(DBHelper.GetDateTime(value.Value));

    internal static DateTime GetUtcDateTime(DateTime value)
    {
      if (value.Kind == DateTimeKind.Unspecified)
        value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
      value = value.ToUniversalTime();
      if (value < DBHelper.s_minAllowedDateTime)
        value = DBHelper.s_minAllowedDateTime;
      if (value > DBHelper.s_maxAllowedDateTime)
        value = DBHelper.s_maxAllowedDateTime;
      return value;
    }

    internal static DateTime? GetUtcDateTime(DateTime? value) => !value.HasValue ? new DateTime?() : new DateTime?(DBHelper.GetUtcDateTime(value.Value));

    internal static string DBPathToServerPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (string) null;
      bool removeTrailingSlashIfPresent = true;
      if (path == "\\")
        removeTrailingSlashIfPresent = false;
      return DBPath.DatabaseToUserPath(path, removeTrailingSlashIfPresent, false);
    }

    internal static string ServerPathToDBPath(string path) => string.IsNullOrEmpty(path) ? (string) null : DBPath.UserToDatabasePath(path, true, false);

    internal static string UserToDBPath(string path) => string.IsNullOrEmpty(path) ? (string) null : DBPath.UserToDatabasePath(path, true);
  }
}
