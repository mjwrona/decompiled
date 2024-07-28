// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlLocation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Globalization;

namespace Microsoft.OData.Edm.Csdl
{
  public class CsdlLocation : EdmLocation
  {
    internal CsdlLocation(int number, int position)
      : this((string) null, number, position)
    {
    }

    internal CsdlLocation(string source, int number, int position)
    {
      this.Source = source;
      this.LineNumber = number;
      this.LinePosition = position;
    }

    public string Source { get; private set; }

    public int LineNumber { get; private set; }

    public int LinePosition { get; private set; }

    public override string ToString() => "(" + Convert.ToString(this.LineNumber, (IFormatProvider) CultureInfo.InvariantCulture) + ", " + Convert.ToString(this.LinePosition, (IFormatProvider) CultureInfo.InvariantCulture) + ")";
  }
}
