// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent5
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
  internal class ExtensionDailyStatsComponent5 : ExtensionDailyStatsComponent4
  {
    public virtual ExtensionEvent GetExtensionEventByEventId(
      long eventId,
      Guid extensionId,
      ExtensionLifecycleEventType eventType)
    {
      ExtensionEvent extensionEventByEventId = (ExtensionEvent) null;
      string str = "Gallery.prc_GetExtensionEventByEventId";
      this.PrepareStoredProcedure(str);
      this.BindLong(nameof (eventId), eventId);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindInt(nameof (eventType), (int) eventType);
      List<ExtensionEvent> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionEvent>((ObjectBinder<ExtensionEvent>) new ExtensionEventBinder());
        items = resultCollection.GetCurrent<ExtensionEvent>().Items;
      }
      if (items != null && items.Count > 0)
        extensionEventByEventId = items[0];
      return extensionEventByEventId;
    }
  }
}
