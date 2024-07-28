// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ElasticsearchIndexSettingsUpdaterJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class ElasticsearchIndexSettingsUpdaterJob : ITeamFoundationJobExtension
  {
    private const int TracePoint = 1080038;
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1080038, "Indexing Pipeline", "Job");

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      requestContext.CheckDeploymentRequestContext();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(ElasticsearchIndexSettingsUpdaterJob.s_traceMetaData, nameof (Run));
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 49);
        ExecutionContext executionContext = requestContext.GetExecutionContext(correlationDetails);
        ISearchPlatform searchPlatform = SearchPlatformFactory.GetInstance().Create(executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString, executionContext.ServiceSettings.JobAgentSearchPlatformSettings, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
        ElasticsearchIndexSettingsUpdaterJob.JobData jobData = ElasticsearchIndexSettingsUpdaterJob.JobData.Deserialize(jobDefinition.Data);
        CatResponse<CatIndicesRecord> indices = searchPlatform.GetIndices(executionContext);
        if (indices != null && indices.Records.Any<CatIndicesRecord>())
        {
          foreach (ElasticsearchIndexSettingsUpdaterJob.IndexSettingData indexSetting in jobData.IndexSettings)
            searchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(indexSetting.Index)).UpdateSettings(executionContext, indexSetting.Setting, (object) indexSetting.Value);
        }
        else
          stringBuilder.Append("No index found in search platform to apply the settings on. Bailing out. ");
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Failed to update Elasticsearch settings with exception [{0}].", (object) ex)));
        return TeamFoundationJobExecutionResult.Failed;
      }
      finally
      {
        resultMessage = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(ElasticsearchIndexSettingsUpdaterJob.s_traceMetaData, nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    public class JobData
    {
      public List<ElasticsearchIndexSettingsUpdaterJob.IndexSettingData> IndexSettings { get; set; }

      public JobData() => this.IndexSettings = new List<ElasticsearchIndexSettingsUpdaterJob.IndexSettingData>();

      [SuppressMessage("Microsoft.Security.Xml", "CA3053", Justification = "PR build is reporting this issue even though it is fixed")]
      public static ElasticsearchIndexSettingsUpdaterJob.JobData Deserialize(XmlNode xmlNode)
      {
        if (xmlNode == null)
          throw new ArgumentNullException(nameof (xmlNode));
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (ElasticsearchIndexSettingsUpdaterJob.JobData));
        using (MemoryStream memoryStream = new MemoryStream())
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.XmlResolver = (XmlResolver) null;
          xmlDocument.AppendChild(xmlDocument.ImportNode(xmlNode, true));
          xmlDocument.Save((Stream) memoryStream);
          memoryStream.Position = 0L;
          XmlReader xmlReader = XmlReader.Create((Stream) memoryStream, new XmlReaderSettings()
          {
            XmlResolver = (XmlResolver) null
          });
          return (ElasticsearchIndexSettingsUpdaterJob.JobData) xmlSerializer.Deserialize(xmlReader);
        }
      }

      [SuppressMessage("Microsoft.Security.Xml", "CA3053", Justification = "PR build is reporting this issue even though it is fixed")]
      public XmlNode Serialize()
      {
        XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
        using (MemoryStream input = new MemoryStream())
        {
          try
          {
            xmlSerializer.Serialize((Stream) input, (object) this);
          }
          catch (InvalidOperationException ex)
          {
            return (XmlNode) null;
          }
          input.Position = 0L;
          XmlReader reader = XmlReader.Create((Stream) input, new XmlReaderSettings()
          {
            XmlResolver = (XmlResolver) null
          });
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.XmlResolver = (XmlResolver) null;
          xmlDocument.Load(reader);
          return (XmlNode) xmlDocument.DocumentElement;
        }
      }
    }

    public class IndexSettingData
    {
      public string Index { get; set; }

      public string Setting { get; set; }

      public string Value { get; set; }
    }
  }
}
