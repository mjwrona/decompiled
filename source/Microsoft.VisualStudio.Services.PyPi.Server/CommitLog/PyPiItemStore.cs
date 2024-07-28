// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.CommitLog.PyPiItemStore
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;

namespace Microsoft.VisualStudio.Services.PyPi.Server.CommitLog
{
  public class PyPiItemStore : PackagingItemStoreBase
  {
    public override bool CanUseTimeBasedReferences => false;

    protected override string GetExperienceName() => PyPiItemStore.ExperienceName;

    public static string ExperienceName => Protocol.PyPi.LowercasedName;
  }
}
