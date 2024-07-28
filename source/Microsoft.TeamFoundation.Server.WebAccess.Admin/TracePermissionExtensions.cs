// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.TracePermissionExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.AdminEngagement.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public static class TracePermissionExtensions
  {
    public static PermissionTrace ToClientTracePermissionModel(
      this TracePermissionModel tracePermissionModel,
      IVssRequestContext requestContext)
    {
      if (tracePermissionModel == null)
        return (PermissionTrace) null;
      return new PermissionTrace()
      {
        InheritanceType = tracePermissionModel.InheritanceType,
        TokenDisplayName = tracePermissionModel.TokenDisplayName,
        AffectingGroups = tracePermissionModel.AffectingGroups.Select<KeyValuePair<TeamFoundationIdentity, PermissionValue>, AffectingGroup>((Func<KeyValuePair<TeamFoundationIdentity, PermissionValue>, AffectingGroup>) (group => new AffectingGroup()
        {
          Group = group.Key.ToIdentityRef(requestContext),
          PermissionValue = group.Value
        })).ToList<AffectingGroup>(),
        Error = tracePermissionModel.Error
      };
    }
  }
}
