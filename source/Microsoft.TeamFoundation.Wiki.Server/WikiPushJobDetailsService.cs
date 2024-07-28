// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPushJobDetailsService
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  internal class WikiPushJobDetailsService : IWikiPushJobDetailsService, IVssFrameworkService
  {
    private const string c_layer = "WikiPushJobDetailsService";
    private IList<(IWikiPushOneTimeJob Job, IList<string> FeatureFlags)> m_wikiPushProcessingJobs;
    private IDisposableReadOnlyList<IWikiPushOneTimeJob> m_jobExtensions;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.TraceEnter(15250704, "Microsoft.TeamFoundation.Wiki.Server", nameof (WikiPushJobDetailsService), nameof (ServiceStart));
      try
      {
        this.m_jobExtensions = systemRequestContext.GetExtensions<IWikiPushOneTimeJob>();
        this.m_wikiPushProcessingJobs = (IList<(IWikiPushOneTimeJob, IList<string>)>) new List<(IWikiPushOneTimeJob, IList<string>)>(this.m_jobExtensions.Count);
        foreach (IWikiPushOneTimeJob jobExtension in (IEnumerable<IWikiPushOneTimeJob>) this.m_jobExtensions)
        {
          IList<string> list = (IList<string>) jobExtension.GetType().GetCustomAttributes(typeof (FeatureEnabledAttribute), true).Cast<FeatureEnabledAttribute>().Select<FeatureEnabledAttribute, string>((Func<FeatureEnabledAttribute, string>) (attribute => attribute.FeatureFlag)).ToList<string>();
          this.m_wikiPushProcessingJobs.Add((jobExtension, list));
        }
      }
      finally
      {
        systemRequestContext.TraceLeave(15250704, "Microsoft.TeamFoundation.Wiki.Server", nameof (WikiPushJobDetailsService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_jobExtensions?.Dispose();
      this.m_jobExtensions = (IDisposableReadOnlyList<IWikiPushOneTimeJob>) null;
      this.m_wikiPushProcessingJobs = (IList<(IWikiPushOneTimeJob, IList<string>)>) null;
    }

    public IList<(IWikiPushOneTimeJob Job, IList<string> FeatureFlags)> GetWikiPushSubscriberJobs(
      IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      return this.m_wikiPushProcessingJobs;
    }
  }
}
