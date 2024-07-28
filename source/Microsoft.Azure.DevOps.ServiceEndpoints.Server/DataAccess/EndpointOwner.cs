// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess.EndpointOwner
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using System;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.DataAccess
{
  [Flags]
  internal enum EndpointOwner
  {
    Library = 1,
    AgentCloud = 2,
    Boards = 4,
    Environment = 8,
  }
}
