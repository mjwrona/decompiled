// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.NetworkPathsNotFoundException
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  [Serializable]
  public class NetworkPathsNotFoundException : IOException
  {
    public NetworkPathsNotFoundException()
    {
    }

    public NetworkPathsNotFoundException(string message)
      : base(message)
    {
    }

    public NetworkPathsNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected NetworkPathsNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
