document.addEventListener('DOMContentLoaded', function () {
    const svgContainer = document.querySelector('.svg-container');
    const aguja = document.querySelector('.aguja-gasolina');
    const valorPlumaInput = document.getElementById('valorPluma');

    // Función para mover la pluma al valor especificado
    function moverPlumaConValor(valor) {
        // Validar que el valor esté dentro del rango
        valor = Math.max(1, Math.min(100, valor));

        const nuevaRotacion = (-60 + (118 * valor) / 100); // Rango de rotación entre -60 y 58 grados
        anime({
            targets: aguja,
            rotate: nuevaRotacion,
            duration: 1000,
            easing: 'easeInOutQuad',
        });
    }

    // Evento al cambiar el valor del input
    valorPlumaInput.addEventListener('change', function () {
        const valor = parseInt(this.value);
        moverPlumaConValor(valor);
    });

    // Funciones para aumentar y disminuir el valor de la pluma
    window.aumentarValor = function () {
        let valor = parseInt(valorPlumaInput.value);
        if (valor < 100) {
            valor += 10;
            valorPlumaInput.value = valor;
            moverPlumaConValor(valor);
        }
    };

    window.disminuirValor = function () {
        let valor = parseInt(valorPlumaInput.value);
        if (valor > 1) {
            valor -= 10;
            valorPlumaInput.value = valor;
            moverPlumaConValor(valor);
        }
    };


});

