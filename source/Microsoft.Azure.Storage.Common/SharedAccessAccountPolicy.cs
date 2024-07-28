// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.SharedAccessAccountPolicy
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Text;

namespace Microsoft.Azure.Storage
{
  public sealed class SharedAccessAccountPolicy
  {
    public DateTimeOffset? SharedAccessStartTime { get; set; }

    public DateTimeOffset? SharedAccessExpiryTime { get; set; }

    public SharedAccessAccountPermissions Permissions { get; set; }

    public SharedAccessAccountServices Services { get; set; }

    public SharedAccessAccountResourceTypes ResourceTypes { get; set; }

    public SharedAccessProtocol? Protocols { get; set; }

    public IPAddressOrRange IPAddressOrRange { get; set; }

    public static string PermissionsToString(SharedAccessAccountPermissions permissions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((permissions & SharedAccessAccountPermissions.Read) == SharedAccessAccountPermissions.Read)
        stringBuilder.Append("r");
      if ((permissions & SharedAccessAccountPermissions.Add) == SharedAccessAccountPermissions.Add)
        stringBuilder.Append("a");
      if ((permissions & SharedAccessAccountPermissions.Create) == SharedAccessAccountPermissions.Create)
        stringBuilder.Append("c");
      if ((permissions & SharedAccessAccountPermissions.Update) == SharedAccessAccountPermissions.Update)
        stringBuilder.Append("u");
      if ((permissions & SharedAccessAccountPermissions.ProcessMessages) == SharedAccessAccountPermissions.ProcessMessages)
        stringBuilder.Append("p");
      if ((permissions & SharedAccessAccountPermissions.Write) == SharedAccessAccountPermissions.Write)
        stringBuilder.Append("w");
      if ((permissions & SharedAccessAccountPermissions.Delete) == SharedAccessAccountPermissions.Delete)
        stringBuilder.Append("d");
      if ((permissions & SharedAccessAccountPermissions.List) == SharedAccessAccountPermissions.List)
        stringBuilder.Append("l");
      return stringBuilder.ToString();
    }

    public static string ServicesToString(SharedAccessAccountServices services)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((services & SharedAccessAccountServices.Blob) == SharedAccessAccountServices.Blob)
        stringBuilder.Append("b");
      if ((services & SharedAccessAccountServices.File) == SharedAccessAccountServices.File)
        stringBuilder.Append("f");
      if ((services & SharedAccessAccountServices.Queue) == SharedAccessAccountServices.Queue)
        stringBuilder.Append("q");
      if ((services & SharedAccessAccountServices.Table) == SharedAccessAccountServices.Table)
        stringBuilder.Append("t");
      return stringBuilder.ToString();
    }

    public static string ResourceTypesToString(SharedAccessAccountResourceTypes resourceTypes)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((resourceTypes & SharedAccessAccountResourceTypes.Service) == SharedAccessAccountResourceTypes.Service)
        stringBuilder.Append("s");
      if ((resourceTypes & SharedAccessAccountResourceTypes.Container) == SharedAccessAccountResourceTypes.Container)
        stringBuilder.Append("c");
      if ((resourceTypes & SharedAccessAccountResourceTypes.Object) == SharedAccessAccountResourceTypes.Object)
        stringBuilder.Append("o");
      return stringBuilder.ToString();
    }
  }
}
