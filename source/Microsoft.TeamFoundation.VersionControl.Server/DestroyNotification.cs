// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DestroyNotification
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class DestroyNotification
  {
    internal DestroyNotification(
      Microsoft.VisualStudio.Services.Identity.Identity user,
      ItemSpec itemSpec,
      VersionSpec versionSpec,
      VersionSpec stopAtSpec,
      int flags)
    {
      this.User = user;
      this.ItemSpec = itemSpec;
      this.VersionSpec = versionSpec;
      this.StopAtSpec = stopAtSpec;
      this.Flags = flags;
    }

    internal DestroyNotification(
      Microsoft.VisualStudio.Services.Identity.Identity user,
      ItemSpec itemSpec,
      VersionSpec versionSpec,
      VersionSpec stopAtSpec,
      int flags,
      int destroyedItemsCount)
      : this(user, itemSpec, versionSpec, stopAtSpec, flags)
    {
      this.DestroyedItemsCount = destroyedItemsCount;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity User { get; private set; }

    public ItemSpec ItemSpec { get; private set; }

    public VersionSpec VersionSpec { get; private set; }

    public VersionSpec StopAtSpec { get; private set; }

    public int Flags { get; private set; }

    public int DestroyedItemsCount { get; private set; }
  }
}
