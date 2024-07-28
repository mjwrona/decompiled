// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileServiceContentValidationComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileServiceContentValidationComponent3 : FileServiceContentValidationComponent2
  {
    public override void CleanupMetadata(int retentionPeriodInDays = 14)
    {
      ArgumentUtility.CheckForOutOfRange(retentionPeriodInDays, nameof (retentionPeriodInDays), -1, int.MaxValue);
      this.PrepareStoredProcedure("prc_CleanupContentValidationMetadata", false);
      this.BindInt(nameof (retentionPeriodInDays), retentionPeriodInDays);
      this.ExecuteNonQuery();
    }
  }
}
