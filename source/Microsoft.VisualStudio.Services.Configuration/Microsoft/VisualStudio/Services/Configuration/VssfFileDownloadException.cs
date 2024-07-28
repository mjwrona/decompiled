// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.VssfFileDownloadException
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Serializable]
  public class VssfFileDownloadException : Exception
  {
    public VssfFileDownloadException()
    {
    }

    public VssfFileDownloadException(string message)
      : base(message)
    {
    }

    public VssfFileDownloadException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected VssfFileDownloadException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.DownloadUrl = info.GetString(nameof (DownloadUrl));
      this.FilePath = info.GetString(nameof (FilePath));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("DownloadUrl", (object) this.DownloadUrl);
      info.AddValue("FilePath", (object) this.FilePath);
    }

    public string DownloadUrl { get; set; }

    public string FilePath { get; set; }
  }
}
