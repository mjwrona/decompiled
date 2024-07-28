// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultExArchivalRecord
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestResultExArchivalRecord
  {
    public DateTime TestRunUpdatedDate { get; internal set; }

    public int TestRunId { get; internal set; }

    public int TestResultId { get; internal set; }

    public string FieldName { get; internal set; }

    public string FieldValue { get; internal set; }

    public Guid ProjectGuid { get; internal set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public DateTime EtlIngestDate => DateTime.UtcNow;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string EtlEntity => "testresultextension";
  }
}
