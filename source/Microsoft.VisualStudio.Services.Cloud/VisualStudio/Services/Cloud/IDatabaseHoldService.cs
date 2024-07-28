// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.IDatabaseHoldService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DefaultServiceImplementation(typeof (DatabaseHoldService))]
  public interface IDatabaseHoldService : IVssFrameworkService
  {
    bool ObtainDatabaseHold(
      IVssRequestContext deploymentRequestContext,
      Guid key,
      int targetDatabaseId,
      string reason,
      TimeSpan timeUntilAbandon,
      ITFLogger logger);

    bool ReleaseDatabasehold(
      IVssRequestContext deploymentRequestContext,
      int targetDatabaseId,
      Guid key,
      ITFLogger logger);

    IList<DatabaseHoldData> ListDatabaseHolds(
      IVssRequestContext deploymentRequestContext,
      ITFLogger logger);
  }
}
