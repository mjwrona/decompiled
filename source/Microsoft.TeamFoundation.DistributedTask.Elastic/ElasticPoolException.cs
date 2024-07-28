// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolException
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  [Serializable]
  public abstract class ElasticPoolException : VssServiceException
  {
    [Obsolete("All exceptions are supposed to have an empty constructor, but they're useless. Use one of the other constructors.")]
    protected ElasticPoolException()
    {
    }

    protected ElasticPoolException(string message)
      : base(message)
    {
    }

    protected ElasticPoolException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ElasticPoolException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
