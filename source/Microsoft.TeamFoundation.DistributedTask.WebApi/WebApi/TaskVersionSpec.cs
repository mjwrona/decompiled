// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskVersionSpec
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public sealed class TaskVersionSpec
  {
    public int? Major { get; set; }

    public int? Minor { get; set; }

    public int? Patch { get; set; }

    public bool IsTest { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      if (!this.Major.HasValue)
      {
        stringBuilder1.Append("*");
      }
      else
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        int? nullable = this.Major;
        int num = nullable.Value;
        stringBuilder2.Append(num);
        nullable = this.Minor;
        if (nullable.HasValue)
        {
          StringBuilder stringBuilder3 = stringBuilder1;
          CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
          nullable = this.Minor;
          // ISSUE: variable of a boxed type
          __Boxed<int> local1 = (ValueType) nullable.Value;
          stringBuilder3.AppendFormat((IFormatProvider) invariantCulture1, ".{0}", (object) local1);
          nullable = this.Patch;
          if (nullable.HasValue)
          {
            StringBuilder stringBuilder4 = stringBuilder1;
            CultureInfo invariantCulture2 = CultureInfo.InvariantCulture;
            nullable = this.Patch;
            // ISSUE: variable of a boxed type
            __Boxed<int> local2 = (ValueType) nullable.Value;
            stringBuilder4.AppendFormat((IFormatProvider) invariantCulture2, ".{0}", (object) local2);
          }
          else
            stringBuilder1.Append(".*");
        }
        else
          stringBuilder1.Append(".*");
      }
      if (this.IsTest)
        stringBuilder1.Append("-test");
      return stringBuilder1.ToString();
    }

    public static explicit operator TaskVersionSpec(string version) => TaskVersionSpec.Parse(version);

    public TaskVersion Match(IEnumerable<TaskVersion> versions)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TaskVersion>>(versions, nameof (versions));
      IEnumerable<TaskVersion> source = versions.Where<TaskVersion>((Func<TaskVersion, bool>) (x => x.IsTest == this.IsTest));
      if (this.Major.HasValue)
      {
        source = source.Where<TaskVersion>((Func<TaskVersion, bool>) (x =>
        {
          int major1 = x.Major;
          int? major2 = this.Major;
          int valueOrDefault = major2.GetValueOrDefault();
          return major1 == valueOrDefault & major2.HasValue;
        }));
        if (this.Minor.HasValue)
        {
          source = source.Where<TaskVersion>((Func<TaskVersion, bool>) (x =>
          {
            int minor1 = x.Minor;
            int? minor2 = this.Minor;
            int valueOrDefault = minor2.GetValueOrDefault();
            return minor1 == valueOrDefault & minor2.HasValue;
          }));
          if (this.Patch.HasValue)
            source = source.Where<TaskVersion>((Func<TaskVersion, bool>) (x =>
            {
              int patch1 = x.Patch;
              int? patch2 = this.Patch;
              int valueOrDefault = patch2.GetValueOrDefault();
              return patch1 == valueOrDefault & patch2.HasValue;
            }));
        }
      }
      return source.OrderByDescending<TaskVersion, TaskVersion>((Func<TaskVersion, TaskVersion>) (x => x)).FirstOrDefault<TaskVersion>();
    }

    public TaskDefinition Match(IEnumerable<TaskDefinition> definitions)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TaskDefinition>>(definitions, nameof (definitions));
      IEnumerable<TaskDefinition> source = definitions.Where<TaskDefinition>((Func<TaskDefinition, bool>) (x => x.Version.IsTest == this.IsTest));
      if (this.Major.HasValue)
      {
        source = source.Where<TaskDefinition>((Func<TaskDefinition, bool>) (x =>
        {
          int major1 = x.Version.Major;
          int? major2 = this.Major;
          int valueOrDefault = major2.GetValueOrDefault();
          return major1 == valueOrDefault & major2.HasValue;
        }));
        if (this.Minor.HasValue)
        {
          source = source.Where<TaskDefinition>((Func<TaskDefinition, bool>) (x =>
          {
            int minor1 = x.Version.Minor;
            int? minor2 = this.Minor;
            int valueOrDefault = minor2.GetValueOrDefault();
            return minor1 == valueOrDefault & minor2.HasValue;
          }));
          if (this.Patch.HasValue)
            source = source.Where<TaskDefinition>((Func<TaskDefinition, bool>) (x =>
            {
              int patch1 = x.Version.Patch;
              int? patch2 = this.Patch;
              int valueOrDefault = patch2.GetValueOrDefault();
              return patch1 == valueOrDefault & patch2.HasValue;
            }));
        }
      }
      return source.OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (x => x.Version)).FirstOrDefault<TaskDefinition>();
    }

    public static TaskVersionSpec Parse(string version)
    {
      TaskVersionSpec versionSpec;
      if (!TaskVersionSpec.TryParse(version, out versionSpec))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The value {0} is not a valid version specification", (object) version), nameof (version));
      return versionSpec;
    }

    public static bool TryParse(string version, out TaskVersionSpec versionSpec)
    {
      string[] strArray = version.Split(new char[1]{ '.' }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length < 1 || strArray.Length > 3)
      {
        versionSpec = (TaskVersionSpec) null;
        return false;
      }
      int? versionValue1 = new int?();
      int? versionValue2 = new int?();
      int? versionValue3 = new int?();
      bool flag = false;
      string str = strArray[strArray.Length - 1];
      if (str.EndsWith("-test", StringComparison.OrdinalIgnoreCase))
      {
        flag = true;
        strArray[strArray.Length - 1] = str.Remove(str.Length - "-test".Length);
      }
      if (strArray.Length == 1)
      {
        if (!TaskVersionSpec.TryParseVersionComponent(version, "major", strArray[0], true, out versionValue1))
        {
          versionSpec = (TaskVersionSpec) null;
          return false;
        }
      }
      else if (strArray.Length == 2)
      {
        if (!TaskVersionSpec.TryParseVersionComponent(version, "major", strArray[0], false, out versionValue1) || !TaskVersionSpec.TryParseVersionComponent(version, "minor", strArray[1], true, out versionValue2))
        {
          versionSpec = (TaskVersionSpec) null;
          return false;
        }
      }
      else if (!TaskVersionSpec.TryParseVersionComponent(version, "major", strArray[0], false, out versionValue1) || !TaskVersionSpec.TryParseVersionComponent(version, "minor", strArray[1], false, out versionValue2) || !TaskVersionSpec.TryParseVersionComponent(version, "patch", strArray[2], true, out versionValue3))
      {
        versionSpec = (TaskVersionSpec) null;
        return false;
      }
      versionSpec = new TaskVersionSpec()
      {
        Major = versionValue1,
        Minor = versionValue2,
        Patch = versionValue3,
        IsTest = flag
      };
      return true;
    }

    private static bool TryParseVersionComponent(
      string version,
      string name,
      string value,
      bool allowStar,
      out int? versionValue)
    {
      versionValue = new int?();
      int result;
      if (int.TryParse(value, out result))
        versionValue = new int?(result);
      else if (!allowStar || value != "*")
        return false;
      return true;
    }
  }
}
