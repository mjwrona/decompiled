// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.ITenantPolicyPermissionExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  [InheritedExport]
  public interface ITenantPolicyPermissionExtension
  {
    bool HasPermission(
      IVssRequestContext requestContext,
      string policyName,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      TenantPolicyPermissionType permissionType);

    bool HasPermission(
      IVssRequestContext requestContext,
      string policyName,
      Guid identityObjectId,
      Guid targetTenantId,
      TenantPolicyPermissionType permissionType);

    bool HasPermission(
      IVssRequestContext requestContext,
      string policyName,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid identityObjectId,
      Guid targetTenantId,
      TenantPolicyPermissionType permissionType);
  }
}
