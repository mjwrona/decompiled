// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ProcessHandlerOptions
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;

namespace Microsoft.VisualStudio.Services.Configuration
{
  [Flags]
  public enum ProcessHandlerOptions
  {
    None = 0,
    LogErrorsAsWarnings = 1,
    LogErrorsAsInfo = 2,
  }
}
