// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ProductContributionAssociateData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ProductContributionAssociateData
  {
    private HashSet<string> mInstallationTargets;
    private IList<string> mChild;

    public HashSet<string> InstallationTargets => this.mInstallationTargets;

    public IList<string> Child => this.mChild;

    public string Parent { get; set; }

    public ProductContributionAssociateData()
    {
      this.mInstallationTargets = new HashSet<string>();
      this.mChild = (IList<string>) new List<string>();
    }
  }
}
