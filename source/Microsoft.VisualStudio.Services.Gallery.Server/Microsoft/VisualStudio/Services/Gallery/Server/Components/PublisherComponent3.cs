// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent3
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublisherComponent3 : PublisherComponent2
  {
    public override void ManagePublisherVisibility(
      string publisherName,
      string extensionName,
      Guid identityId,
      bool removeTag)
    {
      this.PrepareStoredProcedure("Gallery.prc_ManagePublisherVisibility");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindGuid("userId", identityId);
      this.BindBoolean(nameof (removeTag), removeTag);
      this.ExecuteNonQuery();
    }
  }
}
