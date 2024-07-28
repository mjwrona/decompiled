// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BoolWrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class BoolWrapper
  {
    public BoolWrapper(bool value) => this.Value = value;

    public bool Value { get; }

    protected bool Equals(BoolWrapper other) => other != (BoolWrapper) null && this.Value == other.Value;

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((BoolWrapper) obj);
    }

    public override int GetHashCode() => this.Value.GetHashCode();

    public static bool operator ==(BoolWrapper a, BoolWrapper b)
    {
      if ((object) a != null)
        return a.Equals(b);
      return (object) b == null;
    }

    public static bool operator ==(BoolWrapper a, bool b) => !(a == (BoolWrapper) null) && a.Value == b;

    public static bool operator !=(BoolWrapper a, BoolWrapper b) => !(a == b);

    public static bool operator !=(BoolWrapper a, bool b) => !(a == b);

    public override string ToString() => this.Value.ToString();
  }
}
