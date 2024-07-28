// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetSoftDeletedObjectsResponse`2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetSoftDeletedObjectsResponse<TIdentifier, TType> : AadServiceResponse where TType : AadObject
  {
    public GetSoftDeletedObjectsResponse() => this.Objects = (IDictionary<TIdentifier, GetSoftDeletedObjectResponse<TType>>) new Dictionary<TIdentifier, GetSoftDeletedObjectResponse<TType>>();

    public IDictionary<TIdentifier, GetSoftDeletedObjectResponse<TType>> Objects { get; set; }

    public override string CompareAndGetDifference(AadServiceResponse anotherResponse) => !(anotherResponse is GetSoftDeletedObjectsResponse<TIdentifier, TType> deletedObjectsResponse) ? string.Format("Target response is null or not of type '{0}'.", (object) typeof (GetSoftDeletedObjectsResponse<TIdentifier, TType>)) : AadObjectCompareHelpers.CompareAndGetDifferenceDeletedObjects<TIdentifier, TType>(this.Objects, deletedObjectsResponse.Objects);
  }
}
