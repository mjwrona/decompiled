// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ExtendedAttributeComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ExtendedAttributeComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ExtendedAttributeComponent>(0, true)
    }, "ExtendedAttribute");

    public void DeleteDatabaseAttribute(string attributeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(attributeName, nameof (attributeName));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_DeleteExtendedProperty.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@attributeName", attributeName, 128, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void DeleteAllDatabaseAttributes()
    {
      foreach (DatabaseAttribute databaseAttribute in this.ReadDatabaseAttributePattern("%"))
        this.DeleteDatabaseAttribute(databaseAttribute.Name);
    }

    public string ReadDatabaseAttribute(string attributeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(attributeName, nameof (attributeName));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryExtendedProperty.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@attributeName", attributeName, 128, false, SqlDbType.NVarChar);
      ObjectBinder<DatabaseAttribute> objectBinder = (ObjectBinder<DatabaseAttribute>) new DatabaseAttributeColumns(this.ExecuteReader(), "fn_listextendedproperty");
      return objectBinder.Items.Count > 0 ? objectBinder.Items[0].Value : (string) null;
    }

    public string[] ReadDatabaseAttributes(params string[] attributeNames)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) attributeNames, nameof (attributeNames));
      List<DatabaseAttribute> source = this.ReadDatabaseAttributePattern("%");
      string[] strArray = new string[attributeNames.Length];
      for (int index = 0; index < attributeNames.Length; ++index)
      {
        string attributeName = attributeNames[index];
        DatabaseAttribute databaseAttribute = source.FirstOrDefault<DatabaseAttribute>((System.Func<DatabaseAttribute, bool>) (attr => string.Equals(attr.Name, attributeName, StringComparison.OrdinalIgnoreCase)));
        if (databaseAttribute != null)
          strArray[index] = databaseAttribute.Value;
      }
      return strArray;
    }

    public void WriteDatabaseAttribute(string attributeName, string attributeValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(attributeName, nameof (attributeName));
      ArgumentUtility.CheckForNull<string>(attributeValue, nameof (attributeValue));
      if (attributeValue.Length > 3750)
        throw new ArgumentException("The length of an extended attribute cannot be more than 3750 characters.", nameof (attributeValue));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_SetExtendedProperty.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@attributeName", attributeName, 128, false, SqlDbType.NVarChar);
      this.BindString("@attributeValue", attributeValue, attributeValue.Length, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public int RenameDatabaseAttribute(string attributeName, string newAttributeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(attributeName, nameof (attributeName));
      ArgumentUtility.CheckStringForNullOrEmpty(newAttributeName, nameof (newAttributeName));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_RenameExtendedProperty.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@attributeName", attributeName, 128, false, SqlDbType.NVarChar);
      this.BindString("@newAttributeName", newAttributeName, 128, false, SqlDbType.NVarChar);
      return (int) this.ExecuteScalar();
    }

    public List<DatabaseAttribute> ReadDatabaseAttributePattern(string attributePattern)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(attributePattern, nameof (attributePattern));
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryExtendedPropertyPattern.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindString("@attributePattern", attributePattern, 128, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "stmt_QueryExtendedPropertyPattern.sql", (IVssRequestContext) null);
      resultCollection.AddBinder<DatabaseAttribute>((ObjectBinder<DatabaseAttribute>) new DatabaseAttributeColumns());
      return resultCollection.GetCurrent<DatabaseAttribute>().Items;
    }

    public virtual string ReadServiceLevelStamp()
    {
      string str = this.ReadDatabaseAttribute(TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelStamp);
      if (string.IsNullOrEmpty(str) && string.Equals(this.ReadDatabaseAttribute("TFS_FRAMEWORK_SCHEMA_VERSION"), "Team Foundation Server 2010 Beta1", StringComparison.Ordinal))
        str = "Tfs2010.Beta1";
      return str;
    }

    public string ReadServiceLevelToStamp() => this.ReadDatabaseAttribute(TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelToStamp);

    internal void TransferExtendedProperties()
    {
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_TransferExtendedProperties.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
    }

    internal DeploymentType ReadDeploymentTypeStamp()
    {
      DeploymentType deploymentType = DeploymentType.OnPremises;
      string str = this.ReadDatabaseAttribute(TeamFoundationSqlResourceComponent.ExtendedPropertyDeploymentTypeStamp);
      if (!string.IsNullOrEmpty(str))
        deploymentType = (DeploymentType) Enum.Parse(typeof (DeploymentType), str);
      return deploymentType;
    }

    internal string ReadReleaseDescriptionStamp() => this.ReadDatabaseAttribute(TeamFoundationSqlResourceComponent.ExtendedPropertyReleaseDescriptionStamp);
  }
}
