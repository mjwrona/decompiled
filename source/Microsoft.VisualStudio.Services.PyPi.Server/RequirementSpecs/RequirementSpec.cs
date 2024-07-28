// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.RequirementSpec
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public class RequirementSpec
  {
    public Identifier Name { get; }

    public ExtrasList Extras { get; }

    public UrlSpec UrlSpec { get; }

    public VersionConstraintList VersionSpec { get; }

    public MarkerExpression Marker { get; }

    public RequirementSpec(
      Identifier name,
      ExtrasList extras,
      UrlSpec urlSpec,
      MarkerExpression marker)
    {
      this.Name = name;
      this.Extras = extras;
      this.UrlSpec = urlSpec;
      this.Marker = marker;
    }

    public RequirementSpec(
      Identifier name,
      ExtrasList extras,
      VersionConstraintList versionSpec,
      MarkerExpression marker)
    {
      this.Name = name;
      this.Extras = extras;
      this.VersionSpec = versionSpec;
      this.Marker = marker;
    }

    public override string ToString() => this.ToString(RequirementVersionFormattingOptions.Default);

    private string ToString(
      RequirementVersionFormattingOptions versionFormattingOptions)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.FormatNameAndExtras());
      string str = this.FormatVersion(versionFormattingOptions);
      if (!string.IsNullOrEmpty(str))
      {
        stringBuilder.Append(" ");
        stringBuilder.Append(str);
      }
      if (this.Marker != null)
      {
        if (this.UrlSpec != null)
          stringBuilder.Append(" ");
        stringBuilder.Append("; ");
        stringBuilder.Append((object) this.Marker);
      }
      return stringBuilder.ToString();
    }

    public string FormatVersion(RequirementVersionFormattingOptions options) => this.VersionSpec != null ? (!options.HasFlag((Enum) RequirementVersionFormattingOptions.NoParenthesesAroundVersionList) ? string.Format("({0})", (object) this.VersionSpec) : this.VersionSpec.ToString()) : (this.UrlSpec != null ? string.Format("@ {0}", (object) this.UrlSpec) : (string) null);

    public string FormatVersionAndMarker(RequirementVersionFormattingOptions options)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = this.FormatVersion(options);
      if (!string.IsNullOrEmpty(str))
        stringBuilder.Append(str);
      if (this.Marker != null)
      {
        stringBuilder.Append("; ");
        stringBuilder.Append((object) this.Marker);
      }
      string enumerable = stringBuilder.ToString();
      return !enumerable.IsNullOrEmpty<char>() ? enumerable : (string) null;
    }

    public string FormatNameAndExtras() => this.Extras == null || !this.Extras.Extras.Any<Identifier>() ? this.Name.ToString() : string.Format("{0}[{1}]", (object) this.Name, (object) this.Extras);

    public string Dump(string indent, string newline)
    {
      string indent1 = string.IsNullOrEmpty(indent) ? "" : indent + "    ";
      string str = this.VersionSpec?.Dump(indent1, newline) ?? this.UrlSpec?.Dump(indent1, newline) ?? "null";
      return "RequirementSpec(" + newline + indent1 + this.Name.Dump(indent1, newline) + "," + newline + indent1 + (this.Extras?.Dump(indent1, newline) ?? "null") + "," + newline + indent1 + str + "," + newline + indent1 + (this.Marker?.Dump(indent1, newline) ?? "null") + newline + indent + ")";
    }
  }
}
