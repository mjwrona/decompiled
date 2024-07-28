// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.ImpactedTestsEqualityComparer
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  public class ImpactedTestsEqualityComparer : IEqualityComparer<Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test>
  {
    public bool Equals(Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test it1, Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test it2) => it1 == null && it2 == null || it1 != null && it2 != null && string.Equals(it1.TestName, it2.TestName, StringComparison.OrdinalIgnoreCase);

    public int GetHashCode(Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.Test it) => it.TestName.GetHashCode();
  }
}
