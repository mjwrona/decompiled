// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.VssInsufficientPermissionsFaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public class VssInsufficientPermissionsFaultMapper : FaultMapper
  {
    private const string ErrorStr = "you do not have permissions for the operation you are attempting";

    public VssInsufficientPermissionsFaultMapper()
      : base("VssInsufficientPermissions", IndexerFaultSource.TFS)
    {
    }

    public override bool IsMatch(Exception ex) => ex != null && ex.ToString().Contains("Microsoft.VisualStudio.Services.Common.VssServiceException") && ex.ToString().Contains("you do not have permissions for the operation you are attempting");
  }
}
