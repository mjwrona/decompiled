// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.DatabaseInfo
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public struct DatabaseInfo : IEquatable<DatabaseInfo>
  {
    public string DatabaseName { get; set; }

    public string MachineName { get; set; }

    public string ServiceName { get; set; }

    public bool Equals(DatabaseInfo other) => string.Equals(this.DatabaseName, other.DatabaseName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.MachineName, other.MachineName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.ServiceName, other.ServiceName, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object obj) => obj is DatabaseInfo other && this.Equals(other);

    public override int GetHashCode() => DatabaseInfo.GetHashCode(this.DatabaseName) ^ DatabaseInfo.GetHashCode(this.MachineName) ^ DatabaseInfo.GetHashCode(this.ServiceName);

    private static int GetHashCode(string value) => value != null ? value.GetHashCode() : 0;

    public static bool operator ==(DatabaseInfo op1, DatabaseInfo op2) => op1.Equals(op2);

    public static bool operator !=(DatabaseInfo op1, DatabaseInfo op2) => !op1.Equals(op2);
  }
}
