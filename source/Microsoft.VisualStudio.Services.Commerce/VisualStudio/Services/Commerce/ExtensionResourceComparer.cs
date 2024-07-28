// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ExtensionResourceComparer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ExtensionResourceComparer : IEqualityComparer<ExtensionResource>
  {
    public bool Equals(ExtensionResource x, ExtensionResource y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.id == y.id && x.location == y.location && x.name == y.name && x.plan.name == y.plan.name && x.plan.product == y.plan.product && x.plan.promotionCode == y.plan.promotionCode && x.plan.publisher == y.plan.publisher && x.plan.version == y.plan.version && x.properties.SequenceEqual<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) y.properties) && x.tags.SequenceEqual<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) y.tags) && x.type == y.type;
    }

    public int GetHashCode(ExtensionResource x) => x.id.GetHashCode();
  }
}
