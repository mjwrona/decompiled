// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.LatestVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  public sealed class LatestVersionSpec : VersionSpec
  {
    private static LatestVersionSpec m_instance;
    public static readonly char Identifier = VersionSpecCommon.LatestIdentifier;

    private LatestVersionSpec()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static LatestVersionSpec FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      LatestVersionSpec latestVersionSpec = new LatestVersionSpec();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name1 = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name2 = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return latestVersionSpec;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("LatestVersionSpec instance " + this.GetHashCode().ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (element != nameof (LatestVersionSpec))
        writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", nameof (LatestVersionSpec));
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, LatestVersionSpec obj) => obj.ToXml(writer, element);

    public override bool Equals(object obj) => obj is LatestVersionSpec;

    public override int GetHashCode() => 0;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ComputeVersionString() => LatestVersionSpec.Identifier.ToString();

    public static LatestVersionSpec Instance
    {
      get
      {
        if (LatestVersionSpec.m_instance == null)
          LatestVersionSpec.m_instance = new LatestVersionSpec();
        return LatestVersionSpec.m_instance;
      }
    }
  }
}
