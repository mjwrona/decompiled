// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.Exceptions.InvalidChartDimensionsException
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Boards.Charts.Exceptions
{
  [Serializable]
  public class InvalidChartDimensionsException : TeamFoundationServiceException
  {
    public InvalidChartDimensionsException(string message)
      : base(message)
    {
    }

    public InvalidChartDimensionsException(int maxChartDimension)
      : base(string.Format(ChartsResources.InvalidChartDimensionsMessage, (object) maxChartDimension))
    {
    }
  }
}
