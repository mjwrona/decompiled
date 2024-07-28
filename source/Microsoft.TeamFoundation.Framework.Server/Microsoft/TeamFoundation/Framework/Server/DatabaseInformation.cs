// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseInformation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Name: {Name}, Owner: {Owner}, Collation: {Collation}")]
  public class DatabaseInformation
  {
    public ReadOnlyCollection<string> SystemDatabases = Array.AsReadOnly<string>(new string[4]
    {
      TeamFoundationSqlResourceComponent.Master,
      "msdb",
      "model",
      "tempdb"
    });

    public bool AccentSensitive
    {
      get
      {
        string collation = this.Collation;
        return !string.IsNullOrEmpty(collation) && collation.IndexOf("_AI", StringComparison.OrdinalIgnoreCase) < 0;
      }
    }

    public bool AutoClose { get; set; }

    public bool AutoShrink { get; set; }

    public string AvailabilityGroupName { get; set; }

    public string AvailabilityGroupListenerName { get; set; }

    public int AvailabilityGroupListenerPort { get; set; }

    public string AvailabilityGroupListenerIPConfiguration { get; set; }

    public bool BrokerEnabled { get; set; }

    public bool CaseSensitive
    {
      get
      {
        string collation = this.Collation;
        return !string.IsNullOrEmpty(collation) && collation.IndexOf("_CI", StringComparison.OrdinalIgnoreCase) < 0;
      }
    }

    public bool CanTransitionToSimpleRecoveryModel => string.IsNullOrEmpty(this.AvailabilityGroupName) && this.MirroringId == Guid.Empty;

    public string Collation { get; set; }

    public DatabaseCompatibilityLevel CompatibilityLevel { get; set; }

    public DateTime CreateDate { get; set; }

    public bool FullTextEnabled { get; set; }

    public DatabaseParameterization DatabaseParameterization { get; set; }

    public bool EncryptionEnabled { get; set; }

    public int Id { get; set; }

    public bool IsReadCommittedSnapshotOn { get; set; }

    public bool IsSystem => this.SystemDatabases.FirstOrDefault<string>((Func<string, bool>) (db => VssStringComparer.DatabaseName.Equals(db, this.Name))) != null;

    public bool MirroringEnabled => this.MirroringId != Guid.Empty;

    public Guid MirroringId { get; set; }

    public string Name { get; set; }

    public string Owner { get; set; }

    public byte[] OwnerSid { get; set; }

    public bool ReadOnly { get; set; }

    public DatabaseRecoveryModel RecoveryModel { get; set; }

    public DatabaseSnapshotIsolationState SnapshotIsolationState { get; set; }

    public DatabaseState State { get; set; }

    public bool SupportsSupplementaryCharacters
    {
      get
      {
        string collation = this.Collation;
        return !string.IsNullOrEmpty(collation) && collation.IndexOf("_SC", StringComparison.OrdinalIgnoreCase) > 0;
      }
    }

    public DatabaseUserAccess UserAccess { get; set; }
  }
}
