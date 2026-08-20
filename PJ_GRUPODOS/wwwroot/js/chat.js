let connection = null;
let pedidoActual = null;

// ─── Conexión SignalR ──────────────────────────────────────────────────────────

function crearConexion() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl(`${urlHub}?access_token=${jwtToken}`)
        .withAutomaticReconnect()
        .build();

    connection.on('RecibirMensaje', (msg) => agregarMensaje(msg));

    connection.onreconnecting(() => {
        document.getElementById('chatEstadoConexion').textContent = 'Reconectando…';
        document.getElementById('chatEstadoConexion').className = 'text-warning';
    });

    connection.onreconnected(() => {
        document.getElementById('chatEstadoConexion').textContent = 'Conectado';
        document.getElementById('chatEstadoConexion').className = 'text-success';
        if (pedidoActual) connection.invoke('UnirseASala', pedidoActual);
    });

    return connection.start();
}

// ─── Selección de pedido ────────────────────────────────────────────────────────

document.querySelectorAll('.pedido-item').forEach(el => {
    el.addEventListener('click', () => abrirSala(
        parseInt(el.dataset.idpedido),
        el.dataset.interlocutor
    ));
});

async function abrirSala(idPedido, nombreInterlocutor) {
    document.querySelectorAll('.pedido-item').forEach(e => e.classList.remove('active'));
    document.querySelector(`.pedido-item[data-idpedido="${idPedido}"]`)
        .classList.add('active');

    pedidoActual = idPedido;

    const header = document.getElementById('chatHeader');
    header.style.removeProperty('display');
    document.getElementById('inputMensaje').disabled = false;
    document.getElementById('inputMensaje').placeholder = 'Escribe un mensaje…';
    document.getElementById('btnEnviar').disabled = false;
    document.getElementById('chatNombreInterlocutor').textContent = nombreInterlocutor;
    document.getElementById('chatEstadoConexion').textContent = 'Conectando…';
    document.getElementById('chatEstadoConexion').className = 'text-warning';

    const area = document.getElementById('areaMensajes');
    area.innerHTML = '';

    if (!connection || connection.state === signalR.HubConnectionState.Disconnected) {
        await crearConexion();
    }

    await connection.invoke('UnirseASala', idPedido);
    document.getElementById('chatEstadoConexion').textContent = 'Conectado';
    document.getElementById('chatEstadoConexion').className = 'text-success';

    await cargarHistorial(idPedido);
}

// ─── Historial vía REST ───────────────────────────────────────────────────────

async function cargarHistorial(idPedido) {
    const res = await fetch(`/Chat/ConsultarMensajes?idPedido=${idPedido}`);

    if (!res.ok) return;

    const mensajes = await res.json();
    mensajes.forEach(m => agregarMensaje(m, false));
    scrollAbajo();
}

// ─── Envío de mensajes ────────────────────────────────────────────────────────

document.getElementById('btnEnviar').addEventListener('click', enviarMensaje);

document.getElementById('inputMensaje').addEventListener('keydown', (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        enviarMensaje();
    }
});

async function enviarMensaje() {
    const input = document.getElementById('inputMensaje');
    const texto = input.value.trim();

    if (!texto || !pedidoActual || connection?.state !== signalR.HubConnectionState.Connected)
        return;

    input.value = '';
    await connection.invoke('EnviarMensaje', pedidoActual, texto);
}

// ─── Renderizado de mensajes ──────────────────────────────────────────────────

function agregarMensaje(msg, animar = true) {
    const propio = msg.idUsuario === idUsuarioActual;
    const area = document.getElementById('areaMensajes');

    const wrapper = document.createElement('div');
    wrapper.className = `d-flex mb-2 ${propio ? 'justify-content-end' : 'justify-content-start'}`;

    const hora = new Date(msg.fechaHora).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    wrapper.innerHTML = `
    <div class="chat-burbuja ${propio ? 'burbuja-propia' : 'burbuja-ajena'}">
      ${!propio ? `<div class="chat-nombre">${escapeHtml(msg.nombreUsuario)}</div>` : ''}
      <div class="chat-texto">${escapeHtml(msg.mensaje)}</div>
      <div class="chat-hora">${hora}</div>
    </div>`;

    if (animar) wrapper.classList.add('chat-nuevo');
    area.appendChild(wrapper);
    scrollAbajo();
}

function scrollAbajo() {
    const area = document.getElementById('areaMensajes');
    area.scrollTop = area.scrollHeight;
}

function escapeHtml(texto) {
    return texto
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
}