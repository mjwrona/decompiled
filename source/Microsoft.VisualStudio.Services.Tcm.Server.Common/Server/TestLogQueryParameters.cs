// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogQueryParameters
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestLogQueryParameters
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestLogType Type { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DirectoryPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string FileNamePrefix { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool FetchMetaData { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ContinuationToken { get; set; }
  }
}
