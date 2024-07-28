// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.SharedAccessFilePolicy
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Text;

namespace Microsoft.Azure.Storage.File
{
  public sealed class SharedAccessFilePolicy
  {
    public DateTimeOffset? SharedAccessStartTime { get; set; }

    public DateTimeOffset? SharedAccessExpiryTime { get; set; }

    public SharedAccessFilePermissions Permissions { get; set; }

    public static string PermissionsToString(SharedAccessFilePermissions permissions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((permissions & SharedAccessFilePermissions.Read) == SharedAccessFilePermissions.Read)
        stringBuilder.Append("r");
      if ((permissions & SharedAccessFilePermissions.Create) == SharedAccessFilePermissions.Create)
        stringBuilder.Append("c");
      if ((permissions & SharedAccessFilePermissions.Write) == SharedAccessFilePermissions.Write)
        stringBuilder.Append("w");
      if ((permissions & SharedAccessFilePermissions.Delete) == SharedAccessFilePermissions.Delete)
        stringBuilder.Append("d");
      if ((permissions & SharedAccessFilePermissions.List) == SharedAccessFilePermissions.List)
        stringBuilder.Append("l");
      return stringBuilder.ToString();
    }

    public static SharedAccessFilePermissions PermissionsFromString(string input)
    {
      CommonUtility.AssertNotNull(nameof (input), (object) input);
      SharedAccessFilePermissions accessFilePermissions = SharedAccessFilePermissions.None;
      foreach (char ch in input)
      {
        switch (ch)
        {
          case 'c':
            accessFilePermissions |= SharedAccessFilePermissions.Create;
            break;
          case 'd':
            accessFilePermissions |= SharedAccessFilePermissions.Delete;
            break;
          case 'l':
            accessFilePermissions |= SharedAccessFilePermissions.List;
            break;
          case 'r':
            accessFilePermissions |= SharedAccessFilePermissions.Read;
            break;
          case 'w':
            accessFilePermissions |= SharedAccessFilePermissions.Write;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (input));
        }
      }
      if (accessFilePermissions == SharedAccessFilePermissions.None)
        accessFilePermissions |= SharedAccessFilePermissions.None;
      return accessFilePermissions;
    }
  }
}
