// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.RegistryChannelValidator
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class RegistryChannelValidator : IChannelValidator
  {
    private readonly IInternalSettings internalSettings;

    public RegistryChannelValidator(IInternalSettings internalSettings)
    {
      internalSettings.RequiresArgumentNotNull<IInternalSettings>(nameof (internalSettings));
      this.internalSettings = internalSettings;
    }

    public bool IsValid(ISessionChannel channelToValidate)
    {
      channelToValidate.RequiresArgumentNotNull<ISessionChannel>(nameof (channelToValidate));
      int channelSettings = (int) this.internalSettings.GetChannelSettings(channelToValidate.ChannelId);
      if (channelSettings == 1)
        channelToValidate.Properties |= ChannelProperties.Default;
      return channelSettings != 0;
    }
  }
}
