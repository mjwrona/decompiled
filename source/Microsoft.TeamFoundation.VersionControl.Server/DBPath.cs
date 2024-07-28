// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DBPath
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class DBPath
  {
    private const char dbSeparator = '\\';
    private const string dbSeparatorString = "\\";

    public static string ServerToDatabasePath(string serverPath) => string.IsNullOrEmpty(serverPath) ? (string) null : Microsoft.TeamFoundation.Framework.Server.DBPath.UserToDatabasePath(serverPath, true, true);

    public static string LocalToDatabasePath(string localPath) => string.IsNullOrEmpty(localPath) ? (string) null : Microsoft.TeamFoundation.Framework.Server.DBPath.UserToDatabasePath(localPath, true, false);

    public static string DatabaseToServerPath(string serverPath)
    {
      if (string.IsNullOrEmpty(serverPath))
        return (string) null;
      return string.Equals(serverPath, "$\\", StringComparison.Ordinal) ? "$/" : Microsoft.TeamFoundation.Framework.Server.DBPath.DatabaseToUserPath(serverPath, true, true);
    }

    public static string DatabaseToLocalPath(string dbPath)
    {
      if (string.IsNullOrEmpty(dbPath))
        return (string) null;
      bool removeTrailingSlashIfPresent = dbPath.IndexOf('\\', 0) != dbPath.Length - 1;
      return Microsoft.TeamFoundation.Framework.Server.DBPath.DatabaseToUserPath(dbPath, removeTrailingSlashIfPresent, false);
    }

    public static string DatabaseToLocalOrServerPath(string dbPath) => !VersionControlPath.IsServerItem(dbPath) ? DBPath.DatabaseToLocalPath(dbPath) : DBPath.DatabaseToServerPath(dbPath);
  }
}
