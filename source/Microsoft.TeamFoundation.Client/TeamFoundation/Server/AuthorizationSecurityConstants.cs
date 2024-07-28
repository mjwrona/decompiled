// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AuthorizationSecurityConstants
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Server
{
  public static class AuthorizationSecurityConstants
  {
    public static readonly Guid NamespaceSecurityGuid = FrameworkSecurity.TeamProjectCollectionNamespaceId;
    public static readonly Guid ProjectSecurityGuid = FrameworkSecurity.TeamProjectNamespaceId;
    public static readonly Guid CommonStructureNodeSecurityGuid = new Guid("83E28AD4-2D72-4ceb-97B0-C7726D5502C3");
    public static readonly Guid IterationNodeSecurityGuid = new Guid("BF7BFA03-B2B7-47db-8113-FA2E002CC5B1");
    public static readonly string NamespaceSecurityToken = FrameworkSecurity.TeamProjectCollectionNamespaceToken;
    public static readonly string NamespaceSecurityObjectId = PermissionNamespaces.Global;
    public static readonly string ProjectSecurityPrefix = PermissionNamespaces.Project;
    public static readonly char SeparatorChar = ':';
  }
}
