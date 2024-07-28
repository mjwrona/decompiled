SELECT  name,
        uid,
        sid,
        createdate,
        updatedate,
        CONVERT(BIT, hasdbaccess) hasdbaccess,
        CONVERT(BIT, isntgroup) isntgroup,
        CONVERT(BIT, isntuser) isntuser,
        CONVERT(BIT, issqluser) issqluser
        FROM    sys.sysusers u
WHERE   u.sid = @sid