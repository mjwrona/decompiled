// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ScanItemComponent1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  public class ScanItemComponent1 : ScanItemComponent
  {
    public virtual void CreateScanItem(IVssRequestContext requestContext, ScanItem scanItem)
    {
      ArgumentUtility.CheckForEmptyGuid(scanItem.ScanId, "ScanId");
      ArgumentUtility.CheckForEmptyGuid(scanItem.ItemId, "ItemId");
      ArgumentUtility.CheckForNull<string>(scanItem.Description, "Description");
      ArgumentUtility.CheckForNull<string>(scanItem.JobId, "JobId");
      this.PrepareStoredProcedure("Gallery.prc_CreateScanItem");
      this.BindGuid("scanId", scanItem.ScanId);
      this.BindGuid("itemId", scanItem.ItemId);
      this.BindInt("contentType", scanItem.ContentType);
      this.BindString("description", scanItem.Description, 1024, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindString("jobId", scanItem.JobId, 100, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindInt("validationStatus", (int) scanItem.ValidationStatus);
      this.BindString("resultMessage", scanItem.ResultMessage, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual IEnumerable<ScanItem> GetAllScanItems(
      IVssRequestContext requestContext,
      Guid scanId)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetScanItems");
      if (scanId.Equals(Guid.Empty))
        this.BindNullableGuid(nameof (scanId), Guid.Empty);
      else
        this.BindGuid(nameof (scanId), scanId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetScanItems", requestContext))
      {
        resultCollection.AddBinder<ScanItem>((ObjectBinder<ScanItem>) new ScanItemBinder());
        return (IEnumerable<ScanItem>) resultCollection.GetCurrent<ScanItem>().Items;
      }
    }

    public virtual ScanItem UpdateScanItem(IVssRequestContext requestContext, ScanItem scanItem)
    {
      ArgumentUtility.CheckForEmptyGuid(scanItem.ScanId, "ScanId");
      ArgumentUtility.CheckForEmptyGuid(scanItem.ItemId, "ItemId");
      this.PrepareStoredProcedure("Gallery.prc_UpdateScanItem");
      this.BindGuid("scanId", scanItem.ScanId);
      this.BindGuid("itemId", scanItem.ItemId);
      this.BindInt("validationStatus", (int) scanItem.ValidationStatus);
      this.BindString("resultMessage", scanItem.ResultMessage, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateScanItem", requestContext))
      {
        resultCollection.AddBinder<ScanItem>((ObjectBinder<ScanItem>) new ScanItemBinder());
        return resultCollection.GetCurrent<ScanItem>().Items.FirstOrDefault<ScanItem>();
      }
    }

    public virtual void DeleteScanItemsByScanId(IVssRequestContext requestContext, Guid scanId)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteScanItem");
      this.BindGuid(nameof (scanId), scanId);
      this.ExecuteNonQuery();
    }
  }
}
