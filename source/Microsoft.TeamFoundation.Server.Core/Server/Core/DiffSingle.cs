// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.DiffSingle
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class DiffSingle : DiffSpec
  {
    public DiffSingle(object oldValue, object newValue)
    {
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    public object OldValue { get; private set; }

    public object NewValue { get; private set; }

    public override string ToString() => this.OldValue.ToString() + " -> " + this.NewValue;
  }
}
