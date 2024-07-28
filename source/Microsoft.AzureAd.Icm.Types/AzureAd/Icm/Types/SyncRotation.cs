// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.SyncRotation
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AzureAd.Icm.Types
{
  public class SyncRotation
  {
    [Required]
    public long TeamId { get; set; }

    [Required]
    public string TimeZoneId { get; set; }

    [Required]
    public string Scheme { get; set; }

    public string RotationTransitionDay { get; set; }

    [Required]
    [Range(1, 7)]
    public short Length { get; set; }

    [Required]
    public DateTimeOffset? SeedDate { get; set; }

    [Required]
    public DateTimeOffset EffectiveDate { get; set; }

    [Required]
    public TimeSpan TransitionTimeOfDay { get; set; }

    [Required]
    [Range(1, 32767)]
    public short BackupCount { get; set; }

    public DateTimeOffset CurrentRotationStartTime { get; set; }

    public IList<SyncRotation.SyncShiftAndRotationList> ShiftsAndRotationsLists { get; set; }

    public IList<SyncRotation.SyncCurrentOnCall> CurrentOnCalls { get; set; }

    public DateTimeOffset ModifiedTime { get; set; }

    public DateTimeOffset? LastClickUpdateCurrentOnCallTime { get; set; }

    public class SyncShiftAndRotationList
    {
      [Required]
      public long ShiftId { get; set; }

      [Required]
      public long ContactId { get; set; }

      [Required]
      public short RotationIndex { get; set; }

      [Required]
      public short RotationListPosition { get; set; }

      public string ShiftName { get; set; }

      [Required]
      public short ShiftOrder { get; set; }

      [Required]
      [Range(1, 32767)]
      public short ShiftDuration { get; set; }
    }

    public class SyncCurrentOnCall
    {
      [Required]
      public long ShiftId { get; set; }

      [Required]
      public long ContactId { get; set; }

      [Required]
      public long OriginalContactId { get; set; }

      [Required]
      public short CurrentOnCallPosition { get; set; }

      [Required]
      public string ShiftName { get; set; }

      [Required]
      public short ShiftOrder { get; set; }

      [Required]
      public short ShiftDuration { get; set; }
    }
  }
}
