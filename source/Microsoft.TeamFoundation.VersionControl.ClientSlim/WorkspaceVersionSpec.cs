// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.WorkspaceVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.Common.Internal;
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
  public sealed class WorkspaceVersionSpec : VersionSpec
  {
    private string m_name;
    private string m_ownerDisplayName;
    private string m_ownerName;
    private string m_ownerUniqueName;
    public static readonly char Identifier = VersionSpecCommon.WorkspaceIdentifier;

    private WorkspaceVersionSpec()
    {
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public string OwnerDisplayName
    {
      get => this.m_ownerDisplayName;
      set => this.m_ownerDisplayName = value;
    }

    public string OwnerName
    {
      get => this.m_ownerName;
      set => this.m_ownerName = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static WorkspaceVersionSpec FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      WorkspaceVersionSpec workspaceVersionSpec = new WorkspaceVersionSpec();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "name":
              workspaceVersionSpec.m_name = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "ownerDisp":
              workspaceVersionSpec.m_ownerDisplayName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "owner":
              workspaceVersionSpec.m_ownerName = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "ownerUniq":
              workspaceVersionSpec.m_ownerUniqueName = XmlUtility.StringFromXmlAttribute(reader);
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
      workspaceVersionSpec.AfterDeserialize();
      return workspaceVersionSpec;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("WorkspaceVersionSpec instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  OwnerDisplayName: " + this.m_ownerDisplayName);
      stringBuilder.AppendLine("  OwnerName: " + this.m_ownerName);
      stringBuilder.AppendLine("  OwnerUniqueName: " + this.m_ownerUniqueName);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (element != nameof (WorkspaceVersionSpec))
        writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", nameof (WorkspaceVersionSpec));
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "name", this.m_name);
      if (this.m_ownerDisplayName != null)
        XmlUtility.ToXmlAttribute(writer, "ownerDisp", this.m_ownerDisplayName);
      if (this.m_ownerName != null)
        XmlUtility.ToXmlAttribute(writer, "owner", this.m_ownerName);
      if (this.m_ownerUniqueName != null)
        XmlUtility.ToXmlAttribute(writer, "ownerUniq", this.m_ownerUniqueName);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, WorkspaceVersionSpec obj) => obj.ToXml(writer, element);

    internal WorkspaceVersionSpec(string ownerName)
    {
      this.m_ownerName = !string.IsNullOrEmpty(ownerName) ? ownerName : throw new InvalidVersionSpecException(Resources.Get("WorkspaceOwnerEmpty"));
      this.m_ownerDisplayName = ownerName;
    }

    public WorkspaceVersionSpec(string name, string ownerName)
      : this(name, ownerName, ownerName)
    {
    }

    public WorkspaceVersionSpec(string name, string ownerName, string ownerDisplayName)
    {
      if (string.IsNullOrEmpty(name))
        throw new InvalidVersionSpecException(Resources.Get("WorkspaceNameEmpty"));
      if (string.IsNullOrEmpty(ownerName))
        throw new InvalidVersionSpecException(Resources.Get("WorkspaceOwnerEmpty"));
      this.m_name = name;
      this.m_ownerName = ownerName;
      this.m_ownerDisplayName = ownerDisplayName;
    }

    public override bool Equals(object obj) => obj is WorkspaceVersionSpec && TFStringComparer.WorkspaceName.Equals(this.m_name, ((WorkspaceVersionSpec) obj).m_name) && UserNameUtil.Compare(this.m_ownerName, ((WorkspaceVersionSpec) obj).m_ownerName) == 0;

    public override int GetHashCode() => (this.m_ownerName != null ? this.m_ownerName.GetHashCode() : 0) + (this.m_name != null ? this.m_name.GetHashCode() : 0);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ComputeVersionString() => WorkspaceVersionSpec.Identifier.ToString() + WorkspaceSpec.Combine(this.m_name, this.OwnerDisplayName);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string ComputeUniqueVersionString() => WorkspaceVersionSpec.Identifier.ToString() + WorkspaceSpec.Combine(this.m_name, this.OwnerName);

    private void AfterDeserialize()
    {
      if (!string.IsNullOrEmpty(this.m_ownerDisplayName))
        return;
      this.m_ownerDisplayName = this.m_ownerName;
    }
  }
}
