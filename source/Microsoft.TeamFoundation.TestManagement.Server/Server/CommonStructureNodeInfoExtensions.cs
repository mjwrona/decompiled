// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CommonStructureNodeInfoExtensions
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.CssNodes;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class CommonStructureNodeInfoExtensions
  {
    internal static TcmCommonStructureNodeInfo ToTcmCommonStructureNodeInfo(
      this CommonStructureNodeInfo csInfo)
    {
      return new TcmCommonStructureNodeInfo()
      {
        Path = csInfo.Path,
        Uri = csInfo.Uri,
        Name = csInfo.Name,
        StructureType = csInfo.StructureType,
        ProjectUri = csInfo.ProjectUri,
        StartDate = csInfo.StartDate,
        FinishDate = csInfo.FinishDate
      };
    }
  }
}
