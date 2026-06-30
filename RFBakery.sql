-- =============================================
-- BASE DE DATOS: CakeZone Pastelería
-- Tablas necesarias para el registro de usuario
-- =============================================

USE master;

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


--Procedimientos almacenados
--Consulta de tipos de identificacion
USE RFBakery;
GO
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
USE RFBakery;
GO

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

--Select para verificar que el usuario se haya registrado correctamente
USE RFBakery;
GO
SELECT
    P.IDENTIFICACION_PK             AS Identificacion,
    TI.DESCRIPCION                  AS TipoIdentificacion,
    P.NOMBRE_COMPLETO               AS NombreCompleto,
    P.PRIMER_APELLIDO               AS PrimerApellido,
    P.SEGUNDO_APELLIDO              AS SegundoApellido,
    P.GENERO                        AS Genero,
    P.DIRECCION                     AS Direccion,
    P.NACIONALIDAD                  AS Nacionalidad,
    P.FECHA_REGISTRO                AS FechaRegistro,
    T.NUM_TELEFONO                  AS NumTelefono,
    U.EMAIL                         AS Email,
    U.CONTRASENA                    AS Contrasena
FROM PERSONA_TB P
INNER JOIN TIPO_IDENTIFICACION_TB TI ON P.ID_TIPO_IDENTIFICACION_FK = TI.ID_TIPO_IDENTIFICACION_PK
INNER JOIN TELEFONO_TB T             ON P.IDENTIFICACION_PK         = T.IDENTIFICACION_FK
INNER JOIN USUARIO_TB U              ON P.IDENTIFICACION_PK         = U.IDENTIFICACION_FK;
GO

--Procedimiento almacenado para iniciar sesión de usuario (consulto usuario por email y envio los datos de interes para la variable de sesion)
CREATE PROCEDURE SP_IniciarSesion
    @Email VARCHAR(100)
AS
BEGIN
    SELECT
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

--CREACION DE TABLA PARA INSERTAR ERRORES

USE RFBakery;
GO

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

--CREACIION DE PROCEDIMIENTO ALMACENADO PARA GUARDAR DATOS DE ERROR

USE RFBakery;
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
USE RFBakery;
GO
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
USE RFBakery;
GO

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