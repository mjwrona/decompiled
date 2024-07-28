// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.AuthorizationSecurityConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
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
