// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PublicCollectionRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PublicCollectionRequestRestrictionsAttribute : 
    PublicBaseRequestRestrictionsAttribute
  {
    public PublicCollectionRequestRestrictionsAttribute(
      bool s2sOnly = false,
      bool enforceDataspaceRestrictionsForMembers = true,
      string minApiVersion = null)
      : base(s2sOnly, enforceDataspaceRestrictionsForMembers, minApiVersion)
    {
    }

    public PublicCollectionRequestRestrictionsAttribute(
      bool s2sOnly,
      bool enforceDataspaceRestrictionsForMembers,
      string minApiVersion,
      RequiredAuthentication requiredAuthentication,
      bool allowNonSsl,
      bool allowCors,
      AuthenticationMechanisms mechanisms,
      string description,
      UserAgentFilterType agentFilterType,
      string agentFilter)
      : base(s2sOnly, enforceDataspaceRestrictionsForMembers, minApiVersion, requiredAuthentication, allowNonSsl, allowCors, mechanisms, description, agentFilterType, agentFilter, TeamFoundationHostType.ProjectCollection)
    {
    }

    public override AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return new AllowPublicAccessResult(true, true, Guid.Empty);
    }
  }
}
