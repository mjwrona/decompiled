// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.CustomSqlError
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

namespace Microsoft.Azure.Boards.CssNodes
{
  public static class CustomSqlError
  {
    public const int GenericError = 400000;
    public const int CreateACENoObjectError = 400013;
    public const int CreateACENoActionError = 400014;
    public const int UnregisterProjectError = 400015;
    public const int RegisterProjectError = 400016;
    public const int InternalStoredProcdureError = 400017;
    public const int RegisterObjectExistsError = 400018;
    public const int RegisterObjectNoClassError = 400019;
    public const int RegisterObjectNoProjectError = 400020;
    public const int RegisterObjectBadParentError = 400021;
    public const int ProjectUriDoesNotExist = 400025;
    public const int ClassIdDoesNotExist = 400027;
    public const int SecurityObjectDoesNotExist = 400028;
    public const int SecurityActionDoesNotExist = 400029;
    public const int DeleteACEException = 400030;
    public const int AddGroupProjectsDontMatch = 400032;
    public const int BadParentObjectClassId = 400033;
    public const int CircularObjectInheritance = 400034;
    public const int RegisterObjectProjectMismatch = 400035;
    public const int NodeDoesNotExist = 450000;
    public const int ProjectDoesNotExist = 450001;
    public const int ParentNodeDoesNotExist = 450002;
    public const int ReclassifyNodeDoesNotExist = 450003;
    public const int ProjectAlreadyExists = 450004;
    public const int NodeAlreadyExists = 450005;
    public const int CannotModifyRootNode = 450006;
    public const int MoveArgumentOutOfRange = 450007;
    public const int CircularNodeReference = 450008;
    public const int CannotChangeTrees = 450009;
    public const int MaxDepthExceeded = 450010;
    public const int ReclassifiedToDifferentTree = 450011;
    public const int ReclassifiedToSubTree = 450012;
    public const int ProjectNameNotRecognized = 450013;
    public const int CannotAddDateToNonIteration = 450014;
    public const int RegisterSyncAppDefinitionExistingRefName = 470000;
    public const int SyncBadBaselineRev = 470006;
    public const int SyncSupersededBaselineRev = 470007;
  }
}
