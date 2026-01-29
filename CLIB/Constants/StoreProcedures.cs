namespace CLIB.Constants;

public class StoreProcedures
{
    public const string GetManagerDeals = @"DROP PROCEDURE IF EXISTS GetManagerDeals;
    CREATE DEFINER=`root`@`localhost` PROCEDURE `GetManagerDeals`(
        IN p_LoginIds TEXT,
        IN p_Symbols TEXT,
        IN p_FromDate DATETIME,
        IN p_ToDate DATETIME,
        IN p_FilterColumn VARCHAR(64),
        IN p_FilterValue VARCHAR(255),
        IN p_SortColumn VARCHAR(64),
    
        IN p_Page INT,
        IN p_Limit INT
    )
    BEGIN
        DECLARE where_clause TEXT DEFAULT ' WHERE 1=1 ';
        DECLARE Offset INT DEFAULT 0;

        -- LoginId filter
        IF p_LoginIds IS NOT NULL AND LENGTH(TRIM(p_LoginIds)) > 0 THEN
            SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(LoginId, ""', REPLACE(p_LoginIds, '""', '\""'), '"") > 0');
        END IF;

        -- Symbol filter
        IF p_Symbols IS NOT NULL AND LENGTH(TRIM(p_Symbols)) > 0 THEN
            SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(Symbol, ""', REPLACE(p_Symbols, '""', '\""'), '"") > 0');
        END IF;

        -- Date range filter
        IF p_FromDate IS NOT NULL AND p_ToDate IS NOT NULL THEN
            SET where_clause = CONCAT(where_clause, ' AND Time BETWEEN ""', p_FromDate, '"" AND ""', p_ToDate, '""');
        END IF;

        -- Generic column filter
        IF p_FilterColumn IS NOT NULL AND LENGTH(TRIM(p_FilterColumn)) > 0 AND p_FilterValue IS NOT NULL AND LENGTH(TRIM(p_FilterValue)) > 0 THEN
            SET where_clause = CONCAT(where_clause, ' AND ', p_FilterColumn, ' = ""', REPLACE(p_FilterValue, '""', '\""'), '""');
        END IF;

        -- 1️⃣ Get total count
        SET @sql = CONCAT('SELECT COUNT(*) AS TotalCount FROM Deals', where_clause);
        PREPARE count_stmt FROM @sql;
        EXECUTE count_stmt;
        DEALLOCATE PREPARE count_stmt;

        -- 2️⃣ Get paginated data
        SET @sql = CONCAT('SELECT * FROM Deals', where_clause);

        -- Sorting
       IF p_SortColumn IS NOT NULL AND LENGTH(TRIM(p_SortColumn)) > 0 THEN
        SET @orderByClause = '';
        SET @sortColumns = p_SortColumn;

        WHILE LENGTH(TRIM(@sortColumns)) > 0 DO
            SET @commaPos = INSTR(@sortColumns, ',');
            IF @commaPos > 0 THEN
                SET @col = TRIM(SUBSTRING(@sortColumns, 1, @commaPos - 1));
                SET @sortColumns = TRIM(SUBSTRING(@sortColumns, @commaPos + 1));
            ELSE
                SET @col = TRIM(@sortColumns);
                SET @sortColumns = '';
            END IF;

            IF LEFT(@col, 1) = '-' THEN
                SET @colName = SUBSTRING(@col, 2);
                SET @colOrder = CONCAT(@colName, ' DESC');
            ELSE
                SET @colName = @col;
                SET @colOrder = CONCAT(@colName, ' ASC');
            END IF;

            IF LENGTH(TRIM(@orderByClause)) = 0 THEN
                SET @orderByClause = @colOrder;
            ELSE
                SET @orderByClause = CONCAT(@orderByClause, ', ', @colOrder);
            END IF;
        END WHILE;

        SET @sql = CONCAT(@sql, ' ORDER BY ', @orderByClause);
    END IF;

        -- Pagination
        IF p_Page IS NOT NULL AND p_Limit IS NOT NULL AND p_Page > 0 AND p_Limit > 0 THEN
            SET Offset = (p_Page - 1) * p_Limit;
            SET @sql = CONCAT(@sql, ' LIMIT ', Offset, ', ', p_Limit);
        END IF;

        PREPARE data_stmt FROM @sql;
        EXECUTE data_stmt;
        DEALLOCATE PREPARE data_stmt;

    END;";
    public const string GetManagerOrders = @"DROP PROCEDURE IF EXISTS GetManagerOrders;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetManagerOrders`(
    IN p_LoginIds TEXT,
    IN p_Symbols TEXT,
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_FilterColumn VARCHAR(64),
    IN p_FilterValue VARCHAR(255),
    IN p_SortColumn VARCHAR(64),
    
    IN p_Page INT,
    IN p_Limit INT
)
BEGIN
    DECLARE where_clause TEXT DEFAULT ' WHERE 1=1 ';
    DECLARE Offset INT DEFAULT 0;

    -- LoginId filter
    IF p_LoginIds IS NOT NULL AND LENGTH(TRIM(p_LoginIds)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(Login, ""', REPLACE(p_LoginIds, '""', '\""'), '"") > 0');
    END IF;

    -- Symbol filter
    IF p_Symbols IS NOT NULL AND LENGTH(TRIM(p_Symbols)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(Symbol, ""', REPLACE(p_Symbols, '""', '\""'), '"") > 0');
    END IF;

    -- Date range filter
    IF p_FromDate IS NOT NULL AND p_ToDate IS NOT NULL THEN
        SET where_clause = CONCAT(where_clause, ' AND DoneTime BETWEEN ""', p_FromDate, '"" AND ""', p_ToDate, '""');
    END IF;

    -- Generic column filter
    IF p_FilterColumn IS NOT NULL AND LENGTH(TRIM(p_FilterColumn)) > 0 AND p_FilterValue IS NOT NULL AND LENGTH(TRIM(p_FilterValue)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND ', p_FilterColumn, ' = ""', REPLACE(p_FilterValue, '""', '\""'), '""');
    END IF;

    -- 1️⃣ Get total count
    SET @sql = CONCAT('SELECT COUNT(*) AS TotalCount FROM Orders', where_clause);
    PREPARE count_stmt FROM @sql;
    EXECUTE count_stmt;
    DEALLOCATE PREPARE count_stmt;

    -- 2️⃣ Get paginated data
    SET @sql = CONCAT('SELECT * FROM Orders', where_clause);

    -- Sorting
   IF p_SortColumn IS NOT NULL AND LENGTH(TRIM(p_SortColumn)) > 0 THEN
    SET @orderByClause = '';
    SET @sortColumns = p_SortColumn;

    WHILE LENGTH(TRIM(@sortColumns)) > 0 DO
        SET @commaPos = INSTR(@sortColumns, ',');
        IF @commaPos > 0 THEN
            SET @col = TRIM(SUBSTRING(@sortColumns, 1, @commaPos - 1));
            SET @sortColumns = TRIM(SUBSTRING(@sortColumns, @commaPos + 1));
        ELSE
            SET @col = TRIM(@sortColumns);
            SET @sortColumns = '';
        END IF;

        IF LEFT(@col, 1) = '-' THEN
            SET @colName = SUBSTRING(@col, 2);
            SET @colOrder = CONCAT(@colName, ' DESC');
        ELSE
            SET @colName = @col;
            SET @colOrder = CONCAT(@colName, ' ASC');
        END IF;

        IF LENGTH(TRIM(@orderByClause)) = 0 THEN
            SET @orderByClause = @colOrder;
        ELSE
            SET @orderByClause = CONCAT(@orderByClause, ', ', @colOrder);
        END IF;
    END WHILE;

    SET @sql = CONCAT(@sql, ' ORDER BY ', @orderByClause);
END IF;

    -- Pagination
    IF p_Page IS NOT NULL AND p_Limit IS NOT NULL AND p_Page > 0 AND p_Limit > 0 THEN
        SET Offset = (p_Page - 1) * p_Limit;
        SET @sql = CONCAT(@sql, ' LIMIT ', Offset, ', ', p_Limit);
    END IF;

    PREPARE data_stmt FROM @sql;
    EXECUTE data_stmt;
    DEALLOCATE PREPARE data_stmt;

END;
";

    public const string GetManagerDaily = @"DROP PROCEDURE IF EXISTS GetManagerDaily;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetManagerDaily`(
    IN p_LoginIds TEXT,
    
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_FilterColumn VARCHAR(64),
    IN p_FilterValue VARCHAR(255),
    IN p_SortColumn VARCHAR(64),
    
    IN p_Page INT,
    IN p_Limit INT
)
BEGIN
    DECLARE where_clause TEXT DEFAULT ' WHERE 1=1 ';
    DECLARE Offset INT DEFAULT 0;

    -- LoginId filter
    IF p_LoginIds IS NOT NULL AND LENGTH(TRIM(p_LoginIds)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(LoginId, ""', REPLACE(p_LoginIds, '""', '\""'), '"") > 0');
    END IF;

    

    -- Date range filter
    IF p_FromDate IS NOT NULL AND p_ToDate IS NOT NULL THEN
        SET where_clause = CONCAT(where_clause, ' AND Date BETWEEN ""', p_FromDate, '"" AND ""', p_ToDate, '""');
    END IF;

    -- Generic column filter
    IF p_FilterColumn IS NOT NULL AND LENGTH(TRIM(p_FilterColumn)) > 0 AND p_FilterValue IS NOT NULL AND LENGTH(TRIM(p_FilterValue)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND ', p_FilterColumn, ' = ""', REPLACE(p_FilterValue, '""', '\""'), '""');
    END IF;

    -- 1️⃣ Get total count
    SET @sql = CONCAT('SELECT COUNT(*) AS TotalCount FROM Daily', where_clause);
    PREPARE count_stmt FROM @sql;
    EXECUTE count_stmt;
    DEALLOCATE PREPARE count_stmt;

    -- 2️⃣ Get paginated data
    SET @sql = CONCAT('SELECT * FROM Daily', where_clause);

    -- Sorting
   IF p_SortColumn IS NOT NULL AND LENGTH(TRIM(p_SortColumn)) > 0 THEN
    SET @orderByClause = '';
    SET @sortColumns = p_SortColumn;

    WHILE LENGTH(TRIM(@sortColumns)) > 0 DO
        SET @commaPos = INSTR(@sortColumns, ',');
        IF @commaPos > 0 THEN
            SET @col = TRIM(SUBSTRING(@sortColumns, 1, @commaPos - 1));
            SET @sortColumns = TRIM(SUBSTRING(@sortColumns, @commaPos + 1));
        ELSE
            SET @col = TRIM(@sortColumns);
            SET @sortColumns = '';
        END IF;

        IF LEFT(@col, 1) = '-' THEN
            SET @colName = SUBSTRING(@col, 2);
            SET @colOrder = CONCAT(@colName, ' DESC');
        ELSE
            SET @colName = @col;
            SET @colOrder = CONCAT(@colName, ' ASC');
        END IF;

        IF LENGTH(TRIM(@orderByClause)) = 0 THEN
            SET @orderByClause = @colOrder;
        ELSE
            SET @orderByClause = CONCAT(@orderByClause, ', ', @colOrder);
        END IF;
    END WHILE;

    SET @sql = CONCAT(@sql, ' ORDER BY ', @orderByClause);
END IF;

    -- Pagination
    IF p_Page IS NOT NULL AND p_Limit IS NOT NULL AND p_Page > 0 AND p_Limit > 0 THEN
        SET Offset = (p_Page - 1) * p_Limit;
        SET @sql = CONCAT(@sql, ' LIMIT ', Offset, ', ', p_Limit);
    END IF;

    PREPARE data_stmt FROM @sql;
    EXECUTE data_stmt;
    DEALLOCATE PREPARE data_stmt;

END;";
    public const string GetManagerPositions = @"DROP PROCEDURE IF EXISTS GetManagerPositions;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetManagerPositions`(
    IN p_LoginIds TEXT,
    IN p_Symbols TEXT,
  
    IN p_FilterColumn VARCHAR(64),
    IN p_FilterValue VARCHAR(255),
    IN p_SortColumn VARCHAR(64),
    
    IN p_Page INT,
    IN p_Limit INT
)
BEGIN
    DECLARE where_clause TEXT DEFAULT ' WHERE 1=1 ';
    DECLARE Offset INT DEFAULT 0;

    -- LoginId filter
    IF p_LoginIds IS NOT NULL AND LENGTH(TRIM(p_LoginIds)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(LoginId, ""', REPLACE(p_LoginIds, '""', '\""'), '"") > 0');
    END IF;

    -- Symbol filter
    IF p_Symbols IS NOT NULL AND LENGTH(TRIM(p_Symbols)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(Symbol, ""', REPLACE(p_Symbols, '""', '\""'), '"") > 0');
    END IF;

    

    -- Generic column filter
    IF p_FilterColumn IS NOT NULL AND LENGTH(TRIM(p_FilterColumn)) > 0 AND p_FilterValue IS NOT NULL AND LENGTH(TRIM(p_FilterValue)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND ', p_FilterColumn, ' = ""', REPLACE(p_FilterValue, '""', '\""'), '""');
    END IF;

    -- 1️⃣ Get total count
    SET @sql = CONCAT('SELECT COUNT(*) AS TotalCount FROM Positions', where_clause);
    PREPARE count_stmt FROM @sql;
    EXECUTE count_stmt;
    DEALLOCATE PREPARE count_stmt;

    -- 2️⃣ Get paginated data
    SET @sql = CONCAT('SELECT * FROM Positions', where_clause);

    -- Sorting
   IF p_SortColumn IS NOT NULL AND LENGTH(TRIM(p_SortColumn)) > 0 THEN
    SET @orderByClause = '';
    SET @sortColumns = p_SortColumn;

    WHILE LENGTH(TRIM(@sortColumns)) > 0 DO
        SET @commaPos = INSTR(@sortColumns, ',');
        IF @commaPos > 0 THEN
            SET @col = TRIM(SUBSTRING(@sortColumns, 1, @commaPos - 1));
            SET @sortColumns = TRIM(SUBSTRING(@sortColumns, @commaPos + 1));
        ELSE
            SET @col = TRIM(@sortColumns);
            SET @sortColumns = '';
        END IF;

        IF LEFT(@col, 1) = '-' THEN
            SET @colName = SUBSTRING(@col, 2);
            SET @colOrder = CONCAT(@colName, ' DESC');
        ELSE
            SET @colName = @col;
            SET @colOrder = CONCAT(@colName, ' ASC');
        END IF;

        IF LENGTH(TRIM(@orderByClause)) = 0 THEN
            SET @orderByClause = @colOrder;
        ELSE
            SET @orderByClause = CONCAT(@orderByClause, ', ', @colOrder);
        END IF;
    END WHILE;

    SET @sql = CONCAT(@sql, ' ORDER BY ', @orderByClause);
END IF;

    -- Pagination
    IF p_Page IS NOT NULL AND p_Limit IS NOT NULL AND p_Page > 0 AND p_Limit > 0 THEN
        SET Offset = (p_Page - 1) * p_Limit;
        SET @sql = CONCAT(@sql, ' LIMIT ', Offset, ', ', p_Limit);
    END IF;

    PREPARE data_stmt FROM @sql;
    EXECUTE data_stmt;
    DEALLOCATE PREPARE data_stmt;

END;";
    public const string GetManagerSummaries = @"DROP PROCEDURE IF EXISTS GetManagerSummaries;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetManagerSummaries`(
    IN p_LoginIds TEXT,
   
    IN p_FilterColumn VARCHAR(64),
    IN p_FilterValue VARCHAR(255),
    IN p_SortColumn VARCHAR(64),
    IN p_SortDirection VARCHAR(4)
   
)
BEGIN
    SET @sql = 'SELECT * FROM SummaryReports WHERE 1=1';

    -- LoginId filter
    IF p_LoginIds IS NOT NULL AND LENGTH(TRIM(p_LoginIds)) > 0 THEN
        SET @sql = CONCAT(@sql, ' AND FIND_IN_SET(LoginId, ""', REPLACE(p_LoginIds, '""', '\""'), '"") > 0');
    END IF;

   

    -- Generic column filter
    IF p_FilterColumn IS NOT NULL AND LENGTH(TRIM(p_FilterColumn)) > 0 AND p_FilterValue IS NOT NULL AND LENGTH(TRIM(p_FilterValue)) > 0 THEN
        SET @sql = CONCAT(@sql, ' AND ', p_FilterColumn, ' = ""', REPLACE(p_FilterValue, '""', '\""'), '""');
    END IF;

    -- Sorting
    IF p_SortColumn IS NOT NULL AND LENGTH(TRIM(p_SortColumn)) > 0 AND p_SortDirection IS NOT NULL AND LENGTH(TRIM(p_SortDirection)) > 0 THEN
        SET @sql = CONCAT(@sql, ' ORDER BY ', p_SortColumn, ' ', p_SortDirection);
    END IF;

   

    PREPARE stmt FROM @sql;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
END;";
    public const string GetTradeData = @"DROP PROCEDURE IF EXISTS GetTradeData;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTradeData`(
    IN p_LoginIds TEXT,
    IN p_Symbols TEXT,
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME,
    IN p_Page INT,
    IN p_Limit INT
)
BEGIN
    DECLARE where_clause TEXT DEFAULT ' WHERE (Type = 1 OR Type = 2) ';
    DECLARE Offset INT DEFAULT 0;

    -- LoginId filter
    IF p_LoginIds IS NOT NULL AND LENGTH(TRIM(p_LoginIds)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(LoginId, ""', REPLACE(p_LoginIds, '""', '\""'), '"") > 0');
    END IF;

    -- Symbol filter
    IF p_Symbols IS NOT NULL AND LENGTH(TRIM(p_Symbols)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(Symbol, ""', REPLACE(p_Symbols, '""', '\""'), '"") > 0');
    END IF;

    -- Date range filter
    IF p_FromDate IS NOT NULL AND p_ToDate IS NOT NULL THEN
        SET where_clause = CONCAT(where_clause, ' AND Time BETWEEN ""', p_FromDate, '"" AND ""', p_ToDate, '""');
    END IF;

    -- Get total grouped count
    SET @count_sql = CONCAT('SELECT COUNT(*) AS TotalCount FROM (SELECT 1 FROM Deals', where_clause, ' GROUP BY Symbol, LoginId) AS sub');
    PREPARE count_stmt FROM @count_sql;
    EXECUTE count_stmt;
    DEALLOCATE PREPARE count_stmt;

    -- Get grouped and aggregated data with pagination
    SET @sql = CONCAT(
        'SELECT ',
            'd.Symbol, ',
            'd.LoginId, ',
            'u.Name, ',
            'u.LastName, ',
            'u.Email, ',
            'u.Phone, ',
            'u.Group, ',
			'COUNT(*) AS TotalCount, ',
            'MIN(d.Time) AS FirstTrade, ',
            'MAX(d.Time) AS LastTrade, ',
            'ROUND(SUM(d.Profit), 2) AS Profit, ',
            'ROUND(SUM(d.Swap), 2) AS Swap ',
        'FROM Deals d ',
        'LEFT JOIN Users u ON u.LoginId = d.LoginId ',
        where_clause,
        ' GROUP BY d.Symbol, d.LoginId '
    );

    -- Pagination
    IF p_Page IS NOT NULL AND p_Limit IS NOT NULL AND p_Page > 0 AND p_Limit > 0 THEN
        SET Offset = (p_Page - 1) * p_Limit;
        SET @sql = CONCAT(@sql, ' LIMIT ', Offset, ', ', p_Limit);
    END IF;

    PREPARE data_stmt FROM @sql;
    EXECUTE data_stmt;
    DEALLOCATE PREPARE data_stmt;

END;";
    public const string GetTradeSummaryData = @"DROP PROCEDURE IF EXISTS GetTradeSummaryData;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GetTradeSummaryData`(
    IN p_LoginIds TEXT,
    IN p_FromDate DATETIME,
    IN p_ToDate DATETIME
)
BEGIN
    DECLARE where_clause TEXT DEFAULT ' WHERE (d.Type = 1 OR d.Type = 2) ';
    DECLARE Offset INT DEFAULT 0;

    -- LoginId filter
    IF p_LoginIds IS NOT NULL AND LENGTH(TRIM(p_LoginIds)) > 0 THEN
        SET where_clause = CONCAT(where_clause, ' AND FIND_IN_SET(d.LoginId, ""', REPLACE(p_LoginIds, '""', '\""'), '"") > 0');
    END IF;

    -- Date range filter
    IF p_FromDate IS NOT NULL AND p_ToDate IS NOT NULL THEN
        SET where_clause = CONCAT(where_clause, ' AND d.Time BETWEEN ""', p_FromDate, '"" AND ""', p_ToDate, '""');
    END IF;

    -- Get total grouped count
    SET @count_sql = CONCAT(
        'SELECT COUNT(*) AS TotalCount FROM (',
            'SELECT 1 FROM Deals d ',
            where_clause,
            ' GROUP BY d.LoginId',
        ') AS sub'
    );
    PREPARE count_stmt FROM @count_sql;
    EXECUTE count_stmt;
    DEALLOCATE PREPARE count_stmt;

    -- Get grouped and aggregated data
    SET @sql = CONCAT(
        'SELECT ',
            'd.LoginId, ',
            'COUNT(*) AS TotalCount, ',
            'ROUND(SUM(d.Profit), 2) AS Profit, ',
            'IFNULL(u.Balance, 0) AS Balance, ',
            'ROUND(SUM(d.Profit) + IFNULL(u.Balance, 0), 2) AS AUM ',
        'FROM Deals d ',
        'LEFT JOIN users u ON u.LoginId = d.LoginId ',
        where_clause,
        ' GROUP BY d.LoginId'
    );

    PREPARE data_stmt FROM @sql;
    EXECUTE data_stmt;
    DEALLOCATE PREPARE data_stmt;

END;";
}
