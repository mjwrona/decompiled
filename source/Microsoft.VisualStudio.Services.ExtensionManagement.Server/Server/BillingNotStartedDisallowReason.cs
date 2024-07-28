// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.BillingNotStartedDisallowReason
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class BillingNotStartedDisallowReason : AcquisitionOperationDisallowReason
  {
    private string m_extensionName;

    public BillingNotStartedDisallowReason(string extensionName) => this.m_extensionName = extensionName;

    public override string Type => nameof (BillingNotStartedDisallowReason);

    public override string Message => ExtensionResources.TrialDisalloweBillingNotStarted((object) this.m_extensionName);
  }
}
