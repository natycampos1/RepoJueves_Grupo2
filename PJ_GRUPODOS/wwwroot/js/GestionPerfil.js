$(function () {

    $("#SeguridadForm").validate({
        rules: {
            NuevaContrasena: {
                required: true,
                minlength: 8,
                maxlength: 250
            },
            ConfirmarContrasena: {
                required: true,
                equalTo: "#nuevaContrasena"
            }
        },

        messages: {
            NuevaContrasena: {
                required: "Campo obligatorio",
                minlength: "La contraseña debe tener al menos 8 caracteres",
                maxlength: "Máximo 250 caracteres"
            },
            ConfirmarContrasena: {
                required: "Debe confirmar la nueva contraseña",
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

    $("#PerfilForm").validate({
        rules: {
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
            }
        },

        messages: {
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

    // Toggle mostrar/ocultar nueva contraseña
    $("#toggleNuevaContrasena").on("click", function () {
        var input = $("#nuevaContrasena");
        var tipo = input.attr("type") === "password" ? "text" : "password";
        input.attr("type", tipo);
        $(this).toggleClass("bi-eye bi-eye-slash");
    });

    // Toggle mostrar/ocultar confirmar contraseña
    $("#toggleConfirmarContrasena").on("click", function () {
        var input = $("#confirmarContrasena");
        var tipo = input.attr("type") === "password" ? "text" : "password";
        input.attr("type", tipo);
        $(this).toggleClass("bi-eye bi-eye-slash");
    });

});