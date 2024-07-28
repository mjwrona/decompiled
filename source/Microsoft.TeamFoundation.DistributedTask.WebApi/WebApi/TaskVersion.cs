// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskVersion
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskVersion : IComparable<TaskVersion>, IEquatable<TaskVersion>
  {
    public TaskVersion()
    {
    }

    public TaskVersion(string version)
    {
      int major;
      int minor;
      int patch;
      string semanticVersion;
      VersionParser.ParseVersion(version, out major, out minor, out patch, out semanticVersion);
      this.Major = major;
      this.Minor = minor;
      this.Patch = patch;
      if (semanticVersion == null)
        return;
      if (!semanticVersion.Equals("test", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("semVer");
      this.IsTest = true;
    }

    private TaskVersion(TaskVersion taskVersionToClone)
    {
      this.IsTest = taskVersionToClone.IsTest;
      this.Major = taskVersionToClone.Major;
      this.Minor = taskVersionToClone.Minor;
      this.Patch = taskVersionToClone.Patch;
    }

    [DataMember]
    public int Major { get; set; }

    [DataMember]
    public int Minor { get; set; }

    [DataMember]
    public int Patch { get; set; }

    [DataMember]
    public bool IsTest { get; set; }

    public TaskVersion Clone() => new TaskVersion(this);

    public static implicit operator string(TaskVersion version) => version.ToString();

    public override string ToString()
    {
      string str = string.Empty;
      if (this.IsTest)
        str = "-test";
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}{3}", (object) this.Major, (object) this.Minor, (object) this.Patch, (object) str);
    }

    public override int GetHashCode() => this.ToString().GetHashCode();

    public int CompareTo(TaskVersion other)
    {
      int num = this.Major.CompareTo(other.Major);
      if (num == 0)
      {
        num = this.Minor.CompareTo(other.Minor);
        if (num == 0)
        {
          num = this.Patch.CompareTo(other.Patch);
          if (num == 0 && this.IsTest != other.IsTest)
            num = this.IsTest ? -1 : 1;
        }
      }
      return num;
    }

    public bool Equals(TaskVersion other) => (object) other != null && this.CompareTo(other) == 0;

    public override bool Equals(object obj) => this.Equals(obj as TaskVersion);

    public static bool operator ==(TaskVersion v1, TaskVersion v2) => (object) v1 == null ? (object) v2 == null : v1.Equals(v2);

    public static bool operator !=(TaskVersion v1, TaskVersion v2) => (object) v1 == null ? v2 != null : !v1.Equals(v2);

    public static bool operator <(TaskVersion v1, TaskVersion v2)
    {
      ArgumentUtility.CheckForNull<TaskVersion>(v1, nameof (v1));
      ArgumentUtility.CheckForNull<TaskVersion>(v2, nameof (v2));
      return v1.CompareTo(v2) < 0;
    }

    public static bool operator >(TaskVersion v1, TaskVersion v2)
    {
      ArgumentUtility.CheckForNull<TaskVersion>(v1, nameof (v1));
      ArgumentUtility.CheckForNull<TaskVersion>(v2, nameof (v2));
      return v1.CompareTo(v2) > 0;
    }

    public static bool operator <=(TaskVersion v1, TaskVersion v2)
    {
      ArgumentUtility.CheckForNull<TaskVersion>(v1, nameof (v1));
      ArgumentUtility.CheckForNull<TaskVersion>(v2, nameof (v2));
      return v1.CompareTo(v2) <= 0;
    }

    public static bool operator >=(TaskVersion v1, TaskVersion v2)
    {
      ArgumentUtility.CheckForNull<TaskVersion>(v1, nameof (v1));
      ArgumentUtility.CheckForNull<TaskVersion>(v2, nameof (v2));
      return v1.CompareTo(v2) >= 0;
    }
  }
}
