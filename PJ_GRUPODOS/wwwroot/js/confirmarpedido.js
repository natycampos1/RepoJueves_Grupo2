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

    // logica del calendario de entrega (RF-14)
    var esPedidoGrande = $("#ConfirmarPedidoForm").data("pedido-grande") === true;
    var fechaInput = $("#fechaEntrega");
    var horaInput = $("#horaEntrega");

    function formatearFecha(fecha) {
        var anio = fecha.getFullYear();
        var mes = String(fecha.getMonth() + 1).padStart(2, "0");
        var dia = String(fecha.getDate()).padStart(2, "0");
        return anio + "-" + mes + "-" + dia;
    }

    // calculo la fecha minima permitida segun el tipo de pedido
    var hoy = new Date();
    var fechaMinima = new Date(hoy);

    if (esPedidoGrande) {
        // pedido grande: minimo 48 horas de anticipacion
        fechaMinima.setDate(fechaMinima.getDate() + 2);

        // si cae domingo, se pasa al lunes (esto tambien cubre el caso de comprar un viernes)
        if (fechaMinima.getDay() === 0) {
            fechaMinima.setDate(fechaMinima.getDate() + 1);
        }
    } else {
        // pedido pequeño: si hoy es fin de semana, la primera fecha disponible es el lunes siguiente
        if (hoy.getDay() === 0) {
            fechaMinima.setDate(fechaMinima.getDate() + 1);
        } else if (hoy.getDay() === 6) {
            fechaMinima.setDate(fechaMinima.getDate() + 2);
        }

        horaInput.attr("min", "08:00");
        horaInput.attr("max", "17:00");
        $("#horaAyuda").text("El horario de entrega es de 8:00 a.m. a 5:00 p.m.");
    }

    fechaInput.attr("min", formatearFecha(fechaMinima));

    // valido el dia de la semana elegido al cambiar la fecha
    fechaInput.on("change", function () {
        var fechaElegida = new Date(this.value + "T00:00:00");
        var diaSemana = fechaElegida.getDay();

        if (esPedidoGrande) {
            if (diaSemana === 0) {
                alert("No se puede entregar los domingos");
                this.value = "";
            }
        } else {
            if (diaSemana === 0 || diaSemana === 6) {
                alert("Los pedidos pequeños solo se pueden entregar de lunes a viernes");
                this.value = "";
            }
        }
    });

});