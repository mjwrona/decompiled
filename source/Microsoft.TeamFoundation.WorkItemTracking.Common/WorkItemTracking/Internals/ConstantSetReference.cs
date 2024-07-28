// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.ConstantSetReference
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [TypeConverter(typeof (ConstantSetReferenceTypeConverter))]
  [XmlType("set-reference")]
  [XmlInclude(typeof (ExtendedConstantSetRef))]
  public class ConstantSetReference : IEquatable<ConstantSetReference>
  {
    private const string c_excludeGroups = "excludegroups";
    private const string c_includeTop = "includetop";
    private const string c_direct = "direct";

    public ConstantSetReference() => this.Id = 0;

    public ConstantSetReference(int setId)
      : this(setId, true, true, false)
    {
    }

    public ConstantSetReference(int setId, bool direct, bool includeGroups, bool includeTop)
    {
      this.Id = setId;
      this.Direct = direct;
      this.ExcludeGroups = !includeGroups;
      this.IncludeTop = includeTop;
    }

    [XmlAttribute("id")]
    [DefaultValue(0)]
    public int Id { get; set; }

    [XmlAttribute("include-top")]
    [DefaultValue(false)]
    public bool IncludeTop { get; set; }

    [XmlAttribute("direct")]
    [DefaultValue(false)]
    public bool Direct { get; set; }

    [XmlAttribute("exclude-groups")]
    [DefaultValue(false)]
    public bool ExcludeGroups { get; set; }

    [XmlIgnore]
    public Guid TeamFoundationId { get; set; }

    [XmlIgnore]
    public IdentityDescriptor IdentityDescriptor { get; set; }

    public override string ToString()
    {
      string str = this.Id.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      if (this.ExcludeGroups)
        str += "|excludegroups";
      if (this.IncludeTop)
        str += "|includetop";
      if (this.Direct)
        str += "|direct";
      return str;
    }

    public long Handle
    {
      get
      {
        long id = (long) this.Id;
        if (this.Direct)
          id |= 4294967296L;
        if (this.IncludeTop)
          id |= 8589934592L;
        if (!this.ExcludeGroups)
          id |= 17179869184L;
        return id;
      }
    }

    public static ConstantSetReference CreateFrom(string stringValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(stringValue, nameof (stringValue));
      string[] strArray = stringValue.Split(new char[1]
      {
        '|'
      }, StringSplitOptions.RemoveEmptyEntries);
      int result;
      if (!int.TryParse(strArray[0], NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
        throw new ArgumentOutOfRangeException(nameof (stringValue), "The parameter must start with a set number.");
      ConstantSetReference from = new ConstantSetReference()
      {
        Id = result
      };
      for (int index = 1; index < strArray.Length; ++index)
      {
        string str = strArray[index];
        if (StringComparer.OrdinalIgnoreCase.Equals(str, "excludegroups"))
          from.ExcludeGroups = true;
        else if (StringComparer.OrdinalIgnoreCase.Equals(str, "includetop"))
        {
          from.IncludeTop = true;
        }
        else
        {
          if (!StringComparer.OrdinalIgnoreCase.Equals(str, "direct"))
            throw new ArgumentOutOfRangeException(nameof (stringValue), (object) str, "Unknown set specifier.");
          from.Direct = true;
        }
      }
      return from;
    }

    public override int GetHashCode()
    {
      int id = this.Id;
      if (this.Direct)
        id ^= int.MinValue;
      if (this.IncludeTop)
        id ^= 1073741824;
      if (!this.ExcludeGroups)
        id ^= 536870912;
      return id;
    }

    public override bool Equals(object obj) => obj is ConstantSetReference && this.Equals((ConstantSetReference) obj);

    public bool Equals(ConstantSetReference other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.Id == other.Id && this.Direct == other.Direct && this.ExcludeGroups == other.ExcludeGroups && this.IncludeTop == other.IncludeTop;
    }
  }
}
