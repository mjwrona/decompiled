// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ValidateExtensionsForSpamJobData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension
{
  public class ValidateExtensionsForSpamJobData
  {
    public ExtensionsFilterType ExtensionsFilterType { get; set; }

    public DateTime StartDateTimeForPartialScan { get; set; }

    public DateTime EndDateTimeForPartialScan { get; set; } = DateTime.UtcNow;

    public bool LockExtensionsThatFailedValidations { get; set; }
  }
}
