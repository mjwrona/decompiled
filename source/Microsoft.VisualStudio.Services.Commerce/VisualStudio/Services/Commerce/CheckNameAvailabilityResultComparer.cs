// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CheckNameAvailabilityResultComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CheckNameAvailabilityResultComparer : IEqualityComparer<CheckNameAvailabilityResult>
  {
    public bool Equals(CheckNameAvailabilityResult x, CheckNameAvailabilityResult y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.Message == y.Message && x.NameAvailable == y.NameAvailable;
    }

    public int GetHashCode(CheckNameAvailabilityResult x) => x.Message.GetHashCode() ^ x.NameAvailable.GetHashCode();
  }
}
