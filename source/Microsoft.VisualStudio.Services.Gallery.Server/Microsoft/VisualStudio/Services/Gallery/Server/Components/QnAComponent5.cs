// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.QnAComponent5
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class QnAComponent5 : QnAComponent4
  {
    public virtual IEnumerable<ExtensionQnAItem> GetQnAItemsByUserId(Guid userId)
    {
      string str = "Gallery.prc_GetQnAItemsByUserId";
      this.PrepareStoredProcedure(str);
      this.BindGuid(nameof (userId), userId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionQnAItem>((ObjectBinder<ExtensionQnAItem>) new QnAComponent.ExtensionQnAItemBinder());
        return (IEnumerable<ExtensionQnAItem>) resultCollection.GetCurrent<ExtensionQnAItem>().Items;
      }
    }

    public virtual int AnonymizeQnAItems(Guid userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_AnonymizeQnAItems");
      this.BindGuid(nameof (userId), userId);
      SqlParameter sqlParameter = this.BindInt("@rowCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }
  }
}
