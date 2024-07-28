// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ContentInfo
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public class ContentInfo
  {
    public ContentInfo(string sha1Hash, byte flags)
    {
      this.Sha1Hash = sha1Hash;
      this.Flags = flags;
    }

    public string Sha1Hash { get; }

    public byte Flags { get; }
  }
}
