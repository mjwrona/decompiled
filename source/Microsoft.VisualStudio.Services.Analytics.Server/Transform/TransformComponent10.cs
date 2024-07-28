// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent10
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformComponent10 : TransformComponent9
  {
    public override void CreateTransformReworkFromUncorrectedBatches(
      string operationSproc,
      bool fromExistingState,
      bool createDependentWork,
      bool delayReworkPerAttemptHistory)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_CreateProcessingReworkFromUncorrectedBatches", 900);
      this.BindString("@operationSproc", operationSproc, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean("@fromExistingState", fromExistingState);
      this.BindBoolean("@createDependentWork", createDependentWork);
      this.BindBoolean("@delayReworkPerAttemptHistory", delayReworkPerAttemptHistory);
      this.ExecuteNonQuery();
    }
  }
}
