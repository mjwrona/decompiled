// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.CheckExpirationJobData
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  public class CheckExpirationJobData
  {
    [XmlAttribute("ProjectId")]
    public Guid ProjectId { get; set; }

    [XmlArray("Resources")]
    public Resource[] Resources { get; set; }

    [XmlAttribute("EvaluationContext")]
    public string EvaluationContext { get; set; }

    [XmlAttribute("CheckRunId")]
    public Guid CheckRunId { get; set; }

    [XmlAttribute("CheckConfiguration")]
    public string CheckConfiguration { get; set; }

    public static CheckExpirationJobData GetJobData(
      Guid projectId,
      List<Resource> resources,
      string evaluationContext,
      Guid checkRunId,
      string checkconfiguration)
    {
      return new CheckExpirationJobData()
      {
        ProjectId = projectId,
        Resources = resources.ToArray(),
        EvaluationContext = evaluationContext,
        CheckRunId = checkRunId,
        CheckConfiguration = checkconfiguration
      };
    }
  }
}
