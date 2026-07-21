# 🎂 RFBakery — Sistema Web de Pastelería

> Proyecto final del curso **Programación Avanzada Web (SC-701)**  
> Universidad Fidélitas · Prof. Eduardo Calvo Castillo  
> Grupo 2

---

## 🛠️ Tecnologías utilizadas

| Capa | Tecnología |
|------|-----------|
| Frontend | ASP.NET Core MVC + Razor Views |
| UI / Estilos | Bootstrap 5 + CakeZone Template (ThemeWagon) |
| Backend / API | ASP.NET Core Web API |
| ORM | Dapper |
| Base de datos | SQL Server |
| Seguridad | JWT · BCrypt.Net-Next |
| Arquitectura | MVC + Web API separados · Patrón repositorio |

---

## 📁 Estructura del repositorio

```
RFBakery/
├── WEB_GRUPODOS/          # Proyecto MVC (vistas, controllers web)
├── API_GRUPODOS/          # Proyecto Web API (lógica, Dapper, SPs)
├── SQL/
│   └── RFBakery.sql       # Script completo: creación de BD, tablas y SPs
└── README.md
```

> 💡 El script SQL se encuentra en la carpeta `/SQL`. Ejecutarlo en SQL Server Management Studio para recrear la base de datos completa.

---

## ✅ Lo implementado hasta ahora

### 🔧 Configuración y arquitectura
- Solución con dos proyectos separados: **Web MVC** y **Web API**
- Comunicación entre el proyecto Web y la API mediante `HttpClient`
- Cadena de conexión configurada en `appsettings.json`
- Inyección de dependencias con `IConfiguration`
- Manejo de excepciones por Middleware *(pendiente de afinar)*

### 🎨 Frontend
- Integración de la plantilla **CakeZone** (Bootstrap 5) como layout base
- Layout externo (`_LayoutExterno`) para las vistas de autenticación
- Vista de **Registro de usuario** completamente funcional con:
  - Validación client-side con jQuery Validate
  - Campos vinculados al modelo con `asp-for`
  - Select dinámico de tipo de identificación desde la API
  - Medidor de fortaleza de contraseña
  - Toggle de mostrar/ocultar contraseña

### 👤 Registro de usuarios
- Modelo `UsuarioRegistroModel` con Data Annotations
- Hasheo de contraseña con **BCrypt** antes de enviar a la BD
- Validación de duplicados (identificación y correo) dentro del SP
- Inserción en 3 tablas en un solo procedimiento almacenado

---

## 🗄️ Base de datos

### Tablas creadas

| Tabla | Descripción |
|-------|-------------|
| `ESTADO_TB` | Catálogo de estados (Activo / Inactivo) |
| `ROL_TB` | Catálogo de roles (Administrador / Cliente) |
| `TIPO_IDENTIFICACION_TB` | Catálogo de tipos de identificación |
| `PERSONA_TB` | Datos personales del usuario |
| `TELEFONO_TB` | Teléfonos asociados a una persona |
| `USUARIO_TB` | Credenciales de acceso al sistema |

### Tablas pendientes

| Tabla | Descripción |
|-------|-------------|
| `CATEGORIA_TB` | Categorías de productos |
| `PRODUCTO_TB` | Catálogo de productos de la pastelería |
| `CATALOGO_TB` | Catálogos disponibles |
| `DETALLE_CATALOGO_TB` | Productos por catálogo con stock |
| `CLASIFICACION_TB` | Clasificación de pedidos |
| `TIPO_ENTREGA_TB` | Tipos de entrega (delivery / retiro) |
| `PEDIDO_TB` | Pedidos realizados por usuarios |
| `DETALLE_PEDIDO_TB` | Líneas de detalle por pedido |

---

## ⚙️ Procedimientos almacenados

### Creados

| Procedimiento | Descripción |
|---------------|-------------|
| `SP_ConsultarTiposIdentificacion` | Retorna los tipos de identificación activos |
| `SP_RegistrarUsuario` | Valida duplicados e inserta en PERSONA, TELEFONO y USUARIO |

### Pendientes

| Procedimiento | Descripción |
|---------------|-------------|
| `SP_IniciarSesion` | Valida credenciales y retorna datos del usuario |
| `SP_CambiarContrasena` | Actualiza la contraseña del usuario |
| `SP_ConsultarProductos` | Lista productos activos |
| `SP_RegistrarPedido` | Inserta un pedido con su detalle |
| `SP_ConsultarPedidosPorUsuario` | Historial de pedidos de un cliente |
| *(entre otros según avance)* | |

---

## 🚀 Cómo correr el proyecto

1. Clonar el repositorio
2. Ejecutar el script `/SQL/RFBakery.sql` en SQL Server
3. Configurar la cadena de conexión en `appsettings.json` de ambos proyectos:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=RFBakery;Trusted_Connection=True;"
}
```
4. Correr primero el proyecto **API_GRUPODOS** y luego **WEB_GRUPODOS**
5. Asegurarse que la URL de la API en el proyecto Web apunte correctamente al puerto de la API

---

## 📅 Calendario de entregas

| Semana | Entrega | Valor |
|--------|---------|-------|
| Semana 5 | Anteproyecto (documento IEEE + diagrama ER) | 10% |
| Semana 10 | Avance funcional (diseño + desarrollo + BD) | 15% |
| Semana 15 | Versión final + exposición grupal | 25% |

---

## 👥 Integrantes del grupo

Jonathan Páez Padilla
XXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXX
