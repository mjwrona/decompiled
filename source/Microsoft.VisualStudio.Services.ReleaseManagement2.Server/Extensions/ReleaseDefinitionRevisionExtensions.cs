// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionRevisionExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseDefinitionRevisionExtensions
  {
    public static IEnumerable<ReleaseDefinitionRevision> ResolveIdentityRefs(
      this IEnumerable<ReleaseDefinitionRevision> revisions,
      IVssRequestContext requestContext)
    {
      if (revisions == null)
        throw new ArgumentNullException(nameof (revisions));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (!(revisions is IList<ReleaseDefinitionRevision> definitionRevisionList))
        definitionRevisionList = (IList<ReleaseDefinitionRevision>) revisions.ToList<ReleaseDefinitionRevision>();
      IList<ReleaseDefinitionRevision> source = definitionRevisionList;
      IDictionary<string, IdentityRef> identities = source.Select<ReleaseDefinitionRevision, string>((Func<ReleaseDefinitionRevision, string>) (revision => revision.ChangedBy.Id)).ToList<string>().QueryIdentities(requestContext, false);
      if (identities != null && identities.Any<KeyValuePair<string, IdentityRef>>())
      {
        foreach (ReleaseDefinitionRevision definitionRevision in source.Where<ReleaseDefinitionRevision>((Func<ReleaseDefinitionRevision, bool>) (releaseDefinitionRevision => identities.ContainsKey(releaseDefinitionRevision.ChangedBy.Id))))
          definitionRevision.ChangedBy = identities[definitionRevision.ChangedBy.Id];
      }
      return (IEnumerable<ReleaseDefinitionRevision>) source;
    }
  }
}
