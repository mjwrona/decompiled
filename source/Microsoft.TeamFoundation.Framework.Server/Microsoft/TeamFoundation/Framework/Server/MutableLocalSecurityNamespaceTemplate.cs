// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MutableLocalSecurityNamespaceTemplate
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MutableLocalSecurityNamespaceTemplate : IMutableSecurityNamespaceTemplate
  {
    public string Name;
    public string DisplayName;
    public string DataspaceCategory;
    public char SeparatorValue;
    public int ElementLength;
    public SecurityNamespaceStructure Structure;
    public int WritePermission;
    public int ReadPermission;
    public List<ActionDefinition> Actions;
    public Type ExtensionType;
    public bool IsRemotable;
    public bool UseTokenTranslator;
    public int SystemBitMask;

    public TeamFoundationHostType HostType { get; set; }

    public Guid NamespaceId { get; set; }

    public SecurityNamespaceTemplate GetSecurityNamespaceTemplate()
    {
      SecurityNamespaceDescription description = new SecurityNamespaceDescription(this.NamespaceId, this.Name, this.DisplayName ?? this.Name, this.DataspaceCategory, this.SeparatorValue, this.ElementLength, this.Structure, this.WritePermission, this.ReadPermission, this.Actions, this.ExtensionType?.ToString(), this.IsRemotable, this.UseTokenTranslator, this.SystemBitMask);
      description.IsProjected = true;
      return new SecurityNamespaceTemplate(this.HostType, this.NamespaceId, (NamespaceDescription) description);
    }
  }
}
