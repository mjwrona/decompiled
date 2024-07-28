// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DBPath
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DBPath
  {
    public static string UserToDatabasePath(string userPath) => DBPath.UserToDatabasePath(userPath, false, false);

    public static string UserToDatabasePath(string userPath, bool appendTrailingSlashIfNotPresent) => DBPath.UserToDatabasePath(userPath, appendTrailingSlashIfNotPresent, false);

    public static unsafe string UserToDatabasePath(
      string userPath,
      bool appendTrailingSlashIfNotPresent,
      bool convertToBackslashes)
    {
      int length = userPath.Length;
      bool flag = false;
      if (appendTrailingSlashIfNotPresent && length > 0)
      {
        char ch = userPath[length - 1];
        flag = true;
        if (ch == '\\' || convertToBackslashes && ch == '/')
          flag = false;
      }
      if (flag)
        ++length;
      string databasePath;
      fixed (char* chPtr = userPath)
        databasePath = new string(chPtr, 0, length);
      string str = databasePath;
      char* chPtr1 = (char*) str;
      if ((IntPtr) chPtr1 != IntPtr.Zero)
        chPtr1 += RuntimeHelpers.OffsetToStringData;
      for (int index = 0; index < length; ++index)
      {
        switch (chPtr1[index])
        {
          case '%':
            chPtr1[index] = '|';
            break;
          case '\'':
            chPtr1[index] = '\v';
            break;
          case '-':
            chPtr1[index] = '"';
            break;
          case '/':
            if (convertToBackslashes)
            {
              chPtr1[index] = '\\';
              break;
            }
            break;
          case '[':
            chPtr1[index] = '<';
            break;
          case '_':
            chPtr1[index] = '>';
            break;
        }
      }
      if (flag)
        chPtr1[length - 1] = '\\';
      str = (string) null;
      return databasePath;
    }

    public static string DatabaseToUserPath(string databasePath) => DBPath.DatabaseToUserPath(databasePath, false, false);

    public static string DatabaseToUserPath(string databasePath, bool removeTrailingSlashIfPresent) => DBPath.DatabaseToUserPath(databasePath, removeTrailingSlashIfPresent, false);

    public static unsafe string DatabaseToUserPath(
      string databasePath,
      bool removeTrailingSlashIfPresent,
      bool convertToForwardSlashes)
    {
      int length = databasePath.Length;
      if (removeTrailingSlashIfPresent && length > 0 && databasePath[length - 1] == '\\' && (length != 3 || !char.IsLetter(databasePath[0]) || databasePath[1] != ':'))
        --length;
      string userPath;
      fixed (char* chPtr = databasePath)
        userPath = new string(chPtr, 0, length);
      string str = userPath;
      char* chPtr1 = (char*) str;
      if ((IntPtr) chPtr1 != IntPtr.Zero)
        chPtr1 += RuntimeHelpers.OffsetToStringData;
      for (int index = 0; index < length; ++index)
      {
        switch (chPtr1[index])
        {
          case '\v':
            chPtr1[index] = '\'';
            break;
          case '"':
            chPtr1[index] = '-';
            break;
          case '<':
            chPtr1[index] = '[';
            break;
          case '>':
            chPtr1[index] = '_';
            break;
          case '\\':
            if (convertToForwardSlashes)
            {
              chPtr1[index] = '/';
              break;
            }
            break;
          case '|':
            chPtr1[index] = '%';
            break;
        }
      }
      str = (string) null;
      return userPath;
    }
  }
}
