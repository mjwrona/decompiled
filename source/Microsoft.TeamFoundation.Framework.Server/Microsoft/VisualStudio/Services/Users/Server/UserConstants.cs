// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.UserConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  public static class UserConstants
  {
    public static readonly Guid UserServicePrincipalId = new Guid("00000038-0000-8888-8000-000000000000");
    public static readonly Guid UserSecurityNamespaceId = new Guid("140709a6-4d6e-4e13-bea4-8ecbf3d20435");
    public const string UserStorageKeyNamespace = "UserStorageKey";
    public const string RouteKeyRoutingData = "RouteKeyRoutingData";
  }
}
