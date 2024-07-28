// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ScanItemComponent2
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
  public class ScanItemComponent2 : ScanItemComponent1
  {
    public virtual ScanViolationItem CreateorUpdateScanViolationItem(
      ScanViolationItem scanViolationItem)
    {
      ArgumentUtility.CheckForEmptyGuid(scanViolationItem.ExtensionId, "ExtensionId");
      this.PrepareStoredProcedure("Gallery.prc_UpdateScanViolationItem");
      this.BindGuid("extensionId", scanViolationItem.ExtensionId);
      this.BindGuid("latestScanId", scanViolationItem.LatestScanId);
      this.BindBoolean("violationFound", scanViolationItem.ViolationFound);
      this.BindString("resultMessage", scanViolationItem.ResultMessage, -1, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindNullableDateTime("contactDate", scanViolationItem.ContactDate);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateScanViolationItem", this.RequestContext))
      {
        resultCollection.AddBinder<ScanViolationItem>((ObjectBinder<ScanViolationItem>) new ScanViolationItemBinder());
        return resultCollection.GetCurrent<ScanViolationItem>().Items.FirstOrDefault<ScanViolationItem>();
      }
    }

    public virtual int DeleteScanViolationItems(IEnumerable<Guid> extensionIds)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteScanViolationItems");
      this.BindGuidTable(nameof (extensionIds), extensionIds);
      return (int) this.ExecuteNonQuery(true);
    }

    public virtual IEnumerable<ScanViolationItem> GetScanViolationItems(
      IEnumerable<Guid> extensionIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(extensionIds, nameof (extensionIds));
      this.PrepareStoredProcedure("Gallery.prc_GetScanViolationItems");
      this.BindGuidTable(nameof (extensionIds), extensionIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetScanViolationItems", this.RequestContext))
      {
        resultCollection.AddBinder<ScanViolationItem>((ObjectBinder<ScanViolationItem>) new ScanViolationItemBinder());
        return (IEnumerable<ScanViolationItem>) resultCollection.GetCurrent<ScanViolationItem>().Items;
      }
    }
  }
}
