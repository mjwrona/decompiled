// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublisherComponent2 : PublisherComponent
  {
    public override void UpdatePublisherPermissions(PublisherAccessControlEntry publisherAce)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdatePublisherPermissions");
      this.BindString("publisherName", publisherAce.PublisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("extensionName", publisherAce.ExtensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindGuid("userId", publisherAce.IdentityId);
      this.BindInt("allowPermission", publisherAce.AllowPermission);
      this.ExecuteNonQuery();
    }
  }
}
