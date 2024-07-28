// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.PipelineOperation
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public class PipelineOperation
  {
    [DataMember(Name = "Properties", EmitDefaultValue = false)]
    private ResourceProperties m_properties;
    public const string RetryOperationName = "Operations.Retry";

    [DataMember]
    public string Name { get; protected set; }

    public ResourceProperties Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new ResourceProperties();
        return this.m_properties;
      }
    }

    public static PipelineOperation CreateRetryOperation(string name)
    {
      PipelineOperation retryOperation = new PipelineOperation()
      {
        Name = "Operations.Retry"
      };
      if (!string.IsNullOrEmpty(name))
        retryOperation.Properties.Set<string>("Stage", name);
      return retryOperation;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      ResourceProperties properties = this.m_properties;
      if ((properties != null ? (properties.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_properties = (ResourceProperties) null;
    }
  }
}
