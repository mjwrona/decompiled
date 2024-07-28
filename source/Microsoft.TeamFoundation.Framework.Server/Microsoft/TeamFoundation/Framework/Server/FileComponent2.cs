// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent2 : FileComponent
  {
    public override void HardDeleteFiles(IEnumerable<Guid> filesToDelete) => throw new NotSupportedException();

    public override void DeleteFiles(IEnumerable<FileIdentifier> filesToDelete)
    {
      this.PrepareStoredProcedure("prc_DeleteFiles", 3600);
      this.BindInt32Table("@filesToDelete", filesToDelete.Select<FileIdentifier, int>((Func<FileIdentifier, int>) (f => (int) f.FileId)));
      this.ExecuteNonQuery();
    }

    public override List<Guid> QueryUnusedFiles(int maxNumberOfFiles, int retentionPeriodInDays) => throw new NotSupportedException();
  }
}
