// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceValidationException
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ResourceValidationException : PipelineException
  {
    public ResourceValidationException(string message)
      : base(message)
    {
    }

    public ResourceValidationException(string message, string propertyName)
      : base(message)
    {
      this.PropertyName = propertyName;
    }

    public ResourceValidationException(
      string message,
      string propertyName,
      Exception innerException)
      : base(message, innerException)
    {
      this.PropertyName = propertyName;
    }

    public ResourceValidationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ResourceValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public string PropertyName { get; }
  }
}
