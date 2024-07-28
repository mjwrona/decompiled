// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IRequestRetryPolicy`3
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

namespace Microsoft.Azure.Documents
{
  internal interface IRequestRetryPolicy<TPolicyContext, TRequest, TResponse> : 
    IRequestRetryPolicy<TRequest, TResponse>
  {
    TPolicyContext ExecuteContext { get; }
  }
}
