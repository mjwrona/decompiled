// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.MicrosoftGraphRequestDeniedException
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory
{
  [Serializable]
  public class MicrosoftGraphRequestDeniedException : MicrosoftGraphException
  {
    public MicrosoftGraphRequestDeniedException(Exception innerException = null)
      : base("Insufficient privileges to complete the operation in Microsoft Graph", innerException)
    {
    }
  }
}
