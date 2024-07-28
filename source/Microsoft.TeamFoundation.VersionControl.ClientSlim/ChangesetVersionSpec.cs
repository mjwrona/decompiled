// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.ChangesetVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  public sealed class ChangesetVersionSpec : VersionSpec
  {
    private int m_changesetId;
    public static readonly char Identifier = VersionSpecCommon.ChangesetIdentifier;

    private ChangesetVersionSpec()
    {
    }

    public int ChangesetId
    {
      get => this.m_changesetId;
      set => this.m_changesetId = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ChangesetVersionSpec FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ChangesetVersionSpec changesetVersionSpec = new ChangesetVersionSpec();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "cs")
            changesetVersionSpec.m_changesetId = XmlUtility.Int32FromXmlAttribute(reader);
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
      return changesetVersionSpec;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ChangesetVersionSpec instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ChangesetId: " + this.m_changesetId.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (element != nameof (ChangesetVersionSpec))
        writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", nameof (ChangesetVersionSpec));
      if (this.m_changesetId != 0)
        XmlUtility.ToXmlAttribute(writer, "cs", this.m_changesetId);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ChangesetVersionSpec obj) => obj.ToXml(writer, element);

    public ChangesetVersionSpec(string changeset) => this.m_changesetId = ChangesetVersionSpec.ParseChangesetNumber(changeset);

    public ChangesetVersionSpec(int changeset)
    {
      this.m_changesetId = changeset;
      VersionSpecCommon.ValidateNumber(VersionSpec.VersionSpecFactory, this.m_changesetId);
    }

    public static int ParseChangesetNumber(string changeset) => VersionSpecCommon.ParseChangesetNumber(VersionSpec.VersionSpecFactory, changeset);

    public override bool Equals(object obj) => obj is ChangesetVersionSpec && this.m_changesetId == ((ChangesetVersionSpec) obj).m_changesetId;

    public override int GetHashCode() => this.m_changesetId;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ComputeVersionString() => ChangesetVersionSpec.Identifier.ToString() + (object) this.m_changesetId;
  }
}
