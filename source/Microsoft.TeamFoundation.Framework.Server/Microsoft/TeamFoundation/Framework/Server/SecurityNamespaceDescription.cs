// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceDescription
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [CallOnSerialization("PrepareForWebServiceSerialization")]
  [CallOnDeserialization("InitializeFromWebServiceDeserialization")]
  public class SecurityNamespaceDescription : NamespaceDescription
  {
    private List<ActionDefinition> m_actions;
    private string m_displayName;

    public SecurityNamespaceDescription() => this.m_actions = new List<ActionDefinition>();

    internal SecurityNamespaceDescription(
      Guid namespaceId,
      string name,
      string displayName,
      string dataspaceCategory,
      char separatorValue,
      int elementLength,
      SecurityNamespaceStructure structure,
      int writePermission,
      int readPermission,
      List<ActionDefinition> actions,
      string extensionType,
      bool isRemotable,
      bool useTokenTranslator,
      int systemBitMask = 0)
    {
      this.NamespaceId = namespaceId;
      this.Name = name;
      this.DisplayName = displayName;
      this.SeparatorValue = separatorValue;
      this.ElementLength = elementLength;
      this.NamespaceStructure = structure;
      this.ReadPermission = readPermission;
      this.WritePermission = writePermission;
      this.DataspaceCategory = dataspaceCategory;
      this.ExtensionType = extensionType;
      this.m_actions = actions;
      this.IsRemotable = isRemotable;
      this.UseTokenTranslator = useTokenTranslator;
      this.SystemBitMask = systemBitMask;
      if (this.NamespaceStructure == SecurityNamespaceStructure.Hierarchical && (this.SeparatorValue != char.MinValue && this.ElementLength != -1 || this.SeparatorValue == char.MinValue && this.ElementLength == -1))
        throw new InvalidSecurityNamespaceDescriptionException(TFCommonResources.InvalidSecurityNamespaceDescriptionMessage());
    }

    public SecurityNamespaceDescription(
      Guid namespaceId,
      string name,
      string displayName,
      string dataspaceCategory,
      char separatorValue,
      int elementLength,
      SecurityNamespaceStructure structure,
      int writePermission,
      int readPermission,
      List<ActionDefinition> actions)
      : this(namespaceId, name, displayName, dataspaceCategory, separatorValue, elementLength, structure, writePermission, readPermission, actions, (string) null, false, false)
    {
    }

    [XmlAttribute("name")]
    [ClientProperty(ClientVisibility.Private)]
    public string Name { get; set; }

    [XmlAttribute("displayName")]
    [ClientProperty(ClientVisibility.Private)]
    public string DisplayName
    {
      get => string.IsNullOrEmpty(this.m_displayName) ? this.Name : this.m_displayName;
      set => this.m_displayName = value;
    }

    [XmlAttribute("separator")]
    [ClientProperty(ClientVisibility.Private)]
    public char SeparatorValue { get; set; }

    [XmlAttribute("elementLength")]
    [ClientProperty(ClientVisibility.Private)]
    public int ElementLength { get; set; }

    [XmlAttribute("writePermission")]
    [ClientProperty(ClientVisibility.Private)]
    public int WritePermission { get; set; }

    [XmlAttribute("readPermission")]
    [ClientProperty(ClientVisibility.Private)]
    public int ReadPermission { get; set; }

    [XmlAttribute("databaseCategory")]
    [ClientProperty(ClientVisibility.Private)]
    public string DataspaceCategory { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public List<ActionDefinition> Actions => this.m_actions;

    [XmlAttribute("structure")]
    [ClientProperty(ClientVisibility.Private)]
    public int StructureValue
    {
      get => (int) this.NamespaceStructure;
      set => this.NamespaceStructure = (SecurityNamespaceStructure) value;
    }

    [XmlIgnore]
    public SecurityNamespaceStructure NamespaceStructure { get; set; }

    [XmlAttribute("extensionType")]
    [SoapIgnore]
    public string ExtensionType { get; set; }

    [XmlAttribute("isRemotable")]
    [SoapIgnore]
    public bool IsRemotable { get; set; }

    [XmlAttribute("useTokenTranslator")]
    [SoapIgnore]
    public bool UseTokenTranslator { get; set; }

    [XmlAttribute("systemBitMask")]
    [SoapIgnore]
    public int SystemBitMask { get; set; }

    internal string InternalDisplayName => this.m_displayName;

    [SoapIgnore]
    [XmlIgnore]
    internal bool IsRemoted { get; set; }

    public IEnumerable<string> GetLocalizedActions(int permissions)
    {
      foreach (ActionDefinition action in this.m_actions)
      {
        if ((permissions & action.Bit) != 0)
          yield return action.DisplayName;
      }
    }

    public override void Validate(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<string>(this.Name, "Name");
      ArgumentUtility.CheckStringForNullOrEmpty(this.DataspaceCategory, "DataspaceCategory");
      ArgumentUtility.CheckForEmptyGuid(this.NamespaceId, "NamespaceId");
      this.ValidateActions();
      this.ValidateDataspaceCategory(requestContext);
    }

    internal void ValidateActions()
    {
      if (this.Actions == null)
        return;
      int num = 0;
      foreach (ActionDefinition action in this.Actions)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(action.Name, "Name");
        if (action.Bit == 0)
          throw new InvalidSecurityNamespaceDescriptionException(FrameworkResources.SecurityNamespaceHasActionWith0Bit((object) this.Name, (object) this.NamespaceId, (object) action.Name));
        if ((num & action.Bit) != 0)
          throw new InvalidSecurityNamespaceDescriptionException(FrameworkResources.SecurityNamespaceHasDuplicatedAction((object) this.Name, (object) action.Bit));
        num |= action.Bit;
      }
    }

    private void ValidateDataspaceCategory(IVssRequestContext requestContext) => requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, this.DataspaceCategory, Guid.Empty, true);
  }
}
