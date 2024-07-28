// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent25
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent25 : DiagnosticComponent24
  {
    internal override List<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> ReadResults(
      IDataReader reader)
    {
      using (ResultCollection resultCollection = new ResultCollection(reader, this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<(string, string, int, short, string)>((ObjectBinder<(string, string, int, short, string)>) new DiagnosticComponent25.MetadataBinder6());
        (string, string, int, short, string) tuple = resultCollection.GetCurrent<(string, string, int, short, string)>().FirstOrDefault<(string, string, int, short, string)>();
        resultCollection.NextResult();
        resultCollection.AddBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>(this.GetDatabasePerformanceStatisticsViewBinder(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5));
        return resultCollection.GetCurrent<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>().Items;
      }
    }

    protected virtual ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> GetDatabasePerformanceStatisticsViewBinder(
      string serviceObjective,
      string resourceVersion,
      int schedulers,
      short compatibilityLevel,
      string serviceObjectiveHardware)
    {
      return (ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>) new DiagnosticComponent20.DatabasePerformanceStatisticsViewBinder4(serviceObjective, resourceVersion, schedulers, compatibilityLevel, serviceObjectiveHardware);
    }

    protected class MetadataBinder6 : ObjectBinder<(string, string, int, short, string)>
    {
      private SqlColumnBinder ServiceObjective = new SqlColumnBinder(nameof (ServiceObjective));
      private SqlColumnBinder ResourceVersion = new SqlColumnBinder(nameof (ResourceVersion));
      private SqlColumnBinder Schedulers = new SqlColumnBinder(nameof (Schedulers));
      private SqlColumnBinder CompatibilityLevel = new SqlColumnBinder(nameof (CompatibilityLevel));
      private SqlColumnBinder ServiceObjectiveHardware = new SqlColumnBinder(nameof (ServiceObjectiveHardware));

      private string ReturnInternOrEmpty(string val) => val != null ? string.Intern(val) : string.Empty;

      protected override (string, string, int, short, string) Bind() => (this.ReturnInternOrEmpty(this.ServiceObjective.GetString((IDataReader) this.Reader, true)), this.ReturnInternOrEmpty(this.ResourceVersion.GetString((IDataReader) this.Reader, true)), this.Schedulers.GetInt32((IDataReader) this.Reader, 0), this.CompatibilityLevel.GetInt16((IDataReader) this.Reader, (short) 0), this.ReturnInternOrEmpty(this.ServiceObjectiveHardware.GetString((IDataReader) this.Reader, true)));
    }
  }
}
