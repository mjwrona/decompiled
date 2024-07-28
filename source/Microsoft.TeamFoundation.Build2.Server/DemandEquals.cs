// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DemandEquals
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class DemandEquals : Demand
  {
    public DemandEquals(string name, string value)
      : base(name, value)
    {
    }

    public override Demand Clone() => (Demand) new DemandEquals(this.Name, this.Value);

    protected override string GetExpression() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -equals {1}", (object) this.Name, (object) this.Value);
  }
}
