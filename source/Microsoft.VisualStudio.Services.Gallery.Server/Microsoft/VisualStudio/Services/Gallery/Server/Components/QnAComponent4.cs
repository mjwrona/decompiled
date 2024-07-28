// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.QnAComponent4
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class QnAComponent4 : QnAComponent3
  {
    public override int DeleteQnAItems(
      Guid extensionId,
      long? parentId = null,
      long? itemId = null,
      bool isHardDelete = false)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteQnAItems");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindBoolean(nameof (isHardDelete), isHardDelete);
      if (parentId.HasValue)
        this.BindLong(nameof (parentId), parentId.Value);
      if (itemId.HasValue)
        this.BindLong(nameof (itemId), itemId.Value);
      SqlParameter sqlParameter = this.BindInt("@deletedItemsCount", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }
  }
}
