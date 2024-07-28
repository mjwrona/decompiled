// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionAlreadyUnderTrialDisallowReason
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class ExtensionAlreadyUnderTrialDisallowReason : AcquisitionOperationDisallowReason
  {
    private double m_trialDaysRemaining;
    private bool m_isPublicPlans;

    public ExtensionAlreadyUnderTrialDisallowReason(double trialDaysRemaining, bool isPublicPlans = true)
    {
      this.m_trialDaysRemaining = trialDaysRemaining;
      this.m_isPublicPlans = isPublicPlans;
    }

    public override string Type => "AlreadyUnderTrial";

    public override string Message => !this.m_isPublicPlans ? ExtensionResources.ExtensionAlreadyUnderTrialIndefinite() : ExtensionResources.ExtensionAlreadyUnderTrial((object) this.m_trialDaysRemaining);
  }
}
