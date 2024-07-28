// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.SharedAccessTablePolicy
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Text;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class SharedAccessTablePolicy
  {
    public DateTimeOffset? SharedAccessStartTime { get; set; }

    public DateTimeOffset? SharedAccessExpiryTime { get; set; }

    public SharedAccessTablePermissions Permissions { get; set; }

    public static string PermissionsToString(SharedAccessTablePermissions permissions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((permissions & SharedAccessTablePermissions.Query) == SharedAccessTablePermissions.Query)
        stringBuilder.Append("r");
      if ((permissions & SharedAccessTablePermissions.Add) == SharedAccessTablePermissions.Add)
        stringBuilder.Append("a");
      if ((permissions & SharedAccessTablePermissions.Update) == SharedAccessTablePermissions.Update)
        stringBuilder.Append("u");
      if ((permissions & SharedAccessTablePermissions.Delete) == SharedAccessTablePermissions.Delete)
        stringBuilder.Append("d");
      return stringBuilder.ToString();
    }

    public static SharedAccessTablePermissions PermissionsFromString(string input)
    {
      CommonUtility.AssertNotNull(nameof (input), (object) input);
      SharedAccessTablePermissions tablePermissions = SharedAccessTablePermissions.None;
      foreach (char ch in input)
      {
        switch (ch)
        {
          case 'a':
            tablePermissions |= SharedAccessTablePermissions.Add;
            break;
          case 'd':
            tablePermissions |= SharedAccessTablePermissions.Delete;
            break;
          case 'r':
            tablePermissions |= SharedAccessTablePermissions.Query;
            break;
          case 'u':
            tablePermissions |= SharedAccessTablePermissions.Update;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (input));
        }
      }
      if (tablePermissions == SharedAccessTablePermissions.None)
        tablePermissions = tablePermissions;
      return tablePermissions;
    }
  }
}
