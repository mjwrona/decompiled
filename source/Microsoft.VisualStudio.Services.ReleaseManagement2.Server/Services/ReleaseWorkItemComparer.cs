// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseWorkItemComparer
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseWorkItemComparer : IEqualityComparer<ReleaseWorkItemRef>
  {
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Its taken care")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Its taken care")]
    public bool Equals(ReleaseWorkItemRef x, ReleaseWorkItemRef y)
    {
      if (x == null && y == null)
        return true;
      return (x == null || y != null) && (x != null || y == null) && x.Id.Equals(y.Id);
    }

    public int GetHashCode(ReleaseWorkItemRef obj) => obj != null ? obj.Id.GetHashCode() : 0;
  }
}
