// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Aad.IAadTenantDetailProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Framework.Server.Aad
{
  [InheritedExport]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IAadTenantDetailProvider
  {
    bool CanHandleRequest(IVssRequestContext context);

    string GetDisplayName(IVssRequestContext context);

    string GetDisplayName(IVssRequestContext context, string tenantId);

    IEnumerable<string> GetVerifiedDomains(IVssRequestContext context, string tenantId);

    bool IsUserMemberOfAadGroups(
      IVssRequestContext requestContext,
      Guid aadUserId,
      List<Guid> aadGroupIds);

    bool IsServicePrincipalMemberOfAadGroups(
      IVssRequestContext requestContext,
      Guid aadUserId,
      List<Guid> aadGroupIds);

    AadGroup GetGroupFromAad(IVssRequestContext requestContext, Guid aadGroupId);

    List<IdentityDescriptor> GetGroupsDescendants(
      IVssRequestContext requestContext,
      List<IdentityDescriptor> aadGroupDescriptors);
  }
}
