// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.MsdnEntitlementCodeTuple
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class MsdnEntitlementCodeTuple : IEquatable<MsdnEntitlementCodeTuple>
  {
    public string Item1 { get; private set; }

    public string Item2 { get; private set; }

    public MsdnEntitlementCodeTuple(string item1, string item2)
    {
      this.Item1 = item1;
      this.Item2 = item2;
    }

    public bool Equals(MsdnEntitlementCodeTuple other) => other != null && LicensingComparers.MsdnEntitlementComparer.Equals(this.Item1, other.Item1) && LicensingComparers.MsdnEntitlementComparer.Equals(this.Item2, other.Item2);

    public override bool Equals(object obj) => this.Equals(obj as MsdnEntitlementCodeTuple);

    public override int GetHashCode() => 23 * (23 * 0 + (this.Item1 == null ? 0 : this.Item1.GetHashCode())) + (this.Item2 == null ? 0 : this.Item2.GetHashCode());
  }
}
