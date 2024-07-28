// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PluginActivationException
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation
{
  [Serializable]
  public class PluginActivationException : ApplicationException
  {
    public PluginActivationException()
    {
    }

    public PluginActivationException(string message)
      : base(message)
    {
    }

    public PluginActivationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected PluginActivationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
