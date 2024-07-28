// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.SharedAccessBlobPolicy
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Text;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class SharedAccessBlobPolicy
  {
    public DateTimeOffset? SharedAccessStartTime { get; set; }

    public DateTimeOffset? SharedAccessExpiryTime { get; set; }

    public SharedAccessBlobPermissions Permissions { get; set; }

    public static string PermissionsToString(SharedAccessBlobPermissions permissions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((permissions & SharedAccessBlobPermissions.Read) == SharedAccessBlobPermissions.Read)
        stringBuilder.Append("r");
      if ((permissions & SharedAccessBlobPermissions.Add) == SharedAccessBlobPermissions.Add)
        stringBuilder.Append("a");
      if ((permissions & SharedAccessBlobPermissions.Create) == SharedAccessBlobPermissions.Create)
        stringBuilder.Append("c");
      if ((permissions & SharedAccessBlobPermissions.Write) == SharedAccessBlobPermissions.Write)
        stringBuilder.Append("w");
      if ((permissions & SharedAccessBlobPermissions.Delete) == SharedAccessBlobPermissions.Delete)
        stringBuilder.Append("d");
      if ((permissions & SharedAccessBlobPermissions.List) == SharedAccessBlobPermissions.List)
        stringBuilder.Append("l");
      return stringBuilder.ToString();
    }

    public static SharedAccessBlobPermissions PermissionsFromString(string input)
    {
      CommonUtility.AssertNotNull(nameof (input), (object) input);
      SharedAccessBlobPermissions accessBlobPermissions = SharedAccessBlobPermissions.None;
      foreach (char ch in input)
      {
        switch (ch)
        {
          case 'a':
            accessBlobPermissions |= SharedAccessBlobPermissions.Add;
            break;
          case 'c':
            accessBlobPermissions |= SharedAccessBlobPermissions.Create;
            break;
          case 'd':
            accessBlobPermissions |= SharedAccessBlobPermissions.Delete;
            break;
          case 'l':
            accessBlobPermissions |= SharedAccessBlobPermissions.List;
            break;
          case 'r':
            accessBlobPermissions |= SharedAccessBlobPermissions.Read;
            break;
          case 'w':
            accessBlobPermissions |= SharedAccessBlobPermissions.Write;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (input));
        }
      }
      if (accessBlobPermissions == SharedAccessBlobPermissions.None)
        accessBlobPermissions |= SharedAccessBlobPermissions.None;
      return accessBlobPermissions;
    }
  }
}
