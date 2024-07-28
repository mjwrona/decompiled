// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AfnStripContractConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class AfnStripContractConverter
  {
    public static TestResultAttachment Convert(AfnStrip afnStrip)
    {
      if (afnStrip == null)
        return (TestResultAttachment) null;
      return new TestResultAttachment()
      {
        FileName = afnStrip.FileName,
        Id = afnStrip.Id,
        TestRunId = afnStrip.TestRunId,
        TestResultId = afnStrip.TestResultId,
        DownloadQueryString = afnStrip.AuxiliaryUrl,
        CreationDate = afnStrip.CreationDate,
        AttachmentType = "AfnStrip"
      };
    }

    public static DefaultAfnStripBinding Convert(Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding binding)
    {
      if (binding == null)
        return (DefaultAfnStripBinding) null;
      return new DefaultAfnStripBinding()
      {
        TestCaseId = binding.TestCaseId,
        TestRunId = binding.TestRunId,
        TestResultId = binding.TestResultId
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding Convert(
      DefaultAfnStripBinding binding)
    {
      if (binding == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding()
      {
        TestCaseId = binding.TestCaseId,
        TestRunId = binding.TestRunId,
        TestResultId = binding.TestResultId
      };
    }
  }
}
