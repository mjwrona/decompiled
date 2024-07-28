// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.AggregateNotFoundException
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  [Serializable]
  public class AggregateNotFoundException : TeamFoundationServiceException
  {
    public AggregateNotFoundException(string message)
      : base(message)
    {
    }
  }
}
