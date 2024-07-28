// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.CaseInsensitiveStringOrObjectComparar
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class CaseInsensitiveStringOrObjectComparar : EqualityComparer<object>
  {
    public override bool Equals(object x, object y)
    {
      string x1 = x as string;
      string y1 = y as string;
      return x1 != null || y1 != null ? StringComparer.OrdinalIgnoreCase.Equals(x1, y1) : EqualityComparer<object>.Default.Equals(x, y);
    }

    public override int GetHashCode(object obj) => !(obj is string str) ? EqualityComparer<object>.Default.GetHashCode(obj) : StringComparer.OrdinalIgnoreCase.GetHashCode(str);
  }
}
