// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceManagementComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ResourceManagementComponent2 : ResourceManagementComponent
  {
    private const int c_maxSettingValueLength = 64;

    public ResultCollection QueryResourceManagementSettings()
    {
      this.PrepareStoredProcedure("prc_QueryResourceManagementSettings");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryResourceManagementSettings", this.RequestContext);
      resultCollection.AddBinder<ResourceManagementSetting>((ObjectBinder<ResourceManagementSetting>) new ResourceManagementSettingColumns());
      return resultCollection;
    }

    public ResourceManagementSetting GetResourceManagementSetting(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.PrepareStoredProcedure("prc_GetResourceManagementSetting");
      this.BindString("@name", name, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetResourceManagementSetting", this.RequestContext);
      resultCollection.AddBinder<ResourceManagementSetting>((ObjectBinder<ResourceManagementSetting>) new ResourceManagementSettingColumns());
      return resultCollection.GetCurrent<ResourceManagementSetting>().Items.FirstOrDefault<ResourceManagementSetting>();
    }

    public void SetResourceManagementSetting(string name, string value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.PrepareStoredProcedure("prc_SetResourceManagementSetting");
      this.BindString("@name", name, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@value", value, this.GetSettingValueLength(), BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public void DeleteResourceManagementSetting(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.PrepareStoredProcedure("prc_SetResourceManagementSetting");
      this.BindString("@name", name, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@value", string.Empty, this.GetSettingValueLength(), true, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    protected virtual int GetSettingValueLength() => 64;
  }
}
