-- Parameter:
--   @resource string - The resource to obtain the session level exclusive lock on

DECLARE @returnValue INT
EXEC @returnValue = sp_getapplock @Resource=@resource, @LockMode='Exclusive', @LockOwner='Session', @LockTimeout=0
SELECT @returnValue