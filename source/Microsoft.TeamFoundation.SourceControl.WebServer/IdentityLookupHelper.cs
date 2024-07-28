// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.IdentityLookupHelper
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class IdentityLookupHelper
  {
    public static void LoadIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Guid> identityGuids,
      IDictionary<Guid, TeamFoundationIdentity> guidToIdentity)
    {
      HashSet<Guid> source = new HashSet<Guid>(identityGuids.Where<Guid>((Func<Guid, bool>) (guid => !guidToIdentity.ContainsKey(guid))));
      if (!source.Any<Guid>())
        return;
      foreach (TeamFoundationIdentity readIdentity in requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, source.ToArray<Guid>()))
      {
        if (readIdentity != null)
        {
          Guid teamFoundationId = readIdentity.TeamFoundationId;
          guidToIdentity.Add(readIdentity.TeamFoundationId, readIdentity);
        }
      }
    }

    public static void LoadIdentityRefs(
      IVssRequestContext requestContext,
      IEnumerable<Guid> identityGuids,
      IDictionary<Guid, IdentityRef> guidToIdentity)
    {
      requestContext = requestContext ?? throw new ArgumentNullException(nameof (requestContext));
      identityGuids = identityGuids ?? throw new ArgumentNullException(nameof (identityGuids));
      guidToIdentity = guidToIdentity ?? throw new ArgumentNullException(nameof (guidToIdentity));
      if (!new HashSet<Guid>(identityGuids.Where<Guid>((Func<Guid, bool>) (guid => !guidToIdentity.ContainsKey(guid)))).Any<Guid>())
        return;
      Dictionary<Guid, TeamFoundationIdentity> guidToIdentity1 = new Dictionary<Guid, TeamFoundationIdentity>();
      IdentityLookupHelper.LoadIdentities(requestContext, identityGuids.Where<Guid>((Func<Guid, bool>) (guid => guid != Guid.Empty)), (IDictionary<Guid, TeamFoundationIdentity>) guidToIdentity1);
      foreach (KeyValuePair<Guid, TeamFoundationIdentity> keyValuePair in guidToIdentity1)
        guidToIdentity[keyValuePair.Key] = keyValuePair.Value.ToIdentityRef(requestContext);
    }
  }
}
