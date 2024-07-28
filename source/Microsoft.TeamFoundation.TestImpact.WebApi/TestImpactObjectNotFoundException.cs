// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.WebApi.TestImpactObjectNotFoundException
// Assembly: Microsoft.TeamFoundation.TestImpact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7794F4D7-E029-40EA-A47C-F590EB851438
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.TestImpact.WebApi
{
  [Serializable]
  public class TestImpactObjectNotFoundException : TestImpactServiceException
  {
    public TestImpactObjectNotFoundException(string message)
      : this(message, (Exception) null)
    {
    }

    public TestImpactObjectNotFoundException(string message, Exception ex)
      : base(message, ex)
    {
    }
  }
}
