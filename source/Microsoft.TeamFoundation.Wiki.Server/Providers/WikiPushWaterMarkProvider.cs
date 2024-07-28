// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Providers.WikiPushWaterMarkProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server.Providers
{
  internal class WikiPushWaterMarkProvider
  {
    public virtual int? GetWikiPushWaterMark(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor version,
      TimedCiEvent ciData)
    {
      using (new StopWatchHelper(ciData, nameof (GetWikiPushWaterMark)))
      {
        string wikiVersion = version.VersionType == GitVersionType.Branch ? string.Format("refs/heads/" + version.Version) : throw new NotSupportedException(string.Format("Only Branches are supported, given version descriptor:{0}", (object) version.VersionType));
        using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
          return component.GetWikiPushWaterMark(wiki.ProjectId, wiki.Id, wikiVersion);
      }
    }

    public IList<WikiVersionWaterMark> GetAllWikiVersionWaterMark(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
        return component.GetAllWikiVersionWaterMark(projectId);
    }
  }
}
