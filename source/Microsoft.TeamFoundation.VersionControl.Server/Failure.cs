// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Failure
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  public class Failure : ICacheable
  {
    private RequestType m_requestType;
    private string m_code;
    private SeverityType m_severity;
    private List<Warning> m_warnings = new List<Warning>();
    private string m_computerName;
    private string m_identityName;
    private string m_localItem;
    private string m_message;
    private string m_resourceName;
    private string m_serverItem;
    private int m_itemId;
    private string m_workspaceName;
    private string m_workspaceOwner;
    private Exception m_exception;

    internal Failure(Exception e, string item, RequestType requestType)
      : this(e)
    {
      if (this.ServerItem == null && this.LocalItem == null && item != null)
      {
        if (VersionControlPath.IsServerItem(item))
          this.ServerItem = item;
        else
          this.LocalItem = item;
      }
      this.RequestType = requestType;
      this.Exception = e;
    }

    internal Failure(Exception e)
    {
      this.Code = e.GetType().Name;
      this.Message = e.Message;
      this.Exception = e;
      if (!(e is ServerException))
        return;
      ((ServerException) e).SetFailureInfo(this);
    }

    public Failure()
    {
    }

    [XmlAttribute("req")]
    [DefaultValue(RequestType.None)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public RequestType RequestType
    {
      get => this.m_requestType;
      set => this.m_requestType = value;
    }

    [XmlAttribute("code")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Code
    {
      get => this.m_code;
      set => this.m_code = value;
    }

    [XmlAttribute("sev")]
    [DefaultValue(SeverityType.Error)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public SeverityType Severity
    {
      get => this.m_severity;
      set => this.m_severity = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public List<Warning> Warnings
    {
      get => this.m_warnings;
      set => this.m_warnings = value;
    }

    [XmlAttribute("computer")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ComputerName
    {
      get => this.m_computerName;
      set => this.m_computerName = value;
    }

    [XmlAttribute("ident")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string IdentityName
    {
      get => this.m_identityName;
      set => this.m_identityName = value;
    }

    [XmlAttribute("local")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LocalItem
    {
      get => this.m_localItem;
      set => this.m_localItem = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Message
    {
      get => this.m_message;
      set => this.m_message = value;
    }

    [XmlAttribute("res")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ResourceName
    {
      get => this.m_resourceName;
      set => this.m_resourceName = value;
    }

    [XmlAttribute("item")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ServerItem
    {
      get => this.m_serverItem;
      set => this.m_serverItem = value;
    }

    [XmlAttribute("itemid")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int ItemId
    {
      get => this.m_itemId;
      set => this.m_itemId = value;
    }

    [XmlAttribute("ws")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string WorkspaceName
    {
      get => this.m_workspaceName;
      set => this.m_workspaceName = value;
    }

    [XmlAttribute("owner")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string WorkspaceOwner
    {
      get => this.m_workspaceOwner;
      set => this.m_workspaceOwner = value;
    }

    [XmlIgnore]
    public Exception Exception
    {
      get => this.m_exception;
      set => this.m_exception = value;
    }

    private XmlNode getDetailNode()
    {
      XmlDocument document = new XmlDocument();
      XmlNode node = document.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
      Failure.setDetailAttribute(document, node, "ServerItem", this.ServerItem);
      Failure.setDetailAttribute(document, node, "LocalItem", this.LocalItem);
      Failure.setDetailAttribute(document, node, "IdentityName", this.IdentityName);
      Failure.setDetailAttribute(document, node, "ResourceName", this.ResourceName);
      Failure.setDetailAttribute(document, node, "WorkspaceName", this.WorkspaceName);
      Failure.setDetailAttribute(document, node, "WorkspaceOwner", this.WorkspaceOwner);
      Failure.setDetailAttribute(document, node, "ComputerName", this.ComputerName);
      return node;
    }

    internal SoapException toSoapException() => new SoapException(this.Message, SoapException.ServerFaultCode, (string) null, (string) null, this.getDetailNode(), new SoapFaultSubCode(new XmlQualifiedName(this.Code)), (Exception) null);

    private static void setDetailAttribute(
      XmlDocument document,
      XmlNode detailNode,
      string name,
      string val)
    {
      if (val == null)
        return;
      XmlAttribute attribute = document.CreateAttribute(name, string.Empty);
      attribute.Value = val;
      detailNode.Attributes.Append(attribute);
    }

    public int GetCachedSize() => 1000;
  }
}
