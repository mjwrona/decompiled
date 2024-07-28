// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MsaIdentityHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class MsaIdentityHelper
  {
    public static IdentityDescriptor GetIdentityDescriptorUsingPuid(string puid)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(puid, nameof (puid));
      return new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", puid + "@Live.com");
    }
  }
}
