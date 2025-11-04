-- DROP DATABASE evaluacion_practica;
-- USE master;
-- SELECT * FROM sys.schemas;
-- DROP PROCEDURE seguridad.mostrar_roles;
-- DROP PROCEDURE seguridad.actualizar_rol;
-- DROP PROCEDURE seguridad.crear_rol;

CREATE TYPE negocio.tp_listado_detalle_compra AS TABLE (
    fk_producto INT,
    precio_unitario DECIMAL(18,2),
    cantidad_producto INT
);
GO

CREATE DATABASE evaluacion_practica;
GO

USE evaluacion_practica;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'seguridad')
	EXEC('CREATE SCHEMA seguridad');
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'negocio')
	EXEC('CREATE SCHEMA negocio');
GO

CREATE TABLE seguridad.usuarios (
	id_usuario INT IDENTITY(1,1) CONSTRAINT pk_usuarios PRIMARY KEY,
	nombre_usuario VARCHAR(30) NOT NULL CONSTRAINT uq_usuarios_nombre_usuario UNIQUE,
	contrasenia NVARCHAR(300) NOT NULL,
	contrasenia_hash_salt NVARCHAR(300) NOT NULL,
	estado BIT CONSTRAINT df_usuarios_estado DEFAULT 1,
	fecha_creacion DATETIME2 CONSTRAINT df_usuarios_fecha_creacion DEFAULT SYSDATETIME(),
	fecha_modificacion DATETIME2
);
GO

CREATE TABLE seguridad.personas (
	id_persona INT IDENTITY(1,1) CONSTRAINT pk_personas PRIMARY KEY,
	fk_usuario INT NOT NULL,
	nombres NVARCHAR(250) NOT NULL,
	apellidos NVARCHAR(250) NOT NULL,
	edad INT NULL,
	movil NVARCHAR(15) NULL,
	telefono NVARCHAR(15) NULL,
	correo_electronico NVARCHAR(300) NULL,
	fecha_nacimiento DATE NULL,
	fecha_creacion DATETIME2 CONSTRAINT df_personas_fecha_creacion DEFAULT SYSDATETIME(),
	fecha_modificacion DATETIME2,
	CONSTRAINT fk_personas_to_usuarios FOREIGN KEY (fk_usuario) REFERENCES seguridad.usuarios(id_usuario),
	CONSTRAINT uq_personas_fk_usuario UNIQUE (fk_usuario)
);
GO

CREATE TABLE seguridad.roles (
	id_rol INT IDENTITY(1,1) CONSTRAINT pk_roles PRIMARY KEY,
	nombre_rol NVARCHAR(50) NOT NULL CONSTRAINT uq_roles_nombre_rol UNIQUE,
	descripcion_rol NVARCHAR(50) NULL,
	estado BIT CONSTRAINT df_roles_estado DEFAULT 1,
	fecha_creacion DATETIME2 CONSTRAINT df_roles_fecha_creacion DEFAULT SYSDATETIME(),
	fecha_modificacion DATETIME2
);
GO

CREATE TABLE seguridad.usuario_rol (
	id_usuario_rol INT IDENTITY(1,1) CONSTRAINT pk_usuario_rol PRIMARY KEY,
	fk_usuario INT NOT NULL,
	fk_rol INT NOT NULL,
	estado BIT CONSTRAINT df_usuario_rol_estado DEFAULT 1,
	fecha_creacion DATETIME2 CONSTRAINT df_usuario_rol_fecha_creacion DEFAULT SYSDATETIME(),
	fecha_modificacion DATETIME2,
	CONSTRAINT fk_usuario_rol_to_usuarios FOREIGN KEY (fk_usuario) REFERENCES seguridad.usuarios(id_usuario),
	CONSTRAINT fk_usuario_rol_to_roles FOREIGN KEY (fk_rol) REFERENCES seguridad.roles(id_rol)
);
GO

CREATE TABLE negocio.productos (
	id_producto INT IDENTITY(1,1) CONSTRAINT pk_productos PRIMARY KEY,
	nombre_producto NVARCHAR(200) NOT NULL,
	descripcion_producto NVARCHAR(200) NOT NULL,
	precio_unitario DECIMAL(18,2) NOT NULL,
	existencias INT NOT NULL,
	estado BIT CONSTRAINT df_productos_estado DEFAULT 1,
	fecha_creacion DATETIME2 CONSTRAINT df_productos_fecha_creacion DEFAULT SYSDATETIME(),
	fk_usuario_creacion INT NOT NULL,
	CONSTRAINT fk_productos_to_usuarios FOREIGN KEY (fk_usuario_creacion) REFERENCES seguridad.usuarios(id_usuario)
);
GO

CREATE TABLE negocio.historico_precio_productos (
	id_historico_producto INT IDENTITY(1,1) CONSTRAINT pk_historico_precio_productos PRIMARY KEY,
	fk_producto INT NOT NULL,
	fk_usuario INT NOT NULL,
	precio_nuevo DECIMAL(18,2),
	precio_antiguo DECIMAL(18,2),
	fecha_creacion DATETIME2 CONSTRAINT df_historico_precio_productos_fecha_creacion DEFAULT SYSDATETIME(),
	CONSTRAINT fk_historico_precio_productos_to_producto FOREIGN KEY (fk_producto) REFERENCES negocio.productos(id_producto),
	CONSTRAINT fk_historico_precio_productos_to_usuarios FOREIGN KEY (fk_usuario) REFERENCES seguridad.usuarios(id_usuario)
);
GO

CREATE TABLE negocio.compras (
	id_compra INT IDENTITY(1,1) CONSTRAINT pk_compras PRIMARY KEY,
	fk_usuario INT NOT NULL,
	total_compra DECIMAL(18,2) NOT NULL,
	catidad_total DECIMAL(18,2) NULL,
	estado BIT CONSTRAINT df_compras_estado DEFAULT 1,
	fecha_creacion DATETIME2 CONSTRAINT df_compras_fecha_creacion DEFAULT SYSDATETIME(),
	CONSTRAINT fk_compras_to_usuarios FOREIGN KEY (fk_usuario) REFERENCES seguridad.usuarios(id_usuario)
);
GO

CREATE TABLE negocio.detalle_compras (
	id_detalle_compra INT IDENTITY(1,1) CONSTRAINT pk_detalle_compras PRIMARY KEY,
	fk_compra INT NOT NULL,
	fk_producto INT NOT NULL,
	precio_unitario DECIMAL(18,2) NOT NULL,
	cantidad_producto INT NOT NULL,
	total_linea DECIMAL(18,2) NOT NULL,
	estado BIT CONSTRAINT df_detalle_compras_estado DEFAULT 1,
	fecha_creacion DATETIME2 CONSTRAINT df_detalle_compras_fecha_creacion DEFAULT SYSDATETIME(),
	CONSTRAINT fk_detalle_compras_to_compras FOREIGN KEY (fk_compra) REFERENCES negocio.compras(id_compra),
	CONSTRAINT fk_detalle_compras_to_productos FOREIGN KEY (fk_producto) REFERENCES negocio.productos(id_producto)
);
GO

SELECT * FROM seguridad.roles;
GO

ALTER PROCEDURE seguridad.roles_crear
    @nombre_rol NVARCHAR(30),
    @descripcion_rol NVARCHAR(300),
	@resultado INT OUTPUT,
	@mensaje NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
	BEGIN TRY

		INSERT INTO seguridad.roles(nombre_rol, descripcion_rol) VALUES (@nombre_rol, @descripcion_rol);

		SET @resultado = SCOPE_IDENTITY();
		SET @mensaje = 'Exito';
	END TRY
	BEGIN CATCH
		SET @resultado = ERROR_NUMBER();
		SET @mensaje = ERROR_MESSAGE();
	END CATCH;
END;
GO

DECLARE @Id INT, @msg NVARCHAR(500);
EXEC seguridad.roles_crear @nombre_rol = 'Administrador', @descripcion_rol = 'Rol para usuarios administradores', @resultado = @Id OUTPUT, @mensaje = @msg OUTPUT;
SELECT @Id AS CODE, @msg AS MESSAGE;
GO

CREATE PROCEDURE seguridad.roles_actualizar
	@id_rol INT,
    @nombre_rol NVARCHAR(30),
    @descripcion_rol NVARCHAR(300),
	@resultado INT OUTPUT,
	@mensaje NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
	BEGIN TRY

		UPDATE 
			seguridad.roles 
		SET 
			nombre_rol = @nombre_rol, 
			descripcion_rol = @descripcion_rol,
			fecha_modificacion = SYSDATETIME()
		WHERE id_rol = @id_rol;

		SET @resultado = @@ROWCOUNT;
		SET @mensaje = 'Exito';
	END TRY
	BEGIN CATCH
		SET @resultado = ERROR_NUMBER();
		SET @mensaje = ERROR_MESSAGE();
	END CATCH;
END;
GO

DECLARE @Id INT, @msg NVARCHAR(500);
EXEC seguridad.roles_actualizar @id_rol = 2, @nombre_rol = 'Administrador', @descripcion_rol = 'Rol para usuarios administradores', @resultado = @Id OUTPUT, @mensaje = @msg OUTPUT;
SELECT @Id AS CODE, @msg AS MESSAGE;
GO

ALTER PROCEDURE seguridad.roles_mostrar
AS
BEGIN
    SET NOCOUNT ON;
	SELECT id_rol, nombre_rol, descripcion_rol, estado, fecha_creacion, fecha_modificacion FROM seguridad.roles;
END;
GO

EXEC seguridad.roles_mostrar;
GO
