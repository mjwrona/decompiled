// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Cookie
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

namespace Microsoft.AspNet.SignalR
{
  public class Cookie
  {
    public Cookie(string name, string value)
      : this(name, value, string.Empty, string.Empty)
    {
    }

    public Cookie(string name, string value, string domain, string path)
    {
      this.Name = name;
      this.Value = value;
      this.Domain = domain;
      this.Path = path;
    }

    public string Name { get; private set; }

    public string Domain { get; private set; }

    public string Path { get; private set; }

    public string Value { get; private set; }
  }
}
