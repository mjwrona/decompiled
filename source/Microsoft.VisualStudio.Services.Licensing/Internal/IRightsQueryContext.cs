// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.IRightsQueryContext
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public interface IRightsQueryContext
  {
    Guid AccountId { get; }

    string MachineId { get; }

    Version ProductVersion { get; }

    string ProductVersionBuildLab { get; }

    ReleaseType ReleaseType { get; }

    LicensingRequestType RequestType { get; }

    string SingleRightName { get; }

    Microsoft.VisualStudio.Services.Identity.Identity UserIdentity { get; }

    VisualStudioFamily VisualStudioFamily { get; }

    VisualStudioEdition VisualStudioEdition { get; }
  }
}
