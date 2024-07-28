// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Store.IGraphMembershipStore
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Graph.Store
{
  [DefaultServiceImplementation(typeof (GraphMembershipStore))]
  internal interface IGraphMembershipStore : IVssFrameworkService
  {
    IEnumerable<SubjectDescriptor> GetChildren(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor);

    IEnumerable<SubjectDescriptor> GetParents(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor);

    IList<SubjectDescriptor> GetDescendants(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor);

    IDictionary<SubjectDescriptor, List<SubjectDescriptor>> GetDescendantsByDescriptors(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> subjectDescriptors);

    IEnumerable<SubjectDescriptor> GetAncestors(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor);

    bool? IsActive(IVssRequestContext context, SubjectDescriptor subjectDescriptor);

    IDictionary<SubjectDescriptor, bool?> AreActive(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> subjectDescriptors);

    void AddMember(
      IVssRequestContext context,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor);

    void RemoveMember(
      IVssRequestContext context,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor);

    IEnumerable<Guid> GetOrganizationStorageKeysForMembersInScope(IVssRequestContext context);

    bool IsMegaTenant(IVssRequestContext context);
  }
}
