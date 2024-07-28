// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.Branches
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  [CollectionDataContract(Name = "Branches", Namespace = "", ItemName = "BranchName")]
  public class Branches : List<string>
  {
    public override bool Equals(object obj)
    {
      if (!(obj is Branches branches) || this.Count != branches.Count)
        return false;
      foreach (string str in (List<string>) this)
      {
        if (!branches.Contains(str))
          return false;
      }
      return true;
    }

    public override int GetHashCode() => base.GetHashCode();

    public override string ToString() => "(" + string.Join(", ", (IEnumerable<string>) this) + ")";
  }
}
