// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ArtifactBillingException
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [Serializable]
  public class ArtifactBillingException : VssServiceException
  {
    public const string DefaultExceptionMessage = "Artifact cannot be uploaded because max quantity has been exceeded or the payment instrument is invalid. https://aka.ms/artbilling for details.";

    public ArtifactBillingException()
      : base("Artifact cannot be uploaded because max quantity has been exceeded or the payment instrument is invalid. https://aka.ms/artbilling for details.")
    {
    }

    public ArtifactBillingException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public ArtifactBillingException(string message)
      : base(message)
    {
    }

    protected ArtifactBillingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
