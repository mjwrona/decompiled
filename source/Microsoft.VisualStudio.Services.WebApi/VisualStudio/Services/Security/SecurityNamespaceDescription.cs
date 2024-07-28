// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.SecurityNamespaceDescription
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract]
  public sealed class SecurityNamespaceDescription
  {
    public SecurityNamespaceDescription()
    {
    }

    public SecurityNamespaceDescription(
      Guid namespaceId,
      string name,
      string displayName,
      string dataspaceCategory,
      char separatorValue,
      int elementLength,
      int structure,
      int writePermission,
      int readPermission,
      List<ActionDefinition> actions,
      string extensionType,
      bool isRemotable,
      bool useTokenTranslator,
      int systemBitMask)
      : this(namespaceId, name, displayName, dataspaceCategory, separatorValue, elementLength, structure, writePermission, readPermission, actions, extensionType, isRemotable, useTokenTranslator)
    {
      this.SystemBitMask = systemBitMask;
    }

    public SecurityNamespaceDescription(
      Guid namespaceId,
      string name,
      string displayName,
      string dataspaceCategory,
      char separatorValue,
      int elementLength,
      int structure,
      int writePermission,
      int readPermission,
      List<ActionDefinition> actions,
      string extensionType,
      bool isRemotable,
      bool useTokenTranslator)
    {
      this.NamespaceId = namespaceId;
      this.Name = name;
      this.DisplayName = displayName;
      this.SeparatorValue = separatorValue;
      this.ElementLength = elementLength;
      this.StructureValue = structure;
      this.ReadPermission = readPermission;
      this.WritePermission = writePermission;
      this.DataspaceCategory = dataspaceCategory;
      this.ExtensionType = extensionType;
      this.Actions = actions;
      this.IsRemotable = isRemotable;
      this.UseTokenTranslator = useTokenTranslator;
    }

    [DataMember]
    public Guid NamespaceId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public char SeparatorValue { get; set; }

    [DataMember]
    public int ElementLength { get; set; }

    [DataMember]
    public int WritePermission { get; set; }

    [DataMember]
    public int ReadPermission { get; set; }

    [DataMember]
    public string DataspaceCategory { get; set; }

    [DataMember]
    public List<ActionDefinition> Actions { get; set; }

    [DataMember]
    public int StructureValue { get; set; }

    [DataMember]
    public string ExtensionType { get; set; }

    [DataMember]
    public bool IsRemotable { get; set; }

    [DataMember]
    public bool UseTokenTranslator { get; set; }

    [DataMember]
    public int SystemBitMask { get; set; }
  }
}
