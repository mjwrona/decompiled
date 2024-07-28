// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosDiagnostics
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public abstract class CosmosDiagnostics
  {
    public virtual TimeSpan GetClientElapsedTime() => throw new NotImplementedException("CosmosDiagnostics.GetElapsedTime");

    public virtual DateTime? GetStartTimeUtc() => throw new NotImplementedException("CosmosDiagnostics.GetStartTimeUtc)");

    public virtual int GetFailedRequestCount() => throw new NotImplementedException("CosmosDiagnostics.GetFailedRequestCount");

    public abstract override string ToString();

    public abstract IReadOnlyList<(string regionName, Uri uri)> GetContactedRegions();
  }
}
