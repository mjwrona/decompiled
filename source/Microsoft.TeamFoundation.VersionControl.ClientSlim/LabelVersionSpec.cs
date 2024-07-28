// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.LabelVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  public sealed class LabelVersionSpec : VersionSpec
  {
    private string m_label;
    private string m_scope;
    public static readonly char Identifier = VersionSpecCommon.LabelIdentifier;

    private LabelVersionSpec()
    {
    }

    public string Label
    {
      get => this.m_label;
      set => this.m_label = value;
    }

    public string Scope
    {
      get => this.m_scope;
      set => this.m_scope = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static LabelVersionSpec FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      LabelVersionSpec labelVersionSpec = new LabelVersionSpec();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "label":
              labelVersionSpec.m_label = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "scope":
              labelVersionSpec.m_scope = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return labelVersionSpec;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("LabelVersionSpec instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Label: " + this.m_label);
      stringBuilder.AppendLine("  Scope: " + this.m_scope);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (element != nameof (LabelVersionSpec))
        writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", nameof (LabelVersionSpec));
      if (this.m_label != null)
        XmlUtility.ToXmlAttribute(writer, "label", this.m_label);
      if (this.m_scope != null)
        XmlUtility.ToXmlAttribute(writer, "scope", this.m_scope);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, LabelVersionSpec obj) => obj.ToXml(writer, element);

    public LabelVersionSpec(string label)
      : this(label, (string) null)
    {
    }

    public LabelVersionSpec(string label, string scope)
    {
      if (string.IsNullOrEmpty(label))
        throw new InvalidVersionSpecException(Resources.Get("LabelNameEmpty"));
      if (!string.IsNullOrEmpty(scope) && !VersionControlPath.IsServerItem(scope))
        throw new InvalidVersionSpecException(Resources.Format("LabelScopeIllegal", (object) scope));
      this.m_label = label;
      this.m_scope = scope;
    }

    public override bool Equals(object obj) => obj is LabelVersionSpec labelVersionSpec && TFStringComparer.LabelName.Equals(this.m_label, labelVersionSpec.m_label) && VersionControlPath.Equals(this.m_scope, labelVersionSpec.m_scope);

    public override int GetHashCode() => this.m_label.GetHashCode() + (this.m_scope != null ? this.m_scope.GetHashCode() : 0);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ComputeVersionString() => LabelVersionSpec.Identifier.ToString() + LabelSpec.Combine(this.m_label, this.m_scope);
  }
}
