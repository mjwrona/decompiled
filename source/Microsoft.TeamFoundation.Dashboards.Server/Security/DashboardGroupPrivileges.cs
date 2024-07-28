// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Security.DashboardGroupPrivileges
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Security
{
  public class DashboardGroupPrivileges
  {
    public static readonly Guid NamespaceId = new Guid("4E68ADD0-BE79-4686-B64A-CD2FB570A562");
    public static readonly char NamespaceSeparator = '/';
    public static readonly string Root = "$/DashboardGroup";

    [GenerateAllConstants(null)]
    public class Permissions
    {
      public const int Read = 1;
      public const int Edit = 2;
      public const int Manage = 4;
      public const int ManagePermissions = 8;

      public static int UserToNamespacePermission(GroupMemberPermission groupPermission)
      {
        switch (groupPermission)
        {
          case GroupMemberPermission.None:
            return 1;
          case GroupMemberPermission.Edit:
            return 3;
          case GroupMemberPermission.Manage:
            return 7;
          case GroupMemberPermission.ManagePermissions:
            return 15;
          default:
            return 1;
        }
      }

      public static GroupMemberPermission NamespaceToUserPermission(int namespacePermission)
      {
        switch (namespacePermission)
        {
          case 1:
            return GroupMemberPermission.None;
          case 2:
          case 3:
            return GroupMemberPermission.Edit;
          case 4:
          case 6:
          case 7:
            return GroupMemberPermission.Manage;
          case 8:
          case 12:
          case 14:
          case 15:
            return GroupMemberPermission.ManagePermissions;
          default:
            return GroupMemberPermission.None;
        }
      }
    }
  }
}
