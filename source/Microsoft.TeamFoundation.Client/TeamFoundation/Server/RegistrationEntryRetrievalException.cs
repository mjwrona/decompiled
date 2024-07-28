// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RegistrationEntryRetrievalException
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class RegistrationEntryRetrievalException : TeamFoundationServerException
  {
    public RegistrationEntryRetrievalException()
    {
    }

    public RegistrationEntryRetrievalException(string message)
      : base(message)
    {
    }

    public RegistrationEntryRetrievalException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected RegistrationEntryRetrievalException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
