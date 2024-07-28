// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQualityComponent2
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.DataQuality;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class DataQualityComponent2 : DataQualityComponent
  {
    internal override IReadOnlyCollection<DataQualityResult> CheckDataQuality(
      DataQualityDefinition definition,
      int latencyExclusionSeconds,
      DateTime previousEndDate)
    {
      if (definition.MinServiceVersion > this.Version || (definition.MaxServiceVersion ?? int.MaxValue) < this.Version)
        return (IReadOnlyCollection<DataQualityResult>) null;
      if (previousEndDate < Constants.BeginningOfTimeDateTime)
        previousEndDate = Constants.BeginningOfTimeDateTime;
      this.PrepareStoredProcedure(definition.SprocName);
      this.BindInt("@latencyExclusionSeconds", latencyExclusionSeconds);
      this.BindString("@name", definition.Name, 256, false, SqlDbType.VarChar);
      this.BindDateTime("@previousEndDate", previousEndDate);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DataQualityResult>((ObjectBinder<DataQualityResult>) new DataQualityColumns2());
        return (IReadOnlyCollection<DataQualityResult>) resultCollection.GetCurrent<DataQualityResult>().Items;
      }
    }
  }
}
