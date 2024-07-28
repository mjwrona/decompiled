// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IAadGroupHelper
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Server.Core
{
  [InheritedExport]
  internal interface IAadGroupHelper
  {
    IList<Guid> CreateAadGroupsInIms(
      IVssRequestContext context,
      Guid[] aadGroupObjectIds,
      out List<TeamFoundationIdentity> aadGroupsWithCreationFailure,
      out List<string> failureMessages);

    TeamFoundationFilteredIdentitiesList GetAadGroupsFromAad(
      IVssRequestContext context,
      string searchQuery);

    TeamFoundationFilteredIdentitiesList GetAadGroupsFromAad(
      IVssRequestContext context,
      int pageSize,
      ref string searchResultToken);

    TeamFoundationFilteredIdentitiesList GetAadGroupsFromIms(
      IVssRequestContext context,
      string searchQuery);
  }
}
