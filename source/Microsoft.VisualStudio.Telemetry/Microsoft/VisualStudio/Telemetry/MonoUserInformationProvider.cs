// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MonoUserInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class MonoUserInformationProvider : 
    UserInformationProviderBase,
    IUserInformationProvider
  {
    public MonoUserInformationProvider(
      IInternalSettings internalSettings,
      IEnvironmentTools envTools,
      ILegacyApi legacyApi,
      Guid? userId)
      : base(internalSettings, envTools, legacyApi, userId)
    {
    }

    public override UserType UserType => UserType.External;

    protected override bool CalculateIsMicrosoftAADJoined() => false;
  }
}
