// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.StorageMigrationAndProviderBacked
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Hosting;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  internal class StorageMigrationAndProviderBacked : StorageMigrationBacked
  {
    internal AzureProvider AzureProvider { get; private set; }

    internal StorageMigrationAndProviderBacked(Tuple<StorageMigration, AzureProvider> t)
      : base(t.Item1)
    {
      this.AzureProvider = t.Item2;
    }
  }
}
