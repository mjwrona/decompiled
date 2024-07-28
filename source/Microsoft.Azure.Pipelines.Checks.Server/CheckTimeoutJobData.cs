// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.CheckTimeoutJobData
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  public class CheckTimeoutJobData
  {
    [XmlAttribute("ProjectId")]
    public Guid ProjectId { get; set; }

    [XmlAttribute("CheckSuiteId")]
    public Guid CheckSuiteId { get; set; }

    [XmlAttribute("CheckRunId")]
    public Guid CheckRunId { get; set; }

    public static CheckTimeoutJobData GetJobData(
      Guid projectId,
      Guid checkSuiteId,
      Guid checkRunId)
    {
      return new CheckTimeoutJobData()
      {
        ProjectId = projectId,
        CheckSuiteId = checkSuiteId,
        CheckRunId = checkRunId
      };
    }
  }
}
