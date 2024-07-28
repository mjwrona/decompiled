// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.UserIdAndNamespacePair
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal struct UserIdAndNamespacePair
  {
    internal UserIdAndNamespacePair(Guid userId, ResourceState throttleType)
    {
      this.UserId = userId;
      this.ThrottleType = throttleType;
    }

    public Guid UserId { get; }

    public ResourceState ThrottleType { get; }

    public override bool Equals(object obj) => obj is UserIdAndNamespacePair andNamespacePair && this.UserId == andNamespacePair.UserId && this.ThrottleType == ((UserIdAndNamespacePair) obj).ThrottleType;

    public override int GetHashCode() => this.UserId.GetHashCode() ^ this.ThrottleType.GetHashCode();
  }
}
