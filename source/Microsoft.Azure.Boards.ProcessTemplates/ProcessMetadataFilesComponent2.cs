// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessMetadataFilesComponent2
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  internal class ProcessMetadataFilesComponent2 : ProcessMetadataFilesComponent
  {
    public override IEnumerable<ProcessMetadataFileEntry> GetAllProcessMetadataFiles()
    {
      this.PrepareStoredProcedure("prc_GetAllProcessMetadataFiles");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessMetadataFileEntry>((ObjectBinder<ProcessMetadataFileEntry>) new ProcessMetadataFilesComponent.ProcessMetadataFilesComponentBinder());
      return (IEnumerable<ProcessMetadataFileEntry>) resultCollection.GetCurrent<ProcessMetadataFileEntry>().Items;
    }
  }
}
