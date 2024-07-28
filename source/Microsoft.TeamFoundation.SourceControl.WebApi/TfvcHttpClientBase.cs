// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcHttpClientBase
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [ResourceArea("8AA40520-446D-40E6-89F6-9C9F9CE44C48")]
  public abstract class TfvcHttpClientBase : TfvcCompatHttpClientBase
  {
    public TfvcHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TfvcHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TfvcHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TfvcHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TfvcHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<TfvcBranch> GetBranchAsync(
      string project,
      string path,
      bool? includeParent = null,
      bool? includeChildren = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      bool flag;
      if (includeParent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeParent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeParent), str);
      }
      if (includeChildren.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeChildren.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeChildren), str);
      }
      return this.SendAsync<TfvcBranch>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcBranch> GetBranchAsync(
      Guid project,
      string path,
      bool? includeParent = null,
      bool? includeChildren = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      bool flag;
      if (includeParent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeParent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeParent), str);
      }
      if (includeChildren.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeChildren.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeChildren), str);
      }
      return this.SendAsync<TfvcBranch>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcBranch> GetBranchAsync(
      string path,
      bool? includeParent = null,
      bool? includeChildren = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      bool flag;
      if (includeParent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeParent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeParent), str);
      }
      if (includeChildren.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeChildren.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeChildren), str);
      }
      return this.SendAsync<TfvcBranch>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcBranch>> GetBranchesAsync(
      string project,
      bool? includeParent = null,
      bool? includeChildren = null,
      bool? includeDeleted = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeParent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeParent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeParent), str);
      }
      if (includeChildren.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeChildren.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeChildren), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<TfvcBranch>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcBranch>> GetBranchesAsync(
      Guid project,
      bool? includeParent = null,
      bool? includeChildren = null,
      bool? includeDeleted = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeParent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeParent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeParent), str);
      }
      if (includeChildren.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeChildren.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeChildren), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<TfvcBranch>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcBranch>> GetBranchesAsync(
      bool? includeParent = null,
      bool? includeChildren = null,
      bool? includeDeleted = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeParent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeParent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeParent), str);
      }
      if (includeChildren.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeChildren.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeChildren), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<TfvcBranch>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcBranchRef>> GetBranchRefsAsync(
      string project,
      string scopePath,
      bool? includeDeleted = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (scopePath), scopePath);
      bool flag;
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<TfvcBranchRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcBranchRef>> GetBranchRefsAsync(
      Guid project,
      string scopePath,
      bool? includeDeleted = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (scopePath), scopePath);
      bool flag;
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<TfvcBranchRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcBranchRef>> GetBranchRefsAsync(
      string scopePath,
      bool? includeDeleted = null,
      bool? includeLinks = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc1f417e-239d-42e7-85e1-76e80cb2d6eb");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (scopePath), scopePath);
      bool flag;
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeLinks.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLinks.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLinks), str);
      }
      return this.SendAsync<List<TfvcBranchRef>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcChange>> GetChangesetChangesAsync(
      int? id = null,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f32b86f2-15b9-4fe6-81b1-6f8938617ee5");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<TfvcChange>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcChangesetRef> CreateChangesetAsync(
      TfvcChangeset changeset,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      HttpContent httpContent = (HttpContent) new ObjectContent<TfvcChangeset>(changeset, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TfvcChangesetRef>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TfvcChangesetRef> CreateChangesetAsync(
      TfvcChangeset changeset,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TfvcChangeset>(changeset, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TfvcChangesetRef>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TfvcChangesetRef> CreateChangesetAsync(
      TfvcChangeset changeset,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TfvcChangeset>(changeset, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TfvcChangesetRef>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TfvcChangeset> GetChangesetAsync(
      string project,
      int id,
      int? maxChangeCount = null,
      bool? includeDetails = null,
      bool? includeWorkItems = null,
      int? maxCommentLength = null,
      bool? includeSourceRename = null,
      int? skip = null,
      int? top = null,
      string orderby = null,
      TfvcChangesetSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (maxChangeCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxChangeCount.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxChangeCount), str);
      }
      bool flag;
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDetails), str);
      }
      if (includeWorkItems.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeWorkItems.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItems), str);
      }
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (includeSourceRename.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeSourceRename.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeSourceRename), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<TfvcChangeset>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcChangeset> GetChangesetAsync(
      Guid project,
      int id,
      int? maxChangeCount = null,
      bool? includeDetails = null,
      bool? includeWorkItems = null,
      int? maxCommentLength = null,
      bool? includeSourceRename = null,
      int? skip = null,
      int? top = null,
      string orderby = null,
      TfvcChangesetSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      object routeValues = (object) new
      {
        project = project,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (maxChangeCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxChangeCount.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxChangeCount), str);
      }
      bool flag;
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDetails), str);
      }
      if (includeWorkItems.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeWorkItems.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItems), str);
      }
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (includeSourceRename.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeSourceRename.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeSourceRename), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<TfvcChangeset>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcChangeset> GetChangesetAsync(
      int id,
      int? maxChangeCount = null,
      bool? includeDetails = null,
      bool? includeWorkItems = null,
      int? maxCommentLength = null,
      bool? includeSourceRename = null,
      int? skip = null,
      int? top = null,
      string orderby = null,
      TfvcChangesetSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      object routeValues = (object) new{ id = id };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (maxChangeCount.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxChangeCount.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxChangeCount), str);
      }
      bool flag;
      if (includeDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDetails), str);
      }
      if (includeWorkItems.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeWorkItems.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeWorkItems), str);
      }
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (includeSourceRename.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeSourceRename.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeSourceRename), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<TfvcChangeset>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcChangesetRef>> GetChangesetsAsync(
      string project,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      string orderby = null,
      TfvcChangesetSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<TfvcChangesetRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcChangesetRef>> GetChangesetsAsync(
      Guid project,
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      string orderby = null,
      TfvcChangesetSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<TfvcChangesetRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcChangesetRef>> GetChangesetsAsync(
      int? maxCommentLength = null,
      int? skip = null,
      int? top = null,
      string orderby = null,
      TfvcChangesetSearchCriteria searchCriteria = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0bc8f0a4-6bfb-42a9-ba84-139da7b99c49");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (maxCommentLength.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCommentLength.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCommentLength), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (orderby != null)
        keyValuePairList.Add("$orderby", orderby);
      if (searchCriteria != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (searchCriteria), (object) searchCriteria);
      return this.SendAsync<List<TfvcChangesetRef>>(method, locationId, version: new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcChangesetRef>> GetBatchedChangesetsAsync(
      TfvcChangesetsRequestData changesetsRequestData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b7e7c173-803c-4fea-9ec8-31ee35c5502a");
      HttpContent httpContent = (HttpContent) new ObjectContent<TfvcChangesetsRequestData>(changesetsRequestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TfvcChangesetRef>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<AssociatedWorkItem>> GetChangesetWorkItemsAsync(
      int? id = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<AssociatedWorkItem>>(new HttpMethod("GET"), new Guid("64ae0bea-1d71-47c9-a9e5-fe73f5ea0ff4"), (object) new
      {
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<List<TfvcItem>>> GetItemsBatchAsync(
      TfvcItemRequestData itemRequestData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fe6f827b-5f64-480f-b8af-1eca3b80e833");
      HttpContent httpContent = (HttpContent) new ObjectContent<TfvcItemRequestData>(itemRequestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<TfvcItem>>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<List<TfvcItem>>> GetItemsBatchAsync(
      TfvcItemRequestData itemRequestData,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fe6f827b-5f64-480f-b8af-1eca3b80e833");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TfvcItemRequestData>(itemRequestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<TfvcItem>>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<List<TfvcItem>>> GetItemsBatchAsync(
      TfvcItemRequestData itemRequestData,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("fe6f827b-5f64-480f-b8af-1eca3b80e833");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TfvcItemRequestData>(itemRequestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<List<TfvcItem>>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task<Stream> GetItemsBatchZipAsync(
      TfvcItemRequestData itemRequestData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("fe6f827b-5f64-480f-b8af-1eca3b80e833");
      HttpContent content = (HttpContent) new ObjectContent<TfvcItemRequestData>(itemRequestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), content: content, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemsBatchZipAsync(
      TfvcItemRequestData itemRequestData,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("fe6f827b-5f64-480f-b8af-1eca3b80e833");
      object routeValues = (object) new{ project = project };
      HttpContent content = (HttpContent) new ObjectContent<TfvcItemRequestData>(itemRequestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), content, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemsBatchZipAsync(
      TfvcItemRequestData itemRequestData,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("fe6f827b-5f64-480f-b8af-1eca3b80e833");
      object routeValues = (object) new{ project = project };
      HttpContent content = (HttpContent) new ObjectContent<TfvcItemRequestData>(itemRequestData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), content, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<TfvcItem> GetItemAsync(
      string project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      return this.SendAsync<TfvcItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcItem> GetItemAsync(
      Guid project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      return this.SendAsync<TfvcItem>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcItem> GetItemAsync(
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      bool flag;
      if (download.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = download.Value;
        string str = flag.ToString();
        collection.Add(nameof (download), str);
      }
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeContent.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeContent), str);
      }
      return this.SendAsync<TfvcItem>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemContentAsync(
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TfvcItem>> GetItemsAsync(
      string project,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeLinks = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<List<TfvcItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcItem>> GetItemsAsync(
      Guid project,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeLinks = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<List<TfvcItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcItem>> GetItemsAsync(
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      bool? includeLinks = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (includeLinks.HasValue)
        keyValuePairList.Add(nameof (includeLinks), includeLinks.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<List<TfvcItem>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemTextAsync(
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetItemZipAsync(
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      bool? includeContent = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcHttpClientBase tfvcHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (path), path);
      if (fileName != null)
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        tfvcHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      if (includeContent.HasValue)
        keyValuePairList.Add(nameof (includeContent), includeContent.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await tfvcHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await tfvcHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual Task<List<TfvcItem>> GetLabelItemsAsync(
      string labelId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("06166e34-de17-4b60-8cd1-23182a346fda");
      object routeValues = (object) new{ labelId = labelId };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<TfvcItem>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcLabel> GetLabelAsync(
      string project,
      string labelId,
      TfvcLabelRequestData requestData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a5d9bd7f-b661-4d0e-b9be-d9c16affae54");
      object routeValues = (object) new
      {
        project = project,
        labelId = labelId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (requestData), (object) requestData);
      return this.SendAsync<TfvcLabel>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcLabel> GetLabelAsync(
      Guid project,
      string labelId,
      TfvcLabelRequestData requestData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a5d9bd7f-b661-4d0e-b9be-d9c16affae54");
      object routeValues = (object) new
      {
        project = project,
        labelId = labelId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (requestData), (object) requestData);
      return this.SendAsync<TfvcLabel>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcLabel> GetLabelAsync(
      string labelId,
      TfvcLabelRequestData requestData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a5d9bd7f-b661-4d0e-b9be-d9c16affae54");
      object routeValues = (object) new{ labelId = labelId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (requestData), (object) requestData);
      return this.SendAsync<TfvcLabel>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcLabelRef>> GetLabelsAsync(
      string project,
      TfvcLabelRequestData requestData,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a5d9bd7f-b661-4d0e-b9be-d9c16affae54");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (requestData), (object) requestData);
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<TfvcLabelRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcLabelRef>> GetLabelsAsync(
      Guid project,
      TfvcLabelRequestData requestData,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a5d9bd7f-b661-4d0e-b9be-d9c16affae54");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (requestData), (object) requestData);
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<TfvcLabelRef>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcLabelRef>> GetLabelsAsync(
      TfvcLabelRequestData requestData,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a5d9bd7f-b661-4d0e-b9be-d9c16affae54");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (requestData), (object) requestData);
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<TfvcLabelRef>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcChange>> GetShelvesetChangesAsync(
      string shelvesetId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbaf075b-0445-4c34-9e5b-82292f856522");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (shelvesetId), shelvesetId);
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<TfvcChange>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TfvcShelveset> GetShelvesetAsync(
      string shelvesetId,
      TfvcShelvesetRequestData requestData = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e36d44fb-e907-4b0a-b194-f83f1ed32ad3");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (shelvesetId), shelvesetId);
      if (requestData != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (requestData), (object) requestData);
      return this.SendAsync<TfvcShelveset>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TfvcShelvesetRef>> GetShelvesetsAsync(
      TfvcShelvesetRequestData requestData = null,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e36d44fb-e907-4b0a-b194-f83f1ed32ad3");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (requestData != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (requestData), (object) requestData);
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<TfvcShelvesetRef>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<AssociatedWorkItem>> GetShelvesetWorkItemsAsync(
      string shelvesetId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a7a0c1c1-373e-425a-b031-a519474d743d");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (shelvesetId), shelvesetId);
      return this.SendAsync<List<AssociatedWorkItem>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TfvcStatistics> GetTfvcStatisticsAsync(
      string project,
      string scopePath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e15c74c0-3605-40e0-aed4-4cc61e549ed8");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      return this.SendAsync<TfvcStatistics>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TfvcStatistics> GetTfvcStatisticsAsync(
      Guid project,
      string scopePath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e15c74c0-3605-40e0-aed4-4cc61e549ed8");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      return this.SendAsync<TfvcStatistics>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<TfvcStatistics> GetTfvcStatisticsAsync(
      string scopePath = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e15c74c0-3605-40e0-aed4-4cc61e549ed8");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (scopePath != null)
        keyValuePairList.Add(nameof (scopePath), scopePath);
      return this.SendAsync<TfvcStatistics>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
