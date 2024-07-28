DECLARE @useMultiSubnetFailover BIT = 0

-- To use MultiSubnetFailover=true in connection string, HADR must be enabled on SQL Server and connection
-- must be established via TCP protocol. We will query sys.availability_group_listeners to determine
-- if connection is established via availability group listener. If we are not connecting via AGL and MultiSubnetFailover is set to true,
-- we will get the following error:
-- Connecting to a mirrored SQL Server instance using the MultiSubnetFailover connection option is not supported.
IF SERVERPROPERTY('IsHadrEnabled') = 1 AND CONNECTIONPROPERTY('net_transport') = 'TCP'
BEGIN
    BEGIN TRY
        IF (EXISTS( SELECT *
                    FROM    sys.availability_group_listeners
                    -- IPv6 address is 37 characters
                    -- ip_configuration_string_from_cluster is in format of ('IP Address: <IPv4/IPv6>'{ or 'IP Address: <IPv4/IPv6>'})
                    WHERE   ip_configuration_string_from_cluster LIKE '%''IP Address: ' +  CONVERT(VARCHAR(40), CONNECTIONPROPERTY('local_net_address')) + '''%'))
        BEGIN
            -- We are connected via availability group listener
            SET @useMultiSubnetFailover = 1
        END
    END TRY
    BEGIN CATCH
        -- User does not have permission to query sys.availability_group_listeners view. We will not use MultiSubnetFailover in this case
    END CATCH
END

SELECT @useMultiSubnetFailover UseMultiSubnetFailover

