// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationStrongBoxService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class TeamFoundationStrongBoxService : ITfsConnectionObject
  {
    private TfsConnection m_tfs;
    private StrongBoxWebService m_webService;

    private TeamFoundationStrongBoxService()
    {
    }

    void ITfsConnectionObject.Initialize(TfsConnection server)
    {
      this.m_tfs = server;
      this.m_webService = new StrongBoxWebService(server);
    }

    public Guid CreateDrawer(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.m_webService.CreateDrawer(name);
    }

    public Guid UnlockDrawer(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return this.m_webService.UnlockDrawer(name);
    }

    public void DeleteDrawer(Guid drawerId)
    {
      ArgumentUtility.CheckForEmptyGuid(drawerId, nameof (drawerId));
      this.m_webService.DeleteDrawer(drawerId);
    }

    public string GetString(Guid drawerId, string lookupKey)
    {
      ArgumentUtility.CheckForEmptyGuid(drawerId, nameof (drawerId));
      ArgumentUtility.CheckStringForNullOrEmpty(lookupKey, nameof (lookupKey));
      return this.m_webService.GetString(drawerId, lookupKey);
    }

    [Obsolete]
    public string RetrieveFile(Guid drawerId, string lookupKey)
    {
      ArgumentUtility.CheckForEmptyGuid(drawerId, nameof (drawerId));
      ArgumentUtility.CheckStringForNullOrEmpty(lookupKey, nameof (lookupKey));
      throw new NotImplementedException();
    }

    public void AddString(Guid drawerId, string lookupKey, string value)
    {
      ArgumentUtility.CheckForEmptyGuid(drawerId, nameof (drawerId));
      ArgumentUtility.CheckStringForNullOrEmpty(lookupKey, nameof (lookupKey));
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      this.m_webService.AddString(drawerId, lookupKey, value);
    }

    [Obsolete]
    public void UploadFile(Guid drawerId, string lookupKey, Stream file)
    {
      ArgumentUtility.CheckForEmptyGuid(drawerId, nameof (drawerId));
      ArgumentUtility.CheckStringForNullOrEmpty(lookupKey, nameof (lookupKey));
      ArgumentUtility.CheckForNull<Stream>(file, nameof (file));
      throw new NotImplementedException();
    }

    public void DeleteItem(Guid drawerId, string lookupKey)
    {
      ArgumentUtility.CheckForEmptyGuid(drawerId, nameof (drawerId));
      ArgumentUtility.CheckStringForNullOrEmpty(lookupKey, nameof (lookupKey));
      this.m_webService.DeleteItem(drawerId, lookupKey);
    }
  }
}
