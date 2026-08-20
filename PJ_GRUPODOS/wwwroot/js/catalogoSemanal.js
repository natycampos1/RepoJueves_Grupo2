$(document).ready(function () {
    new DataTable('#tblCatalogoSemanal', {
        responsive: true,
        pageLength: 10,
        order: [],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/2.3.4/i18n/es-ES.json'
        }
    });

    // uso delegacion de eventos porque DataTables redibuja las filas al buscar/paginar
    $('#tblCatalogoSemanal').on('click', '.btn-guardar-catalogo', function () {
        var fila = $(this).closest('tr');

        var idCatalogoSemanal = fila.data('idcatalogosemanal');
        var stockDisponible = fila.find('.input-stock').val();
        var limitePorPersona = fila.find('.input-limite').val();
        var activo = fila.find('.select-activo').val();

        // armo el formulario dinamicamente en el momento del clic, con los valores actuales de la fila
        var form = document.createElement('form');
        form.method = 'post';
        form.action = '/AdministrarMenu/ActualizarCatalogoSemanal';

        function agregarCampo(nombre, valor) {
            var input = document.createElement('input');
            input.type = 'hidden';
            input.name = nombre;
            input.value = valor;
            form.appendChild(input);
        }

        agregarCampo('idCatalogoSemanal', idCatalogoSemanal);
        agregarCampo('stockDisponible', stockDisponible);
        agregarCampo('limitePorPersona', limitePorPersona);
        agregarCampo('activo', activo);

        document.body.appendChild(form);
        form.submit();
    });
});