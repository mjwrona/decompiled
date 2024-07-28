// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent10
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent10 : ExtensionDailyStatsComponent9
  {
    public override IEnumerable<ExtensionEvent> GetExtensionEventsByUserId(string userId)
    {
      string str = "Gallery.prc_GetExtensionEventsByUserIdFromView";
      this.PrepareStoredProcedure(str);
      this.BindString(nameof (userId), userId, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionEvent>((ObjectBinder<ExtensionEvent>) new ExtensionEventBinder());
        return (IEnumerable<ExtensionEvent>) resultCollection.GetCurrent<ExtensionEvent>().Items;
      }
    }
  }
}
