// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.DuplicatePropertyNameChecker
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal class DuplicatePropertyNameChecker : IDuplicatePropertyNameChecker
  {
    private IDictionary<string, DuplicatePropertyNameChecker.State> propertyState = (IDictionary<string, DuplicatePropertyNameChecker.State>) new Dictionary<string, DuplicatePropertyNameChecker.State>();

    public void ValidatePropertyUniqueness(ODataPropertyInfo property)
    {
      try
      {
        this.propertyState.Add(property.Name, DuplicatePropertyNameChecker.State.NonNestedResource);
      }
      catch (ArgumentException ex)
      {
        throw new ODataException(Strings.DuplicatePropertyNamesNotAllowed((object) property.Name));
      }
    }

    public void ValidatePropertyUniqueness(ODataNestedResourceInfo property)
    {
      DuplicatePropertyNameChecker.State state;
      if (!this.propertyState.TryGetValue(property.Name, out state))
      {
        this.propertyState[property.Name] = DuplicatePropertyNameChecker.State.NestedResource;
      }
      else
      {
        if (state != DuplicatePropertyNameChecker.State.AssociationLink)
          throw new ODataException(Strings.DuplicatePropertyNamesNotAllowed((object) property.Name));
        this.propertyState[property.Name] = DuplicatePropertyNameChecker.State.NestedResource | DuplicatePropertyNameChecker.State.AssociationLink;
      }
    }

    public void ValidatePropertyOpenForAssociationLink(string propertyName)
    {
      DuplicatePropertyNameChecker.State state;
      if (!this.propertyState.TryGetValue(propertyName, out state))
      {
        this.propertyState[propertyName] = DuplicatePropertyNameChecker.State.AssociationLink;
      }
      else
      {
        if (state != DuplicatePropertyNameChecker.State.NestedResource)
          throw new ODataException(Strings.DuplicatePropertyNamesNotAllowed((object) propertyName));
        this.propertyState[propertyName] = DuplicatePropertyNameChecker.State.NestedResource | DuplicatePropertyNameChecker.State.AssociationLink;
      }
    }

    public void Reset() => this.propertyState.Clear();

    [Flags]
    private enum State
    {
      NonNestedResource = 0,
      NestedResource = 1,
      AssociationLink = 2,
    }
  }
}
