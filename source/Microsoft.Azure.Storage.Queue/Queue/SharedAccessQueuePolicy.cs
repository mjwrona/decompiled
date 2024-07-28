// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.SharedAccessQueuePolicy
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Text;

namespace Microsoft.Azure.Storage.Queue
{
  public sealed class SharedAccessQueuePolicy
  {
    public DateTimeOffset? SharedAccessStartTime { get; set; }

    public DateTimeOffset? SharedAccessExpiryTime { get; set; }

    public SharedAccessQueuePermissions Permissions { get; set; }

    public static string PermissionsToString(SharedAccessQueuePermissions permissions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((permissions & SharedAccessQueuePermissions.Read) == SharedAccessQueuePermissions.Read)
        stringBuilder.Append("r");
      if ((permissions & SharedAccessQueuePermissions.Add) == SharedAccessQueuePermissions.Add)
        stringBuilder.Append("a");
      if ((permissions & SharedAccessQueuePermissions.Update) == SharedAccessQueuePermissions.Update)
        stringBuilder.Append("u");
      if ((permissions & SharedAccessQueuePermissions.ProcessMessages) == SharedAccessQueuePermissions.ProcessMessages)
        stringBuilder.Append("p");
      return stringBuilder.ToString();
    }

    public static SharedAccessQueuePermissions PermissionsFromString(string input)
    {
      CommonUtility.AssertNotNull(nameof (input), (object) input);
      SharedAccessQueuePermissions queuePermissions = SharedAccessQueuePermissions.None;
      foreach (char ch in input)
      {
        switch (ch)
        {
          case 'a':
            queuePermissions |= SharedAccessQueuePermissions.Add;
            break;
          case 'p':
            queuePermissions |= SharedAccessQueuePermissions.ProcessMessages;
            break;
          case 'r':
            queuePermissions |= SharedAccessQueuePermissions.Read;
            break;
          case 'u':
            queuePermissions |= SharedAccessQueuePermissions.Update;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof (input));
        }
      }
      if (queuePermissions == SharedAccessQueuePermissions.None)
        queuePermissions |= SharedAccessQueuePermissions.None;
      return queuePermissions;
    }
  }
}
