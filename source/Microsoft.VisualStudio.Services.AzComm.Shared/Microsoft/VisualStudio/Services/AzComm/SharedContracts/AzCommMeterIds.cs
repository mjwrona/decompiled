// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.SharedContracts.AzCommMeterIds
// Assembly: Microsoft.VisualStudio.Services.AzComm.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A693FC7-F172-4045-B6F2-EA8BEA32D4F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.Shared.dll

using System;

namespace Microsoft.VisualStudio.Services.AzComm.SharedContracts
{
  public static class AzCommMeterIds
  {
    public static readonly Guid BasicsMeterId = Guid.Parse("2586716a-fa90-4760-984d-3e6d9f9f3b1d");
    public static readonly Guid TestManagerMeterId = Guid.Parse("95576bc1-dbf3-4386-9276-4fe294a3a809");
    public static readonly Guid BuildMinutesMeterId = Guid.Parse("e2c73ec7-cb60-4142-b8b2-e216b6c09c1a");
    public static readonly Guid MSHostedCICDMeterId = Guid.Parse("4bad9897-8d87-43bb-80be-5e6e8fefa3de");
    public static readonly Guid SelfHostedCICDMeterId = Guid.Parse("f44a67f2-53ae-4044-bd58-1c8aca386b98");
    public static readonly Guid MSHostedCICDforOSSMeterId = Guid.Parse("3fa30bbe-3437-42d4-a978-0ef84811f470");
    public static readonly Guid SelfHostedCICDforOSSMeterId = Guid.Parse("2fa36572-3c3d-46be-ac59-6053cbb377b4");
    public static readonly Guid ArtifactsMeterId = Guid.Parse("3efc2e47-d73e-4213-8368-3a8723ceb1cc");
    public static readonly Guid CloudLoadTestMeterId = Guid.Parse("a7d460a9-a56d-4b64-837f-14728d6d54d4");
    public static readonly Guid AdvancedSecurityMeterId = Guid.Parse("256caf12-9779-4531-99ac-b46e295130a3");
  }
}
