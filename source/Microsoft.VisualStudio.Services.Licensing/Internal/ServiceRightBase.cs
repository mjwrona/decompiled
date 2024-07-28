// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.ServiceRightBase
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public abstract class ServiceRightBase : IServiceRight, IUsageRight, IComparable
  {
    public abstract Dictionary<string, object> Attributes { get; }

    public virtual DateTimeOffset ExpirationDate { get; set; }

    public abstract string Name { get; }

    public virtual VisualStudioOnlineServiceLevel ServiceLevel { get; set; }

    public virtual Version Version { get; set; }

    public override bool Equals(object obj)
    {
      ServiceRightBase right = obj as ServiceRightBase;
      return obj != null && right != null && this.Equals(right);
    }

    public bool Equals(ServiceRightBase right) => right != null && this.ServiceLevel == right.ServiceLevel && this.ExpirationDate.Equals(right.ExpirationDate);

    public override int GetHashCode() => 21 * this.ServiceLevel.GetHashCode() + this.ExpirationDate.GetHashCode();

    public abstract int CompareTo(object obj);
  }
}
