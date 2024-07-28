// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspacePermissions
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal static class WorkspacePermissions
  {
    public const int Read = 1;
    public const int Use = 2;
    public const int CheckIn = 4;
    public const int Administer = 8;

    public static string WorkspacePermissionToString(int permission)
    {
      switch (permission)
      {
        case 1:
        case 2:
        case 4:
        case 8:
          return ((WorkspacePermissions.WorkspacePermissionsEnum) permission).ToString();
        default:
          return permission.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    private enum WorkspacePermissionsEnum
    {
      Read = 1,
      Use = 2,
      CheckIn = 4,
      Administer = 8,
    }
  }
}
