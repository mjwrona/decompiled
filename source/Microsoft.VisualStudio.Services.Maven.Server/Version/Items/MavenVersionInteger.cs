// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Version.Items.MavenVersionInteger
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers;
using System.Globalization;
using System.Numerics;

namespace Microsoft.VisualStudio.Services.Maven.Server.Version.Items
{
  public class MavenVersionInteger : IMavenVersionItem
  {
    public MavenVersionInteger(BigInteger value) => this.Value = value;

    public MavenVersionInteger(string value)
      : this(BigInteger.Parse(value, NumberStyles.Integer))
    {
    }

    public BigInteger Value { get; }

    public static MavenVersionInteger Zero => new MavenVersionInteger(BigInteger.Zero);

    public bool IsEquivalentToNull() => this.Value.Equals(BigInteger.Zero);

    public override string ToString() => this.Value.ToString();
  }
}
