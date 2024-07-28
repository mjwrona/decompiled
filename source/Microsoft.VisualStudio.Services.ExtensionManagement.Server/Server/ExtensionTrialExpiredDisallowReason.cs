// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionTrialExpiredDisallowReason
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class ExtensionTrialExpiredDisallowReason : AcquisitionOperationDisallowReason
  {
    private string m_accountName;
    private string m_trialExpiryDate;
    private string m_buyRedirectText;

    public ExtensionTrialExpiredDisallowReason(
      string accountName,
      string trialExpiryDate,
      string buyRedirectText)
    {
      this.m_accountName = accountName;
      this.m_trialExpiryDate = trialExpiryDate;
      this.m_buyRedirectText = buyRedirectText;
    }

    public override string Type => "TrialExpired";

    public override string Message => ExtensionResources.ExtensionTrialExpired((object) this.m_accountName, (object) this.m_trialExpiryDate, (object) this.m_buyRedirectText);
  }
}
