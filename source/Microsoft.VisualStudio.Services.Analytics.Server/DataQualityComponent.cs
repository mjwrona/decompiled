// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQualityComponent
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
  public class DataQualityComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[17]
    {
      (IComponentCreator) new ComponentCreator<DataQualityComponent>(1, true),
      (IComponentCreator) new ComponentCreator<DataQualityComponent2>(2),
      (IComponentCreator) new ComponentCreator<DataQualityComponent2>(3),
      (IComponentCreator) new ComponentCreator<DataQualityComponent2>(4),
      (IComponentCreator) new ComponentCreator<DataQualityComponent2>(5),
      (IComponentCreator) new ComponentCreator<DataQualityComponent2>(6),
      (IComponentCreator) new ComponentCreator<DataQualityComponent2>(7),
      (IComponentCreator) new ComponentCreator<DataQualityComponent2>(8),
      (IComponentCreator) new ComponentCreator<DataQualityComponent2>(9),
      (IComponentCreator) new ComponentCreator<DataQualityComponent10>(10),
      (IComponentCreator) new ComponentCreator<DataQualityComponent11>(11),
      (IComponentCreator) new ComponentCreator<DataQualityComponent11>(12),
      (IComponentCreator) new ComponentCreator<DataQualityComponent11>(13),
      (IComponentCreator) new ComponentCreator<DataQualityComponent11>(14),
      (IComponentCreator) new ComponentCreator<DataQualityComponent15>(15),
      (IComponentCreator) new ComponentCreator<DataQualityComponent15>(16),
      (IComponentCreator) new ComponentCreator<DataQualityComponent15>(17)
    }, "DataQualityService");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    internal virtual IReadOnlyCollection<DataQualityResult> CheckDataQuality(
      DataQualityDefinition definition,
      int latencyExclusionSeconds,
      DateTime previousEndDate)
    {
      DateTime parameterValue = DateTime.UtcNow.AddSeconds((double) -latencyExclusionSeconds);
      this.PrepareStoredProcedure(definition.SprocName);
      this.BindDateTime("@compareEndDate", parameterValue);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DataQualityResult>((ObjectBinder<DataQualityResult>) new DataQualityColumns());
        return (IReadOnlyCollection<DataQualityResult>) resultCollection.GetCurrent<DataQualityResult>().Items;
      }
    }

    public DataQualityComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DataQualityComponent.s_sqlExceptionFactories;

    public virtual CleanupDataQualityResult CleanupDataQualityResults(int retainHistoryDays) => throw new NotImplementedException();

    public virtual IReadOnlyCollection<DataQualityResult> GetLatestDataQualityResults() => (IReadOnlyCollection<DataQualityResult>) new List<DataQualityResult>();

    public virtual void InitializeModelReady()
    {
    }
  }
}
