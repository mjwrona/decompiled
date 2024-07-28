// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist.IOAuthWhitelistService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.OAuthWhitelist;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.OAuthWhitelist
{
  [DefaultServiceImplementation(typeof (FrameworkOAuthWhitelistService))]
  public interface IOAuthWhitelistService : IVssFrameworkService
  {
    void AddOAuthWhitelistEntry(
      IVssRequestContext requestContext,
      Guid applicationId,
      string updatedBy,
      string updateReason);

    void DeleteOAuthWhitelistEntry(
      IVssRequestContext requestContext,
      Guid applicationId,
      string updatedBy,
      string updateReason);

    IEnumerable<OAuthWhitelistEntry> GetOAuthWhitelistEntries(IVssRequestContext vssRequestContext);

    bool IsWhitelisted(IVssRequestContext vssRequestContext, Guid appId);
  }
}
