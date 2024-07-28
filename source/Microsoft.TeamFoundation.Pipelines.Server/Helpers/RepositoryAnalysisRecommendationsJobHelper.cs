// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Helpers.RepositoryAnalysisRecommendationsJobHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.Pipelines.Server.Helpers
{
  public static class RepositoryAnalysisRecommendationsJobHelper
  {
    private const string c_jobName = "Repository analysis recommendations job";
    private const string c_jobClass = "Microsoft.TeamFoundation.Pipelines.Server.Extensions.RepositoryAnalysisRecommendationsJobExtension";

    public static Guid QueueJob(IVssRequestContext requestContext, RecommendationsInputs inputs) => requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, "Repository analysis recommendations job", "Microsoft.TeamFoundation.Pipelines.Server.Extensions.RepositoryAnalysisRecommendationsJobExtension", RepositoryAnalysisRecommendationsJobHelper.InputsToXml(inputs), JobPriorityLevel.Normal, JobPriorityClass.AboveNormal, TimeSpan.Zero);

    public static RecommendationsInputs XmlToInputs(XmlNode data) => JsonConvert.DeserializeObject<RepositoryAnalysisRecommendationsJobHelper.InputsWrapper>(JsonConvert.SerializeXmlNode(data)).Inputs;

    public static XmlNode InputsToXml(RecommendationsInputs inputs) => (XmlNode) JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject((object) new RepositoryAnalysisRecommendationsJobHelper.InputsWrapper(inputs)));

    private class InputsWrapper
    {
      public InputsWrapper(RecommendationsInputs inputs) => this.Inputs = inputs;

      public RecommendationsInputs Inputs { get; set; }
    }
  }
}
