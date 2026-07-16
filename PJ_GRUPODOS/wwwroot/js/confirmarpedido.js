$(function () {

    $("#tipoEntregaSelect").on("change", function () {
        var opcionSeleccionada = this.options[this.selectedIndex];
        var descripcion = opcionSeleccionada.getAttribute("data-descripcion") || "";
        var direccionGroup = $("#direccionGroup");
        var direccionInput = $("#direccionEntrega");

        if (descripcion === "Delivery") {
            direccionGroup.removeClass("d-none");
            direccionInput.attr("required", "required");
        } else {
            direccionGroup.addClass("d-none");
            direccionInput.removeAttr("required");
            direccionInput.val("");
        }
    });

});