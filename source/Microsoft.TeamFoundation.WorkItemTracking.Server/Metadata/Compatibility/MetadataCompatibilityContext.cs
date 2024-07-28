// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility.MetadataCompatibilityContext
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility
{
  public class MetadataCompatibilityContext
  {
    private const string c_DeletedRowGenerationLimit = "/Service/WorkItemTracking/Settings/DeletedRowGenerationLimit";
    private const int c_DefaultDeletedRowGenerationLimit = 20000;
    private int? m_cachedDeletedRowGenerationLimit;

    public IReadOnlyCollection<MetadataProjectCompatibilityDescriptor> ProjectDescriptors { get; set; }

    public IDictionary<string, int> ConstantMap { get; set; }

    public IDictionary<int, int> TypeIdToFormPropIdMap { get; set; }

    public IDictionary<int, int> TypeIdToDescriptionPropIdMap { get; set; }

    public IDictionary<string, List<RuleRecord>> OutOfBoxRuleRecordsCache { get; set; }

    internal void SetupFakeWorkItemTypeIds(int maxProvisionedTypeId)
    {
      int num = maxProvisionedTypeId + 1;
      foreach (MetadataProjectCompatibilityDescriptor projectDescriptor in (IEnumerable<MetadataProjectCompatibilityDescriptor>) this.ProjectDescriptors)
      {
        foreach (MetadataWorkItemTypeCompatibilityDescriptor typeDescriptor in (IEnumerable<MetadataWorkItemTypeCompatibilityDescriptor>) projectDescriptor.TypeDescriptors)
        {
          if (!typeDescriptor.Type.Id.HasValue)
            typeDescriptor.Type.Id = new int?(num++);
        }
      }
    }

    internal bool IsBelowDeletedRowGenerationLimit(
      IVssRequestContext requestContext,
      int id,
      int userHighestId,
      string generatorName)
    {
      int rowGenerationLimit = this.GetDeletedRowGenerationLimit(requestContext);
      bool flag = userHighestId - id > rowGenerationLimit;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(nameof (id), (double) id);
      properties.Add(nameof (userHighestId), (double) userHighestId);
      properties.Add("RowCount", (double) (userHighestId - id));
      properties.Add("deletedRowGenerationLimitExceeded", flag);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, generatorName, "DeletedRowGeneration", properties);
      return !flag;
    }

    internal static void ReportError(
      IVssRequestContext requestContext,
      string function,
      string error)
    {
      requestContext.Trace(910601, TraceLevel.Warning, "WorkItemMetadataGenerator", function, error);
    }

    public int GetDeletedRowGenerationLimit(IVssRequestContext requestContext)
    {
      if (!this.m_cachedDeletedRowGenerationLimit.HasValue)
        this.m_cachedDeletedRowGenerationLimit = new int?(requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/DeletedRowGenerationLimit", true, 20000));
      return this.m_cachedDeletedRowGenerationLimit.Value;
    }
  }
}
