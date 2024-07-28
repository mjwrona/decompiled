// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.OtherConnectionConfig
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal class OtherConnectionConfig
  {
    private readonly Lazy<string> lazyString;
    private readonly Lazy<string> lazyJsonString;

    public OtherConnectionConfig(bool limitToEndpoint, bool allowBulkExecution)
    {
      OtherConnectionConfig connectionConfig = this;
      this.LimitToEndpoint = limitToEndpoint;
      this.AllowBulkExecution = allowBulkExecution;
      this.lazyString = new Lazy<string>((Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(ed:{0}, be:{1})", (object) limitToEndpoint, (object) allowBulkExecution)));
      this.lazyJsonString = new Lazy<string>((Func<string>) (() => JsonConvert.SerializeObject((object) connectionConfig)));
    }

    public bool LimitToEndpoint { get; }

    public bool AllowBulkExecution { get; }

    public override string ToString() => this.lazyString.Value;

    public string ToJsonString() => this.lazyJsonString.Value;
  }
}
