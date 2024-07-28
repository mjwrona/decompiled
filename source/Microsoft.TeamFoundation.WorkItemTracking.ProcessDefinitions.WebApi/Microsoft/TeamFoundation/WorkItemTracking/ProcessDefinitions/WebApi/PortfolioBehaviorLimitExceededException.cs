// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.PortfolioBehaviorLimitExceededException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BABD213-FC9A-4DAB-8690-D2FF2DA1955C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi
{
  [Serializable]
  public class PortfolioBehaviorLimitExceededException : VssServiceException
  {
    public PortfolioBehaviorLimitExceededException(string message)
      : base(message)
    {
    }
  }
}
