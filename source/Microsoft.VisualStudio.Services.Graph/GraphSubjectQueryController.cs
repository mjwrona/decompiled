// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphSubjectQueryController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "SubjectQuery")]
  public class GraphSubjectQueryController : GraphControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (IEnumerable<GraphSubject>), null, null)]
    public HttpResponseMessage QuerySubjects(GraphSubjectQuery subjectQuery)
    {
      ArgumentUtility.CheckForNull<GraphSubjectQuery>(subjectQuery, nameof (subjectQuery), "Graph");
      ArgumentUtility.CheckStringForNullOrEmpty(subjectQuery.Query, "Query", "Graph");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) subjectQuery.SubjectKind, "SubjectKind", "Graph");
      this.TfsRequestContext.TraceDataConditionally(10006077, TraceLevel.Info, this.TraceArea, nameof (GraphSubjectQueryController), "Received input parameters", (Func<object>) (() => (object) new
      {
        subjectQuery = subjectQuery
      }), nameof (QuerySubjects));
      Guid guid = subjectQuery.ScopeDescriptor == new SubjectDescriptor() ? this.TfsRequestContext.ServiceHost.InstanceId : subjectQuery.ScopeDescriptor.GetLocalScopeId(this.TfsRequestContext);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string query = subjectQuery.Query;
      IEnumerable<string> subjectKind = subjectQuery.SubjectKind;
      List<string> propertiesToSearch = new List<string>();
      propertiesToSearch.Add("DisplayName");
      propertiesToSearch.Add("Mail");
      propertiesToSearch.Add("MailNickname");
      propertiesToSearch.Add("SignInAddress");
      string str;
      ref string local = ref str;
      Guid scopeId = guid;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = VisualStudioDirectorySearchHelper.SearchVsdIdentities(tfsRequestContext, query, subjectKind, (IEnumerable<string>) null, (IEnumerable<string>) null, (IEnumerable<string>) propertiesToSearch, (IEnumerable<string>) null, 100, QueryType.Search, out local, scopeId);
      List<GraphSubject> list = source != null ? source.Select<Microsoft.VisualStudio.Services.Identity.Identity, GraphSubject>((Func<Microsoft.VisualStudio.Services.Identity.Identity, GraphSubject>) (identity => identity.ToGraphSubjectClient(this.TfsRequestContext))).OrderBy<GraphSubject, string>((Func<GraphSubject, string>) (graphSubject => graphSubject.DisplayName)).ToList<GraphSubject>() : (List<GraphSubject>) null;
      list?.ForEach((Action<GraphSubject>) (x => x.FillSerializeInternalsField(this.TfsRequestContext)));
      return this.Request.CreateResponse<List<GraphSubject>>(HttpStatusCode.OK, list);
    }
  }
}
