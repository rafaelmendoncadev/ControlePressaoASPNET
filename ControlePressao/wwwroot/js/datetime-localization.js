// Configuração de localização para inputs datetime-local
// Força o formato brasileiro (dd/MM/yyyy HH:mm) nos inputs datetime-local

document.addEventListener('DOMContentLoaded', function() {
    // Configurar todos os inputs datetime-local para formato brasileiro
    const datetimeInputs = document.querySelectorAll('input[type="datetime-local"]');
    
    datetimeInputs.forEach(function(input) {
        // Configurar o valor inicial se existir
        if (input.value) {
            // Converter de formato ISO para brasileiro se necessário
            const date = new Date(input.value);
            if (!isNaN(date.getTime())) {
                input.value = formatDateTimeForInput(date);
            }
        }
        
        // Adicionar evento para validação e formatação
        input.addEventListener('change', function() {
            const date = new Date(this.value);
            if (!isNaN(date.getTime())) {
                // Garantir que o valor está no formato correto
                this.value = formatDateTimeForInput(date);
            }
        });
        
        // Configurar atributos para melhor UX
        input.setAttribute('step', '60'); // Passos de 1 minuto
        input.setAttribute('pattern', '[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}');
    });
});

/**
 * Formata uma data para o formato aceito pelo input datetime-local
 * @param {Date} date - Data a ser formatada
 * @returns {string} - Data no formato YYYY-MM-DDTHH:mm
 */
function formatDateTimeForInput(date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    
    return `${year}-${month}-${day}T${hours}:${minutes}`;
}

/**
 * Converte uma string de data brasileira para Date
 * @param {string} dateString - Data no formato dd/MM/yyyy HH:mm
 * @returns {Date} - Objeto Date
 */
function parseBrazilianDateTime(dateString) {
    const parts = dateString.match(/(\d{2})\/(\d{2})\/(\d{4})\s+(\d{2}):(\d{2})/);
    if (parts) {
        const [, day, month, year, hours, minutes] = parts;
        return new Date(year, month - 1, day, hours, minutes);
    }
    return null;
}

/**
 * Formata uma data para exibição brasileira
 * @param {Date} date - Data a ser formatada
 * @returns {string} - Data no formato dd/MM/yyyy HH:mm
 */
function formatBrazilianDateTime(date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    
    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

// Configurar localização global para JavaScript
if (typeof Intl !== 'undefined') {
    // Configurar locale brasileiro como padrão
    const originalToLocaleString = Date.prototype.toLocaleString;
    Date.prototype.toLocaleString = function(locales, options) {
        return originalToLocaleString.call(this, 'pt-BR', options);
    };
    
    const originalToLocaleDateString = Date.prototype.toLocaleDateString;
    Date.prototype.toLocaleDateString = function(locales, options) {
        return originalToLocaleDateString.call(this, 'pt-BR', options);
    };
    
    const originalToLocaleTimeString = Date.prototype.toLocaleTimeString;
    Date.prototype.toLocaleTimeString = function(locales, options) {
        return originalToLocaleTimeString.call(this, 'pt-BR', options);
    };
}