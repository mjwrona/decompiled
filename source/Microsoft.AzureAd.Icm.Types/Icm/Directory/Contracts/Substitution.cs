// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.Substitution
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.Icm.Directory.Contracts
{
  public class Substitution
  {
    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public long TeamId { get; set; }

    public long OriginalContactId { get; set; }

    public long SubstituteContactId { get; set; }

    public short OnCallPosition { get; set; }

    public long? IcmSubstitutionId { get; set; }

    public Guid? SubstitutionCollectionId { get; set; }

    public string CreatedBy { get; set; }

    public DateTimeOffset CreatedTime { get; set; }

    public string ModifiedBy { get; set; }

    public DateTimeOffset ModifiedTime { get; set; }
  }
}
