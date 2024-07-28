// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphResultExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphResultExtensions
  {
    public static GraphStorageKeyResult GetGraphStorageKeyResult(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!GraphResultExtensions.IsResourceAccessibleForGivenRequest(requestContext, subjectDescriptor))
        return (GraphStorageKeyResult) null;
      ReferenceLinks storageKeyResultLinks = GraphResultExtensions.GetStorageKeyResultLinks(requestContext, subjectDescriptor, storageKey);
      return new GraphStorageKeyResult(storageKey, storageKeyResultLinks);
    }

    public static GraphDescriptorResult GetGraphDescriptorResult(
      IVssRequestContext requestContext,
      Guid storageKey,
      SubjectDescriptor subjectDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!GraphResultExtensions.IsResourceAccessibleForGivenRequest(requestContext, subjectDescriptor))
        return (GraphDescriptorResult) null;
      ReferenceLinks descriptorResultLinks = GraphResultExtensions.GetDescriptorResultLinks(requestContext, storageKey, subjectDescriptor);
      return new GraphDescriptorResult(subjectDescriptor, descriptorResultLinks);
    }

    public static GraphMembershipState GetGraphMembershipState(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      bool active)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!GraphResultExtensions.IsResourceAccessibleForGivenRequest(requestContext, subjectDescriptor))
        return (GraphMembershipState) null;
      ReferenceLinks membershipStateLinks = GraphResultExtensions.GetMembershipStateLinks(requestContext, subjectDescriptor);
      return new GraphMembershipState(active, membershipStateLinks);
    }

    public static GraphMembership CreateGraphMembership(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      SubjectDescriptor containerDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ReferenceLinks membershipLinks = GraphResultExtensions.GetMembershipLinks(requestContext, subjectDescriptor, containerDescriptor);
      return new GraphMembership(subjectDescriptor, containerDescriptor, membershipLinks);
    }

    public static GraphMembership GetGraphMembership(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      SubjectDescriptor containerDescriptor)
    {
      if (requestContext != null)
      {
        if (GraphResultExtensions.IsResourceAccessibleForGivenRequest(requestContext, subjectDescriptor, containerDescriptor))
          return !Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()) && (!subjectDescriptor.IsPubliclyAvailableGraphSubjectType() || !containerDescriptor.IsPubliclyAvailableGraphSubjectType()) ? (GraphMembership) null : GraphResultExtensions.CreateGraphMembership(requestContext, subjectDescriptor, containerDescriptor);
      }
      return (GraphMembership) null;
    }

    private static bool IsResourceAccessibleForGivenRequest(
      IVssRequestContext requestContext,
      params SubjectDescriptor[] subjectDescriptors)
    {
      if (Microsoft.TeamFoundation.Framework.Server.ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor()))
        return true;
      foreach (SubjectDescriptor subjectDescriptor in subjectDescriptors)
      {
        if (!subjectDescriptor.IsPubliclyAvailableGraphSubjectType())
          return false;
      }
      return true;
    }

    private static ReferenceLinks GetStorageKeyResultLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      Guid storageKey)
    {
      string graphStorageKeyUrl = GraphUrlHelper.GetGraphStorageKeyUrl(context, subjectDescriptor);
      string graphDescriptorUrl = GraphUrlHelper.GetGraphDescriptorUrl(context, storageKey);
      ReferenceLinks storageKeyResultLinks = new ReferenceLinks();
      storageKeyResultLinks.AddLink("self", graphStorageKeyUrl);
      storageKeyResultLinks.AddLink("descriptor", graphDescriptorUrl);
      return storageKeyResultLinks;
    }

    private static ReferenceLinks GetDescriptorResultLinks(
      IVssRequestContext context,
      Guid storageKey,
      SubjectDescriptor subjectDescriptor)
    {
      string graphDescriptorUrl = GraphUrlHelper.GetGraphDescriptorUrl(context, storageKey);
      string graphStorageKeyUrl = GraphUrlHelper.GetGraphStorageKeyUrl(context, subjectDescriptor);
      string graphSubjectUrl = GraphUrlHelper.GetGraphSubjectUrl(context, subjectDescriptor);
      ReferenceLinks descriptorResultLinks = new ReferenceLinks();
      descriptorResultLinks.AddLink("self", graphDescriptorUrl);
      descriptorResultLinks.AddLink(nameof (storageKey), graphStorageKeyUrl);
      descriptorResultLinks.AddLink("subject", graphSubjectUrl);
      return descriptorResultLinks;
    }

    private static ReferenceLinks GetMembershipStateLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor)
    {
      string membershipStateUrl = GraphUrlHelper.GetGraphMembershipStateUrl(context, subjectDescriptor);
      string graphSubjectUrl = GraphUrlHelper.GetGraphSubjectUrl(context, subjectDescriptor);
      ReferenceLinks membershipStateLinks = new ReferenceLinks();
      membershipStateLinks.AddLink("self", membershipStateUrl);
      membershipStateLinks.AddLinkIfIsNotEmpty("member", graphSubjectUrl);
      return membershipStateLinks;
    }

    private static ReferenceLinks GetMembershipLinks(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      SubjectDescriptor containerDescriptor)
    {
      string href1 = subjectDescriptor == new SubjectDescriptor() || containerDescriptor == new SubjectDescriptor() ? string.Empty : GraphUrlHelper.GetGraphMembershipUrl(context, subjectDescriptor, containerDescriptor);
      string href2 = subjectDescriptor == new SubjectDescriptor() ? string.Empty : GraphUrlHelper.GetGraphSubjectUrl(context, subjectDescriptor);
      string href3 = containerDescriptor == new SubjectDescriptor() ? string.Empty : GraphUrlHelper.GetGraphSubjectUrl(context, containerDescriptor);
      ReferenceLinks membershipLinks = new ReferenceLinks();
      membershipLinks.AddLink("self", href1);
      membershipLinks.AddLinkIfIsNotEmpty("member", href2);
      membershipLinks.AddLinkIfIsNotEmpty("container", href3);
      return membershipLinks;
    }
  }
}
