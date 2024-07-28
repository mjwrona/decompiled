// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildRepositoryNameAndUrl
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using System;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public class BuildRepositoryNameAndUrl
  {
    public string Name { get; set; }

    public Uri Url { get; set; }
  }
}
