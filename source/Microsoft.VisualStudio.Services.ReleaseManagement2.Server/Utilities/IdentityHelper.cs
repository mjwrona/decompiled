// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public class IdentityHelper
  {
    private IDictionary<string, IdentityRef> teamFoundationIdentities;

    protected IdentityHelper()
    {
    }

    protected IdentityHelper(IDictionary<string, IdentityRef> teamFoundationIds) => this.teamFoundationIdentities = teamFoundationIds;

    public static IdentityHelper GetIdentityHelper(
      IVssRequestContext requestContext,
      ICollection<string> teamFoundationIds,
      bool includeIdentityUrls)
    {
      IdentityHelper identityHelper = new IdentityHelper();
      identityHelper.PopulateIdentities(requestContext, teamFoundationIds, includeIdentityUrls);
      return identityHelper;
    }

    public static IdentityHelper GetIdentityHelper(
      IVssRequestContext requestContext,
      ICollection<string> teamFoundationIds)
    {
      return IdentityHelper.GetIdentityHelper(requestContext, teamFoundationIds, true);
    }

    public virtual IdentityRef GetIdentity(IdentityRef identity)
    {
      if (identity == null || identity.Id == null || string.Compare(identity.Id, Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
        return (IdentityRef) null;
      return !this.teamFoundationIdentities.ContainsKey(identity.Id) ? identity : this.teamFoundationIdentities[identity.Id];
    }

    private void PopulateIdentities(
      IVssRequestContext requestContext,
      ICollection<string> teamFoundationIds,
      bool includeIdentityUrls)
    {
      this.teamFoundationIdentities = teamFoundationIds.QueryIdentities(requestContext, includeUrls: includeIdentityUrls);
    }
  }
}
