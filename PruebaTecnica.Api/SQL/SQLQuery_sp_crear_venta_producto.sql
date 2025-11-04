SET NOCOUNT ON;
GO

CREATE OR ALTER PROCEDURE negocio.sp_crear_venta_producto
    @fk_usuario INT,
    @listado_articulos negocio.tp_listado_detalle_compra READONLY,
    @resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @total_compra DECIMAL(18,2) = 0;
    DECLARE @cantidad_total_art DECIMAL(18,2) = 0;
    DECLARE @id INT = 0;

    BEGIN TRY

        SELECT 
            @total_compra = ISNULL(SUM(precio_unitario * cantidad_producto),0),
            @cantidad_total_art = ISNULL(SUM(cantidad_producto),0)
        FROM @listado_articulos;

        ;WITH listado_dist AS (
            SELECT DISTINCT fk_producto FROM @listado_articulos
        )
        SELECT ld.fk_producto
        INTO #missing_products
        FROM listado_dist ld
        LEFT JOIN negocio.productos p WITH (NOLOCK)
            ON p.id_producto = ld.fk_producto
        WHERE p.id_producto IS NULL;

        IF EXISTS (SELECT 1 FROM #missing_products)
        BEGIN
            SET @resultado = JSON_OBJECT('data': JSON_OBJECT('code': 0,'id_compra': @id,'msg':N'Existen productos no encontrados'),'errors': NULL);
            RETURN;
        END

        --DROP TABLE #missing_products;

        ;WITH req AS (
            SELECT fk_producto, SUM(cantidad_producto) AS cantidad_solicitada
            FROM @listado_articulos
            GROUP BY fk_producto
        )
        SELECT r.fk_producto, r.cantidad_solicitada, p.existencias
        INTO #insuficientes
        FROM req r
        INNER JOIN negocio.productos p WITH (UPDLOCK, HOLDLOCK) -- bloqueamos filas que vamos a actualizar
            ON p.id_producto = r.fk_producto
        WHERE p.existencias < r.cantidad_solicitada;

        IF EXISTS (SELECT 1 FROM #insuficientes)
        BEGIN
            SET @resultado = JSON_OBJECT('data': JSON_OBJECT('code': 0,'id_compra': @id,'msg':N'Existencias insuficientes'),'errors': NULL);
            DROP TABLE #insuficientes;
            RETURN;
        END

        --DROP TABLE #insuficientes;

        -- -------- SI TODO OK, HACER TRANSACCIÓN (insert compras, detalle, update existencias) ----------
        BEGIN TRAN;
            -- insert al maestro
            INSERT INTO negocio.compras (fk_usuario, total_compra, catidad_total)
            VALUES (@fk_usuario, @total_compra, @cantidad_total_art);

            SET @id = CAST(SCOPE_IDENTITY() AS INT);

            -- insert al detalle
            INSERT INTO negocio.detalle_compras (fk_compra, fk_producto, precio_unitario, cantidad_producto, total_linea)
            SELECT @id, fk_producto, precio_unitario, cantidad_producto, (precio_unitario * cantidad_producto)
            FROM @listado_articulos;

            -- actualizar existencias: restar la suma de cantidades por producto
            ;WITH agregado AS (
                SELECT fk_producto, SUM(cantidad_producto) AS total_solicitado
                FROM @listado_articulos
                GROUP BY fk_producto
            )
            UPDATE negocio.productos
            SET existencias = existencias - a.total_solicitado
            FROM negocio.productos p WITH (ROWLOCK)
            INNER JOIN agregado a ON p.id_producto = a.fk_producto;

        COMMIT TRAN;

        SET @resultado = JSON_OBJECT('data': JSON_OBJECT('code': 0,'id_compra': @id,'msg':N'Exito'), 'errors': NULL);

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRAN;

		SET @resultado = JSON_OBJECT(
                    'data': JSON_OBJECT(
                        'code': 0,
                        'id_compra': @id,
                        'msg': NULL),
                    'errors': JSON_OBJECT(
                        'error_line': ERROR_LINE(),
                        'error_message': ERROR_MESSAGE(),
                        'error_number': ERROR_NUMBER(),
                        'error_procedure': ERROR_PROCEDURE(),
                        'error_severity': ERROR_SEVERITY(),
                        'error_state': ERROR_STATE())
                );
    END CATCH
END;
GO

DELETE FROM negocio.productos WHERE id_producto = 6;

SELECT * FROM negocio.compras
SELECT * FROM negocio.detalle_compras
SELECT * FROM negocio.productos;
go

DECLARE @EmpTable negocio.tp_listado_detalle_compra;
insert into @EmpTable(fk_producto,precio_unitario,cantidad_producto) VALUES (3,86.99,2),(4,59.99,1);
DECLARE @result NVARCHAR(MAX);
EXEC negocio.sp_crear_venta_producto @fk_usuario = 6, @listado_articulos = @EmpTable, @resultado = @result OUTPUT;
SELECT @result;
GO