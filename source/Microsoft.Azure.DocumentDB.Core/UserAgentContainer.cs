// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.UserAgentContainer
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Text;

namespace Microsoft.Azure.Documents
{
  internal class UserAgentContainer
  {
    private static readonly string baseUserAgent = CustomTypeExtensions.GenerateBaseUserAgentString();
    private string userAgent;
    private byte[] userAgentUTF8;
    private string suffix;
    private const int maxSuffixLength = 64;

    public UserAgentContainer()
    {
      this.userAgent = this.BaseUserAgent;
      this.userAgentUTF8 = Encoding.UTF8.GetBytes(this.BaseUserAgent);
    }

    public UserAgentContainer(string suffix)
      : this()
    {
      this.Suffix = suffix;
    }

    public string UserAgent => this.userAgent;

    public byte[] UserAgentUTF8 => this.userAgentUTF8;

    public string Suffix
    {
      get => this.suffix;
      set
      {
        this.suffix = value;
        if (this.suffix.Length > 64)
          this.suffix = this.suffix.Substring(0, 64);
        this.userAgent = this.BaseUserAgent + this.suffix;
        this.userAgentUTF8 = Encoding.UTF8.GetBytes(this.userAgent);
      }
    }

    internal virtual string BaseUserAgent => UserAgentContainer.baseUserAgent;
  }
}
