// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent8
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
  internal class ExtensionDailyStatsComponent8 : ExtensionDailyStatsComponent7
  {
    public virtual IEnumerable<ExtensionEvent> GetExtensionEventsByUserId(string userId)
    {
      string str = "Gallery.prc_GetExtensionEventsByUserId";
      this.PrepareStoredProcedure(str);
      this.BindString(nameof (userId), userId, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionEvent>((ObjectBinder<ExtensionEvent>) new ExtensionEventBinder());
        return (IEnumerable<ExtensionEvent>) resultCollection.GetCurrent<ExtensionEvent>().Items;
      }
    }

    public virtual int AnonymizeExtensionEvents(string userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_AnonymizeExtensionEvents");
      this.BindString(nameof (userId), userId, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }
  }
}
