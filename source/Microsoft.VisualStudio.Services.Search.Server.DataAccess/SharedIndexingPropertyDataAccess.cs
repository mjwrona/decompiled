// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SharedIndexingPropertyDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public class SharedIndexingPropertyDataAccess : ISharedIndexingPropertyDataAccess
  {
    public DeploymentIndexingProperties GetIndexingProperties(IVssRequestContext requestContext)
    {
      string configValue = requestContext.GetConfigValue("/Service/ALMSearch/Settings/SettingEntityIndexProperties");
      return string.IsNullOrWhiteSpace(configValue) ? (DeploymentIndexingProperties) null : (DeploymentIndexingProperties) Serializers.FromXmlString(configValue, typeof (DeploymentIndexingProperties));
    }

    public void UpdateIndexingProperties(
      IVssRequestContext requestContext,
      DeploymentIndexingProperties indexingProperties)
    {
      string str = indexingProperties != null ? Serializers.ToXmlString((object) indexingProperties, typeof (DeploymentIndexingProperties)) : throw new ArgumentNullException(nameof (indexingProperties));
      requestContext.SetConfigValue<string>("/Service/ALMSearch/Settings/SettingEntityIndexProperties", str);
    }
  }
}
