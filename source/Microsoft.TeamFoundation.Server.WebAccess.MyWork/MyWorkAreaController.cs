// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MyWork.MyWorkAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.MyWork, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8442996D-DF5E-4B6F-9622-CCF23EF07ED1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.MyWork.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.MyWork
{
  public abstract class MyWorkAreaController : TfsAreaController
  {
    public override string AreaName => "MyWork";

    public override string TraceArea => "WebAccess.MyWork";
  }
}
