// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphScopeExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphScopeExtensions
  {
    public static GraphScope ToGraphScopeClient(
      this IdentityScope scope,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (scope == null)
        return (GraphScope) null;
      SubjectDescriptor subjectDescriptor1 = scope.SubjectDescriptor;
      SubjectDescriptor subjectDescriptor2 = scope.Administrators.ToSubjectDescriptor(requestContext);
      string url = (string) null;
      ReferenceLinks links = (ReferenceLinks) null;
      GraphScopeExtensions.GetScopeUrlAndLinks(requestContext, subjectDescriptor1, out url, out links);
      SubjectDescriptor parentDescriptor = new SubjectDescriptor("scp", scope.ParentId.ToString());
      SubjectDescriptor securingHostDescriptor = new SubjectDescriptor("scp", scope.SecuringHostId.ToString());
      return new GraphScope("vsts", scope.Id.ToString(), subjectDescriptor1, (IdentityDescriptor) null, scope.Name, links, url, subjectDescriptor2, scope.IsGlobal, parentDescriptor, securingHostDescriptor, scope.ScopeType);
    }

    private static void GetScopeUrlAndLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      out string url,
      out ReferenceLinks links)
    {
      string graphScopeUrl = GraphUrlHelper.GetGraphScopeUrl(context, subjectDescriptor);
      string graphGroupsUrl = GraphUrlHelper.GetGraphGroupsUrl(context, subjectDescriptor);
      string graphStorageKeyUrl = GraphUrlHelper.GetGraphStorageKeyUrl(context, subjectDescriptor);
      links = new ReferenceLinks();
      links.AddLink("self", graphScopeUrl);
      links.AddLink("groups", graphGroupsUrl);
      links.AddLink("storageKey", graphStorageKeyUrl);
      url = graphScopeUrl;
    }
  }
}
