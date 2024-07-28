// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.WellKnownAclStores
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Security
{
  public static class WellKnownAclStores
  {
    public static readonly Guid User = new Guid("CA3D400B-E690-47AC-83EA-FDAE80FC8D76");
    public static readonly Guid System = new Guid("0C458ADA-F17C-4F66-B644-3663707D17DD");
    private const string c_userString = "CA3D400B-E690-47AC-83EA-FDAE80FC8D76";
    private const string c_systemString = "0C458ADA-F17C-4F66-B644-3663707D17DD";
  }
}
