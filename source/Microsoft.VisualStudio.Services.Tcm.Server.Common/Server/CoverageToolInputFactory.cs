// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageToolInputFactory
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageToolInputFactory : ICoverageToolInputFactory
  {
    public virtual Dictionary<string, IAttachmentFilter> GetAttachmentFilterSet(
      TestManagementRequestContext requestContext,
      bool preferBuildLevelCoverageAttachments = false)
    {
      string[] supportedCoverageTools = new CoverageConfiguration().GetSupportedCoverageTools(requestContext.RequestContext);
      Dictionary<string, IAttachmentFilter> attachmentFilterSet = new Dictionary<string, IAttachmentFilter>();
      foreach (string str in supportedCoverageTools)
      {
        switch (str)
        {
          case "VstestCoverageInput":
            attachmentFilterSet.Add("VstestCoverageInput", new VstestCoverageInput().AttachmentFilter);
            break;
          case "NativeCoverageInput":
            attachmentFilterSet.Add("NativeCoverageInput", new NativeCoverageInput().AttachmentFilter);
            break;
          case "VstestDotCoverageInput":
            IAttachmentFilter attachmentFilter = new VstestDotCoverageInput().AttachmentFilter;
            if (preferBuildLevelCoverageAttachments)
              attachmentFilter = (IAttachmentFilter) new AttachmentFilter(AttachmentType.CodeCoverage, TestLogType.CodeCoverage, TestLogScope.Build, attachmentFilter.GetKnownExtensions());
            attachmentFilterSet.Add("VstestDotCoverageInput", attachmentFilter);
            break;
          default:
            requestContext.Logger.Error(1015425, "No tool found with Name " + str);
            break;
        }
      }
      return attachmentFilterSet;
    }

    public virtual CoverageToolInput GetCoverageToolInput(string toolName, string coveredFileName)
    {
      switch (toolName)
      {
        case "VstestCoverageInput":
          VstestCoverageInput coverageToolInput1 = new VstestCoverageInput();
          coverageToolInput1.ModuleName = coveredFileName;
          coverageToolInput1.FriendlyName = coveredFileName;
          coverageToolInput1.ToolType = new VstestCoverageFileOperator().CoverageTool;
          return (CoverageToolInput) coverageToolInput1;
        case "NativeCoverageInput":
          NativeCoverageInput coverageToolInput2 = new NativeCoverageInput();
          coverageToolInput2.CoverageFile = coveredFileName;
          coverageToolInput2.FriendlyName = coveredFileName;
          coverageToolInput2.ToolType = new NativeCoverageFileOperator().CoverageTool;
          return (CoverageToolInput) coverageToolInput2;
        case "VstestDotCoverageInput":
          VstestDotCoverageInput coverageToolInput3 = new VstestDotCoverageInput();
          coverageToolInput3.FriendlyName = coveredFileName;
          coverageToolInput3.ToolType = new VstestDotCoverageFileOperator().CoverageTool;
          return (CoverageToolInput) coverageToolInput3;
        default:
          return (CoverageToolInput) null;
      }
    }

    public virtual string GetUniqueFileName(string toolName, string coveredFileName) => toolName == "VstestDotCoverageInput" ? "VstestDotCoverageInput" : Path.GetFileNameWithoutExtension(coveredFileName);

    public BatchType GetBatchTypeForCodeCoverageTool(string toolName)
    {
      if (string.Equals(new VstestCoverageFileOperator().CoverageTool, toolName, StringComparison.OrdinalIgnoreCase))
        return new VstestCoverageInput().BatchType;
      if (string.Equals(new NativeCoverageFileOperator().CoverageTool, toolName, StringComparison.OrdinalIgnoreCase))
        return new NativeCoverageInput().BatchType;
      return string.Equals(new VstestDotCoverageFileOperator().CoverageTool, toolName, StringComparison.OrdinalIgnoreCase) ? new VstestDotCoverageInput().BatchType : BatchType.Default;
    }
  }
}
