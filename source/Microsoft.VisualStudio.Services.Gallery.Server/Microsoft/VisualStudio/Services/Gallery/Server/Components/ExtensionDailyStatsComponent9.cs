// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent9
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent9 : ExtensionDailyStatsComponent8
  {
    public virtual int AnonymizeExtensionEvents(
      IEnumerable<ExtensionEvent> extensionEvents,
      string userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_AnonymizeExtensionEvents");
      this.BindExtensionEventsTable3(nameof (extensionEvents), extensionEvents);
      this.BindString(nameof (userId), userId, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }
  }
}
