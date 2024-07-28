// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Exceptions.MavenGavException
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Exceptions
{
  [Serializable]
  public class MavenGavException : VssException
  {
    public MavenGavException()
    {
    }

    public MavenGavException(string message)
      : base(message)
    {
    }

    public MavenGavException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected MavenGavException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
