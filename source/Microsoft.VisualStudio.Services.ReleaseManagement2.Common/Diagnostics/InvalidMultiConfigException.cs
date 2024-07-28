// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics.InvalidMultiConfigException
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Well known word in VSTS")]
  [Serializable]
  public class InvalidMultiConfigException : TeamFoundationServiceException
  {
    public InvalidMultiConfigException()
    {
    }

    public InvalidMultiConfigException(string message)
      : base(message)
    {
    }

    public InvalidMultiConfigException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected InvalidMultiConfigException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
