// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.AbstractProjectSecuredObject
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97A9928B-E499-4978-909F-1EBC8C5535AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public abstract class AbstractProjectSecuredObject : ISecuredObject
  {
    private ISecuredObject m_securedObject;

    public AbstractProjectSecuredObject()
    {
    }

    public AbstractProjectSecuredObject(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    public virtual void SetSecuredObject(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    [IgnoreDataMember]
    Guid ISecuredObject.NamespaceId
    {
      get
      {
        this.CheckForNullSecuredObject();
        return this.m_securedObject.NamespaceId;
      }
    }

    [IgnoreDataMember]
    int ISecuredObject.RequiredPermissions
    {
      get
      {
        this.CheckForNullSecuredObject();
        return this.m_securedObject.RequiredPermissions;
      }
    }

    string ISecuredObject.GetToken()
    {
      this.CheckForNullSecuredObject();
      return this.m_securedObject.GetToken();
    }

    protected static void SetSecuredObject(
      ISecuredObject securedObject,
      IEnumerable<AbstractProjectSecuredObject> securableObjects)
    {
      if (securableObjects == null)
        return;
      foreach (AbstractProjectSecuredObject securableObject in securableObjects)
        securableObject.SetSecuredObject(securedObject);
    }

    private void CheckForNullSecuredObject() => ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
  }
}
