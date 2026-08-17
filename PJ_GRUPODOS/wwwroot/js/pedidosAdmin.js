$(document).ready(function () {
    new DataTable('#tblPedidos', {
        responsive: true,
        pageLength: 10,
        order: [],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/2.3.4/i18n/es-ES.json'
        }
    });
});