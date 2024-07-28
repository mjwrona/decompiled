// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.ChangeExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class ChangeExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Change ToWebApiChange(
      this Microsoft.TeamFoundation.Build2.Server.Change srvChange,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvChange == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Change) null;
      Microsoft.TeamFoundation.Build.WebApi.Change webApiChange = new Microsoft.TeamFoundation.Build.WebApi.Change(securedObject)
      {
        DisplayUri = srvChange.DisplayUri,
        Id = srvChange.Id,
        Location = srvChange.Location,
        Message = srvChange.Message,
        MessageTruncated = srvChange.MessageTruncated,
        Timestamp = srvChange.Timestamp,
        Type = srvChange.Type,
        Pusher = srvChange.Pusher
      };
      if (srvChange.Author != null)
      {
        Microsoft.TeamFoundation.Build.WebApi.Change change = webApiChange;
        ExternalIdentityRef externalIdentityRef = new ExternalIdentityRef(securedObject);
        externalIdentityRef.Descriptor = srvChange.Author.Descriptor;
        externalIdentityRef.DirectoryAlias = srvChange.Author.DirectoryAlias;
        externalIdentityRef.DisplayName = srvChange.Author.DisplayName;
        externalIdentityRef.Id = srvChange.Author.Id;
        externalIdentityRef.ImageUrl = srvChange.Author.ImageUrl;
        externalIdentityRef.Inactive = srvChange.Author.Inactive;
        externalIdentityRef.IsAadIdentity = srvChange.Author.IsAadIdentity;
        externalIdentityRef.IsContainer = srvChange.Author.IsContainer;
        externalIdentityRef.ProfileUrl = srvChange.Author.ProfileUrl;
        externalIdentityRef.Url = srvChange.Author.Url;
        externalIdentityRef.UniqueName = srvChange.Author.UniqueName;
        change.Author = (IdentityRef) externalIdentityRef;
        if (srvChange.Author.Links != null)
        {
          webApiChange.Author.Links = new ReferenceLinks();
          foreach (KeyValuePair<string, object> link in (IEnumerable<KeyValuePair<string, object>>) srvChange.Author.Links.Links)
          {
            ReferenceLink referenceLink = link.Value as ReferenceLink;
            webApiChange.Author.Links.AddLink(link.Key, referenceLink.Href, securedObject);
          }
        }
      }
      return webApiChange;
    }
  }
}
