// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent2 : ExtensionDailyStatsComponent1
  {
    public virtual void AddExtensionEvents(IEnumerable<ExtensionEvents> extensionEvents)
    {
      this.PrepareStoredProcedure("Gallery.prc_AddExtensionEvents");
      this.BindExtensionEventsTable(nameof (extensionEvents), extensionEvents);
      this.ExecuteNonQuery();
    }

    public virtual List<ExtensionEvent> GetExtensionEvents(
      Guid extensionId,
      ExtensionLifecycleEventType eventType,
      int? count,
      DateTime? afterDate)
    {
      string str = "Gallery.prc_GetExtensionEvents";
      this.PrepareStoredProcedure(str);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindInt(nameof (eventType), (int) eventType);
      if (count.HasValue)
        this.BindInt(nameof (count), count.Value);
      if (afterDate.HasValue)
        this.BindDateTime(nameof (afterDate), afterDate.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionEvent>((ObjectBinder<ExtensionEvent>) new ExtensionEventBinder());
        return resultCollection.GetCurrent<ExtensionEvent>().Items;
      }
    }
  }
}
