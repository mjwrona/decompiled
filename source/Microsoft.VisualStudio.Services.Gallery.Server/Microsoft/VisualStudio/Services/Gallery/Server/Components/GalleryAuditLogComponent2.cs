// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.GalleryAuditLogComponent2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class GalleryAuditLogComponent2 : GalleryAuditLogComponent
  {
    public override void CleanUpAuditLog(int noOfDays)
    {
      try
      {
        this.TraceEnter(12061038, "Enter CleanUpAuditLog");
        this.PrepareStoredProcedure("Gallery.prc_CleanUpAuditLog");
        this.BindInt(nameof (noOfDays), noOfDays);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(12061038, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061038, "Leave CleanupAuditLog");
      }
    }
  }
}
