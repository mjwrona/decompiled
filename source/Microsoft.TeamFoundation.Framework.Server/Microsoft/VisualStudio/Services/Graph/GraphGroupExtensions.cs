// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphGroupExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphGroupExtensions
  {
    public static GraphGroup ToGraphGroupClient(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return identity.ToGraphGroupClientInternal(requestContext);
    }

    private static GraphGroup ToGraphGroupClientInternal(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      if (identity == null)
        return (GraphGroup) null;
      if (!identity.IsContainer)
        throw new ArgumentException(FrameworkResources.RequiresContainerIdentity((object) identity.SubjectDescriptor));
      string origin1 = (string) null;
      string originId1 = (string) null;
      GraphGroupExtensions.GetGroupOriginAndOriginId(identity, out origin1, out originId1);
      string property1 = identity.GetProperty<string>("Account", (string) null);
      string displayName1 = identity.DisplayName;
      string property2 = identity.GetProperty<string>("Domain", (string) null);
      string property3 = identity.GetProperty<string>("Mail", (string) null);
      string property4 = identity.GetProperty<string>("Description", (string) null);
      SubjectDescriptor subjectDescriptor = identity.SubjectDescriptor;
      string url1 = (string) null;
      ReferenceLinks links1 = (ReferenceLinks) null;
      GraphGroupExtensions.GetGroupUrlAndLinks(requestContext, subjectDescriptor, out url1, out links1);
      string property5 = identity.GetProperty<string>("SpecialType", (string) null);
      Guid property6 = identity.GetProperty<Guid>("ScopeId", new Guid());
      string property7 = identity.GetProperty<string>("ScopeType", (string) null);
      Guid property8 = identity.GetProperty<Guid>("LocalScopeId", new Guid());
      Guid property9 = identity.GetProperty<Guid>("SecuringHostId", new Guid());
      string property10 = identity.GetProperty<string>("ScopeName", (string) null);
      bool flag1 = identity.GetProperty<string>("RestrictedVisible", (string) null) == "RestrictedVisible";
      bool flag2 = identity.GetProperty<string>("CrossProject", (string) null) == "CrossProject";
      bool flag3 = identity.GetProperty<string>("GlobalScope", (string) null) == "GlobalScope";
      bool property11 = identity.GetProperty<bool>("IsGroupDeleted", false);
      string origin2 = origin1;
      string originId2 = originId1;
      SubjectDescriptor descriptor1 = subjectDescriptor;
      IdentityDescriptor descriptor2 = identity.Descriptor;
      string displayName2 = property1;
      string str = displayName1;
      ReferenceLinks links2 = links1;
      string url2 = url1;
      string domain = property2;
      string principalName = str;
      string mailAddress = property3;
      string description = property4;
      string specialType = property5;
      Guid scopeId = property6;
      string scopeType = property7;
      Guid guid1 = property8;
      Guid guid2 = property9;
      string scopeName = property10;
      Guid localScopeId = guid1;
      Guid securingHostId = guid2;
      int num1 = flag1 ? 1 : 0;
      int num2 = flag2 ? 1 : 0;
      int num3 = flag3 ? 1 : 0;
      int num4 = property11 ? 1 : 0;
      return new GraphGroup(origin2, originId2, descriptor1, descriptor2, displayName2, links2, url2, domain, principalName, mailAddress, description, specialType, scopeId, scopeType, scopeName, localScopeId, securingHostId, num1 != 0, num2 != 0, num3 != 0, num4 != 0);
    }

    private static void GetGroupOriginAndOriginId(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      out string origin,
      out string originId)
    {
      SecurityIdentifierInfo securityIdentifierInfo = new SecurityIdentifierInfo(new SecurityIdentifier(identity.Descriptor.Identifier));
      if (GraphGroupExtensions.IsWindowsGroup(identity))
      {
        origin = "ad";
        originId = securityIdentifierInfo.ToString();
      }
      else if (SidIdentityHelper.IsAadGroupSid(securityIdentifierInfo.GetBinaryForm()))
      {
        origin = "aad";
        originId = AadIdentityHelper.ExtractAadGroupId(identity.Descriptor).ToString();
      }
      else
      {
        origin = "vsts";
        originId = identity.Id.ToString();
      }
    }

    private static bool IsWindowsGroup(Microsoft.VisualStudio.Services.Identity.Identity identity) => identity?.Descriptor != (IdentityDescriptor) null && identity.IsContainer && identity.Descriptor.IsWindowsType() && string.Equals(identity.GetProperty<string>("SchemaClassName", (string) null), "Group");

    private static void GetGroupUrlAndLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      out string url,
      out ReferenceLinks links)
    {
      if (context == null)
      {
        links = (ReferenceLinks) null;
        url = (string) null;
      }
      else
      {
        string graphGroupUrl = GraphUrlHelper.GetGraphGroupUrl(context, subjectDescriptor);
        string graphMembershipsUrl = GraphUrlHelper.GetGraphMembershipsUrl(context, subjectDescriptor);
        string membershipStateUrl = GraphUrlHelper.GetGraphMembershipStateUrl(context, subjectDescriptor);
        string graphStorageKeyUrl = GraphUrlHelper.GetGraphStorageKeyUrl(context, subjectDescriptor);
        links = new ReferenceLinks();
        links.AddLink("self", graphGroupUrl);
        links.AddLink("memberships", graphMembershipsUrl);
        links.AddLink("membershipState", membershipStateUrl);
        links.AddLink("storageKey", graphStorageKeyUrl);
        url = graphGroupUrl;
      }
    }
  }
}
