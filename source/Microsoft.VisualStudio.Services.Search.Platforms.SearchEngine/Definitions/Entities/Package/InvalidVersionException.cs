// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package.InvalidVersionException
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package
{
  [Serializable]
  public class InvalidVersionException : VssServiceException
  {
    public InvalidVersionException()
    {
    }

    public InvalidVersionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public InvalidVersionException(string message)
      : base(message)
    {
    }

    protected InvalidVersionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
