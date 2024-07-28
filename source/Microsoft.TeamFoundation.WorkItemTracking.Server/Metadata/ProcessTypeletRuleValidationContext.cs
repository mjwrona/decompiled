// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessTypeletRuleValidationContext
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ProcessTypeletRuleValidationContext : IRuleValidationContext
  {
    private IFieldTypeDictionary m_fieldDict;

    public ProcessTypeletRuleValidationContext(IVssRequestContext requestContext) => this.m_fieldDict = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);

    public bool IsValidField(string fieldReferenceName) => this.m_fieldDict.TryGetField(fieldReferenceName, out FieldEntry _);

    public bool IsValidField(int fieldId) => this.m_fieldDict.TryGetField(fieldId, out FieldEntry _);

    public int GetFieldId(string fieldReferenceName)
    {
      FieldEntry field;
      return this.m_fieldDict.TryGetField(fieldReferenceName, out field) ? field.FieldId : 0;
    }
  }
}
