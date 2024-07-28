// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Comparer.ReleaseChangeComparer
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Comparer
{
  public class ReleaseChangeComparer : IEqualityComparer<Change>
  {
    public bool Equals(Change x, Change y)
    {
      if (x == y)
        return true;
      return x != null && y != null && x.Id == y.Id && x.DisplayUri == y.DisplayUri;
    }

    public int GetHashCode(Change obj) => obj == null ? 0 : (obj.Id == null ? 0 : obj.Id.GetHashCode()) ^ (obj.DisplayUri == (Uri) null ? 0 : obj.DisplayUri.GetHashCode());
  }
}
