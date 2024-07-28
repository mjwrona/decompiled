// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.ItemAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E6307531-8252-47C3-B21C-ECA66F38ED4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ItemStore.Server
{
  [Serializable]
  public class ItemAlreadyExistsException : InvalidOperationException
  {
    public ItemAlreadyExistsException()
    {
    }

    public ItemAlreadyExistsException(string message)
      : base(message)
    {
    }

    public ItemAlreadyExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ItemAlreadyExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
