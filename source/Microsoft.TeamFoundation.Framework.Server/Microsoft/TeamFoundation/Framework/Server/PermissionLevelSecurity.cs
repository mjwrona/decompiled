// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PermissionLevelSecurity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class PermissionLevelSecurity
  {
    public static readonly Guid NamespaceId = new Guid("25FB0ED7-EB8F-42B8-9A5E-836A25F67E37");
    public static readonly string RootToken = "/";
    public static readonly char PathSeparator = '/';
    public static readonly string DefinitionsToken = PermissionLevelSecurity.RootToken + "Definitions";
    public static readonly string AssignmentsToken = PermissionLevelSecurity.RootToken + "Assignments";

    public static class Permissions
    {
      public const int Read = 1;
      public const int Create = 2;
      public const int Update = 4;
      public const int Delete = 8;
      public const int All = 15;
    }
  }
}
