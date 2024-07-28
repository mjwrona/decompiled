// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.ExtensionEventType
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  public enum ExtensionEventType
  {
    ExtensionCreated = 1,
    ExtensionUpdated = 2,
    ExtensionDeleted = 3,
    ExtensionShared = 4,
    ExtensionUnshared = 5,
    ExtensionDisabled = 6,
    ExtensionEnabled = 7,
    ExtensionLocked = 8,
    ExtensionUnlocked = 9,
  }
}
