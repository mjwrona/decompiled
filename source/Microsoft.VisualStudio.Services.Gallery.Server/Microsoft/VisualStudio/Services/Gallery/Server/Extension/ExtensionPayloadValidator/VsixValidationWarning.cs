// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.VsixValidationWarning
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal class VsixValidationWarning
  {
    public VsixValidationWarning(string message, string errorType, string messageId)
    {
      this.ErrorType = errorType;
      this.Message = message;
      this.MessageId = messageId;
    }

    public VsixValidationWarning(string errorType) => this.ErrorType = errorType;

    public string ErrorType { get; set; }

    public string Message { get; set; }

    public string MessageId { get; set; }
  }
}
