// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlServerUpdateInfo
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

namespace Microsoft.VisualStudio.Services.Configuration
{
  public sealed class SqlServerUpdateInfo
  {
    public SqlServerUpdateInfo(
      string url,
      string description,
      string version,
      int kb,
      int size,
      string datePublished)
    {
      this.Url = url;
      this.Description = description;
      this.Version = version;
      this.KB = kb;
      this.Size = size;
      this.DatePublished = datePublished;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public string Url { get; }

    public string Description { get; }

    public string Version { get; }

    public int Size { get; }

    public int KB { get; }

    public string DatePublished { get; }

    public override string ToString() => string.Format("Url: {0}\r\nDescription: \"{1}\"\r\nSize: {2}\r\nKB: {3}\r\nPublished: \"{4}\"", (object) this.\u003Curl\u003EP, (object) this.\u003Cdescription\u003EP, (object) this.Size, (object) this.KB, (object) this.DatePublished);
  }
}
