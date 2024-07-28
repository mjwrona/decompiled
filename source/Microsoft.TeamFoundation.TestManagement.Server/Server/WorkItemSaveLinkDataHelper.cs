// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WorkItemSaveLinkDataHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class WorkItemSaveLinkDataHelper : IWorkItemSaveLinkDataHelper
  {
    private List<LinkInfo> addedLinks;
    private List<LinkInfo> deletedLinks;

    private IVssRequestContext TfsRequestContext { get; set; }

    public WorkItemSaveLinkDataHelper(List<LinkInfo> addedLinks, IVssRequestContext requestContext)
    {
      this.addedLinks = addedLinks;
      this.TfsRequestContext = requestContext;
    }

    public WorkItemSaveLinkDataHelper(
      List<LinkInfo> addedLinks,
      List<LinkInfo> deletedLinks,
      IVssRequestContext requestContext)
    {
      this.addedLinks = addedLinks;
      this.deletedLinks = deletedLinks;
      this.TfsRequestContext = requestContext;
    }

    public IEnumerable<LinkInfo> AddedLinks => (IEnumerable<LinkInfo>) this.addedLinks;

    public IEnumerable<LinkInfo> DeletedLinks => (IEnumerable<LinkInfo>) this.deletedLinks;

    public IEnumerable<KeyValuePair<LinkInfo, LinkUpdate>> UpdatedLinks => (IEnumerable<KeyValuePair<LinkInfo, LinkUpdate>>) null;

    public void AddWorkItemLinkInfo(WorkItemLinkInfo link) => link.SourceId = WITConstants.NewClonedEntityTempId;

    public void ResetWorkItemLinkInfo()
    {
    }
  }
}
