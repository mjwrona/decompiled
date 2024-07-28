// Decompiled with JetBrains decompiler
// Type: Nest.GetTaskResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class GetTaskResponse : ResponseBase
  {
    [DataMember(Name = "completed")]
    public bool Completed { get; internal set; }

    [DataMember(Name = "task")]
    public TaskInfo Task { get; internal set; }

    [DataMember(Name = "response")]
    internal LazyDocument Response { get; set; }

    public TResponse GetResponse<TResponse>() where TResponse : class, IResponse
    {
      LazyDocument response = this.Response;
      return response == null ? default (TResponse) : response.AsUsingRequestResponseSerializer<TResponse>();
    }
  }
}
