// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ResultCallbackInfo
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class ResultCallbackInfo
  {
    public ResultCallbackInfo(Uri url, string signatureKey)
    {
      this.SignatureKey = signatureKey;
      this.Url = url;
    }

    public string SignatureKey { get; }

    public Uri Url { get; }
  }
}
