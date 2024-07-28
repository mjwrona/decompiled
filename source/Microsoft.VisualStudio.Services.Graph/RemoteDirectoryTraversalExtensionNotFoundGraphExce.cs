// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.RemoteDirectoryTraversalExtensionNotFoundGraphException
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using System;

namespace Microsoft.VisualStudio.Services.Graph
{
  [Serializable]
  public class RemoteDirectoryTraversalExtensionNotFoundGraphException : GraphException
  {
    public RemoteDirectoryTraversalExtensionNotFoundGraphException()
      : base(string.Empty)
    {
    }

    public RemoteDirectoryTraversalExtensionNotFoundGraphException(string message)
      : base(message)
    {
    }

    public RemoteDirectoryTraversalExtensionNotFoundGraphException(
      string message,
      Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
