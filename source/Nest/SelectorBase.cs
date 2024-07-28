// Decompiled with JetBrains decompiler
// Type: Nest.SelectorBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.ComponentModel;

namespace Nest
{
  public abstract class SelectorBase : ISelector
  {
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => base.Equals(obj);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => base.GetHashCode();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString() => base.ToString();
  }
}
