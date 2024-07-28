// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.DateVersionSpec
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
  public sealed class DateVersionSpec : VersionSpec
  {
    private DateTime m_date = DateTime.MinValue;
    private string m_originalText;
    public static readonly char Identifier = VersionSpecCommon.DateIdentifier;

    private DateVersionSpec()
    {
    }

    public DateTime Date
    {
      get => this.m_date;
      set => this.m_date = value;
    }

    public string OriginalText
    {
      get => this.m_originalText;
      set => this.m_originalText = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateVersionSpec FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      DateVersionSpec dateVersionSpec = new DateVersionSpec();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "date":
              dateVersionSpec.m_date = XmlUtility.DateTimeFromXmlAttribute(reader);
              continue;
            case "otext":
              dateVersionSpec.m_originalText = XmlUtility.StringFromXmlAttribute(reader);
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
      return dateVersionSpec;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("DateVersionSpec instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Date: " + this.m_date.ToString());
      stringBuilder.AppendLine("  OriginalText: " + this.m_originalText);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (element != nameof (DateVersionSpec))
        writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", nameof (DateVersionSpec));
      if (this.m_date != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "date", this.m_date);
      if (this.m_originalText != null)
        XmlUtility.ToXmlAttribute(writer, "otext", this.m_originalText);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, DateVersionSpec obj) => obj.ToXml(writer, element);

    public DateVersionSpec(DateTime date)
      : this(date, (string) null)
    {
    }

    public DateVersionSpec(DateTime date, string originalText)
    {
      this.m_originalText = originalText;
      if (date.Kind == DateTimeKind.Unspecified)
        this.m_date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, DateTimeKind.Local);
      else
        this.m_date = date;
    }

    public override bool Equals(object obj) => obj is DateVersionSpec && this.m_date.Equals(((DateVersionSpec) obj).m_date);

    public override int GetHashCode() => this.m_date.GetHashCode();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ComputeVersionString() => DateVersionSpec.Identifier.ToString() + this.m_date.ToString();
  }
}
