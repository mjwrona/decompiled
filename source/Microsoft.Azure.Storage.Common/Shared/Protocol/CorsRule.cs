// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.CorsRule
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Shared.Protocol
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
