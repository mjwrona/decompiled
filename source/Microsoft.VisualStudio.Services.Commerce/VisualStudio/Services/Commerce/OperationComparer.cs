// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OperationComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class OperationComparer : IEqualityComparer<Operation>
  {
    public bool Equals(Operation x, Operation y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.display.description == y.display.description && x.display.operation == y.display.operation && x.display.provider == y.display.provider && x.display.resource == y.display.resource && x.name == y.name;
    }

    public int GetHashCode(Operation x) => x.name.GetHashCode();
  }
}
