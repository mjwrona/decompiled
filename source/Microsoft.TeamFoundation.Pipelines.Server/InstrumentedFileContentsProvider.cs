// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.InstrumentedFileContentsProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class InstrumentedFileContentsProvider : IFileContentsProvider
  {
    private readonly IFileContentsProvider m_actualContentsProvider;

    public InstrumentedFileContentsProvider(IFileContentsProvider actualContentsProvider)
    {
      ArgumentUtility.CheckForNull<IFileContentsProvider>(actualContentsProvider, nameof (actualContentsProvider));
      this.m_actualContentsProvider = actualContentsProvider;
    }

    internal StatisticsBuilder StatisticsBuilder { get; private set; }

    public void StartGatheringStatistics(
      string detectorName,
      BuildFrameworkDetectionType detectionType,
      int totalFileCount)
    {
      this.StatisticsBuilder = new StatisticsBuilder(detectorName, detectionType, totalFileCount);
    }

    public DetectorStats GetStatistics() => this.StatisticsBuilder?.Build();

    public virtual string GetFileContents(IVssRequestContext requestContext, string filePath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(filePath, nameof (filePath));
      string fileContents = this.m_actualContentsProvider.GetFileContents(requestContext, filePath);
      if (string.IsNullOrEmpty(fileContents))
        return fileContents;
      StatisticsBuilder statisticsBuilder = this.StatisticsBuilder;
      if (statisticsBuilder == null)
        return fileContents;
      statisticsBuilder.IncrementFileReadCount();
      return fileContents;
    }
  }
}
