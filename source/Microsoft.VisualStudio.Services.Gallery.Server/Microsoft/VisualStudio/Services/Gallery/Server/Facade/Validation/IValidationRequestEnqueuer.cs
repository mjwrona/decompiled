// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.IValidationRequestEnqueuer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using NuGet.Services.Validation;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation
{
  public interface IValidationRequestEnqueuer
  {
    void SendMessage(PackageValidationMessageData message);

    void SendMessage(PackageValidationMessageData message, DateTimeOffset postponeProcessingTill);
  }
}
