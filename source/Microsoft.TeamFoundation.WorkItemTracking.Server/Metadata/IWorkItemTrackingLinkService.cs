// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IWorkItemTrackingLinkService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (WorkItemTrackingLinkService))]
  public interface IWorkItemTrackingLinkService : IVssFrameworkService
  {
    IEnumerable<MDWorkItemLinkType> GetLinkTypes(IVssRequestContext requestContext);

    IEnumerable<OobLinkType> GetAllOutOfBoxLinkTypes(IVssRequestContext requestContext);

    MDWorkItemLinkType GetLinkTypeById(IVssRequestContext requestContext, int id);

    MDWorkItemLinkType GetLinkTypeByName(IVssRequestContext requestContext, string name);

    MDWorkItemLinkType GetLinkTypeByReferenceName(
      IVssRequestContext requestContext,
      string referenceName);

    bool ContainsLinkTypeName(IVssRequestContext requestContext, string name);

    bool ContainsLinkTypeReferenceName(IVssRequestContext requestContext, string referenceName);

    bool TryGetLinkTypeById(
      IVssRequestContext requestContext,
      int id,
      out MDWorkItemLinkType linkType);

    bool TryGetLinkTypeByName(
      IVssRequestContext requestContext,
      string name,
      out MDWorkItemLinkType linkType);

    bool TryGetLinkTypeByReferenceName(
      IVssRequestContext requestContext,
      string referenceName,
      out MDWorkItemLinkType linkType);

    WorkItemLinkTypeEnd GetLinkTypeEndById(IVssRequestContext requestContext, int id);

    WorkItemLinkTypeEnd GetLinkTypeEndByName(IVssRequestContext requestContext, string name);

    WorkItemLinkTypeEnd GetLinkTypeEndByReferenceName(
      IVssRequestContext requestContext,
      string referenceName);

    bool ContainsLinkTypeEndReferenceName(IVssRequestContext requestContext, string referenceName);

    bool TryGetLinkTypeEndById(
      IVssRequestContext requestContext,
      int id,
      out WorkItemLinkTypeEnd linkType);

    bool TryGetLinkTypeEndByName(
      IVssRequestContext requestContext,
      string name,
      out WorkItemLinkTypeEnd linkType);

    bool TryGetLinkTypeEndByReferenceName(
      IVssRequestContext requestContext,
      string referenceName,
      out WorkItemLinkTypeEnd linkType);

    int GetCount(IVssRequestContext requestContext);

    IEnumerable<string> GetLinkTypeReferenceNames(IVssRequestContext requestContext);

    IEnumerable<string> GetLinkTypeEndReferenceNames(IVssRequestContext requestContext);

    IEnumerable<int> GetLinkTypeIds(IVssRequestContext requestContext, bool includeRemoteLinks = true);
  }
}
