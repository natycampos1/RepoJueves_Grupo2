USE master;
GO

CREATE DATABASE RFBakery;
GO

USE RFBakery;
GO

-- =============================================
-- TABLAS CATÁLOGO (sin dependencias)
-- =============================================

CREATE TABLE ESTADO_TB (
    ID_ESTADO_PK        INT             NOT NULL IDENTITY(1,1),
    DESCRIPCION         VARCHAR(100)    NOT NULL,

    CONSTRAINT PK_ESTADO PRIMARY KEY (ID_ESTADO_PK),
    CONSTRAINT UQ_ESTADO_DESCRIPCION UNIQUE (DESCRIPCION)
);
GO

CREATE TABLE ROL_TB (
    ID_ROL_PK           INT             NOT NULL IDENTITY(1,1),
    DESCRIPCION         VARCHAR(50)     NOT NULL,
    ID_ESTADO_FK        INT             NOT NULL,

    CONSTRAINT PK_ROL PRIMARY KEY (ID_ROL_PK),
    CONSTRAINT UQ_ROL_DESCRIPCION UNIQUE (DESCRIPCION),
    CONSTRAINT FK_ROL_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);
GO

CREATE TABLE TIPO_IDENTIFICACION_TB (
    ID_TIPO_IDENTIFICACION_PK   INT             NOT NULL IDENTITY(1,1),
    DESCRIPCION                 VARCHAR(50)     NOT NULL,
    ID_ESTADO_FK                INT             NOT NULL,

    CONSTRAINT PK_TIPO_IDENTIFICACION PRIMARY KEY (ID_TIPO_IDENTIFICACION_PK),
    CONSTRAINT UQ_TIPO_IDENTIFICACION_DESCRIPCION UNIQUE (DESCRIPCION),
    CONSTRAINT FK_TIPO_IDENTIFICACION_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);
GO

-- =============================================
-- TABLAS PRINCIPALES
-- =============================================


CREATE TABLE PERSONA_TB (
    IDENTIFICACION_PK           VARCHAR(20)     NOT NULL,
    ID_TIPO_IDENTIFICACION_FK   INT             NOT NULL,
    NOMBRE_COMPLETO             VARCHAR(100)    NOT NULL,
    PRIMER_APELLIDO             VARCHAR(100)    NOT NULL,
    SEGUNDO_APELLIDO            VARCHAR(100)    NULL,
    GENERO                      VARCHAR(10)     NULL,
    DIRECCION                   VARCHAR(250)    NULL,
    NACIONALIDAD                VARCHAR(50)     NULL,
    FECHA_REGISTRO              DATE            NOT NULL,
    ID_ESTADO_FK                INT             NOT NULL,

    CONSTRAINT PK_PERSONA PRIMARY KEY (IDENTIFICACION_PK),
    CONSTRAINT FK_PERSONA_TIPO_IDENTIFICACION FOREIGN KEY (ID_TIPO_IDENTIFICACION_FK)
        REFERENCES TIPO_IDENTIFICACION_TB(ID_TIPO_IDENTIFICACION_PK),
    CONSTRAINT FK_PERSONA_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);
GO

CREATE TABLE TELEFONO_TB (
    IDENTIFICACION_FK   VARCHAR(20)     NOT NULL,
    NUM_TELEFONO        VARCHAR(20)     NOT NULL,
    ID_ESTADO_FK        INT             NOT NULL,

    CONSTRAINT PK_TELEFONO PRIMARY KEY (IDENTIFICACION_FK, NUM_TELEFONO),
    CONSTRAINT FK_TELEFONO_PERSONA FOREIGN KEY (IDENTIFICACION_FK)
        REFERENCES PERSONA_TB(IDENTIFICACION_PK),
    CONSTRAINT FK_TELEFONO_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);
GO


CREATE TABLE USUARIO_TB (
    ID_USUARIO_PK       INT             NOT NULL IDENTITY(1,1),
    IDENTIFICACION_FK   VARCHAR(20)     NOT NULL,
    ID_ROL_FK           INT             NOT NULL,
    EMAIL               VARCHAR(100)    NOT NULL,
    CONTRASENA          VARCHAR(250)    NOT NULL,
    ID_ESTADO_FK        INT             NOT NULL,
    INDICADOR_CONTRASENA_TEMP BIT       NOT NULL DEFAULT 0,

    CONSTRAINT PK_USUARIO PRIMARY KEY (ID_USUARIO_PK),
    CONSTRAINT UQ_USUARIO_EMAIL UNIQUE (EMAIL),
    CONSTRAINT UQ_USUARIO_IDENTIFICACION UNIQUE (IDENTIFICACION_FK),
    CONSTRAINT FK_USUARIO_PERSONA FOREIGN KEY (IDENTIFICACION_FK)
        REFERENCES PERSONA_TB(IDENTIFICACION_PK),
    CONSTRAINT FK_USUARIO_ROL FOREIGN KEY (ID_ROL_FK)
        REFERENCES ROL_TB(ID_ROL_PK),
    CONSTRAINT FK_USUARIO_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);

CREATE TABLE [dbo].[tbError](
	[Consecutivo] [int] IDENTITY(1,1) NOT NULL,
	[Mensaje] [varchar](max) NOT NULL,
	[Lugar] [varchar](50) NOT NULL,
	[FechaHora] [datetime] NOT NULL,
	[ConsecutivoUsuario] [int] NOT NULL,
 CONSTRAINT [PK_tbError] PRIMARY KEY CLUSTERED 
(
	[Consecutivo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE CATEGORIA_PRODUCTO_TB (
    ID_CATEGORIA_PK     INT             NOT NULL IDENTITY(1,1),
    DESCRIPCION         VARCHAR(50)     NOT NULL,
    ID_ESTADO_FK        INT             NOT NULL,

    CONSTRAINT PK_CATEGORIA_PRODUCTO PRIMARY KEY (ID_CATEGORIA_PK),
    CONSTRAINT UQ_CATEGORIA_DESCRIPCION UNIQUE (DESCRIPCION),
    CONSTRAINT FK_CATEGORIA_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);
GO

CREATE TABLE PRODUCTO_TB (
    ID_PRODUCTO_PK      INT             NOT NULL IDENTITY(1,1),
    ID_CATEGORIA_FK     INT             NOT NULL,
    NOMBRE              VARCHAR(100)    NOT NULL,
    DESCRIPCION         VARCHAR(250)    NOT NULL,
    PRECIO              DECIMAL(10,2)   NOT NULL,
    IMAGEN              VARCHAR(200)    NULL,
    ID_ESTADO_FK        INT             NOT NULL,
    STOCK               INT NOT NULL DEFAULT 0,

    CONSTRAINT PK_PRODUCTO PRIMARY KEY (ID_PRODUCTO_PK),
    CONSTRAINT FK_PRODUCTO_CATEGORIA FOREIGN KEY (ID_CATEGORIA_FK)
        REFERENCES CATEGORIA_PRODUCTO_TB(ID_CATEGORIA_PK),
    CONSTRAINT FK_PRODUCTO_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);

CREATE TABLE MENSAJE_CONTACTO_TB (
    ID_MENSAJE_PK       INT             NOT NULL IDENTITY(1,1),
    NOMBRE              VARCHAR(100)    NOT NULL,
    EMAIL               VARCHAR(100)    NOT NULL,
    ASUNTO              VARCHAR(150)    NOT NULL,
    MENSAJE             VARCHAR(1000)   NOT NULL,
    FECHA_ENVIO         DATETIME        NOT NULL,
    ID_ESTADO_FK        INT             NOT NULL,

    CONSTRAINT PK_MENSAJE_CONTACTO PRIMARY KEY (ID_MENSAJE_PK),
    CONSTRAINT FK_MENSAJE_CONTACTO_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);
GO

CREATE TABLE TIPO_ENTREGA_TB (
    ID_TIPO_ENTREGA_PK  INT             NOT NULL IDENTITY(1,1),
    DESCRIPCION         VARCHAR(50)     NOT NULL,
    ID_ESTADO_FK        INT             NOT NULL,

    CONSTRAINT PK_TIPO_ENTREGA PRIMARY KEY (ID_TIPO_ENTREGA_PK),
    CONSTRAINT FK_TIPO_ENTREGA_ESTADO FOREIGN KEY (ID_ESTADO_FK)
        REFERENCES ESTADO_TB(ID_ESTADO_PK)
);
GO

CREATE TABLE ESTADO_PEDIDO_TB (
    ID_ESTADO_PEDIDO_PK     INT             NOT NULL IDENTITY(1,1),
    DESCRIPCION             VARCHAR(50)     NOT NULL,

    CONSTRAINT PK_ESTADO_PEDIDO PRIMARY KEY (ID_ESTADO_PEDIDO_PK)
);
GO

CREATE TABLE PEDIDO_TB (
    ID_PEDIDO_PK            INT             NOT NULL IDENTITY(1,1),
    ID_USUARIO_FK           INT             NOT NULL,
    FECHA_PEDIDO            DATETIME        NOT NULL,
    ID_TIPO_ENTREGA_FK      INT             NOT NULL,
    DIRECCION_ENTREGA       VARCHAR(250)    NULL,
    ID_ESTADO_PEDIDO_FK     INT             NOT NULL,
    TOTAL                   DECIMAL(10,2)   NOT NULL,

    CONSTRAINT PK_PEDIDO PRIMARY KEY (ID_PEDIDO_PK),
    CONSTRAINT FK_PEDIDO_USUARIO FOREIGN KEY (ID_USUARIO_FK)
        REFERENCES USUARIO_TB(ID_USUARIO_PK),
    CONSTRAINT FK_PEDIDO_TIPO_ENTREGA FOREIGN KEY (ID_TIPO_ENTREGA_FK)
        REFERENCES TIPO_ENTREGA_TB(ID_TIPO_ENTREGA_PK),
    CONSTRAINT FK_PEDIDO_ESTADO_PEDIDO FOREIGN KEY (ID_ESTADO_PEDIDO_FK)
        REFERENCES ESTADO_PEDIDO_TB(ID_ESTADO_PEDIDO_PK)
);
GO

--CREACIÓN DE TABLA DE DETALLE DE PEDIDOS
CREATE TABLE DETALLE_PEDIDO_TB (
    ID_DETALLE_PEDIDO_PK    INT             NOT NULL IDENTITY(1,1),
    ID_PEDIDO_FK            INT             NOT NULL,
    ID_PRODUCTO_FK          INT             NOT NULL,
    CANTIDAD                INT             NOT NULL,
    PRECIO_UNITARIO         DECIMAL(10,2)   NOT NULL,
    SUBTOTAL                DECIMAL(10,2)   NOT NULL,

    CONSTRAINT PK_DETALLE_PEDIDO PRIMARY KEY (ID_DETALLE_PEDIDO_PK),
    CONSTRAINT FK_DETALLE_PEDIDO_PEDIDO FOREIGN KEY (ID_PEDIDO_FK)
        REFERENCES PEDIDO_TB(ID_PEDIDO_PK),
    CONSTRAINT FK_DETALLE_PEDIDO_PRODUCTO FOREIGN KEY (ID_PRODUCTO_FK)
        REFERENCES PRODUCTO_TB(ID_PRODUCTO_PK)
);
GO


-- =============================================
-- DATOS INICIALES (catálogos base)
-- =============================================
-- Estados
INSERT INTO ESTADO_TB (DESCRIPCION) VALUES ('Activo');
INSERT INTO ESTADO_TB (DESCRIPCION) VALUES ('Inactivo');
GO

-- Roles
INSERT INTO ROL_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Administrador', 1);
INSERT INTO ROL_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Cliente', 1);
GO

-- Tipos de identificación
INSERT INTO TIPO_IDENTIFICACION_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Cédula Nacional', 1);
INSERT INTO TIPO_IDENTIFICACION_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Pasaporte', 1);
INSERT INTO TIPO_IDENTIFICACION_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('DIMEX', 1);
GO

--DATOS INICIALES: CATEGORÍAS
INSERT INTO CATEGORIA_PRODUCTO_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Cumpleaños', 1);   -- 1
INSERT INTO CATEGORIA_PRODUCTO_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Bodas', 1);         -- 2
INSERT INTO CATEGORIA_PRODUCTO_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Cupcakes', 1);      -- 3
INSERT INTO CATEGORIA_PRODUCTO_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Baby Shower', 1);   -- 4

--DATOS INICIALES: PRODUCTOS DE EJEMPLO
-- Cumpleaños (1)
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (1, 'Torta de Vainilla y Fresa', 'Bizcocho de vainilla relleno de crema y fresas frescas', 25000, 'cumpleanos-1.jpg', 1, 15);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (1, 'Torta de Chocolate Intenso', 'Bizcocho húmedo de chocolate con ganache', 27000, 'cumpleanos-2.jpg', 1, 8);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (1, 'Torta Arcoíris Infantil', 'Colorida torta ideal para fiestas infantiles', 30000, 'cumpleanos-3.jpg', 1, 0);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (1, 'Torta de Oreo', 'Bizcocho de chocolate con crema y galletas Oreo trituradas', 28000, 'cumpleanos-4.jpg', 1, 12);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (1, 'Torta Tres Leches', 'Clásica torta tres leches, suave y jugosa', 24000, 'cumpleanos-5.jpg', 1, 5);

-- Bodas (2)
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (2, 'Torta Clásica de 3 Pisos', 'Elegante torta de tres pisos decorada con flores comestibles', 85000, 'bodas-1.jpg', 1, 3);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (2, 'Torta Rústica Naked Cake', 'Estilo naked cake con frutas frescas y flores', 70000, 'bodas-2.jpg', 1, 0);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (2, 'Torta Elegante Blanca', 'Torta de fondant blanco con detalles dorados, ideal para bodas formales', 90000, 'bodas-3.jpg', 1, 2);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (2, 'Torta de 2 Pisos con Perlas', 'Torta de dos pisos decorada con perlas comestibles y encaje de azúcar', 65000, 'bodas-4.jpg', 1, 4);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (2, 'Mesa de Postres para Boda', 'Selección de mini postres variados para mesa de dulces', 55000, 'bodas-5.jpg', 1, 6);

-- Cupcakes (3)
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (3, 'Cupcakes de Vainilla', 'Docena de cupcakes de vainilla con buttercream', 12000, 'cupcakes-1.jpg', 1, 25);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (3, 'Cupcakes Red Velvet', 'Docena de cupcakes red velvet con frosting de queso crema', 14000, 'cupcakes-2.jpg', 1, 0);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (3, 'Cupcakes de Chocolate', 'Docena de cupcakes de chocolate con ganache', 13000, 'cupcakes-3.jpg', 1, 18);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (3, 'Cupcakes de Limón', 'Docena de cupcakes de limón con buttercream cítrico', 13000, 'cupcakes-4.jpg', 1, 10);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (3, 'Cupcakes Surtidos', 'Docena de cupcakes surtidos en varios sabores', 14000, 'cupcakes-5.jpg', 1, 20);

-- Baby Shower (4)
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (4, 'Torta Baby Shower Celeste', 'Torta decorada en tonos celestes con detalles de bebé', 32000, 'babyshower-1.jpg', 1, 7);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (4, 'Torta Baby Shower Rosada', 'Torta decorada en tonos rosados con detalles tiernos', 32000, 'babyshower-2.jpg', 1, 9);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (4, 'Torta Baby Shower Neutra', 'Torta en tonos amarillos y verdes, ideal cuando no se sabe el género', 32000, 'babyshower-3.jpg', 1, 0);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (4, 'Cupcakes Baby Shower', 'Docena de cupcakes temáticos para baby shower', 15000, 'babyshower-4.jpg', 1, 14);
INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, ID_ESTADO_FK, STOCK) VALUES (4, 'Torta con Cigüeña', 'Torta decorada con la clásica cigüeña, para anunciar la llegada del bebé', 34000, 'babyshower-5.jpg', 1, 5);
GO


-- DATOS INICIALES: TIPOS DE ENTREGA
INSERT INTO TIPO_ENTREGA_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Delivery', 1);
INSERT INTO TIPO_ENTREGA_TB (DESCRIPCION, ID_ESTADO_FK) VALUES ('Retiro en Tienda', 1);
GO

-- DATOS INICIALES: ESTADOS DE PEDIDO
INSERT INTO ESTADO_PEDIDO_TB (DESCRIPCION) VALUES ('Pendiente');       -- 1
INSERT INTO ESTADO_PEDIDO_TB (DESCRIPCION) VALUES ('En Preparación');  -- 2
INSERT INTO ESTADO_PEDIDO_TB (DESCRIPCION) VALUES ('Listo');           -- 3
INSERT INTO ESTADO_PEDIDO_TB (DESCRIPCION) VALUES ('Entregado');       -- 4
INSERT INTO ESTADO_PEDIDO_TB (DESCRIPCION) VALUES ('Cancelado');       -- 5
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS
-- =============================================

-- Consulta de tipos de identificacion
CREATE PROCEDURE SP_ConsultarTiposIdentificacion
AS
BEGIN
    SELECT
        ID_TIPO_IDENTIFICACION_PK   AS IdTipoIdentificacion,
        DESCRIPCION                 AS Descripcion
    FROM TIPO_IDENTIFICACION_TB
    WHERE ID_ESTADO_FK = 1
END
GO

--Registro de usuario
CREATE PROCEDURE SP_RegistrarUsuario
    @Identificacion         VARCHAR(20),
    @IdTipoIdentificacion   INT,
    @NombreCompleto         VARCHAR(100),
    @PrimerApellido         VARCHAR(100),
    @SegundoApellido        VARCHAR(100),
    @Genero                 VARCHAR(10),
    @Direccion              VARCHAR(250),
    @Nacionalidad           VARCHAR(50),
    @NumTelefono            VARCHAR(20),
    @Email                  VARCHAR(100),
    @Contrasena             VARCHAR(250)
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM PERSONA_TB WHERE IDENTIFICACION_PK = @Identificacion)
    AND NOT EXISTS (SELECT 1 FROM USUARIO_TB WHERE EMAIL = @Email)
    BEGIN
        INSERT INTO PERSONA_TB (
            IDENTIFICACION_PK,
            ID_TIPO_IDENTIFICACION_FK,
            NOMBRE_COMPLETO,
            PRIMER_APELLIDO,
            SEGUNDO_APELLIDO,
            GENERO,
            DIRECCION,
            NACIONALIDAD,
            FECHA_REGISTRO,
            ID_ESTADO_FK
        )
        VALUES (
            @Identificacion,
            @IdTipoIdentificacion,
            @NombreCompleto,
            @PrimerApellido,
            @SegundoApellido,
            @Genero,
            @Direccion,
            @Nacionalidad,
            GETDATE(),
            1
        );

        INSERT INTO TELEFONO_TB (
            IDENTIFICACION_FK,
            NUM_TELEFONO,
            ID_ESTADO_FK
        )
        VALUES (
            @Identificacion,
            @NumTelefono,
            1
        );

        INSERT INTO USUARIO_TB (
            IDENTIFICACION_FK,
            ID_ROL_FK,
            EMAIL,
            CONTRASENA,
            ID_ESTADO_FK
        )
        VALUES (
            @Identificacion,
            2,
            @Email,
            @Contrasena,
            1
        );
    END

END
GO

--Procedimiento almacenado para iniciar sesión de usuario (consulto usuario por email y envio los datos de interes para la variable de sesion)
CREATE PROCEDURE SP_IniciarSesion
    @Email VARCHAR(100)
AS
BEGIN
    SELECT
        U.ID_USUARIO_PK                 AS IdUsuario,
        P.IDENTIFICACION_PK             AS Identificacion,
        P.ID_TIPO_IDENTIFICACION_FK     AS IdTipoIdentificacion,
        P.NOMBRE_COMPLETO               AS NombreCompleto,
        P.PRIMER_APELLIDO               AS PrimerApellido,
        P.SEGUNDO_APELLIDO              AS SegundoApellido,
        P.GENERO                        AS Genero,
        P.DIRECCION                     AS Direccion,
        P.NACIONALIDAD                  AS Nacionalidad,
        P.FECHA_REGISTRO                AS FechaRegistro,
        T.NUM_TELEFONO                  AS NumTelefono,
        U.EMAIL                         AS Email,
        U.CONTRASENA                    AS Contrasena,
        U.ID_ROL_FK                     AS IdRol
    FROM USUARIO_TB U
    INNER JOIN PERSONA_TB P   ON U.IDENTIFICACION_FK  = P.IDENTIFICACION_PK
    INNER JOIN TELEFONO_TB T  ON P.IDENTIFICACION_PK  = T.IDENTIFICACION_FK
    WHERE U.EMAIL = @Email
    AND U.ID_ESTADO_FK = 1
END
GO

CREATE PROCEDURE [dbo].[SP_RegistrarError]
    @Mensaje                varchar(max),
    @Lugar                  varchar(50),
    @FechaHora              datetime,
    @ConsecutivoUsuario     int
AS
BEGIN


    INSERT INTO dbo.tbError
               (Mensaje
               ,Lugar
               ,FechaHora
               ,ConsecutivoUsuario)
         VALUES
               (@Mensaje,@Lugar,@FechaHora,@ConsecutivoUsuario)

END
GO

--Para perfil de usuario
CREATE PROCEDURE SP_ConsultarUsuario
    @Identificacion VARCHAR(20)
AS
BEGIN
    SELECT
        P.IDENTIFICACION_PK     AS Identificacion,
        TI.DESCRIPCION          AS TipoIdentificacion,
        P.NOMBRE_COMPLETO       AS NombreCompleto,
        P.PRIMER_APELLIDO       AS PrimerApellido,
        P.SEGUNDO_APELLIDO      AS SegundoApellido,
        P.GENERO                AS Genero,
        P.DIRECCION             AS Direccion,
        P.NACIONALIDAD          AS Nacionalidad,
        T.NUM_TELEFONO          AS NumTelefono,
        U.EMAIL                 AS Email,
        R.DESCRIPCION           AS Rol
    FROM PERSONA_TB P
    INNER JOIN TIPO_IDENTIFICACION_TB TI ON P.ID_TIPO_IDENTIFICACION_FK = TI.ID_TIPO_IDENTIFICACION_PK
    INNER JOIN TELEFONO_TB T             ON P.IDENTIFICACION_PK         = T.IDENTIFICACION_FK
    INNER JOIN USUARIO_TB U              ON P.IDENTIFICACION_PK         = U.IDENTIFICACION_FK
    INNER JOIN ROL_TB R                  ON U.ID_ROL_FK                 = R.ID_ROL_PK
    WHERE P.IDENTIFICACION_PK = @Identificacion
END
GO

--Para editar usuario
CREATE PROCEDURE SP_ActualizarUsuario
    @Identificacion     VARCHAR(20),
    @NombreCompleto      VARCHAR(100),
    @PrimerApellido      VARCHAR(100),
    @SegundoApellido     VARCHAR(100),
    @Genero              VARCHAR(10),
    @Direccion           VARCHAR(250),
    @Nacionalidad        VARCHAR(50),
    @NumTelefono         VARCHAR(20),
    @Email               VARCHAR(100),
    @NuevaContrasena     VARCHAR(250) = NULL
AS
BEGIN

    -- Actualizar datos de PERSONA_TB
    UPDATE PERSONA_TB
    SET
        NOMBRE_COMPLETO  = @NombreCompleto,
        PRIMER_APELLIDO  = @PrimerApellido,
        SEGUNDO_APELLIDO = @SegundoApellido,
        GENERO           = @Genero,
        DIRECCION        = @Direccion,
        NACIONALIDAD     = @Nacionalidad
    WHERE IDENTIFICACION_PK = @Identificacion;

    -- Actualizar teléfono
    UPDATE TELEFONO_TB
    SET NUM_TELEFONO = @NumTelefono
    WHERE IDENTIFICACION_FK = @Identificacion;

    -- Actualizar email
    UPDATE USUARIO_TB
    SET EMAIL = @Email
    WHERE IDENTIFICACION_FK = @Identificacion;

    -- Solo actualizar contraseña si llega un valor
    IF @NuevaContrasena IS NOT NULL
    BEGIN
        UPDATE USUARIO_TB
        SET CONTRASENA = @NuevaContrasena
        WHERE IDENTIFICACION_FK = @Identificacion;
    END

END
GO 

-- SP para validar que el correo existe (paso 1 de recuperar acceso)
CREATE PROCEDURE SP_ValidarCorreo
    @Email VARCHAR(100)
AS
BEGIN
    SELECT
        U.ID_USUARIO_PK      AS IdUsuario,
        P.IDENTIFICACION_PK  AS Identificacion,
        P.NOMBRE_COMPLETO    AS NombreCompleto,
        U.EMAIL              AS Email
    FROM USUARIO_TB U
    INNER JOIN PERSONA_TB P ON U.IDENTIFICACION_FK = P.IDENTIFICACION_PK
    WHERE U.EMAIL = @Email
    AND U.ID_ESTADO_FK = 1
END
GO

-- SP para actualizar la contraseña (usado tanto en recuperar acceso como en cambio de contraseña normal)
CREATE PROCEDURE SP_ActualizarContrasena
    @IdUsuario                  INT,
    @Contrasena                 VARCHAR(250),
    @IndicadorContrasenaTemp    BIT
AS
BEGIN
    UPDATE USUARIO_TB
    SET CONTRASENA = @Contrasena,
        INDICADOR_CONTRASENA_TEMP = @IndicadorContrasenaTemp
    WHERE ID_USUARIO_PK = @IdUsuario
END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR LAS CATEGORÍAS DE PRODUCTO ACTIVAS
CREATE PROCEDURE SP_ConsultarCategoriasProducto
AS
BEGIN
    SELECT
        ID_CATEGORIA_PK     AS IdCategoria,
        DESCRIPCION         AS Descripcion
    FROM CATEGORIA_PRODUCTO_TB
    WHERE ID_ESTADO_FK = 1
    ORDER BY ID_CATEGORIA_PK
END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR LOS PRODUCTOS DE UNA CATEGORÍA ESPECÍFICA
CREATE PROCEDURE SP_ConsultarProductosPorCategoria
    @IdCategoria INT
AS
BEGIN
    SELECT
        P.ID_PRODUCTO_PK    AS IdProducto,
        P.ID_CATEGORIA_FK   AS IdCategoria,
        P.NOMBRE            AS Nombre,
        P.DESCRIPCION       AS Descripcion,
        P.PRECIO            AS Precio,
        P.IMAGEN            AS Imagen,
        P.STOCK             AS Stock
    FROM PRODUCTO_TB P
    WHERE P.ID_CATEGORIA_FK = @IdCategoria
    AND P.ID_ESTADO_FK = 1
    ORDER BY P.NOMBRE
END
GO

--PROCEDIMIENTO ALMACENADO PARA REGISTRAR UN MENSAJE DE CONTACTO
CREATE PROCEDURE SP_RegistrarMensajeContacto
    @Nombre     VARCHAR(100),
    @Email      VARCHAR(100),
    @Asunto     VARCHAR(150),
    @Mensaje    VARCHAR(1000)
AS
BEGIN

    INSERT INTO MENSAJE_CONTACTO_TB (NOMBRE, EMAIL, ASUNTO, MENSAJE, FECHA_ENVIO, ID_ESTADO_FK)
    VALUES (@Nombre, @Email, @Asunto, @Mensaje, GETDATE(), 1)

END
GO

--PROCEDIMIENTO ALMACENADO PARA INSERTAR UNA CATEGORÍA
CREATE PROCEDURE SP_InsertarCategoria
    @Descripcion VARCHAR(50)
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM CATEGORIA_PRODUCTO_TB WHERE DESCRIPCION = @Descripcion)
    BEGIN
        INSERT INTO CATEGORIA_PRODUCTO_TB (DESCRIPCION, ID_ESTADO_FK)
        VALUES (@Descripcion, 1)
    END

END
GO

--PROCEDIMIENTO ALMACENADO PARA ACTUALIZAR UNA CATEGORÍA
CREATE PROCEDURE SP_ActualizarCategoria
    @IdCategoria    INT,
    @Descripcion    VARCHAR(50)
AS
BEGIN

    UPDATE CATEGORIA_PRODUCTO_TB
    SET DESCRIPCION = @Descripcion
    WHERE ID_CATEGORIA_PK = @IdCategoria

END
GO

--PROCEDIMIENTO ALMACENADO PARA DESACTIVAR UNA CATEGORÍA
CREATE PROCEDURE SP_DesactivarCategoria
    @IdCategoria INT
AS
BEGIN

    UPDATE CATEGORIA_PRODUCTO_TB
    SET ID_ESTADO_FK = 2
    WHERE ID_CATEGORIA_PK = @IdCategoria

END
GO

--PROCEDIMIENTO ALMACENADO PARA INSERTAR UN PRODUCTO
CREATE PROCEDURE SP_InsertarProducto
    @IdCategoria    INT,
    @Nombre         VARCHAR(100),
    @Descripcion    VARCHAR(250),
    @Precio         DECIMAL(10,2),
    @Imagen         VARCHAR(200),
    @Stock          INT
AS
BEGIN

    INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, STOCK, ID_ESTADO_FK)
    VALUES (@IdCategoria, @Nombre, @Descripcion, @Precio, @Imagen, @Stock, 1)

END
GO

--PROCEDIMIENTO ALMACENADO PARA ACTUALIZAR UN PRODUCTO
CREATE PROCEDURE SP_ActualizarProducto
    @IdProducto     INT,
    @IdCategoria    INT,
    @Nombre         VARCHAR(100),
    @Descripcion    VARCHAR(250),
    @Precio         DECIMAL(10,2),
    @Imagen         VARCHAR(200),
    @Stock          INT
AS
BEGIN

    UPDATE PRODUCTO_TB
    SET ID_CATEGORIA_FK = @IdCategoria,
        NOMBRE          = @Nombre,
        DESCRIPCION     = @Descripcion,
        PRECIO          = @Precio,
        IMAGEN          = @Imagen,
        STOCK           = @Stock
    WHERE ID_PRODUCTO_PK = @IdProducto

END
GO

--PROCEDIMIENTO ALMACENADO PARA DESACTIVAR UN PRODUCTO
CREATE PROCEDURE SP_DesactivarProducto
    @IdProducto INT
AS
BEGIN

    UPDATE PRODUCTO_TB
    SET ID_ESTADO_FK = 2
    WHERE ID_PRODUCTO_PK = @IdProducto

END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR UN PRODUCTO POR ID (para editar)
CREATE PROCEDURE SP_ConsultarProductoPorId
    @IdProducto INT
AS
BEGIN

    SELECT
        ID_PRODUCTO_PK      AS IdProducto,
        ID_CATEGORIA_FK     AS IdCategoria,
        NOMBRE              AS Nombre,
        DESCRIPCION         AS Descripcion,
        PRECIO              AS Precio,
        IMAGEN              AS Imagen
    FROM PRODUCTO_TB
    WHERE ID_PRODUCTO_PK = @IdProducto

END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR TODOS LOS PRODUCTOS ACTIVOS (para el panel de administración)
CREATE PROCEDURE SP_ConsultarTodosLosProductos
AS
BEGIN

    SELECT
        P.ID_PRODUCTO_PK   AS IdProducto,
        P.ID_CATEGORIA_FK  AS IdCategoria,
        C.DESCRIPCION      AS Categoria,
        P.NOMBRE           AS Nombre,
        P.DESCRIPCION      AS Descripcion,
        P.PRECIO           AS Precio,
        P.IMAGEN           AS Imagen,
        P.STOCK            AS Stock
    FROM PRODUCTO_TB P
    INNER JOIN CATEGORIA_PRODUCTO_TB C ON P.ID_CATEGORIA_FK = C.ID_CATEGORIA_PK
    WHERE P.ID_ESTADO_FK = 1
    ORDER BY C.DESCRIPCION, P.NOMBRE

END
GO


--PROCEDIMIENTO ALMACENADO PARA CONSULTAR CATEGORÍAS INACTIVAS
CREATE PROCEDURE SP_ConsultarCategoriasInactivas
AS
BEGIN
    SELECT
        ID_CATEGORIA_PK     AS IdCategoria,
        DESCRIPCION         AS Descripcion
    FROM CATEGORIA_PRODUCTO_TB
    WHERE ID_ESTADO_FK = 2
    ORDER BY DESCRIPCION
END
GO

--PROCEDIMIENTO ALMACENADO PARA REACTIVAR UNA CATEGORÍA
CREATE PROCEDURE SP_ReactivarCategoria
    @IdCategoria INT
AS
BEGIN

    UPDATE CATEGORIA_PRODUCTO_TB
    SET ID_ESTADO_FK = 1
    WHERE ID_CATEGORIA_PK = @IdCategoria

END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR PRODUCTOS INACTIVOS
CREATE PROCEDURE SP_ConsultarProductosInactivos
AS
BEGIN
    SELECT
        P.ID_PRODUCTO_PK    AS IdProducto,
        P.ID_CATEGORIA_FK   AS IdCategoria,
        C.DESCRIPCION       AS Categoria,
        P.NOMBRE            AS Nombre,
        P.DESCRIPCION       AS Descripcion,
        P.PRECIO            AS Precio,
        P.IMAGEN            AS Imagen,
        P.STOCK             AS Stock
    FROM PRODUCTO_TB P
    INNER JOIN CATEGORIA_PRODUCTO_TB C ON P.ID_CATEGORIA_FK = C.ID_CATEGORIA_PK
    WHERE P.ID_ESTADO_FK = 2
    ORDER BY P.NOMBRE
END
GO

CREATE PROCEDURE SP_ReactivarProducto
    @IdProducto INT
AS
BEGIN

    UPDATE PRODUCTO_TB
    SET ID_ESTADO_FK = 1
    WHERE ID_PRODUCTO_PK = @IdProducto

END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR TIPOS DE ENTREGA
CREATE PROCEDURE SP_ConsultarTiposEntrega
AS
BEGIN
    SELECT
        ID_TIPO_ENTREGA_PK  AS IdTipoEntrega,
        DESCRIPCION         AS Descripcion
    FROM TIPO_ENTREGA_TB
    WHERE ID_ESTADO_FK = 1
    ORDER BY ID_TIPO_ENTREGA_PK
END
GO

--PROCEDIMIENTO ALMACENADO PARA REGISTRAR EL ENCABEZADO DE UN PEDIDO
CREATE PROCEDURE SP_RegistrarPedido
    @IdUsuario          INT,
    @IdTipoEntrega      INT,
    @DireccionEntrega   VARCHAR(250) = NULL,
    @Total              DECIMAL(10,2)
AS
BEGIN

    INSERT INTO PEDIDO_TB (ID_USUARIO_FK, FECHA_PEDIDO, ID_TIPO_ENTREGA_FK, DIRECCION_ENTREGA, ID_ESTADO_PEDIDO_FK, TOTAL)
    VALUES (@IdUsuario, GETDATE(), @IdTipoEntrega, @DireccionEntrega, 1, @Total)

    SELECT SCOPE_IDENTITY() AS IdPedido

END
GO

--PROCEDIMIENTO ALMACENADO PARA REGISTRAR UNA LÍNEA DE DETALLE DE PEDIDO
CREATE PROCEDURE SP_RegistrarDetallePedido
    @IdPedido           INT,
    @IdProducto         INT,
    @Cantidad           INT,
    @PrecioUnitario     DECIMAL(10,2)
AS
BEGIN

    INSERT INTO DETALLE_PEDIDO_TB (ID_PEDIDO_FK, ID_PRODUCTO_FK, CANTIDAD, PRECIO_UNITARIO, SUBTOTAL)
    VALUES (@IdPedido, @IdProducto, @Cantidad, @PrecioUnitario, @PrecioUnitario * @Cantidad)

END
GO

--PROCEDIMIENTO ALMACENADO PARA DESCONTAR STOCK DE UN PRODUCTO
CREATE PROCEDURE SP_DescontarStock
    @IdProducto     INT,
    @Cantidad       INT
AS
BEGIN

    UPDATE PRODUCTO_TB
    SET STOCK = STOCK - @Cantidad
    WHERE ID_PRODUCTO_PK = @IdProducto

END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR EL STOCK DISPONIBLE DE UN PRODUCTO
CREATE PROCEDURE SP_ConsultarStockProducto
    @IdProducto INT
AS
BEGIN

    SELECT STOCK
    FROM PRODUCTO_TB
    WHERE ID_PRODUCTO_PK = @IdProducto

END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR PEDIDOS DE UN USUARIO ESPECÍFICO
CREATE PROCEDURE SP_ConsultarPedidosPorUsuario
    @IdUsuario INT
AS
BEGIN
    SELECT
        PD.ID_PEDIDO_PK         AS IdPedido,
        PD.FECHA_PEDIDO         AS FechaPedido,
        TE.DESCRIPCION          AS TipoEntrega,
        PD.DIRECCION_ENTREGA    AS DireccionEntrega,
        EP.DESCRIPCION          AS EstadoPedido,
        PD.TOTAL                AS Total
    FROM PEDIDO_TB PD
    INNER JOIN TIPO_ENTREGA_TB TE ON PD.ID_TIPO_ENTREGA_FK = TE.ID_TIPO_ENTREGA_PK
    INNER JOIN ESTADO_PEDIDO_TB EP ON PD.ID_ESTADO_PEDIDO_FK = EP.ID_ESTADO_PEDIDO_PK
    WHERE PD.ID_USUARIO_FK = @IdUsuario
    ORDER BY PD.FECHA_PEDIDO DESC
END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR EL DETALLE DE UN PEDIDO ESPECÍFICO
CREATE PROCEDURE SP_ConsultarDetallePedido
    @IdPedido INT
AS
BEGIN
    SELECT
        DP.ID_DETALLE_PEDIDO_PK AS IdDetallePedido,
        P.NOMBRE                AS NombreProducto,
        DP.CANTIDAD             AS Cantidad,
        DP.PRECIO_UNITARIO      AS PrecioUnitario,
        DP.SUBTOTAL             AS Subtotal
    FROM DETALLE_PEDIDO_TB DP
    INNER JOIN PRODUCTO_TB P ON DP.ID_PRODUCTO_FK = P.ID_PRODUCTO_PK
    WHERE DP.ID_PEDIDO_FK = @IdPedido
END
GO

--PROCEDIMIENTO ALMACENADO PARA CONSULTAR TODOS LOS PEDIDOS (para el Administrador)
CREATE PROCEDURE SP_ConsultarTodosLosPedidos
AS
BEGIN
    SELECT
        PD.ID_PEDIDO_PK         AS IdPedido,
        P.NOMBRE_COMPLETO       AS Cliente,
        PD.FECHA_PEDIDO         AS FechaPedido,
        TE.DESCRIPCION          AS TipoEntrega,
        EP.DESCRIPCION          AS EstadoPedido,
        PD.TOTAL                AS Total
    FROM PEDIDO_TB PD
    INNER JOIN USUARIO_TB U ON PD.ID_USUARIO_FK = U.ID_USUARIO_PK
    INNER JOIN PERSONA_TB P ON U.IDENTIFICACION_FK = P.IDENTIFICACION_PK
    INNER JOIN TIPO_ENTREGA_TB TE ON PD.ID_TIPO_ENTREGA_FK = TE.ID_TIPO_ENTREGA_PK
    INNER JOIN ESTADO_PEDIDO_TB EP ON PD.ID_ESTADO_PEDIDO_FK = EP.ID_ESTADO_PEDIDO_PK
    ORDER BY PD.FECHA_PEDIDO DESC
END
GO

--PROCEDIMIENTO ALMACENADO PARA ACTUALIZAR EL ESTADO DE UN PEDIDO (uso del Administrador)
CREATE PROCEDURE SP_ActualizarEstadoPedido
    @IdPedido           INT,
    @IdEstadoPedido     INT
AS
BEGIN

    UPDATE PEDIDO_TB
    SET ID_ESTADO_PEDIDO_FK = @IdEstadoPedido
    WHERE ID_PEDIDO_PK = @IdPedido

END
GO

--PROCEDIMIENTO ALMACENADO PARA ACTUALIZAR SOLO EL PERFIL (sin contraseña)
CREATE PROCEDURE SP_ActualizarPerfil
    @Identificacion     VARCHAR(20),
    @NombreCompleto     VARCHAR(100),
    @PrimerApellido     VARCHAR(100),
    @SegundoApellido    VARCHAR(100),
    @Genero             VARCHAR(10),
    @Direccion          VARCHAR(250),
    @Nacionalidad       VARCHAR(50),
    @NumTelefono        VARCHAR(20),
    @Email              VARCHAR(100)
AS
BEGIN

    UPDATE PERSONA_TB
    SET
        NOMBRE_COMPLETO  = @NombreCompleto,
        PRIMER_APELLIDO  = @PrimerApellido,
        SEGUNDO_APELLIDO = @SegundoApellido,
        GENERO           = @Genero,
        DIRECCION        = @Direccion,
        NACIONALIDAD     = @Nacionalidad
    WHERE IDENTIFICACION_PK = @Identificacion;

    UPDATE TELEFONO_TB
    SET NUM_TELEFONO = @NumTelefono
    WHERE IDENTIFICACION_FK = @Identificacion;

    UPDATE USUARIO_TB
    SET EMAIL = @Email
    WHERE IDENTIFICACION_FK = @Identificacion;

END
GO

--PROCEDIMIENTO ALMACENADO PARA CAMBIAR SOLO LA CONTRASEÑA DESDE EL PERFIL
CREATE PROCEDURE SP_CambiarContrasenaPerfil
    @Identificacion     VARCHAR(20),
    @NuevaContrasena    VARCHAR(250)
AS
BEGIN

    UPDATE USUARIO_TB
    SET CONTRASENA = @NuevaContrasena,
        INDICADOR_CONTRASENA_TEMP = 0
    WHERE IDENTIFICACION_FK = @Identificacion

END
GO

----seguimos 1/08/2026

USE RFBakery;
GO

-- agrego las 2 columnas que pide el RF-04: puntos de esfuerzo y bandera de anticipado
ALTER TABLE PRODUCTO_TB
ADD PUNTOS_ESFUERZO   INT NOT NULL DEFAULT 0,
    PEDIDO_ANTICIPADO BIT NOT NULL DEFAULT 0;
GO

-- actualizo el SP de insertar producto para que reciba los 2 valores nuevos
ALTER PROCEDURE SP_InsertarProducto
    @IdCategoria       INT,
    @Nombre            VARCHAR(100),
    @Descripcion       VARCHAR(250),
    @Precio            DECIMAL(10,2),
    @Imagen            VARCHAR(200),
    @Stock             INT,
    @PuntosEsfuerzo    INT,
    @PedidoAnticipado  BIT
AS
BEGIN

    INSERT INTO PRODUCTO_TB (ID_CATEGORIA_FK, NOMBRE, DESCRIPCION, PRECIO, IMAGEN, STOCK, PUNTOS_ESFUERZO, PEDIDO_ANTICIPADO, ID_ESTADO_FK)
    VALUES (@IdCategoria, @Nombre, @Descripcion, @Precio, @Imagen, @Stock, @PuntosEsfuerzo, @PedidoAnticipado, 1)

END
GO

-- actualizo el SP de editar producto para que también actualice los 2 valores nuevos
ALTER PROCEDURE SP_ActualizarProducto
    @IdProducto        INT,
    @IdCategoria       INT,
    @Nombre            VARCHAR(100),
    @Descripcion       VARCHAR(250),
    @Precio            DECIMAL(10,2),
    @Imagen            VARCHAR(200),
    @Stock             INT,
    @PuntosEsfuerzo    INT,
    @PedidoAnticipado  BIT
AS
BEGIN

    UPDATE PRODUCTO_TB
    SET ID_CATEGORIA_FK   = @IdCategoria,
        NOMBRE            = @Nombre,
        DESCRIPCION       = @Descripcion,
        PRECIO            = @Precio,
        IMAGEN            = @Imagen,
        STOCK             = @Stock,
        PUNTOS_ESFUERZO   = @PuntosEsfuerzo,
        PEDIDO_ANTICIPADO = @PedidoAnticipado
    WHERE ID_PRODUCTO_PK = @IdProducto

END
GO

-- actualizo el SP de consultar producto por id para que traiga los 2 valores nuevos (se usa al editar)
ALTER PROCEDURE SP_ConsultarProductoPorId
    @IdProducto INT
AS
BEGIN

    SELECT
        ID_PRODUCTO_PK      AS IdProducto,
        ID_CATEGORIA_FK     AS IdCategoria,
        NOMBRE              AS Nombre,
        DESCRIPCION         AS Descripcion,
        PRECIO              AS Precio,
        IMAGEN              AS Imagen,
        STOCK               AS Stock,
        PUNTOS_ESFUERZO     AS PuntosEsfuerzo,
        PEDIDO_ANTICIPADO   AS PedidoAnticipado
    FROM PRODUCTO_TB
    WHERE ID_PRODUCTO_PK = @IdProducto

END
GO
---
USE RFBakery;
GO

-- tabla nueva: catalogo semanal (RF-08)
-- aqui el admin configura, para la semana en curso, que productos del catalogo maestro
-- van a estar disponibles, con cuanto stock y con que limite por persona
CREATE TABLE CATALOGO_SEMANAL_TB (
    ID_CATALOGO_SEMANAL_PK  INT             NOT NULL IDENTITY(1,1),
    ID_PRODUCTO_FK          INT             NOT NULL,
    FECHA_INICIO_SEMANA     DATE            NOT NULL,
    STOCK_DISPONIBLE        INT             NOT NULL,
    LIMITE_POR_PERSONA      INT             NOT NULL,
    ACTIVO                  BIT             NOT NULL DEFAULT 1,

    CONSTRAINT PK_CATALOGO_SEMANAL PRIMARY KEY (ID_CATALOGO_SEMANAL_PK),
    CONSTRAINT FK_CATALOGO_SEMANAL_PRODUCTO FOREIGN KEY (ID_PRODUCTO_FK)
        REFERENCES PRODUCTO_TB(ID_PRODUCTO_PK)
);
GO

-- SP para que el admin agregue un producto al catalogo de la semana (RF-08)
CREATE PROCEDURE SP_AgregarProductoCatalogoSemanal
    @IdProducto         INT,
    @FechaInicioSemana  DATE,
    @StockDisponible    INT,
    @LimitePorPersona   INT
AS
BEGIN

    INSERT INTO CATALOGO_SEMANAL_TB (ID_PRODUCTO_FK, FECHA_INICIO_SEMANA, STOCK_DISPONIBLE, LIMITE_POR_PERSONA, ACTIVO)
    VALUES (@IdProducto, @FechaInicioSemana, @StockDisponible, @LimitePorPersona, 1)

END
GO

-- SP para editar la config semanal de un producto (stock, limite, si esta visible o no)
CREATE PROCEDURE SP_ActualizarCatalogoSemanal
    @IdCatalogoSemanal  INT,
    @StockDisponible    INT,
    @LimitePorPersona   INT,
    @Activo             BIT
AS
BEGIN

    UPDATE CATALOGO_SEMANAL_TB
    SET STOCK_DISPONIBLE   = @StockDisponible,
        LIMITE_POR_PERSONA = @LimitePorPersona,
        ACTIVO             = @Activo
    WHERE ID_CATALOGO_SEMANAL_PK = @IdCatalogoSemanal

END
GO

-- SP para el admin: ver el catalogo completo de la semana (activos e inactivos, para poder gestionarlo)
CREATE PROCEDURE SP_ConsultarCatalogoSemanalAdmin
    @FechaInicioSemana  DATE
AS
BEGIN

    SELECT
        CS.ID_CATALOGO_SEMANAL_PK  AS IdCatalogoSemanal,
        P.ID_PRODUCTO_PK           AS IdProducto,
        P.NOMBRE                   AS Nombre,
        C.DESCRIPCION              AS Categoria,
        CS.STOCK_DISPONIBLE        AS StockDisponible,
        CS.LIMITE_POR_PERSONA      AS LimitePorPersona,
        CS.ACTIVO                  AS Activo
    FROM CATALOGO_SEMANAL_TB CS
    INNER JOIN PRODUCTO_TB P ON CS.ID_PRODUCTO_FK = P.ID_PRODUCTO_PK
    INNER JOIN CATEGORIA_PRODUCTO_TB C ON P.ID_CATEGORIA_FK = C.ID_CATEGORIA_PK
    WHERE CS.FECHA_INICIO_SEMANA = @FechaInicioSemana
    ORDER BY C.DESCRIPCION, P.NOMBRE

END
GO

-- SP para el cliente: solo lo que esta activo y visible del catalogo de la semana (RF-09, RF-10)
-- @IdCategoria es opcional, si llega NULL trae todas las categorias
CREATE PROCEDURE SP_ConsultarCatalogoSemanalCliente
    @FechaInicioSemana  DATE,
    @IdCategoria        INT = NULL
AS
BEGIN

    SELECT
        CS.ID_CATALOGO_SEMANAL_PK  AS IdCatalogoSemanal,
        P.ID_PRODUCTO_PK           AS IdProducto,
        P.NOMBRE                   AS Nombre,
        P.DESCRIPCION              AS Descripcion,
        P.IMAGEN                   AS Imagen,
        P.PRECIO                   AS Precio,
        P.PEDIDO_ANTICIPADO        AS PedidoAnticipado,
        C.ID_CATEGORIA_PK          AS IdCategoria,
        C.DESCRIPCION              AS Categoria,
        CS.STOCK_DISPONIBLE        AS StockDisponible,
        CS.LIMITE_POR_PERSONA      AS LimitePorPersona
    FROM CATALOGO_SEMANAL_TB CS
    INNER JOIN PRODUCTO_TB P ON CS.ID_PRODUCTO_FK = P.ID_PRODUCTO_PK
    INNER JOIN CATEGORIA_PRODUCTO_TB C ON P.ID_CATEGORIA_FK = C.ID_CATEGORIA_PK
    WHERE CS.FECHA_INICIO_SEMANA = @FechaInicioSemana
        AND CS.ACTIVO = 1
        AND CS.STOCK_DISPONIBLE > 0
        AND (@IdCategoria IS NULL OR C.ID_CATEGORIA_PK = @IdCategoria)
    ORDER BY C.DESCRIPCION, P.NOMBRE

END
GO

-- SP para saber si un producto ya esta activo en el catalogo semanal vigente
-- (lo voy a usar en el controller para bloquear editar/inactivar el producto, RF-05 y RF-06)
CREATE PROCEDURE SP_ValidarProductoEnCatalogoSemanal
    @IdProducto  INT
AS
BEGIN

    SELECT COUNT(*) AS Cantidad
    FROM CATALOGO_SEMANAL_TB
    WHERE ID_PRODUCTO_FK = @IdProducto
        AND ACTIVO = 1

END
GO

-- SP para descontar el stock semanal cuando el cliente confirma un pedido (reemplaza el stock del producto)
CREATE PROCEDURE SP_DescontarStockSemanal
    @IdCatalogoSemanal  INT,
    @Cantidad            INT
AS
BEGIN

    UPDATE CATALOGO_SEMANAL_TB
    SET STOCK_DISPONIBLE = STOCK_DISPONIBLE - @Cantidad
    WHERE ID_CATALOGO_SEMANAL_PK = @IdCatalogoSemanal

END
GO

EXEC SP_AgregarProductoCatalogoSemanal
    @IdProducto = 1,
    @FechaInicioSemana = '2026-07-27',
    @StockDisponible = 10,
    @LimitePorPersona = 3

    ---
    USE RFBakery;
GO

-- agrego los productos del 2 al 20 al catalogo semanal (el 1 ya lo tenias)
-- uso un bucle para no escribir 19 EXEC a mano
DECLARE @IdProducto INT = 2

WHILE @IdProducto <= 20
BEGIN

    EXEC SP_AgregarProductoCatalogoSemanal
        @IdProducto = @IdProducto,
        @FechaInicioSemana = '2026-07-27',
        @StockDisponible = 10,
        @LimitePorPersona = 3

    SET @IdProducto = @IdProducto + 1

END
GO

--  deberian salir 20 filas
SELECT * FROM CATALOGO_SEMANAL_TB
GO

USE RFBakery;
GO

-- SP para consultar el stock disponible de un item del catalogo semanal (no del producto maestro)
CREATE PROCEDURE SP_ConsultarStockCatalogoSemanal
    @IdCatalogoSemanal INT
AS
BEGIN

    SELECT STOCK_DISPONIBLE AS Stock
    FROM CATALOGO_SEMANAL_TB
    WHERE ID_CATALOGO_SEMANAL_PK = @IdCatalogoSemanal

END
GO

----06/08

USE RFBakery;
GO

ALTER PROCEDURE SP_ConsultarCatalogoSemanalCliente
    @FechaInicioSemana  DATE,
    @IdCategoria        INT = NULL
AS
BEGIN

    SELECT
        CS.ID_CATALOGO_SEMANAL_PK  AS IdCatalogoSemanal,
        P.ID_PRODUCTO_PK           AS IdProducto,
        P.NOMBRE                   AS Nombre,
        P.DESCRIPCION              AS Descripcion,
        P.IMAGEN                   AS Imagen,
        P.PRECIO                   AS Precio,
        P.PEDIDO_ANTICIPADO        AS PedidoAnticipado,
        P.PUNTOS_ESFUERZO          AS PuntosEsfuerzo,
        C.ID_CATEGORIA_PK          AS IdCategoria,
        C.DESCRIPCION              AS Categoria,
        CS.STOCK_DISPONIBLE        AS StockDisponible,
        CS.LIMITE_POR_PERSONA      AS LimitePorPersona
    FROM CATALOGO_SEMANAL_TB CS
    INNER JOIN PRODUCTO_TB P ON CS.ID_PRODUCTO_FK = P.ID_PRODUCTO_PK
    INNER JOIN CATEGORIA_PRODUCTO_TB C ON P.ID_CATEGORIA_FK = C.ID_CATEGORIA_PK
    WHERE CS.FECHA_INICIO_SEMANA = @FechaInicioSemana
        AND CS.ACTIVO = 1
        AND CS.STOCK_DISPONIBLE > 0
        AND (@IdCategoria IS NULL OR C.ID_CATEGORIA_PK = @IdCategoria)
    ORDER BY C.DESCRIPCION, P.NOMBRE

END
GO

-- agrego la columna para guardar cuando el cliente quiere recoger/recibir el pedido
ALTER TABLE PEDIDO_TB
ADD FECHA_ENTREGA_PROGRAMADA DATETIME NULL;
GO

-- actualizo el SP para que reciba y guarde esa fecha
ALTER PROCEDURE SP_RegistrarPedido
    @IdUsuario                  INT,
    @IdTipoEntrega               INT,
    @DireccionEntrega            VARCHAR(250) = NULL,
    @Total                       DECIMAL(10,2),
    @FechaEntregaProgramada      DATETIME
AS
BEGIN

    INSERT INTO PEDIDO_TB (ID_USUARIO_FK, FECHA_PEDIDO, ID_TIPO_ENTREGA_FK, DIRECCION_ENTREGA, ID_ESTADO_PEDIDO_FK, TOTAL, FECHA_ENTREGA_PROGRAMADA)
    VALUES (@IdUsuario, GETDATE(), @IdTipoEntrega, @DireccionEntrega, 1, @Total, @FechaEntregaProgramada)

    SELECT SCOPE_IDENTITY() AS IdPedido

END
GO
--prueba
-- le pongo 150 puntos a un producto para forzar que sea "pedido grande" al pedir 1 unidad
UPDATE PRODUCTO_TB
SET PUNTOS_ESFUERZO = 150
WHERE ID_PRODUCTO_PK = 1

-- le activo la bandera de anticipado a otro producto distinto, para probar ese caso por separado
UPDATE PRODUCTO_TB
SET PEDIDO_ANTICIPADO = 1
WHERE ID_PRODUCTO_PK = 2

USE RFBakery;
GO

-- recorto el SP para que el cliente solo pueda cambiar nombre y telefono (RF-03)
ALTER PROCEDURE SP_ActualizarPerfil
    @Identificacion     VARCHAR(20),
    @NombreCompleto     VARCHAR(100),
    @NumTelefono        VARCHAR(20)
AS
BEGIN

    UPDATE PERSONA_TB
    SET NOMBRE_COMPLETO = @NombreCompleto
    WHERE IDENTIFICACION_PK = @Identificacion;

    UPDATE TELEFONO_TB
    SET NUM_TELEFONO = @NumTelefono
    WHERE IDENTIFICACION_FK = @Identificacion;

END
GO

-- SP para saber si el usuario tiene algun pedido activo (Pendiente o En Preparacion)
-- lo uso para bloquear la edicion de perfil mientras tenga un pedido en curso (RF-03)
CREATE PROCEDURE SP_ValidarPedidoActivoUsuario
    @Identificacion  VARCHAR(20)
AS
BEGIN

    SELECT COUNT(*) AS Cantidad
    FROM PEDIDO_TB PD
    INNER JOIN USUARIO_TB U ON PD.ID_USUARIO_FK = U.ID_USUARIO_PK
    WHERE U.IDENTIFICACION_FK = @Identificacion
        AND PD.ID_ESTADO_PEDIDO_FK IN (1, 2)

END
GO
---11/08
USE RFBakery;
GO

-- tabla catalogo de generos, para que el dropdown ya no este quemado
CREATE TABLE GENERO_TB (
    ID_GENERO_PK    INT             NOT NULL IDENTITY(1,1),
    DESCRIPCION     VARCHAR(30)     NOT NULL,

    CONSTRAINT PK_GENERO PRIMARY KEY (ID_GENERO_PK),
    CONSTRAINT UQ_GENERO_DESCRIPCION UNIQUE (DESCRIPCION)
);
GO

INSERT INTO GENERO_TB (DESCRIPCION) VALUES ('Masculino');
INSERT INTO GENERO_TB (DESCRIPCION) VALUES ('Femenino');
INSERT INTO GENERO_TB (DESCRIPCION) VALUES ('Otro');
GO

-- SP para consultar los generos disponibles y llenar el dropdown
CREATE PROCEDURE SP_ConsultarGeneros
AS
BEGIN

    SELECT
        ID_GENERO_PK    AS IdGenero,
        DESCRIPCION     AS Descripcion
    FROM GENERO_TB
    ORDER BY ID_GENERO_PK

END
GO
-----procediientos para el chat
USE RFBakery;
GO

-- tabla de mensajes del chat, uno por pedido
CREATE TABLE MENSAJE_TB (
    ID_MENSAJE_PK       INT             NOT NULL IDENTITY(1,1),
    MENSAJE             VARCHAR(MAX)    NOT NULL,
    FECHA_HORA          DATETIME        NOT NULL,
    ID_USUARIO_FK       INT             NOT NULL,
    ID_PEDIDO_FK        INT             NOT NULL,

    CONSTRAINT PK_MENSAJE PRIMARY KEY (ID_MENSAJE_PK),
    CONSTRAINT FK_MENSAJE_USUARIO FOREIGN KEY (ID_USUARIO_FK)
        REFERENCES USUARIO_TB(ID_USUARIO_PK),
    CONSTRAINT FK_MENSAJE_PEDIDO FOREIGN KEY (ID_PEDIDO_FK)
        REFERENCES PEDIDO_TB(ID_PEDIDO_PK)
);
GO

-- SP para traer el historial de mensajes de un pedido
CREATE PROCEDURE SP_ConsultarMensajesPedido
    @IdPedido  INT
AS
BEGIN

    SELECT  M.ID_MENSAJE_PK      AS IdMensaje,
            M.MENSAJE            AS Mensaje,
            M.FECHA_HORA         AS FechaHora,
            M.ID_USUARIO_FK      AS IdUsuario,
            P.NOMBRE_COMPLETO    AS NombreUsuario
    FROM    MENSAJE_TB M
    INNER JOIN USUARIO_TB U ON M.ID_USUARIO_FK = U.ID_USUARIO_PK
    INNER JOIN PERSONA_TB P ON U.IDENTIFICACION_FK = P.IDENTIFICACION_PK
    WHERE   M.ID_PEDIDO_FK = @IdPedido
    ORDER BY M.FECHA_HORA

END
GO

-- SP para insertar un mensaje nuevo
CREATE PROCEDURE SP_RegistrarMensaje
    @IdUsuario  INT,
    @IdPedido   INT,
    @Mensaje    VARCHAR(MAX)
AS
BEGIN

    INSERT INTO MENSAJE_TB (MENSAJE, FECHA_HORA, ID_USUARIO_FK, ID_PEDIDO_FK)
    VALUES (@Mensaje, GETDATE(), @IdUsuario, @IdPedido)

    SELECT SCOPE_IDENTITY() AS IdMensaje

END
GO

-- SP para validar acceso a la sala del chat de un pedido
-- Cliente: solo si el pedido es suyo. Admin: siempre tiene acceso (cualquier admin puede ver cualquier pedido)
CREATE PROCEDURE SP_ValidarAccesoChatPedido
    @IdPedido     INT,
    @IdUsuario    INT,
    @IdRol        INT
AS
BEGIN

    IF @IdRol = 1
    BEGIN
        -- administrador: acceso a cualquier pedido
        SELECT 1 AS TieneAcceso
    END
    ELSE
    BEGIN
        -- cliente: solo si el pedido es suyo
        SELECT COUNT(1) AS TieneAcceso
        FROM PEDIDO_TB
        WHERE ID_PEDIDO_PK = @IdPedido
            AND ID_USUARIO_FK = @IdUsuario
    END

END
GO

-- SP para listar los pedidos que aparecen en la lista de conversaciones del chat
-- Cliente: sus propios pedidos. Admin: todos los pedidos (cualquier admin ve todo)
CREATE PROCEDURE SP_ConsultarPedidosChat
    @IdUsuario  INT,
    @IdRol      INT
AS
BEGIN

    IF @IdRol = 1
    BEGIN
        -- administrador: todos los pedidos, mostrando el nombre del cliente
        SELECT  PD.ID_PEDIDO_PK      AS IdPedido,
                P.NOMBRE_COMPLETO    AS NombreInterlocutor,
                EP.DESCRIPCION       AS EstadoPedido
        FROM    PEDIDO_TB PD
        INNER JOIN USUARIO_TB U ON PD.ID_USUARIO_FK = U.ID_USUARIO_PK
        INNER JOIN PERSONA_TB P ON U.IDENTIFICACION_FK = P.IDENTIFICACION_PK
        INNER JOIN ESTADO_PEDIDO_TB EP ON PD.ID_ESTADO_PEDIDO_FK = EP.ID_ESTADO_PEDIDO_PK
        ORDER BY PD.FECHA_PEDIDO DESC
    END
    ELSE
    BEGIN
        -- cliente: solo sus propios pedidos, mostrando "RF Bakery" como interlocutor fijo
        SELECT  PD.ID_PEDIDO_PK      AS IdPedido,
                'RF Bakery'          AS NombreInterlocutor,
                EP.DESCRIPCION       AS EstadoPedido
        FROM    PEDIDO_TB PD
        INNER JOIN ESTADO_PEDIDO_TB EP ON PD.ID_ESTADO_PEDIDO_FK = EP.ID_ESTADO_PEDIDO_PK
        WHERE   PD.ID_USUARIO_FK = @IdUsuario
        ORDER BY PD.FECHA_PEDIDO DESC
    END

END
GO
----13/08
USE RFBakery;
GO

-- SP para consultar los estados de pedido disponibles, para llenar el dropdown del admin
CREATE PROCEDURE SP_ConsultarEstadosPedido
AS
BEGIN

    SELECT
        ID_ESTADO_PEDIDO_PK    AS IdEstadoPedido,
        DESCRIPCION            AS Descripcion
    FROM ESTADO_PEDIDO_TB
    ORDER BY ID_ESTADO_PEDIDO_PK

END
GO

---19/8
USE RFBakery;
GO

-- SP para que el cliente cancele su propio pedido (RF-18)
-- solo permite cancelar si el pedido es suyo Y esta en estado "En Preparacion" (2)
CREATE PROCEDURE SP_CancelarPedidoCliente
    @IdPedido   INT,
    @IdUsuario  INT
AS
BEGIN

    UPDATE PEDIDO_TB
    SET ID_ESTADO_PEDIDO_FK = 5  -- Cancelado
    WHERE ID_PEDIDO_PK = @IdPedido
        AND ID_USUARIO_FK = @IdUsuario
        AND ID_ESTADO_PEDIDO_FK = 2  -- solo si esta En Preparacion

    SELECT @@ROWCOUNT AS FilasAfectadas

END
GO

