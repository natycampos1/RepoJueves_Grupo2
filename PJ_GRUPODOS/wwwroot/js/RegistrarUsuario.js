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
    // Toggle mostrar/ocultar contraseña
    $("#toggleRegPassword").on("click", function () {
        var input = $("#regPassword");
        var tipo = input.attr("type") === "password" ? "text" : "password";
        input.attr("type", tipo);
        $(this).toggleClass("bi-eye bi-eye-slash");
    });

    // Toggle mostrar/ocultar confirmar contraseña
    $("#toggleConfirmPassword").on("click", function () {
        var input = $("#confirmPassword");
        var tipo = input.attr("type") === "password" ? "text" : "password";
        input.attr("type", tipo);
        $(this).toggleClass("bi-eye bi-eye-slash");
    });

    // Medidor de fortaleza de contraseña
    $("#regPassword").on("input", function () {
        var valor = $(this).val();
        var puntaje = 0;

        if (valor.length >= 8) puntaje++;
        if (/[A-Z]/.test(valor)) puntaje++;
        if (/[0-9]/.test(valor)) puntaje++;
        if (/[^A-Za-z0-9]/.test(valor)) puntaje++;

        var porcentaje = (puntaje / 4) * 100;
        var color = "#dc3545"; // rojo (débil)
        var texto = "Débil";

        if (puntaje === 2) {
            color = "#fd7e14"; // naranja (media)
            texto = "Media";
        } else if (puntaje === 3) {
            color = "#ffc107"; // amarillo (buena)
            texto = "Buena";
        } else if (puntaje === 4) {
            color = "#28a745"; // verde (fuerte)
            texto = "Fuerte";
        }

        if (valor.length === 0) {
            porcentaje = 0;
            texto = "Ingresa una contraseña";
            color = "#aaa";
        }

        $("#strengthFill").css({ width: porcentaje + "%", "background-color": color });
        $("#strengthText").css("color", color).text(texto);
    });

});