// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionRightsCacheKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ExtensionRightsCacheKey
  {
    public Guid AccountId { get; private set; }

    public Guid UserId { get; private set; }

    public bool IsPublicResource { get; private set; }

    public ExtensionRightsCacheKey(Guid accountId, Guid userId, bool isPublicResource)
    {
      this.AccountId = accountId;
      this.UserId = userId;
      this.IsPublicResource = isPublicResource;
    }

    public override string ToString() => string.Format("Ext.V3/{0}/{1}/{2}", (object) this.AccountId, (object) this.UserId, (object) this.IsPublicResource);

    public override int GetHashCode() => this.ToString().GetHashCode();

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj is ExtensionRightsCacheKey extensionRightsCacheKey && this.AccountId == extensionRightsCacheKey.AccountId && this.UserId == extensionRightsCacheKey.UserId && this.IsPublicResource == extensionRightsCacheKey.IsPublicResource;
    }
  }
}
