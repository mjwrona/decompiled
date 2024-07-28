// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Comparer.ReleaseWorkItemComparer
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Comparer
{
  public class ReleaseWorkItemComparer : IEqualityComparer<ReleaseWorkItemRef>
  {
    public bool Equals(ReleaseWorkItemRef x, ReleaseWorkItemRef y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.Id == y.Id && x.Url == y.Url;
    }

    public int GetHashCode(ReleaseWorkItemRef obj) => obj == null ? 0 : (obj.Id == null ? 0 : obj.Id.GetHashCode()) ^ (obj.Url == null ? 0 : obj.Url.GetHashCode());
  }
}
