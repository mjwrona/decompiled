// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent33
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent33 : IdentityManagementComponent32
  {
    public override int UpdateIdentityVsid(Guid oldVsid, Guid newVsid)
    {
      this.TraceEnter(47011211, nameof (UpdateIdentityVsid));
      try
      {
        this.PrepareStoredProcedure("prc_UpdateIdentityVsid");
        this.BindGuid("@oldVsid", oldVsid);
        this.BindGuid("@newVsid", newVsid);
        this.BindGuid("@eventAuthor", this.Author);
        return (int) this.ExecuteNonQuery(true);
      }
      finally
      {
        this.TraceLeave(47011212, nameof (UpdateIdentityVsid));
      }
    }
  }
}
