// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public sealed class TestResultAttachment
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string FileName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public AttachmentType AttachmentType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public long Length { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IterationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ActionPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid TmiRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int SessionId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string DownloadQueryString { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsComplete { get; set; }

    public void PopulateUrlField(string projectId, string serviceName) => this.DownloadQueryString = this.DownloadQueryString + "&" + string.Join("&", new Dictionary<string, string>()
    {
      {
        "project",
        projectId
      },
      {
        "testRunId",
        this.TestRunId.ToString()
      },
      {
        "testResultId",
        this.TestResultId.ToString()
      },
      {
        "attachmentId",
        this.Id.ToString()
      },
      {
        "storedIn",
        serviceName
      },
      {
        "sessionId",
        this.SessionId.ToString()
      }
    }.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key + "=" + kvp.Value)));
  }
}
