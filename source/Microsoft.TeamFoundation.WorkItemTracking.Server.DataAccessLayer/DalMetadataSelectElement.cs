// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalMetadataSelectElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalMetadataSelectElement : DalSqlElement
  {
    private int m_numberOfMetadataTables;
    private int[] m_tablesRequestedInParamOrder;
    private MetadataTable[] m_tablesRequested;
    private bool m_streamMetadata;
    private bool m_filterMetadata;
    private static readonly PayloadTableSchema[] s_metadataTableSchemas = new PayloadTableSchema[13]
    {
      new PayloadTableSchema(new PayloadTableSchema.Column[9]
      {
        new PayloadTableSchema.Column("AreaID", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("TypeID", typeof (int)),
        new PayloadTableSchema.Column("Name", typeof (string)),
        new PayloadTableSchema.Column("ParentID", typeof (int)),
        new PayloadTableSchema.Column("fAdminOnly", typeof (bool)),
        new PayloadTableSchema.Column("StructureType", typeof (int)),
        new PayloadTableSchema.Column("GUID", typeof (string)),
        new PayloadTableSchema.Column("CacheStamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[15]
      {
        new PayloadTableSchema.Column("FldID", typeof (int)),
        new PayloadTableSchema.Column("Type", typeof (int)),
        new PayloadTableSchema.Column("ParentFldID", typeof (int)),
        new PayloadTableSchema.Column("Name", typeof (string)),
        new PayloadTableSchema.Column("ReferenceName", typeof (string)),
        new PayloadTableSchema.Column("fEditable", typeof (bool)),
        new PayloadTableSchema.Column("fSemiEditable", typeof (bool)),
        new PayloadTableSchema.Column("ReportingType", typeof (int)),
        new PayloadTableSchema.Column("ReportingFormula", typeof (int)),
        new PayloadTableSchema.Column("ReportingName", typeof (string)),
        new PayloadTableSchema.Column("ReportingReferenceName", typeof (string)),
        new PayloadTableSchema.Column("fReportingEnabled", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[])),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("fSupportsTextQuery", typeof (bool))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[8]
      {
        new PayloadTableSchema.Column("PropID", typeof (int)),
        new PayloadTableSchema.Column("AreaID", typeof (int)),
        new PayloadTableSchema.Column("TreeType", typeof (int)),
        new PayloadTableSchema.Column("Name", typeof (string)),
        new PayloadTableSchema.Column("Value", typeof (string)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("CacheStamp", typeof (byte[])),
        new PayloadTableSchema.Column("WorkItemTypeID", typeof (int))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[6]
      {
        new PayloadTableSchema.Column("ConstID", typeof (int)),
        new PayloadTableSchema.Column("DisplayName", typeof (string)),
        new PayloadTableSchema.Column("String", typeof (string)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("Sid", typeof (string)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[27]
      {
        new PayloadTableSchema.Column("RuleID", typeof (int)),
        new PayloadTableSchema.Column("RootTreeID", typeof (int)),
        new PayloadTableSchema.Column("AreaID", typeof (int)),
        new PayloadTableSchema.Column("PersonID", typeof (int)),
        new PayloadTableSchema.Column("ObjectTypeScopeID", typeof (int)),
        new PayloadTableSchema.Column("Fld1ID", typeof (int)),
        new PayloadTableSchema.Column("Fld1IsConstID", typeof (int)),
        new PayloadTableSchema.Column("Fld1WasConstID", typeof (int)),
        new PayloadTableSchema.Column("Fld2ID", typeof (int)),
        new PayloadTableSchema.Column("Fld2IsConstID", typeof (int)),
        new PayloadTableSchema.Column("Fld2WasConstID", typeof (int)),
        new PayloadTableSchema.Column("Fld3ID", typeof (int)),
        new PayloadTableSchema.Column("Fld3IsConstID", typeof (int)),
        new PayloadTableSchema.Column("Fld3WasConstID", typeof (int)),
        new PayloadTableSchema.Column("Fld4ID", typeof (int)),
        new PayloadTableSchema.Column("Fld4IsConstID", typeof (int)),
        new PayloadTableSchema.Column("Fld4WasConstID", typeof (int)),
        new PayloadTableSchema.Column("IfFldID", typeof (int)),
        new PayloadTableSchema.Column("IfConstID", typeof (int)),
        new PayloadTableSchema.Column("If2FldID", typeof (int)),
        new PayloadTableSchema.Column("If2ConstID", typeof (int)),
        new PayloadTableSchema.Column("ThenFldID", typeof (int)),
        new PayloadTableSchema.Column("ThenConstID", typeof (int)),
        new PayloadTableSchema.Column("RuleFlags1", typeof (int)),
        new PayloadTableSchema.Column("RuleFlags2", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[5]
      {
        new PayloadTableSchema.Column("RuleSetID", typeof (int)),
        new PayloadTableSchema.Column("ParentID", typeof (int)),
        new PayloadTableSchema.Column("ConstID", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[10]
      {
        new PayloadTableSchema.Column("FldUsageID", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("ObjectID", typeof (int)),
        new PayloadTableSchema.Column("FldID", typeof (int)),
        new PayloadTableSchema.Column("DirectObjectID", typeof (int)),
        new PayloadTableSchema.Column("fOftenQueried", typeof (bool)),
        new PayloadTableSchema.Column("fCore", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[])),
        new PayloadTableSchema.Column("fOftenQueriedAsText", typeof (bool)),
        new PayloadTableSchema.Column("fSupportsTextQuery", typeof (bool))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[6]
      {
        new PayloadTableSchema.Column("WorkItemTypeID", typeof (int)),
        new PayloadTableSchema.Column("NameConstantID", typeof (int)),
        new PayloadTableSchema.Column("ProjectID", typeof (int)),
        new PayloadTableSchema.Column("DescriptionID", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[6]
      {
        new PayloadTableSchema.Column("WorkItemTypeUsageID", typeof (int)),
        new PayloadTableSchema.Column("FieldID", typeof (int)),
        new PayloadTableSchema.Column("WorkItemTypeID", typeof (int)),
        new PayloadTableSchema.Column("fGreyOut", typeof (bool)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[7]
      {
        new PayloadTableSchema.Column("ActionID", typeof (int)),
        new PayloadTableSchema.Column("Name", typeof (string)),
        new PayloadTableSchema.Column("WorkItemTypeID", typeof (int)),
        new PayloadTableSchema.Column("FromStateConstID", typeof (int)),
        new PayloadTableSchema.Column("ToStateConstID", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[8]
      {
        new PayloadTableSchema.Column("ReferenceName", typeof (string)),
        new PayloadTableSchema.Column("ForwardName", typeof (string)),
        new PayloadTableSchema.Column("ForwardID", typeof (short)),
        new PayloadTableSchema.Column("ReverseName", typeof (string)),
        new PayloadTableSchema.Column("ReverseID", typeof (short)),
        new PayloadTableSchema.Column("Rules", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("CacheStamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[7]
      {
        new PayloadTableSchema.Column("WorkItemTypeCategoryID", typeof (int)),
        new PayloadTableSchema.Column("ProjectID", typeof (int)),
        new PayloadTableSchema.Column("Name", typeof (string)),
        new PayloadTableSchema.Column("ReferenceName", typeof (string)),
        new PayloadTableSchema.Column("DefaultWorkItemTypeID", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[]))
      }),
      new PayloadTableSchema(new PayloadTableSchema.Column[5]
      {
        new PayloadTableSchema.Column("WorkItemTypeCategoryMemberID", typeof (int)),
        new PayloadTableSchema.Column("WorkItemTypeCategoryID", typeof (int)),
        new PayloadTableSchema.Column("WorkItemTypeID", typeof (int)),
        new PayloadTableSchema.Column("fDeleted", typeof (bool)),
        new PayloadTableSchema.Column("Cachestamp", typeof (byte[]))
      })
    };

    public DalMetadataSelectElement()
    {
      this.m_numberOfMetadataTables = 13;
      this.m_tablesRequestedInParamOrder = new int[this.m_numberOfMetadataTables];
    }

    public virtual void JoinBatch(
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      bool streamMetadata = false)
    {
      this.m_streamMetadata = streamMetadata;
      if (tablesRequested == null || tablesRequested.Length == 0)
        return;
      if (rowVersions == null)
        throw new ArgumentNullException(nameof (rowVersions));
      if (tablesRequested.Length != rowVersions.Length)
        throw new ArgumentException(DalResourceStrings.Get("InvalidRowVersionsProvidedException"), nameof (tablesRequested));
      bool aadBackedAccount = this.SqlBatch.RequestContext.WitContext().IsAadBackedAccount;
      this.m_tablesRequested = tablesRequested;
      for (int index = 0; index < rowVersions.Length; ++index)
      {
        string newSql = this.SqlBatch.AddParameterBinary(DalMetadataSelectElement.ConvertLongToByteArray(rowVersions[index]));
        this.SqlBatch.AppendSql("declare ");
        this.SqlBatch.AppendSql("@RowVer");
        this.SqlBatch.AppendSql(index.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.SqlBatch.AppendSql(" as binary(8) ");
        this.SqlBatch.AppendSql("set ");
        this.SqlBatch.AppendSql("@RowVer");
        this.SqlBatch.AppendSql(index.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.SqlBatch.AppendSql("=");
        this.SqlBatch.AppendSql(newSql);
        this.SqlBatch.AppendSql(Environment.NewLine);
      }
      for (int index1 = 0; index1 < this.m_numberOfMetadataTables; ++index1)
      {
        this.m_tablesRequestedInParamOrder[index1] = -1;
        for (int index2 = 0; index2 < tablesRequested.Length; ++index2)
        {
          if (tablesRequested[index2] == (MetadataTable) index1)
          {
            this.m_tablesRequestedInParamOrder[index1] = index2;
            break;
          }
        }
      }
      this.SqlBatch.AppendSql("exec dbo.");
      this.SqlBatch.AppendSql("GetAdminData");
      this.SqlBatch.AppendSql(" ");
      this.AppendPartitionIdVariable();
      this.m_filterMetadata = this.SqlBatch.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.SqlBatch.RequestContext).MetadataFilterEnabled;
      if (this.m_filterMetadata)
        this.SqlBatch.AppendSql("@PersonId");
      else
        this.SqlBatch.AppendSql("default");
      this.SqlBatch.AppendSql(",");
      this.SqlBatch.AppendSql(this.SqlBatch.AddParameterInt(this.ClientVersion));
      for (int index = 0; index < this.m_numberOfMetadataTables; ++index)
      {
        this.SqlBatch.AppendSql(",");
        if (this.m_tablesRequestedInParamOrder[index] == -1)
        {
          this.SqlBatch.AppendSql("null");
        }
        else
        {
          this.SqlBatch.AppendSql("@RowVer");
          this.SqlBatch.AppendSql(this.m_tablesRequestedInParamOrder[index].ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
      }
      if (this.Version >= 9)
      {
        this.SqlBatch.AppendSql(",");
        if (this.m_filterMetadata)
        {
          IEnumerable<int> projects;
          this.SqlBatch.AppendSql(this.SqlBatch.AddParameterBit(this.SqlBatch.RequestContext.GetService<UserProjectsCache>().GetUserProjects(this.SqlBatch.RequestContext, out projects)));
          this.SqlBatch.AppendSql(",");
          this.SqlBatch.AppendSql(this.SqlBatch.AddParameterTable<int>((WorkItemTrackingTableValueParameter<int>) new Int32Table(projects)));
          this.SqlBatch.SetTableSchema(1, (IPayloadTableSchema) DalGetDbStampElement.DbStampTableSchema);
        }
        else
          this.SqlBatch.AppendSql("default, default");
      }
      if (this.Version >= 30)
      {
        this.SqlBatch.AppendSql(",");
        bool hostedDeployment = this.SqlBatch.RequestContext.ExecutionEnvironment.IsHostedDeployment;
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterBit(aadBackedAccount));
        this.SqlBatch.AppendSql(",");
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterBit(hostedDeployment));
      }
      if (this.Version >= 33)
      {
        this.SqlBatch.AppendSql(",");
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterBit(this.SqlBatch.RequestContext.GetIdentityDisplayType() == IdentityDisplayType.DisplayName));
      }
      if (this.Version >= 40)
      {
        List<int> intList = new List<int>();
        if (WorkItemTrackingFeatureFlags.IsPartialRuleGenerationEnabled(this.SqlBatch.RequestContext))
        {
          IProjectService service1 = this.SqlBatch.RequestContext.GetService<IProjectService>();
          ITeamFoundationProcessService service2 = this.SqlBatch.RequestContext.GetService<ITeamFoundationProcessService>();
          IVssRequestContext requestContext1 = this.SqlBatch.RequestContext.Elevate();
          WorkItemTrackingRequestContext trackingRequestContext = this.SqlBatch.RequestContext.WitContext();
          IVssRequestContext requestContext2 = requestContext1;
          IEnumerable<ProjectInfo> projects = service1.GetProjects(requestContext2, ProjectState.WellFormed);
          IEnumerable<ProjectInfo> projectInfos = this.SqlBatch.RequestContext.GetService<ILegacyProjectPropertiesReaderService>().PopulateProperties(projects, requestContext1, ProcessTemplateIdPropertyNames.ProcessTemplateType);
          HashSet<Guid> guidSet = new HashSet<Guid>();
          foreach (ProjectInfo projectInfo in projectInfos)
          {
            IList<ProjectProperty> properties = projectInfo.Properties;
            string input = properties != null ? (string) properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => StringComparer.OrdinalIgnoreCase.Equals(p.Name, ProcessTemplateIdPropertyNames.ProcessTemplateType)))?.Value : (string) (object) null;
            Guid result;
            TreeNode node;
            if (!string.IsNullOrEmpty(input) && Guid.TryParse(input, out result) && trackingRequestContext.TreeService.TryGetTreeNode(projectInfo.Id, projectInfo.Id, out node))
            {
              if (guidSet.Contains(result))
              {
                intList.Add(node.Id);
              }
              else
              {
                ProcessDescriptor descriptor;
                if (service2.TryGetProcessDescriptor(this.SqlBatch.RequestContext, result, out descriptor) && !descriptor.IsCustom)
                {
                  intList.Add(node.Id);
                  guidSet.Add(result);
                }
              }
            }
          }
        }
        this.SqlBatch.AppendSql(",");
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterTable<int>((WorkItemTrackingTableValueParameter<int>) new Int32Table((IEnumerable<int>) intList)));
      }
      this.SqlBatch.AppendSql(Environment.NewLine);
      for (int index = 0; index < DalMetadataSelectElement.s_metadataTableSchemas.Length; ++index)
      {
        if (this.m_tablesRequestedInParamOrder[index] == index)
          this.SqlBatch.SetTableSchema(index + this.MetadataTableOffset + 1, (IPayloadTableSchema) DalMetadataSelectElement.s_metadataTableSchemas[index]);
      }
      this.m_outputs = tablesRequested.Length + this.MetadataTableOffset;
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
    }

    public static byte[] ConvertLongToByteArray(long rowVersion)
    {
      long[] numArray = new long[8]
      {
        9151314442816847872L,
        71776119061217280L,
        280375465082880L,
        1095216660480L,
        4278190080L,
        16711680L,
        65280L,
        (long) byte.MaxValue
      };
      byte[] byteArray = new byte[8];
      for (int index = 0; index < 8; ++index)
        byteArray[index] = (byte) ((numArray[index] & rowVersion) >> 8 * (8 - index - 1));
      return byteArray;
    }

    public void GetResults(Payload metadataPayload)
    {
      if (this.m_streamMetadata)
      {
        metadataPayload.SqlAccess = this.SqlBatch.ResultPayload.SqlAccess;
        metadataPayload.SqlExceptionHandler = this.SqlBatch.ResultPayload.SqlExceptionHandler;
        metadataPayload.SqlTypeExeptionHandler = this.SqlBatch.ResultPayload.SqlTypeExeptionHandler;
      }
      if (this.m_tablesRequested == null || this.m_tablesRequested.Length == 0)
        return;
      int index1 = 0;
      for (int index2 = 0; index2 < this.m_tablesRequested.Length; ++index2)
      {
        while (this.m_tablesRequestedInParamOrder[index1] == -1)
        {
          ++index1;
          if (index1 >= this.m_numberOfMetadataTables)
            break;
        }
        if (index1 < this.m_numberOfMetadataTables)
        {
          string str = this.m_tablesRequested[this.m_tablesRequestedInParamOrder[index1]].ToString();
          PayloadTable table = this.SqlBatch.ResultPayload.Tables[this.MetadataTableIndex];
          table.TableName = str;
          this.SqlBatch.ResultPayload.Tables.Remove(table);
          metadataPayload.Tables.Add(table);
        }
        ++index1;
      }
    }

    private int MetadataTableOffset => this.SqlBatch.Version < 9 || !this.m_filterMetadata ? 0 : 1;

    private int MetadataTableIndex => this.m_index + this.MetadataTableOffset;

    public string GetDbStamp()
    {
      if (!this.m_filterMetadata)
        throw new InvalidOperationException();
      return (string) this.SqlBatch.ResultPayload.Tables[this.m_index].Rows[0][0];
    }
  }
}
