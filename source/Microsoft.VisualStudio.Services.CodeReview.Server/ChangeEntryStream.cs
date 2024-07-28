// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ChangeEntryStream
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.IO;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public struct ChangeEntryStream
  {
    public Stream FileStream;
    public CompressionType CompressionType;
    public string FileName;

    public ChangeEntryStream(Stream fileStream, CompressionType compressionType, string fileName = null)
    {
      this.FileName = fileName;
      this.FileStream = fileStream;
      this.CompressionType = compressionType;
    }
  }
}
