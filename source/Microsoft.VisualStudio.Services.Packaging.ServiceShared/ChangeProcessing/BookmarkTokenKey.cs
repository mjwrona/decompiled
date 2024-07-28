// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.BookmarkTokenKey
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public class BookmarkTokenKey
  {
    public BookmarkTokenKey(
      string protocolName,
      string containerTypeName,
      string tokenName,
      int tokenVersion)
    {
      this.ProtocolName = protocolName;
      this.ContainerTypeName = containerTypeName;
      this.TokenName = tokenName;
      this.TokenVersion = tokenVersion;
    }

    public string ProtocolName { get; }

    public string ContainerTypeName { get; }

    public string TokenName { get; }

    public int TokenVersion { get; }

    public override string ToString() => string.Format(string.Format("Protocol name: {0}. Container type name: {1}. Token name: {2}. Token version: {3}.", (object) this.ProtocolName, (object) this.ContainerTypeName, (object) this.TokenName, (object) this.TokenVersion));
  }
}
