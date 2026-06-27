$(function () {

    $("#RegistrarUsuarioForm").validate({
        rules: {
            IdTipoIdentificacion: {
                required: true
            },
            Identificacion: {
                required: true,
                minlength: 5,
                maxlength: 20
            },
            NombreCompleto: {
                required: true,
                minlength: 2,
                maxlength: 100
            },
            PrimerApellido: {
                required: true,
                minlength: 2,
                maxlength: 100
            },
            SegundoApellido: {
                required: false,
                maxlength: 100
            },
            Genero: {
                required: false
            },
            Direccion: {
                required: false,
                maxlength: 250
            },
            Nacionalidad: {
                required: false,
                maxlength: 50
            },
            NumTelefono: {
                required: true,
                maxlength: 20
            },
            Email: {
                required: true,
                email: true,
                maxlength: 100
            },
            Contrasena: {
                required: true,
                minlength: 8,
                maxlength: 250
            },
            ConfirmarContrasena: {
                required: true,
                equalTo: "#regPassword"
            }
        },

        messages: {
            IdTipoIdentificacion: {
                required: "Debe seleccionar un tipo de identificación"
            },
            Identificacion: {
                required: "Campo obligatorio",
                minlength: "Mínimo 5 caracteres",
                maxlength: "Máximo 20 caracteres"
            },
            NombreCompleto: {
                required: "Campo obligatorio",
                minlength: "Mínimo 2 caracteres",
                maxlength: "Máximo 100 caracteres"
            },
            PrimerApellido: {
                required: "Campo obligatorio",
                minlength: "Mínimo 2 caracteres",
                maxlength: "Máximo 100 caracteres"
            },
            SegundoApellido: {
                maxlength: "Máximo 100 caracteres"
            },
            Direccion: {
                maxlength: "Máximo 250 caracteres"
            },
            Nacionalidad: {
                maxlength: "Máximo 50 caracteres"
            },
            NumTelefono: {
                required: "Campo obligatorio",
                maxlength: "Máximo 20 caracteres"
            },
            Email: {
                required: "Campo obligatorio",
                email: "Ingrese un correo electrónico válido",
                maxlength: "Máximo 100 caracteres"
            },
            Contrasena: {
                required: "Campo obligatorio",
                minlength: "La contraseña debe tener al menos 8 caracteres",
                maxlength: "Máximo 250 caracteres"
            },
            ConfirmarContrasena: {
                required: "Campo obligatorio",
                equalTo: "Las contraseñas no coinciden"
            }
        },

        errorElement: "span",

        errorPlacement: function (error, element) {
            error.addClass("text-danger small");
            element.closest(".input-wrapper").after(error);
        },

        highlight: function (element) {
            $(element).addClass("is-invalid");
        },

        unhighlight: function (element) {
            $(element).removeClass("is-invalid").addClass("is-valid");
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

});