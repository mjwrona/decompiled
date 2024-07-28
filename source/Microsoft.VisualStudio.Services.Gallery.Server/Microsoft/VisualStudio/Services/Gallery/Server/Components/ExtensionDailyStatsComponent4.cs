// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent4
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent4 : ExtensionDailyStatsComponent3
  {
    public override void AddExtensionEvents(IEnumerable<ExtensionEvents> extensionEvents)
    {
      this.PrepareStoredProcedure("Gallery.prc_AddExtensionEvents");
      this.BindExtensionEventsTable2(nameof (extensionEvents), extensionEvents);
      this.ExecuteNonQuery();
    }
  }
}
