// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.VsixIdManagerComponent2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class VsixIdManagerComponent2 : VsixIdManagerComponent
  {
    public virtual IEnumerable<ReservedVsixId> GetReservedVsixIdsByUserId(string userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetReservedVsixIdsByUserId");
      this.BindString(nameof (userId), userId, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetReservedVsixIdsByUserId", this.RequestContext))
      {
        resultCollection.AddBinder<ReservedVsixId>((ObjectBinder<ReservedVsixId>) new ReservedVsixIdBinder());
        return (IEnumerable<ReservedVsixId>) resultCollection.GetCurrent<ReservedVsixId>().Items;
      }
    }

    public virtual int AnonymizeReservedVsixIds(string userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_AnonymizeReservedVsixIds");
      this.BindString(nameof (userId), userId, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }
  }
}
