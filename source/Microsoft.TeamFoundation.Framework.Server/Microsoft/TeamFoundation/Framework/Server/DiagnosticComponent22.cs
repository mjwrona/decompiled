// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent22
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent22 : DiagnosticComponent21
  {
    internal override List<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> ReadResults(
      IDataReader reader)
    {
      using (ResultCollection resultCollection = new ResultCollection(reader, this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<(string, string, int, short)>(this.GetMetadataBinder());
        (string, string, int, short) tuple = resultCollection.GetCurrent<(string, string, int, short)>().FirstOrDefault<(string, string, int, short)>();
        resultCollection.NextResult();
        resultCollection.AddBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>(this.GetDatabasePerformanceStatisticsViewBinder(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
        return resultCollection.GetCurrent<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>().Items;
      }
    }

    protected virtual ObjectBinder<(string, string, int, short)> GetMetadataBinder() => (ObjectBinder<(string, string, int, short)>) new DiagnosticComponent22.MetadataBinder5();

    protected virtual ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> GetDatabasePerformanceStatisticsViewBinder(
      string serviceObjective,
      string resourceVersion,
      int schedulers,
      short compatibilityLevel)
    {
      return (ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>) new DiagnosticComponent20.DatabasePerformanceStatisticsViewBinder4(serviceObjective, resourceVersion, schedulers, compatibilityLevel);
    }

    protected class MetadataBinder5 : ObjectBinder<(string, string, int, short)>
    {
      private SqlColumnBinder ServiceObjective = new SqlColumnBinder(nameof (ServiceObjective));
      private SqlColumnBinder ResourceVersion = new SqlColumnBinder(nameof (ResourceVersion));
      private SqlColumnBinder Schedulers = new SqlColumnBinder(nameof (Schedulers));
      private SqlColumnBinder CompatibilityLevel = new SqlColumnBinder(nameof (CompatibilityLevel));

      private string ReturnInternOrEmpty(string val) => val != null ? string.Intern(val) : string.Empty;

      protected override (string, string, int, short) Bind() => (this.ReturnInternOrEmpty(this.ServiceObjective.GetString((IDataReader) this.Reader, true)), this.ReturnInternOrEmpty(this.ResourceVersion.GetString((IDataReader) this.Reader, true)), this.Schedulers.GetInt32((IDataReader) this.Reader, 0), this.CompatibilityLevel.GetInt16((IDataReader) this.Reader, (short) 0));
    }
  }
}
