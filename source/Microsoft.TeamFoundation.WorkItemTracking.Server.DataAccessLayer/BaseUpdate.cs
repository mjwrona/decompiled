// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.BaseUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class BaseUpdate
  {
    protected XmlNode m_actionNode;
    protected XmlNode m_outputNode;
    protected Update m_parentUpdate;
    protected SqlBatchBuilder m_sqlBatch;
    protected int m_inputId;
    protected int m_actionId;
    protected int m_tempId;
    protected DalSqlElement m_updateSqlElement;
    protected ClientCapabilities m_clientCapabilities;
    protected IVssIdentity m_user;
    private IVssRequestContext m_requestContext;
    private WorkItemTrackingRequestContext m_witRequestContext;
    protected bool m_isDummy;
    private int? m_finalId;

    public BaseUpdate(
      SqlBatchBuilder sqlBatch,
      XmlNode actionNode,
      Update parentUpdate,
      XmlNode outputNode,
      IVssIdentity user)
      : this(sqlBatch, actionNode, parentUpdate, outputNode, user, false)
    {
    }

    public BaseUpdate(
      SqlBatchBuilder sqlBatch,
      XmlNode actionNode,
      Update parentUpdate,
      XmlNode outputNode,
      IVssIdentity user,
      bool isDummy)
    {
      this.m_sqlBatch = sqlBatch;
      this.m_actionNode = actionNode;
      this.m_parentUpdate = parentUpdate;
      this.m_outputNode = outputNode;
      this.m_user = user;
      this.m_isDummy = isDummy;
    }

    protected IVssIdentity User => this.m_user;

    protected int InitializeActionId()
    {
      XmlAttribute xmlAttribute = (XmlAttribute) null;
      this.m_actionId = 0;
      if (this.m_actionNode != null)
      {
        xmlAttribute = (XmlAttribute) this.m_actionNode.Attributes.GetNamedItem("TempID");
        if (xmlAttribute != null)
        {
          this.m_actionId = Convert.ToInt32(xmlAttribute.Value, 10);
          this.m_tempId = this.m_actionId;
        }
      }
      if (this.m_actionNode == null || xmlAttribute == null)
        this.m_actionId = this.GetNextActionId();
      return this.m_actionId;
    }

    protected int GetNextActionId() => this.m_parentUpdate.GetNextActionId();

    internal static bool GetAttributeString(
      XmlNode actionNode,
      string attributeName,
      bool required,
      string defaultValue,
      out string stringValue)
    {
      XmlAttribute namedItem = (XmlAttribute) actionNode.Attributes.GetNamedItem(attributeName);
      if (required && namedItem == null)
        throw new ArgumentException(DalResourceStrings.Format("MissingAttributeInXmlException", (object) attributeName));
      stringValue = namedItem == null ? (defaultValue != null ? defaultValue : string.Empty) : namedItem.Value;
      return namedItem != null;
    }

    protected bool GetAttributeString(
      string attributeName,
      bool required,
      string defaultValue,
      out string stringValue)
    {
      return BaseUpdate.GetAttributeString(this.m_actionNode, attributeName, required, defaultValue, out stringValue);
    }

    protected bool GetAttributeInt(
      string attributeName,
      bool required,
      int defaultValue,
      out int intValue)
    {
      return BaseUpdate.GetAttributeInt(this.m_actionNode, attributeName, required, defaultValue, out intValue);
    }

    protected bool GetAttributeTempId(
      string attributeName,
      bool required,
      int defaultValue,
      out int intValue)
    {
      if (!BaseUpdate.GetAttributeInt(this.m_actionNode, attributeName, required, defaultValue, out intValue))
        return false;
      this.m_parentUpdate.CheckTempId(intValue);
      return true;
    }

    internal static bool GetAttributeInt(
      XmlNode actionNode,
      string attributeName,
      bool required,
      int defaultValue,
      out int intValue)
    {
      XmlAttribute namedItem = (XmlAttribute) actionNode.Attributes.GetNamedItem(attributeName);
      string empty = string.Empty;
      if (required && namedItem == null)
        throw new ArgumentException(DalResourceStrings.Format("MissingAttributeInXmlException", (object) attributeName));
      if (namedItem != null)
      {
        if (!int.TryParse(namedItem.Value, out intValue))
          throw new ArgumentException(DalResourceStrings.Get("UpdateInvalidAttributeIntegerException"), "updateElement");
      }
      else
        intValue = defaultValue;
      return namedItem != null;
    }

    protected bool GetAttributeBool(
      string attributeName,
      bool required,
      bool defaultValue,
      out bool outValue)
    {
      return BaseUpdate.GetAttributeBool(this.m_actionNode, attributeName, required, defaultValue, out outValue);
    }

    internal void GetAttributeBool(string attributeName, bool required, out bool? outValue)
    {
      outValue = new bool?();
      bool outValue1;
      if (!this.GetAttributeBool(attributeName, required, false, out outValue1))
        return;
      outValue = new bool?(outValue1);
    }

    internal static bool GetAttributeBool(
      XmlNode actionNode,
      string attributeName,
      bool required,
      bool defaultValue,
      out bool outValue)
    {
      outValue = defaultValue;
      string stringValue;
      int num = BaseUpdate.GetAttributeString(actionNode, attributeName, required, string.Empty, out stringValue) ? 1 : 0;
      if (num == 0)
        return num != 0;
      if (stringValue.Trim().Equals("-1") || stringValue.Trim().Equals("0"))
      {
        outValue = false;
        return num != 0;
      }
      if (stringValue.Trim().Equals("1"))
      {
        outValue = true;
        return num != 0;
      }
      if (bool.TryParse(stringValue, out outValue))
        return num != 0;
      throw new ArgumentException(DalResourceStrings.Get("UpdateInvalidBooleanAttributeException"), "updateElement");
    }

    protected bool GetAttributeGuid(string attributeName, out Guid guidValue) => BaseUpdate.GetAttributeGuid(this.m_actionNode, attributeName, true, Guid.Empty, out guidValue);

    protected bool GetAttributeGuid(
      string attributeName,
      bool required,
      Guid defaultGuid,
      out Guid guidValue)
    {
      return BaseUpdate.GetAttributeGuid(this.m_actionNode, attributeName, required, defaultGuid, out guidValue);
    }

    internal static bool GetAttributeGuid(
      XmlNode actionNode,
      string attributeName,
      bool required,
      Guid defaultGuid,
      out Guid guidValue)
    {
      Guid guid = defaultGuid;
      string stringValue;
      bool attributeString = BaseUpdate.GetAttributeString(actionNode, attributeName, required, string.Empty, out stringValue);
      if (attributeString)
      {
        try
        {
          guid = XmlConvert.ToGuid(stringValue);
        }
        catch
        {
          throw new ArgumentException(DalResourceStrings.Get("UpdateInvalidGuidAttributeException"));
        }
      }
      guidValue = guid;
      return attributeString;
    }

    internal bool GetAttributeCacheStamp(
      string attributeName,
      bool required,
      out byte[] cacheStamp)
    {
      return BaseUpdate.GetAttributeCacheStamp(this.m_actionNode, attributeName, required, out cacheStamp);
    }

    internal static bool GetAttributeCacheStamp(
      XmlNode actionNode,
      string attributeName,
      bool required,
      out byte[] cacheStamp)
    {
      string stringValue = string.Empty;
      int num = BaseUpdate.GetAttributeString(actionNode, attributeName, required, (string) null, out stringValue) ? 1 : 0;
      cacheStamp = (byte[]) null;
      if (stringValue.Length <= 0)
        return num != 0;
      long int64 = Convert.ToInt64("0x" + stringValue, 16);
      cacheStamp = DalMetadataSelectElement.ConvertLongToByteArray(int64);
      return num != 0;
    }

    internal virtual void GenerateOutput() => this.RequestContext.TraceBlock(900739, 900740, "Update", nameof (BaseUpdate), "BaseUpdate.GenerateOutput", (Action) (() =>
    {
      XmlDocument ownerDocument = this.m_outputNode.OwnerDocument;
      if (this.FinalId > 0)
      {
        XmlAttribute attribute = ownerDocument.CreateAttribute("ID");
        attribute.Value = this.FinalId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute);
      }
      XmlAttribute attribute1 = ownerDocument.CreateAttribute("Number");
      attribute1.Value = "0";
      this.m_outputNode.Attributes.Append(attribute1);
      if (this.m_tempId == 0)
        return;
      XmlAttribute attribute2 = ownerDocument.CreateAttribute("TempID");
      attribute2.Value = this.m_tempId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.m_outputNode.Attributes.Append(attribute2);
    }));

    protected virtual int GetOutputId() => this.m_updateSqlElement != null ? this.m_updateSqlElement.GetResultId() : this.m_parentUpdate.GetOutputId(-this.m_actionId);

    protected IVssRequestContext RequestContext
    {
      get => this.m_requestContext ?? this.m_parentUpdate.RequestContext;
      set
      {
        this.m_requestContext = value;
        this.m_witRequestContext = (WorkItemTrackingRequestContext) null;
      }
    }

    protected WorkItemTrackingRequestContext WitRequestContext
    {
      get
      {
        if (this.m_witRequestContext == null)
          this.m_witRequestContext = this.m_requestContext == null ? this.m_parentUpdate.WitRequestContext : this.m_requestContext.WitContext();
        return this.m_witRequestContext;
      }
    }

    public bool IsDummy => this.m_isDummy;

    public int FinalId
    {
      get
      {
        if (!this.m_finalId.HasValue)
        {
          int outputId = this.GetOutputId();
          this.m_finalId = outputId <= 0 ? new int?(this.m_inputId) : new int?(outputId);
        }
        return this.m_finalId.Value;
      }
    }
  }
}
