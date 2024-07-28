// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.TooManyRequestedItemsException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph
{
  [Serializable]
  public class TooManyRequestedItemsException : GraphException
  {
    [DataMember]
    public int? RequestedCount { get; set; }

    [DataMember]
    public int? MaxLimit { get; set; }

    public TooManyRequestedItemsException()
      : base(IdentityResources.TooManyRequestedItemsError())
    {
    }

    public TooManyRequestedItemsException(string message)
      : base(message)
    {
    }

    public TooManyRequestedItemsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected TooManyRequestedItemsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public TooManyRequestedItemsException(int providedCount, int maxCount)
      : base(IdentityResources.TooManyRequestedItemsErrorWithCount((object) providedCount, (object) maxCount))
    {
      this.RequestedCount = new int?(providedCount);
      this.MaxLimit = new int?(maxCount);
    }
  }
}
