// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.CorsRule
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class CorsRule
  {
    private IList<string> allowedOrigins;
    private IList<string> exposedHeaders;
    private IList<string> allowedHeaders;

    public IList<string> AllowedOrigins
    {
      get => this.allowedOrigins ?? (this.allowedOrigins = (IList<string>) new List<string>());
      set => this.allowedOrigins = value;
    }

    public IList<string> ExposedHeaders
    {
      get => this.exposedHeaders ?? (this.exposedHeaders = (IList<string>) new List<string>());
      set => this.exposedHeaders = value;
    }

    public IList<string> AllowedHeaders
    {
      get => this.allowedHeaders ?? (this.allowedHeaders = (IList<string>) new List<string>());
      set => this.allowedHeaders = value;
    }

    public CorsHttpMethods AllowedMethods { get; set; }

    public int MaxAgeInSeconds { get; set; }
  }
}
