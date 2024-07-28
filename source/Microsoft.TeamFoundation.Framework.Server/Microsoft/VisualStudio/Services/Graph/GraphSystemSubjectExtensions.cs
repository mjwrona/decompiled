// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphSystemSubjectExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphSystemSubjectExtensions
  {
    public static GraphSystemSubject ToGraphSystemSubjectClient(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext context)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      if (identity == null)
        return (GraphSystemSubject) null;
      SubjectDescriptor subjectDescriptor = identity.SubjectDescriptor;
      string url = (string) null;
      ReferenceLinks links = (ReferenceLinks) null;
      GraphSystemSubjectExtensions.GetSystemSubjectUrlAndLinks(context, subjectDescriptor, out url, out links);
      return new GraphSystemSubject("vsts", identity.Id.ToString(), subjectDescriptor, identity.Descriptor, identity.DisplayName, links, url);
    }

    private static void GetSystemSubjectUrlAndLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      out string url,
      out ReferenceLinks links)
    {
      url = GraphUrlHelper.GetGraphSubjectUrl(context, subjectDescriptor);
      links = new ReferenceLinks();
      links.AddLink("self", url);
    }
  }
}
