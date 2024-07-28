// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.EsrpSignRequest
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class EsrpSignRequest
  {
    public readonly Guid ValidationId;
    public readonly string DestinationPath;
    public readonly EsrpSourceFileDescription SourceFileDescription;
    public readonly string KeyCode;

    public EsrpSignRequest(
      string sourceFilePath,
      long sourceFileSize,
      string sourceFileHash,
      string destinationFilePath,
      Guid validationId,
      string keyCode)
    {
      this.ValidationId = validationId;
      this.SourceFileDescription = new EsrpSourceFileDescription(sourceFilePath, sourceFileSize, sourceFileHash);
      this.DestinationPath = destinationFilePath ?? throw new ArgumentNullException(nameof (destinationFilePath));
      this.KeyCode = keyCode ?? throw new ArgumentNullException(nameof (keyCode));
    }
  }
}
