$.validator.addMethod("extensionImagen", function (value, element) {
    if (element.files.length === 0) return true;
    var extension = element.files[0].name.split(".").pop().toLowerCase();
    return ["png", "jpg", "jpeg"].includes(extension);
}, "Solo se permiten imágenes .png, .jpg o .jpeg");

$.validator.addMethod("tamanoMaximo", function (value, element, maxMB) {
    if (element.files.length === 0) return true;
    return element.files[0].size <= maxMB * 1024 * 1024;
}, "La imagen no puede superar 2 MB");

function activarValidacionImagen(formId) {
    $(formId).validate({
        rules: {
            ImagenArchivo: {
                extensionImagen: true,
                tamanoMaximo: 2
            }
        },
        messages: {
            ImagenArchivo: {
                extensionImagen: "Solo se permiten imágenes .png, .jpg o .jpeg",
                tamanoMaximo: "La imagen no puede superar 2 MB"
            }
        },
        errorElement: "span",
        errorClass: "text-danger small d-block",
        highlight: function (element) {
            $(element).addClass("is-invalid");
        },
        unhighlight: function (element) {
            $(element).removeClass("is-invalid");
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        }
    });

    $("#ImagenArchivo").on("change", function () {
        $(formId).validate().element($(this));
    });
}

$(function () {
    if ($("#NuevoProductoForm").length) {
        activarValidacionImagen("#NuevoProductoForm");
    }
    if ($("#EditarProductoForm").length) {
        activarValidacionImagen("#EditarProductoForm");
    }
});