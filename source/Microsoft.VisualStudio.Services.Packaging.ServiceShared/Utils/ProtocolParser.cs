// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ProtocolParser
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class ProtocolParser : IProtocolParser
  {
    private readonly IProtocolRegistrar protocolRegistrar;

    public ProtocolParser(IProtocolRegistrar protocolRegistrar) => this.protocolRegistrar = protocolRegistrar;

    public bool TryParse(string protocol, out IProtocol p)
    {
      p = this.protocolRegistrar.GetProtocolOrDefault(protocol);
      return p != null;
    }
  }
}
