// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.InvalidPartitionSizeException
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  [Serializable]
  public class InvalidPartitionSizeException : Exception
  {
    public InvalidPartitionSizeException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public InvalidPartitionSizeException(string message)
      : this(message, (Exception) null)
    {
    }
  }
}
