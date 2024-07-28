// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.CorsProperties
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  public sealed class CorsProperties
  {
    public CorsProperties() => this.CorsRules = (IList<CorsRule>) new List<CorsRule>();

    public IList<CorsRule> CorsRules { get; internal set; }
  }
}
