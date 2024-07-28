// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration.ImportSettings
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.DataImport;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration
{
  public abstract class ImportSettings
  {
    private const string TransferRegistryKeyBase = "/Configuration/DataImport/";
    private const string TransferLastPKWatermarkKey = "/Configuration/DataImport/{0}/{1}/{2}PKWatermark";
    private const string TransferCheckpointKey = "/Configuration/DataImport/{0}TransferCheckpoint";
    private const string TransferParallelismKey = "/Configuration/DataImport/{0}TransferParallelism";
    private readonly Lazy<IASTableRetriever> m_retriever;
    private readonly Lazy<AzureTableUploader> m_uploader;
    private string m_watermarkKey;

    public ImportSettings(
      string category,
      string tablePrefix,
      IServicingContext servicingContext,
      IVssRequestContext requestContext,
      IVssRequestContext deploymentContext,
      int sourceDatabaseId)
    {
      this.m_retriever = new Lazy<IASTableRetriever>((Func<IASTableRetriever>) (() => this.CreateASTableRetriever()));
      this.m_uploader = new Lazy<AzureTableUploader>((Func<AzureTableUploader>) (() => this.CreateAzureTableUploader()));
      this.TablePrefix = tablePrefix;
      this.TableId = SQLTableAdapter.ConvertTableNameToId(tablePrefix);
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      this.TransferCheckpoint = service.GetValue<int>(deploymentContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Configuration/DataImport/{0}TransferCheckpoint", (object) category), false, this.TransferCheckpoint);
      this.TransferParallelism = service.GetValue<int>(deploymentContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Configuration/DataImport/{0}TransferParallelism", (object) category), false, this.TransferParallelism);
      this.m_watermarkKey = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Configuration/DataImport/{0}/{1}/{2}PKWatermark", (object) servicingContext.GetDataImportId(), (object) category, (object) tablePrefix);
      this.WatermarkValue = ImportSettings.GetPkMin(deploymentContext, this.m_watermarkKey);
      try
      {
        this.DatabaseProperties = deploymentContext.GetService<ITeamFoundationDatabaseManagementService>().GetDatabase(deploymentContext, sourceDatabaseId);
      }
      catch (DatabaseNotFoundException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("The source database was not found: {0}", (object) sourceDatabaseId), ex);
      }
      this.PartitionId = this.QueryPartitionIdOnSourceDatabase(this.DatabaseProperties);
      this.TableRequestOptions = new TableRequestOptions()
      {
        MaximumExecutionTime = new TimeSpan?(TimeSpan.FromMinutes(20.0)),
        RetryPolicy = (IRetryPolicy) new ExponentialRetry(TimeSpan.FromSeconds(1.0), 10)
      };
      this.Logger = servicingContext.TFLogger;
      this.SourceProcessor = VssRequestPump.Processor.CreateWithNullRequestContext(requestContext.ActivityId, requestContext.E2EId, requestContext.UniqueIdentifier, requestContext.CancellationToken);
      this.TargetProcessor = VssRequestPump.Processor.CreatePassthroughForOriginalThread(requestContext);
    }

    internal ITeamFoundationDatabaseProperties DatabaseProperties { get; private set; }

    internal int PartitionId { get; private set; }

    internal int TableId { get; private set; }

    internal string TablePrefix { get; private set; }

    internal string WatermarkValue { get; private set; }

    internal ITFLogger Logger { get; private set; }

    internal VssRequestPump.Processor SourceProcessor { get; private set; }

    internal VssRequestPump.Processor TargetProcessor { get; private set; }

    public IASTableRetriever ASTableRetriever => this.m_retriever.Value;

    public AzureTableUploader AzureTableUploader => this.m_uploader.Value;

    public int TransferCheckpoint { get; set; } = 1000;

    public int TransferParallelism { get; set; } = 32;

    public TableRequestOptions TableRequestOptions { get; private set; }

    internal virtual int TableBatchSize => 50;

    protected virtual IASTableRetriever CreateASTableRetriever() => (IASTableRetriever) new Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration.ASTableRetriever(this);

    protected abstract AzureTableUploader CreateAzureTableUploader();

    internal void UpdateWatermark(IVssRequestContext requestContext, string lastPk)
    {
      this.WatermarkValue = lastPk;
      ImportSettings.SetPkMin(requestContext.To(TeamFoundationHostType.Deployment), this.m_watermarkKey, this.WatermarkValue);
    }

    internal virtual int QueryPartitionIdOnSourceDatabase(ITeamFoundationDatabaseProperties dbProps)
    {
      using (DatabasePartitionComponent componentRaw = dbProps.SqlConnectionInfo.CreateComponentRaw<DatabasePartitionComponent>())
        return componentRaw.QueryOnlyPartition().PartitionId;
    }

    private static string GetPkMin(IVssRequestContext deploymentContext, string pkMinWatermarkKey) => deploymentContext.GetService<IVssRegistryService>().GetValue(deploymentContext, (RegistryQuery) pkMinWatermarkKey, false, string.Empty);

    private static void SetPkMin(
      IVssRequestContext deploymentContext,
      string pkMinWatermarkKey,
      string pkMin)
    {
      deploymentContext.GetService<IVssRegistryService>().SetValue<string>(deploymentContext, pkMinWatermarkKey, pkMin);
    }
  }
}
