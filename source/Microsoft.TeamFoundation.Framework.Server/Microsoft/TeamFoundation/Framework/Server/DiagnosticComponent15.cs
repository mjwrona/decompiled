// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent15
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent15 : DiagnosticComponent14
  {
    internal override List<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> ReadResults(
      IDataReader reader)
    {
      using (ResultCollection resultCollection = new ResultCollection(reader, this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tuple<string, string>>((ObjectBinder<Tuple<string, string>>) new DiagnosticComponent15.MetadataBinder());
        Tuple<string, string> tuple = resultCollection.GetCurrent<Tuple<string, string>>().FirstOrDefault<Tuple<string, string>>();
        resultCollection.NextResult();
        resultCollection.AddBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>((ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>) new DiagnosticComponent13.DatabasePerformanceStatisticsViewBinder3(tuple.Item1, tuple.Item2));
        return resultCollection.GetCurrent<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>().Items;
      }
    }

    protected class MetadataBinder : ObjectBinder<Tuple<string, string>>
    {
      private SqlColumnBinder ServiceObjective = new SqlColumnBinder(nameof (ServiceObjective));
      private SqlColumnBinder ResourceVersion = new SqlColumnBinder(nameof (ResourceVersion));

      private string ReturnInternOrEmpty(string val) => val != null ? string.Intern(val) : string.Empty;

      protected override Tuple<string, string> Bind() => new Tuple<string, string>(this.ReturnInternOrEmpty(this.ServiceObjective.GetString((IDataReader) this.Reader, true)), this.ReturnInternOrEmpty(this.ResourceVersion.GetString((IDataReader) this.Reader, true)));
    }
  }
}
