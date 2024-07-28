// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SecurityHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.DataImport;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class SecurityHelper
  {
    private readonly string m_drawerName;
    private readonly Guid m_importId;
    private const string c_area = "DataImport";
    private const string c_layer = "SecretUtility";

    public SecurityHelper(Guid importId)
    {
      ArgumentUtility.CheckForEmptyGuid(importId, nameof (importId));
      this.m_drawerName = string.Format("DataImport-{0}", (object) importId);
      this.m_importId = importId;
    }

    public string SecureConnectionString(IVssRequestContext requestContext, string connectionString)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckDeploymentRequestContext();
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      this.AddSecret(requestContext, ServicingTokenConstants.SourceDatabasePasswordDrawerItem, connectionStringBuilder.Password);
      requestContext.TraceAlways(15080820, TraceLevel.Info, "DataImport", "SecretUtility", DataImportConnectionStringHelper.ConnectionStringHashLogMessage(this.m_importId, connectionString));
      connectionStringBuilder.Password = string.Empty;
      return connectionStringBuilder.ToString();
    }

    public string UnlockConnectionString(IVssRequestContext requestContext, string connectionString)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckDeploymentRequestContext();
      string connectionString1 = new SqlConnectionStringBuilder(connectionString)
      {
        Password = (this.RetrieveSecret(requestContext, ServicingTokenConstants.SourceDatabasePasswordDrawerItem) ?? string.Empty)
      }.ToString();
      requestContext.TraceAlways(15080821, TraceLevel.Info, "DataImport", "SecretUtility", DataImportConnectionStringHelper.ConnectionStringHashLogMessage(this.m_importId, connectionString1));
      return connectionString1;
    }

    public void AddSecret(IVssRequestContext requestContext, string lookupKey, string secret)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(lookupKey, nameof (lookupKey));
      requestContext.CheckDeploymentRequestContext();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId;
      try
      {
        drawerId = service.CreateDrawer(requestContext, this.m_drawerName);
      }
      catch (StrongBoxDrawerExistsException ex)
      {
        drawerId = service.UnlockDrawer(requestContext, this.m_drawerName, false);
      }
      service.AddString(requestContext, drawerId, lookupKey, secret);
    }

    public string RetrieveSecret(IVssRequestContext requestContext, string lookupKey)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(lookupKey, nameof (lookupKey));
      requestContext.CheckDeploymentRequestContext();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string str = (string) null;
      Guid guid = service.UnlockDrawer(requestContext, this.m_drawerName, false);
      if (Guid.Empty.Equals(guid))
      {
        requestContext.TraceAlways(15080819, TraceLevel.Warning, "DataImport", "SecretUtility", "Failed to locate drawer with name " + this.m_drawerName);
      }
      else
      {
        try
        {
          str = service.GetString(requestContext, guid, lookupKey);
        }
        catch (StrongBoxItemNotFoundException ex)
        {
          requestContext.TraceAlways(15080822, TraceLevel.Warning, "DataImport", "SecretUtility", string.Format("Exception while looking up {0} in draw {1} with name {2} {3}", (object) lookupKey, (object) guid, (object) this.m_drawerName, (object) ex));
        }
      }
      return str;
    }

    public void DeleteAll(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckDeploymentRequestContext();
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, this.m_drawerName, false);
      if (!(drawerId != Guid.Empty))
        return;
      service.DeleteDrawer(requestContext, drawerId);
    }
  }
}
