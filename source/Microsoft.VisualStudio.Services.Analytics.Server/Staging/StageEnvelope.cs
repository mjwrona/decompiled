// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Staging.StageEnvelope
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.Staging
{
  public class StageEnvelope
  {
    internal const bool ReplaceDefault = false;
    internal const bool KeysOnlyDefault = false;

    public int ContentVersion { get; set; }

    public int ProviderStageVersion { get; set; }

    public int StageVersion { get; set; }

    public string FromWatermark { get; set; }

    public string ToWatermark { get; set; }

    public bool? IsCurrent { get; set; }

    public DateTime? SyncDate { get; set; }

    public bool? Replace { get; set; } = new bool?(false);

    public bool? KeysOnly { get; set; } = new bool?(false);

    public ICollection<SqlDataRecord> Records { get; set; }

    public ICollection<SqlDataRecord> ExtendedFields { get; set; }

    public ICollection<SqlDataRecord> DeleteKeys { get; set; }

    public Dictionary<string, int> PropertyParseErrorCounts { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this);
  }
}
