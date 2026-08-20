$(document).ready(function () {
    new DataTable('#tblMisPedidos', {
        responsive: true,
        pageLength: 10,
        order: [],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/2.3.4/i18n/es-ES.json'
        }
    });
});

$(document).on('click', '.btn-cancelar-pedido', function () {
    var idPedido = $(this).data('idpedido');

    Swal.fire({
        title: '¿Cancelar pedido #' + idPedido + '?',
        text: 'Si ya realizaste el pago, esta acción implica la pérdida del dinero ya pagado, sin posibilidad de reembolso.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, cancelar pedido',
        cancelButtonText: 'No',
        confirmButtonColor: '#dc3545'
    }).then(function (result) {
        if (!result.isConfirmed)
            return;

        $.ajax({
            url: '/Carrito/CancelarPedido',
            method: 'POST',
            data: { idPedido: idPedido },
            dataType: 'json',
            success: function (mensaje) {
                Swal.fire({
                    title: 'Información',
                    text: mensaje,
                    icon: 'info',
                    confirmButtonText: 'Aceptar'
                }).then(function () {
                    location.reload();
                });
            }
        });
    });
});