// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator.FieldDefinitionComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator
{
  internal class FieldDefinitionComparer : IEqualityComparer<ProcessFieldDefinition>
  {
    private readonly IEqualityComparer<string> nameComparer;
    private readonly bool refNameOnly;

    public FieldDefinitionComparer(IEqualityComparer<string> nameComparer, bool refNameOnly = false)
    {
      this.nameComparer = nameComparer;
      this.refNameOnly = refNameOnly;
    }

    public bool Equals(ProcessFieldDefinition x, ProcessFieldDefinition y)
    {
      if (x == null)
        return y == null;
      if (y == null)
        return false;
      if (this.refNameOnly)
        return TFStringComparer.WorkItemFieldReferenceName.Equals(x.ReferenceName, y.ReferenceName);
      return this.nameComparer.Equals(x.Name, y.Name) && TFStringComparer.WorkItemFieldReferenceName.Equals(x.ReferenceName, y.ReferenceName) && VssStringComparer.FieldType.Equals((object) x.Type, (object) y.Type);
    }

    public int GetHashCode(ProcessFieldDefinition obj) => this.refNameOnly ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ReferenceName) : CommonUtils.CombineHashCodes(this.nameComparer.GetHashCode(obj.Name), CommonUtils.CombineHashCodes(TFStringComparer.WorkItemFieldReferenceName.GetHashCode(obj.ReferenceName), VssStringComparer.FieldType.GetHashCode((object) obj.Type)));
  }
}
