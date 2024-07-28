// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent9
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsMetadataComponent9 : AnalyticsMetadataComponent8
  {
    public override List<Guid> GetModelProjectGuids()
    {
      this.PrepareStoredProcedure("AnalyticsModel.prc_GetProjectSKs");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder projectSKBinder = new SqlColumnBinder("ProjectSK");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => projectSKBinder.GetGuid(reader))));
        return resultCollection.GetCurrent<Guid>().Items;
      }
    }
  }
}
