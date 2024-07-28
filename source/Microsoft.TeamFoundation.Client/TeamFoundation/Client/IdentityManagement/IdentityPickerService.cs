// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IdentityManagement.IdentityPickerService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Client.IdentityManagement
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IdentityPickerService : ITfsTeamProjectCollectionObject
  {
    private TfsTeamProjectCollection Collection { get; set; }

    public void Initialize(TfsTeamProjectCollection projectCollection) => this.Collection = projectCollection;

    public async Task<IEnumerable<IdentityRef>> SearchIdentitiesAsync(
      string query,
      bool includeGroups = false)
    {
      IdentityPickerService identityPickerService = this;
      IdentitiesSearchRequestModel identitiesRequest = new IdentitiesSearchRequestModel()
      {
        Query = query,
        IdentityTypes = (IList<string>) new List<string>()
        {
          "user"
        },
        OperationScopes = (IList<string>) new List<string>()
        {
          "ims"
        },
        Properties = (IList<string>) new List<string>()
        {
          "DisplayName",
          "SamAccountName",
          "SignInAddress",
          "Active",
          "ScopeName",
          "SubjectDescriptor"
        },
        FilterByAncestorEntityIds = (IList<string>) new List<string>(),
        Options = new SearchOptions()
        {
          Options = new Dictionary<string, object>()
          {
            {
              "MinResults",
              (object) 40
            },
            {
              "MaxResults",
              (object) 40
            }
          }
        }
      };
      if (includeGroups)
        identitiesRequest.IdentityTypes.Add("group");
      if (identityPickerService.Collection.IsHostedServer)
        identitiesRequest.OperationScopes.Add("source");
      IList<QueryTokenResult> results = (await identityPickerService.Collection.GetClient<IdentityPickerHttpClient>().GetIdentitiesAsync(identitiesRequest).ConfigureAwait(false))?.Results;
      // ISSUE: reference to a compiler-generated method
      return (results != null ? results.SelectMany<QueryTokenResult, Identity>((Func<QueryTokenResult, IEnumerable<Identity>>) (qtr => (IEnumerable<Identity>) qtr.Identities)) : (IEnumerable<Identity>) null).Select<Identity, IdentityRef>(new Func<Identity, IdentityRef>(identityPickerService.\u003CSearchIdentitiesAsync\u003Eb__5_1));
    }

    public async Task<IEnumerable<IdentityRef>> GetMruAsync()
    {
      IdentityPickerService identityPickerService = this;
      IdentitiesGetMruResponseModel mruResponseModel = await identityPickerService.Collection.GetClient<IdentityPickerHttpClient>().GetMruAsync("me", "common", new IdentitiesGetMruRequestModel()
      {
        OperationScopes = (IList<string>) new List<string>()
        {
          "ims"
        },
        Properties = (IList<string>) new List<string>()
        {
          "DisplayName",
          "SamAccountName",
          "SignInAddress",
          "Active",
          "ScopeName",
          "SubjectDescriptor"
        },
        FilterByAncestorEntityIds = (IList<string>) new List<string>()
      }).ConfigureAwait(false);
      IEnumerable<IdentityRef> identityRefs;
      if (mruResponseModel == null)
      {
        identityRefs = (IEnumerable<IdentityRef>) null;
      }
      else
      {
        IList<Identity> mruIdentities = mruResponseModel.MruIdentities;
        // ISSUE: reference to a compiler-generated method
        identityRefs = mruIdentities != null ? mruIdentities.Select<Identity, IdentityRef>(new Func<Identity, IdentityRef>(identityPickerService.\u003CGetMruAsync\u003Eb__6_0)) : (IEnumerable<IdentityRef>) null;
      }
      return identityRefs ?? Enumerable.Empty<IdentityRef>();
    }

    private IdentityRef ConvertToIdentityRef(Identity identity)
    {
      string str = this.Collection.IsHostedServer || string.IsNullOrEmpty(identity.SamAccountName?.Trim()) ? identity.SignInAddress ?? string.Empty : (string.IsNullOrEmpty(identity.ScopeName?.Trim()) ? (string.IsNullOrEmpty(identity.SamAccountName) ? identity.LocalId : identity.SamAccountName) : identity.ScopeName + "\\" + identity.SamAccountName);
      IdentityRef identityRef = new IdentityRef();
      identityRef.DisplayName = identity.DisplayName;
      identityRef.UniqueName = str;
      SubjectDescriptor? subjectDescriptor1 = identity.SubjectDescriptor;
      SubjectDescriptor subjectDescriptor2;
      if (!subjectDescriptor1.HasValue)
      {
        subjectDescriptor2 = new SubjectDescriptor();
      }
      else
      {
        subjectDescriptor1 = identity.SubjectDescriptor;
        subjectDescriptor2 = subjectDescriptor1.Value;
      }
      identityRef.Descriptor = subjectDescriptor2;
      bool? active = identity.Active;
      int num;
      if (!active.HasValue)
      {
        num = 0;
      }
      else
      {
        active = identity.Active;
        num = !active.Value ? 1 : 0;
      }
      identityRef.Inactive = num != 0;
      identityRef.IsContainer = string.Equals(identity.EntityType, "group", StringComparison.OrdinalIgnoreCase);
      return identityRef;
    }
  }
}
