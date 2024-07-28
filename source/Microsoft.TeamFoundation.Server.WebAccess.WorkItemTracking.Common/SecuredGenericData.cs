// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SecuredGenericData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class SecuredGenericData : ISecuredObject
  {
    [DataMember]
    public object[] Data;
    private Guid namespaceId;
    private int requiredPermissions;
    private string token;

    public SecuredGenericData(
      IEnumerable<object> data,
      Guid namespaceId,
      int permissions,
      string token)
    {
      this.Data = data.ToArray<object>();
      this.namespaceId = namespaceId;
      this.requiredPermissions = permissions;
      this.token = token;
    }

    public SecuredGenericData(IEnumerable<object> data, ISecuredObject securedObject)
      : this(data, securedObject.NamespaceId, securedObject.RequiredPermissions, securedObject.GetToken())
    {
    }

    public object this[int i] => this.Data[i];

    Guid ISecuredObject.NamespaceId => this.namespaceId;

    int ISecuredObject.RequiredPermissions => this.requiredPermissions;

    internal string Token => this.token;

    string ISecuredObject.GetToken() => this.token;
  }
}
