// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.StorageKeyNotFoundException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph
{
  [Serializable]
  public class StorageKeyNotFoundException : GraphException
  {
    public StorageKeyNotFoundException()
    {
    }

    public StorageKeyNotFoundException(string message)
      : base(message)
    {
    }

    public StorageKeyNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected StorageKeyNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public StorageKeyNotFoundException(SubjectDescriptor descriptor)
      : base(GraphResources.StorageKeyNotFound((object) descriptor))
    {
    }
  }
}
