// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ServicingOrchestrationRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [DataContract]
  [JsonConverter(typeof (ServicingOrchestrationJsonConverter<ServicingOrchestrationRequest>))]
  public abstract class ServicingOrchestrationRequest
  {
    public ServicingOrchestrationRequest() => this.Properties = new PropertyCollection();

    public ServicingOrchestrationRequest(ServicingOrchestrationRequest other)
    {
      this.RequestId = other.RequestId;
      this.ServicingJobId = other.ServicingJobId;
      this.Properties = other.Properties.Clone();
    }

    [DataMember]
    public string TypeName => this.GetType().FullName;

    [DataMember]
    public Guid RequestId { get; set; }

    [XmlElement("JobId")]
    [DataMember(Name = "JobId")]
    public Guid ServicingJobId { get; set; }

    [DataMember]
    public PropertyCollection Properties { get; set; }

    [IgnoreDataMember]
    public abstract string JobPluginName { get; }

    [IgnoreDataMember]
    public abstract bool IsRootRequest { get; }

    public override string ToString() => string.Format("[Type={0}, RequestId={1}, ServicingJobId={2}, Properties={3}]", (object) this.TypeName, (object) this.RequestId, (object) this.ServicingJobId, (object) this.Properties);
  }
}
