// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Common.ItemTooBigException
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8190F04D-5888-4DB5-A838-8C98A67C6E45
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ItemStore.Server.Common
{
  [Serializable]
  public class ItemTooBigException : Exception
  {
    public ItemTooBigException()
    {
    }

    public ItemTooBigException(string message)
      : base(message)
    {
    }

    public ItemTooBigException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ItemTooBigException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
