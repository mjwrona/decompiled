// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphServicePrincipalExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphServicePrincipalExtensions
  {
    public static GraphServicePrincipal ToGraphServicePrincipalClient(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (identity.IsContainer)
        throw new ArgumentException(FrameworkResources.RequiresNonContainerIdentity((object) identity.SubjectDescriptor));
      if (Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor))
        throw new ArgumentException(FrameworkResources.RequiresNonServicePrincipalIdentity((object) identity.SubjectDescriptor));
      string origin1;
      string originId1;
      IdentityHelper.GetUserOriginAndOriginId(identity, out origin1, out originId1);
      string property1 = identity.GetProperty<string>("Account", (string) null);
      string property2 = identity.GetProperty<string>("Domain", (string) null);
      string property3 = identity.GetProperty<string>("DirectoryAlias", (string) null);
      SubjectDescriptor subjectDescriptor = identity.SubjectDescriptor;
      string graphUserMetaType = GraphObjectExtensionHelpers.ConvertToGraphUserMetaType(identity.MetaType);
      DateTime property4 = identity.GetProperty<DateTime>("MetadataUpdateDate", DateTime.MinValue);
      bool property5 = identity.GetProperty<bool>("IsDeletedInOrigin", false);
      string property6 = identity.GetProperty<string>("ApplicationId", (string) null);
      string url1;
      ReferenceLinks links1;
      GraphObjectExtensionHelpers.GetServicePrincipalUrlAndLinks(requestContext, subjectDescriptor, identity.Id, out url1, out links1);
      string origin2 = origin1;
      string originId2 = originId1;
      SubjectDescriptor descriptor1 = subjectDescriptor;
      IdentityDescriptor descriptor2 = identity.Descriptor;
      string displayName = identity.DisplayName;
      string str = property1;
      ReferenceLinks links2 = links1;
      string url2 = url1;
      string domain = property2;
      string principalName = str;
      string metaType = graphUserMetaType;
      DateTime metadataUpdateDate = property4;
      int num = property5 ? 1 : 0;
      string directoryAlias = property3;
      string applicationId = property6;
      return new GraphServicePrincipal(origin2, originId2, descriptor1, descriptor2, displayName, links2, url2, domain, principalName, (string) null, metaType, metadataUpdateDate, num != 0, directoryAlias, applicationId);
    }
  }
}
