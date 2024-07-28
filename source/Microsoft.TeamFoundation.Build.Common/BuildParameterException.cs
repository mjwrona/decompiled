// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildParameterException
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.Common
{
  [Serializable]
  public class BuildParameterException : Exception
  {
    public BuildParameterException(string message)
      : base(message)
    {
    }

    public BuildParameterException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected BuildParameterException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
