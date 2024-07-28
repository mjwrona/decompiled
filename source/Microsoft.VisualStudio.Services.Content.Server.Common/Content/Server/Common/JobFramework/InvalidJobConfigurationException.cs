// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.InvalidJobConfigurationException
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  [Serializable]
  public class InvalidJobConfigurationException : Exception
  {
    public const string DefaultExceptionMessage = " Job has been or is being, incorrectly configured. See https://aka.ms/jobconfig for details.";

    public InvalidJobConfigurationException()
      : base(" Job has been or is being, incorrectly configured. See https://aka.ms/jobconfig for details.")
    {
    }

    public InvalidJobConfigurationException(string message)
      : base(message + " Job has been or is being, incorrectly configured. See https://aka.ms/jobconfig for details.")
    {
    }

    public InvalidJobConfigurationException(Exception ex)
      : base(" Job has been or is being, incorrectly configured. See https://aka.ms/jobconfig for details.", ex)
    {
    }

    public InvalidJobConfigurationException(string message, Exception ex)
      : base(message, ex)
    {
    }
  }
}
