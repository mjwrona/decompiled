// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableLoader
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public interface TableLoader
  {
    ITable LoadSource(IVssRequestContext deploymentContext, StorageMigration storage);

    ITable LoadTarget(IVssRequestContext deploymentContext, MigrateStorageInfo info, string name);
  }
}
