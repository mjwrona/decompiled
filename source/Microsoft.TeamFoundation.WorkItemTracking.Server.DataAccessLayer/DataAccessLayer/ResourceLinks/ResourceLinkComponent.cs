// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks.ResourceLinkComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.ResourceLinks
{
  internal class ResourceLinkComponent : WorkItemTrackingResourceComponent
  {
    public static ResourceLinkComponent CreateComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<ResourceLinkComponent>("WorkItem");

    public virtual IEnumerable<ResourceLinkData> GetResourceLinks(IEnumerable<int> workItemIds)
    {
      workItemIds = (IEnumerable<int>) workItemIds.Distinct<int>().ToArray<int>();
      this.PrepareStoredProcedure("prc_GetResourceLinkDataFromIds");
      this.BindInt32Table("@workItemIds", workItemIds);
      return this.ExecuteUnknown<IEnumerable<ResourceLinkData>>((System.Func<IDataReader, IEnumerable<ResourceLinkData>>) (reader => (IEnumerable<ResourceLinkData>) new ResourceLinkComponent.ResourceLinkDataBinder().BindAll(reader).ToArray<ResourceLinkData>()));
    }

    protected class ResourceLinkDataBinder : WorkItemTrackingObjectBinder<ResourceLinkData>
    {
      private SqlColumnBinder AreaId = new SqlColumnBinder(nameof (AreaId));
      private SqlColumnBinder RemovedDate = new SqlColumnBinder(nameof (RemovedDate));
      private SqlColumnBinder AddedDate = new SqlColumnBinder(nameof (AddedDate));
      private SqlColumnBinder FldId = new SqlColumnBinder(nameof (FldId));
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder FilePath = new SqlColumnBinder(nameof (FilePath));
      private SqlColumnBinder FilePathHash = new SqlColumnBinder(nameof (FilePathHash));
      private SqlColumnBinder OriginalName = new SqlColumnBinder(nameof (OriginalName));
      private SqlColumnBinder ExtId = new SqlColumnBinder(nameof (ExtId));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastWriteDate = new SqlColumnBinder(nameof (LastWriteDate));
      private SqlColumnBinder Length = new SqlColumnBinder(nameof (Length));

      public override ResourceLinkData Bind(IDataReader reader) => new ResourceLinkData()
      {
        AddedDate = this.AddedDate.GetDateTime(reader),
        AreaId = this.AreaId.GetInt32(reader),
        Comment = this.Comment.GetString(reader, true),
        CreationDate = this.CreationDate.GetDateTime(reader),
        ExtId = this.ExtId.GetInt32(reader),
        FilePath = this.FilePath.GetString(reader, true),
        FilePathHash = this.FilePathHash.GetInt32(reader),
        FldId = this.FldId.GetInt32(reader),
        Id = this.Id.GetInt32(reader),
        LastWriteDate = this.LastWriteDate.GetDateTime(reader),
        Length = this.Length.GetInt32(reader),
        OriginalName = this.OriginalName.GetString(reader, true),
        RemovedDate = this.RemovedDate.GetDateTime(reader)
      };
    }
  }
}
