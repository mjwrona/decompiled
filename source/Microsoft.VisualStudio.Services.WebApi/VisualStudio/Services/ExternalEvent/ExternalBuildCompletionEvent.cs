// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalBuildCompletionEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  [DataContract]
  public class ExternalBuildCompletionEvent
  {
    [IgnoreDataMember]
    public static ApiResourceVersion CurrentVersion = new ApiResourceVersion(new Version(1, 0), 1);
    [DataMember]
    public string PublisherId;
    [DataMember]
    public string SourceId;
    [DataMember]
    public string Id;
    [DataMember]
    public string Name;
    [DataMember]
    public ExternalBuildStatus Status;
    [DataMember]
    public TimeSpan Duration;
    [DataMember]
    public DateTime StartTime;
    [DataMember]
    public string StartedBy;
    [DataMember]
    public string Details;
    [DataMember]
    public IDictionary<string, string> Properties;

    [DataMember]
    public string PipelineEventId { get; set; }
  }
}
